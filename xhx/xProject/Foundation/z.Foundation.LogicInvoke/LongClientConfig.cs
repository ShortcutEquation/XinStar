using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace z.Foundation.LogicInvoke
{
    public class LongClientConfig
    {
        /// <summary>
        /// 并发的线程数
        /// </summary>
        public int MaxThreadCount
        {
            get;set;
        }

        /// <summary>
        /// 心跳时间
        /// </summary>
        public DateTime AliveTime
        {
            get;set;
        }

        /// <summary>
        /// 剩余的线程数
        /// </summary>
        public int LeftThreadCount
        {
            get;set;
        }

        ///// <summary>
        ///// 剩下需要处理的任务
        ///// </summary>
        //public List<int> LeftTasks
        //{
        //    get;set;
        //}
    }
}
