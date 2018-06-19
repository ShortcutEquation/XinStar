using z.AdminCenter.Entity;
using z.Logic.Base;
using z.Foundation.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using z.Foundation;
using NHibernate;
using NHibernate.Linq;

namespace z.AdminCenter.Logic
{
    /// <summary>
    /// 用户组管理
    /// </summary>
    public class AdminRoleManage : AdminRoleManageBase
    {
        /// <summary>
        /// 保存对象
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public BoolResult Save(AdminRoleExt param)
        {
            BoolResult result = new BoolResult();

            //保持对象的唯一性(保证同一分组下角色名称不重复)
            var adminGroupRoleList = Repository.Find<admin_group_role>(e => e.GroupId == param.SubGroupId);
            if(adminGroupRoleList.Count > 0 )
            {
                var roleIds = adminGroupRoleList.Select(a => a.RoleId).ToList();
                var adminRoleList = Repository.Find<admin_role>(e => roleIds.Contains(e.AdminRoleId) && e.Deleted == false);

                if (adminRoleList.Any(e => e.Name == param.Name))
                {
                    result.Succeeded = false;
                    result.Message = "已存在相同的Name值";
                    return result;
                }
            }

            admin_role _admin_role = new admin_role();
            param.CopyTo(_admin_role);

            using (ISession session = NHibernateHelper<admin_role>.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                try
                {
                    //添加角色
                    object obj = session.Save(_admin_role);
                    if (obj == null)
                    {
                        result.Succeeded = false;
                        result.Message = "保存对象失败";
                        return result;
                    }
                    //添加分组-角色的对应关系
                    admin_group_role _admin_group_role = new admin_group_role()
                    {
                        RoleId = (int)obj,
                        GroupId = param.SubGroupId
                    };
                    session.Save(_admin_group_role);

                    transaction.Commit();

                    //更新缓存
                    SetAllAdminRoles();

                    //主动更新用户会话
                    new AdminAccountManage().UpdateUserSession();

                    result.Result = obj;
                    result.Succeeded = true;
                }
                catch
                {
                    transaction.Rollback();
                }
            }

            return result;
        }

        /// <summary>
        /// 根据角色ID获取对象(从缓存中获取)
        /// </summary>
        /// <param name="param">分组ID</param>
        /// <returns></returns>
        public AdminRoleExt GetRoleData(int roleId)
        {
            AdminRoleExt adminRoleExt = null;

            admin_role _admin_role = new AdminRoleManage().GetAllAdminRoles().FirstOrDefault(e=>e.AdminRoleId == roleId);
            if (_admin_role != null)
            {
                adminRoleExt = new AdminRoleExt();
                _admin_role.CopyTo(adminRoleExt);
                adminRoleExt.UserModelsJson = new AdminUserManage().GetUserDataJson(roleId);

                IList<admin_role_permission> adminRolePermissionList = Repository.Find<admin_role_permission>(exp => exp.RoleId == _admin_role.AdminRoleId);
                StringBuilder permissionIds = new StringBuilder();
                foreach (var obj in adminRolePermissionList)
                {
                    permissionIds.AppendFormat("{0},", obj.PermissionId);
                }
                adminRoleExt.PermissionIds = permissionIds.ToString().TrimEnd(',');
            }

            return adminRoleExt;
        }

