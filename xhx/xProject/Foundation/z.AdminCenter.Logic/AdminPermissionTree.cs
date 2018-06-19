using z.AdminCenter.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace z.AdminCenter.Logic
{
    /// <summary>
    /// 权限树
    /// </summary>
    [Serializable]
    public class AdminPermissionTree : admin_permission
    {
        private List<AdminPermissionTree> _ChildAdminPermissions = new List<AdminPermissionTree>();
        /// <summary>
        /// 权限子树
        /// </summary>
        public List<AdminPermissionTree> ChildAdminPermissions
        {
            get { return _ChildAdminPermissions; }
            set { _ChildAdminPermissions = value; }
        }
    }
}
