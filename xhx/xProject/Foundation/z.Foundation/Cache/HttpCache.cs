using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Caching;

namespace z.Foundation.Cache
{
    /// <summary>
    /// 应用程序内部缓存
    /// </summary>
    public class HttpCache : ICache
    {
        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="key">缓存键值</param>
        /// <returns></returns>
        public object GetCache(string key)
        {
            System.Web.Caching.Cache cache = HttpRuntime.Cache;
            return cache[key];
        }

        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <param name="key">缓存键值</param>
        /// <param name="obj">缓存对象</param>
        /// <param name="slidingExpiration">相对过期时间</param>
        public void SetCache(string key, object obj, TimeSpan slidingExpiration)
        {
            System.Web.Caching.Cache cache = HttpRuntime.Cache;
            cache.Insert(key, obj, null, System.Web.Caching.Cache.NoAbsoluteExpiration, slidingExpiration, CacheItemPriority.NotRemovable, null);
        }

        /// <summary>
        /// 移除缓存
        /// </summary>
        /// <param name="key">缓存键值</param>
        public void RemoveCache(string key)
        {
            System.Web.Caching.Cache cache = HttpRuntime.Cache;
            cache.Remove(key);
        }
    }
}
