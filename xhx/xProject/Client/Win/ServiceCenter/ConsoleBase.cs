using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using z.Foundation.LogicInvoke;

namespace ServiceCenter
{
    public class ConsoleBase
    {
        #region 业务逻辑调用方法

        /// <summary>
        /// 业务逻辑调用
        /// </summary>
        /// <param name="assemblyName">程序集名称(e.g. com.buygego.Logic.dll)</param>
        /// <param name="className">类名(e.g. com.buygego.Logic.ClassName)</param>
        /// <param name="methodName">方法名(e.g. MethodName)</param>
        /// <param name="param">参数</param>
        /// <returns></returns>
        public static IResponse CallLogic(string assemblyName, string className, string methodName, object param)
        {
            IRequest request = new Request()
            {
                Target = assemblyName,
                Class = className,
                Method = methodName,
                Parameter = param
            };
            return LogicInvoker.Instance.Call(request);
        }

        /// <summary>
        /// 业务逻辑调用
        /// </summary>
        /// <typeparam name="T1">参数类型</typeparam>
        /// <typeparam name="T2">结果集类型</typeparam>
        /// <param name="assemblyName">程序集名称(e.g. com.buygego.Logic.dll)</param>
        /// <param name="className">类名(e.g. com.buygego.Logic.ClassName)</param>
        /// <param name="methodName">方法名(e.g. MethodName)</param>
        /// <param name="param">参数</param>
        /// <returns></returns>
        public static IResponse<T2> CallLogic<T1, T2>(string assemblyName, string className, string methodName, T1 param)
        {
            IRequest<T1> request = new Request<T1>()
            {
                Target = assemblyName,
                Class = className,
                Method = methodName,
                Parameter = param
            };
            return LogicInvoker.Instance.Call<T1, T2>(request);
        }

        #endregion
    }
}
