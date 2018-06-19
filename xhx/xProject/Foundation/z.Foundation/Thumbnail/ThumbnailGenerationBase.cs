using Gif.Components;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;

namespace z.Foundation.Thumbnail
{
    public class ThumbnailGenerationBase
    {
        /// <summary>
        /// 生成高清晰缩略图基方法，图片按比例大小缩放，可以生成固定大小的图片，不足处以透明背景填充
        /// </summary>
        /// <param name="image"></param>
        /// <param name="newFile"></param>
        /// <param name="maxWidth"></param>
        /// <param name="maxHeight"></param>
        /// <param name="Quality"></param>
        /// <param name="FixedSize">当此值为true时，生成固定大小图片时，不足处以透明背景填充</param>
        /// <param name="waterMark"></param>
        public static void MakeThumbnail(System.Drawing.Image image, string newFile, int maxWidth, int maxHeight, int Quality, bool FixedSize, WaterMark waterMark, WaterMarkContent waterMarkContent)
        {
            if (image.RawFormat.Guid == ImageFormat.Gif.Guid)
            {
                FrameDimension frameDimension = new FrameDimension(image.FrameDimensionsList[0]);
                int count = image.GetFrameCount(frameDimension);
                AnimatedGifEncoder animatedGifEncoder = new AnimatedGifEncoder();
                animatedGifEncoder.Start(newFile);
                animatedGifEncoder.SetRepeat(0);

                for (int i = 0; i < count; i++)
                {
                    image.SelectActiveFrame(frameDimension, i);//激活当前帧
                    int delay = 0;
                    for (int j = 0; j < image.PropertyIdList.Length; j++)
                    {
                        if ((int)image.PropertyIdList.GetValue(j) == 0x5100)
                        {
                            PropertyItem propertyItem = (PropertyItem)image.PropertyItems.GetValue(j);
                            byte[] delayByte = new byte[4];
                            delayByte[0] = propertyItem.Value[i * 4];
                            delayByte[1] = propertyItem.Value[1 + i * 4];
                            delayByte[2] = propertyItem.Value[2 + i * 4];
                            delayByte[3] = propertyItem.Value[3 + i * 4];
                            delay = BitConverter.ToInt32(delayByte, 0) * 10;
                            break;
                        }
                    }
                    using (Bitmap bitmap = ThumbnailGeneration.GetThumbnail(image, maxWidth, maxHeight, FixedSize, waterMark, waterMarkContent))
                    {
                        animatedGifEncoder.SetDelay(delay);
                        animatedGifEncoder.AddFrame(bitmap);
                    }
                }

                animatedGifEncoder.Finish();
            }
            else
            {
                using (Bitmap outBmp = GetThumbnail(image, maxWidth, maxHeight, FixedSize, waterMark, waterMarkContent))
                {
                    //设置图片压缩质量
                    EncoderParameters _EncoderParameters = new EncoderParameters(1);
                    EncoderParameter _EncoderParameter = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, Quality < 30 ? 70 : Quality);
                    _EncoderParameters.Param[0] = _EncoderParameter;

                    //设定图片保存格式（固定大小时必须保存为png图片，否则无法保证背景透明）
                    ImageCodecInfo _ImageCodecInfo = null;
                    if (FixedSize)
                        _ImageCodecInfo = GetImageCodecInfo("PNG");
                    else
                        _ImageCodecInfo = GetImageCodecInfo(image.RawFormat.Guid);

                    //保存图片
                    if (_ImageCodecInfo == null)
                        outBmp.Save(newFile, ImageFormat.Png);
                    else
                        outBmp.Save(newFile, _ImageCodecInfo, _EncoderParameters);
                }
            }
        }

