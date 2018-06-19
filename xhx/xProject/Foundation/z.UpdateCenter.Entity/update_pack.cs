using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using z.Foundation.Data;

namespace z.UpdateCenter.Entity
{
	[Serializable, CustomData(ConnectionName = "UpdateCenterDB"), Table("update_pack")]
	public partial class update_pack : EntityBase
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
		/// 升级包版本号
		/// </summary>
		[Column("Version")]
		public virtual String Version
        {
            get;
            set;
        }

		/// <summary>
		/// Status(0 待升级，1上传中，2上传成功，3 服务端可以开始执行升级包，4 升级中，10 升级完成，-1 升级失败)
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
		[Column("CreateTime")]
		public virtual DateTime? CreateTime
        {
            get;
            set;
        }

		/// <summary>
		///
		/// </summary>
		[Column("UploadFinishTime")]
		public virtual DateTime? UploadFinishTime
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
