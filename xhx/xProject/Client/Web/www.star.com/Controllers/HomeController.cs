using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using z.Foundation;
using log4net;
using com.star.Logic.Passport;
using com.star.Entity.StarDB;
using z.Foundation.LogicInvoke;
using z.Foundation.Data;

namespace www.star.com.Controllers
{
    public class HomeController : BaseController
    {
        // GET: Home
        public ActionResult Index()
        {
            //var s = new PassportManage().Passport("xiehx");
            var model = new test();
            var name = "xiehx";
            //调用服务 test
            IResponse<test> response = CallLogic<string, test>("com.star.Logic.Passport.dll", "com.star.Logic.Passport.PassportManage", "Passport", name);
            if (response.Succeeded)
            {
                model = response.Result;
                Console.WriteLine(model.Name + ":" + model.Remark);
                Console.ReadLine();
            }
            else
            {
                // 日志 log4net test
                Logger.Info("这代码有毛病");
            }

            return View(model);
        }
    }
}