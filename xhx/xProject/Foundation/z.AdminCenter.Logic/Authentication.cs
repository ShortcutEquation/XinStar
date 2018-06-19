using z.AdminCenter.Entity;
using z.Foundation;
using z.Foundation.Cache;
using z.Foundation.Data;
using z.Logic.Base;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Text;

namespace z.AdminCenter.Logic
{
    public class Authentication : AuthenticationBase
    {
        public BoolResult Verify(NameValueCollection param)
        {
            BoolResult result = new BoolResult();

            //对参数进行排序
            List<string> keys = param.AllKeys.OrderBy(e => e).ToList();

            //判断是否包含必需的参数、参数格式是否正确
            DateTime dtAuthTime = DateTime.Now;
            if (keys.IndexOf("sign") < 0 || keys.IndexOf("authCode") < 0 || keys.IndexOf("authTime") < 0 || !DateTime.TryParse(param["authTime"], out dtAuthTime))
            {
                result.Succeeded = false;
                result.Message = "ERROR_PARAM";

                return result;
            }

            //验证签名有效期
            if (DateTime.Now.Subtract(dtAuthTime).TotalMinutes > int.Parse(Utility.GetConfigValue("AuthExpired")))
            {
                result.Succeeded = false;
                result.Message = "AUTH_EXPIRED";

                return result;
            }

            //验证Code是否存在
            var strAuthCode = param["authCode"];
            auth_guest _auth_guest = GetAllAuthGuest().FirstOrDefault(e => e.Code == strAuthCode);
            if (_auth_guest == null)
            {
                result.Succeeded = false;
                result.Message = "UNAUTH_GUEST";

                return result;
            }

            //验证数据签名是否一致
            StringBuilder builder = new StringBuilder();
            foreach (var item in keys)
            {
                if (item == "sign")
                {
                    continue;
                }

                builder.AppendFormat("&{0}={1}", item, param[item]);
            }
            string strPostData = builder.ToString().TrimStart('&');
            string strSign = (strPostData + _auth_guest.AuthKey).MD5Encrypt("utf-8");
            if (strSign != param["sign"])
            {
                result.Succeeded = false;
                result.Message = "UNAUTH_KEY";

                return result;
            }
            
            result.Succeeded = true;
            return result;
        }
    }

    public class AuthenticationBase : NHLogicBase
    {
        private static readonly object lock_obj = new object();

        /// <summary>
        /// 获取所有访客身份认证数据，并缓存（缓存时间为1天）
        /// </summary>
        /// <returns></returns>
        public IList<auth_guest> GetAllAuthGuest()
        {
            ICache cache = new HttpCache();
            string strKey = "AllAuthGuest";
            object obj = cache.GetCache(strKey);

            if (obj == null)
            {
                lock (lock_obj)
                {
                    obj = cache.GetCache(strKey);
                    if (obj == null)
                    {
                        DataTable dataTable = MsSqlHelper.ExecuteDataTable("AdminCenterDB", "SELECT * FROM auth_guest WHERE Disabled = 0 AND Deleted = 0", CommandType.Text, null);
                        IList<auth_guest> listAuthGuest = dataTable.DataTableToModel<auth_guest>();
                        cache.SetCache(strKey, listAuthGuest, TimeSpan.FromDays(1));

                        return listAuthGuest;
                    }
                }
            }

            return obj as IList<auth_guest>;
        }
    }
}
