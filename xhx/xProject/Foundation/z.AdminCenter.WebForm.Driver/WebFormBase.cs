using z.Foundation;
using z.Foundation.Data;
using z.Foundation.LogicInvoke;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminCenter.WebForm.Driver
{
    public class WebFormBase : System.Web.UI.Page
    {
        #region 业务逻辑调用方法

        /// <summary>
        /// 业务逻辑调用
        /// </summary>
        /// <param name="assemblyName">程序集名称(e.g. com.buygego.Logic.dll)</param>
        /// <param name="className">类名(e.g. com.buygego.Logic.ClassName)</param>
        /// <param name="methodName">方法名(e.g. MethodName)</param>
        /// <param name="param">参数</param>
        /// <returns></returns>
        public IResponse CallLogic(string assemblyName, string className, string methodName, object param)
        {
            IRequest request = new Request()
            {
                Target = assemblyName,
                Class = className,
                Method = methodName,
                Parameter = param
            };
            return LogicInvoker.Instance.Call(request);
        }

        /// <summary>
        /// 业务逻辑调用
        /// </summary>
        /// <typeparam name="T1">参数类型</typeparam>
        /// <typeparam name="T2">结果集类型</typeparam>
        /// <param name="assemblyName">程序集名称(e.g. com.buygego.Logic.dll)</param>
        /// <param name="className">类名(e.g. com.buygego.Logic.ClassName)</param>
        /// <param name="methodName">方法名(e.g. MethodName)</param>
        /// <param name="param">参数</param>
        /// <returns></returns>
        public IResponse<T2> CallLogic<T1, T2>(string assemblyName, string className, string methodName, T1 param)
        {
            IRequest<T1> request = new Request<T1>()
            {
                Target = assemblyName,
                Class = className,
                Method = methodName,
                Parameter = param
            };
            return LogicInvoker.Instance.Call<T1, T2>(request);
        }

        #endregion

        /// <summary>
        /// URL不存在公共跳转方法（页面跳转、AJAX请求）
        /// </summary>
        protected void _404()
        {
            bool bAjaxRequest = false;
            if (!string.IsNullOrEmpty(Request.Params["AjaxRequest"]) && Request.Params["AjaxRequest"] == "true")
            {
                bAjaxRequest = true;
            }

            if (!bAjaxRequest)
            {
                //跳转至404页面
                Response.Redirect(string.Format("http://{0}/404.html", AdminCenterDomain));
            }
            else
            {
                JsonResult jsonResult = new JsonResult();
                jsonResult.Succeeded = false;
                jsonResult.MsgType = JsonResult.MessageType.error.ToString();
                jsonResult.Message = "URL不存在";
                Response.Write(jsonResult.JsonSerialize());
                Response.End();
            }
        }

        /// <summary>
        /// 获取分页工具条
        /// </summary>
        /// <param name="param">IPagedList对象</param>
        /// <returns></returns> 
        public string GetPager<T>(IPagedList<T> param)
        {
            string strPager = "";
            int showPages = 5;
            int midPos = 2;
            int forStart = 1;

            if (param.TotalPages > 1)
            {
                string url = Request.Url.AbsolutePath;
                string queryString = Request.QueryString.ToString().RegexReplace(@"&?page=(\d+)?", "");
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

        #region 获取配置

        /// <summary>
        /// AdminCenter站点域名
        /// </summary>
        protected string AdminCenterDomain
        {
            get
            {
                return Utility.GetConfigValue("AdminCenterDomain");
            }
        }

        /// <summary>
        /// 安全校验码
        /// </summary>
        protected string Key
        {
            get
            {
                return Utility.GetConfigValue("Key");
            }
        }

        /// <summary>
        /// 当前系统Code
        /// </summary>
        protected string SystemCode
        {
            get
            {
                return Utility.GetConfigValue("SystemCode");
            }
        }

        #endregion
    }
}
