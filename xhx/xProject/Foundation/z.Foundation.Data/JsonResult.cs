using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace z.Foundation.Data
{
    [Serializable]
    public class JsonResult
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool Succeeded
        {
            get;
            set;
        }

        public string MsgType
        {
            get;
            set;
        }

        /// <summary>
        /// 消息
        /// </summary>
        public string Message
        {
            get;
            set;
        }

        /// <summary>
        /// 返回结果集对象
        /// </summary>
        public object Result
        {
            get;
            set;
        }

        public enum MessageType
        {
            success,
            info,
            warning,
            error
        }
    }
}
