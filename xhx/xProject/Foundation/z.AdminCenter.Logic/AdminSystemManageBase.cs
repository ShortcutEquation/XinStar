using z.AdminCenter.Entity;
using z.Logic.Base;
using z.Foundation;
using z.Foundation.Cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace z.AdminCenter.Logic
{
    public class AdminSystemManageBase : NHLogicBase
    {
        private static readonly object lockObj = new object();

        /// <summary>
        /// 获取管理系统缓存
        /// </summary>
        /// <returns></returns>
        public IList<admin_system> GetAllAdminSystems()
        {
            ICache cache = new HttpCache();
            object obj = cache.GetCache("admin_system");
            if (obj == null)
            {
                lock (lockObj)
                {
                    obj = cache.GetCache("admin_system");
                    if (obj == null)
                    {
                        obj = Repository.AsQueryable<admin_system>().Where(e => e.Deleted == false).ToList();
                        cache.SetCache("admin_system", obj, TimeSpan.Zero);
                    }
                }
            }

            return (List<admin_system>)obj;
        }

        /// <summary>
        /// 设置管理系统缓存
        /// </summary>
        protected void SetAllAdminSystems()
        {
            ICache cache = new HttpCache();
            object obj = Repository.AsQueryable<admin_system>().Where(e => e.Deleted == false).ToList();
            cache.SetCache("admin_system", obj, TimeSpan.Zero);
        }
    }
}
