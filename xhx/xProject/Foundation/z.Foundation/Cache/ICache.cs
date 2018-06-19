using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace z.Foundation.Cache
{
    /// <summary>
    /// 缓存接口
    /// </summary>
    public interface ICache
    {
        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="key">缓存键值</param>
        /// <returns></returns>
        object GetCache(string key);

        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <param name="key">缓存键值</param>
        /// <param name="obj">缓存对象</param>
        /// <param name="slidingExpiration">相对过期时间</param>
        void SetCache(string key, object obj, TimeSpan slidingExpiration);

        /// <summary>
        /// 移除缓存
        /// </summary>
        /// <param name="key">缓存键值</param>
        void RemoveCache(string key);
    }
}
