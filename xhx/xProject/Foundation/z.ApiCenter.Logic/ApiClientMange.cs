using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using z.ApiCenter.Entity;
using z.Foundation;
using z.Foundation.Cache;
using z.Foundation.Data;
using z.Logic.Base;

namespace z.ApiCenter.Logic
{
    public class ApiClientManage : NHLogicBase
    {
        public  readonly static ApiClientManage Instance = new ApiClientManage();

        #region 缓存
        private static readonly object lockObj = new object();

        /// <summary>
        /// 获取所有Client缓存
        /// </summary>
        /// <returns></returns>
        public IList<api_client> GetAllClients()
        {
            ICache cache = new HttpCache();
            object obj = cache.GetCache("api_client");
            if (obj == null)
            {
                lock (lockObj)
                {
                    obj = cache.GetCache("api_client");
                    if (obj == null)
                    {
                        obj = Repository.AsQueryable<api_client>().Where(e => e.Deleted == false).ToList();
                        cache.SetCache("api_client", obj, TimeSpan.Zero);
                    }
                }
            }

            return (List<api_client>)obj;
        }

        /// <summary>
        /// 设置Client缓存
        /// </summary>
        protected void SetAllClients()
        {
            ICache cache = new HttpCache();
            object obj = Repository.AsQueryable<api_client>().Where(e => e.Deleted == false).ToList();
            cache.SetCache("api_client", obj, TimeSpan.Zero);
        }

        #endregion

        public api_client GetClientByKey(string access_key)
        {
            return GetAllClients().FirstOrDefault<api_client>(a => a.AccessKey == access_key && a.Deleted == false);
        }

        /// <summary>
        /// 获取对象
        /// </summary>
        /// <param name="param">主键ID</param>
        /// <returns></returns>
        public api_client Get(object param)
        {
            return Repository.Get<api_client>(param);
        }

        public ApiClientExt GetClientAndFunction(int clientId)
        {
            ApiClientExt result = new ApiClientExt();

            api_client entity = Get(clientId);
            entity.CopyTo(result);

            List<api_client_function> clientFunctions = Repository.Find<api_client_function>(a => a.ClientId == clientId).ToList();
            result.FunctionIds = clientFunctions.Select(a => a.FunctionId).ToList();

            return result;
        }

        /// <summary>
        /// 保存对象
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public BoolResult Save(ApiClientExt param)
        {
            BoolResult result = new BoolResult();

            //保持对象的唯一性
            if (Repository.Exists<api_client>(e => (e.AccessKey == param.AccessKey || e.Name == param.Name) && e.Deleted == false))
            {
                result.Succeeded = false;
                result.Message = "已存在相同的Code值或Name值";
                return result;
            }

            api_client entity = new api_client();
            param.CopyTo(entity);

            using (ISession session = NHibernateHelper<api_client>.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                try
                {
                    session.Save(entity);
                    if (param.FunctionIds != null && param.FunctionIds.Count > 0)
                    {
                        foreach(var functionId in param.FunctionIds)
                        {
                            api_client_function clientFunction = new api_client_function()
                            {
                                ClientId = entity.Id,
                                FunctionId = functionId
                            };
                            session.Save(clientFunction);
                        }
                    }

                    transaction.Commit();

                    //更新缓存
                    SetAllClients();
                    ApiClientFunctionManage.Instance.SetAllClientFunctions();

                    result.Succeeded = true;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    result.Succeeded = false;
                    result.Message = "保存对象失败";
                }
            }
            return result;
        }

        /// <summary>
        /// 更新对象
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public BoolResult Update(ApiClientExt param)
        {
            BoolResult result = new BoolResult();

            api_client entity = Repository.First<api_client>(e => e.Id == param.Id && e.Deleted == false);
            if (entity == null)
            {
                result.Succeeded = false;
                result.Message = "修改的对象不存在";
                return result;
            }

            if (entity.AccessKey != param.AccessKey)
            {
                if (Repository.Exists<api_client>(e => e.AccessKey == param.AccessKey && e.Deleted == false))
                {
                    result.Succeeded = false;
                    result.Message = "已存在相同的Key";
                    return result;
                }
            }
            else if (entity.Name != param.Name)
            {
                if (Repository.Exists<api_client>(e => e.Name == param.Name && e.Deleted == false))
                {
                    result.Succeeded = false;
                    result.Message = "已存在相同的Name";
                    return result;
                }
            }

            entity.Name = param.Name;
            entity.UpdatedBy = param.UpdatedBy;
            entity.UpdatedOn = param.UpdatedOn;

            using (ISession session = NHibernateHelper<api_client>.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                try
                {
                    session.Update(entity);

                    List<api_client_function> clientFunctions = Repository.Find<api_client_function>(a => a.ClientId == entity.Id).ToList();
                    foreach(var clientFunction in clientFunctions)
                    {
                        session.Delete(clientFunction);
                    }

                    if (param.FunctionIds != null && param.FunctionIds.Count > 0)
                    {
                        foreach (var functionId in param.FunctionIds)
                        {
                            api_client_function clientFunction = new api_client_function()
                            {
                                ClientId = entity.Id,
                                FunctionId = functionId
                            };
                            session.Save(clientFunction);
                        }
                    }

                    transaction.Commit();
                    //更新缓存
                    SetAllClients();
                    ApiClientFunctionManage.Instance.SetAllClientFunctions();
                    result.Succeeded = true;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    result.Succeeded = false;
                    result.Message = "保存对象失败";
                }
            }

            
            return result;
        }

        /// <summary>
        /// 单个、批量删除对象
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public BoolResult Deleted(ApiClientExt param)
        {
            BoolResult result = new BoolResult();

            IList<api_client> itemList = Repository.Find<api_client>(e => param.ClientIds.Contains(e.Id) && e.Deleted == false);
            if (itemList.Count == 0)
            {
                result.Succeeded = false;
                result.Message = "删除的对象不存在";
                return result;
            }
            else
            {
                using (ISession session = NHibernateHelper<api_client>.OpenSession())
                using (ITransaction transaction = session.BeginTransaction())
                {
                    try
                    {
                        int intRecord = 0;
                        foreach (var entity in itemList)
                        {
                            //删除系统时需要删除系统下包含的所有权限，同时删除权限与用户组、用户的关系

                            entity.Deleted = true;
                            entity.UpdatedBy = param.UpdatedBy;
                            entity.UpdatedOn = param.UpdatedOn;
                            session.Update(entity);

                            IList<api_client_function> clientFunctions = Repository.Find<api_client_function>(a => a.ClientId == entity.Id);
                            foreach (var clientFunction in clientFunctions)
                            {
                                session.Delete(clientFunction);
                            }

                            intRecord++;
                        }

                        transaction.Commit();

                        //更新缓存
                        SetAllClients();
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
        public IPagedList<api_client> GetPageList(IPagedParam<api_client> param)
        {
            IQueryable<api_client> queryable = Repository.AsQueryable<api_client>().Where(e => e.Deleted == false);
            if (!string.IsNullOrEmpty(param.model.Name))
            {
                queryable = queryable.Where(e => e.Name.Contains(param.model.Name));
            }
            queryable = queryable.OrderByDescending(e => e.CreatedOn);

            return new PagedList<api_client>(queryable, param.PageIndex, param.PageSize);
        }

        
    }
}
