using z.AdminCenter.Entity;
using z.Logic.Base;
using z.Foundation;
using z.Foundation.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using z.Foundation.Cache;

namespace z.AdminCenter.Logic
{
    /// <summary>
    /// 权限管理
    /// </summary>
    public class AdminPermissionManage : AdminPermissionManageBase
    {
        /// <summary>
        /// 获取对象
        /// </summary>
        /// <param name="param">主键ID</param>
        /// <returns></returns>
        public admin_permission Get(object param)
        {
            return Repository.Get<admin_permission>(param);
        }

        /// <summary>
        /// 保存对象
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public BoolResult Save(admin_permission param)
        {
            BoolResult result = new BoolResult();

            try
            {
                //确保权限关联的系统有效存在
                if (!Repository.Exists<admin_system>(e => e.AdminSystemId == param.SystemId && e.Deleted == false))
                {
                    result.Succeeded = false;
                    result.Message = "关联的系统不存在";
                    return result;
                }

                //创建非顶级权限时判断父级权限是否有效存在、父级权限所属系统是否与当前选择系统一致
                //创建顶级权限时需要设置顶级权限图标
                if (param.ParentId > -1)
                {
                    admin_permission _admin_permission = Repository.Get<admin_permission>(param.ParentId);
                    if (_admin_permission.SystemId != param.SystemId)
                    {
                        result.Succeeded = false;
                        result.Message = "父级权限所属系统与当前选择系统不一致";
                        return result;
                    }
                    else if (_admin_permission.Disabled)
                    {
                        result.Succeeded = false;
                        result.Message = "关联的父级权限已被禁用";
                        return result;
                    }
                    else if (_admin_permission.Deleted)
                    {
                        result.Succeeded = false;
                        result.Message = "关联的父级权限不存在";
                        return result;
                    }
                }

                //保持对象的唯一性
                if (Repository.Exists<admin_permission>(e => (e.PermissionCode == param.PermissionCode) && e.Deleted == false))
                {
                    result.Succeeded = false;
                    result.Message = "已存在相同的PermissionCode值";
                    return result;
                }

                //获取SortNo
                int intMaxSortNo = 0;
                IList<admin_permission> adminPermissions = Repository.Find<admin_permission>(e => e.ParentId == param.ParentId && e.Deleted == false);
                if (adminPermissions.Count > 0)
                {
                    intMaxSortNo = adminPermissions.Max(e => e.SortNo);
                }
                param.SortNo = intMaxSortNo + 1;

                object obj = Repository.Save<admin_permission>(param);
                if (obj == null)
                {
                    result.Succeeded = false;
                    result.Message = "保存对象失败";
                    return result;
                }
                else
                {
                    result.Succeeded = true;

                    //更新缓存
                    SetAllAdminPermissions();
                    //主动更新用户会话
                    new AdminAccountManage().UpdateUserSession();
                }
            }
            catch
            { }

            return result;
        }

        /// <summary>
        /// 更新对象
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public BoolResult Update(admin_permission param)
        {
            BoolResult result = new BoolResult();

            try
            {
                admin_permission _admin_permission = Repository.First<admin_permission>(e => e.AdminPermissionId == param.AdminPermissionId && e.Deleted == false);
                if (_admin_permission == null)
                {
                    result.Succeeded = false;
                    result.Message = "修改的对象不存在";
                    return result;
                }

                //确保权限关联的系统有效存在
                if (!Repository.Exists<admin_system>(e => e.AdminSystemId == param.SystemId && e.Deleted == false))
                {
                    result.Succeeded = false;
                    result.Message = "关联的系统不存在";
                    return result;
                }

                //创建非顶级权限时判断父级权限是否有效存在
                if (param.ParentId > -1)
                {
                    if (param.ParentId == param.AdminPermissionId)
                    {
                        result.Succeeded = false;
                        result.Message = "父级权限不能设置为自己";
                        return result;
                    }
                    admin_permission adminPermission = Repository.Get<admin_permission>(param.ParentId);
                    if (adminPermission.SystemId != param.SystemId)
                    {
                        result.Succeeded = false;
                        result.Message = "父级权限所属系统与当前选择系统不一致";
                        return result;
                    }
                    else if (adminPermission.Disabled)
                    {
                        result.Succeeded = false;
                        result.Message = "关联的父级权限已被禁用";
                        return result;
                    }
                    else if (adminPermission.Deleted)
                    {
                        result.Succeeded = false;
                        result.Message = "关联的父级权限不存在";
                        return result;
                    }
                }

                //保持对象的唯一性
                if (_admin_permission.PermissionCode != param.PermissionCode)
                {
                    if (Repository.Exists<admin_permission>(e => e.PermissionCode == param.PermissionCode && e.Deleted == false))
                    {
                        result.Succeeded = false;
                        result.Message = "已存在相同的PermissionCode值";
                        return result;
                    }
                }

                _admin_permission.SystemId = param.SystemId;
                _admin_permission.ParentId = param.ParentId;
                _admin_permission.PermissionCode = param.PermissionCode;
                _admin_permission.Name = param.Name;
                _admin_permission.Description = param.Description;
                _admin_permission.Img = param.Img;
                _admin_permission.IsMenu = param.IsMenu;
                _admin_permission.IsLink = param.IsLink;
                _admin_permission.Url = param.Url;
                _admin_permission.Target = param.Target;
                _admin_permission.UpdateBy = param.UpdateBy;
                _admin_permission.UpdateOn = param.UpdateOn;
                Repository.Update<admin_permission>(_admin_permission);

                //更新缓存
                SetAllAdminPermissions();
                //主动更新用户会话
                new AdminAccountManage().UpdateUserSession();

                result.Succeeded = true;
            }
            catch
            { }

            return result;
        }

