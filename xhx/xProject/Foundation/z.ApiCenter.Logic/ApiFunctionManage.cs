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
    public class ApiFunctionManage : NHLogicBase
    {
        public readonly static ApiFunctionManage Instance = new ApiFunctionManage();

        #region 缓存
        private static readonly object lockObj = new object();

        /// <summary>
        /// 获取所有Function缓存
        /// </summary>
        /// <returns></returns>
        public IList<api_function> GetAllFunctions()
        {
            ICache cache = new HttpCache();
            object obj = cache.GetCache("api_function");
            if (obj == null)
            {
                lock (lockObj)
                {
                    obj = cache.GetCache("api_function");
                    if (obj == null)
                    {
                        obj = Repository.AsQueryable<api_function>().Where(a => a.Deleted == false).ToList();
                        cache.SetCache("api_function", obj, TimeSpan.Zero);
                    }
                }
            }

            return (List<api_function>)obj;
        }

        /// <summary>
        /// 设置Function缓存
        /// </summary>
        public void SetAllFunctions()
        {
            ICache cache = new HttpCache();
            object obj = Repository.AsQueryable<api_function>().Where(a => a.Deleted == false).ToList();
            cache.SetCache("api_function", obj, TimeSpan.Zero);
        }

        #endregion

        /// <summary>
        /// 获取对象
        /// </summary>
        /// <param name="param">主键ID</param>
        /// <returns></returns>
        public api_function Get(object param)
        {
            return Repository.Get<api_function>(param);
        }

        /// <summary>
        /// 保存对象
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public BoolResult Save(api_function param)
        {
            BoolResult result = new BoolResult();

            //保持对象的唯一性
            if (Repository.Exists<api_function>(e => (e.FunctionCode == param.FunctionCode || e.Name == param.Name) && e.Deleted == false))
            {
                result.Succeeded = false;
                result.Message = "已存在相同的Code值或Name值";
                return result;
            }

            object obj = Repository.Save<api_function>(param);
            if (obj != null)
            {
                result.Succeeded = true;

                //更新缓存
                SetAllFunctions();
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
        public BoolResult Update(api_function param)
        {
            BoolResult result = new BoolResult();

            api_function entity = Repository.First<api_function>(e => e.Id == param.Id && e.Deleted == false);
            if (entity == null)
            {
                result.Succeeded = false;
                result.Message = "修改的对象不存在";
                return result;
            }

            if (entity.FunctionCode != param.FunctionCode)
            {
                if (Repository.Exists<api_function>(e => e.FunctionCode == param.FunctionCode && e.Deleted == false))
                {
                    result.Succeeded = false;
                    result.Message = "已存在相同的Code";
                    return result;
                }
            }
            else if (entity.Name != param.Name)
            {
                if (Repository.Exists<api_function>(e => e.Name == param.Name && e.Deleted == false))
                {
                    result.Succeeded = false;
                    result.Message = "已存在相同的Name";
                    return result;
                }
            }

            entity.FunctionCode = param.FunctionCode;
            entity.Name = param.Name;
            entity.UpdatedBy = param.UpdatedBy;
            entity.UpdatedOn = param.UpdatedOn;
            Repository.Update<api_function>(entity);

            //更新缓存
            SetAllFunctions();

            result.Succeeded = true;
            return result;
        }

        /// <summary>
        /// 单个、批量删除对象
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public BoolResult Deleted(ApiFunctionExt param)
        {
            BoolResult result = new BoolResult();

            IList<api_function> adminSystemList = Repository.Find<api_function>(e => param.FunctionIds.Contains(e.Id) && e.Deleted == false);
            if (adminSystemList.Count == 0)
            {
                result.Succeeded = false;
                result.Message = "删除的对象不存在";
                return result;
            }
            else
            {
                using (ISession session = NHibernateHelper<api_function>.OpenSession())
                using (ITransaction transaction = session.BeginTransaction())
                {
                    try
                    {
                        int intRecord = 0;
                        foreach (var entity in adminSystemList)
                        {
                            //删除功能以及客户功能关系

                            entity.Deleted = true;
                            entity.UpdatedBy = param.UpdatedBy;
                            entity.UpdatedOn = param.UpdatedOn;
                            session.Update(entity);

                            IList<api_client_function> clientFunctions = Repository.Find<api_client_function>(a => a.FunctionId == entity.Id);
                            foreach(var clientFunction in clientFunctions)
                            {
                                session.Delete(clientFunction);
                            }

                            intRecord++;
                        }

                        transaction.Commit();

                        //更新缓存
                        SetAllFunctions();
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


        public api_function GetFunctionByCode(string code)
        {
            return GetAllFunctions().FirstOrDefault(a => a.FunctionCode == code);
        }

        /// <summary>
        /// 分页获取列表（提供按Code或Name进行模糊搜索，搜索时给Name属性赋值，不提供客户端自定义排序，默认按创建时间倒序排列）
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public IPagedList<api_function> GetPageList(IPagedParam<api_function> param)
        {
            IQueryable<api_function> queryable = Repository.AsQueryable<api_function>().Where(e => e.Deleted == false);
            if (!string.IsNullOrEmpty(param.model.Name))
            {
                queryable = queryable.Where(e => e.FunctionCode.Contains(param.model.Name) || e.Name.Contains(param.model.Name));
            }
            queryable = queryable.OrderByDescending(e => e.CreatedOn);

            return new PagedList<api_function>(queryable, param.PageIndex, param.PageSize);
        }

        /// <summary>
        /// 获取支持ZTree的JSON结构
        /// </summary>
        /// <param name="adminPermissionTreeList"></param>
        /// <returns></returns>
        protected string GetAdminPermissionZTreeJson(List<ApiPermissionTree> adminPermissionTreeList)
        {
            StringBuilder jsonBuilder = new StringBuilder();
            foreach (var obj in adminPermissionTreeList)
            {
                jsonBuilder.AppendFormat("{{ id: \"{0}\", name: \"{1}\", code: \"{2}\", parentId: \"{3}\"", obj.Id, obj.Name, obj.FunctionCode, obj.ParentId);
                if (obj.ChildApiPermissions.Count > 0)
                {
                    jsonBuilder.AppendFormat(", children: [{0}", GetAdminPermissionZTreeJson(obj.ChildApiPermissions));
                    jsonBuilder.Append("] ");
                }
                jsonBuilder.Append("}, ");
            }
            return jsonBuilder.ToString().Trim().TrimEnd(",".ToCharArray());
        }

        /// <summary>
        /// 获取各个系统包含的所有权限列表（支持ZTree的JSON结构----用于添加/更新用户组、添加/更新用户页面，供用户勾选组或用户对应的权限）
        /// </summary>
        /// <returns></returns>
        public string GetAllApiPermissionZTreeJson()
        {
            StringBuilder builder = new StringBuilder();

            IList<api_system> adminSystemList = ApiSystemManage.Instance.GetAllAdminSystems();
            IList<api_function> adminPermissionList = Repository.Find<api_function>(e => e.Deleted == false).ToList();

            foreach (var item in adminSystemList)
            {

                IList<api_function> adminPermissionListBySystem = adminPermissionList.Where(e => e.SystemId == item.Id).ToList();
                List<ApiPermissionTree> adminPermissionTreeList = GetApiPermissionTree(adminPermissionListBySystem, -1);

                string strZTreeJson = GetAdminPermissionZTreeJson(adminPermissionTreeList);

                if (!string.IsNullOrEmpty(strZTreeJson))
                {
                    strZTreeJson = "{ id: \"-1\", name: \"" + item.Name + "\", code: \"\", nocheck: true, children: [" + strZTreeJson + "] }, ";
                    builder.Append(strZTreeJson);
                }
            }

            string strJson = builder.ToString();
            strJson = strJson.Trim().TrimEnd(',');
            strJson = string.Format("[{0}]", strJson);

            return strJson;
        }

        /// <summary>
        /// 获取权限树
        /// </summary>
        /// <param name="adminPermissionList"></param>
        /// <param name="parentId"></param>
        /// <returns></returns>
        protected List<ApiPermissionTree> GetApiPermissionTree(IList<api_function> adminPermissionList, int parentId)
        {
            List<ApiPermissionTree> adminPermissionTreeList = new List<ApiPermissionTree>();

            var adminPermissions = from record in adminPermissionList
                                   where record.ParentId == parentId
                                   select record;
            foreach (var obj in adminPermissions)
            {
                ApiPermissionTree adminPermissionTree = new ApiPermissionTree();
                obj.CopyTo(adminPermissionTree);
                adminPermissionTreeList.Add(adminPermissionTree);
                adminPermissionTree.ChildApiPermissions.AddRange(GetApiPermissionTree(adminPermissionList, adminPermissionTree.Id));
            }
            return adminPermissionTreeList;
        }

        /// <summary>
        /// 获取指定系统对应包含的所有权限列表（支持ZTree的JSON结构----用于添加/更新权限页面，选择系统时列出该系统下包含的权限树）
        /// </summary>
        /// <param name="intSystemId"></param>
        /// <returns></returns>
        public string GetSpecifySystemPermissionZTreeJson(int intSystemId)
        {
            IList<api_function> adminPermissionList = Repository.Find<api_function>(e => e.Deleted == false && e.SystemId == intSystemId).ToList();
            List<ApiPermissionTree> adminPermissionTreeList = GetApiPermissionTree(adminPermissionList, -1);

            string strZTreeJson = GetAdminPermissionZTreeJson(adminPermissionTreeList);

            strZTreeJson = strZTreeJson.Trim().TrimEnd(',');
            strZTreeJson = string.Format("[{0}]", strZTreeJson);

            return strZTreeJson;
        }
    }
}
