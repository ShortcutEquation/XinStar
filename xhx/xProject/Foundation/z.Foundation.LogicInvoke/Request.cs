using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace z.Foundation.LogicInvoke
{
    /// <summary>
    /// 逻辑调用参数实体对象
    /// </summary>
    [Serializable]
    public class Request : IRequest
    {
        /// <summary>
        /// 请求目标或者程序集名称
        /// </summary>
        public string Target
        {
            get;
            set;
        }

        /// <summary>
        /// 逻辑处理类名称
        /// </summary>
        public string Class
        {
            get;
            set;
        }

        /// <summary>
        /// 方法名称
        /// </summary>
        public string Method
        {
            get;
            set;
        }

        /// <summary>
        /// object类型参数对象
        /// </summary>
        public object Parameter
        {
            get;
            set;
        }

        /// <summary>
        /// 服务地址（非正式失效，不建议使用）
        /// </summary>
        public string Address
        {
            get;
            set;
        }

        /// <summary>
        /// WCF绑定协议（非正式失效，不建议使用）
        /// </summary>
        public string Binding
        {
            get;
            set;
        }
    }

    /// <summary>
    /// 逻辑调用参数实体对象
    /// </summary>
    public class Request<T> : IRequest<T>
    {
        /// <summary>
        /// 请求目标或者程序集名称
        /// </summary>
        public string Target
        {
            get;
            set;
        }

        /// <summary>
        /// 逻辑处理类名称
        /// </summary>
        public string Class
        {
            get;
            set;
        }

        /// <summary>
        /// 方法名称
        /// </summary>
        public string Method
        {
            get;
            set;
        }

        /// <summary>
        /// 泛型类型参数对象
        /// </summary>
        public T Parameter
        {
            get;
            set;
        }

        /// <summary>
        /// 服务地址（非正式失效，不建议使用）
        /// </summary>
        public string Address
        {
            get;
            set;
        }

        /// <summary>
        /// WCF绑定协议（非正式失效，不建议使用）
        /// </summary>
        public string Binding
        {
            get;
            set;
        }
    }
}
