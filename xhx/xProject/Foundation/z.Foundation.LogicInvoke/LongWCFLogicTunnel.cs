using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Xml;
using System.Threading.Tasks;
using System.Threading;

namespace z.Foundation.LogicInvoke
{
    /// <summary>
    /// WCF方式逻辑调用
    /// </summary>
    internal class LongWCFLogicTunnel : ILogicTunnel
    {
        /// <summary>
        /// 缓存配置文件的值
        /// </summary>
        private static Dictionary<string, WCFApp> dict;

        /// <summary>
        /// WCF配置文件
        /// </summary>
        private string ConfigFile
        {
            get;
            set;
        }

        //心跳检测频率
        private static int ALIVE_CHECK_INTERVAL = 3000;

        //重试时间
        private static int[] SERVICE_RETRY_TIMES = new int[] { 1000, 2000, 5000, 10000, 20000, 40000, 60000, 120000, 240000, 400000 };

        //长连接实例
        LongWCFServiceClient longService;

        //长连接请求参数
        Request longRequest;

        //长连接回调
        public LongCallback longCallback
        {
            get; set;
        }

        private static readonly object objLockCheck = new object();

        //心跳检测线程
        private Thread thUpdateAlive;

        /// <summary>
        /// 构造函数
        /// </summary>
        public LongWCFLogicTunnel(LongCallback longCallback)
        {
            ConfigFile = string.Format("{0}\\WCF.config", Utility.ApplicationPath());
            this.longCallback = longCallback;

            if (IsLongCallback())
            {
                //长连接进行心跳检测
                thUpdateAlive = new Thread(new ThreadStart(UpdateAliveMethod));
                thUpdateAlive.Start();
            }
        }

        /// <summary>
        /// 检测心跳
        /// </summary>
        public void UpdateAliveMethod()
        {
            //longCallback.IsReconnect = false;
            //longCallback.ReconnectTime = DateTime.MinValue;
            while (true)
            {
                if (longService != null)
                {
                    Console.WriteLine("心跳检测开始" + DateTime.Now);
                    try
                    {
                        longService.UpdateAlive(longCallback.MaxThreadCount);
                        //if (longCallback.IsReconnect && DateTime.Now.Subtract(longCallback.ReconnectTime).TotalSeconds > 10)
                        //{
                        //    //来自服务器心跳检测，并重连服务器
                        //    longCallback.ReconnectTime = DateTime.Now;
                        //    longCallback.IsReconnect = false;
                        //    ReConnectServie();
                        //}
                    }
                    catch(Exception ex)
                    {
                        Logger.Error(ex);
                        //重连
                        ReConnectServie();
                    }
                    Console.WriteLine("心跳检测结束" + DateTime.Now);
                    Thread.Sleep(ALIVE_CHECK_INTERVAL);
                }
            }
        }

        /// <summary>
        /// 完成任务，释放线程数
        /// </summary>
        public void FinishTask()
        {
            int nTrytimes = 3;
            while (nTrytimes-- > 0)
            {
                try
                {
                    longService.FinishTask();
                    break;
                }
                catch
                {
                    Thread.Sleep(1500);
                }
            }
        }

        /// <summary>
        /// 重试10次
        /// </summary>
        private void ReConnectServie()
        {
            int i = 0;
            while (i < 10)
            {
                Console.WriteLine("正在进行" + (i + 1) + "次重连服务器...");
                try
                {
                    if (longService != null)
                    {
                        longService.Abort();
                        longService.Close();
                    }
                    longService = null;
                    LongWCFServiceClient serviceClient = GetServiceClient(longRequest);
                    serviceClient.UpdateAlive(longCallback.MaxThreadCount);
                    Call(longRequest);
                    Console.WriteLine("重连成功...");
                    break;
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                    Console.WriteLine((i + 1) + "次重连失败,等待下次重试...");
                    Thread.Sleep(SERVICE_RETRY_TIMES[i]);
                }
                i++;
            }
        }

