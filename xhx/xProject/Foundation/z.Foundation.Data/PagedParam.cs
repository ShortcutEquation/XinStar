using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace z.Foundation.Data
{
    /// <summary>
    /// 分页参数类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class PagedParam<T> : IPagedParam<T>
    {
        /// <summary>
        /// 页码
        /// </summary>
        public int PageIndex
        {
            get;
            set;
        }

        /// <summary>
        /// 每页显示数量
        /// </summary>
        public int PageSize
        {
            get;
            set;
        }

        /// <summary>
        /// 排序条件
        /// </summary>
        public List<OrderBy<T>> OrderByList
        {
            get;
            set;
        }

        /// <summary>
        /// 自定义参数实体
        /// </summary>
        public T model
        {
            get;
            set;
        }
    }
}
