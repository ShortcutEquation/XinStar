using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate;

namespace z.Foundation.Data
{
    /// <summary>
    /// 具有分页信息的强类型数据列表
    /// </summary>
    [Serializable]
    public class PagedList<T> : IPagedList<T>
    {
        /// <summary>
        /// 创建分页的强类型对象列表
        /// </summary>
        public PagedList()
        {
            this.PageSize = 15;
            this.PageIndex = 1;
            this.Rows = new List<T>();
        }

        /// <summary>
        /// 创建分页的强类型对象列表
        /// </summary>
        /// <param name="source">未分页的对象列表</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页记录数</param>
        public PagedList(IEnumerable<T> source, int pageIndex, int pageSize)
            : this(source.AsQueryable(), pageIndex, pageSize)
        {

        }

        /// <summary>
        /// 创建分页的强类型对象列表
        /// </summary>
        /// <param name="source">已分页的对象列表</param>
        /// <param name="totalRecords">总记录数</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页记录数</param>
        public PagedList(IEnumerable<T> source, int totalRecords, int pageIndex, int pageSize)
            : this(source.AsQueryable(), totalRecords, pageIndex, pageSize)
        {

        }

        /// <summary>
        /// 创建分页的强类型对象列表
        /// </summary>
        /// <param name="source">未分页的对象列表</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页记录数</param>
        public PagedList(IQueryable<T> source, int pageIndex, int pageSize)
        {
            pageIndex = pageIndex < 1 ? 1 : pageIndex;
            pageSize = pageSize < 1 ? 15 : pageSize;

            this.TotalCount = source.Count();
            this.TotalPages = TotalCount / pageSize;
            if (TotalCount % pageSize > 0)
                this.TotalPages++;
            this.PageSize = pageSize;
            this.PageIndex = pageIndex;

            int skipCount = 0;
            if (pageIndex > 1)
            {
                skipCount = pageSize * pageIndex - pageSize;
            }

            this.Rows = new List<T>();
            this.Rows.AddRange(source.Skip(skipCount).Take(pageSize).ToList());
        }

        /// <summary>
        /// 创建分页的强类型对象列表
        /// </summary>
        /// <param name="source">已分页的对象列表</param>
        /// <param name="totalRecords">总记录数</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页记录数</param>
        public PagedList(IQueryable<T> source, int totalRecords, int pageIndex, int pageSize)
        {
            pageIndex = pageIndex < 1 ? 1 : pageIndex;
            pageSize = pageSize < 1 ? 15 : pageSize;

            this.TotalCount = totalRecords;
            this.TotalPages = TotalCount / pageSize;
            if (TotalCount % pageSize > 0)
                this.TotalPages++;
            this.PageSize = pageSize;
            this.PageIndex = pageIndex;

            this.Rows = new List<T>();
            this.Rows.AddRange(source.ToList());
        }

        #region IPagedList Members

        /// <summary>
        /// 总记录数
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// 总页数
        /// </summary>
        public int TotalPages { get; set; }

        /// <summary>
        /// 当前页码
        /// </summary>
        public int PageIndex { get; set; }

        /// <summary>
        /// 每页记录数
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// 是否有上一页
        /// </summary>
        public bool HasPreviousPage
        {
            get
            {
                return (PageIndex > 1);
            }
        }

        /// <summary>
        /// 是否有下一页
        /// </summary>
        public bool HasNextPage
        {
            get
            {
                return (this.PageIndex * this.PageSize) < this.TotalCount;
            }
        }

        /// <summary>
        /// 数据列表
        /// </summary>
        public List<T> Rows { get; set; }

        #endregion
    }
}
