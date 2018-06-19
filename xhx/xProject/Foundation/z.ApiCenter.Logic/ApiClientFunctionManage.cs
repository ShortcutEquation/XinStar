using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using z.ApiCenter.Entity;
using z.Foundation.Cache;
using z.Logic.Base;

namespace z.ApiCenter.Logic
{
    public class ApiClientFunctionManage: NHLogicBase
    {
        public readonly static ApiClientFunctionManage Instance = new ApiClientFunctionManage();

        #region 缓存
        private static readonly object lockObj = new object();

        /// <summary>
        /// 获取所有Client缓存
        /// </summary>
        /// <returns></returns>
        public IList<api_client_function> GetAllClientFunctions()
        {
            ICache cache = new HttpCache();
            object obj = cache.GetCache("api_client_function");
            if (obj == null)
            {
                lock (lockObj)
                {
                    obj = cache.GetCache("api_client_function");
                    if (obj == null)
                    {
                        obj = Repository.AsQueryable<api_client_function>().ToList();
                        cache.SetCache("api_client_function", obj, TimeSpan.Zero);
                    }
                }
            }

            return (List<api_client_function>)obj;
        }

        /// <summary>
        /// 设置ClientFunction缓存
        /// </summary>
        public void SetAllClientFunctions()
        {
            ICache cache = new HttpCache();
            object obj = Repository.AsQueryable<api_client_function>().ToList();
            cache.SetCache("api_client_function", obj, TimeSpan.Zero);
        }

        #endregion

        public bool IsHasPermission(int clientId,int functionId)
        {
            return GetAllClientFunctions().Count(a => a.ClientId == clientId && a.FunctionId == functionId) > 0;
        }
    }
}
