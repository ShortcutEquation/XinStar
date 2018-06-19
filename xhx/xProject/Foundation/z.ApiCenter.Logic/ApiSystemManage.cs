using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using z.ApiCenter.Entity;
using z.Foundation.Cache;
using z.Foundation.Data;
using z.Logic.Base;

namespace z.ApiCenter.Logic
{
    public class ApiSystemManage : NHLogicBase
    {
        public readonly static ApiSystemManage Instance = new ApiSystemManage();

        #region 缓存
        private static readonly object lockObj = new object();

        /// <summary>
        /// 获取管理系统缓存
        /// </summary>
        /// <returns></returns>
        public IList<api_system> GetAllAdminSystems()
        {
            ICache cache = new HttpCache();
            object obj = cache.GetCache("api_system");
            if (obj == null)
            {
                lock (lockObj)
                {
                    obj = cache.GetCache("api_system");
                    if (obj == null)
                    {
                        obj = Repository.AsQueryable<api_system>().Where(e => e.Deleted == false).ToList();
                        cache.SetCache("api_system", obj, TimeSpan.Zero);
                    }
                }
            }

            return (List<api_system>)obj;
        }

        /// <summary>
        /// 设置管理系统缓存
        /// </summary>
        protected void SetAllAdminSystems()
        {
            ICache cache = new HttpCache();
            object obj = Repository.AsQueryable<api_system>().Where(e => e.Deleted == false).ToList();
            cache.SetCache("api_system", obj, TimeSpan.Zero);
        }

        #endregion

        /// <summary>
        /// 获取对象
        /// </summary>
        /// <param name="param">主键ID</param>
        /// <returns></returns>
        public api_system Get(object param)
        {
            return Repository.Get<api_system>(param);
        }

        /// <summary>
        /// 保存对象
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public BoolResult Save(api_system param)
        {
            BoolResult result = new BoolResult();

            //保持对象的唯一性
            if (Repository.Exists<api_system>(e => (e.SystemCode == param.SystemCode || e.Name == param.Name) && e.Deleted == false))
            {
                result.Succeeded = false;
                result.Message = "已存在相同的Code值或Name值";
                return result;
            }

            object obj = Repository.Save<api_system>(param);
            if (obj != null)
            {
                result.Succeeded = true;

                //更新缓存
                SetAllAdminSystems();
            }
            else
            {
                result.Succeeded = false;
                result.Message = "保存对象失败";
            }

            return result;
        }

        /// <summary>
        /// 更新对象
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public BoolResult Update(api_system param)
        {
            BoolResult result = new BoolResult();

            api_system _api_system = Repository.First<api_system>(e => e.Id == param.Id && e.Deleted == false);
            if (_api_system == null)
            {
                result.Succeeded = false;
                result.Message = "修改的对象不存在";
                return result;
            }

            if (_api_system.SystemCode != param.SystemCode)
            {
                if (Repository.Exists<api_system>(e => e.SystemCode == param.SystemCode && e.Deleted == false))
                {
                    result.Succeeded = false;
                    result.Message = "已存在相同的Code";
                    return result;
                }
            }
            else if (_api_system.Name != param.Name)
            {
                if (Repository.Exists<api_system>(e => e.Name == param.Name && e.Deleted == false))
                {
                    result.Succeeded = false;
                    result.Message = "已存在相同的Name";
                    return result;
                }
            }

            _api_system.SystemCode = param.SystemCode;
            _api_system.Name = param.Name;
            _api_system.Description = param.Description;
            _api_system.Url = param.Url;
            _api_system.UpdatedBy = param.UpdatedBy;
            _api_system.UpdatedOn = param.UpdatedOn;
            Repository.Update<api_system>(_api_system);

            //更新缓存
            SetAllAdminSystems();

            result.Succeeded = true;
            return result;
        }

        /// <summary>
        /// 单个、批量删除对象
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public BoolResult Deleted(ApiSystemExt param)
        {
            BoolResult result = new BoolResult();

            IList<api_system> adminSystemList = Repository.Find<api_system>(e => param.AdminSystemIds.Contains(e.Id) && e.Deleted == false);
            if (adminSystemList.Count == 0)
            {
                result.Succeeded = false;
                result.Message = "删除的对象不存在";
                return result;
            }
            else
            {
                using (ISession session = NHibernateHelper<api_system>.OpenSession())
                using (ITransaction transaction = session.BeginTransaction())
                {
                    try
                    {
                        int intRecord = 0;
                        foreach (var _admin_system in adminSystemList)
                        {
                            //删除系统时需要删除系统下包含的所有功能，同时删除功能与用户的关系

                            _admin_system.Deleted = true;
                            _admin_system.UpdatedBy = param.UpdatedBy;
                            _admin_system.UpdatedOn = param.UpdatedOn;
                            session.Update(_admin_system);

                            //删除功能
                            IList<api_function> apiFuntions = Repository.Find<api_function>(a => a.Deleted == false && a.SystemId == _admin_system.Id);
                            foreach(var apiFunction in apiFuntions)
                            {
                                apiFunction.Deleted = true;
                                apiFunction.UpdatedBy = param.UpdatedBy;
                                apiFunction.UpdatedOn = param.UpdatedOn;
                                session.Update(apiFunction);
                            }
                            IList<int> functionIds = apiFuntions.Select(a => a.Id).ToList();

                            //删除用户功能关系
                            IList<api_client_function> clientFuntions = Repository.Find<api_client_function>(a => functionIds.Contains(a.FunctionId));
                            foreach(var clientFunction in clientFuntions)
                            {
                                session.Delete(clientFunction);
                            }

                            intRecord++;
                        }

                        transaction.Commit();

                        //更新缓存
                        SetAllAdminSystems();
                        ApiFunctionManage.Instance.SetAllFunctions();
                        ApiClientFunctionManage.Instance.SetAllClientFunctions();

                        result.Succeeded = true;
                        result.Result = intRecord;
                    }
                    catch
                    {
                        transaction.Rollback();
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// 分页获取列表（提供按Code或Name进行模糊搜索，搜索时给Name属性赋值，不提供客户端自定义排序，默认按创建时间倒序排列）
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public IPagedList<api_system> GetPageList(IPagedParam<api_system> param)
        {
            IQueryable<api_system> queryable = Repository.AsQueryable<api_system>().Where(e => e.Deleted == false);
            if (!string.IsNullOrEmpty(param.model.Name))
            {
                queryable = queryable.Where(e => e.SystemCode.Contains(param.model.Name) || e.Name.Contains(param.model.Name));
            }
            queryable = queryable.OrderByDescending(e => e.CreatedOn);

            return new PagedList<api_system>(queryable, param.PageIndex, param.PageSize);
        }
    }
}
