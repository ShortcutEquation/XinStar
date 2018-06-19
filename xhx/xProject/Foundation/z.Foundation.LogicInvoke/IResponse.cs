using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace z.Foundation.LogicInvoke
{
    /// <summary>
    /// 逻辑调用结果集对象接口
    /// </summary>
    public interface IResponse
    {
        /// <summary>
        /// 执行状态: 成功/失败
        /// </summary>
        bool Succeeded
        {
            get;
            set;
        }

        /// <summary>
        /// 返回信息
        /// </summary>
        string Message
        {
            get;
            set;
        }

        /// <summary>
        /// 调试信息
        /// </summary>
        string DebugMessage
        {
            get;
            set;
        }

        /// <summary>
        /// 异常对象
        /// </summary>
        Exception Exception
        {
            get;
            set;
        }

        /// <summary>
        /// 结果对象
        /// </summary>
        object Result
        {
            get;
            set;
        }
    }

    /// <summary>
    /// 逻辑调用结果集对象接口
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IResponse<T>
    {
        /// <summary>
        /// 执行状态: 成功/失败
        /// </summary>
        bool Succeeded
        {
            get;
            set;
        }

        /// <summary>
        /// 返回信息
        /// </summary>
        string Message
        {
            get;
            set;
        }

        /// <summary>
        /// 调试信息
        /// </summary>
        string DebugMessage
        {
            get;
            set;
        }

        /// <summary>
        /// 异常对象
        /// </summary>
        Exception Exception
        {
            get;
            set;
        }

        /// <summary>
        /// 结果对象
        /// </summary>
        T Result
        {
            get;
            set;
        }
    }
}
