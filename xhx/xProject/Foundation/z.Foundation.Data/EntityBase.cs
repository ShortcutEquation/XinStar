using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace z.Foundation.Data
{
    /// <summary>
    /// 实体类基类
    /// </summary>
    [Serializable]
    public class EntityBase : IEntity
    {
        /// <summary>
        /// 用户会话对象
        /// </summary>
        public virtual object UserSession
        {
            get;
            set;
        }
    }
}
