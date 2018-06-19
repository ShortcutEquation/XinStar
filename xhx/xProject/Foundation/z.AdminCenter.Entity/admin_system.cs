using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using z.Foundation.Data;

namespace z.AdminCenter.Entity
{
	[Serializable, CustomData(ConnectionName = "AdminCenterDB"), Table("admin_system")]
	public partial class admin_system : EntityBase
    {
		/// <summary>
		///
		/// </summary>
		[Key, Column("AdminSystemId")]
		public virtual Int32 AdminSystemId
        {
            get;
            set;
        }

		/// <summary>
		///
		/// </summary>
		[Column("SysKey")]
		public virtual String SysKey
        {
            get;
            set;
        }

		/// <summary>
		///
		/// </summary>
		[Column("Code")]
		public virtual String Code
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
		[Column("URL")]
		public virtual String URL
        {
            get;
            set;
        }

		/// <summary>
		///
		/// </summary>
		[Column("CallBackUrl")]
		public virtual String CallBackUrl
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
