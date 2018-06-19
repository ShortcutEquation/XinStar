using z.AdminCenter.Entity;
using z.Foundation;
using z.Foundation.Data;
using z.AdminCenter.Logic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace z.AdminCenter.MVC.Driver
{
    public class ControllerAdminBase : ControllerBase
    {
        /// <summary>
        /// 当前用户对象
        /// </summary>
        public static AdminUserExt AdminUser
        {
            get
            {
                return (AdminUserExt)System.Web.HttpContext.Current.Session["AdminUser"];
            }
        }

        #region 页面内容

        /// <summary>
        /// 修改个人信息URL地址
        /// </summary>
        public static string MyProfileURL
        {
            get
            {
                return string.Format("http://{0}/MyProfile.aspx?code={1}&to={2}", AdminCenterDomain, AdminUser.Ticket, System.Web.HttpContext.Current.Server.UrlEncode(System.Web.HttpContext.Current.Request.Url.AbsoluteUri));
            }
        }

        /// <summary>
        /// 锁屏URL地址
        /// </summary>
        public static string LockURL
        {
            get
            {
                return string.Format("http://{0}/Lock.aspx?code={1}&to={2}", AdminCenterDomain, AdminUser.Ticket, System.Web.HttpContext.Current.Server.UrlEncode(System.Web.HttpContext.Current.Request.Url.AbsoluteUri));
            }
        }

        /// <summary>
        /// 登出URL地址
        /// </summary>
        public static string LogoutURL
        {
            get
            {
                return string.Format("http://{0}/Logout.aspx?code={1}&to={2}", AdminCenterDomain, AdminUser.Ticket, System.Web.HttpContext.Current.Server.UrlEncode(System.Web.HttpContext.Current.Request.Url.AbsoluteUri));
            }
        }

        /// <summary>
        /// 系统Logo
        /// </summary>
        public static string SystemLogo
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

        /// <summary>
        /// 获取上部导航
        /// </summary>
        /// <returns></returns>
        public static string GetTopNavigation()
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

        /// <summary>
        /// 获取上部导航用户头像
        /// </summary>
        /// <returns></returns>
        public static string GetTopUserLogo()
        {
            StringBuilder html = new StringBuilder();

            if (!string.IsNullOrEmpty(AdminUser.Logo))
            {
                html.AppendFormat("<img alt=\"\" src=\"http://{0}/{1}\" />", AdminCenterDomain, UploadFile.GetThumbnail(AdminUser.Logo, "s1"));
            }

            return html.ToString();
        }

        #region 获取左侧导航

        /// <summary>
        /// 获取左侧导航
        /// </summary>
        /// <returns></returns>
        public static string GetLeftNavigation()
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

        static string ReturnNavigationTree(List<AdminPermissionTree> adminPermissionTreeList, int level)
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

        #endregion

        /// <summary>
        /// 获取分页工具条
        /// </summary>
        /// <param name="param">IPagedList对象</param>
        /// <returns></returns> 
        public static string GetPager<T>(IPagedList<T> param)
        {
            string strPager = "";
            int showPages = 5;
            int midPos = 2;
            int forStart = 1;

            if (param.TotalPages > 1)
            {
                string url = System.Web.HttpContext.Current.Request.Url.AbsolutePath;
                string queryString = System.Web.HttpContext.Current.Request.QueryString.ToString().RegexReplace(@"&?page=(\d+)?", "");
                queryString = queryString.Trim('&');
                string pageQueryString = (queryString.Length > 0 ? "&" : "") + "page=";
                url = string.Format("{0}?{1}{2}", url, queryString, pageQueryString);

                StringBuilder pageHtml = new StringBuilder();

                string previousPageLink = "<li class=\"prev disabled\"><a href=\"#\" title=\"Prev\"><i class=\"fa fa-angle-left\"></i></a></li>";
                if (param.HasPreviousPage)
                {
                    previousPageLink = string.Format("<li class=\"prev\"><a href=\"{0}{1}\" title=\"Prev\"><i class=\"fa fa-angle-left\"></i></a></li>", url, param.PageIndex - 1);
                }
                pageHtml.Append(previousPageLink);

                if (param.TotalPages > showPages)
                {
                    if (param.PageIndex <= midPos)
                    {
                        forStart = 1;
                    }
                    else if (param.PageIndex + midPos > param.TotalPages)
                    {
                        forStart = param.PageIndex - (showPages - (param.TotalPages - param.PageIndex)) + 1;
                    }
                    else
                    {
                        forStart = param.PageIndex - midPos;
                    }
                }

                int forMax = forStart + showPages - 1;
                if (forMax > param.TotalPages)
                {
                    forMax = param.TotalPages;
                }

                for (int i = forStart; i <= forMax; i++)
                {
                    if (param.PageIndex == i)
                    {
                        pageHtml.AppendFormat("<li class=\"active\"><a href=\"#\">{0}</a></li>", i);
                    }
                    else
                    {
                        pageHtml.AppendFormat("<li><a href=\"{0}{1}\">{1}</a></li>", url, i);
                    }
                }

                string nextPageLink = "<li class=\"next disabled\"><a href=\"#\" title=\"Next\"><i class=\"fa fa-angle-right\"></i></a></li>";
                if (param.HasNextPage)
                {
                    nextPageLink = string.Format("<li class=\"next\"><a href=\"{0}{1}\" title=\"Next\"><i class=\"fa fa-angle-right\"></i></a></li>", url, param.PageIndex + 1);
                }
                pageHtml.Append(nextPageLink);

                //pageHtml.AppendFormat("<li><span>{0}</span></li>", param.TotalPages);

                strPager = string.Format("<ul class=\"pagination\" style=\"visibility: visible;\">{0}</ul>", pageHtml.ToString());
            }

            return strPager;
        }

        /// <summary>
        /// View页面权限检查
        /// </summary>
        /// <param name="permissionCode"></param>
        /// <returns></returns>
        public static bool CheckPermission(string permissionCode)
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

            return bOwnPermission;
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //浏览器版本判断
            if (filterContext.HttpContext.Request.Browser.Browser.ToLower() == "ie" && (filterContext.HttpContext.Request.Browser.MajorVersion == 6 || filterContext.HttpContext.Request.Browser.MajorVersion == 7 || filterContext.HttpContext.Request.Browser.MajorVersion == 8))
            {
                filterContext.Result = Redirect(string.Format("http://{0}/bury-ie6.html", AdminCenterDomain));
                return;
            }

            HttpCookie cookie = filterContext.HttpContext.Request.Cookies["Ticket"];
            if (cookie == null)
            {
                filterContext.Result = Redirect(string.Format("http://{0}/Login.aspx?to={1}", AdminCenterDomain, filterContext.HttpContext.Server.UrlEncode(filterContext.HttpContext.Request.Url.AbsoluteUri)));
                return;
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
            if (!string.IsNullOrEmpty(filterContext.HttpContext.Request.Params["AjaxRequest"]) && filterContext.HttpContext.Request.Params["AjaxRequest"] == "true")
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
                    z.Foundation.Data.JsonResult jsonResult = new z.Foundation.Data.JsonResult();
                    jsonResult.Succeeded = false;
                    jsonResult.MsgType = z.Foundation.Data.JsonResult.MessageType.error.ToString();
                    jsonResult.Message = strJsonMsg;

                    filterContext.Result = new System.Web.Mvc.JsonResult() { Data = jsonResult };
                    return;
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
                        filterContext.Result = Redirect(string.Format("http://{0}/Login.aspx?to={1}&errorCode={2}", AdminCenterDomain, filterContext.HttpContext.Server.UrlEncode(filterContext.HttpContext.Request.Url.AbsoluteUri), strResultCode));
                        return;
                    case "LOCKED":
                        //跳转至解锁页面
                        filterContext.Result = Redirect(string.Format("http://{0}/UnLock.aspx?code={1}&to={2}", AdminCenterDomain, strTicket, filterContext.HttpContext.Server.UrlEncode(filterContext.HttpContext.Request.Url.AbsoluteUri)));
                        return;
                    case "NOT_FOUND":
                        //跳转至404页面
                        filterContext.Result = Redirect(string.Format("http://{0}/404.html", AdminCenterDomain));
                        return;
                    case "SERVER_ERROR":
                        //跳转至500页面
                        filterContext.Result = Redirect(string.Format("http://{0}/500.html", AdminCenterDomain));
                        return;
                }
            }

            #endregion

            #region 反序列化结果集字符串

            try
            {
                AdminUserExt adminUser = strResultCode.JsonDeserialize<AdminUserExt>();
                adminUser.Ticket = strTicket;

                Session["AdminUser"] = adminUser;
            }
            catch
            {
                filterContext.Result = Redirect(string.Format("http://{0}/500.html", AdminCenterDomain));
                return;
            }

            #endregion

            base.OnActionExecuting(filterContext);
        }
    }
}
