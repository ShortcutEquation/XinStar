using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace z.Foundation.Data
{
    /// <summary>
    /// 分页类接口
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IPagedList<T> : IPaged
    {
        /// <summary>
        /// 总记录数
        /// </summary>
        int TotalCount { get; set; }

        /// <summary>
        /// 总页数
        /// </summary>  
        int TotalPages { get; set; }

        /// <summary>
        /// 是否有上一页
        /// </summary>
        bool HasPreviousPage { get; }

        /// <summary>
        /// 是否有下一页
        /// </summary>
        bool HasNextPage { get; }

        /// <summary>
        /// 数据列表
        /// </summary>
        List<T> Rows { get; set; }
    }
}
