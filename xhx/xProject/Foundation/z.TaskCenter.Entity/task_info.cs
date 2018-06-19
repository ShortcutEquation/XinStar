using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using z.Foundation.Data;

namespace z.TaskCenter.Entity
{
	[Serializable, CustomData(ConnectionName = "TaskCenterDB"), Table("task_info")]
	public partial class task_info : EntityBase
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
		/// 总任务id，用于关联前端任务
		/// </summary>
		[Column("TaskMainId")]
		public virtual Int32 TaskMainId
        {
            get;
            set;
        }

		/// <summary>
		/// 任务内容
		/// </summary>
		[Column("Content")]
		public virtual String Content
        {
            get;
            set;
        }

		/// <summary>
		/// 任务类型，用代码表示
		/// </summary>
		[Column("TaskType")]
		public virtual String TaskType
        {
            get;
            set;
        }

		/// <summary>
		/// 任务状态（0待处理，1已发送客户端，2处理成功，-1失败）
		/// </summary>
		[Column("TaskStatus")]
		public virtual Int32 TaskStatus
        {
            get;
            set;
        }

		/// <summary>
		/// 父任务的编号
		/// </summary>
		[Column("ParentTaskId")]
		public virtual Int32 ParentTaskId
        {
            get;
            set;
        }

		/// <summary>
		/// 任务排序
		/// </summary>
		[Column("SortNo")]
		public virtual Int32 SortNo
        {
            get;
            set;
        }

		/// <summary>
		/// 任务创建时间
		/// </summary>
		[Column("CreatedOn")]
		public virtual DateTime? CreatedOn
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
		/// 重试几次
		/// </summary>
		[Column("RetryTimes")]
		public virtual Int32 RetryTimes
        {
            get;
            set;
        }

		/// <summary>
		/// 执行结果
		/// </summary>
		[Column("ExecuteResult")]
		public virtual String ExecuteResult
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

	}
}
