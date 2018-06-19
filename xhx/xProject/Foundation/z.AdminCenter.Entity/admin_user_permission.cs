using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using z.Foundation.Data;

namespace z.AdminCenter.Entity
{
	[Serializable, CustomData(ConnectionName = "AdminCenterDB"), Table("admin_user_permission")]
	public partial class admin_user_permission : EntityBase
    {
		/// <summary>
		///
		/// </summary>
		[Key, Column("AdminUserPermissionId")]
		public virtual Int32 AdminUserPermissionId
        {
            get;
            set;
        }

		/// <summary>
		///
		/// </summary>
		[Column("UserId")]
		public virtual Int32 UserId
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
