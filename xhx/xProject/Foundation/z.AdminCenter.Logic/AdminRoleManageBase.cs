using z.AdminCenter.Entity;
using z.Logic.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using z.Foundation;
using z.Foundation.Cache;
using System.Data;
using z.Foundation.Data;

namespace z.AdminCenter.Logic
{
    public class AdminRoleManageBase : NHLogicBase
    {
        /// <summary>
        /// 获取指定父级角色包含的所有角色列表（用于AdminRoleManage的Deleted方法）
        /// </summary>
        /// <param name="intParentId"></param>
        /// <returns></returns>
        public IList<admin_role> GetAdminRoleList(int intParentId)
        {
            List<admin_role> result = new List<admin_role>();

            IList<admin_role> adminRoleList = Repository.Find<admin_role>(e => e.ParentId == intParentId && e.Deleted == false);
            if (adminRoleList.Count > 0)
            {
                foreach (var item in adminRoleList)
                {
                    result.AddRange(GetAdminRoleList(item.AdminRoleId));
                }

                result.AddRange(adminRoleList);
            }

            return result;
        }

        #region 获取系统权限树结构

        /// <summary>
        /// 获取角色树
        /// </summary>
        /// <param name="adminRoleList"></param>
        /// <param name="parentId"></param>
        /// <returns></returns>
        protected List<AdminRoleTree> GetAdminRoleTree(IList<admin_role> adminRoleList, int parentId)
        {
            List<AdminRoleTree> adminRoleTreeList = new List<AdminRoleTree>();

            var adminRoles = from record in adminRoleList
                                   where record.ParentId == parentId
                                   select record;
            foreach (var obj in adminRoles)
            {
                AdminRoleTree adminRoleTree = new AdminRoleTree();
                obj.CopyTo(adminRoleTree);
                adminRoleTreeList.Add(adminRoleTree);
                adminRoleTree.ChildAdminRoles.AddRange(GetAdminRoleTree(adminRoleList, adminRoleTree.AdminRoleId));
            }
            return adminRoleTreeList;
        }

        #endregion

        #region 获取系统权限支持ZTree的JSON数据

        /// <summary>
        /// 获取支持ZTree的JSON结构
        /// </summary>
        /// <param name="adminRoleTreeList"></param>
        /// <returns></returns>
        protected string GetAdminRoleZTreeJson(List<AdminRoleTree> adminRoleTreeList,int groupId)
        {
            StringBuilder jsonBuilder = new StringBuilder();
            foreach (var obj in adminRoleTreeList)
            {
                jsonBuilder.AppendFormat("{{ id: \"{0}\", name: \"{1}\", parentId: \"{2}\", subGroupId: \"{3}\"", obj.AdminRoleId, obj.Name, obj.ParentId, groupId);
                if (obj.ChildAdminRoles.Count > 0)
                {
                    jsonBuilder.AppendFormat(", children: [{0}", GetAdminRoleZTreeJson(obj.ChildAdminRoles, groupId));
                    jsonBuilder.Append("] ");
                }
                jsonBuilder.Append("}, ");
            }
            return jsonBuilder.ToString().Trim().TrimEnd(",".ToCharArray());
        }

        #endregion

        #region 缓存
        ICache cache = new HttpCache();
        private string CacheKey_AdminRoleList = "Admin_Role";
        private string CacheKey_AdminGroupRoleList = "Admin_Group_Role";

        private static readonly object RoleListLockObj = new object();
        private static readonly object GroupRoleListLockObj = new object();

        /// <summary>
        /// 获取角色缓存
        /// </summary>
        /// <returns></returns>
        public IList<admin_role> GetAllAdminRoles()
        {
            object obj = cache.GetCache(CacheKey_AdminRoleList);
            if (obj == null)
            {
                lock (RoleListLockObj)
                {
                    obj = cache.GetCache(CacheKey_AdminRoleList);
                    if (obj == null)
                    {
                        obj = Repository.AsQueryable<admin_role>().Where(e => e.Disabled == false && e.Deleted == false).OrderBy(e => e.SortNo).ThenBy(e => e.AdminRoleId).ToList();
                        cache.SetCache(CacheKey_AdminRoleList, obj, TimeSpan.Zero);
                    }
                }
            }

            return (List<admin_role>)obj;
        }

        /// <summary>
        /// 获取分组-角色列表缓存
        /// </summary>
        /// <returns></returns>
        public IList<admin_group_role> GetAllAdminGroupRoles()
        {
            object obj = cache.GetCache(CacheKey_AdminGroupRoleList);
            if (obj == null)
            {
                lock (GroupRoleListLockObj)
                {
                    obj = cache.GetCache(CacheKey_AdminGroupRoleList);
                    if (obj == null)
                    {
                        obj = Repository.AsQueryable<admin_group_role>().ToList();
                        cache.SetCache(CacheKey_AdminGroupRoleList, obj, TimeSpan.Zero);
                    }
                }
            }

            return (List<admin_group_role>)obj;
        }

        /// <summary>
        /// 重新设置所有角色、分组—角色缓存
        /// </summary>
        public void SetAllAdminRoles()
        {
            object obj_r = Repository.AsQueryable<admin_role>().Where(e => e.Disabled == false && e.Deleted == false).OrderBy(e => e.SortNo).ThenBy(e => e.AdminRoleId).ToList();
            object obj_gr = Repository.AsQueryable<admin_group_role>().ToList();

            cache.SetCache(CacheKey_AdminRoleList, obj_r, TimeSpan.Zero);
            cache.SetCache(CacheKey_AdminGroupRoleList, obj_gr, TimeSpan.Zero);
        }
        #endregion
    }
}