        private LongWCFServiceClient GetServiceClient(Request req, bool isLong = true)
        {
            WCFApp wcfApp = new WCFApp();
            LongWCFServiceClient wcfServiveClient = null;

            if (!string.IsNullOrEmpty(req.Address))
            {
                wcfApp = new WCFApp() { Address = req.Address, Binding = string.IsNullOrEmpty(req.Binding) ? "WSDualHttpBinding" : req.Binding };
            }
            else
            {
                string key = string.Format("{0}:{1}:{2}", req.Target, req.Class, req.Method);
                wcfApp = GetWcfApp(key);
            }

            string serviceKey = wcfApp.Address + wcfApp.Binding;

            if (longService == null || !isLong)
            {
                //WCF调用
                Binding binding = null;
                switch (wcfApp.Binding)
                {
                    case "WSDualHttpBinding":
                        binding = new WSDualHttpBinding()
                        {
                            MaxReceivedMessageSize = 2147483647,
                            OpenTimeout = new TimeSpan(0, 30, 0),
                            CloseTimeout = new TimeSpan(0, 30, 0),
                            ReceiveTimeout = new TimeSpan(0, 30, 0),
                            SendTimeout = new TimeSpan(0, 0, 30),
                            Security = new WSDualHttpSecurity()
                            {
                                Mode = WSDualHttpSecurityMode.None
                            },

                        };
                        break;
                    case "NetTcpBinding":
                    default:
                        binding = new NetTcpBinding()
                        {
                            MaxReceivedMessageSize = 2147483647,
                            OpenTimeout = new TimeSpan(0, 30, 0),
                            CloseTimeout = new TimeSpan(0, 30, 0),
                            ReceiveTimeout = new TimeSpan(0, 0, 15),
                            SendTimeout = new TimeSpan(int.MaxValue),
                            Security = new NetTcpSecurity()
                            {
                                Mode = SecurityMode.None
                            }
                        };
                        break;
                }

                InstanceContext clientContext;
                if (longCallback == null)
                {
                    clientContext = new InstanceContext(new LongDefaultCallback());
                }
                else
                {
                    clientContext = new InstanceContext(longCallback);
                }

                wcfServiveClient = new LongWCFServiceClient(clientContext, binding, new EndpointAddress(wcfApp.Address));
                longService = wcfServiveClient;
                if (isLong)
                {
                    longRequest = req;
                }
            }
            else
            {
                wcfServiveClient = longService;
            }

            return wcfServiveClient;
        }

        private bool IsLongCallback()
        {
            if (longCallback != null && longCallback.GetType() != typeof(LongDefaultCallback))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 调用（非泛型）
        /// </summary>
        /// <param name="request">请求参数对象</param>
        /// <returns></returns>
        public IResponse Call(IRequest request)
        {
            IResponse response = new Response();
            Request req = (Request)request;
            try
            {
                if (!IsLongCallback())
                {
                    //默认使用短连接
                    using (LongWCFServiceClient service = GetServiceClient(req, false))
                    {
                        Request model = (Request)request;
                        string base64 = model.Serialize<Request>();
                        response.Result = service.ExecuteShort(base64);
                    }
                }
                else
                {
                    //使用长连接
                    LongWCFServiceClient service = GetServiceClient(req);
                    Request model = (Request)request;
                    string base64 = model.Serialize<Request>();
                    service.Execute(base64);
                }
                
                response.Succeeded = true;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                response.Succeeded = false;
                response.Message = (ex.InnerException == null ? ex.Message : ex.InnerException.Message) + string.Format("\r\n调用:Target:{0} | Class:{1} | Method:{2} | Parameter:{3}", request.Target, request.Class, request.Method, request.Parameter == null ? "参数为null" : "参数不为null");
                response.Exception = ex;

                Logger.Error(response.Message, ex);
            }

            return response;
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
            Request requestModel = new Request();
            requestModel.Target = request.Target;
            requestModel.Class = request.Class;
            requestModel.Method = request.Method;
            requestModel.Parameter = request.Parameter;
            IResponse responseModel = Call(requestModel);
            IResponse<T2> response = new Response<T2>();
            response.Succeeded = responseModel.Succeeded;
            response.Message = responseModel.Message;
            response.DebugMessage = responseModel.DebugMessage;
            response.Exception = responseModel.Exception;
            try
            {
                if (responseModel.Succeeded)
                {
                    string result = responseModel.Result.ToString();
                    if (!string.IsNullOrEmpty(result))
                    {
                        response.Result = result.Deserialize<T2>();
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return response;
        }

        /// <summary>
        /// 获取WCF调用所需的相关参数
        /// </summary>
        /// <param name="appName"></param>
        /// <returns></returns>
        private WCFApp GetWcfApp(string appName)
        {
            if (dict == null)
            {
                ConfigOperate<WCFConfig> model = new ConfigOperate<WCFConfig>(ConfigFile);
                WCFConfig wcfConfig = model.Load();
                dict = wcfConfig.Apps.ToDictionary(p => p.Key);
            }
            WCFApp wcfApp = new WCFApp();
            if (dict.ContainsKey(appName))
            {
                wcfApp = dict[appName];
            }
            else if (dict.ContainsKey("*"))
            {
                wcfApp = dict["*"];
            }
            else
            {
                throw new Exception("App未找到对应的WCF调用地址");
            }
            return wcfApp;
        }
    }
}
