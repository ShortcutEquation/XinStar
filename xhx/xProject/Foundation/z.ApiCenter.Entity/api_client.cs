using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using z.Foundation.Data;

namespace z.ApiCenter.Entity
{
	[Serializable, CustomData(ConnectionName = "ApiCenterDB"), Table("api_client")]
	public partial class api_client : EntityBase
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
		/// 客户名称
		/// </summary>
		[Column("Name")]
		public virtual String Name
        {
            get;
            set;
        }

		/// <summary>
		/// 客户的验证标识
		/// </summary>
		[Column("AccessKey")]
		public virtual String AccessKey
        {
            get;
            set;
        }

		/// <summary>
		/// 客户的验证密钥
		/// </summary>
		[Column("SecretKey")]
		public virtual String SecretKey
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
