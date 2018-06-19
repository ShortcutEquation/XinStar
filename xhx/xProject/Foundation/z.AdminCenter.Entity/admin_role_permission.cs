using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using z.Foundation.Data;

namespace z.AdminCenter.Entity
{
	[Serializable, CustomData(ConnectionName = "AdminCenterDB"), Table("admin_role_permission")]
	public partial class admin_role_permission : EntityBase
    {
		/// <summary>
		///
		/// </summary>
		[Key, Column("AdminRolePermissionId")]
		public virtual Int32 AdminRolePermissionId
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
