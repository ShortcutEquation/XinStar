using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace z.Foundation
{
    public class FileOperate
    {
        /// <summary>
        /// 获取年月日文件路径 /2008/10/16/
        /// </summary>
        /// <returns></returns>
        public static string GetDateFolder()
        {
            return string.Format("/{0}/{1}/{2}/", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
        }

        /// <summary>
        /// 获取年月日时分秒文件名  20081117223521
        /// </summary>
        /// <returns></returns>
        public static string GetDateFileName()
        {
            return DateTime.Now.ToString("yyyyMMddHHmmss");
        }

        /// <summary>
        /// 读取文件内容
        /// </summary>
        /// <param name="strFilePath">文件路径</param>
        /// <returns>文件内容字符串</returns>
        public static string Read(string strFilePath)
        {
            strFilePath = strFilePath.ToLower();
            if (!File.Exists(strFilePath))
                return "";
            string strFileContent = string.Empty;
            StreamReader sr = new StreamReader(strFilePath, System.Text.UnicodeEncoding.GetEncoding("utf-8"));
            try
            {
                strFileContent = sr.ReadToEnd();
            }
            catch (Exception e)
            {
                throw (new Exception(e.Message));
            }
            finally
            {
                sr.Close();
            }
            return strFileContent;
        }

        /// <summary>
        /// 读取文件内容 可指定文件编码
        /// </summary>
        /// <param name="strFilePath">文件路径</param>
        /// /// <param name="strEncodingName">编码</param>
        /// <returns>文件内容字符串</returns>
        public static string Read(string strFilePath, string strEncodingName)
        {
            strFilePath = strFilePath.ToLower();
            if (!File.Exists(strFilePath))
                return "";

            string strFileContent = string.Empty;
            StreamReader sr = new StreamReader(strFilePath, System.Text.UnicodeEncoding.GetEncoding(strEncodingName));
            try
            {
                strFileContent = sr.ReadToEnd();
            }
            catch (Exception e)
            {
                throw (new Exception(e.Message));
            }
            finally
            {
                sr.Close();
            }
            return strFileContent;
        }

        /// <summary>
        /// 写文件
        /// </summary>
        /// <param name="strFileContent">写入的字符串</param>
        /// <param name="strDestinationFilePath">目的文件路径</param>
        public static void Write(string strFileContent, string strDestinationFilePath)
        {
            strDestinationFilePath = strDestinationFilePath.Replace("/", "\\\\").ToLower();
            string strDirectory = strDestinationFilePath.Substring(0, strDestinationFilePath.LastIndexOf("\\"));
            StreamWriter sw = null;
            try
            {
                if (!System.IO.Directory.Exists(strDirectory))
                {
                    System.IO.Directory.CreateDirectory(strDirectory);
                }
                sw = new StreamWriter(strDestinationFilePath, false, System.Text.UnicodeEncoding.GetEncoding("utf-8"));
                sw.Write(strFileContent);
                sw.Flush();
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                if (sw != null)
                {
                    sw.Close();
                }
            }
        }

        /// <summary>
        /// 写文件，可指定追加还是覆盖
        /// </summary>
        /// <param name="strFileContent"></param>
        /// <param name="strDestinationFilePath"></param>
        /// <param name="append"></param>
        public static void Write(string strFileContent, string strDestinationFilePath, bool append, string strEncodingName)
        {
            strDestinationFilePath = strDestinationFilePath.Replace("/", "\\\\").ToLower();
            string strDirectory = strDestinationFilePath.Substring(0, strDestinationFilePath.LastIndexOf("\\"));
            StreamWriter sw = null;
            try
            {
                if (!System.IO.Directory.Exists(strDirectory))
                {
                    System.IO.Directory.CreateDirectory(strDirectory);
                }
                sw = new StreamWriter(strDestinationFilePath, append, System.Text.UnicodeEncoding.GetEncoding(strEncodingName));
                sw.Write(strFileContent);
                sw.Flush();
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                if (sw != null)
                {
                    sw.Close();
                }
            }
        }

        /// <summary>
        /// 从磁盘删除指定路径的文件
        /// </summary>
        /// <param name="strFilePath">文件绝对路径</param>
        public static void Delete(string strFilePath)
        {
            strFilePath = strFilePath.ToLower();

            try
            {
                if (System.IO.File.Exists(strFilePath))
                    System.IO.File.Delete(strFilePath);
            }
            catch (Exception e)
            {
                throw (new Exception(e.Message));
            }
        }

        /// <summary>
        /// 删除目录
        /// </summary>
        /// <param name="strDirectory"></param>
        public static void DeleteDirectory(string strDirectory)
        {
            try
            {
                if (System.IO.Directory.Exists(strDirectory))
                    Directory.Delete(strDirectory, true);
            }
            catch { }
        }
    }
}