        /// <summary>
        /// 更新对象
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public BoolResult Update(AdminRoleExt param)
        {
            BoolResult result = new BoolResult();

            //保持对象的唯一性(保证同一分组下角色名称不重复)
            var _admin_role = new admin_role();
            var adminGroupRoleList = Repository.Find<admin_group_role>(e => e.GroupId == param.SubGroupId);
            if (adminGroupRoleList.Count > 0)
            {
                var roleIds = adminGroupRoleList.Select(a => a.RoleId).ToList();
                var adminRoleList = Repository.Find<admin_role>(e => roleIds.Contains(e.AdminRoleId) && e.Deleted == false);

                _admin_role = adminRoleList.FirstOrDefault(e => e.AdminRoleId == param.AdminRoleId && e.Deleted == false);
                if (_admin_role == null)
                {
                    result.Succeeded = false;
                    result.Message = "修改的对象不存在";
                    return result;
                }

                if (_admin_role.Name != param.Name)
                {
                    //保持对象的唯一性
                    if (adminRoleList.Any(e => e.Name == param.Name && e.Deleted == false))
                    {
                        result.Succeeded = false;
                        result.Message = "已存在相同的Name值";
                        return result;
                    }
                }
            }
            else
            {
                result.Succeeded = false;
                result.Message = "该管理组下不存在任何角色";
                return result;
            }

            using (ISession session = NHibernateHelper<admin_role>.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                try
                {
                    _admin_role.Name = param.Name;
                    _admin_role.Description = param.Description;
                    _admin_role.UpdateBy = param.UpdateBy;
                    _admin_role.UpdateOn = param.UpdateOn;
                    session.Update(_admin_role);
                  
                    transaction.Commit();

                    //更新缓存
                    SetAllAdminRoles();

                    //主动更新用户会话
                    new AdminAccountManage().UpdateUserSession();

                    result.Succeeded = true;
                }
                catch
                {
                    transaction.Rollback();
                }
            }

            return result;
        }

        /// <summary>
        /// 更新权限
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public BoolResult UpdatePermission(AdminRoleExt param)
        {
            BoolResult result = new BoolResult();

            admin_role _admin_role = Repository.First<admin_role>(e => e.AdminRoleId == param.AdminRoleId && e.Deleted == false);
            if (_admin_role == null)
            {
                result.Succeeded = false;
                result.Message = "修改的对象不存在";
                return result;
            }

            using (ISession session = NHibernateHelper<admin_role_permission>.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                try
                {
                    IList<admin_role_permission> adminRolePermissionList = Repository.Find<admin_role_permission>(e => e.RoleId == _admin_role.AdminRoleId);

                    #region 当角色更新，比较该组包含的新、旧权限集合，新增需要新增的权限，删除需要删除的权限

                    #region 取出两个数组的差集(交换算法)

                    int[] oldPermissionIdArray = adminRolePermissionList.Select(exp => exp.PermissionId).Distinct().ToArray();
                    string[] permissionIds = param.PermissionIds.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Distinct().ToArray();
                    int[] newPermissionIdArray = Array.ConvertAll<string, int>(permissionIds, delegate (string s) { return int.Parse(s); });

                    int oldPermissionIdArraySize = oldPermissionIdArray.Length;
                    int newPermissionIdArraySize = newPermissionIdArray.Length;
                    int end = oldPermissionIdArraySize;
                    bool swap = false;

                    for (int i = 0; i < end;)
                    {
                        swap = false;
                        for (int j = i; j < newPermissionIdArraySize; j++)
                        {
                            if (oldPermissionIdArray[i] == newPermissionIdArray[j])
                            {
                                int tmp = newPermissionIdArray[i];
                                newPermissionIdArray[i] = newPermissionIdArray[j];
                                newPermissionIdArray[j] = tmp;
                                swap = true;
                                break;
                            }
                        }
                        if (swap != true)
                        {
                            int tmp = oldPermissionIdArray[i];
                            oldPermissionIdArray[i] = oldPermissionIdArray[--end];
                            oldPermissionIdArray[end] = tmp;
                        }
                        else
                        {
                            i++;
                        }
                    }

                    #endregion

                    //需删除的
                    List<int> deleteRolePermissionIdArray = new List<int>();
                    for (int i = end; i < oldPermissionIdArraySize; i++)
                    {
                        deleteRolePermissionIdArray.Add(oldPermissionIdArray[i]);
                    }

                    //需添加的
                    List<int> addRolePermissionIdArray = new List<int>();
                    for (int i = end; i < newPermissionIdArraySize; i++)
                    {
                        addRolePermissionIdArray.Add(newPermissionIdArray[i]);
                    }

                    //执行删除
                    var deleteRolePermissionObj = from obj in adminRolePermissionList
                                                   where deleteRolePermissionIdArray.Contains(obj.PermissionId)
                                                   select obj;
                    foreach (var obj in deleteRolePermissionObj)
                    {
                        session.Delete(obj);
                    }

                    //执行添加
                    foreach (var obj in addRolePermissionIdArray)
                    {
                        admin_role_permission _admin_role_permission = new admin_role_permission();
                        _admin_role_permission.RoleId = _admin_role.AdminRoleId;
                        _admin_role_permission.PermissionId = obj;
                        session.Save(_admin_role_permission);
                    }

                    #endregion

                    transaction.Commit();

                    //主动更新用户会话
                    new AdminAccountManage().UpdateUserSession();

                    result.Succeeded = true;
                }
                catch
                {
                    transaction.Rollback();
                }
            }

            return result;
        }

