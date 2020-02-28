using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Sites.Payment.Methods.XMLCommon
{
    public class HttpService
    {
        protected static readonly Encoding encoding = Encoding.UTF8;
        public int Timeout { get; set; }

        public HttpService(int timeout = 60000)
        {
            this.Timeout = timeout;
        }

        public HttpWebRequest GetHttpRequest(string URL, string method, string accessToken = null, string username = null, string password = null)
        {
            const int SecurityProtocolTypeTls12 = 3072;
            ServicePointManager.SecurityProtocol = ServicePointManager.SecurityProtocol | ((SecurityProtocolType)SecurityProtocolTypeTls12);

            var request = WebRequest.Create(URL) as HttpWebRequest;

            var authorize = GetAuthorizationHeader(accessToken, username, password);
            if (!string.IsNullOrEmpty(authorize))
            {
                request.Headers.Add("Authorization", authorize);
            }
            SetRequestHeaders(request);
            request.Method = method;
            request.KeepAlive = false;
            request.Timeout = Timeout;
            request.ReadWriteTimeout = Timeout;
            request.ContentType = "application/xml";
            request.Accept = "application/xml";

            return request;
        }

        public string GetHttpResponse(HttpWebRequest request)
        {
            try
            {
                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    return ParseResponseStream(GetResponseStream(response));
                }
            }
            catch (WebException e)
            {
                throw e;
            }
        }

        public async Task<string> GetHttpResponseAsync(HttpWebRequest request)
        {
            try
            {
                using (var response = (HttpWebResponse)await request.GetResponseAsync().ConfigureAwait(false))
                {
                    return await ParseResponseStreamAsync(GetResponseStream(response));
                }
            }
            catch (WebException e)
            {
                throw e;
            }
        }

        public virtual void SetRequestHeaders(HttpWebRequest request)
        {
            request.Headers.Add("Accept-Encoding", "gzip");
        }

        public string GetAuthorizationHeader(string accessToken, string username, string password)
        {

            string credentials;
            if (!string.IsNullOrEmpty(accessToken))
            {
                return "Bearer " + accessToken;
            }
            else if (username != null && password != null)
            {
                credentials = username + ":" + password;
                return "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes(credentials)).Trim();
            }

            return "";
        }

        protected string ParseResponseStream(Stream stream)
        {
            string body;
            using (var streamReader = new StreamReader(stream))
            {
                body = streamReader.ReadToEnd();
            }

            return body;
        }

        protected async Task<string> ParseResponseStreamAsync(Stream stream)
        {
            string body;
            using (var streamReader = new StreamReader(stream))
            {
                body = await streamReader.ReadToEndAsync().ConfigureAwait(false);
            }

            return body;
        }

        protected Stream GetResponseStream(HttpWebResponse response)
        {
            var stream = response.GetResponseStream();
            if (response.ContentEncoding.Equals("gzip", StringComparison.CurrentCultureIgnoreCase))
            {
                stream = new GZipStream(stream, CompressionMode.Decompress);
            }
            return stream;
        }
    }
}
