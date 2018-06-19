using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace z.ApiCenter.Logic
{
    public class VerifySignParam
    {
        /// <summary>
        /// 访问者的唯一标识
        /// </summary>
        public string Access_Key
        {
            get;set;
        }

        /// <summary>
        /// 功能编码
        /// </summary>
        public string Access_Function
        {
            get;set;
        }

        /// <summary>
        /// 签名后的内容
        /// </summary>
        public string Sign
        {
            get;set;
        }

        /// <summary>
        /// UTC时间戳 (自1970-1-1 00:00:00至今的总毫秒数)。允许客户端请求时间误差为30分钟
        /// </summary>
        public long Timestamp
        {
            get; set;
        }

        /// <summary>
        /// 所有参数
        /// </summary>
        public Dictionary<string,string> UrlParams
        {
            get;set;
        }
    }
}
