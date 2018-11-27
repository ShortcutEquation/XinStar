using z.Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using z.Foundation.Data;

namespace com.star.Logic.Search
{
    public class Constant
    {
        //文件服务器地址
        internal static string FILE_SERVER = @"G:\xindex\project_about\file_server";//   DConfigHelper.GetConfigValue("service_fileserver");

        //盘古分词器XML地址
        internal static string PanGuXML = @"G:\xindex\project_about\file_server\pangu\PanGu.xml"; // DConfigHelper.GetConfigValue("service_search_panguxml");

        //索引库过期时间间隔（day）
        internal static int IndexExpireInterval
        {
            get
            {
                return int.Parse(DConfigHelper.GetConfigValue("service_search_indexexpireinterval"));
            }
        }

        //搜索缓存自动更新时间间隔（minute）
        internal static int AutoInitializationInterval
        {
            get
            {
                return int.Parse(DConfigHelper.GetConfigValue("service_search_autoinitializationinterval"));
            }
        }
    }
}
