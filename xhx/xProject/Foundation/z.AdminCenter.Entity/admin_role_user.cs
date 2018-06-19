using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using z.Foundation.Data;

namespace z.AdminCenter.Entity
{
	[Serializable, CustomData(ConnectionName = "AdminCenterDB"), Table("admin_role_user")]
	public partial class admin_role_user : EntityBase
    {
		/// <summary>
		///
		/// </summary>
		[Key, Column("AdminRoleUserId")]
		public virtual Int32 AdminRoleUserId
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
		[Column("UserId")]
		public virtual Int32 UserId
        {
            get;
            set;
        }

	}
}
