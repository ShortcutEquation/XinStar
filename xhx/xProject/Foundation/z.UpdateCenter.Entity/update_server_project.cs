using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using z.Foundation.Data;

namespace z.UpdateCenter.Entity
{
	[Serializable, CustomData(ConnectionName = "UpdateCenterDB"), Table("update_server_project")]
	public partial class update_server_project : EntityBase
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
		/// 服务器编号
		/// </summary>
		[Column("ServerId")]
		public virtual Int32 ServerId
        {
            get;
            set;
        }

		/// <summary>
		/// 项目名称
		/// </summary>
		[Column("ProjectName")]
		public virtual String ProjectName
        {
            get;
            set;
        }

		/// <summary>
		/// 项目别名，用来更发布文件名对应
		/// </summary>
		[Column("AliasProjectName")]
		public virtual String AliasProjectName
        {
            get;
            set;
        }

		/// <summary>
		/// 项目路径
		/// </summary>
		[Column("ProjectPath")]
		public virtual String ProjectPath
        {
            get;
            set;
        }

		/// <summary>
		/// ProjectType(1 Web,2 Service)
		/// </summary>
		[Column("ProjectType")]
		public virtual Int32 ProjectType
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

	}
}
