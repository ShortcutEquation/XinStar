using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace z.Foundation.LogicInvoke
{
    /// <summary>
    /// Remoting方式逻辑调用
    /// </summary>
    internal class RemotingLogicTunnel : ILogicTunnel
    {
        /// <summary>
        /// 调用（非泛型）
        /// </summary>
        /// <param name="request">请求参数对象</param>
        /// <returns></returns>
        public IResponse Call(IRequest request)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }
    }
}
