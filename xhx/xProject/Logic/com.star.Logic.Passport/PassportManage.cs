using com.star.Entity.StarDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using z.Foundation;
using z.Logic.Base;
using z.Foundation.Data;

namespace com.star.Logic.Passport
{
    public class PassportManage : NHLogicBase
    {
        public test Passport(string param)
        {
            var result = new test();

            try
            {
                //var t = new test()
                //{
                //    Name = "star",
                //    Remark = "test",
                //    CreateOn = DateTime.Now
                //};
                //var tid = Repository.Save(t);

                //NHibernate test
                var testList = Repository.Find<test>(e => e.Name == param).ToList();
                var model = testList.FirstOrDefault();
                if (model != null)
                {
                    result = model;
                }
            }
            catch (Exception ex)
            {
                Logger.Info(ex);
            }

            return result;
        }
    }
}
