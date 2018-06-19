using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace z.Foundation.Data
{
    /// <summary>
    /// 基于EntityFramework数据仓储类辅助类
    /// </summary>
    /// <typeparam name="T">实体类</typeparam>
    internal class MongoDBHelper<T> where T : class
    {
        static readonly object lockObj = new object();
        static Dictionary<string, string> dict = new Dictionary<string, string>();

        /// <summary>
        /// 构造函数
        /// </summary>
        public MongoDBHelper() 
        {
            var connectionString = ConfigurationManager.ConnectionStrings[ConnectionName].ConnectionString;
            var client = new MongoClient(connectionString);
            var server = client.GetServer();
            var database = server.GetDatabase(new MongoUrl(connectionString).DatabaseName);
            Collection = database.GetCollection<T>(typeof(T).Name);
        }

        /// <summary>
        /// MongoDB中的collection对象
        /// </summary>
        public MongoCollection<T> Collection
        {
            get;
            set;
        }

        /// <summary>
        /// 获取数据库连接字符串名称
        /// </summary>
        private static string ConnectionName
        {
            get
            {
                var classFullName = typeof(T).FullName;
                if (!dict.ContainsKey(classFullName))
                {
                    lock (lockObj)
                    {
                        if (!dict.ContainsKey(classFullName))
                        {
                            dict[classFullName] = "";

                            var customAttributes = typeof(T).GetCustomAttributes(false);
                            foreach (var item in customAttributes)
                            {
                                if (item is CustomDataAttribute)
                                {
                                    CustomDataAttribute customDataAttribute = item as CustomDataAttribute;
                                    dict[classFullName] = customDataAttribute.ConnectionName;

                                    break;
                                }
                            }
                        }
                    }
                }

                return dict[classFullName];
            }
        }
    
    }
}
