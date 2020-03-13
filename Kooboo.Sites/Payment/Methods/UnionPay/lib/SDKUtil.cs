﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace Kooboo.Sites.Payment.Methods.UnionPay.lib
{
    public class SDKUtil
    {
        /// <summary>
        /// 把请求要素按照“参数=参数值”的模式用“&”字符拼接成字符串
        /// </summary>
        /// <param name="para">请求要素</param>
        /// <param name="sort">是否需要根据key值作升序排列</param>
        /// <param name="encode">是否需要URL编码</param>
        /// <returns>拼接成的字符串</returns>
        public static String CreateLinkString(Dictionary<String, String> para, bool sort, bool encode, Encoding encoding)
        {
            if (para == null || para.Count == 0)
                return "";
            List<String> list = new List<String>(para.Keys);

            if (sort)
                list.Sort(StringComparer.Ordinal);

            StringBuilder sb = new StringBuilder();
            foreach (String key in list)
            {
                String value = para[key];
                if (encode && value != null)
                {
                    try
                    {
                        value = HttpUtility.UrlEncode(value, encoding);
                    }
                    catch (Exception ex)
                    {
                        //LogError(ex);
                        return "#ERROR: HttpUtility.UrlEncode Error!" + ex.Message;
                    }
                }
                sb.Append(key).Append("=").Append(value).Append("&");
            }

            return sb.Remove(sb.Length - 1, 1).ToString();
        }

        // 小写返回
        public static string ByteArray2HexString(byte[] data)
        {
            return BitConverter.ToString(data).Replace("-", "").ToLower();
        }

        /// <summary>
        /// 将字符串key1=value1&key2=value2转换为Dictionary数据结构。
        /// deprecated：为兼容原始sdk没加中文编码，遇到中文乱码请改调用parseQString。
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static Dictionary<string, string> CoverStringToDictionary(string str, Encoding encoding)
        {
            return parseQString(str, encoding);
        }

        /// <summary>
        /// 将字符串key1=value1&key2=value2转换为Dictionary数据结构。
        /// 这个故事告诉我们，应答报文不带url编码是一件无比蛋疼的事。
        /// </summary>
        /// <param name="data"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static Dictionary<string, string> parseQString(string str, Encoding encoding)
        {
            Dictionary<String, String> Dictionary = new Dictionary<String, String>();
            int len = str.Length;
            StringBuilder temp = new StringBuilder();
            char curChar;
            String key = null;
            bool isKey = true;
            bool isOpen = false;//值里有嵌套
            char openName = '\0'; //关闭符

            for (int i = 0; i < len; i++)
            {// 遍历整个带解析的字符串
                curChar = str[i];// 取当前字符
                if (isOpen)
                {
                    if (curChar == openName)
                    {
                        isOpen = false;
                    }
                    temp.Append(curChar);
                }
                else if (curChar == '{')
                {
                    isOpen = true;
                    openName = '}';
                    temp.Append(curChar);
                }
                else if (curChar == '[')
                {
                    isOpen = true;
                    openName = ']';
                    temp.Append(curChar);
                }
                else if (isKey && curChar == '=')
                {// 如果当前生成的是key且如果读取到=分隔符
                    key = temp.ToString();
                    temp = new StringBuilder();
                    isKey = false;
                }
                else if (curChar == '&' && !isOpen)
                {// 如果读取到&分割符
                    putKeyValueToDictionary(temp, isKey, key, Dictionary, encoding);
                    temp = new StringBuilder();
                    isKey = true;
                }
                else
                {
                    temp.Append(curChar);
                }
            }
            if (key != null)
                putKeyValueToDictionary(temp, isKey, key, Dictionary, encoding);
            return Dictionary;
        }

        private static void putKeyValueToDictionary(StringBuilder temp, bool isKey, String key, Dictionary<String, String> Dictionary, Encoding encoding)
        {
            if (isKey)
            {
                key = temp.ToString();
                if (key.Length == 0)
                {
                    throw new System.Exception("QString format illegal");
                }
                Dictionary[key] = "";
            }
            else
            {
                if (key.Length == 0)
                {
                    throw new System.Exception("QString format illegal");
                }
                //Dictionary[key] = HttpUtility.UrlDecode(temp.ToString(), encoding);
                Dictionary[key] = temp.ToString();
            }
        }
    }
}
