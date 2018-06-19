using z.AdminCenter.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace z.AdminCenter.Logic
{
    /// <summary>
    /// 组织树
    /// </summary>
    [Serializable]
    public class AdminRoleTree : admin_role
    {
        private List<AdminRoleTree> _ChildAdminRoles = new List<AdminRoleTree>();
        /// <summary>
        /// 角色子树
        /// </summary>
        public List<AdminRoleTree> ChildAdminRoles
        {
            get { return _ChildAdminRoles; }
            set { _ChildAdminRoles = value; }
        }
    }
}
