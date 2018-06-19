using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using z.AdminCenter.Entity;
using z.Logic.Base;
using z.Foundation;
using z.Foundation.Data;
using NHibernate;
using hk.papago.Entity.PaPaGoDB;

namespace z.AdminCenter.Logic
{
    /// <summary>
    /// 用户管理
    /// </summary>
    public class AdminUserManage : AdminUserManageBase
    {
        /// <summary>
        /// 获取对象
        /// </summary>
        /// <param name="param">主键ID</param>
        /// <returns></returns>
        public AdminUserExt Get(object param)
        {
            AdminUserExt adminUserExt = null;

            admin_user _admin_user = Repository.Get<admin_user>(param);
            if (_admin_user != null)
            {
                adminUserExt = new AdminUserExt();
                _admin_user.CopyTo(adminUserExt);

                adminUserExt.PermissionIds = GetUserPermissions(_admin_user.AdminUserId);
            }

            return adminUserExt;
        }

        /// <summary>
        /// 保存对象
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public BoolResult Save(AdminUserExt param)
        {
            return SaveBasic(param, false);
        }

        /// <summary>
        /// 保存对象-同步添加supplier_admin_user
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public BoolResult SaveAndUnion(AdminUserExt param)
        {
            return SaveBasic(param, true);
        }

        /// <summary>
        /// 保存对象Basic
        /// </summary>
        /// <param name="param"></param>
        /// <param name="isUnion">是否关联添加到supplier_admin_user</param>
        /// <returns></returns>
        public BoolResult SaveBasic(AdminUserExt param, bool isUnion)
        {
            BoolResult result = new BoolResult();

            //保持对象的唯一性
            if (Repository.Exists<admin_user>(e => e.AdminName == param.AdminName && e.Deleted == false))
            {
                result.Succeeded = false;
                result.Message = "已存在相同的AdminName值";
                return result;
            }

            admin_user _admin_user = new admin_user();
            param.CopyTo(_admin_user);

            using (ISession session = NHibernateHelper<admin_user>.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                try
                {
                    object obj = session.Save(_admin_user);
                    if (obj == null)
                    {
                        result.Succeeded = false;
                        result.Message = "保存对象失败";
                        return result;
                    }

                    #region 供应商后台同步过来的数据处理（暂时不启用）
                    ////添加用户与权限的关系
                    //string[] permissionIds = param.PermissionIds.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    //foreach (var permissionId in permissionIds)
                    //{
                    //    admin_user_permission _admin_user_permission = new admin_user_permission();
                    //    _admin_user_permission.UserId = (int)obj;
                    //    _admin_user_permission.PermissionId = int.Parse(permissionId);
                    //    session.Save(_admin_user_permission);
                    //}

                    ////添加用户与角色的关系
                    //string[] roleIds = param.RoleIds.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    //foreach (var roleId in roleIds)
                    //{
                    //    admin_role_user _admin_role_user = new admin_role_user();
                    //    _admin_role_user.UserId = (int)obj;
                    //    _admin_role_user.RoleId = int.Parse(roleId);
                    //    session.Save(_admin_role_user);
                    //}
                    #endregion

                    if (isUnion)
                    {
                        #region 添加用户supplier_admin_user
                        param.AdminUserId = (int)obj;
                        result = SaveSupplierAdminUser(param);
                        if (result.Succeeded)
                        {
                            transaction.Commit();
                        }
                        #endregion
                    }
                    else
                    {
                        transaction.Commit();
                        result.Result = obj;
                        result.Succeeded = true;
                    }

                    //更新缓存
                    SetAllUsersCache();
                }
                catch
                {
                    transaction.Rollback();
                }
            }

            return result;
        }

        /// <summary>
        /// 更新对象
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public BoolResult Update(AdminUserExt param)
        {
            return UpdateBasic(param, false);
        }

        /// <summary>
        /// 更新对象-同步更新supplier_admin_user
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public BoolResult UpdateAndUnion(AdminUserExt param)
        {
            return UpdateBasic(param, true);
        }

