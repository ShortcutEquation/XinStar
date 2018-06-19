using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using log4net.Config;
using System.IO;

namespace z.Foundation
{
    /// <summary>
    /// 日志记录器
    /// </summary>
    public class Logger
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        static Logger()
        {
            XmlConfigurator.Configure(new FileInfo(string.Format("{0}\\log4net.config", AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\'))));
        }

        private readonly static ILog DefaultLogger = LogManager.GetLogger("DefaultLogger");

        /// <summary>
        /// 记录级别为[Debug]的日志信息
        /// </summary>
        /// <param name="message">日志信息</param>
        public static void Debug(object message)
        {
            DefaultLogger.Debug(message);
        }

        /// <summary>
        /// 记录级别为[Debug]的日志信息
        /// </summary>
        /// <param name="message">日志信息</param>
        /// <param name="exception">异常信息</param>
        public static void Debug(object message, Exception exception)
        {
            DefaultLogger.Debug(message, exception);
        }

        /// <summary>
        /// 记录级别为[Error]的日志信息
        /// </summary>
        /// <param name="message">日志信息</param>
        public static void Error(object message)
        {
            DefaultLogger.Error(message);
        }

        /// <summary>
        /// 记录级别为[Error]的日志信息
        /// </summary>
        /// <param name="message">日志信息</param>
        /// <param name="exception">异常信息</param>
        public static void Error(object message, Exception exception)
        {
            DefaultLogger.Error(message, exception);
        }

        /// <summary>
        /// 记录级别为[Fatal]的日志信息
        /// </summary>
        /// <param name="message">日志信息</param>
        public static void Fatal(object message)
        {
            DefaultLogger.Fatal(message);
        }

        /// <summary>
        /// 记录级别为[Fatal]的日志信息
        /// </summary>
        /// <param name="message">日志信息</param>
        /// <param name="exception">异常信息</param>
        public static void Fatal(object message, Exception exception)
        {
            DefaultLogger.Fatal(message, exception);
        }

        /// <summary>
        /// 记录级别为[Info]的日志信息
        /// </summary>
        /// <param name="message">日志信息</param>
        public static void Info(object message)
        {
            DefaultLogger.Info(message);
        }

        /// <summary>
        /// 记录级别为[Info]的日志信息
        /// </summary>
        /// <param name="message">日志信息</param>
        /// <param name="exception">异常信息</param>
        public static void Info(object message, Exception exception)
        {
            DefaultLogger.Info(message, exception);
        }

        /// <summary>
        /// 记录级别为[Warn]的日志信息
        /// </summary>
        /// <param name="message">日志信息</param>
        public static void Warn(object message)
        {
            DefaultLogger.Warn(message);
        }

        /// <summary>
        /// 记录级别为[Warn]的日志信息
        /// </summary>
        /// <param name="message">日志信息</param>
        /// <param name="exception">异常信息</param>
        public static void Warn(object message, Exception exception)
        {
            DefaultLogger.Warn(message, exception);
        }
    }
}
