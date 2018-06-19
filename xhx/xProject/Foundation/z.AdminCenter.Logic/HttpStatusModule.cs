using z.Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace z.AdminCenter.Logic
{
    public class HttpStatusModule : IHttpModule
    {
        public void Dispose()
        {
            //throw new NotImplementedException();
        }

        public void Init(HttpApplication context)
        {
            context.EndRequest += context_EndRequest;
        }

        void context_EndRequest(object sender, EventArgs e)
        {
            HttpApplication application = (HttpApplication)sender;
            HttpContext context = application.Context;
            HttpRequest request = application.Request;
            HttpResponse response = application.Response;
            int intStatusCode = response.StatusCode;
            if (intStatusCode == 404)
            {
                context.Response.Redirect(string.Format("http://{0}/404.html", Utility.GetConfigValue("AdminCenterDomain")));
            }
            else if (intStatusCode == 403)
            {
                context.Response.Redirect(string.Format("http://{0}/403.html", Utility.GetConfigValue("AdminCenterDomain")));
            }
            else if (intStatusCode == 500)
            {
                context.Response.Redirect(string.Format("http://{0}/500.html", Utility.GetConfigValue("AdminCenterDomain")));
            }
        }
    }
}
