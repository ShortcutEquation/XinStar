using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using z.ApiCenter.Entity;

namespace z.ApiCenter.Logic
{
    public class ApiSystemExt:api_system
    {
        /// <summary>
        /// 批量删除时传值使用
        /// </summary>
        public List<int> AdminSystemIds
        {
            get;
            set;
        }
    }
}
