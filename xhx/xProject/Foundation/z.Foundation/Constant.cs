using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace z.Foundation
{
    /// <summary>
    /// 常参定义
    /// </summary>
    public class Constant
    {
        /// <summary>
        /// 正整数-正则表达式
        /// </summary>
        public static readonly string PositiveIntegerRegex = @"^[1-9]{1}\d*$";

        /// <summary>
        /// 网站用户名正则表达式
        /// </summary>
        public static readonly string UserNameRegex = @"^[a-z][a-z0-9_]{2,12}[a-z0-9]$";

        /// <summary>
        /// Html标签正则表达式
        /// </summary>
        public static readonly string HtmlTagRegex = @"<[^>]+>";

        /// <summary>
        /// Img标签正则表达式
        /// </summary>
        public static readonly string ImageRegex = @"<img[^<]+>";

        /// <summary>
        /// Src属性正则表达式
        /// </summary>
        public static readonly string SrcRegex = @"src=""[^""]+""|src='[^']+'";

        /// <summary>
        /// Href属性正则表达式
        /// </summary>
        public static readonly string HrefRegex = @"href=""[^""]+""|href='[^']+'";

        /// <summary>
        /// IP地址正则表达式
        /// </summary>
        public static readonly string IPRegex = @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$";

        /// <summary>
        /// 手机号码正则表达式
        /// </summary>
        public static readonly string MobileRegex = @"^0?(13[0-9]|14[57]|15[012356789]|17[0-9]|18[0-9]|19[0-9]|16[0-9])[0-9]{8}$";
        
        /// <summary>
        /// 邮箱正则表达式
        /// </summary>
        public static readonly string EmailRegex = @"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$";

        /// <summary>
        /// 身份证正则表达式
        /// </summary>
        public static readonly string IDCardRegex = @"(^\d{15}$)|(^\d{18}$)|(^\d{17}(\d|X|x)$)";
    }
}
