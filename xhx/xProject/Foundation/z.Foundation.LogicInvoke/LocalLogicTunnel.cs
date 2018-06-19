using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace z.Foundation.LogicInvoke
{
    /// <summary>
    /// 本地逻辑调用
    /// </summary>
    internal class LocalLogicTunnel : ILogicTunnel
    {
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
                response.Result = DynamicInvoke.Call(request);
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
            IResponse<T2> response = new Response<T2>();
            try
            {
                response.Result = (T2)DynamicInvoke.Call<T1>(request);  
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
    }
}
