using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace z.Foundation.Data
{
    /// <summary>
    /// 分页参数类接口
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IPagedParam<T> : IPaged
    {
        /// <summary>
        /// 排序条件
        /// </summary>
        List<OrderBy<T>> OrderByList { get; set; }

        /// <summary>
        /// 自定义参数实体
        /// </summary>
        T model { get; set; }
    }
}
