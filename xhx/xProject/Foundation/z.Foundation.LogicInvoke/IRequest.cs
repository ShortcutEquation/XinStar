using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace z.Foundation.LogicInvoke
{
    /// <summary>
    /// 逻辑调用参数对象接口
    /// </summary>
    public interface IRequest
    {
        /// <summary>
        /// 请求目标或者程序集名称
        /// </summary>
        string Target
        {
            get;
            set;
        }

        /// <summary>
        /// 逻辑处理类名称
        /// </summary>
        string Class
        {
            get;
            set;
        }

        /// <summary>
        /// 方法名称
        /// </summary>
        string Method
        {
            get;
            set;
        }

        /// <summary>
        /// object类型参数对象
        /// </summary>
        object Parameter
        {
            get;
            set;
        }
    }

    /// <summary>
    /// 逻辑调用参数对象接口
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IRequest<T>
    {
        /// <summary>
        /// 请求目标或者程序集名称
        /// </summary>
        string Target
        {
            get;
            set;
        }

        /// <summary>
        /// 逻辑处理类名称
        /// </summary>
        string Class
        {
            get;
            set;
        }

        /// <summary>
        /// 方法名称
        /// </summary>
        string Method
        {
            get;
            set;
        }

        /// <summary>
        /// 泛型类型参数对象
        /// </summary>
        T Parameter
        {
            get;
            set;
        }
    }
}
