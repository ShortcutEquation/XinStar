using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using z.Foundation;
using z.Foundation.Data;
using z.Foundation.LogicInvoke;

namespace ServiceCenter
{
    public class Program : ConsoleBase
    {
        static string wcfAddress = Utility.GetConfigValue("WCFAddress");

        static void Main(string[] args)
        {
            //初始化 缓存Isession对象
            Console.WriteLine("Initialization...");
            InitializationSection initializationSection = (InitializationSection)ConfigurationManager.GetSection("Initialization");
            FunctionsSection functionsSection = initializationSection.FunctionsSetting;
            foreach (ItemSection item in functionsSection)
            {
                IResponse response = CallLogic(item.Assembly, item.Class, item.Method, item.Param);
            }

            StartHost();
        }

        static void StartHost()
        {
            try
            {
                BasicHttpBinding binding = new BasicHttpBinding()
                {
                    MaxReceivedMessageSize = 2147483647,
                    CloseTimeout = new TimeSpan(0, 30, 0),
                    OpenTimeout = new TimeSpan(0, 30, 0),
                    ReceiveTimeout = new TimeSpan(0, 30, 0),
                    SendTimeout = new TimeSpan(0, 30, 0),
                    //ReaderQuotas = new XmlDictionaryReaderQuotas()
                    //{
                    //    MaxStringContentLength = 2147483647
                    //},
                    Security = new BasicHttpSecurity()
                    {
                        Mode = BasicHttpSecurityMode.None
                    }
                };

                ServiceHost serviceHost = new ServiceHost(typeof(WCFService), new Uri(wcfAddress));
                serviceHost.AddServiceEndpoint(typeof(IWCFService), binding, "");
                ServiceThrottlingBehavior throttlingBehavior = serviceHost.Description.Behaviors.Find<ServiceThrottlingBehavior>();
                if (null == throttlingBehavior)
                {
                    throttlingBehavior = new ServiceThrottlingBehavior();
                    throttlingBehavior.MaxConcurrentCalls = 1000;
                    throttlingBehavior.MaxConcurrentInstances = 1000;
                    throttlingBehavior.MaxConcurrentSessions = 1000;
                    serviceHost.Description.Behaviors.Add(throttlingBehavior);
                }

                serviceHost.Open();
                ThreadPool.SetMinThreads(500, 500);
                Console.WriteLine(throttlingBehavior.MaxConcurrentCalls);
                Console.WriteLine(throttlingBehavior.MaxConcurrentInstances);
                Console.WriteLine(throttlingBehavior.MaxConcurrentSessions);

                Console.WriteLine("{0} Service start ...", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
                Console.WriteLine("");
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
                Console.WriteLine(ex.Message);
            }

            Console.ReadLine();
        }
    }
}
