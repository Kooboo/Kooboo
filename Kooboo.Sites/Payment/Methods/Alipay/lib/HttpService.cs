using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Web;

namespace Kooboo.Sites.Payment.Methods.Alipay.lib
{
    public class HttpService
    {
        public static string DoPost(string url, IDictionary<string, string> parameters, string charset)
        {
            var req = GetWebRequest(url, "POST");
            req.ContentType = "application/x-www-form-urlencoded;charset=" + charset;

            var postData = Encoding.GetEncoding(charset).GetBytes(BuildQuery(parameters, charset));
            var reqStream = req.GetRequestStream();
            reqStream.Write(postData, 0, postData.Length);
            reqStream.Close();

            var rsp = (HttpWebResponse)req.GetResponse();
            var encoding = Encoding.GetEncoding(rsp.CharacterSet);
            return GetResponseAsString(rsp, encoding);
        }

        private static string GetResponseAsString(HttpWebResponse rsp, Encoding encoding)
        {
            var result = new StringBuilder();
            Stream stream = null;
            StreamReader reader = null;

            try
            {
                // 以字符流的方式读取HTTP响应
                stream = rsp.GetResponseStream();
                reader = new StreamReader(stream, encoding);

                // 按字符读取并写入字符串缓冲
                var ch = -1;
                while ((ch = reader.Read()) > -1)
                {
                    // 过滤结束符
                    var c = (char)ch;
                    if (c != '\0')
                        result.Append(c);
                }
            }
            finally
            {
                // 释放资源
                if (reader != null) reader.Close();
                if (stream != null) stream.Close();
                if (rsp != null) rsp.Close();
            }

            return result.ToString();
        }

        private static HttpWebRequest GetWebRequest(string url, string method, int timeout = 100000)
        {
            var req = (HttpWebRequest)WebRequest.Create(url);
            //			req.ServicePoint.Expect100Continue = false;
            req.Method = method;
            req.KeepAlive = true;
            req.UserAgent = "Aop4Net";
            req.Timeout = timeout;
            return req;
        }

        private static string BuildQuery(IDictionary<string, string> parameters, string charset)
        {
            var postData = new StringBuilder();
            var hasParam = false;

            var dem = parameters.GetEnumerator();
            while (dem.MoveNext())
            {
                var name = dem.Current.Key;
                var value = dem.Current.Value;
                // 忽略参数名或参数值为空的参数
                if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(value))
                {
                    if (hasParam)
                        postData.Append("&");

                    postData.Append(name);
                    postData.Append("=");

                    var encodedValue = HttpUtility.UrlEncode(value, Encoding.GetEncoding(charset));

                    postData.Append(encodedValue);
                    hasParam = true;
                }
            }

            return postData.ToString();
        }
    }
}
