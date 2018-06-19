using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace z.Foundation.Data
{
    /// <summary>
    /// 数据层自定义属性类
    /// </summary>
    public class CustomDataAttribute : Attribute
    {
        /// <summary>
        /// 数据库连接字符串名称（EF）、数据库连接文件名称（NHibernate）
        /// </summary>
        public string ConnectionName
        {
            get;
            set;
        }
    }
}
