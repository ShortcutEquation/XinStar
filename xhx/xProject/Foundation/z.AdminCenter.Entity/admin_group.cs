using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using z.Foundation.Data;

namespace z.AdminCenter.Entity
{
	[Serializable, CustomData(ConnectionName = "AdminCenterDB"), Table("admin_group")]
	public partial class admin_group : EntityBase
    {
		/// <summary>
		///
		/// </summary>
		[Key, Column("AdminGroupId")]
		public virtual Int32 AdminGroupId
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
		[Column("Name")]
		public virtual String Name
        {
            get;
            set;
        }

		/// <summary>
		/// 0-超级管理员（默认拥有所有权限）1-普通管理员（设定其拥有的权限）
		/// </summary>
		[Column("Type")]
		public virtual Int32 Type
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
		[Column("Description")]
		public virtual String Description
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
		/// 必须为UTC时间
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
		/// 必须为UTC时间
		/// </summary>
		[Column("UpdateOn")]
		public virtual DateTime? UpdateOn
        {
            get;
            set;
        }

	}
}
