using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace z.Foundation.LogicInvoke
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class LongWCFService : ILongWCFService
    {
        public static Dictionary<string, LongClientConfig> ClientServers = new Dictionary<string, LongClientConfig>();
        public static object lockCientServer = new object();

        #region 服务实现方法

        public void Execute(string serializeString)
        {
            IRequest request = serializeString.Deserialize<Request>();
            object obj = DynamicInvoke.Call(request);  
        }

        public string ExecuteShort(string serializeString)
        {
            IRequest request = serializeString.Deserialize<Request>();
            object obj = DynamicInvoke.Call(request);
            return obj == null ? "" : obj.Serialize<object>();
        }

        public void UpdateAlive(int maxThreadCount)
        {
            string key = OperationContext.Current.SessionId;
            lock (lockCientServer)
            {
                if (!ClientServers.ContainsKey(key))
                {
                    ClientServers.Add(key, new LongClientConfig() { AliveTime = DateTime.Now, MaxThreadCount = 0, LeftThreadCount = 0 });
                }
                ClientServers[key].AliveTime = DateTime.Now;
                ClientServers[key].LeftThreadCount += maxThreadCount - ClientServers[key].MaxThreadCount;
                ClientServers[key].MaxThreadCount = maxThreadCount;

                //回调给客户端，说明客户端可用，如果服务端已经将客户端踢下线，则客户端需要重新连服务
                if (!LongWCFService.IsAlive())
                {
                    //ILongCallback longCallback = OperationContext.Current.GetCallbackChannel<ILongCallback>();
                    //longCallback.Heartbeat();
                }

                Console.WriteLine("From Client Check："+ ClientIpAndPort()+ ","+ key + DateTime.Now);
            }
        }

        public void FinishTask()
        {
            string key = OperationContext.Current.SessionId;
            lock (lockCientServer)
            {
                if (ClientServers.ContainsKey(key))
                {
                    ClientServers[key].LeftThreadCount++;
                }
            }
        }

        #endregion

        #region Logic调用
        public static bool IsAlive()
        {
            string key = OperationContext.Current.SessionId;
            lock (lockCientServer)
            {
                if (ClientServers.ContainsKey(key))
                {
                    if (ClientServers[key].AliveTime.AddSeconds(30) < DateTime.Now)
                    {
                        return false;
                    }
                    return true;
                }
            }
            return false;
        }

        public static void InitKey()
        {
            string key = OperationContext.Current.SessionId;
            lock (lockCientServer)
            {
                if (!ClientServers.ContainsKey(key))
                {
                    ClientServers.Add(key, new LongClientConfig() { AliveTime = DateTime.Now, MaxThreadCount = 0, LeftThreadCount = 0 });
                }
            }
        }

        public static void CallTask(Func<object, object> func, Func<object, object> completefunc, Func<object, bool> checkResult, Action<object> funcFail, Action<object> funcSendClient, object para)
        {
            string clientKey = OperationContext.Current.SessionId;
            InitKey();

            Console.WriteLine("客户端"+ ClientIpAndPort() + clientKey + "上线。。。");
            while (true)
            {
                //客户端检测
                if (!LongWCFService.IsAlive())
                {
                    Console.WriteLine("客户端" + clientKey + "掉线");
                    return;
                }

                if (LongWCFService.ClientServers[clientKey].LeftThreadCount <= 0)
                {
                    //超过任务最大线程数
                    Thread.Sleep(100);
                    continue;
                }

                //获取完成的任务
                object completeFuncResult = null;
                completeFuncResult = completefunc(para);
                if (completeFuncResult != null)
                {
                    //任务发送给客户端
                    Console.WriteLine("开始Complete回调..." + clientKey);
                    try
                    {
                        ILongCallback longCallback = OperationContext.Current.GetCallbackChannel<ILongCallback>();
                        longCallback.Completed(completeFuncResult.Serialize());
                    }
                    catch (Exception ex)
                    {
                        Logger.Error("Complete回调异常", ex);
                        Console.WriteLine("Complete回调异常");
                    }
                    Console.WriteLine("Complete回调成功");
                }

                //获取任务
                object funcResult=null;
                try
                {
                    funcResult = func(para);
                    if (!checkResult(funcResult))
                    {
                        //获取任务失败
                        Thread.Sleep(100);
                        continue;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("获取任务异常:" + ex.Message);
                    Logger.Error(ex);
                    continue;
                }

                Console.WriteLine("push task:" + DateTime.Now);

                try
                {
                    //可用线程数据减少
                    LongWCFService.ClientServers[clientKey].LeftThreadCount--;

                    //任务发送给客户端
                    Console.WriteLine("开始回调..." + clientKey);
                    funcSendClient?.Invoke(funcResult);
                    ILongCallback longCallback = OperationContext.Current.GetCallbackChannel<ILongCallback>();
                    longCallback.Callback(funcResult.Serialize());
                    Console.WriteLine("回调成功");
                    //Thread.Sleep(100);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                    //还原线程数
                    LongWCFService.ClientServers[clientKey].LeftThreadCount++;
                    Console.WriteLine("回调失败");
                    funcFail?.Invoke(funcResult);
                }
            };
        }

        public static string ClientIpAndPort()
        {
            OperationContext context = OperationContext.Current;
            MessageProperties properties = context.IncomingMessageProperties;
            RemoteEndpointMessageProperty endpoint = properties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
            return endpoint.Address + ":" + endpoint.Port.ToString();
        }

        #endregion
    }
}
