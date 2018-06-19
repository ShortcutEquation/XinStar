using z.AdminCenter.Entity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace z.AdminCenter.Logic
{
    /// <summary>
    /// AdminUser对象扩展类
    /// </summary>
    [Serializable]
    public class AdminUserExt : admin_user
    {
        /// <summary>
        /// 用户登录时是否选择记住密码
        /// </summary>
        public string RememberMe
        {
            get;
            set;
        }

        /// <summary>
        /// 旧密码
        /// </summary>
        public string OldPassword { get; set; }

        /// <summary>
        /// 修改密码时再次输入的新密码
        /// </summary>
        public string ConfirmPassword
        {
            get;
            set;
        }

        /// <summary>
        /// 用户组ID集合
        /// </summary>
        public string GroupIds { get; set; }

        /// <summary>
        /// 权限ID集合
        /// </summary>
        public string PermissionIds { get; set; }

        /// <summary>
        /// 批量删除时传值使用
        /// </summary>
        public List<int> AdminUserIds
        {
            get;
            set;
        }

        /// <summary>
        /// 用户登录凭据
        /// </summary>
        public string Ticket
        {
            get;
            set;
        }

        /// <summary>
        /// 导航数据源
        /// </summary>
        public Dictionary<string, List<AdminPermissionTree>> MenuTrees
        {
            get;
            set;
        }

        /// <summary>
        /// 所有系统
        /// </summary>
        public IList<admin_system> AllAdminSystems
        {
            get;
            set;
        }

        /// <summary>
        /// 拥有权限的管理系统
        /// </summary>
        public IList<admin_system> OwnAdminSystems
        {
            get;
            set;
        }

        /// <summary>
        /// 是否为超级管理员
        /// </summary>
        public bool IsSuperUser
        {
            get;
            set;
        }

        /// <summary>
        /// 是否被迫下线
        /// </summary>
        public bool IsOffline
        {
            get;
            set;
        }

        /// <summary>
        /// 拥有的权限集合
        /// </summary>
        public List<string> OwnPermissionCodes
        {
            get;
            set;
        }

        /// <summary>
        /// supplier_admin_user的LoginName
        /// </summary>
        public string SupplierLoginName { get; set; }

        /// <summary>
        /// 原密码：用于后续组成supplier_admin_user的密码
        /// </summary>
        public string SupplierPassword { get; set; }
    }

    /// <summary>
    /// AdminUser信息
    /// </summary>
    [Serializable]
    public class AdminUserInfo
    {
        public int status { get; set; }
        public string errorCode { get; set; }
        public string errorMessage { get; set; }

        public string RealName { get; set; }
        public bool IsExist { get; set; }
    }

    /// <summary>
    /// 添加角色-用户关系
    /// </summary>
    [Serializable]
    public class AdminRoleUserParam
    {
        public int AdminUserId { get; set; }
        public int RoleId { get; set; }

    }
}
