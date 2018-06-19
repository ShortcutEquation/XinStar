using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace z.Foundation.LogicInvoke
{
    /// <summary>
    /// WebService方式调用配置类
    /// </summary>
    public class WebServiceConfig
    {
        /// <summary>
        /// WebService应用集合
        /// </summary>
        public List<WebServiceApp> Apps
        {
            get;
            set;
        }
    }

    /// <summary>
    /// WebService应用
    /// </summary>
    public class WebServiceApp
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
    }
}
