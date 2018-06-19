using z.AdminCenter.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace z.AdminCenter.Logic
{
    /// <summary>
    /// AdminGroup对象扩展类
    /// </summary>
    [Serializable]
    public class AdminRoleExt : admin_role
    {
        /// <summary>
        /// 权限ID集合
        /// </summary>
        public string PermissionIds { get; set; }

        /// <summary>
        /// 分组下成员集合（json格式）
        /// </summary>
        public string UserModelsJson { get; set; }

        /// <summary>
        /// 批量删除时传值使用
        /// </summary>
        public List<int> AdminRoleIds
        {
            get;
            set;
        }

        /// <summary>
        /// 所属分组ID
        /// </summary>
        public int SubGroupId { get; set; }
    }
}