        /// <summary>
        /// 单个、批量禁用/启用对象
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public BoolResult Disabled(AdminPermissionExt param)
        {
            BoolResult result = new BoolResult();

            try
            {
                IList<admin_permission> adminPermissionList = Repository.Find<admin_permission>(e => param.AdminPermissionIds.Contains(e.AdminPermissionId) && e.Disabled != param.Disabled && e.Deleted == false);
                if (adminPermissionList.Count == 0)
                {
                    result.Succeeded = false;
                    result.Message = string.Format("{0}的对象不存在", param.Disabled ? "禁用" : "启用");
                    return result;
                }
                else
                {
                    using (ISession session = NHibernateHelper<admin_permission>.OpenSession())
                    using (ITransaction transaction = session.BeginTransaction())
                    {
                        try
                        {
                            int intRecord = 0;
                            foreach (var _admin_permission in adminPermissionList)
                            {
                                //禁用、启用权限时不要对其子权限做任何操作，若父级权限禁用、启用，展示时需控制子级权限是否显示，而非改变子级权限的Disabled状态

                                _admin_permission.Disabled = param.Disabled;
                                _admin_permission.UpdateBy = param.UpdateBy;
                                _admin_permission.UpdateOn = param.UpdateOn;
                                session.Update(_admin_permission);

                                intRecord++;
                            }

                            transaction.Commit();

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
            }
            catch
            { }

            return result;
        }

        /// <summary>
        /// 单个、批量删除对象
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public BoolResult Deleted(AdminPermissionExt param)
        {
            BoolResult result = new BoolResult();

            try
            {
                IList<admin_permission> adminPermissionList = Repository.Find<admin_permission>(e => param.AdminPermissionIds.Contains(e.AdminPermissionId));
                if (adminPermissionList.Count == 0)
                {
                    result.Succeeded = false;
                    result.Message = "删除的对象不存在";
                    return result;
                }
                else
                {
                    using (ISession session = NHibernateHelper<admin_permission>.OpenSession())
                    using (ITransaction transaction = session.BeginTransaction())
                    {
                        try
                        {
                            int intRecord = 0;
                            foreach (var _admin_permission in adminPermissionList)
                            {
                                //删除权限时需要删除权限下的子权限

                                _admin_permission.Deleted = true;
                                _admin_permission.UpdateBy = param.UpdateBy;
                                _admin_permission.UpdateOn = param.UpdateOn;
                                session.Update(_admin_permission);

                                IList<admin_permission> adminPermissionLineTreeList = GetAdminPermissionList(_admin_permission.AdminPermissionId);
                                foreach (var adminPermission in adminPermissionLineTreeList)
                                {
                                    adminPermission.Deleted = true;
                                    adminPermission.UpdateBy = param.UpdateBy;
                                    adminPermission.UpdateOn = param.UpdateOn;
                                    session.Update(adminPermission);
                                }

                                //删除权限与用户的关系
                                IList<admin_user_permission> adminUserPermissionList = Repository.Find<admin_user_permission>(e => e.PermissionId == _admin_permission.AdminPermissionId);
                                foreach (var _admin_user_permission in adminUserPermissionList)
                                {
                                    session.Delete(_admin_user_permission);
                                }

                                //删除权限与角色的关系
                                IList<admin_role_permission> adminRolePermissionList = Repository.Find<admin_role_permission>(e => e.PermissionId == _admin_permission.AdminPermissionId);
                                foreach (var _admin_role_permission in adminRolePermissionList)
                                {
                                    session.Delete(_admin_role_permission);
                                }

                                //删除权限与用户组的关系
                                IList<admin_group_permission> adminGroupPermissionList = Repository.Find<admin_group_permission>(e => e.PermissionId == _admin_permission.AdminPermissionId);
                                foreach (var _admin_group_permission in adminGroupPermissionList)
                                {
                                    session.Delete(_admin_group_permission);
                                }

                                intRecord++;
                            }

                            transaction.Commit();

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
            }
            catch
            { }

            return result;
        }

        /// <summary>
        /// 排序
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public BoolResult Sort(PermissionSort param)
        {
            BoolResult result = new BoolResult();

            try
            {
                if (param.DragNode.AdminPermissionId == param.TargetNode.AdminPermissionId)
                {
                    return result;
                }

                List<admin_permission> dragGroup = Repository.Find<admin_permission>(e => e.AdminPermissionId == param.DragNode.AdminPermissionId || e.AdminPermissionId == param.TargetNode.AdminPermissionId).ToList();
                if (dragGroup.Count != 2)
                {
                    return result;
                }

                admin_permission dragAdminPermission = dragGroup.First(e => e.AdminPermissionId == param.DragNode.AdminPermissionId);
                admin_permission targetAdminPermission = dragGroup.First(e => e.AdminPermissionId == param.TargetNode.AdminPermissionId);
                if (dragAdminPermission.ParentId != targetAdminPermission.ParentId)
                {
                    return result;
                }

                #region  使用最短距离算法对节点进行重新排序

                IList<admin_permission> updateAdminPermissionList = new List<admin_permission>();

                //取出与当前移动节点、目标节点属于同一层级的兄弟节点（包含它们自已）
                List<admin_permission> theSameLevelAdminPermissions = Repository.Find<admin_permission>(e => e.ParentId == dragAdminPermission.ParentId && e.Deleted == false).OrderBy(e => e.SortNo).ThenBy(e => e.AdminPermissionId).ToList();

                int intDragIndex = theSameLevelAdminPermissions.FindIndex(e => e.AdminPermissionId == dragAdminPermission.AdminPermissionId);
                int intTargetIndex = theSameLevelAdminPermissions.FindIndex(e => e.AdminPermissionId == targetAdminPermission.AdminPermissionId);

                int d_t = System.Math.Abs(intTargetIndex - intDragIndex);//Drag-Target
                int t_t = intTargetIndex;//Target-Top
                int t_b = theSameLevelAdminPermissions.Count - intTargetIndex;//Target-Bottom

                if (d_t <= t_t || d_t >= t_b)
                {
                    if (param.Next)
                    {
                        //下移内部更新
                        for (int i = intTargetIndex; i > intDragIndex; i--)
                        {
                            admin_permission _admin_permission = theSameLevelAdminPermissions[i];

                            _admin_permission.SortNo--;
                            _admin_permission.UpdateBy = param.DragNode.UpdateBy;
                            _admin_permission.UpdateOn = param.DragNode.UpdateOn;

                            updateAdminPermissionList.Add(_admin_permission);
                        }
                    }
                    else
                    {
                        //上移内部更新
                        for (int i = intTargetIndex; i < intDragIndex; i++)
                        {
                            admin_permission _admin_permission = theSameLevelAdminPermissions[i];

                            _admin_permission.SortNo++;
                            _admin_permission.UpdateBy = param.DragNode.UpdateBy;
                            _admin_permission.UpdateOn = param.DragNode.UpdateOn;

                            updateAdminPermissionList.Add(_admin_permission);
                        }
                    }

                    dragAdminPermission.SortNo = targetAdminPermission.SortNo;
                    dragAdminPermission.UpdateBy = param.DragNode.UpdateBy;
                    dragAdminPermission.UpdateOn = param.DragNode.UpdateOn;

                    updateAdminPermissionList.Add(dragAdminPermission);
                }
                else if (t_t >= t_b)
                {
                    //下移更新下层
                    for (int i = intTargetIndex + 1; i < theSameLevelAdminPermissions.Count; i++)
                    {
                        admin_permission _admin_permission = theSameLevelAdminPermissions[i];

                        _admin_permission.SortNo++;
                        _admin_permission.UpdateBy = param.DragNode.UpdateBy;
                        _admin_permission.UpdateOn = param.DragNode.UpdateOn;

                        updateAdminPermissionList.Add(_admin_permission);
                    }

                    dragAdminPermission.SortNo = targetAdminPermission.SortNo + 1;
                    dragAdminPermission.UpdateBy = param.DragNode.UpdateBy;
                    dragAdminPermission.UpdateOn = param.DragNode.UpdateOn;

                    updateAdminPermissionList.Add(dragAdminPermission);
                }
                else
                {
                    //上移更新上层
                    for (int i = intTargetIndex - 1; i > -1; i--)
                    {
                        admin_permission _admin_permission = theSameLevelAdminPermissions[i];

                        _admin_permission.SortNo--;
                        _admin_permission.UpdateBy = param.DragNode.UpdateBy;
                        _admin_permission.UpdateOn = param.DragNode.UpdateOn;

                        updateAdminPermissionList.Add(_admin_permission);
                    }

                    dragAdminPermission.SortNo = targetAdminPermission.SortNo - 1;
                    dragAdminPermission.UpdateBy = param.DragNode.UpdateBy;
                    dragAdminPermission.UpdateOn = param.DragNode.UpdateOn;

                    updateAdminPermissionList.Add(dragAdminPermission);
                }

                #endregion

                using (ISession session = NHibernateHelper<admin_permission>.OpenSession())
                using (ITransaction transaction = session.BeginTransaction())
                {
                    try
                    {
                        foreach (var obj in updateAdminPermissionList)
                        {
                            session.Update(obj);
                        }

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
            }
            catch
            { }

            return result;
        }

        /// <summary>
        /// 分页获取列表（提供按SystemId、ParentId全字匹配搜索；按PermissionCode、Name进行模糊搜索，搜索时给SystemId、ParentId、PermissionCode、Name属性赋值，提供客户端自定义排序，默认按创建时间倒序排列）
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public IPagedList<admin_permission> GetPageList(IPagedParam<admin_permission> param)
        {
            IQueryable<admin_permission> queryable = Repository.AsQueryable<admin_permission>().Where(e => e.Deleted == false);
            if (!string.IsNullOrEmpty(param.model.Name))
            {
                queryable = queryable.Where(e => e.Name.Contains(param.model.Name) || e.PermissionCode.Contains(param.model.Name));
            }
            if (param.model.SystemId > 0)
            {
                queryable = queryable.Where(e => e.SystemId == param.model.SystemId);
            }
            if (param.model.AdminPermissionId > 0)
            {
                IList<admin_permission> adminPermissions = GetAdminPermissionList(param.model.AdminPermissionId);
                List<int> adminPermissionIds = adminPermissions.Select(e => e.AdminPermissionId).ToList();
                adminPermissionIds.Add(param.model.AdminPermissionId);
                queryable = queryable.Where(e => adminPermissionIds.Contains(e.AdminPermissionId));
            }
            queryable = queryable.OrderByDescending(e => e.AdminPermissionId);

            return new PagedList<admin_permission>(queryable, param.PageIndex, param.PageSize);
        }

        /// <summary>
        /// 获取指定系统对应包含的所有权限列表（支持ZTree的JSON结构----用于添加/更新权限页面，选择系统时列出该系统下包含的权限树）
        /// </summary>
        /// <param name="intSystemId"></param>
        /// <returns></returns>
        public string GetSpecifySystemPermissionZTreeJson(int intSystemId)
        {
            IList<admin_permission> adminPermissionList = Repository.Find<admin_permission>(e => e.Deleted == false && e.Disabled == false && e.SystemId == intSystemId).OrderBy(e => e.SortNo).ThenBy(e => e.AdminPermissionId).ToList();
            List<AdminPermissionTree> adminPermissionTreeList = GetAdminPermissionTree(adminPermissionList, -1);

            string strZTreeJson = GetAdminPermissionZTreeJson(adminPermissionTreeList);

            strZTreeJson = strZTreeJson.Trim().TrimEnd(',');
            strZTreeJson = string.Format("[{0}]", strZTreeJson);

            return strZTreeJson;
        }

        /// <summary>
        /// 获取各个系统包含的所有权限列表（支持ZTree的JSON结构----用于添加/更新用户组、添加/更新用户页面，供用户勾选组或用户对应的权限）
        /// </summary>
        /// <returns></returns>
        public string GetAllSystemPermissionZTreeJson()
        {
            StringBuilder builder = new StringBuilder();

            IList<admin_system> adminSystemList = (new AdminSystemManage()).GetList();
            IList<admin_permission> adminPermissionList = Repository.Find<admin_permission>(e => e.Deleted == false && e.Disabled == false).OrderBy(e => e.SortNo).ThenBy(e => e.AdminPermissionId).ToList();

            foreach (var item in adminSystemList)
            {
                IList<admin_permission> adminPermissionListBySystem = adminPermissionList.Where(e => e.SystemId == item.AdminSystemId).ToList();
                List<AdminPermissionTree> adminPermissionTreeList = GetAdminPermissionTree(adminPermissionListBySystem, -1);

                string strZTreeJson = GetAdminPermissionZTreeJson(adminPermissionTreeList);

                if (!string.IsNullOrEmpty(strZTreeJson))
                {
                    strZTreeJson = "{ id: \"-1\", name: \"" + item.Name + "\", code: \"\", nocheck: true, children: [" + strZTreeJson + "] }, ";
                    builder.Append(strZTreeJson);
                }
            }

            string strJson = builder.ToString();
            strJson = strJson.Trim().TrimEnd(',');
            strJson = string.Format("[{0}]", strJson);

            return strJson;
        }

        /// <summary>
        /// 获取用于排序的所有系统权限列表
        /// </summary>
        /// <returns></returns>
        public string GetSortPermissionZTreeJson()
        {
            StringBuilder builder = new StringBuilder();

            IList<admin_system> adminSystemList = (new AdminSystemManage()).GetList();
            IList<admin_permission> adminPermissionList = Repository.Find<admin_permission>(e => e.Deleted == false && e.Disabled == false).OrderBy(e => e.SortNo).ThenBy(e => e.AdminPermissionId).ToList();

            foreach (var item in adminSystemList)
            {
                IList<admin_permission> adminPermissionListBySystem = adminPermissionList.Where(e => e.SystemId == item.AdminSystemId).ToList();
                List<AdminPermissionTree> adminPermissionTreeList = GetAdminPermissionTree(adminPermissionListBySystem, -1);

                string strZTreeJson = GetAdminPermissionZTreeJson(adminPermissionTreeList);

                if (!string.IsNullOrEmpty(strZTreeJson))
                {
                    strZTreeJson = "{ id: \"-1\", name: \"" + item.Name + "\", code: \"\", nocheck: true, children: [" + strZTreeJson + "] }, ";
                    builder.Append(strZTreeJson);
                }
            }

            string strJson = builder.ToString();
            strJson = strJson.Trim().TrimEnd(',');
            strJson = string.Format("[{0}]", strJson);

            return strJson;
        }

        #region 缓存
        private static readonly object lockObj = new object();

        /// <summary>
        /// 获取系统权限缓存
        /// </summary>
        /// <returns></returns>
        public string GetSystemPermissionZTreeJson()
        {
            ICache cache = new HttpCache();
            object obj = cache.GetCache("Admin_System_Permission_ZTreeJson");
            if (obj == null)
            {
                lock (lockObj)
                {
                    obj = cache.GetCache("Admin_System_Permission_ZTreeJson");
                    if (obj == null)
                    {
                        obj = GetAllSystemPermissionZTreeJson();
                        cache.SetCache("Admin_System_Permission_ZTreeJson", obj, TimeSpan.Zero);
                    }
                }
            }

            return (string)obj;
        }

        /// <summary>
        /// 设置系统权限缓存
        /// </summary>
        public void SetSystemPermissionZTreeJson()
        {
            ICache cache = new HttpCache();
            object obj = GetAllSystemPermissionZTreeJson();
            cache.SetCache("Admin_System_Permission_ZTreeJson", obj, TimeSpan.Zero);
        }

        #endregion
    }
}
