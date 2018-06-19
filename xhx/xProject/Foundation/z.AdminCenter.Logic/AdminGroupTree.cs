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
    public class AdminGroupTree : admin_group
    {
        private List<AdminGroupTree> _ChildAdminGroups = new List<AdminGroupTree>();
        /// <summary>
        /// 组织子树
        /// </summary>
        public List<AdminGroupTree> ChildAdminGroups
        {
            get { return _ChildAdminGroups; }
            set { _ChildAdminGroups = value; }
        }
    }
}
