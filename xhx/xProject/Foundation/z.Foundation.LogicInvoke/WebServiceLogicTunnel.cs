using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace z.Foundation.LogicInvoke
{
    /// <summary>
    /// WebService方式逻辑调用
    /// </summary>
    internal class WebServiceLogicTunnel : ILogicTunnel
    {
        /// <summary>
        /// 缓存配置文件的值
        /// </summary>
        private static Dictionary<string, WebServiceApp> dict;

        /// <summary>
        /// WEBService配置文件
        /// </summary>
        private string ConfigFile
        {
            get;
            set;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public WebServiceLogicTunnel()
        {
            ConfigFile = string.Format("{0}\\WebService.config", Utility.ApplicationPath());
        }

        /// <summary>
        /// 调用（非泛型）
        /// </summary>
        /// <param name="request">请求参数对象</param>
        /// <returns></returns>
        public IResponse Call(IRequest request)
        {
            IResponse response = new Response();

            try
            {
                string key = string.Format("{0}:{1}:{2}", request.Target, request.Class, request.Method);
                WebServiceApp webServiceApp = GetWebServiceApp(key);
                WebServiceServer webServiceServer = new WebServiceServer();
                webServiceServer.Url = webServiceApp.Address;
                Request model = (Request)request;
                string base64 = model.Serialize<Request>();
                string result = webServiceServer.Execute(base64);
                response.Result = result;
                response.Succeeded = true;
            }
            catch (Exception ex)
            {
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
            if (responseModel.Succeeded)
            {
                string result = responseModel.Result.ToString();
                if (!string.IsNullOrEmpty(result))
                {
                    response.Result = result.Deserialize<T2>();
                }
            }
            return response;
        }

        /// <summary>
        /// 获取WEBService调用所需的相关参数
        /// </summary>
        /// <param name="appName"></param>
        /// <returns></returns>
        private WebServiceApp GetWebServiceApp(string appName)
        {
            if (dict == null)
            {
                ConfigOperate<WebServiceConfig> model = new ConfigOperate<WebServiceConfig>(ConfigFile);
                WebServiceConfig webServiceConfig = model.Load();
                dict = webServiceConfig.Apps.ToDictionary(p => p.Key);
            }
            WebServiceApp webserviceApp = new WebServiceApp();
            if (dict.ContainsKey(appName))
            {
                webserviceApp = dict[appName];
            }
            else if (dict.ContainsKey("*"))
            {
                webserviceApp = dict["*"];
            }
            else
            {
                throw new Exception("App未找到对应的WEBService调用地址");
            }
            return webserviceApp;
        }
    }
}
