using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace z.Foundation
{
    /// <summary>
    /// 字符串操作扩展方法
    /// </summary>
    public static class Strings
    {
        /// <summary>
        /// 是否匹配
        /// </summary>
        /// <param name="source">输入内容</param>
        /// <param name="regex">正则表达式</param>
        /// <returns></returns>
        public static bool IsMatch(this string source, string regex)
        {
            if (string.IsNullOrEmpty(source))
            {
                return false;
            }
            Regex re = new Regex(regex, RegexOptions.IgnoreCase);
            Match mc = re.Match(source);
            return mc.Success;
        }

        /// <summary>
        /// 单个匹配内容
        /// </summary>
        /// <param name="source">输入内容</param>
        /// <param name="regex">正则表达式</param>
        /// <returns></returns>
        public static string GetText(this string source, string regex)
        {
            if (string.IsNullOrEmpty(source))
            {
                return "";
            }
            Regex re = new Regex(regex, RegexOptions.IgnoreCase);
            Match mc = re.Match(source);
            if (mc.Success)
            {
                return mc.Value;
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// 多个匹配内容
        /// </summary>
        /// <param name="source">输入内容</param>
        /// <param name="regex">正则表达式</param>
        /// <returns></returns>
        public static List<string> GetList(this string source, string regex)
        {
            List<string> list = new List<string>();

            if (string.IsNullOrEmpty(source))
            {
                return list;
            }
            Regex re = new Regex(regex, RegexOptions.IgnoreCase);
            MatchCollection mcs = re.Matches(source);
            foreach (Match mc in mcs)
            {
                list.Add(mc.Value);
            }
            return list;
        }

        /// <summary>
        /// 替换指定内容
        /// </summary>
        /// <param name="source">输入内容</param>
        /// <param name="regex">正则表达式</param>
        /// <param name="replace">替换内容</param>
        /// <returns></returns>
        public static string RegexReplace(this string source, string regex, string replace)
        {
            if (string.IsNullOrEmpty(source))
            {
                return "";
            }
            Regex re = new Regex(regex, RegexOptions.IgnoreCase);
            MatchCollection mcs = re.Matches(source);
            foreach (Match mc in mcs)
            {
                if (!string.IsNullOrEmpty(mc.Value))
                    source = source.Replace(mc.Value, replace);
            }
            return source;
        }

        /// <summary>
        /// 截取字符串
        /// </summary>
        /// <param name="source"></param>
        /// <param name="maxLength"></param>
        /// <returns></returns>
        public static string SubString(this string source, int length, string endString)
        {
            if (string.IsNullOrEmpty(source))
            {
                return "";
            }

            string temp = source;
            int j = 0, k = 0;

            CharEnumerator charEnumerator = source.GetEnumerator();
            while (charEnumerator.MoveNext())
            {
                j += (charEnumerator.Current > 0 && charEnumerator.Current < 255) ? 1 : 2;

                if (j <= length)
                {
                    k++;
                }
                else
                {
                    temp = source.Substring(0, k);
                    break;
                }
            }

            return temp + endString;
        }

        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string MD5Encrypt(this string source)
        {
            if (!string.IsNullOrEmpty(source))
            {
                MD5 md5Hasher = MD5.Create();
                byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(source));
                StringBuilder sBuilder = new StringBuilder();
                for (int i = 0; i < data.Length; i++)
                {
                    sBuilder.Append(data[i].ToString("x2"));
                }
                return sBuilder.ToString();
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string MD5Encrypt(this string source, string inputCharset)
        {
            if (!string.IsNullOrEmpty(source))
            {
                MD5 md5Hasher = MD5.Create();
                byte[] data = md5Hasher.ComputeHash(Encoding.GetEncoding(inputCharset).GetBytes(source));
                StringBuilder sBuilder = new StringBuilder();
                for (int i = 0; i < data.Length; i++)
                {
                    sBuilder.Append(data[i].ToString("x2"));
                }
                return sBuilder.ToString();
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// AES加密(高级加密标准，是下一代的加密算法标准，速度快，安全级别高，目前AES标准的一个实现是Rijndael算法)
        /// </summary>
        /// <param name="source">待加密密文</param>
        /// <param name="encryptionKey">密钥</param>
        /// <returns></returns>
        public static string AESEncrypt(this string source, string encryptionKey)
        {
            if (string.IsNullOrEmpty(source)) { throw (new Exception("明文不得为空")); }
            if (string.IsNullOrEmpty(encryptionKey)) { throw (new Exception("密钥不得为空")); }

            string m_strEncrypt = "";
            byte[] m_btIV = Convert.FromBase64String("Rkb4jvUy/ye7Cd7k89QQgQ==");
            Rijndael m_AESProvider = Rijndael.Create();

            try
            {
                byte[] m_btEncryptString = Encoding.Default.GetBytes(source);
                MemoryStream m_stream = new MemoryStream();
                CryptoStream m_csstream = new CryptoStream(m_stream, m_AESProvider.CreateEncryptor(Encoding.Default.GetBytes(encryptionKey), m_btIV), CryptoStreamMode.Write);

                m_csstream.Write(m_btEncryptString, 0, m_btEncryptString.Length);
                m_csstream.FlushFinalBlock();
                m_strEncrypt = Convert.ToBase64String(m_stream.ToArray());
                m_stream.Close();
            }
            catch { throw; }
            finally { m_AESProvider.Clear(); }

            return m_strEncrypt;
        }

        /// <summary>
        /// AES解密
        /// </summary>
        /// <param name="source">密文</param>
        /// <param name="encryptionKey">密钥</param>
        /// <returns></returns>
        public static string AESDecrypt(this string source, string encryptionKey)
        {
            if (string.IsNullOrEmpty(source)) { throw (new Exception("密文不得为空")); }
            if (string.IsNullOrEmpty(encryptionKey)) { throw (new Exception("密钥不得为空")); }

            string m_strDecrypt = "";
            byte[] m_btIV = Convert.FromBase64String("Rkb4jvUy/ye7Cd7k89QQgQ==");
            Rijndael m_AESProvider = Rijndael.Create();

            try
            {
                byte[] m_btDecryptString = Convert.FromBase64String(source);
                MemoryStream m_stream = new MemoryStream();
                CryptoStream m_csstream = new CryptoStream(m_stream, m_AESProvider.CreateDecryptor(Encoding.Default.GetBytes(encryptionKey), m_btIV), CryptoStreamMode.Write);

                m_csstream.Write(m_btDecryptString, 0, m_btDecryptString.Length);
                m_csstream.FlushFinalBlock();
                m_strDecrypt = Encoding.Default.GetString(m_stream.ToArray());
                m_stream.Close();
            }
            catch { throw; }
            finally { m_AESProvider.Clear(); }

            return m_strDecrypt;
        }

        /// <summary>
        /// Base64字符串反序列化为对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static T Deserialize<T>(this string source)
        {
            //T obj = default(T);
            //try
            //{
            //    IFormatter formatter = new BinaryFormatter();
            //    byte[] bt = System.Convert.FromBase64String(source);
            //    MemoryStream ms = new MemoryStream(bt);
            //    obj = (T)formatter.Deserialize(ms);
            //    ms.Close();
            //}
            //catch
            //{
            //    throw;
            //}
            //return obj;


            T obj = default(T);
            IFormatter formatter = new BinaryFormatter();
            using (MemoryStream memoryStream = new MemoryStream())
            {
                byte[] bytes = Convert.FromBase64String(source);

                memoryStream.Write(bytes, 0, bytes.Length);
                memoryStream.Seek(0, SeekOrigin.Begin);
                obj = (T)formatter.Deserialize(memoryStream);
            }

            return obj;
        }

        /// <summary>
        /// XML反序列化为对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static T XmlDeserialize<T>(this string source, Encoding encoding)
        {
            T obj = default(T);
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
            Encoding _Encoding = Encoding.UTF8;
            if (encoding != null)
            {
                _Encoding = encoding;
            }
            using (Stream xmlStream = new MemoryStream(_Encoding.GetBytes(source)))
            {
                using (XmlReader xmlReader = XmlReader.Create(xmlStream))
                {
                    obj = (T)xmlSerializer.Deserialize(xmlReader);
                }
            }
            return obj;
        }

        /// <summary>
        /// Json字符串反序列化对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static T JsonDeserialize<T>(this string source)
        {
            return JsonConvert.DeserializeObject<T>(source);
        }

        /// <summary>
        /// 是否为正确的身份证号码-支持15位以及18位
        /// </summary>
        /// <param name="idCardNum"></param>
        /// <returns></returns>
        public static bool IsIDCard(this string idCardNum)
        {
            if (string.IsNullOrEmpty(idCardNum))
            {
                return false;
            }

            long n = 0;
            DateTime time;
            const string address = "11x22x35x44x53x12x23x36x45x54x13x31x37x46x61x14x32x41x50x62x15x33x42x51x63x21x34x43x52x64x65x71x81x82x91";
            string strBirthday;

            //省份验证
            if (address.IndexOf(idCardNum.Remove(2)) == -1)
            {
                return false;
            }

            switch (idCardNum.Length)
            {
                case 18:

                    #region 18位身份证

                    //数字验证
                    if (long.TryParse(idCardNum.Remove(17), out n) == false || n < Math.Pow(10, 16) || long.TryParse(idCardNum.Replace('x', '0').Replace('X', '0'), out n) == false)
                    {
                        return false;
                    }

                    //生日验证
                    strBirthday = idCardNum.Substring(6, 8).Insert(6, "-").Insert(4, "-");

                    if (DateTime.TryParse(strBirthday, out time) == false)
                    {
                        return false;
                    }

                    //最后一位校验码验证
                    var arrVarifyCode = ("1,0,x,9,8,7,6,5,4,3,2").Split(',');
                    var Wi = ("7,9,10,5,8,4,2,1,6,3,7,9,10,5,8,4,2").Split(',');
                    var Ai = idCardNum.Remove(17).ToCharArray();
                    int sum = 0;
                    for (int i = 0; i < 17; i++)
                    {
                        sum += int.Parse(Wi[i]) * int.Parse(Ai[i].ToString());
                    }
                    int varifyCodeIndex;
                    Math.DivRem(sum, 11, out varifyCodeIndex);
                    if (arrVarifyCode[varifyCodeIndex] != idCardNum.Substring(17, 1).ToLower())
                    {
                        return false;
                    }

                    #endregion

                    break;
                case 15:

                    #region 15位身份证

                    if (long.TryParse(idCardNum, out n) == false || n < Math.Pow(10, 14))
                    {
                        return false;
                    }
                    strBirthday = idCardNum.Substring(6, 6).Insert(4, "-").Insert(2, "-");
                    if (DateTime.TryParse(strBirthday, out time) == false)
                    {
                        return false;
                    }

                    #endregion

                    break;
                default:
                    return false;
            }

            return true;
        }

        /// <summary>
        /// 计算SHA1哈希值
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string SHA1_Hash(this string source)
        {
            Encoding encoding = Encoding.UTF8;
            byte[] bytes = encoding.GetBytes(source);
            SHA1 sha1 = SHA1.Create();
            byte[] hashBytes = sha1.ComputeHash(bytes);
            return BitConverter.ToString(hashBytes).Replace("-", "");
        }
    }
}
