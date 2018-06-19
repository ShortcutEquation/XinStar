using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using z.Foundation.Data;

namespace z.AdminCenter.Entity
{
	[Serializable, CustomData(ConnectionName = "AdminCenterDB"), Table("admin_group_role")]
	public partial class admin_group_role : EntityBase
    {
		/// <summary>
		///
		/// </summary>
		[Key, Column("AdminGroupRoleId")]
		public virtual Int32 AdminGroupRoleId
        {
            get;
            set;
        }

		/// <summary>
		///
		/// </summary>
		[Column("GroupId")]
		public virtual Int32 GroupId
        {
            get;
            set;
        }

		/// <summary>
		///
		/// </summary>
		[Column("RoleId")]
		public virtual Int32 RoleId
        {
            get;
            set;
        }

	}
}
