using z.AdminCenter.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace z.AdminCenter.Logic
{
    /// <summary>
    /// AdminPermission对象扩展类
    /// </summary>
    [Serializable]
    public class AdminPermissionExt : admin_permission
    {
        /// <summary>
        /// 批量删除时传值使用
        /// </summary>
        public List<int> AdminPermissionIds
        {
            get;
            set;
        }
    }

    [Serializable]
    public class PermissionSort
    {
        /// <summary>
        /// 拖动节点
        /// </summary>
        public admin_permission DragNode { get; set; }

        /// <summary>
        /// 目标节点
        /// </summary>
        public admin_permission TargetNode { get; set; }

        /// <summary>
        /// 移动方向
        /// </summary>
        public bool Next { get; set; }
    }
}
