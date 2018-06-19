using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace z.Foundation.Data
{
    /// <summary>
    /// 返回Bool类型结果集对象
    /// </summary>
    [Serializable]
    public class BoolResult
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool Succeeded
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
    /// 返回Bool类型结果集对象
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class BoolResult<T>
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool Succeeded
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
