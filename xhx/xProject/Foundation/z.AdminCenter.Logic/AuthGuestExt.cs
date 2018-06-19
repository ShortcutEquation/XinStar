using z.AdminCenter.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace z.AdminCenter.Logic
{
    [Serializable]
    public class AuthGuestExt : auth_guest
    {
        /// <summary>
        /// 批量删除时传值使用
        /// </summary>
        public List<int> AuthGuestIds
        {
            get;
            set;
        }
    }
}
