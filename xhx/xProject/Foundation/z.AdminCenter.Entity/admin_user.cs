using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using z.Foundation.Data;

namespace z.AdminCenter.Entity
{
	[Serializable, CustomData(ConnectionName = "AdminCenterDB"), Table("admin_user")]
	public partial class admin_user : EntityBase
    {
		/// <summary>
		///
		/// </summary>
		[Key, Column("AdminUserId")]
		public virtual Int32 AdminUserId
        {
            get;
            set;
        }

		/// <summary>
		///
		/// </summary>
		[Column("AdminName")]
		public virtual String AdminName
        {
            get;
            set;
        }

		/// <summary>
		///
		/// </summary>
		[Column("Password")]
		public virtual String Password
        {
            get;
            set;
        }

		/// <summary>
		///
		/// </summary>
		[Column("RealName")]
		public virtual String RealName
        {
            get;
            set;
        }

		/// <summary>
		///
		/// </summary>
		[Column("Logo")]
		public virtual String Logo
        {
            get;
            set;
        }

		/// <summary>
		///
		/// </summary>
		[Column("Disabled")]
		public virtual Boolean Disabled
        {
            get;
            set;
        }

		/// <summary>
		///
		/// </summary>
		[Column("Deleted")]
		public virtual Boolean Deleted
        {
            get;
            set;
        }

		/// <summary>
		///
		/// </summary>
		[Column("CreateBy")]
		public virtual String CreateBy
        {
            get;
            set;
        }

		/// <summary>
		///
		/// </summary>
		[Column("CreateOn")]
		public virtual DateTime CreateOn
        {
            get;
            set;
        }

		/// <summary>
		///
		/// </summary>
		[Column("UpdateBy")]
		public virtual String UpdateBy
        {
            get;
            set;
        }

		/// <summary>
		///
		/// </summary>
		[Column("UpdateOn")]
		public virtual DateTime? UpdateOn
        {
            get;
            set;
        }

	}
}
