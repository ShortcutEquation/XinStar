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
    public class AdminGroupManageBase : NHLogicBase
    {
        /// <summary>
        /// 获取指定父级分组包含的所有分组列表（用于AdminGroupManage的Deleted方法）
        /// </summary>
        /// <param name="intParentId"></param>
        /// <returns></returns>
        public IList<admin_group> GetAdminGroupList(int intParentId)
        {
            List<admin_group> result = new List<admin_group>();

            IList<admin_group> adminGroupList = Repository.Find<admin_group>(e => e.ParentId == intParentId && e.Deleted == false);
            if (adminGroupList.Count > 0)
            {
                foreach (var item in adminGroupList)
                {
                    result.AddRange(GetAdminGroupList(item.AdminGroupId));
                }

                result.AddRange(adminGroupList);
            }

            return result;
        }

        #region 获取系统权限树结构

        /// <summary>
        /// 获取组织树
        /// </summary>
        /// <param name="adminGroupList"></param>
        /// <param name="parentId"></param>
        /// <returns></returns>
        protected List<AdminGroupTree> GetAdminGroupTree(IList<admin_group> adminGroupList, int parentId)
        {
            List<AdminGroupTree> adminGroupTreeList = new List<AdminGroupTree>();

            var adminGroups = from record in adminGroupList
                                   where record.ParentId == parentId
                                   select record;
            foreach (var obj in adminGroups)
            {
                AdminGroupTree adminGroupTree = new AdminGroupTree();
                obj.CopyTo(adminGroupTree);
                adminGroupTreeList.Add(adminGroupTree);
                adminGroupTree.ChildAdminGroups.AddRange(GetAdminGroupTree(adminGroupList, adminGroupTree.AdminGroupId));
            }
            return adminGroupTreeList;
        }
        #endregion

        #region 获取系统权限支持ZTree的JSON数据

        /// <summary>
        /// 获取支持ZTree的JSON结构
        /// </summary>
        /// <param name="adminGroupTreeList"></param>
        /// <returns></returns>
        protected string GetAdminGroupZTreeJson(List<AdminGroupTree> adminGroupTreeList)
        {
            StringBuilder jsonBuilder = new StringBuilder();
            foreach (var obj in adminGroupTreeList)
            {
                jsonBuilder.AppendFormat("{{ id: \"{0}\", name: \"{1}\", parentId: \"{2}\"", obj.AdminGroupId, obj.Name, obj.ParentId);
                if (obj.ChildAdminGroups.Count > 0)
                {
                    jsonBuilder.AppendFormat(", children: [{0}", GetAdminGroupZTreeJson(obj.ChildAdminGroups));
                    jsonBuilder.Append("] ");
                }
                jsonBuilder.Append("}, ");
            }
            return jsonBuilder.ToString().Trim().TrimEnd(",".ToCharArray());
        }

        #endregion

        #region 缓存
        ICache cache = new HttpCache();
        private string CacheKey_AdminGroupList = "Admin_Group";
        private string CacheKey_AdminGroupTree = "Admin_Group_Tree";

        private static readonly object ListLockObj = new object();
        private static readonly object TreeLockObj = new object();

        /// <summary>
        /// 获取分组缓存
        /// </summary>
        /// <returns></returns>
        public IList<admin_group> GetAllAdminGroups()
        {           
            object obj = cache.GetCache(CacheKey_AdminGroupList);
            if (obj == null)
            {
                lock (ListLockObj)
                {
                    obj = cache.GetCache(CacheKey_AdminGroupList);
                    if (obj == null)
                    {
                        obj = Repository.AsQueryable<admin_group>().Where(e => e.Disabled == false && e.Deleted == false).OrderBy(e => e.SortNo).ThenBy(e => e.AdminGroupId).ToList();
                        cache.SetCache(CacheKey_AdminGroupList, obj, TimeSpan.Zero);
                    }
                }
            }

            return (List<admin_group>)obj;
        }

        /// <summary>
        /// 获取分组树结构缓存
        /// </summary>
        /// <returns></returns>
        public List<AdminGroupTree> GetAllAdminGroupTree()
        {
            object obj = cache.GetCache(CacheKey_AdminGroupTree);
            if (obj == null)
            {
                lock (TreeLockObj)
                {
                    obj = cache.GetCache(CacheKey_AdminGroupTree);
                    if (obj == null)
                    {
                        obj = GetAdminGroupTree(GetAllAdminGroups(), -1);
                        cache.SetCache(CacheKey_AdminGroupTree, obj, TimeSpan.Zero);
                    }
                }
            }

            return (List<AdminGroupTree>)obj;
        }

        /// <summary>
        /// 重新设置所有分组缓存
        /// </summary>
        protected void SetAllAdminGroups()
        {
            object obj = Repository.AsQueryable<admin_group>().Where(e => e.Disabled == false && e.Deleted == false).OrderBy(e => e.SortNo).ThenBy(e => e.AdminGroupId).ToList();
            cache.SetCache(CacheKey_AdminGroupList, obj, TimeSpan.Zero);
        }

        /// <summary>
        /// 重新设置所有分组树缓存
        /// </summary>
        protected void SetAllAdminGroupTree()
        {
            SetAllAdminGroups();
            object obj = GetAdminGroupTree(GetAllAdminGroups(), -1);
            cache.SetCache(CacheKey_AdminGroupTree, obj, TimeSpan.Zero);
        }


        #region 手动刷新所有缓存
        /// <summary>
        /// 刷新权限管理页面的所有缓存数据
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public BoolResult RefreshAllCache()
        {
            BoolResult result = new BoolResult();

            try
            {
                //刷新缓存
                SetAllAdminGroupTree();//管理员组
                new AdminRoleManageBase().SetAllAdminRoles();//角色树
                new AdminUserManageBase().SetAllUsersCache();//用户，用户角色关系
                new AdminPermissionManage().SetAllAdminPermissions();//权限
                new AdminPermissionManage().SetSystemPermissionZTreeJson();//权限树

                result.Succeeded = true;
            }
            catch (Exception ex)
            {
                result.Succeeded = false;
                result.Message = ex.Message;
            }

            return result;
        }
        #endregion

        #endregion

    }
}
