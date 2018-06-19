using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace z.Foundation.LogicInvoke
{
    /// <summary>
    /// 逻辑调用入口
    /// </summary>
    public class LongLogicInvoker
    {
        /// <summary>
        /// 构造函数单一实例
        /// </summary>
        public static LongLogicInvoker Instance = new LongLogicInvoker();

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

        ILogicTunnel longLogicTunnel;

        public LongCallback longCallback
        {
            get;set;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public LongLogicInvoker()
        {
            ConfigFile = string.Format("{0}\\LogicInvoker.config", Utility.ApplicationPath());
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public LongLogicInvoker(LongCallback callback)
        {
            longCallback = callback;
            callback.invoker = this;
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
            //ILogicTunnel logicTunnel = new LocalLogicTunnel();
            switch (callingType)
            {
                case CallingType.Local:
                    longLogicTunnel = new LocalLogicTunnel();
                    break;
                case CallingType.Remoting:
                    longLogicTunnel = new RemotingLogicTunnel();
                    break;
                case CallingType.Wcf:
                    if (longLogicTunnel == null)
                    {
                        longLogicTunnel = new LongWCFLogicTunnel(longCallback);
                    }
                    break;
                case CallingType.Webservice:
                    longLogicTunnel = new WebServiceLogicTunnel();
                    break;
            }
            return longLogicTunnel;
        }

        /// <summary>
        /// 长连接操作完成通知操作
        /// </summary>
        public void LongTaskFinishNotify()
        {
            (longLogicTunnel as LongWCFLogicTunnel).FinishTask();
        }
    }
}
