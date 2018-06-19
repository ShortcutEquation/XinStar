using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using z.Foundation.Data;

namespace z.UpdateCenter.Entity
{
	[Serializable, CustomData(ConnectionName = "UpdateCenterDB"), Table("update_pack_project")]
	public partial class update_pack_project : EntityBase
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
		/// 升级包编号
		/// </summary>
		[Column("PackId")]
		public virtual Int32 PackId
        {
            get;
            set;
        }

		/// <summary>
		/// 服务器编号
		/// </summary>
		[Column("ServerId")]
		public virtual Int32? ServerId
        {
            get;
            set;
        }

		/// <summary>
		/// 项目编号
		/// </summary>
		[Column("ProjectId")]
		public virtual Int32 ProjectId
        {
            get;
            set;
        }

		/// <summary>
		/// Status（0 待升级，1升级中，10升级完成，-1 升级失败）
		/// </summary>
		[Column("Status")]
		public virtual Int32 Status
        {
            get;
            set;
        }

		/// <summary>
		///
		/// </summary>
		[Column("Result")]
		public virtual String Result
        {
            get;
            set;
        }

		/// <summary>
		///
		/// </summary>
		[Column("CreateTime")]
		public virtual DateTime? CreateTime
        {
            get;
            set;
        }

		/// <summary>
		///
		/// </summary>
		[Column("FinishTime")]
		public virtual DateTime? FinishTime
        {
            get;
            set;
        }

	}
}
