using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace z.Foundation.LogicInvoke
{
    /// <summary>
    /// 逻辑调用入口配置类
    /// </summary>
    public class LogicInvokerConfig
    {
        /// <summary>
        /// 应用集合
        /// </summary>
        public List<App> Apps
        {
            get;
            set;
        }
    }

    /// <summary>
    /// 应用
    /// </summary>
    public class App
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
        /// 调用方式
        /// </summary>
        public CallingType CallType
        {
            get;
            set;
        }
    }

    /// <summary>
    /// 调用方式
    /// </summary>
    public enum CallingType
    {
        /// <summary>
        /// WCF方式
        /// </summary>
        Wcf,
        /// <summary>
        /// Remoting方式
        /// </summary>
        Remoting,
        /// <summary>
        /// WebService方式
        /// </summary>
        Webservice,
        /// <summary>
        /// 本地调用方式
        /// </summary>
        Local
    }
}
