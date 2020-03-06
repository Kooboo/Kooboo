using System;
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
    }
}
