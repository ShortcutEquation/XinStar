using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Text;

namespace z.Foundation.Data
{
    /// <summary>
    /// 基于EntityFramework数据仓储类
    /// </summary>
    public class EntityFrameworkRepository : IRepository
    {
        /// <summary>
        /// 支持Linq语法查询结果集对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IQueryable<T> AsQueryable<T>() where T : class, IEntity, new()
        {
            var db = new EntityFrameworkHelper<T>();

            return db.Model.AsQueryable();
        }

        /// <summary>
        /// 根据条件表达式统计满足条件的数据条数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression">条件表达式</param>
        /// <returns></returns>
        public long Count<T>(System.Linq.Expressions.Expression<Func<T, bool>> expression) where T : class, IEntity, new()
        {
            using (var db = new EntityFrameworkHelper<T>())
            {
                return db.Model.AsQueryable().Where(expression).Count();
            }
        }

        /// <summary>
        /// 根据条件表达式判断是否存在符合条件的数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression">条件表达式</param>
        /// <returns></returns>
        public bool Exists<T>(System.Linq.Expressions.Expression<Func<T, bool>> expression) where T : class, IEntity, new()
        {
            using (var db = new EntityFrameworkHelper<T>())
            {
                return db.Model.AsQueryable().Where(expression).Any();
            }
        }

        /// <summary>
        /// 根据条件表达式获取满足条件的某个类型为T的数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression">条件表达式</param>
        /// <returns></returns>
        public T First<T>(System.Linq.Expressions.Expression<Func<T, bool>> expression) where T : class, IEntity, new()
        {
            using (var db = new EntityFrameworkHelper<T>())
            {
                return db.Model.AsQueryable().Where(expression).FirstOrDefault();
            }
        }

        /// <summary>
        /// 根据条件表达式获取满足条件的前n条类型为T的数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression">条件表达式</param>
        /// <param name="n">需获取的记录数</param>
        /// <returns></returns>
        public IList<T> Top<T>(System.Linq.Expressions.Expression<Func<T, bool>> expression, int n) where T : class, IEntity, new()
        {
            using (var db = new EntityFrameworkHelper<T>())
            {
                return db.Model.AsQueryable().Where(expression).Take(n).ToList();
            }
        }

        /// <summary>
        /// 根据条件表达式获取满足条件的前n条类型为T自定义排序数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression">条件表达式</param>
        /// <param name="n">需获取的记录数</param>
        /// <param name="sortList">排序对象集合</param>
        /// <returns></returns>
        public IList<T> Top<T>(System.Linq.Expressions.Expression<Func<T, bool>> expression, int n, List<OrderBy<T>> sortList) where T : class, IEntity, new()
        {
            using (var db = new EntityFrameworkHelper<T>())
            {
                var result = db.Model.AsQueryable().Where(expression);

                foreach (var obj in sortList)
                {
                    if (obj.Sort)
                    {
                        result = result.OrderBy(obj.exp);
                    }
                    else
                    {
                        result = result.OrderByDescending(obj.exp);
                    }
                }
                return result.Take(n).ToList();
            }
        }

        /// <summary>
        /// 根据主键获取类型为T的数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id">主键字段值</param>
        /// <returns></returns>
        public T Get<T>(object id) where T : class, IEntity, new()
        {
            using (var db = new EntityFrameworkHelper<T>())
            {
                return db.Model.Find(id);
            }
        }

        /// <summary>
        /// 根据条件表达式获取满足条件的所有类型为T的数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression">条件表达式</param>
        /// <returns></returns>
        public IList<T> Find<T>(System.Linq.Expressions.Expression<Func<T, bool>> expression) where T : class, IEntity, new()
        {
            using (var db = new EntityFrameworkHelper<T>())
            {
                return db.Model.AsQueryable().Where(expression).ToList();
            }
        }