        /// <summary>
        /// 单个、批量禁用/启用对象
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public BoolResult Disabled(AdminRoleExt param)
        {
            BoolResult result = new BoolResult();
            IList<admin_role> updateRoleList = new List<admin_role>();

            IList<admin_role> adminRoleList = Repository.Find<admin_role>(e => param.AdminRoleIds.Contains(e.AdminRoleId) && e.Disabled != param.Disabled && e.Deleted == false);
            if (adminRoleList.Count == 0)
            {
                result.Succeeded = false;
                result.Message = string.Format("{0}的对象不存在", param.Disabled ? "禁用" : "启用");
                return result;
            }
            else if (param.Disabled == false)//启用
            {
                //找出角色所属已经禁用的分组
                var roleIds = adminRoleList.Select(e => e.AdminRoleId).ToList();
                var adminGroupRoles = Repository.Find<admin_group_role>(e => roleIds.Contains(e.RoleId)).ToList();
                var adminGroupIds = adminGroupRoles.Select(e=>e.GroupId).Distinct();
                var adminGroupList = Repository.Find<admin_group>(e => adminGroupIds.Contains(e.AdminGroupId) && e.Deleted == false && e.Disabled == true);

                foreach (var _admin_role in adminRoleList)
                {
                    if (Repository.Exists<admin_role>(e => e.AdminRoleId == _admin_role.ParentId && e.Disabled == true && e.Deleted == false) 
                        ||
                        (adminGroupList.Count > 0 && adminGroupRoles.Any(e=>e.RoleId == _admin_role.AdminRoleId && adminGroupList.Select(a=>a.AdminGroupId).Contains(e.GroupId))))
                    {
                        continue;
                    }
                    updateRoleList.Add(_admin_role);
                }
            }
            else
            {
                updateRoleList = adminRoleList;
            }

            if (updateRoleList.Count == 0)
            {
                result.Succeeded = false;
                result.Message = string.Format("没有可{0}的对象", param.Disabled ? "禁用" : "启用");
                return result;
            }
            else
            {
                using (ISession session = NHibernateHelper<admin_role>.OpenSession())
                using (ITransaction transaction = session.BeginTransaction())
                {
                    try
                    {
                        int intRecord = 0;
                        foreach (var _admin_role in updateRoleList)
                        {
                            //启用、禁用该角色（所属组或上级角色禁用时，不得启用）
                            _admin_role.Disabled = param.Disabled;
                            _admin_role.UpdateBy = param.UpdateBy;
                            _admin_role.UpdateOn = param.UpdateOn;
                            session.Update(_admin_role);

                            intRecord++;
                        }

                        transaction.Commit();
                        //更新缓存
                        SetAllAdminRoles();
                        //主动更新用户会话
                        new AdminAccountManage().UpdateUserSession();

                        result.Succeeded = true;
                        result.Result = intRecord;
                    }
                    catch
                    {
                        transaction.Rollback();
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// 单个、批量删除对象
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public BoolResult Deleted(AdminRoleExt param)
        {
            BoolResult result = new BoolResult();

            IList<admin_role> adminRoleList = Repository.Find<admin_role>(e => param.AdminRoleIds.Contains(e.AdminRoleId) && e.Deleted == false);
            if (adminRoleList.Count == 0)
            {
                result.Succeeded = false;
                result.Message = "删除的对象不存在";
                return result;
            }
            else
            {
                using (ISession session = NHibernateHelper<admin_role>.OpenSession())
                using (ITransaction transaction = session.BeginTransaction())
                {
                    try
                    {
                        int intRecord = 0;
                        foreach (var _admin_role in adminRoleList)
                        {
                            var delRoleIds = new List<int>();

                            //删除角色以及其子角色
                            _admin_role.Deleted = true;
                            _admin_role.UpdateBy = param.UpdateBy;
                            _admin_role.UpdateOn = param.UpdateOn;
                            session.Update(_admin_role);
                            delRoleIds.Add(_admin_role.AdminRoleId);

                            IList<admin_role> adminRoleLineTreeList = GetAdminRoleList(_admin_role.AdminRoleId);
                            foreach (var adminRole in adminRoleLineTreeList)
                            {
                                adminRole.Deleted = true;
                                adminRole.UpdateBy = param.UpdateBy;
                                adminRole.UpdateOn = param.UpdateOn;
                                session.Update(adminRole);
                            }

                            if (adminRoleLineTreeList.Count > 0)
                            {
                                delRoleIds.AddRange(adminRoleLineTreeList.Select(e => e.AdminRoleId));
                            }

                            //删除角色—权限
                            IList<admin_role_permission> adminRolePermissionList = Repository.Find<admin_role_permission>(e => delRoleIds.Contains(e.RoleId));
                            foreach (var _admin_role_permission in adminRolePermissionList)
                            {
                                session.Delete(_admin_role_permission);
                            }
                            //删除角色—用户
                            IList<admin_role_user> adminRoleUserList = Repository.Find<admin_role_user>(e => delRoleIds.Contains(e.RoleId));
                            foreach (var _admin_role_user in adminRoleUserList)
                            {
                                session.Delete(_admin_role_user);
                            }
                            //删除分组—角色
                            IList<admin_group_role> adminGroupRoleList = Repository.Find<admin_group_role>(e => e.RoleId == _admin_role.AdminRoleId);
                            foreach (var _admin_group_role in adminGroupRoleList)
                            {
                                session.Delete(_admin_group_role);
                            }

                            intRecord++;
                        }

                        transaction.Commit();

                        //更新缓存
                        SetAllAdminRoles();
                        new AdminUserManage().SetAllAdminRoleUsers();
                       
                        //主动更新用户会话
                        new AdminAccountManage().UpdateUserSession();

                        result.Succeeded = true;
                        result.Result = intRecord;
                    }
                    catch
                    {
                        transaction.Rollback();
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// 获取指定组织下角色列表（支持ZTree的JSON结构----用于添加/更新角色）
        /// </summary>
        /// <returns></returns>
        public string GetRolePartZTreeJson(int groupId)
        {
            StringBuilder builder = new StringBuilder();

            IList<admin_group_role> groupRoleList = GetAllAdminGroupRoles().Where(e => e.GroupId == groupId).ToList();
            if (groupRoleList.Count == 0)
            {
                return "";
            }

            List<int> roleIds = groupRoleList.Select(e => e.RoleId).ToList();
            IList<admin_role> adminRoleList = GetAllAdminRoles().Where(e => roleIds.Contains(e.AdminRoleId) && e.Deleted == false && e.Disabled == false).OrderBy(e => e.SortNo).ThenBy(e => e.AdminRoleId).ToList();
            List<AdminRoleTree> adminRoleTreeList = GetAdminRoleTree(adminRoleList, -1);

            string strZTreeJson = GetAdminRoleZTreeJson(adminRoleTreeList,groupId);
            builder.Append(strZTreeJson);
            string strJson = builder.ToString();
            strJson = strJson.Trim().TrimEnd(',');
            strJson = string.Format("[{0}]", strJson);

            return strJson;
        }


        /// <summary>
        /// 移动分组
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public BoolResult MoveRoleNode(AdminNodeMoveParam param)
        {
            BoolResult result = new BoolResult();

            admin_role _admin_role = Repository.First<admin_role>(e => e.AdminRoleId == param.NodeId && e.Deleted == false);
            if (_admin_role == null)
            {
                result.Succeeded = false;
                result.Message = "[移动的对象不存在]";
                return result;
            }

            using (ISession session = NHibernateHelper<admin_group>.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                try
                {
                    _admin_role.ParentId = param.TargetNodeId;
                    _admin_role.UpdateBy = param.UpdateBy;
                    _admin_role.UpdateOn = DateTime.Now;
                    session.Update(_admin_role);

                    transaction.Commit();

                    //刷新缓存
                    SetAllAdminRoles();

                    //主动更新用户会话
                    new AdminAccountManage().UpdateUserSession();

                    result.Succeeded = true;
                }
                catch
                {
                    transaction.Rollback();
                }
            }

            return result;
        }
    }
}
