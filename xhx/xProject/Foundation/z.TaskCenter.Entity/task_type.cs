using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using z.Foundation.Data;

namespace z.TaskCenter.Entity
{
	[Serializable, CustomData(ConnectionName = "TaskCenterDB"), Table("task_type")]
	public partial class task_type : EntityBase
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
		/// 任务类型编码
		/// </summary>
		[Column("TaskType")]
		public virtual String TaskType
        {
            get;
            set;
        }

		/// <summary>
		/// 任务类型名称
		/// </summary>
		[Column("Name")]
		public virtual String Name
        {
            get;
            set;
        }

		/// <summary>
		/// 业务类型（0默认，1导出）
		/// </summary>
		[Column("BussinessType")]
		public virtual Int32 BussinessType
        {
            get;
            set;
        }

	}
}
