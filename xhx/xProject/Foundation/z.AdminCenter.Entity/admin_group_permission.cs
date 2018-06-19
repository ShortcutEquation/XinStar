using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using z.Foundation.Data;

namespace z.AdminCenter.Entity
{
	[Serializable, CustomData(ConnectionName = "AdminCenterDB"), Table("admin_group_permission")]
	public partial class admin_group_permission : EntityBase
    {
		/// <summary>
		///
		/// </summary>
		[Key, Column("AdminGroupPermissionId")]
		public virtual Int32 AdminGroupPermissionId
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
		[Column("PermissionId")]
		public virtual Int32 PermissionId
        {
            get;
            set;
        }

	}
}
