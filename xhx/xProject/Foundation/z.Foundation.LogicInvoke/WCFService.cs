using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace z.Foundation.LogicInvoke
{
    /// <summary>
    /// WCF服务
    /// </summary>
    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    public class WCFService : IWCFService
    {
        /// <summary>
        /// 执行方法
        /// </summary>
        /// <param name="serializeString">序列化二进制字符串</param>
        /// <returns></returns>
        public string Execute(string serializeString)
        {
            IRequest request = serializeString.Deserialize<Request>();
            object obj = DynamicInvoke.Call(request);
            return obj == null ? "" : obj.Serialize<object>();
        }

        /// <summary>
        /// 执行方法（异步）
        /// </summary>
        /// <param name="serializeString">序列化二进制字符串</param>
        /// <returns></returns>
        public System.Threading.Tasks.Task<string> ExecuteAsync(string serializeString)
        {
            throw new NotImplementedException();
        }
    }
}
