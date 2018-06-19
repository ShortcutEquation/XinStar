using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace z.Foundation.Thumbnail
{
    public class WaterMarkContent
    {
        public string Text { get; set; }
        public string FontFimily { get; set; }
        public float FontSize { get; set; }
        /// <summary>
        /// 文字颜色及透明度 Color.FromArgb(a,r,g,b) 参数说明：a-透明度0-100,100表示不透明; r-红色; g-绿色; b-蓝色 
        /// </summary>
        public string FontArgb { get; set; }
        /// <summary>
        /// 水印图片
        /// </summary>
        public Image Img { get; set; }
        /// <summary>
        /// 水印图片透明度，值越大越不透明: 1为不透明、0为全透明, 此值类型为float
        /// </summary>
        public float ImgAlpha { get; set; }
    }
}
