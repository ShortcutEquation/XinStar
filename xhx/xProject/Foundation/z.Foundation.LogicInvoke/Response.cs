using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace z.Foundation.LogicInvoke
{
    /// <summary>
    /// 逻辑调用结果集实体对象
    /// </summary>
    public class Response : IResponse
    {
        /// <summary>
        /// 执行状态: 成功/失败
        /// </summary>
        public bool Succeeded
        {
            get;
            set;
        }

        /// <summary>
        /// 返回信息
        /// </summary>
        public string Message
        {
            get;
            set;
        }

        /// <summary>
        /// 调试信息
        /// </summary>
        public string DebugMessage
        {
            get;
            set;
        }

        /// <summary>
        /// 异常对象
        /// </summary>
        public Exception Exception
        {
            get;
            set;
        }

        /// <summary>
        /// 结果对象
        /// </summary>
        public object Result
        {
            get;
            set;
        }
    }

    /// <summary>
    /// 逻辑调用结果集实体对象
    /// </summary>
    public class Response<T> : IResponse<T>
    {
        /// <summary>
        /// 执行状态: 成功/失败
        /// </summary>
        public bool Succeeded
        {
            get;
            set;
        }

        /// <summary>
        /// 返回信息
        /// </summary>
        public string Message
        {
            get;
            set;
        }

        /// <summary>
        /// 调试信息
        /// </summary>
        public string DebugMessage
        {
            get;
            set;
        }

        /// <summary>
        /// 异常对象
        /// </summary>
        public Exception Exception
        {
            get;
            set;
        }

        /// <summary>
        /// 结果对象
        /// </summary>
        public T Result
        {
            get;
            set;
        }
    }
}
