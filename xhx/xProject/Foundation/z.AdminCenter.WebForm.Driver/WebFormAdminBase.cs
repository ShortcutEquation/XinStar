using z.AdminCenter.Entity;
using z.Foundation;
using z.Foundation.Data;
using z.Foundation.LogicInvoke;
using z.AdminCenter.Logic;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace AdminCenter.WebForm.Driver
{
    public class WebFormAdminBase : WebFormBase
    {
        protected AdminUserExt AdminUser
        {
            get;
            set;
        }

        #region 页面内容

        /// <summary>
        /// 修改个人信息URL地址
        /// </summary>
        protected string MyProfileURL
        {
            get
            {
                return string.Format("http://{0}/MyProfile.aspx?code={1}&to={2}", AdminCenterDomain, AdminUser.Ticket, Server.UrlEncode(Request.Url.AbsoluteUri));
            }
        }

        /// <summary>
        /// 锁屏URL地址
        /// </summary>
        protected string LockURL
        {
            get
            {
                return string.Format("http://{0}/Lock.aspx?code={1}&to={2}", AdminCenterDomain, AdminUser.Ticket, Server.UrlEncode(Request.Url.AbsoluteUri));
            }
        }

        /// <summary>
        /// 登出URL地址
        /// </summary>
        protected string LogoutURL
        {
            get
            {
                return string.Format("http://{0}/Logout.aspx?code={1}&to={2}", AdminCenterDomain, AdminUser.Ticket, Server.UrlEncode(Request.Url.AbsoluteUri));
            }
        }

        /// <summary>
        /// 系统Logo
        /// </summary>
        protected string SystemLogo
        {
            get
            {
                string strHtml = "<a class=\"navbar-brand text-logo\" style=\"color:white\" href=\"/\">{0}</a>";

                admin_system _admin_system = AdminUser.OwnAdminSystems.Where(e => e.Code == SystemCode).FirstOrDefault();
                if (_admin_system != null)
                {
                    if (!string.IsNullOrEmpty(_admin_system.Logo))
                    {
                        return string.Format(strHtml, string.Format("<img alt=\"logo\" src=\"http://{0}/{1}\" class=\"img-responsive\" />", AdminCenterDomain, UploadFile.GetThumbnail(_admin_system.Logo, "s1")));
                    }
                    else
                    {
                        return string.Format(strHtml, _admin_system.Name);
                    }
                }
                else
                {
                    return string.Format(strHtml, "");
                }
            }
        }

        #region 获取左侧导航

        /// <summary>
        /// 获取左侧导航
        /// </summary>
        /// <returns></returns>
        protected string GetLeftNavigation()
        {
            StringBuilder html = new StringBuilder();

            if (AdminUser.MenuTrees.ContainsKey(SystemCode))
            {
                List<AdminPermissionTree> trees = AdminUser.MenuTrees[SystemCode];
                if (trees.Count > 0)
                {
                    html.Append("<ul class=\"page-sidebar-menu\" data-auto-scroll=\"true\" data-slide-speed=\"200\">");
                    html.Append(ReturnNavigationTree(trees, 0));
                    html.Append("</ul>");
                }
            }

            return html.ToString(); ;
        }

        private string ReturnNavigationTree(List<AdminPermissionTree> adminPermissionTreeList, int level)
        {
            StringBuilder html = new StringBuilder();
            foreach (var obj in adminPermissionTreeList)
            {
                html.AppendFormat("<li data-code=\"{4}\"><a href=\"{0}\" {3}><i class=\"fa {1}\"></i>{2}<span class=\"arrow \"></span></a>", obj.IsLink ? obj.Url : "javascript:;", obj.Img, level == 0 ? string.Format("<span class=\"title\">{0}</span>", obj.Name) : obj.Name, obj.IsLink ? string.Format("target=\"{0}\"", obj.Target) : "", obj.PermissionCode);
                if (obj.ChildAdminPermissions.Count > 0)
                {
                    html.AppendFormat("<ul class=\"sub-menu\">{0}", ReturnNavigationTree(obj.ChildAdminPermissions, 1));
                    html.Append("</ul>");
                }
                html.Append("</li>");
            }
            return html.ToString();
        }

        #endregion

        #region 获取Top工具条

        /// <summary>
        /// 获取上部导航
        /// </summary>
        /// <returns></returns>
        protected string GetTopNavigation()
        {
            StringBuilder html = new StringBuilder();
            if (AdminUser.OwnAdminSystems.Count > 0)
            {
                html.Append("<ul class=\"nav navbar-nav\">");
                foreach (var item in AdminUser.OwnAdminSystems)
                {
                    html.AppendFormat("<li class=\"classic-menu-dropdown {0}\"><a href=\"{1}\">{2}{3}</a></li>", SystemCode == item.Code ? "active" : "", item.URL, item.Name, SystemCode == item.Code ? "<span class=\"selected\"></span>" : "");
                }
                html.Append("</ul>");
            }
            return html.ToString();
        }

        protected string GetTopUserLogo()
        {
            StringBuilder html = new StringBuilder();

            if (!string.IsNullOrEmpty(AdminUser.Logo))
            {
                html.AppendFormat("<img alt=\"\" src=\"http://{0}/{1}\" />", AdminCenterDomain, UploadFile.GetThumbnail(AdminUser.Logo, "s1"));
            }

            return html.ToString();
        }

        #endregion

        #endregion

        /// <summary>
        /// 权限验证
        /// </summary>
        /// <param name="permissionCode"></param>
        /// <param name="pageLevel">是否属于页面级权限验证</param>
        /// <returns></returns>
        protected bool CheckPermission(string permissionCode, bool pageLevel)
        {
            bool bOwnPermission = false;
            if (!string.IsNullOrEmpty(permissionCode))
            {
                if (AdminUser != null)
                {
                    if (AdminUser.IsSuperUser)
                    {
                        bOwnPermission = true;
                    }
                    else if (AdminUser.OwnPermissionCodes.Contains(permissionCode))
                    {
                        bOwnPermission = true;
                    }
                }
            }

            if (pageLevel && !bOwnPermission)
            {
                bool bAjaxRequest = false;
                if (!string.IsNullOrEmpty(Request.Params["AjaxRequest"]) && Request.Params["AjaxRequest"] == "true")
                {
                    bAjaxRequest = true;
                }

                if (!bAjaxRequest)
                {
                    //跳转至403页面
                    Response.Redirect(string.Format("http://{0}/403.html", AdminCenterDomain));
                }
                else
                {
                    JsonResult jsonResult = new JsonResult();
                    jsonResult.Succeeded = false;
                    jsonResult.MsgType = JsonResult.MessageType.error.ToString();
                    jsonResult.Message = "无操作权限";
                    Response.Write(jsonResult.JsonSerialize());
                    Response.End();
                }
            }

            return bOwnPermission;
        }

        public override void ProcessRequest(System.Web.HttpContext context)
        {
            //浏览器版本判断
            if (context.Request.Browser.Browser.ToLower() == "ie" && (context.Request.Browser.MajorVersion == 6 || context.Request.Browser.MajorVersion == 7 || context.Request.Browser.MajorVersion == 8))
            {
                context.Response.Redirect(string.Format("http://{0}/bury-ie6.html", AdminCenterDomain));
            }

            HttpCookie cookie = context.Request.Cookies["Ticket"];
            if (cookie == null)
            {
                context.Response.Redirect(string.Format("http://{0}/Login.aspx?to={1}", AdminCenterDomain, context.Server.UrlEncode(context.Request.Url.AbsoluteUri)));
            }

            string strResultCode = "";
            string strTicket = cookie.Value;

            #region 模拟请求

            try
            {
                string strPostData = string.Format("system_code={0}&ticket={1}", SystemCode, strTicket);
                string strSign = (strPostData + Key).MD5Encrypt("utf-8");
                strPostData += "&sign=" + strSign;
                byte[] bytePostData = Encoding.GetEncoding("utf-8").GetBytes(strPostData);

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(string.Format("http://{0}/AuthAPI.aspx", AdminCenterDomain));
                request.Proxy = new WebProxy();
                request.UserAgent = "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/27.0.1453.93 Safari/537.36";
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = bytePostData.Length;
                using (Stream requestStream = request.GetRequestStream())
                {
                    requestStream.Write(bytePostData, 0, bytePostData.Length);
                }

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream responseStream = response.GetResponseStream())
                using (StreamReader streamReader = new StreamReader(responseStream, Encoding.UTF8))
                {
                    strResultCode = streamReader.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                strResultCode = "SERVER_ERROR";
                Logger.Error(ex.InnerException == null ? ex.Message : ex.InnerException.Message, ex);
            }

            #endregion

            #region 辨别是否为Ajax请求

            bool bAjaxRequest = false;
            if (!string.IsNullOrEmpty(context.Request.Params["AjaxRequest"]) && context.Request.Params["AjaxRequest"] == "true")
            {
                bAjaxRequest = true;
            }

            #endregion

            #region 返回值判断

            if (bAjaxRequest)
            {
                string strJsonMsg = "";
                switch (strResultCode)
                {
                    case "ERROR_PARAM":
                        strJsonMsg = "参数错误";
                        break;
                    case "UNAUTH_SYSTEM":
                        strJsonMsg = "未授权的SystemCode";
                        break;
                    case "UNAUTH_KEY":
                        strJsonMsg = "未授权的Key";
                        break;
                    case "LOGIN_FAIL":
                        strJsonMsg = "用户会话已过期，请重新登录";
                        break;
                    case "OFFLINE":
                        strJsonMsg = "您的账号已在别处登录，被迫下线";
                        break;
                    case "LOCKED":
                        strJsonMsg = "用户会话已锁定，请解锁后操作";
                        break;
                    case "UNAUTH_USER":
                        strJsonMsg = "用户未被授权登录当前系统";
                        break;
                    case "NOT_FOUND":
                        strJsonMsg = "未找到请求的页面";
                        break;
                    case "SERVER_ERROR":
                        strJsonMsg = "系统错误，请稍候重试";
                        break;
                }

                if (!string.IsNullOrEmpty(strJsonMsg))
                {
                    JsonResult jsonResult = new JsonResult();
                    jsonResult.Succeeded = false;
                    jsonResult.MsgType = JsonResult.MessageType.error.ToString();
                    jsonResult.Message = strJsonMsg;

                    context.Response.Write(jsonResult.JsonSerialize());
                    context.Response.End();
                }
            }
            else
            {
                switch (strResultCode)
                {
                    case "ERROR_PARAM":
                    case "UNAUTH_SYSTEM":
                    case "UNAUTH_KEY":
                    case "LOGIN_FAIL":
                    case "OFFLINE":
                    case "UNAUTH_USER":
                        //跳转至登录页面
                        context.Response.Redirect(string.Format("http://{0}/Login.aspx?to={1}&errorCode={2}", AdminCenterDomain, context.Server.UrlEncode(context.Request.Url.AbsoluteUri), strResultCode));
                        break;
                    case "LOCKED":
                        //跳转至解锁页面
                        context.Response.Redirect(string.Format("http://{0}/UnLock.aspx?code={1}&to={2}", AdminCenterDomain, strTicket, context.Server.UrlEncode(context.Request.Url.AbsoluteUri)));
                        break;
                    case "NOT_FOUND":
                        //跳转至404页面
                        context.Response.Redirect(string.Format("http://{0}/404.html", AdminCenterDomain));
                        break;
                    case "SERVER_ERROR":
                        //跳转至500页面
                        context.Response.Redirect(string.Format("http://{0}/500.html", AdminCenterDomain));
                        break;
                }
            }

            #endregion

            #region 反序列化结果集字符串

            try
            {
                AdminUser = strResultCode.JsonDeserialize<AdminUserExt>();
                AdminUser.Ticket = strTicket;
            }
            catch
            {
                context.Response.Redirect(string.Format("http://{0}/500.html", AdminCenterDomain));
            }

            #endregion

            base.ProcessRequest(context);
        }
    }
}
