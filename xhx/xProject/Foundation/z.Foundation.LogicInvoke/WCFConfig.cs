using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace z.Foundation.LogicInvoke
{
    /// <summary>
    /// WCF方式调用配置类
    /// </summary>
    public class WCFConfig
    {
        /// <summary>
        /// WCF应用集合
        /// </summary>
        public List<WCFApp> Apps
        {
            get;
            set;
        }
    }

    /// <summary>
    /// WCF应用
    /// </summary>
    public class WCFApp
    {
        /// <summary>
        /// 应用键值：AssemblyName:FullClassName:MethodName
        /// </summary>
        public string Key
        {
            get;
            set;
        }

        /// <summary>
        /// 应用地址
        /// </summary>
        public string Address
        {
            get;
            set;
        }

        /// <summary>
        /// 绑定方式
        /// </summary>
        public string Binding
        {
            get;
            set;
        }
    }
}
