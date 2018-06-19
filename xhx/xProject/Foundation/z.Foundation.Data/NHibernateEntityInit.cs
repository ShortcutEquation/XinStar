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
    public class NHibernateEntityInit
    {
        static ICache cache = new HttpCache();

        public void InitSessionFactory()
        {
            string strAllDB = Utility.GetConfigValue("AllDB");
            if (!string.IsNullOrEmpty(strAllDB))
            {
                var dbs = strAllDB.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                foreach (var item in dbs)
                {
                    var sessionFactory = (new Configuration()).Configure(string.Format("{0}/{1}.cfg.xml", Utility.ApplicationPath(), item)).BuildSessionFactory();
                    cache.SetCache(item, sessionFactory, TimeSpan.Zero);
                }
            }
        }
    }
}
