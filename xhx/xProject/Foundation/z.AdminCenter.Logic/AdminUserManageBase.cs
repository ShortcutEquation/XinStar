using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using z.AdminCenter.Entity;
using z.Foundation.Cache;
using z.Logic.Base;

namespace z.AdminCenter.Logic
{
    public class AdminUserManageBase : NHLogicBase
    {
        #region 转化为JSON数据
        /// <summary>
        /// 转化成JSON结构
        /// </summary>
        /// <param name="adminUserList"></param>
        /// <returns></returns>
        protected string GetAdminUsersJson(IList<admin_user> adminUserList)
        {
            StringBuilder jsonBuilder = new StringBuilder();
            foreach (var obj in adminUserList)
            {
                jsonBuilder.AppendFormat("{{ AdminUserId: \"{0}\", AdminName: \"{1}\", RealName: \"{2}\" }},", obj.AdminUserId, obj.AdminName, obj.RealName);
                var sss =jsonBuilder.ToString();
            }

            return jsonBuilder.ToString().Trim().TrimEnd(",".ToCharArray());
        }
        #endregion

        #region 缓存
        ICache cache = new HttpCache();
        private string CacheKey_AdminUserList = "Admin_User";
        private string CacheKey_AdminRoleUserList = "Admin_Role_User";

        private static readonly object UserListLockObj = new object();
        private static readonly object RoleUserListLockObj = new object();

        /// <summary>
        /// 获取角色缓存
        /// </summary>
        /// <returns></returns>
        public IList<admin_user> GetAllAdminUsers()
        {
            object obj = cache.GetCache(CacheKey_AdminUserList);
            if (obj == null)
            {
                lock (UserListLockObj)
                {
                    obj = cache.GetCache(CacheKey_AdminUserList);
                    if (obj == null)
                    {
                        obj = Repository.Find<admin_user>(e => e.Disabled == false && e.Deleted == false);
                        cache.SetCache(CacheKey_AdminUserList, obj, TimeSpan.Zero);
                    }
                }
            }

            return (List<admin_user>)obj;
        }

        /// <summary>
        /// 获取角色-用户列表缓存
        /// </summary>
        /// <returns></returns>
        public IList<admin_role_user> GetAllAdminRoleUsers()
        {
            object obj = cache.GetCache(CacheKey_AdminRoleUserList);
            if (obj == null)
            {
                lock (RoleUserListLockObj)
                {
                    obj = cache.GetCache(CacheKey_AdminRoleUserList);
                    if (obj == null)
                    {
                        obj = Repository.AsQueryable<admin_role_user>().ToList();
                        cache.SetCache(CacheKey_AdminRoleUserList, obj, TimeSpan.Zero);
                    }
                }
            }

            return (List<admin_role_user>)obj;
        }

        /// <summary>
        /// 重新设置所有用户缓存
        /// </summary>
        public void SetAllAdminUsers()
        {
            object obj_u = Repository.Find<admin_user>(e => e.Disabled == false && e.Deleted == false);
            cache.SetCache(CacheKey_AdminUserList, obj_u, TimeSpan.Zero);
        }

        /// <summary>
        /// 重新设置角色-用户缓存
        /// </summary>
        public void SetAllAdminRoleUsers()
        {
            object obj_ru = Repository.AsQueryable<admin_role_user>().ToList();
            cache.SetCache(CacheKey_AdminRoleUserList, obj_ru, TimeSpan.Zero);
        }

        /// <summary>
        /// 重新设置用户相关缓存
        /// </summary>
        public void SetAllUsersCache()
        {
            SetAllAdminUsers();
            SetAllAdminRoleUsers();
        }
        #endregion
    }
}
