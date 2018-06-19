using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Configuration;
using z.Foundation;

namespace z.AdminCenter.Logic
{
    public class WriteTicketHandle : IHttpHandler
    {
        public bool IsReusable
        {
            get { return false; }
        }

        public void ProcessRequest(HttpContext context)
        {
            string strTicket = context.Request.Params["code"];
            if (string.IsNullOrEmpty(strTicket))
            {
                return;
            }

            string strRemember = context.Request.Params["remember"];

            HttpCookie cookie = new HttpCookie("Ticket");
            cookie.Value = strTicket;
            if (strRemember == "on")
            {
                cookie.Expires = DateTime.Now.AddDays(3);
            }

            context.Response.AddHeader("p3p", "CP=CURa ADMa DEVa PSAo PSDo OUR BUS UNI PUR INT DEM STA PRE COM NAV OTC NOI DSP COR");
            context.Response.AppendCookie(cookie);
        }
    }
}
