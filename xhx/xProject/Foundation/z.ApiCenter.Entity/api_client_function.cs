using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using z.Foundation.Data;

namespace z.ApiCenter.Entity
{
	[Serializable, CustomData(ConnectionName = "ApiCenterDB"), Table("api_client_function")]
	public partial class api_client_function : EntityBase
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
		[Column("ClientId")]
		public virtual Int32 ClientId
        {
            get;
            set;
        }

		/// <summary>
		///
		/// </summary>
		[Column("FunctionId")]
		public virtual Int32 FunctionId
        {
            get;
            set;
        }

	}
}
