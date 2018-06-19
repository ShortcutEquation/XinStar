using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace z.Foundation
{
    /// <summary>
    /// 配置文件操作类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ConfigOperate<T> where T : class
    {
        private string ConfigFilePath
        {
            get;
            set;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="configFilePath">配置文件路径</param>
        public ConfigOperate(string configFilePath)
        {
            ConfigFilePath = configFilePath;
        }

        /// <summary>
        /// 从配置文件中读取配置信息
        /// </summary>
        /// <returns></returns>
        public T Load()
        {
            T result = default(T);

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
            StreamReader streamReader = new StreamReader(ConfigFilePath, Encoding.UTF8);
            try
            {
                result = xmlSerializer.Deserialize(streamReader) as T;
            }
            catch (Exception e)
            {
                throw new Exception("读取配置文件出错:" + e.Message);
            }
            finally
            {
                streamReader.Close();
            }
            return result;
        }

        /// <summary>
        /// 保存数据到配置文件
        /// </summary>
        public void Save(T model)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
            StreamWriter streamWriter = new StreamWriter(ConfigFilePath, false, Encoding.UTF8);
            try
            {
                xmlSerializer.Serialize(streamWriter, model);
            }
            catch (Exception e)
            {
                throw new Exception("保存配置文件出错:" + e.Message);
            }
            finally
            {
                streamWriter.Close();
            }
        }
    }
}
