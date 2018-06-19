using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace z.Foundation.LogicInvoke
{
    [ServiceContract(SessionMode = SessionMode.Required, CallbackContract = typeof(ILongCallback))]
    public interface ILongWCFService
    {
        /// <summary>
        /// 执行长连接方法
        /// </summary>
        /// <param name="serializeString">序列化二进制字符串</param>
        /// <returns></returns>
        [System.ServiceModel.OperationContractAttribute(Action = "http://tempuri.org/ILongWCFService/Execute", Name = "Execute", IsOneWay = true)]
        void Execute(string serializeString);

        /// <summary>
        /// 执行短连接方法
        /// </summary>
        /// <param name="serializeString">序列化二进制字符串</param>
        /// <returns></returns>
        [System.ServiceModel.OperationContractAttribute(Action = "http://tempuri.org/ILongWCFService/ExecuteShort", ReplyAction = "http://tempuri.org/ILongWCFService/ExecuteShortResponse", Name = "ExecuteShort")]
        string ExecuteShort(string serializeString);


        /// <summary>
        /// 心跳检测
        /// </summary>
        /// <param name="key">序列化二进制字符串</param>
        /// <returns></returns>
        [System.ServiceModel.OperationContractAttribute(Action = "http://tempuri.org/ILongWCFService/UpdateAlive", Name = "UpdateAlive",IsOneWay = true)]
        void UpdateAlive(int maxThreadCount);

        /// <summary>
        /// 完成任务
        /// </summary>
        /// <param name="key">序列化二进制字符串</param>
        /// <returns></returns>
        [System.ServiceModel.OperationContractAttribute(Action = "http://tempuri.org/ILongWCFService/FinishTask", Name = "FinishTask", IsOneWay = true)]
        void FinishTask();
    }
}
