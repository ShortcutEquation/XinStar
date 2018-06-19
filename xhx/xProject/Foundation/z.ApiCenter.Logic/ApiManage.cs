using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using z.ApiCenter.Entity;
using z.Foundation;
using z.Foundation.Data;

namespace z.ApiCenter.Logic
{
    public class ApiManage
    {
        /// <summary>
        /// 验证签名以及权限
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public BoolResult VerifySign(VerifySignParam param)
        {
            BoolResult boolResult = new BoolResult();
            api_client apiClient;

            #region 基本参数校验
            //判断时间戳是10位还是13位（10位单位为秒，13位为毫秒）
            long systemTime = param.Timestamp.ToString().Length > 10 ? (long)Math.Floor((DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalMilliseconds) : (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000;
            long timeDiff = param.Timestamp.ToString().Length > 10 ? 1800000 : 1800;
            //时间差大于30分钟
            if ((systemTime - param.Timestamp) > timeDiff)
            {
                boolResult.Message = "时间戳已过期";
                return boolResult;
            }

            if (string.IsNullOrEmpty(param.Access_Key))
            {
                boolResult.Message = "不存在AccessKey参数";
                return boolResult;
            }

            if (string.IsNullOrEmpty(param.Access_Function))
            {
                boolResult.Message = "不存在功能模块";
                return boolResult;
            }

            if (string.IsNullOrEmpty(param.Sign))
            {
                boolResult.Message = "不存在Sign参数";
                return boolResult;
            }

            apiClient = ApiClientManage.Instance.GetClientByKey(param.Access_Key);
            if (apiClient == null)
            {
                boolResult.Message = "不存在AccessKey参数";
                return boolResult;
            }

            #endregion

            #region  签名验证 

            string builderSignParam = "";
            var urlParams = param.UrlParams.OrderBy(a => a.Key);
            foreach (var urlParam in urlParams)
            {
                if (urlParam.Key.ToLower() != "sign" && urlParam.Key.ToLower() != "access_function")
                {
                    builderSignParam += urlParam.Key + urlParam.Value;
                }
            }
            builderSignParam += apiClient.SecretKey;
            builderSignParam = builderSignParam.SHA1_Hash().ToLower();
            if (param.Sign.ToLower() != builderSignParam)
            {
                boolResult.Message = "签名不正确";
                return boolResult;
            }

            #endregion

            #region 权限验证

            api_function apiFunction = ApiFunctionManage.Instance.GetFunctionByCode(param.Access_Function);
            if (apiFunction == null)
            {
                boolResult.Message = "功能不存在";
                return boolResult;
            }

            if (!ApiClientFunctionManage.Instance.IsHasPermission(apiClient.Id, apiFunction.Id))
            {
                boolResult.Message = "无权限访问";
                return boolResult;
            }

            #endregion

            boolResult.Succeeded = true;
            return boolResult;
        }
    }
}