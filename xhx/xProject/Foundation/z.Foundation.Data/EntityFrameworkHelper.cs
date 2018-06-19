using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;

namespace z.Foundation.Data
{
    /// <summary>
    /// 基于EntityFramework数据仓储类辅助类
    /// </summary>
    /// <typeparam name="T">实体类</typeparam>
    public class EntityFrameworkHelper<T> : DbContext where T : class
    {
        static readonly object lockObj = new object();
        static Dictionary<string, string> dict = new Dictionary<string, string>();

        static bool bSetInitializer = false;

        /// <summary>
        /// 构造函数
        /// </summary>
        public EntityFrameworkHelper()
            : base(ConnectionName)
        {
            if(!bSetInitializer)
            {
                Database.SetInitializer<EntityFrameworkHelper<T>>(null);

                bSetInitializer = true;
            }
        }

        /// <summary>
        /// 数据操作基础方法对象
        /// </summary>
        public DbSet<T> Model { get; set; }

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
                                if(item is CustomDataAttribute)
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
