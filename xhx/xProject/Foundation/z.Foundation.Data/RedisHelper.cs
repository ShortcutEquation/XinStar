using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using StackExchange.Redis;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Net;
using System.Configuration;
using z.Foundation;
using System.Threading;

namespace z.Foundation.Data
{
    public class RedisHelper
    {
        #region Properties
        private static ConcurrentDictionary<string, ConnectionMultiplexer> mConnectionMultiplexers = new ConcurrentDictionary<string, ConnectionMultiplexer>();
        private static ConcurrentDictionary<string, object> mLockObject = new ConcurrentDictionary<string, object>();
        private string mConnectionName
        {
            get;set;
        }
        private bool mIsUseThreadPool
        {
            get;set;
        }
        private int mDefaultDbIndex
        {
            get;set;
        }
        private string mConnectionString
        {
            get;set;
        }
        #endregion

        #region Public Methods

        public RedisHelper(string connectionName, bool isUseThreadPool = true, int defaultDbIndex = -1)
        {
            mConnectionName = connectionName;
            if (ConfigurationManager.ConnectionStrings[connectionName] == null)
            {
                throw new Exception(connectionName + "连接字符串不存在");
            }
            mConnectionString = ConfigurationManager.ConnectionStrings[connectionName].ConnectionString;
            mIsUseThreadPool = isUseThreadPool;
            mDefaultDbIndex = defaultDbIndex;
        }

