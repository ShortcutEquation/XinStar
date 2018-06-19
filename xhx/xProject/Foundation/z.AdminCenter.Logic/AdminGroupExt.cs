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
    public class AdminGroupExt : admin_group
    {
        /// <summary>
        /// 权限ID集合
        /// </summary>
        public string PermissionIds { get; set; }

        /// <summary>
        /// 分组下角色集合（json格式）
        /// </summary>
        public string RoleZtreeJson { get; set; }

        /// <summary>
        /// 批量删除时传值使用
        /// </summary>
        public List<int> AdminGroupIds
        {
            get;
            set;
        }
    }

    /// <summary>
    /// 移动节点
    /// </summary>
    [Serializable]
    public class AdminNodeMoveParam
    {
        /// <summary>
        /// 当前节点Id
        /// </summary>
        public int NodeId { get; set; }

        /// <summary>
        /// 目标节点Id
        /// </summary>
        public int TargetNodeId{ get; set; }

        public string UpdateBy { get; set; }
    }
}
