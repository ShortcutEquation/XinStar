using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace z.Foundation.Data
{
    /// <summary>
    /// 排序参数类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class OrderBy<T>
    {
        /// <summary>
        /// 升序=true OR 降序=false
        /// </summary>
        public bool Sort { get; set; }

        /// <summary>
        /// 排序字段表达式树
        /// </summary>
        public Expression<Func<T, object>> exp { get; set; }
    }
}
