using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace z.Foundation.LogicInvoke
{
    /// <summary>
    /// WCF服务契约
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName = "IWCFService")]
    public interface IWCFService
    {
        /// <summary>
        /// 执行方法
        /// </summary>
        /// <param name="serializeString">序列化二进制字符串</param>
        /// <returns></returns>
        [System.ServiceModel.OperationContractAttribute(Action = "http://tempuri.org/IWCFService/Execute", ReplyAction = "http://tempuri.org/IWCFService/ExecuteResponse", Name = "Execute")]
        string Execute(string serializeString);

        /// <summary>
        /// 执行方法（异步）
        /// </summary>
        /// <param name="serializeString">序列化二进制字符串</param>
        /// <returns></returns>
        [System.ServiceModel.OperationContractAttribute(Action = "http://tempuri.org/IWCFService/ExecuteAsync", ReplyAction = "http://tempuri.org/IWCFService/ExecuteAsyncResponse", Name = "ExecuteAsync")]
        System.Threading.Tasks.Task<string> ExecuteAsync(string serializeString);
    }
}
