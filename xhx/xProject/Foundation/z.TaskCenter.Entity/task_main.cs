using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using z.Foundation.Data;

namespace z.TaskCenter.Entity
{
	[Serializable, CustomData(ConnectionName = "TaskCenterDB"), Table("task_main")]
	public partial class task_main : EntityBase
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
		/// 系统类型（1分销商、2供应商、3erp、4eim、5api）
		/// </summary>
		[Column("SystemType")]
		public virtual Int32 SystemType
        {
            get;
            set;
        }

		/// <summary>
		/// 任务类型代码
		/// </summary>
		[Column("TaskType")]
		public virtual String TaskType
        {
            get;
            set;
        }

		/// <summary>
		/// 0待开始，1正在进行，2已经完成，-1失败
		/// </summary>
		[Column("TaskStatus")]
		public virtual Int32 TaskStatus
        {
            get;
            set;
        }

		/// <summary>
		/// 异常状态（0无异常、1有异常、2异常已经处理）
		/// </summary>
		[Column("AbnormalStatus")]
		public virtual Int32 AbnormalStatus
        {
            get;
            set;
        }

		/// <summary>
		/// 导出合并完成
		/// </summary>
		[Column("IsExportCompleted")]
		public virtual Boolean IsExportCompleted
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
		/// 完成时间
		/// </summary>
		[Column("FinishedOn")]
		public virtual DateTime? FinishedOn
        {
            get;
            set;
        }

		/// <summary>
		/// 总数
		/// </summary>
		[Column("TotalCount")]
		public virtual Int32 TotalCount
        {
            get;
            set;
        }

		/// <summary>
		/// 已完成数
		/// </summary>
		[Column("SuccessCount")]
		public virtual Int32 SuccessCount
        {
            get;
            set;
        }

		/// <summary>
		/// 失败数
		/// </summary>
		[Column("FailCount")]
		public virtual Int32 FailCount
        {
            get;
            set;
        }

		/// <summary>
		/// 用户自定义内容
		/// </summary>
		[Column("CustomContent")]
		public virtual String CustomContent
        {
            get;
            set;
        }

		/// <summary>
		/// 任务公共内容
		/// </summary>
		[Column("Content")]
		public virtual String Content
        {
            get;
            set;
        }

	}
}