        /// <summary>
        /// 更新对象Basic
        /// </summary>
        /// <param name="param"></param>
        /// <param name="isUnion">是否同步更新supplier_admin_user</param>
        /// <returns></returns>
        public BoolResult UpdateBasic(AdminUserExt param,bool isUnion)
        {
            BoolResult result = new BoolResult();

            admin_user _admin_user = Repository.First<admin_user>(e => e.AdminUserId == param.AdminUserId && e.Deleted == false);
            if (_admin_user == null)
            {
                result.Succeeded = false;
                result.Message = "修改的对象不存在";
                return result;
            }

            //当更新时输入了密码，则认为需要修改该管理员的登录密码，否则密码维持不变
            if (!string.IsNullOrEmpty(param.Password))
            {
                _admin_user.Password = param.Password;                
            }

            using (ISession session = NHibernateHelper<admin_user>.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                try
                {
                    _admin_user.RealName = param.RealName;
                    _admin_user.Logo = param.Logo;
                    _admin_user.UpdateBy = param.UpdateBy;
                    _admin_user.UpdateOn = param.UpdateOn;
                    session.Update(_admin_user);

                    //同步更新supplier_admin_user密码
                    if (isUnion && !string.IsNullOrEmpty(param.Password))
                    {
                        supplier_admin_user _supplier = Repository.First<supplier_admin_user>(e => e.AdminUserId == param.AdminUserId && e.Deleted == false);
                        if (_supplier != null)
                        {
                            _supplier.LoginPassword = (_supplier.LoginName + param.SupplierPassword).MD5Encrypt();
                            _supplier.UpdateBy = param.UpdateBy;
                            _supplier.UpdateOn = param.UpdateOn;

                            Repository.Update(_supplier);
                        }
                    }
                    #region 比较该用户包含的新、旧角色集合，新增需要新增的角色，删除需要删除的角色

                    //IList<admin_role_user> adminRoleUserList = Repository.Find<admin_role_user>(exp => exp.UserId == _admin_user.AdminUserId);

                    //#region 取出两个数组的差集(交换算法)

                    //int[] oldRoleIdArray = adminRoleUserList.Select(exp => exp.RoleId).Distinct().ToArray();
                    //string[] roleIds = string.IsNullOrEmpty(param.RoleIds) ? new string[] { } : param.RoleIds.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Distinct().ToArray();
                    //int[] newRoleIdArray = Array.ConvertAll<string, int>(roleIds, delegate (string s) { return int.Parse(s); });

                    //int oldRoleIdArraySize = oldRoleIdArray.Length;
                    //int newRoleIdArraySize = newRoleIdArray.Length;
                    //int end = oldRoleIdArraySize;
                    //bool swap = false;

                    //for (int i = 0; i < end;)
                    //{
                    //    swap = false;
                    //    for (int j = i; j < newRoleIdArraySize; j++)
                    //    {
                    //        if (oldRoleIdArray[i] == newRoleIdArray[j])
                    //        {
                    //            int tmp = newRoleIdArray[i];
                    //            newRoleIdArray[i] = newRoleIdArray[j];
                    //            newRoleIdArray[j] = tmp;
                    //            swap = true;
                    //            break;
                    //        }
                    //    }
                    //    if (swap != true)
                    //    {
                    //        int tmp = oldRoleIdArray[i];
                    //        oldRoleIdArray[i] = oldRoleIdArray[--end];
                    //        oldRoleIdArray[end] = tmp;
                    //    }
                    //    else
                    //    {
                    //        i++;
                    //    }
                    //}

                    //#endregion

                    ////需删除的
                    //List<int> deleteRoleUserIdArray = new List<int>();
                    //for (int i = end; i < oldRoleIdArraySize; i++)
                    //{
                    //    deleteRoleUserIdArray.Add(oldRoleIdArray[i]);
                    //}

                    ////需添加的
                    //List<int> addRoleUserIdArray = new List<int>();
                    //for (int i = end; i < newRoleIdArraySize; i++)
                    //{
                    //    addRoleUserIdArray.Add(newRoleIdArray[i]);
                    //}

                    ////执行删除
                    //var deleteRoleUserObj = from obj in adminRoleUserList
                    //                         where deleteRoleUserIdArray.Contains(obj.RoleId)
                    //                         select obj;
                    //foreach (var obj in deleteRoleUserObj)
                    //{
                    //    session.Delete(obj);
                    //}

                    ////执行添加
                    //foreach (var obj in addRoleUserIdArray)
                    //{
                    //    admin_role_user _admin_role_user = new admin_role_user();
                    //    _admin_role_user.UserId = _admin_user.AdminUserId;
                    //    _admin_role_user.RoleId = obj;
                    //    session.Save(_admin_role_user);
                    //}

                    #endregion

                    #region 比较该用户包含的新、旧权限集合，新增需要新增的权限，删除需要删除的权限

                    //IList<admin_user_permission> adminUserPermissionList = Repository.Find<admin_user_permission>(exp => exp.UserId == _admin_user.AdminUserId);

                    #region 取出两个数组的差集(交换算法)

                    //int[] oldPermissionIdArray = adminUserPermissionList.Select(exp => exp.PermissionId).Distinct().ToArray();
                    //string[] permissionIds = string.IsNullOrEmpty(param.PermissionIds) ? new string[] { } : param.PermissionIds.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Distinct().ToArray();
                    //int[] newPermissionIdArray = Array.ConvertAll<string, int>(permissionIds, delegate (string s) { return int.Parse(s); });

                    //int oldPermissionIdArraySize = oldPermissionIdArray.Length;
                    //int newPermissionIdArraySize = newPermissionIdArray.Length;
                    //int end = oldPermissionIdArraySize;
                    //bool swap = false;

                    //for (int i = 0; i < end;)
                    //{
                    //    swap = false;
                    //    for (int j = i; j < newPermissionIdArraySize; j++)
                    //    {
                    //        if (oldPermissionIdArray[i] == newPermissionIdArray[j])
                    //        {
                    //            int tmp = newPermissionIdArray[i];
                    //            newPermissionIdArray[i] = newPermissionIdArray[j];
                    //            newPermissionIdArray[j] = tmp;
                    //            swap = true;
                    //            break;
                    //        }
                    //    }
                    //    if (swap != true)
                    //    {
                    //        int tmp = oldPermissionIdArray[i];
                    //        oldPermissionIdArray[i] = oldPermissionIdArray[--end];
                    //        oldPermissionIdArray[end] = tmp;
                    //    }
                    //    else
                    //    {
                    //        i++;
                    //    }
                    //}

                    #endregion

                    ////需删除的
                    //List<int> deleteUserPermissionIdArray = new List<int>();
                    //for (int i = end; i < oldPermissionIdArraySize; i++)
                    //{
                    //    deleteUserPermissionIdArray.Add(oldPermissionIdArray[i]);
                    //}

                    ////需添加的
                    //List<int> addUserPermissionIdArray = new List<int>();
                    //for (int i = end; i < newPermissionIdArraySize; i++)
                    //{
                    //    addUserPermissionIdArray.Add(newPermissionIdArray[i]);
                    //}

                    ////执行删除
                    //var deleteUserPermissionObj = from obj in adminUserPermissionList
                    //                              where deleteUserPermissionIdArray.Contains(obj.PermissionId)
                    //                              select obj;
                    //foreach (var obj in deleteUserPermissionObj)
                    //{
                    //    session.Delete(obj);
                    //}

                    ////执行添加
                    //foreach (var obj in addUserPermissionIdArray)
                    //{
                    //    admin_user_permission _admin_user_permission = new admin_user_permission();
                    //    _admin_user_permission.UserId = _admin_user.AdminUserId;
                    //    _admin_user_permission.PermissionId = obj;
                    //    session.Save(_admin_user_permission);
                    //}

                    #endregion

                    transaction.Commit();

                    //更新缓存
                    SetAllUsersCache();

                    //主动更新用户会话
                    new AdminAccountManage().UpdateUserSession(_admin_user.AdminUserId);

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
        public BoolResult Disabled(AdminUserExt param)
        {
            return DisabledBasic(param, false);
        }

        /// <summary>
        /// 单个、批量禁用/启用对象-同步更新supplier_admin_user
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public BoolResult DisabledAndUnion(AdminUserExt param)
        {
            return DisabledBasic(param, true);
        }

        /// <summary>
        /// 单个、批量禁用/启用对象Basic
        /// </summary>
        /// <param name="param"></param>
        /// <param name="isUnion">是否同步更新supplier_admin_user</param>
        /// <returns></returns>
        public BoolResult DisabledBasic(AdminUserExt param,bool isUnion)
        {
            BoolResult result = new BoolResult();

            IList<admin_user> adminUserList = Repository.Find<admin_user>(e => param.AdminUserIds.Contains(e.AdminUserId) && e.Disabled != param.Disabled && e.Deleted == false);
            if (adminUserList.Count == 0)
            {
                result.Succeeded = false;
                result.Message = string.Format("{0}的对象不存在", param.Disabled ? "禁用" : "启用");
                return result;
            }
            else
            {
                using (ISession session = NHibernateHelper<admin_user>.OpenSession())
                using (ITransaction transaction = session.BeginTransaction())
                {

                    try
                    {
                        int intRecord = 0;
                        foreach (var _admin_user in adminUserList)
                        {
                            _admin_user.Disabled = param.Disabled;
                            _admin_user.UpdateBy = param.UpdateBy;
                            _admin_user.UpdateOn = param.UpdateOn;
                            session.Update(_admin_user);


                            #region 同步更新supplier_admin_user
                            if (isUnion)
                            {
                                supplier_admin_user _supplier = Repository.First<supplier_admin_user>(e => e.AdminUserId == _admin_user.AdminUserId && e.Deleted == false);
                                if (_supplier != null)
                                {
                                    _supplier.Disabled = param.Disabled;
                                    _supplier.UpdateBy = param.UpdateBy;
                                    _supplier.UpdateOn = param.UpdateOn;

                                    Repository.Update(_supplier);
                                }
                            }
                            #endregion

                            intRecord++;
                        }

                        transaction.Commit();

                        //更新缓存
                        SetAllUsersCache();

                        foreach (var _admin_user in adminUserList)
                        {
                            //主动更新用户会话
                            new AdminAccountManage().UpdateUserSession(_admin_user.AdminUserId);
                        }

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
        public BoolResult Deleted(AdminUserExt param)
        {
            return DeletedBasic(param, false);
        }

        /// <summary>
        /// 单个、批量删除对象-同步更新supplier_admin_user
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public BoolResult DeletedAndUnion(AdminUserExt param)
        {
            return DeletedBasic(param, true);
        }

        /// <summary>
        /// 单个、批量删除对象Basic
        /// </summary>
        /// <param name="param"></param>
        /// <param name="isUnion">是否同步删除supplier_admin_user</param>
        /// <returns></returns>
        public BoolResult DeletedBasic(AdminUserExt param, bool isUnion)
        {
            BoolResult result = new BoolResult();

            IList<admin_user> adminUserList = Repository.Find<admin_user>(e => param.AdminUserIds.Contains(e.AdminUserId) && e.Deleted == false);
            if (adminUserList.Count == 0)
            {
                result.Succeeded = false;
                result.Message = "删除的对象不存在";
                return result;
            }
            else
            {
                using (ISession session = NHibernateHelper<admin_user>.OpenSession())
                using (ITransaction transaction = session.BeginTransaction())
                {
                    try
                    {
                        int intRecord = 0;
                        foreach (var _admin_user in adminUserList)
                        {
                            //删除用户时需要删除用户与权限的关系、用户与角色的关系、用户与隶属系统的关系

                            _admin_user.Deleted = true;
                            _admin_user.UpdateBy = param.UpdateBy;
                            _admin_user.UpdateOn = param.UpdateOn;
                            session.Update(_admin_user);

                            //删除用户与权限的关系
                            IList<admin_user_permission> adminUserPermissionList = Repository.Find<admin_user_permission>(e => e.UserId == _admin_user.AdminUserId);
                            foreach (var _admin_user_permission in adminUserPermissionList)
                            {
                                session.Delete(_admin_user_permission);
                            }

                            //删除用户与角色的关系
                            IList<admin_role_user> adminRoleUserList = Repository.Find<admin_role_user>(e => e.UserId == _admin_user.AdminUserId);
                            foreach (var _admin_role_user in adminRoleUserList)
                            {
                                session.Delete(_admin_role_user);
                            }

                            #region 同步删除supplier_admin_user
                            if (isUnion)
                            {
                                supplier_admin_user _supplier = Repository.First<supplier_admin_user>(e => e.AdminUserId == _admin_user.AdminUserId && e.Deleted == false);
                                if (_supplier != null)
                                {
                                    _supplier.Deleted = true;
                                    _supplier.UpdateBy = param.UpdateBy;
                                    _supplier.UpdateOn = param.UpdateOn;

                                    Repository.Update(_supplier);
                                }
                            }
                            #endregion

                            intRecord++;
                        }

                        transaction.Commit();

                        //更新缓存
                        SetAllUsersCache();

                        foreach (var _admin_user in adminUserList)
                        {
                            //主动更新用户会话
                            new AdminAccountManage().UpdateUserSession(_admin_user.AdminUserId);
                        }

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
        /// 分页获取列表（提供按AdminName、RealName进行模糊搜索，搜索时给AdminName属性赋值，不提供客户端自定义排序，默认按创建时间倒序排列）
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public IPagedList<admin_user> GetPageList(IPagedParam<admin_user> param)
        {
            IQueryable<admin_user> queryable = Repository.AsQueryable<admin_user>().Where(e => e.Deleted == false);
            if (!string.IsNullOrEmpty(param.model.AdminName))
            {
                queryable = queryable.Where(e => e.AdminName.Contains(param.model.AdminName) || e.RealName.Contains(param.model.AdminName));
            }
            queryable = queryable.OrderByDescending(e => e.CreateOn);

            return new PagedList<admin_user>(queryable, param.PageIndex, param.PageSize);
        }

        /// <summary>
        /// 获取所有用户
        /// </summary>
        /// <returns></returns>
        public List<admin_user> GetAllUsers()
        {
            return Repository.Find<admin_user>(e => e.Deleted == false).ToList();
        }

        /// <summary>
        /// 获取所有用户列表
        /// </summary>
        /// <returns></returns>
        public IList<admin_user> GetList()
        {
            return Repository.Find<admin_user>(e => e.Disabled == false && e.Deleted == false);
        }

        /// <summary>
        /// 根据角色ID获取对应的用户列表
        /// </summary>
        /// <returns></returns>
        public IList<admin_user> GetListByRoleID(int roleId)
        {
            IList<admin_user> result = new List<admin_user>();

            var _admin_role_user = Repository.Find<admin_role_user>(e => e.RoleId == roleId);
            if (_admin_role_user.Count > 0)
            {
                var userIds = _admin_role_user.Select(e => e.UserId).ToList();
                result = Repository.Find<admin_user>(e => userIds.Contains(e.AdminUserId) && e.Disabled == false && e.Deleted == false);
            }

            return result;
        }

        /// <summary>
        /// 获取对象
        /// </summary>
        /// <param name="param">主键ID</param>
        /// <returns></returns>
        public admin_user GetModel(object param)
        {
            return Repository.Get<admin_user>(param);
        }

        /// <summary>
        /// 保存supplier_admin_user
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public BoolResult SaveSupplierAdminUserBasic(AdminUserExt param)
        {
            BoolResult result = new BoolResult();

            //保持对象的唯一性
            if (Repository.Exists<supplier_admin_user>(e => e.LoginName == param.SupplierLoginName && e.Deleted == false))
            {
                result.Succeeded = false;
                result.Message = "已存在相同的LoginName值";
                return result;
            }
            supplier_admin_user _supplier = new supplier_admin_user();

            param.CopyTo(_supplier);

            var SupplierParentId = Utility.GetConfigValue("SupplierParentId");

            _supplier.LoginName = param.SupplierLoginName;
            _supplier.LoginPassword = (param.SupplierLoginName + param.SupplierPassword).MD5Encrypt(); 
            _supplier.ParentId = Convert.ToInt32(SupplierParentId);

            using (ISession session = NHibernateHelper<supplier_admin_user>.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                try
                {
                    object obj = session.Save(_supplier);
                    if (obj == null)
                    {
                        result.Succeeded = false;
                        result.Message = "保存对象失败";
                        return result;
                    }
                    transaction.Commit();
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
        /// 保存对象
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public BoolResult SaveSupplierAdminUser(AdminUserExt param)
        {
            BoolResult result = SaveSupplierAdminUserBasic(param);

            while (!result.Succeeded)
            {
                if (result.Message == "已存在相同的LoginName值")
                {
                    var random = new Random();
                    param.SupplierLoginName = param.AdminName + new Random().Next();
                    result = SaveSupplierAdminUserBasic(param);
                    if (result.Succeeded)
                    {
                        break;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 获取指定角色下的用户数据（json格式）
        /// </summary>
        /// <returns></returns>
        public string GetUserDataJson(int roleId)
        {
            StringBuilder builder = new StringBuilder();

            IList<admin_role_user> roleUserList = GetAllAdminRoleUsers().Where(e => e.RoleId == roleId).ToList();
            if (roleUserList.Count == 0)
            {
                return "";
            }

            List<int> userIds = roleUserList.Select(e => e.UserId).ToList();
            IList<admin_user> adminUserList = GetAllAdminUsers().Where(e => userIds.Contains(e.AdminUserId) && e.Deleted == false && e.Disabled == false).ToList();

            string strUsersModelJson = GetAdminUsersJson(adminUserList);
            builder.Append(strUsersModelJson);
            string strJson = builder.ToString();
            strJson = strJson.Trim().TrimEnd(',');
            strJson = string.Format("[{0}]", strJson);

            return strJson;
        }

        /// <summary>
        /// 根据用户名称模糊查找用户，显示5条
        /// </summary>
        /// <param name="q"></param>
        /// <returns></returns>
        public IList<admin_user> SearchAdminUser(string kewords)
        {
            if (string.IsNullOrEmpty(kewords))
            {
                return new List<admin_user>();
            }

            IQueryable<admin_user> query = Repository.AsQueryable<admin_user>()
                .Where(e => e.Deleted == false)
                .Where(e => e.AdminName.Contains(kewords) || e.RealName.Contains(kewords));

            return query.Take(5).ToList();
        }

        /// <summary>
        /// 保存用户与成员的关系
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public BoolResult RoleToUserSave(AdminRoleUserParam param)
        {
            BoolResult result = new BoolResult();

            //保持对象的唯一性
            if (Repository.Exists<admin_role_user>(e => e.RoleId == param.RoleId && e.UserId == param.AdminUserId))
            {
                result.Succeeded = false;
                result.Message = "该角色下已存在相同的用户";
                return result;
            }

            admin_role_user _admin_role_user = new admin_role_user() {
                RoleId = param.RoleId,
                UserId = param.AdminUserId
            };

            using (ISession session = NHibernateHelper<admin_user>.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                try
                {
                    object obj = session.Save(_admin_role_user);
                    if (obj == null)
                    {
                        result.Succeeded = false;
                        result.Message = "保存对象失败";
                        return result;
                    }

                    transaction.Commit();

                    //更新缓存
                    SetAllAdminRoleUsers();

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
        /// 删除用户与成员的关系
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public BoolResult RoleToUserDelect(AdminRoleUserParam param)
        {
            BoolResult result = new BoolResult();

            var roleUserList = Repository.Find<admin_role_user>(e => e.RoleId == param.RoleId && e.UserId == param.AdminUserId);
            if (roleUserList.Count == 0)
            {
                result.Succeeded = false;
                result.Message = "该角色下不存在该用户";
                return result;
            }
           
            using (ISession session = NHibernateHelper<admin_role_user>.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                try
                {
                    foreach (var _admin_role_user in roleUserList)
                    {
                        session.Delete(_admin_role_user);
                    }

                    transaction.Commit();

                    //更新缓存
                    SetAllAdminRoleUsers();

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
        /// 获取成员权限
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public string GetUserPermissions(int userId)
        {
            var result = "";

            IList<admin_user_permission> adminUserPermissionList = Repository.Find<admin_user_permission>(exp => exp.UserId == userId);
            StringBuilder permissionIds = new StringBuilder();
            foreach (var obj in adminUserPermissionList)
            {
                permissionIds.AppendFormat("{0},", obj.PermissionId);
            }
            result = permissionIds.ToString().TrimEnd(',');

            return result;
        }

        /// <summary>
        /// 更新权限
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public BoolResult UpdatePermission(AdminUserExt param)
        {
            BoolResult result = new BoolResult();

            admin_user _admin_user = Repository.First<admin_user>(e => e.AdminUserId == param.AdminUserId && e.Deleted == false);
            if (_admin_user == null)
            {
                result.Succeeded = false;
                result.Message = "修改的对象不存在";
                return result;
            }

            using (ISession session = NHibernateHelper<admin_user_permission>.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                try
                {
                    IList<admin_user_permission> adminUserPermissionList = Repository.Find<admin_user_permission>(e => e.UserId == _admin_user.AdminUserId);

                    #region 当角色更新，比较该组包含的新、旧权限集合，新增需要新增的权限，删除需要删除的权限

                    #region 取出两个数组的差集(交换算法)

                    int[] oldPermissionIdArray = adminUserPermissionList.Select(exp => exp.PermissionId).Distinct().ToArray();
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
                    List<int> deleteUserPermissionIdArray = new List<int>();
                    for (int i = end; i < oldPermissionIdArraySize; i++)
                    {
                        deleteUserPermissionIdArray.Add(oldPermissionIdArray[i]);
                    }

                    //需添加的
                    List<int> addUserPermissionIdArray = new List<int>();
                    for (int i = end; i < newPermissionIdArraySize; i++)
                    {
                        addUserPermissionIdArray.Add(newPermissionIdArray[i]);
                    }

                    //执行删除
                    var deleteUserPermissionObj = from obj in adminUserPermissionList
                                                  where deleteUserPermissionIdArray.Contains(obj.PermissionId)
                                                  select obj;
                    foreach (var obj in deleteUserPermissionObj)
                    {
                        session.Delete(obj);
                    }

                    //执行添加
                    foreach (var obj in addUserPermissionIdArray)
                    {
                        admin_user_permission _admin_user_permission = new admin_user_permission();
                        _admin_user_permission.UserId = _admin_user.AdminUserId;
                        _admin_user_permission.PermissionId = obj;
                        session.Save(_admin_user_permission);
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
        /// 获取该管理员下管理的所有成员
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public BoolResult GetManageUsers(int userId)
        {
            var result = new BoolResult();
            var adminRoleList = new List<admin_role>();

            IList<admin_role_user> adminRoleUserList = Repository.Find<admin_role_user>(exp => exp.UserId == userId);

            if (adminRoleUserList.Count == 0)
            {
                result.Succeeded = false;
                result.Message = "该管理员没有分配角色";
                return result;
            }

            foreach (var role_user in adminRoleUserList)
            {
                var roleList =new AdminRoleManageBase().GetAdminRoleList(role_user.RoleId).ToList();
                adminRoleList.AddRange(roleList);
            }

            if (adminRoleList.Count == 0)
            {
                result.Succeeded = false;
                result.Message = "该管理员下没有管理的成员";
                return result;
            }

            var roleIds = adminRoleList.Select(e => e.AdminRoleId).ToList();
            var subRoleUser = Repository.Find<admin_role_user>(exp => roleIds.Contains(exp.RoleId));

            if (subRoleUser.Count == 0)
            {
                result.Succeeded = false;
                result.Message = "该管理员下没有管理的成员";
                return result;
            }

            var subUserIds = subRoleUser.Select(e => e.UserId).ToList();
            var userList = Repository.Find<admin_user>(exp => subUserIds.Contains(exp.AdminUserId)).Select(e=>new {e.AdminUserId,e.AdminName,e.RealName}).ToList();

            result.Succeeded = true;
            result.Result = userList;

            return result;
        }
    }
}
