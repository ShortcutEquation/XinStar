using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml;
using z.Foundation.Thumbnail;

namespace z.Foundation
{
    public class UploadFile
    {
        private static XmlDocument xmlDocument;
        private static string NormalDictionary = "";
        private static string TemporaryDictionary = "";

        /// <summary>
        /// 文件上传
        /// </summary>
        /// <param name="appName">应用名称</param>
        /// <param name="param">参数（自定义文件存放路径）</param>
        /// <returns></returns>
        public static string Upload(string appName, object param)
        {
            string returnPath = "";

            //加载配置文件
            LoadConfig();

            //根据appName查找该app对应的文件上传配置
            XmlNode appNode = xmlDocument.SelectSingleNode(string.Format("FileConfiguration/App[@Name='{0}']", appName));
            if (appNode == null)
            {
                throw new Exception(string.Format("未能找到Name=\"{0}\"的App节点", appName));
            }

            //解析File节点
            XmlNode fileNode = appNode.SelectSingleNode("File");
            if (fileNode == null)
            {
                throw new Exception(string.Format("在Name=\"{0}\"的App节点中未能找到File节点", appName));
            }
            XmlNode extensionNode = fileNode.SelectSingleNode("Extensions");
            XmlNode sizeNode = fileNode.SelectSingleNode("Size");
            if (extensionNode == null || string.IsNullOrEmpty(extensionNode.InnerText.Trim()))
            {
                throw new Exception(string.Format("在Name=\"{0}\"的App节点下的File节点中未能找到Entension节点或Extension节点未包含任何值", appName));
            }
            if (sizeNode == null || string.IsNullOrEmpty(sizeNode.InnerText.Trim()))
            {
                throw new Exception(string.Format("在Name=\"{0}\"的App节点下的File节点中未能找到Size节点或Size节点未包含任何值", appName));
            }

            //判断上传文件相关限制
            int filesCount = HttpContext.Current.Request.Files.Count;
            if (HttpContext.Current.Request.ContentLength / 1024 > Convert.ToInt32(sizeNode.InnerText) * filesCount)
            {
                throw new Exception("上传文件超过规定大小");
            }
            List<string> allowFileExtensions = extensionNode.InnerText.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList<string>();
            for (int i = 0; i < filesCount; i++)
            {
                HttpPostedFile httpPostedFile = HttpContext.Current.Request.Files[i];
                List<string> strList = (from extension in allowFileExtensions
                                        where string.Format(".{0}", extension).ToLower() == Path.GetExtension(httpPostedFile.FileName).ToLower()
                                        select extension).ToList();
                if (strList.Count == 0)
                {
                    throw new Exception("上传文件列表中包含不允许上传文件类型");
                }
            }

            //判断是否需要生成缩略图（针对图片类型文件上传）
            bool fixedSize = false;
            int quality = 70;
            List<WHMark> whMarkList = new List<WHMark>();
            XmlNode thumbnailNode = fileNode.SelectSingleNode("Thumbnail");
            if (thumbnailNode != null)
            {
                XmlAttribute fixedSizeAttribute = thumbnailNode.Attributes["FixedSize"];
                XmlAttribute qualityAttribute = thumbnailNode.Attributes["Quality"];
                fixedSize = fixedSizeAttribute == null ? true : Convert.ToBoolean(fixedSizeAttribute.Value);
                quality = qualityAttribute == null ? 70 : Convert.ToInt32(qualityAttribute.Value);
                XmlNodeList whNodeList = thumbnailNode.SelectNodes("WH");
                foreach (XmlNode whNode in whNodeList)
                {
                    XmlAttribute whAttribute = whNode.Attributes["Mark"];
                    string mark = whAttribute == null ? "" : whAttribute.InnerText.Trim();
                    string whNodeValue = whNode.InnerText.Trim();
                    if (!string.IsNullOrEmpty(whNodeValue) && !string.IsNullOrEmpty(mark))
                    {
                        string[] wh = whNodeValue.Split("*".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                        if (wh.Length == 2)
                        {
                            int width = 0;
                            int height = 0;
                            bool tryWidth = int.TryParse(wh[0], out width);
                            bool tryHeight = int.TryParse(wh[1], out height);
                            if (tryWidth || tryHeight)
                            {
                                whMarkList.Add(new WHMark() { Width = width, Height = height, Mark = mark });
                            }
                        }
                    }
                }
            }

            //判断是否需要打水印（针对图片类型文件上传）
            WaterMark waterMark = WaterMark.None;
            WaterMarkContent waterMarkContent = new WaterMarkContent();
            int waterMarkArea = 180 * 180;//默认打水印最小图片面积
            XmlNode waterMarkNode = fileNode.SelectSingleNode("WaterMark");
            if (waterMarkNode != null)
            {
                XmlNode imgNode = waterMarkNode.SelectSingleNode("Img");
                string strImgNodeText = imgNode == null ? "" : imgNode.InnerText.Trim();
                if (!string.IsNullOrEmpty(strImgNodeText))
                {
                    string strWaterMarkImgPath = GetPhysicsPath(strImgNodeText);
                    if (File.Exists(strWaterMarkImgPath))
                    {
                        waterMarkContent.Img = Image.FromFile(strWaterMarkImgPath);
                    }
                }
                XmlNode alphaNode = waterMarkNode.SelectSingleNode("Alpha");
                waterMarkContent.ImgAlpha = alphaNode == null || string.IsNullOrEmpty(alphaNode.InnerText.Trim()) ? 0.1f : (float)Convert.ToDouble(alphaNode.InnerText);
                XmlNode textNode = waterMarkNode.SelectSingleNode("Text");
                waterMarkContent.Text = textNode == null ? "" : textNode.InnerText.Trim();
                XmlNode fontFimilyNode = waterMarkNode.SelectSingleNode("FontFimily");
                waterMarkContent.FontFimily = fontFimilyNode == null ? "宋体" : fontFimilyNode.InnerText.Trim();
                XmlNode fontSizeNode = waterMarkNode.SelectSingleNode("FontSize");
                waterMarkContent.FontSize = fontSizeNode == null || string.IsNullOrEmpty(fontSizeNode.InnerText.Trim()) ? 20f : (float)Convert.ToDouble(fontSizeNode.InnerText);
                XmlNode argbNode = waterMarkNode.SelectSingleNode("Argb");
                waterMarkContent.FontArgb = argbNode == null ? "80,125,188,237" : argbNode.InnerText.Trim();

                if (!string.IsNullOrEmpty(waterMarkContent.Text) && waterMarkContent.Img != null)
                {
                    waterMark = WaterMark.Both;
                }
                else if (waterMarkContent.Img != null)
                {
                    waterMark = WaterMark.Image;
                }
                else if (!string.IsNullOrEmpty(waterMarkContent.Text))
                {
                    waterMark = WaterMark.Text;
                }

                if (waterMark != WaterMark.None)
                {
                    XmlAttribute waterMarkSizeAttribute = waterMarkNode.Attributes["Size"];
                    string strWaterMarkSize = waterMarkSizeAttribute.Value;
                    if (!string.IsNullOrEmpty(strWaterMarkSize))
                    {
                        string[] arrWaterMarkSize = strWaterMarkSize.Split("*".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                        if (arrWaterMarkSize.Length == 2)
                        {
                            int width = 0;
                            int height = 0;
                            bool tryWidth = int.TryParse(arrWaterMarkSize[0], out width);
                            bool tryHeight = int.TryParse(arrWaterMarkSize[1], out height);
                            if (tryWidth || tryHeight)
                            {
                                waterMarkArea = width * height;
                            }
                        }
                    }
                }
            }

            //获取文件存放路径并创建此路径
            string dictionaryName = "";
            XmlAttribute dictionaryNameAttribute = appNode.Attributes["DictionaryName"];
            if (dictionaryNameAttribute != null)
            {
                dictionaryName = dictionaryNameAttribute.Value.Trim();
            }
            if (!string.IsNullOrEmpty(dictionaryName) && dictionaryName.ToLower() == "@Custom".ToLower())
            {
                dictionaryName = param.ToString();
            }
            else
            {
                dictionaryName = appName;
            }
            string savePath = string.Format("/{0}/{1}/", TemporaryDictionary, dictionaryName);
            string physicsPath = GetPhysicsPath(savePath);
            if (!Directory.Exists(physicsPath))
            {
                Directory.CreateDirectory(physicsPath);
            }

            //开始上传文件
            for (int i = 0; i < filesCount; i++)
            {
                //获取文件名称、文件扩展名称
                HttpPostedFile httpPostedFile = HttpContext.Current.Request.Files[i];
                string fileName = GetFileName();
                string extensionName = Path.GetExtension(httpPostedFile.FileName).ToLower();

                foreach (var whMark in whMarkList)
                {
                    //判断缩略图面积是否大于打水印指定最小缩略图面积
                    WaterMark _WaterMark = waterMark;

                    if (whMark.Width > 0 && whMark.Height > 0)
                    {
                        if (waterMark != WaterMark.None && whMark.Width * whMark.Height <= waterMarkArea)
                        {
                            _WaterMark = WaterMark.None;
                        }

                        ThumbnailGeneration.MakeThumbnail(httpPostedFile.InputStream, physicsPath + fileName + whMark.Mark + extensionName, whMark.Width, whMark.Height, quality, fixedSize, _WaterMark, waterMarkContent);
                    }
                    else
                    {
                        using (Image image = Image.FromStream(httpPostedFile.InputStream))
                        {
                            if (whMark.Width > 0)
                            {
                                whMark.Height = whMark.Width * image.Height / image.Width;
                            }
                            else
                            {
                                whMark.Width = image.Width * whMark.Height / image.Height;
                            }

                            if (waterMark != WaterMark.None && whMark.Width * whMark.Height <= waterMarkArea)
                            {
                                _WaterMark = WaterMark.None;
                            }

                            ThumbnailGeneration.MakeThumbnail(image, physicsPath + fileName + whMark.Mark + extensionName, whMark.Width, whMark.Height, quality, false, _WaterMark, waterMarkContent);
                        }
                    }
                }

                //保存原始文件
                string saveFileName = fileName + extensionName;
                httpPostedFile.SaveAs(physicsPath + saveFileName);
                returnPath += savePath + saveFileName + ";";
            }

            //图片生成完毕后，释放水印图片内存（针对图片类型文件上传）
            if (waterMarkContent.Img != null)
            {
                waterMarkContent.Img.Dispose();
            }

            return returnPath.TrimEnd(';');
        }

        /// <summary>
        /// 正式保存文件
        /// </summary>
        /// <param name="fullName">临时文件全路径，多个文件请使用“,”号隔开</param>
        /// <returns></returns>
        public static string Save(string appName, string fullName)
        {
            string strDBPath = "";
            //加载配置文件
            LoadConfig();

            if (!string.IsNullOrEmpty(fullName))
            {
                string[] tempFiles = fullName.Split(";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                foreach (var obj in tempFiles)
                {
                    if (!obj.IsMatch(string.Format(@"^/{0}/.+", TemporaryDictionary)))
                    {
                        throw new Exception("fullName参数必须为相对临时目录路径");
                    }
                }
                foreach (string obj in tempFiles)
                {
                    //获取临时文件物理路径
                    string strTempPhysicsPath = GetPhysicsPath(obj);

                    //获取持久文件物理路径
                    string strPhysicsPath = GetPhysicsPath(obj.Replace(TemporaryDictionary, NormalDictionary));

                    //获取存入数据库的路径
                    strDBPath += obj.Replace(TemporaryDictionary, NormalDictionary) + ";";

                    //创建目录
                    string strDirectory = strPhysicsPath.Substring(0, strPhysicsPath.LastIndexOf("\\"));
                    if (!Directory.Exists(strDirectory))
                        Directory.CreateDirectory(strDirectory);
                    //文件Copy
                    if (File.Exists(strTempPhysicsPath) && !File.Exists(strPhysicsPath))
                    {
                        File.Copy(strTempPhysicsPath, strPhysicsPath);
                    }

                    #region 缩略图文件Copy

                    //根据appName查找该app对应的文件上传配置
                    XmlNode appNode = xmlDocument.SelectSingleNode(string.Format("FileConfiguration/App[@Name='{0}']", appName));
                    if (appNode != null)
                    {
                        XmlNodeList whNodeList = appNode.SelectNodes("File/Thumbnail/WH");
                        foreach (XmlNode whNode in whNodeList)
                        {
                            XmlAttribute whAttribute = whNode.Attributes["Mark"];
                            if (whAttribute != null)
                            {
                                string mark = whAttribute.InnerText.Trim();
                                if (!string.IsNullOrEmpty(mark))
                                {
                                    string tempThumbnail = GetThumbnail(obj, mark);
                                    string normalThumbnail = tempThumbnail.Replace(TemporaryDictionary, NormalDictionary);
                                    string tempThumbnailPhysicsPath = GetPhysicsPath(tempThumbnail);
                                    string normalThumbnailPhysicsPath = GetPhysicsPath(normalThumbnail);
                                    if (File.Exists(tempThumbnailPhysicsPath) && !File.Exists(normalThumbnailPhysicsPath))
                                    {
                                        File.Copy(tempThumbnailPhysicsPath, normalThumbnailPhysicsPath);
                                    }
                                }
                            }
                        }
                    }
                    #endregion
                }
            }
            else
            {
                throw new Exception("传入的路径为空");
            }
            return strDBPath.TrimEnd(';');
        }

        /// <summary>
        /// 装载配置文件
        /// </summary>
        private static void LoadConfig()
        {
            if (xmlDocument == null)
            {
                FileInfo fileInfo = new FileInfo(string.Format("{0}/UploadFile.config", Utility.ApplicationPath()));
                //判断配置文件是否存在
                if (fileInfo.Exists)
                {
                    try
                    {
                        //将配置文件Load进XmlDocument，以便进行相关操作
                        FileStream fileStream = fileInfo.Open(FileMode.Open, FileAccess.Read, FileShare.Read);
                        XmlReader xmlReader = XmlReader.Create(fileStream);
                        xmlDocument = new XmlDocument();
                        xmlDocument.Load(xmlReader);
                    }
                    catch
                    {
                        throw;
                    }
                }
                else
                {
                    throw new Exception("UploadFile.config配置文件不存在");
                }
            }
            //获取NormalDictionary及TemporaryDictionary
            if (string.IsNullOrEmpty(NormalDictionary) || string.IsNullOrEmpty(TemporaryDictionary))
            {
                XmlNode xmlNode = xmlDocument.SelectSingleNode("FileConfiguration");
                if (xmlNode != null)
                {
                    XmlAttribute xmlAttribute = xmlNode.Attributes["NormalDictionary"];
                    if (xmlAttribute != null)
                    {
                        string xmlAttributeValue = xmlAttribute.Value.Trim();
                        if (!string.IsNullOrEmpty(xmlAttributeValue))
                        {
                            NormalDictionary = xmlAttributeValue;
                        }
                        else
                        {
                            throw new Exception("FileConfiguration节点的NormalDictionary属性值不允许为空");
                        }
                    }
                    else
                    {
                        throw new Exception("FileConfiguration节点中不存在NormalDictionary属性");
                    }
                    xmlAttribute = xmlNode.Attributes["TemporaryDictionary"];
                    if (xmlAttribute != null)
                    {
                        string xmlAttributeValue = xmlAttribute.Value.Trim();
                        if (!string.IsNullOrEmpty(xmlAttributeValue))
                        {
                            TemporaryDictionary = xmlAttributeValue;
                        }
                        else
                        {
                            throw new Exception("FileConfiguration节点的TemporaryDictionary属性值不允许为空");
                        }
                    }
                    else
                    {
                        throw new Exception("FileConfiguration节点中不存在TemporaryDictionary属性");
                    }
                }
                else
                {
                    throw new Exception("File.config配置文件中不存在FileConfiguration节点");
                }
            }
        }

        /// <summary>
        /// 获取随机文件名（Guid的Hash值）
        /// </summary>
        /// <param name="fileExtension"></param>
        /// <param name="mark"></param>
        /// <returns></returns>
        public static string GetFileName()
        {
            //return Guid.NewGuid().GetHashCode().ToString();
            Guid guid = Guid.NewGuid();
            byte[] buffer = guid.ToByteArray();
            return BitConverter.ToInt64(buffer, 0).ToString();
        }

        /// <summary>
        /// 获取文件物理路径
        /// </summary>
        /// <param name="strPath"></param>
        /// <returns></returns>
        private static string GetPhysicsPath(string path)
        {
            return HttpContext.Current.Server.MapPath(path);
        }

        /// <summary>
        /// 删除图片
        /// </summary>
        /// <param name="appName"></param>
        /// <param name="fullName"></param>
        /// <returns></returns>
        public static void Delete(string appName, string fullName)
        {
            //加载配置文件
            LoadConfig();

            if (!string.IsNullOrEmpty(fullName))
            {
                string[] files = fullName.Split(";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

                foreach (var obj in files)
                {
                    //删除主文件
                    string strPhysicsPath = GetPhysicsPath(obj);
                    if (File.Exists(strPhysicsPath))
                    {
                        File.Delete(strPhysicsPath);
                    }
                    //删除附属文件
                    XmlNode appNode = xmlDocument.SelectSingleNode(string.Format("FileConfiguration/App[@Name='{0}']", appName));
                    if (appNode != null)
                    {
                        XmlNodeList whNodeList = appNode.SelectNodes("File/Thumbnail/WH");
                        foreach (XmlNode whNode in whNodeList)
                        {
                            XmlAttribute whAttribute = whNode.Attributes["Mark"];
                            if (whAttribute != null)
                            {
                                string mark = whAttribute.InnerText.Trim();
                                if (!string.IsNullOrEmpty(mark))
                                {
                                    string Thumbnail = GetThumbnail(obj, mark);
                                    string ThumbnailPhysicsPath = GetPhysicsPath(Thumbnail);
                                    if (File.Exists(ThumbnailPhysicsPath))
                                    {
                                        File.Delete(ThumbnailPhysicsPath);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 获取缩略图
        /// </summary>
        /// <param name="fullName">原始文件名</param>
        /// <param name="mark">缩略图标识名称</param>
        /// <returns></returns>
        public static string GetThumbnail(string fullName, string mark)
        {
            string thumbnail = "";

            if (!string.IsNullOrEmpty(fullName) && !string.IsNullOrEmpty(mark))
            {
                string[] files = fullName.Split(";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

                foreach (var obj in files)
                {
                    int pos = obj.LastIndexOf(".");
                    thumbnail += string.Format("{0}{1}{2};", obj.Substring(0, pos), mark, obj.Substring(pos, obj.Length - pos));
                }
            }
            return thumbnail.TrimEnd(';');
        }

        /// <summary>
        /// 获取文件存入数据库的路径
        /// </summary>
        /// <param name="fullName">临时文件全路径</param>
        /// <returns></returns>
        public static string GetDbPath(string fullName)
        {
            //加载配置文件
            LoadConfig();

            //获取存入数据库的路径
            string strDBPath = fullName.Replace(TemporaryDictionary, NormalDictionary);

            return strDBPath;
        }

        private class WHMark
        {
            public int Width
            {
                get;
                set;
            }

            public int Height
            {
                get;
                set;
            }

            public string Mark
            {
                get;
                set;
            }
        }

        /// <summary>
        /// 图片域
        /// </summary>
        public static string ImgDomain
        {
            get
            {
                return string.Format("http://{0}", Utility.GetConfigValue("ImgDomain"));
            }
        }

        /// <summary>
        /// 获取图片全路径
        /// </summary>
        /// <param name="strImgType">图片类型：
        /// DeliveryImage[]、
        /// Editor[编辑器上传图片]、
        /// SiteLogo[]、
        /// SiteLinks[]、
        /// SiteAd[广告上传图片]、
        /// SiteNavigation[]、
        /// NewsInfo[]、
        /// AdminAvatar[]、
        /// EBankLogo[支付平台LOGO]、
        /// UserGroup[]、
        /// UserLogo[用户图像]、
        /// IDCard[身份证上传图片]、
        /// ProductImage[产品图片]、
        /// SaleImage[]、
        /// PlatformImage[平台LOGO]、
        /// BrandImage[品牌LOGO]、
        /// OrderImage[订单截图]、
        /// CatalogImage[产品类别图片]、
        /// ProductEditorDesc[]、
        /// IndexImage[首页运营图片]、
        /// TariffImage[税单上传图片]、
        /// ActivityConfigImage[后台活动配置图片]、
        /// ShareImg[晒单图片]、
        /// </param>
        /// <param name="strFileName">图片名称：数据库存储字段值</param>
        /// <param name="strImgSize">图片尺寸：W350H350</param>
        /// <returns></returns>
        public static string GetImgFullPath(string strImgType, string strFileName, string strImgSize = "")
        {
            if (!string.IsNullOrEmpty(strFileName))
            {
                int intPos = strFileName.LastIndexOf(".");
                if (intPos > -1 && !string.IsNullOrEmpty(strImgSize))
                {
                    string strPartOfFileName = strFileName.Substring(0, intPos);
                    string strFileExtension = strFileName.Substring(intPos, strFileName.Length - intPos);
                    strFileName = string.Format("{0}-{1}{2}", strPartOfFileName, strImgSize, strFileExtension);
                }
            }

            return string.Format("{0}/{1}/{2}", ImgDomain, strImgType, strFileName);
        }
    }
}
