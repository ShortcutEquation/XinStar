using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace z.Foundation
{
    /// <summary>
    /// 公共方法
    /// </summary>
    public class Utility
    {
        /// <summary>
        /// 获取应用程序动态链结库文件物理路径（包括WebSite、Service等），结尾不带斜杠"/"
        /// </summary>
        /// <returns></returns>
        public static string AssemblyPath()
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "Bin";  // Web Site

            if (Environment.CurrentDirectory == AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\')) // Others
            {
                path = AppDomain.CurrentDomain.BaseDirectory;
            }

            if (Environment.CurrentDirectory.Equals(@"C:\WINDOWS\system32")) // Service
            {
                path = AppDomain.CurrentDomain.BaseDirectory;
            }

            return path.TrimEnd('\\');
        }

        /// <summary>
        /// 获取应用程序根目录物理路径（包括WebSite、Service等），结尾不带斜杠"/"
        /// </summary>
        /// <returns></returns>
        public static string ApplicationPath()
        {
            string path = AppDomain.CurrentDomain.BaseDirectory;
            path = path.TrimEnd('\\');
            return path;
        }

        /// <summary>
        /// 获取配置文件appSettings中节点的值
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns></returns>
        public static string GetConfigValue(string key)
        {
            if (ConfigurationManager.AppSettings.AllKeys.Contains(key))
            {
                return ConfigurationManager.AppSettings[key].ToString();
            }
            else
            {
                throw (new Exception(string.Format("appSettings节中未找到key=\"{0}\"节点", key))); ;
            }
        }

        /// <summary>
        /// 获取配置文件connectionStrings中节点的值
        /// </summary>
        /// <param name="name">Name</param>
        /// <returns></returns>
        public static string GetConnectionString(string name)
        {
            return ConfigurationManager.ConnectionStrings[name].ToString();
        }

        /// <summary>
        /// 获取客户端IP地址（无视代理）
        /// </summary>
        /// <returns>若失败则返回回送地址</returns>
        public static string GetHostAddress()
        {
            string userHostAddress = HttpContext.Current.Request.UserHostAddress;

            if (string.IsNullOrEmpty(userHostAddress))
            {
                userHostAddress = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }

            //最后判断获取是否成功，并检查IP地址的格式（检查其格式非常重要）
            if (!string.IsNullOrEmpty(userHostAddress) && userHostAddress.IsMatch(Constant.IPRegex))
            {
                return userHostAddress;
            }

            return "127.0.0.1";
        }
    }
}
