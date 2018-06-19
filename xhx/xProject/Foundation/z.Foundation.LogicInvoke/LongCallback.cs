using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace z.Foundation.LogicInvoke
{
    public class LongCallback : ILongCallback
    {
        public LongLogicInvoker invoker
        {
            get;set;
        }

        public int MaxThreadCount
        {
            get; set;
        }

        public DateTime ReconnectTime
        {
            get;set;
        }

        public bool IsReconnect
        {
            get;set;
        }

        public virtual void Callback(object obj)
        {

        }

        public virtual void Completed(object obj)
        {

        }

        public void CallbackEnd()
        {
            invoker.LongTaskFinishNotify();
            Console.WriteLine("任务完成");
        }

        public virtual void Heartbeat()
        {
            IsReconnect = true;
            Console.WriteLine("来自服务端的心跳检测");
        }
    }
}
