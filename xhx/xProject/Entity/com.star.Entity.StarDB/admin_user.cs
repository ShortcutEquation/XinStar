using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using z.Foundation.Data;

namespace com.star.Entity.StarDB
{
	[Serializable, CustomData(ConnectionName = "StarDB"), Table("admin_user")]
	public partial class admin_user : EntityBase
    {
		/// <summary>
		///
		/// </summary>
		[Key, Column("Id")]
		public virtual Int32 Id
        {
            get;
            set;
        }

		/// <summary>
		///
		/// </summary>
		[Column("ParentId")]
		public virtual Int32 ParentId
        {
            get;
            set;
        }

		/// <summary>
		///
		/// </summary>
		[Column("PermissionCode")]
		public virtual String PermissionCode
        {
            get;
            set;
        }

		/// <summary>
		///
		/// </summary>
		[Column("Name")]
		public virtual String Name
        {
            get;
            set;
        }

		/// <summary>
		/// 根权限图标（即可为样式名称，也可为IMG路径，视具体情况而定）
		/// </summary>
		[Column("Img")]
		public virtual String Img
        {
            get;
            set;
        }

		/// <summary>
		///
		/// </summary>
		[Column("Description")]
		public virtual String Description
        {
            get;
            set;
        }

		/// <summary>
		///
		/// </summary>
		[Column("IsMenu")]
		public virtual Boolean IsMenu
        {
            get;
            set;
        }

		/// <summary>
		///
		/// </summary>
		[Column("IsLink")]
		public virtual Boolean IsLink
        {
            get;
            set;
        }

		/// <summary>
		///
		/// </summary>
		[Column("Url")]
		public virtual String Url
        {
            get;
            set;
        }

		/// <summary>
		///
		/// </summary>
		[Column("Target")]
		public virtual String Target
        {
            get;
            set;
        }

		/// <summary>
		///
		/// </summary>
		[Column("SortNo")]
		public virtual Int32 SortNo
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
