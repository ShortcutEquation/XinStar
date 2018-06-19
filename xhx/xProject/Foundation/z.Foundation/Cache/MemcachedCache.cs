using Enyim.Caching;
using Enyim.Caching.Configuration;
using Enyim.Caching.Memcached;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace z.Foundation.Cache
{
    /// <summary>
    /// Memcached缓存
    /// </summary>
    public class MemcachedCache : ICache
    {
        static MemcachedClient client = new MemcachedClient();

        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="key">缓存键值</param>
        /// <returns></returns>
        public object GetCache(string key)
        {
            return client.Get(key);
        }

        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <param name="key">缓存键值</param>
        /// <param name="obj">缓存对象</param>
        /// <param name="slidingExpiration">相对过期时间</param>
        public void SetCache(string key, object obj, TimeSpan slidingExpiration)
        {
            if (slidingExpiration != TimeSpan.MaxValue && slidingExpiration != TimeSpan.Zero)
            {
                double dMemcachedTimeZone = 0;
                string strMemcachedTimeZone = ConfigurationManager.AppSettings["MemcachedTimeZone"];

                if (!string.IsNullOrEmpty(strMemcachedTimeZone) && double.TryParse(strMemcachedTimeZone, out dMemcachedTimeZone))
                {
                    slidingExpiration = slidingExpiration.Add(TimeSpan.FromHours(dMemcachedTimeZone));
                }
            }

            client.Store(StoreMode.Set, key, obj, slidingExpiration);
        }

        /// <summary>
        /// 移除缓存
        /// </summary>
        /// <param name="key">缓存键值</param>
        public void RemoveCache(string key)
        {
            client.Remove(key);
        }
    }
}
