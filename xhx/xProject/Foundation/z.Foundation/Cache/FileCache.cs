using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace z.Foundation.Cache
{
    public class FileCache : ICache
    {
        private static string FILE_SERVER = Utility.GetConfigValue("FileServer");

        /// <summary>
        /// 获取文件缓存，返回值为文件内容
        /// </summary>
        /// <param name="key">相对于文件缓存服务器根目录的文件缓存地址</param>
        /// <returns></returns>
        public object GetCache(string key)
        {
            string strPath = FILE_SERVER + (key.IsMatch(@"^[\\|/].+") ? key : ("\\" + key));

            //判断文件缓存是否存在或过期
            bool bCached = true;
            if (!File.Exists(strPath))
            {
                bCached = false;
            }
            else
            {
                FileInfo fileInfo = new FileInfo(strPath);
                if ((DateTime.Now - fileInfo.LastWriteTime).TotalMinutes > Convert.ToInt32(z.Foundation.Utility.GetConfigValue("PageLevelCacheTime")))
                {
                    bCached = false;
                }
            }

            if(bCached)
            {
                return FileOperate.Read(strPath);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 设置文件缓存
        /// </summary>
        /// <param name="key">相对于文件缓存服务器根目录的文件缓存地址</param>
        /// <param name="obj">文件缓存内容（该对象能做ToString操作）</param>
        /// <param name="slidingExpiration">设置文件缓存时该值无效（文件缓存过期时间由配置文件全局控制）</param>
        public void SetCache(string key, object obj, TimeSpan slidingExpiration)
        {
            string strPath = FILE_SERVER + (key.IsMatch(@"^[\\|/].+") ? key : ("\\" + key));

            FileOperate.Write(obj.ToString(), strPath);
        }

        /// <summary>
        /// 删除文件缓存
        /// </summary>
        /// <param name="key">相对于文件缓存服务器根目录的文件缓存地址</param>
        public void RemoveCache(string key)
        {
            string strPath = FILE_SERVER + (key.IsMatch(@"^[\\|/].+") ? key : ("\\" + key));

            FileOperate.Delete(strPath);
        }
    }
}