        /// <summary>
        /// 根据key获取缓存对象
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public object Get(string key, int dbindex = -1)
        {
            int iRetrytimes = 3;
            while (iRetrytimes-- > 0)
            {
                try
                {
                    if (!mIsUseThreadPool)
                    {
                        using (ConnectionMultiplexer conn = ConnectionMultiplexer.Connect(mConnectionString))
                        {
                            var result = GetDatabase(dbindex, conn).StringGet(key);
                            return result.HasValue ? (object)result : null;
                        }
                    }
                    else
                    {
                        var result = GetDatabase(dbindex).StringGet(key);
                        return result.HasValue ? (object)result : null;
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                    Logger.Info(key + "开始重试(" + iRetrytimes + ")..");
                    Thread.Sleep(500);
                }
            }
            Logger.Error("获取Redis缓存key=" + key + "失败");
            return null;
        }

        /// <summary>
        /// 根据key获取缓存对象(json)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T Get<T>(string key, int dbindex = -1)
        {
            RedisValue result;
            if (!mIsUseThreadPool)
            {
                using (ConnectionMultiplexer conn = ConnectionMultiplexer.Connect(mConnectionString))
                {
                    result = GetDatabase(dbindex, conn).StringGet(key);
                }
            }
            else
            {
                result = GetDatabase(dbindex).StringGet(key);
            }
            if (!result.HasValue)
            {
                return default(T);
            }
            return result.ToString().JsonDeserialize<T>();
        }

        /// <summary>
        /// 设置缓存对象
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="dbindex"></param>
        public void Set(string key, string value, int dbindex = -1)
        {
            try
            {
                if (!mIsUseThreadPool)
                {
                    using (ConnectionMultiplexer conn = ConnectionMultiplexer.Connect(mConnectionString))
                    {
                        GetDatabase(dbindex, conn).StringSet(key, value);
                    }
                }
                else
                {
                    GetDatabase(dbindex).StringSet(key, value);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw new Exception("设置缓存( " + key + ":" + value + " )失败", ex);
            }
        }

        /// <summary>
        /// 设置缓存对象
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expire">过期时间</param>
        /// <param name="dbindex"></param>
        public void Set(string key, string value, DateTime expire, int dbindex = -1)
        {
            try
            {
                if (!mIsUseThreadPool)
                {
                    using (ConnectionMultiplexer conn = ConnectionMultiplexer.Connect(mConnectionString))
                    {
                        var dataBase = GetDatabase(dbindex, conn);
                        dataBase.StringSet(key, value, new TimeSpan(expire.Subtract(DateTime.Now).Ticks));
                    }
                }
                else
                {
                    var dataBase = GetDatabase(dbindex);
                    dataBase.StringSet(key, value, new TimeSpan(expire.Subtract(DateTime.Now).Ticks));
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw new Exception("设置缓存( " + key + ":" + value + " )失败", ex);
            }
        }

        /// <summary>
        /// 设置缓存对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="dbindex"></param>
        public void Set<T>(string key, T value, int dbindex = -1)
        {
            try
            {
                if (!mIsUseThreadPool)
                {
                    using (ConnectionMultiplexer conn = ConnectionMultiplexer.Connect(mConnectionString))
                    {
                        GetDatabase(dbindex, conn).StringSet(key, value.JsonSerialize());
                    }
                }
                else
                {
                    GetDatabase(dbindex).StringSet(key, value.JsonSerialize());
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw new Exception("设置缓存( " + key + ":" + value + " )失败", ex);
            }
        }

        /// <summary>
        /// 设置缓存对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expire">过期时间</param>
        /// <param name="dbindex"></param>
        public void Set<T>(string key, T value, DateTime expire, int dbindex = -1)
        {
            try
            {
                if (!mIsUseThreadPool)
                {
                    using (ConnectionMultiplexer conn = ConnectionMultiplexer.Connect(mConnectionString))
                    {
                        var dataBase = GetDatabase(dbindex, conn);
                        dataBase.StringSet(key, value.JsonSerialize(), new TimeSpan(expire.Subtract(DateTime.Now).Ticks));
                    }
                }
                else
                {
                    var dataBase = GetDatabase(dbindex);
                    dataBase.StringSet(key, value.JsonSerialize(), new TimeSpan(expire.Subtract(DateTime.Now).Ticks));
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw new Exception("设置缓存( " + key + ":" + value + " )失败", ex);
            }
        }

        /// <summary>
        /// 模糊查询获取key
        /// </summary>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public IEnumerable<string> GetPatternKey(string pattern, int dbindex = -1, int pageIndex = 1, int pageSize = int.MaxValue)
        {
            dbindex = (dbindex == -1) ? mDefaultDbIndex : dbindex;
            try
            {
                if (!mIsUseThreadPool)
                {
                    using (ConnectionMultiplexer conn = ConnectionMultiplexer.Connect(mConnectionString))
                    {
                        var endpoints = conn.GetEndPoints();
                        var server = conn.GetServer(endpoints[0]);
                        if (dbindex > -1)
                        {
                            return server.Keys(pattern: pattern, database: dbindex).Skip((pageIndex - 1) * pageSize).Take(pageSize).Select(w => w.ToString()).ToList();
                        }
                        else
                        {
                            return server.Keys(pattern: pattern).Skip((pageIndex - 1) * pageSize).Take(pageSize).Select(w => w.ToString()).ToList();
                        }
                    }
                }
                else
                {
                    var endpoints = GetConnectionMultiplexer().GetEndPoints();
                    var server = GetConnectionMultiplexer().GetServer(endpoints[0]);
                    if (dbindex > -1)
                    {
                        return server.Keys(pattern: pattern, database: dbindex).Skip((pageIndex - 1) * pageSize).Take(pageSize).Select(w => w.ToString()).ToList();
                    }
                    else
                    {
                        return server.Keys(pattern: pattern).Skip((pageIndex - 1) * pageSize).Take(pageSize).Select(w => w.ToString()).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw new Exception("模糊查询获取key( " + pattern + " )失败", ex);
            }
        }

        /// <summary>
        /// 判断在缓存中是否存在该key的缓存数据
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Exists(string key, int dbindex = -1)
        {
            try
            {
                if (!mIsUseThreadPool)
                {
                    using (ConnectionMultiplexer conn = ConnectionMultiplexer.Connect(mConnectionString))
                    {
                        return GetDatabase(dbindex, conn).KeyExists(key);
                    }
                }
                else
                {
                    return GetDatabase(dbindex).KeyExists(key);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw new Exception("获取缓存( " + key + " )失败", ex);
            }
        }

        /// <summary>
        /// 移除指定key的缓存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dbindex"></param>
        /// <returns></returns>
        public bool Remove(string key, int dbindex = -1)
        {
            try
            {
                if (!mIsUseThreadPool)
                {
                    using (ConnectionMultiplexer conn = ConnectionMultiplexer.Connect(mConnectionString))
                    {
                        return GetDatabase(dbindex, conn).KeyDelete(key);
                    }
                }
                else
                {
                    return GetDatabase(dbindex).KeyDelete(key);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw new Exception("移除指定key的缓存( " + key + " )失败", ex);
            }
        }

        /// <summary>
        /// 实现递增
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dbindex"></param>
        /// <returns></returns>
        public long Increment(string key, int dbindex = -1)
        {
            try
            {
                if (!mIsUseThreadPool)
                {
                    using (ConnectionMultiplexer conn = ConnectionMultiplexer.Connect(mConnectionString))
                    {
                        return GetDatabase(dbindex, conn).StringIncrement(key);
                    }
                }
                else
                {
                    return GetDatabase(dbindex).StringIncrement(key);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw new Exception("实现递增( " + key + " )失败", ex);
            }
        }

        /// <summary>
        /// 实现递减
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dbindex"></param>
        /// <returns></returns>
        public long Decrement(string key, int dbindex = -1)
        {
            try
            {
                if (!mIsUseThreadPool)
                {
                    using (ConnectionMultiplexer conn = ConnectionMultiplexer.Connect(mConnectionString))
                    {
                        return GetDatabase(dbindex, conn).StringDecrement(key);
                    }
                }
                else
                {
                    return GetDatabase(dbindex).StringDecrement(key);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw new Exception("实现递减( " + key + " )失败", ex);
            }
        }

        /// <summary>
        /// 异步根据key获取缓存对象
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<object> GetAsync(string key, int dbindex = -1)
        {
            try
            {
                if (!mIsUseThreadPool)
                {
                    using (ConnectionMultiplexer conn = ConnectionMultiplexer.Connect(mConnectionString))
                    {
                        var result = await GetDatabase(dbindex, conn).StringGetAsync(key);
                        return result.HasValue ? (object)result : null;
                    }
                }
                else
                {
                    var result = await GetDatabase(dbindex).StringGetAsync(key);
                    return result.HasValue ? (object)result : null;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw new Exception("获取缓存失败", ex);
            }
        }

        /// <summary>
        /// 异步设置缓存对象
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="dbindex"></param>
        public async Task SetAsync(string key, string value, int dbindex = -1)
        {
            try
            {
                if (!mIsUseThreadPool)
                {
                    using (ConnectionMultiplexer conn = ConnectionMultiplexer.Connect(mConnectionString))
                    {
                        await GetDatabase(dbindex, conn).StringGetSetAsync(key, value);
                    }
                }
                else
                {
                    await GetDatabase(dbindex).StringGetSetAsync(key, value);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw new Exception("设置缓存( " + key + ":" + value + " )失败", ex);
            }
        }

        #endregion

        #region Private Methods

        private ConnectionMultiplexer CreateConnectionMultiplexer()
        {
            ConnectionMultiplexer _instance = ConnectionMultiplexer.Connect(mConnectionString);
            _instance.ConnectionFailed += MuxerConnectionFailed;
            _instance.ConnectionRestored += MuxerConnectionRestored;
            _instance.ErrorMessage += MuxerErrorMessage;
            _instance.ConfigurationChanged += MuxerConfigurationChanged;
            _instance.HashSlotMoved += MuxerHashSlotMoved;
            _instance.InternalError += MuxerInternalError;
            return _instance;
        }

        private ConnectionMultiplexer GetConnectionMultiplexer()
        {
            if (!mLockObject.ContainsKey(mConnectionName))
            {
                mLockObject.TryAdd(mConnectionName, new object());
            }

            if (!mConnectionMultiplexers.ContainsKey(mConnectionName) || !mConnectionMultiplexers[mConnectionName].IsConnected)
            {
                lock (mLockObject[mConnectionName])
                {
                    if (!mConnectionMultiplexers.ContainsKey(mConnectionName) || !mConnectionMultiplexers[mConnectionName].IsConnected)
                    {
                        mConnectionMultiplexers.TryAdd(mConnectionName, CreateConnectionMultiplexer());
                    }
                }
            }

            return mConnectionMultiplexers[mConnectionName];
        }

        private IDatabase GetDatabase(int dbindex, ConnectionMultiplexer connectionMultiplexer = null)
        {
            dbindex = (dbindex == -1) ? mDefaultDbIndex : dbindex;
            if (connectionMultiplexer == null) connectionMultiplexer = GetConnectionMultiplexer();
            if (dbindex == -1)
            {
                return connectionMultiplexer.GetDatabase();
            }
            else
            {
                return connectionMultiplexer.GetDatabase(dbindex);
            }
        }

        /// <summary>
        /// 配置更改时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MuxerConfigurationChanged(object sender, EndPointEventArgs e)
        {
            Logger.Info("Configuration changed: " + e.EndPoint);
        }

        /// <summary>
        /// 发生错误时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MuxerErrorMessage(object sender, RedisErrorEventArgs e)
        {
            Logger.Info("ErrorMessage: " + e.Message);
        }

        /// <summary>
        /// 重新建立连接之前的错误
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MuxerConnectionRestored(object sender, ConnectionFailedEventArgs e)
        {
            Logger.Info("ConnectionRestored: " + e.EndPoint);
        }

        /// <summary>
        /// 连接失败 ， 如果重新连接成功你将不会收到这个通知
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MuxerConnectionFailed(object sender, ConnectionFailedEventArgs e)
        {
            Logger.Info("重新连接：Endpoint failed: " + e.EndPoint + ", " + e.FailureType + (e.Exception == null ? "" : (", " + e.Exception.Message)));
        }

        /// <summary>
        /// 更改集群
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MuxerHashSlotMoved(object sender, HashSlotMovedEventArgs e)
        {
            Logger.Info("HashSlotMoved:NewEndPoint" + e.NewEndPoint + ", OldEndPoint" + e.OldEndPoint);
        }

        /// <summary>
        /// redis类库错误
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MuxerInternalError(object sender, InternalErrorEventArgs e)
        {
            Logger.Info("InternalError:Message" + e.Exception.Message);
        }

        #endregion
    }
}