        /// <summary>
        /// 根据条件表达式获取满足条件的所有类型为T的自定义排序数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression">条件表达式</param>
        /// <param name="sortList">排序对象集合</param>
        /// <returns></returns>
        public IList<T> Find<T>(System.Linq.Expressions.Expression<Func<T, bool>> expression, List<OrderBy<T>> sortList) where T : class, IEntity, new()
        {
            using (var db = new EntityFrameworkHelper<T>())
            {
                var result = db.Model.AsQueryable().Where(expression);

                foreach (var obj in sortList)
                {
                    if (obj.Sort)
                    {
                        result = result.OrderBy(obj.exp);
                    }
                    else
                    {
                        result = result.OrderByDescending(obj.exp);
                    }
                }
                return result.ToList();
            }
        }

        /// <summary>
        /// 获取指定分页标准为T的数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页数据量</param>
        /// <returns></returns>
        public IPagedList<T> FindPagedList<T>(int pageIndex, int pageSize) where T : class, IEntity, new()
        {
            using (var db = new EntityFrameworkHelper<T>())
            {
                return new PagedList<T>(db.Model.AsQueryable(), pageIndex, pageSize);
            }
        }

        /// <summary>
        /// 根据条件表达式获取满足条件、指定分页标准的所有类型为T的数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页数据量</param>
        /// <param name="expression">条件表达式</param>
        /// <returns></returns>
        public IPagedList<T> FindPagedList<T>(int pageIndex, int pageSize, System.Linq.Expressions.Expression<Func<T, bool>> expression) where T : class, IEntity, new()
        {
            using (var db = new EntityFrameworkHelper<T>())
            {
                return new PagedList<T>(db.Model.AsQueryable().Where(expression), pageIndex, pageSize);
            }
        }

        /// <summary>
        /// 根据条件表达式获取满足条件、指定分页标准的所有类型为T的自定义排序数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页数据量</param>
        /// <param name="expression">条件表达式</param>
        /// <param name="sortList">排序对象集合</param>
        /// <returns></returns>
        public IPagedList<T> FindPagedList<T>(int pageIndex, int pageSize, System.Linq.Expressions.Expression<Func<T, bool>> expression, List<OrderBy<T>> sortList) where T : class, IEntity, new()
        {
            using (var db = new EntityFrameworkHelper<T>())
            {
                var result = db.Model.AsQueryable().Where(expression);

                foreach (var obj in sortList)
                {
                    if (obj.Sort)
                        result = result.OrderBy(obj.exp);
                    else
                        result = result.OrderByDescending(obj.exp);
                }
                return new PagedList<T>(result, pageIndex, pageSize);
            }
        }

        /// <summary>
        /// 保存新的类型为T的数据对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item">需保存的对象</param>
        /// <returns></returns>
        public object Save<T>(T item) where T : class, IEntity, new()
        {
            using (var db = new EntityFrameworkHelper<T>())
            {
                item = db.Model.Add(item);
                db.SaveChanges();
                return item;
            }
        }

        /// <summary>
        /// 更新指定的类型为T的对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item">需更新的对象</param>
        public void Update<T>(T item) where T : class, IEntity, new()
        {
            using (var db = new EntityFrameworkHelper<T>())
            {
                db.Entry(item).State = EntityState.Modified;
                db.SaveChanges();
            }
        }

        /// <summary>
        /// 根据主键删除类型为T的数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id">主键字段值</param>
        public void Delete<T>(object id) where T : class, IEntity, new()
        {
            using (var db = new EntityFrameworkHelper<T>())
            {
                db.Model.Remove(Get<T>(id));
                db.SaveChanges();
            }
        }

        /// <summary>
        /// 删除某个类型为T的数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        public void Delete<T>(T item) where T : class, IEntity, new()
        {
            using (var db = new EntityFrameworkHelper<T>())
            {
                db.Model.Attach(item);
                db.Model.Remove(item);
                db.SaveChanges();
            }
        }
    }
}
