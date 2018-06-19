using z.Foundation;
using z.AdminCenter.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace z.AdminCenter.MVC.Driver
{
    public class CheckPermissionAttribute : ActionFilterAttribute
    {
        public CheckPermissionAttribute(string permissionCode)
        {
            PermissionCode = permissionCode;
        }

        string PermissionCode
        {
            get; set;
        }
        
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            bool bOwnPermission = false;
            if (!string.IsNullOrEmpty(PermissionCode))
            {
                if (ControllerAdminBase.AdminUser != null)
                {
                    if (ControllerAdminBase.AdminUser.IsSuperUser)
                    {
                        bOwnPermission = true;
                    }
                    else if (ControllerAdminBase.AdminUser.OwnPermissionCodes.Contains(PermissionCode))
                    {
                        bOwnPermission = true;
                    }
                }
            }

            if (!bOwnPermission)
            {
                bool bAjaxRequest = false;
                if (!string.IsNullOrEmpty(filterContext.HttpContext.Request.Params["AjaxRequest"]) && filterContext.HttpContext.Request.Params["AjaxRequest"] == "true")
                {
                    bAjaxRequest = true;
                }

                if (!bAjaxRequest)
                {
                    //跳转至403页面
                    filterContext.Result = new RedirectResult(string.Format("http://{0}/403.html", Utility.GetConfigValue("AdminCenterDomain")));
                    return;
                }
                else
                {
                    z.Foundation.Data.JsonResult jsonResult = new z.Foundation.Data.JsonResult();
                    jsonResult.Succeeded = false;
                    jsonResult.MsgType = z.Foundation.Data.JsonResult.MessageType.error.ToString();
                    jsonResult.Message = "无操作权限";

                    filterContext.Result = new JsonResult() { Data = jsonResult };
                    return;
                }
            }

            base.OnActionExecuting(filterContext);
        }
    }
}
