using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace z.Foundation.Data
{
    /// <summary>
    /// 定义数据仓储公共接口
    /// </summary>
    public interface IRepository
    {
        /// <summary>
        /// 支持Linq语法查询结果集对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        IQueryable<T> AsQueryable<T>() where T : class,IEntity, new();

        /// <summary>
        /// 根据条件表达式统计满足条件的数据条数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression">条件表达式</param>
        /// <returns></returns>
        long Count<T>(Expression<Func<T, bool>> expression) where T : class, IEntity, new();

        /// <summary>
        /// 根据条件表达式判断是否存在符合条件的数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression">条件表达式</param>
        /// <returns></returns>
        bool Exists<T>(Expression<Func<T, bool>> expression) where T : class, IEntity, new();

        /// <summary>
        /// 根据条件表达式获取满足条件的某个类型为T的数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression">条件表达式</param>
        /// <returns></returns>
        T First<T>(Expression<Func<T, bool>> expression) where T : class, IEntity, new();

        /// <summary>
        /// 根据条件表达式获取满足条件的前n条类型为T的数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression">条件表达式</param>
        /// <param name="n">需获取的记录数</param>
        /// <returns></returns>
        IList<T> Top<T>(Expression<Func<T, bool>> expression, int n) where T : class, IEntity, new();

        /// <summary>
        /// 根据条件表达式获取满足条件的前n条类型为T自定义排序数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression">条件表达式</param>
        /// <param name="n">需获取的记录数</param>
        /// <param name="sortList">排序对象集合</param>
        /// <returns></returns>
        IList<T> Top<T>(Expression<Func<T, bool>> expression, int n, List<OrderBy<T>> sortList) where T : class, IEntity, new();

        /// <summary>
        /// 根据主键获取类型为T的数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id">主键字段值</param>
        /// <returns></returns>
        T Get<T>(object id) where T : class, IEntity, new();

        /// <summary>
        /// 根据条件表达式获取满足条件的所有类型为T的数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression">条件表达式</param>
        /// <returns></returns>
        IList<T> Find<T>(Expression<Func<T, bool>> expression) where T : class, IEntity, new();

        /// <summary>
        /// 根据条件表达式获取满足条件的所有类型为T的自定义排序数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression">条件表达式</param>
        /// <param name="sortList">排序对象集合</param>
        /// <returns></returns>
        IList<T> Find<T>(Expression<Func<T, bool>> expression, List<OrderBy<T>> sortList) where T : class, IEntity, new();

        /// <summary>
        /// 获取指定分页标准为T的数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页数据量</param>
        /// <returns></returns>
        IPagedList<T> FindPagedList<T>(int pageIndex, int pageSize) where T : class, IEntity, new();

        /// <summary>
        /// 根据条件表达式获取满足条件、指定分页标准的所有类型为T的数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页数据量</param>
        /// <param name="expression">条件表达式</param>
        /// <returns></returns>
        IPagedList<T> FindPagedList<T>(int pageIndex, int pageSize, Expression<Func<T, bool>> expression) where T : class, IEntity, new();

        /// <summary>
        /// 根据条件表达式获取满足条件、指定分页标准的所有类型为T的自定义排序数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页数据量</param>
        /// <param name="expression">条件表达式</param>
        /// <param name="sortList">排序对象集合</param>
        /// <returns></returns>
        IPagedList<T> FindPagedList<T>(int pageIndex, int pageSize, Expression<Func<T, bool>> expression, List<OrderBy<T>> sortList) where T : class, IEntity, new();

        /// <summary>
        /// 保存新的类型为T的数据对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item">需保存的对象</param>
        /// <returns></returns>
        object Save<T>(T item) where T : class, IEntity, new();

        /// <summary>
        /// 更新指定的类型为T的对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item">需更新的对象</param>
        void Update<T>(T item) where T : class, IEntity, new();

        /// <summary>
        /// 根据主键删除类型为T的数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id">主键字段值</param>
        void Delete<T>(object id) where T : class, IEntity, new();

        /// <summary>
        /// 删除某个类型为T的数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        void Delete<T>(T item) where T : class, IEntity, new();
    }
}
