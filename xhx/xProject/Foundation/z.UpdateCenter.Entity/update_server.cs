using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using z.Foundation.Data;

namespace z.UpdateCenter.Entity
{
	[Serializable, CustomData(ConnectionName = "UpdateCenterDB"), Table("update_server")]
	public partial class update_server : EntityBase
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
		/// 服务器名称
		/// </summary>
		[Column("ServerName")]
		public virtual String ServerName
        {
            get;
            set;
        }

		/// <summary>
		/// IP地址
		/// </summary>
		[Column("IpAddress")]
		public virtual String IpAddress
        {
            get;
            set;
        }

		/// <summary>
		///
		/// </summary>
		[Column("FtpUserName")]
		public virtual String FtpUserName
        {
            get;
            set;
        }

		/// <summary>
		///
		/// </summary>
		[Column("FtpUserPassword")]
		public virtual String FtpUserPassword
        {
            get;
            set;
        }

		/// <summary>
		///
		/// </summary>
		[Column("FtpPort")]
		public virtual Int32 FtpPort
        {
            get;
            set;
        }

		/// <summary>
		///
		/// </summary>
		[Column("FtpUploadPath")]
		public virtual String FtpUploadPath
        {
            get;
            set;
        }

		/// <summary>
		///
		/// </summary>
		[Column("FileFunctions")]
		public virtual String FileFunctions
        {
            get;
            set;
        }

		/// <summary>
		///
		/// </summary>
		[Column("FileMD5")]
		public virtual String FileMD5
        {
            get;
            set;
        }

		/// <summary>
		/// 压缩之前的内容大小
		/// </summary>
		[Column("FileLength")]
		public virtual Int64? FileLength
        {
            get;
            set;
        }

		/// <summary>
		/// 校验状态（0待校验，1校验成功，-1校验失败）
		/// </summary>
		[Column("CheckFileValid")]
		public virtual Int32? CheckFileValid
        {
            get;
            set;
        }

		/// <summary>
		/// 备份路径
		/// </summary>
		[Column("BackupPath")]
		public virtual String BackupPath
        {
            get;
            set;
        }

	}
}