        /// <summary>
        /// 获取缩略图
        /// </summary>
        /// <param name="image"></param>
        /// <param name="newFile"></param>
        /// <param name="maxWidth"></param>
        /// <param name="maxHeight"></param>
        /// <param name="Quality"></param>
        /// <param name="FixedSize"></param>
        /// <param name="waterMark"></param>
        /// <param name="waterMarkContent"></param>
        /// <returns></returns>
        private static Bitmap GetThumbnail(System.Drawing.Image image, int maxWidth, int maxHeight, bool FixedSize, WaterMark waterMark, WaterMarkContent waterMarkContent)
        {
            int OldWidth = image.Width;//图片实际宽度
            int OldHeight = image.Height;//图片实际高度
            int NewWidth = maxWidth;
            int NewHeight = maxHeight;
            int x = 0;
            int y = 0;

            string mode = string.Empty;
            if (OldWidth <= NewWidth && OldHeight <= NewHeight)
            {
                if (FixedSize)
                {
                    x = (maxWidth - OldWidth) / 2;
                    y = (maxHeight - OldHeight) / 2;
                }
                NewWidth = OldWidth;
                NewHeight = OldHeight;
            }
            else if (OldWidth > NewWidth && OldHeight < NewHeight)
                mode = "W";
            else if (OldWidth < NewWidth && OldHeight > NewHeight)
                mode = "H";
            else if (OldWidth >= NewWidth && OldHeight >= NewHeight)
                if (OldWidth * NewHeight > NewWidth * OldHeight)
                    mode = "W";
                else
                    mode = "H";

            switch (mode)
            {
                case "W"://指定宽，高按比例                    
                    NewHeight = OldHeight * NewWidth / OldWidth;
                    if (FixedSize)
                        y = (maxHeight - NewHeight) / 2;
                    break;
                case "H"://指定高，宽按比例
                    NewWidth = OldWidth * NewHeight / OldHeight;
                    if (FixedSize)
                        x = (maxWidth - NewWidth) / 2;
                    break;
            }
            int bmpWidth = 0;
            int bmpHeight = 0;
            if (FixedSize)
            {
                bmpWidth = maxWidth;
                bmpHeight = maxHeight;
            }
            else
            {
                bmpWidth = NewWidth;
                bmpHeight = NewHeight;
            }

            Bitmap outBmp = new Bitmap(bmpWidth, bmpHeight);
            using (Graphics g = Graphics.FromImage(outBmp))
            {
                // 设置画布描绘质量及背景
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;//指定插值质量（注：当启用高质量插值后会在图片周围产生白线）
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;//指定高像素偏移质量
                g.CompositingQuality = CompositingQuality.HighQuality;//指定影像合成
                g.SmoothingMode = SmoothingMode.HighQuality;//指定平滑度
                if (image.RawFormat.Guid == ImageFormat.Gif.Guid)
                {
                    g.Clear(Color.White);
                }
                else
                {
                    g.Clear(System.Drawing.Color.Transparent);//清除背景以透明色填充
                }

                //在指定位置并且按指定大小绘制指定的Image的指定部分。
                using (ImageAttributes wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    g.DrawImage(image, new System.Drawing.Rectangle(x, y, NewWidth, NewHeight), 0, 0, OldWidth, OldHeight, GraphicsUnit.Pixel, wrapMode);
                }

                #region 添加水印
                //添加文字水印(可设文字透明度)
                if (waterMark == WaterMark.Both || waterMark == WaterMark.Text)
                {
                    System.Drawing.Font _Font = new System.Drawing.Font(waterMarkContent.FontFimily, waterMarkContent.FontSize);
                    //Color.FromArgb(a,r,g,b) 参数说明：a-透明度0-100,100表示不透明; r-红色; g-绿色; b-蓝色
                    string[] ArrARGB = waterMarkContent.FontArgb.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    System.Drawing.Brush _Brush = new System.Drawing.SolidBrush(System.Drawing.Color.FromArgb(int.Parse(ArrARGB[0]), int.Parse(ArrARGB[1]), int.Parse(ArrARGB[2]), int.Parse(ArrARGB[3])));

                    string strWaterMarkText = waterMarkContent.Text;

                    //根据字体及字体大小获取字符串所占宽度及高度
                    SizeF _SizeF = g.MeasureString(strWaterMarkText, _Font);

                    //设置水印位置
                    if (FixedSize)
                    {
                        x = (maxWidth - NewWidth) / 2;
                        y = (maxHeight + NewHeight) / 2 - Convert.ToInt32(_SizeF.Height);
                    }
                    else
                    {
                        y = NewHeight - Convert.ToInt32(_SizeF.Height);
                    }

                    g.DrawString(strWaterMarkText, _Font, _Brush, x, y);
                }

                //添加图片水印（可设图片透明度）
                if (waterMark == WaterMark.Both || waterMark == WaterMark.Image)
                {
                    System.Drawing.Image WaterMarkImage = waterMarkContent.Img;
                    System.Drawing.Imaging.ImageAttributes _ImageAttributes = new System.Drawing.Imaging.ImageAttributes();

                    ColorMap colorMap = new ColorMap();
                    colorMap.OldColor = Color.FromArgb(255, 0, 255, 0);
                    colorMap.NewColor = Color.FromArgb(0, 0, 0, 0);
                    ColorMap[] remapTable = { colorMap };
                    _ImageAttributes.SetRemapTable(remapTable, ColorAdjustType.Bitmap);

                    //第四行第四个值用来设置透明度，值越大越不透明: 1为不透明、0为全透明
                    //正对角线的值分别代表红、绿、蓝、透明度、第五个值也没做太深入研究，想了解ColorMatrix即5x5矩阵可以在网上搜一下，这个矩阵的各个值的设置听说还申请了专利。
                    float[][] colorMatrixElements = {  
                new float[] {1.0f, 0.0f, 0.0f, 0.0f, 0.0f},  
                new float[] {0.0f, 1.0f, 0.0f, 0.0f, 0.0f},  
                new float[] {0.0f, 0.0f, 1.0f, 0.0f, 0.0f},  
                new float[] {0.0f, 0.0f, 0.0f, waterMarkContent.ImgAlpha, 0.0f},
                new float[] {0.0f, 0.0f, 0.0f, 0.0f, 1.0f}  
                };
                    System.Drawing.Imaging.ColorMatrix colorMatrix = new System.Drawing.Imaging.ColorMatrix(colorMatrixElements);
                    _ImageAttributes.SetColorMatrix(colorMatrix, System.Drawing.Imaging.ColorMatrixFlag.Default, System.Drawing.Imaging.ColorAdjustType.Bitmap);

                    //设置水印位置
                    if (FixedSize)
                    {
                        x = (maxWidth + NewWidth - 2 * WaterMarkImage.Width) / 2;
                        y = (maxHeight + NewHeight - 2 * WaterMarkImage.Height) / 2;
                    }
                    else
                    {
                        x = NewWidth - WaterMarkImage.Width;
                        y = NewHeight - WaterMarkImage.Height;
                    }
                    x -= 5;
                    y -= 10;

                    g.DrawImage(WaterMarkImage, new System.Drawing.Rectangle(x, y, WaterMarkImage.Width, WaterMarkImage.Height), 0, 0, WaterMarkImage.Width, WaterMarkImage.Height, System.Drawing.GraphicsUnit.Pixel, _ImageAttributes);

                    _ImageAttributes.Dispose();
                }
                #endregion

                #region 知识补充
                //g.DrawArc 绘制一段弧线，它表示由一对坐标、宽度和高度指定的椭圆部分。 
                //g.DrawBezier 绘制由4个Point结构定义的贝塞尔样条。
                //g.DrawBeziers 用Point结构数组绘制一系列贝塞尔样条。
                //g.DrawClosedCurve 绘制由Point结构的数组定义的闭合基数样条。
                //g.DrawCurve 绘制经过一组指定的Point结构的基数样条。 
                //g.DrawEllipse 绘制一个由边框（该边框由一对坐标、高度和宽度指定）定义的椭圆。
                //g.DrawIcon 在指定坐标处绘制由指定的 Icon 表示的图像。 
                //g.DrawIconUnstretched 绘制指定的 Icon 表示的图像，而不缩放该图像。 
                //g.DrawImage 在指定位置并且按原始大小绘制指定的 Image。 
                //g.DrawImageUnscaled 在由坐标对指定的位置，使用图像的原始物理大小绘制指定的图像。 
                //g.DrawImageUnscaledAndClipped 在不进行缩放的情况下绘制指定的图像，并在需要时剪辑该图像以适合指定的矩形。
                //g.DrawLine 绘制一条连接由坐标对指定的两个点的线条。 
                //g.DrawLines 绘制一系列连接一组 Point 结构的线段。 
                //g.DrawPath 
                //g.DrawPie 绘制一个扇形，该形状由一个坐标对、宽度、高度以及两条射线所指定的椭圆定义。
                //g.DrawPolygon 绘制由一组 Point 结构定义的多边形。 
                //g.DrawRectangle 绘制由坐标对、宽度和高度指定的矩形。
                //g.DrawRectangles 绘制一系列由 Rectangle 结构指定的矩形。 
                //g.DrawString 在指定位置并且用指定的 Brush 和 Font 对象绘制指定的文本字符串.
                #endregion
            }
            return outBmp;
        }

