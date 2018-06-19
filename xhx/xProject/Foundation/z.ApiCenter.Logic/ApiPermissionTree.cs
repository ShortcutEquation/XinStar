using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using z.ApiCenter.Entity;

namespace z.ApiCenter.Logic
{
    [Serializable]
    public class ApiPermissionTree:api_function
    {
        private List<ApiPermissionTree> _ChildApiPermissions = new List<ApiPermissionTree>();
        /// <summary>
        /// 权限子树
        /// </summary>
        public List<ApiPermissionTree> ChildApiPermissions
        {
            get { return _ChildApiPermissions; }
            set { _ChildApiPermissions = value; }
        }
    }
}
