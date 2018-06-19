using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using z.Foundation.Data;

namespace com.star.Entity.StarDB
{
	[Serializable, CustomData(ConnectionName = "StarDB"), Table("test")]
	public partial class test : EntityBase
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
		[Column("Name")]
		public virtual String Name
        {
            get;
            set;
        }

		/// <summary>
		///
		/// </summary>
		[Column("Remark")]
		public virtual String Remark
        {
            get;
            set;
        }

		/// <summary>
		///
		/// </summary>
		[Column("CreateOn")]
		public virtual DateTime? CreateOn
        {
            get;
            set;
        }

	}
}
