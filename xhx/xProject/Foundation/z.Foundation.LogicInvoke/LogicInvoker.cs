using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace z.Foundation.LogicInvoke
{
    /// <summary>
    /// 逻辑调用入口
    /// </summary>
    public class LogicInvoker
    {
        /// <summary>
        /// 构造函数单一实例
        /// </summary>
        public static LogicInvoker Instance = new LogicInvoker();

        /// <summary>
        /// 缓存配置文件的值
        /// </summary>
        private static Dictionary<string, App> dict;

        /// <summary>
        /// 配置文件路径
        /// </summary>
        private string ConfigFile
        {
            get;
            set;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        private LogicInvoker()
        {
            ConfigFile = string.Format("{0}\\LogicInvoker.config", Utility.ApplicationPath());
        }

        /// <summary>
        /// 调用（非泛型）
        /// </summary>
        /// <param name="request">请求参数对象</param>
        /// <returns></returns>
        public IResponse Call(IRequest request)
        {
            string key = string.Format("{0}:{1}:{2}", request.Target, request.Class, request.Method);
            ILogicTunnel logicTunnel = GetLogicTunnel(key);
            return logicTunnel.Call(request);
        }

        /// <summary>
        /// 调用（泛型）
        /// </summary>
        /// <typeparam name="T1">参数类型</typeparam>
        /// <typeparam name="T2">结果集类型</typeparam>
        /// <param name="request">请求参数对象</param>
        /// <returns></returns>
        public IResponse<T2> Call<T1, T2>(IRequest<T1> request)
        {
            string key = string.Format("{0}:{1}:{2}", request.Target, request.Class, request.Method);
            ILogicTunnel logicTunnel = GetLogicTunnel(key);
            return logicTunnel.Call<T1, T2>(request);
        }

        /// <summary>
        /// 获取逻辑调用管道
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private ILogicTunnel GetLogicTunnel(string key) 
        {
            if (dict == null)
            {
                ConfigOperate<LogicInvokerConfig> model = new ConfigOperate<LogicInvokerConfig>(ConfigFile);
                LogicInvokerConfig logicInvokerConfig = model.Load();
                dict = logicInvokerConfig.Apps.ToDictionary(p => p.Key);
            }
            CallingType callingType = CallingType.Wcf;
            if (dict.ContainsKey(key))
            {
                callingType = dict[key].CallType;
            }
            else if(dict.ContainsKey("*"))
            {
                callingType = dict["*"].CallType;
            }
            ILogicTunnel logicTunnel = new LocalLogicTunnel();
            switch (callingType)
            {
                case CallingType.Local:
                    logicTunnel = new LocalLogicTunnel();
                    break;
                case CallingType.Remoting:
                    logicTunnel = new RemotingLogicTunnel();
                    break;
                case CallingType.Wcf:
                    logicTunnel = new WCFLogicTunnel();
                    break;
                case CallingType.Webservice:
                    logicTunnel = new WebServiceLogicTunnel();
                    break;
            }
            return logicTunnel;
        }
    }
}
