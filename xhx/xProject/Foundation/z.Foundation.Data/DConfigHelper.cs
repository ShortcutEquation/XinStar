using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Caching;

namespace z.Foundation.Data
{
    public class DConfigHelper
    {
        public const string RedisConfigConnectionName = "RedisConfigConnection";
        public const string MysqlConfigConnectionName = "MysqlConfigConnection";
        public const int DEFAULT_CONFIG_CACHE_SECONDS = 300;//默认的本地缓存时间（秒）

        /// <summary>
        /// 获取配置（首先判断appseting中是否存在ReadConfigFromLocal，不存在则走下面逻辑： 优先Redis，不存在则从数据库中取）
        /// appsetting中存在以下参数的含义：
        /// 1、EnableConfigReadFromLocal 为1  从本地配置appsetting中读取
        /// 2、EnableConfigCache 为1 启用本地缓存（对本地配置不生效）
        /// 3、ConfigCacheSeconds 本地缓存过期时间（单位秒，EnableConfigCache存在时有效）
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetConfigValue(string key)
        {
            if (ConfigurationManager.AppSettings.AllKeys.Contains("EnableConfigReadFromLocal") && ConfigurationManager.AppSettings["EnableConfigReadFromLocal"].ToString() == "1")
            {
                if (ConfigurationManager.AppSettings.AllKeys.Contains(key))
                {
                    return ConfigurationManager.AppSettings[key].ToString();
                }
                else
                {
                    throw (new Exception(string.Format("appSettings节中未找到key=\"{0}\"节点", key)));
                }
            }
            else
            {
                string cacheKey = "configcache_" + key;          
                int cacheSeconds = DEFAULT_CONFIG_CACHE_SECONDS;
                bool isCache = false;
                if (ConfigurationManager.AppSettings.AllKeys.Contains("EnableConfigCache") && ConfigurationManager.AppSettings["EnableConfigCache"].ToString() == "1")
                {
                    isCache = true;
                    object objCache = HttpRuntime.Cache.Get(cacheKey);
                    if (objCache != null)
                    {
                        return objCache.ToString();
                    }
                    if (ConfigurationManager.AppSettings.AllKeys.Contains("ConfigCacheSeconds"))
                    {
                        if(!Int32.TryParse(ConfigurationManager.AppSettings["ConfigCacheSeconds"], out cacheSeconds))
                        {
                            cacheSeconds = DEFAULT_CONFIG_CACHE_SECONDS;
                        }
                    }
                }

                RedisHelper redisHelper = new RedisHelper(RedisConfigConnectionName);
                var redisVal = redisHelper.Get(key);
                if (redisVal == null)
                {
                    MySqlParameter paraKey = new MySqlParameter()
                    {
                        ParameterName = "dkey",
                        MySqlDbType = MySqlDbType.VarChar,
                        Value = key
                    };
                    var data = MySqlHelper.ExecuteDataTable(MysqlConfigConnectionName, "select * from config_info where dkey= @dkey and deleted=0", System.Data.CommandType.Text, paraKey);
                    if (data != null && data.Rows.Count > 0)
                    {
                        try
                        {
                            redisHelper.Set(key, data.Rows[0]["dvalue"].ToString());
                        }
                        catch (Exception ex)
                        {
                            Logger.Error(ex);
                        }
                        if (isCache)
                        {
                            HttpRuntime.Cache.Insert(cacheKey, data.Rows[0]["dvalue"].ToString(), null, DateTime.Now.AddSeconds(cacheSeconds), System.Web.Caching.Cache.NoSlidingExpiration, CacheItemPriority.NotRemovable, null);
                        }
                        return data.Rows[0]["dvalue"].ToString();
                    }
                }
                else
                {
                    if (isCache)
                    {
                        HttpRuntime.Cache.Insert(cacheKey, redisVal.ToString(), null, DateTime.Now.AddSeconds(cacheSeconds), System.Web.Caching.Cache.NoSlidingExpiration, CacheItemPriority.NotRemovable, null);
                    }
                    return redisVal.ToString();
                }
            }
            return null;
        }

        /// <summary>
        /// 设置配置
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool SetConfigValue(string key, string value)
        {
            try
            {
                RedisHelper redisHelper = new RedisHelper(RedisConfigConnectionName);
                redisHelper.Set(key, value);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 设置配置
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expire"></param>
        /// <returns></returns>
        public static bool SetConfigValue(string key, string value, DateTime expire)
        {
            try
            {
                RedisHelper redisHelper = new RedisHelper(RedisConfigConnectionName);
                redisHelper.Set(key, value, expire);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 移除配置
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool RemoveConfigValue(string key)
        {
            try
            {
                RedisHelper redisHelper = new RedisHelper(RedisConfigConnectionName);
                redisHelper.Remove(key);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
