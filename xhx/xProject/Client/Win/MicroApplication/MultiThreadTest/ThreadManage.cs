using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MultiThreadTest
{
    class ThreadManage
    {
        private static readonly Queue<int> intQueue = new Queue<int>();
        private static Thread stockChangeThread;
        private static int count = 10;

        static ThreadManage()
        {
            count++;

            stockChangeThread = new Thread(Process);
            stockChangeThread.IsBackground = true;
            stockChangeThread.Start();
        }

        public static void Do()
        {
            intQueue.Enqueue(count);
            Console.WriteLine(intQueue.Count);

            //创建Task
            //for (var i = 0; i < 5; i++)
            //{
            //    var temp = i;
            //    Task.Run(() => Add(temp));
            //    Thread.Sleep(1000);
            //}

            //Console.ReadLine();
            Thread.Sleep(1000);
        }

        public void Add(int i)
        {
            Process _proc = new Process();
            ProcessStartInfo _procStartInfo = new ProcessStartInfo("IExplore.exe", "http://www.baidu.com");
            _proc.StartInfo = _procStartInfo;
            _proc.Start();

         
            if(_proc.WaitForExit(-1)); //等待退出
            Console.WriteLine(Thread.CurrentThread.ManagedThreadId.ToString() + "---" + i);
            //Thread.Sleep(1000);
        }

        private static void Process()
        {
            while (true)
            {
                try
                {
                   var enti= intQueue.Dequeue(); Console.WriteLine("dd"+ enti*2);
                }
                catch (Exception ex)
                {
                   // Logger.Error("stock error:", ex);
                }
                finally
                {
                   // hasNew.Set();
                }
            }
        }

    }
}
