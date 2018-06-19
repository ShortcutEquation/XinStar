using Gif.Components;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;

namespace z.Foundation.Thumbnail
{
    public class ThumbnailGeneration : ThumbnailGenerationBase
    {
        /// <summary>
        /// 生成缩略图，此方法省略两个参数：｛是否指定大小生成图片=按图片比例缩放，是否需要打上水印=不打水印｝
        /// </summary>
        /// <param name="inputstream">Image文件流</param>
        /// <param name="newFile">生成的文件名称；注：全路径</param>
        /// <param name="maxWidth">生成图片的宽</param>
        /// <param name="maxHeight">生成图片高</param>
        /// <param name="Quality">生成图片质量</param>
        public static void MakeThumbnail(Stream inputstream, string newFile, int maxWidth, int maxHeight, int Quality)
        {
            using (System.Drawing.Image image = System.Drawing.Image.FromStream(inputstream))
            {
                MakeThumbnail(image, newFile, maxWidth, maxHeight, Quality, false, WaterMark.None, null);
            }
        }

        /// <summary>
        /// 生成缩略图
        /// </summary>
        /// <param name="inputstream">Image文件流</param>
        /// <param name="newFile">生成的文件名称；注：全路径</param>
        /// <param name="maxWidth">生成图片的宽</param>
        /// <param name="maxHeight">生成图片高</param>
        /// <param name="Quality">生成图片质量</param>
        /// <param name="FixedSize">指定大小生成图片，不足处用透明底色填补</param>
        /// <param name="waterMark">水印</param>
        /// <param name="waterMarkContent">水印详细参数</param>
        public static void MakeThumbnail(Stream inputstream, string newFile, int maxWidth, int maxHeight, int Quality, bool FixedSize, WaterMark waterMark, WaterMarkContent waterMarkContent)
        {
            using (System.Drawing.Image image = System.Drawing.Image.FromStream(inputstream))
            {
                MakeThumbnail(image, newFile, maxWidth, maxHeight, Quality, FixedSize, waterMark, waterMarkContent);
            }
        }

        /// <summary>
        /// 生成缩略图，此方法省略两个参数：｛是否指定大小生成图片=按图片比例缩放，是否需要打上水印=不打水印｝
        /// </summary>
        /// <param name="inputstream">Image文件路径</param>
        /// <param name="newFile">生成的文件名称；注：全路径</param>
        /// <param name="maxWidth">生成图片的宽</param>
        /// <param name="maxHeight">生成图片高</param>
        /// <param name="Quality">生成图片质量</param>
        public static void MakeThumbnail(string strImgPath, string newFile, int maxWidth, int maxHeight, int Quality)
        {
            using (System.Drawing.Image image = System.Drawing.Image.FromFile(strImgPath))
            {
                MakeThumbnail(image, newFile, maxWidth, maxHeight, Quality, false, WaterMark.None, new WaterMarkContent());
            }
        }

        /// <summary>
        /// 生成缩略图
        /// </summary>
        /// <param name="inputstream">Image文件路径</param>
        /// <param name="newFile">生成的文件名称；注：全路径</param>
        /// <param name="maxWidth">生成图片的宽</param>
        /// <param name="maxHeight">生成图片高</param>
        /// <param name="Quality">生成图片质量</param>
        /// <param name="FixedSize">指定大小生成图片，不足处用透明底色填补</param>
        /// <param name="waterMark">水印</param>
        /// <param name="waterMarkContent">水印详细参数</param>
        public static void MakeThumbnail(string strImgPath, string newFile, int maxWidth, int maxHeight, int Quality, bool FixedSize, WaterMark waterMark, WaterMarkContent waterMarkContent)
        {
            using (System.Drawing.Image image = System.Drawing.Image.FromFile(strImgPath))
            {
                MakeThumbnail(image, newFile, maxWidth, maxHeight, Quality, FixedSize, waterMark, waterMarkContent);
            }
        }
    }
}
