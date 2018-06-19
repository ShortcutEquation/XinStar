using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace z.Foundation.Data
{
    /// <summary>
    /// 返回String类型结果集对象
    /// </summary>
    [Serializable]
    public class StringResult
    {
        /// <summary>
        /// 文本状态标识
        /// </summary>
        public string StatusCode
        {
            get;
            set;
        }

        /// <summary>
        /// 消息
        /// </summary>
        public string Message
        {
            get;
            set;
        }

        /// <summary>
        /// 返回结果集对象
        /// </summary>
        public object Result
        {
            get;
            set;
        }
    }

    /// <summary>
    /// 返回String类型结果集对象
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class StringResult<T>
    {
        /// <summary>
        /// 文本状态标识
        /// </summary>
        public string StatusCode
        {
            get;
            set;
        }

        /// <summary>
        /// 消息
        /// </summary>
        public string Message
        {
            get;
            set;
        }

        /// <summary>
        /// 返回结果集对象
        /// </summary>
        public T Result
        {
            get;
            set;
        }
    }
}
