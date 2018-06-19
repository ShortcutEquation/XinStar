using z.Foundation.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace z.Logic.Base
{
    public class NHLogicBase
    {
        public IRepository Repository
        {
            get { return new NHibernateRepository(); }
        }
    }
}
