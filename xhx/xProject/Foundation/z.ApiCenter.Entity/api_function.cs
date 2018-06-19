using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using z.Foundation.Data;

namespace z.ApiCenter.Entity
{
	[Serializable, CustomData(ConnectionName = "ApiCenterDB"), Table("api_function")]
	public partial class api_function : EntityBase
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
		/// 功能编码
		/// </summary>
		[Column("FunctionCode")]
		public virtual String FunctionCode
        {
            get;
            set;
        }

		/// <summary>
		/// 功能名称
		/// </summary>
		[Column("Name")]
		public virtual String Name
        {
            get;
            set;
        }

		/// <summary>
		/// 关联的系统id
		/// </summary>
		[Column("SystemId")]
		public virtual Int32 SystemId
        {
            get;
            set;
        }

		/// <summary>
		/// 功能上级id
		/// </summary>
		[Column("ParentId")]
		public virtual Int32 ParentId
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