        /// <summary>
        /// 根据FormatDescription获取ImageCodeInfo
        /// </summary>
        /// <param name="strFormatDescription"></param>
        /// <returns></returns>
        private static ImageCodecInfo GetImageCodecInfo(string strFormatDescription)
        {
            ImageCodecInfo _ImageCodecInfo = null;
            ImageCodecInfo[] ArrImageCodecInfo = ImageCodecInfo.GetImageEncoders();
            foreach (var obj in ArrImageCodecInfo)
            {
                if (obj.FormatDescription == strFormatDescription.ToUpper())
                {
                    _ImageCodecInfo = obj;
                    break;
                }
            }
            return _ImageCodecInfo;
        }

        /// <summary>
        /// 根据Guid获取ImageCodeInfo
        /// </summary>
        /// <param name="_Guid"></param>
        /// <returns></returns>
        private static ImageCodecInfo GetImageCodecInfo(Guid _Guid)
        {
            ImageCodecInfo _ImageCodecInfo = null;
            ImageCodecInfo[] ArrImageCodecInfo = ImageCodecInfo.GetImageEncoders();
            foreach (var obj in ArrImageCodecInfo)
            {
                if (obj.FormatID == _Guid)
                {
                    _ImageCodecInfo = obj;
                    break;
                }
            }
            return _ImageCodecInfo;
        }
    }
}
