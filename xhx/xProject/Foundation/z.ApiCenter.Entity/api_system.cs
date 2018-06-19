using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using z.Foundation.Data;

namespace z.ApiCenter.Entity
{
	[Serializable, CustomData(ConnectionName = "ApiCenterDB"), Table("api_system")]
	public partial class api_system : EntityBase
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
		/// 系统代码
		/// </summary>
		[Column("SystemCode")]
		public virtual String SystemCode
        {
            get;
            set;
        }

		/// <summary>
		/// 系统名称
		/// </summary>
		[Column("Name")]
		public virtual String Name
        {
            get;
            set;
        }

		/// <summary>
		/// 描述
		/// </summary>
		[Column("Description")]
		public virtual String Description
        {
            get;
            set;
        }

		/// <summary>
		/// 系统的网站地址
		/// </summary>
		[Column("Url")]
		public virtual String Url
        {
            get;
            set;
        }

		/// <summary>
		/// 创建人
		/// </summary>
		[Column("CreatedBy")]
		public virtual String CreatedBy
        {
            get;
            set;
        }

		/// <summary>
		/// 创建时间
		/// </summary>
		[Column("CreatedOn")]
		public virtual DateTime CreatedOn
        {
            get;
            set;
        }

		/// <summary>
		/// 是否删除
		/// </summary>
		[Column("Deleted")]
		public virtual Boolean Deleted
        {
            get;
            set;
        }

		/// <summary>
		/// 更新人
		/// </summary>
		[Column("UpdatedBy")]
		public virtual String UpdatedBy
        {
            get;
            set;
        }

		/// <summary>
		/// 更新时间
		/// </summary>
		[Column("UpdatedOn")]
		public virtual DateTime? UpdatedOn
        {
            get;
            set;
        }

	}
}
