using z.Foundation;
using z.Foundation.Data;
using z.Foundation.LogicInvoke;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace z.AdminCenter.MVC.Driver
{
    public class ControllerBase : Controller
    {
        #region 读取配置文件中appSettings节点中的值

        /// <summary>
        /// 基础域
        /// </summary>
        public static string BaseDomain
        {
            get
            {
                return string.Format("http://{0}", Utility.GetConfigValue("BaseDomain"));
            }
        }

        /// <summary>
        /// 主域
        /// </summary>
        public static string Domain
        {
            get
            {
                return string.Format("http://{0}", Utility.GetConfigValue("Domain"));
            }
        }

        /// <summary>
        /// 图片域
        /// </summary>
        public static string ImgDomain
        {
            get
            {
                return string.Format("http://{0}", Utility.GetConfigValue("ImgDomain"));
            }
        }

        /// <summary>
        /// 资源文件域
        /// </summary>
        public static string ResourceDomain
        {
            get
            {
                return string.Format("http://{0}", Utility.GetConfigValue("ResourceDomain"));
            }
        }

        /// <summary>
        /// 资源文件版本号
        /// </summary>
        public static string ResourceVersion
        {
            get
            {
                return Utility.GetConfigValue("ResourceVersion");
            }
        }

        public static string CollectUploadUrl
        {
            get
            {
                return Utility.GetConfigValue("CollectUploadUrl"); 
            }
        }

        #endregion

        #region 业务逻辑调用方法

        /// <summary>
        /// 业务逻辑调用
        /// </summary>
        /// <param name="assemblyName">程序集名称(e.g. com.buygego.Logic.dll)</param>
        /// <param name="className">类名(e.g. com.buygego.Logic.ClassName)</param>
        /// <param name="methodName">方法名(e.g. MethodName)</param>
        /// <param name="param">参数</param>
        /// <returns></returns>
        protected IResponse CallLogic(string assemblyName, string className, string methodName, object param)
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
        protected IResponse<T2> CallLogic<T1, T2>(string assemblyName, string className, string methodName, T1 param)
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
        protected ActionResult _404()
        {
            bool bAjaxRequest = false;
            if (!string.IsNullOrEmpty(Request.Params["AjaxRequest"]) && Request.Params["AjaxRequest"] == "true")
            {
                bAjaxRequest = true;
            }

            if (!bAjaxRequest)
            {
                //跳转至404页面
                return Redirect(string.Format("http://{0}/404.html", AdminCenterDomain));
            }
            else
            {
                z.Foundation.Data.JsonResult jsonResult = new z.Foundation.Data.JsonResult();
                jsonResult.Succeeded = false;
                jsonResult.MsgType = z.Foundation.Data.JsonResult.MessageType.error.ToString();
                jsonResult.Message = "URL不存在";

                return Json(jsonResult);
            }
        }

        #region AdminCenter配置

        /// <summary>
        /// AdminCenter站点域名
        /// </summary>
        public static string AdminCenterDomain
        {
            get
            {
                return Utility.GetConfigValue("AdminCenterDomain");
            }
        }

        /// <summary>
        /// 安全校验码
        /// </summary>
        protected static string Key
        {
            get
            {
                return Utility.GetConfigValue("Key");
            }
        }

        /// <summary>
        /// 当前系统Code
        /// </summary>
        protected static string SystemCode
        {
            get
            {
                return Utility.GetConfigValue("SystemCode");
            }
        }

        #endregion

        /// <summary>
        /// 获取图片全路径
        /// </summary>
        /// <param name="strImgType">图片类型：
        /// DeliveryImage[]、
        /// Editor[编辑器上传图片]、
        /// SiteLogo[]、
        /// SiteLinks[]、
        /// SiteAd[广告上传图片]、
        /// SiteNavigation[]、
        /// NewsInfo[]、
        /// AdminAvatar[]、
        /// EBankLogo[支付平台LOGO]、
        /// UserGroup[]、
        /// UserLogo[用户图像]、
        /// IDCard[身份证上传图片]、
        /// ProductImage[产品图片]、
        /// SaleImage[]、
        /// PlatformImage[平台LOGO]、
        /// BrandImage[品牌LOGO]、
        /// OrderImage[订单截图]、
        /// CatalogImage[产品类别图片]、
        /// ProductEditorDesc[]、
        /// IndexImage[首页运营图片]、
        /// TariffImage[税单上传图片]、
        /// ActivityConfigImage[后台活动配置图片]、
        /// ShareImg[晒单图片]、
        /// </param>
        /// <param name="strFileName">图片名称：数据库存储字段值</param>
        /// <param name="strImgSize">图片尺寸：W350H350</param>
        /// <returns></returns>
        public static string GetImgFullPath(string strImgType, string strFileName, string strImgSize = "")
        {
            if (!string.IsNullOrWhiteSpace(strFileName))
            {
                int intPos = strFileName.LastIndexOf(".");
                if (intPos > -1 && !string.IsNullOrEmpty(strImgSize))
                {
                    string strPartOfFileName = strFileName.Substring(0, intPos);
                    string strFileExtension = strFileName.Substring(intPos, strFileName.Length - intPos);
                    strFileName = string.Format("{0}-{1}{2}", strPartOfFileName, strImgSize, strFileExtension);
                }
            }

            return string.Format("{0}/{1}/{2}", ImgDomain, strImgType, strFileName);
        }
    }
}
