using NHibernate;
using NHibernate.Cfg;
using NHibernate.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using z.Foundation.Cache;

namespace z.Foundation.Data
{
    /// <summary>
    /// 基于NHibernate数据仓储类辅助类
    /// </summary>
    public class NHibernateHelper<T>
    {
        static ICache cache = new HttpCache();
        static readonly object lockSessionFactory = new object();

        static readonly object lockConfigFileName = new object();
        static Dictionary<string, string> dict = new Dictionary<string, string>();

        static ISessionFactory _SessionFactory;

        /// <summary>
        /// 获取SessionFactory
        /// </summary>
        private static ISessionFactory SessionFactory
        {
            get
            {
                string strCacheKey = ConfigFileName;
                var obj = cache.GetCache(strCacheKey);

                if (obj == null)
                {
                    lock (lockSessionFactory)
                    {
                        obj = cache.GetCache(strCacheKey);
                        if (obj == null)
                        {
                            var na = string.Format("{0}/{1}.cfg.xml", Utility.ApplicationPath(), ConfigFileName);

                            _SessionFactory = (new Configuration()).Configure(string.Format("{0}/{1}.cfg.xml", Utility.ApplicationPath(), ConfigFileName)).BuildSessionFactory();
                            cache.SetCache(strCacheKey, _SessionFactory, TimeSpan.Zero);
                        }
                        else
                        {
                            _SessionFactory = (ISessionFactory)obj;
                        }
                    }
                }
                else
                {
                    _SessionFactory = (ISessionFactory)obj;
                }

                return _SessionFactory;
            }
        }

        /// <summary>
        /// 获取Session对象
        /// </summary>
        /// <returns></returns>
        public static ISession OpenSession()
        {
            ISession session;

            //if (CurrentSessionContext.HasBind(SessionFactory))
            //{
            //    session = SessionFactory.GetCurrentSession();
            //}
            //else
            //{
            //    session = SessionFactory.OpenSession();
            //    CurrentSessionContext.Bind(session);
            //}

            session = SessionFactory.OpenSession();
            //CurrentSessionContext.Bind(session);

            return session;
        }

        /// <summary>
        /// 获取数据库连接配置文件名称
        /// </summary>
        private static string ConfigFileName
        {
            get
            {
                var classFullName = typeof(T).FullName;
                if (!dict.ContainsKey(classFullName))
                {
                    lock (lockConfigFileName)
                    {
                        if (!dict.ContainsKey(classFullName))
                        {
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
