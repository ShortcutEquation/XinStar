using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using z.Foundation.Data;

namespace z.AdminCenter.Entity
{
	[Serializable, CustomData(ConnectionName = "AdminCenterDB"), Table("admin_user_group")]
	public partial class admin_user_group : EntityBase
    {
		/// <summary>
		///
		/// </summary>
		[Key, Column("AdminUserGroupId")]
		public virtual Int32 AdminUserGroupId
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
		[Column("GroupId")]
		public virtual Int32 GroupId
        {
            get;
            set;
        }

	}
}
