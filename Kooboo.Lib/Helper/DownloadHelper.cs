//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Net.Http.Headers;
using System.IO.Compression;
using System.Collections.Generic;

namespace Kooboo.Lib.Helper
{
    public static class DownloadHelper
    {
        static DownloadHelper()
        {
           // ServicePointManager.ServerCertificateValidationCallback += CheckValidationResult;
            //turn on tls12 and tls11,default is ssl3 and tls
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11; 
        }

        private static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            //make self signed cert ,so not validate cert in client
            return true;
        }
  
        public static byte[] DownloadFile(string absoluteUrl, string containsContentType = null)
        {
            byte[] bytes = null;
            bool downloadok = true;

            try
            {
                using (MyWebClient client = new MyWebClient())
                {
                    client.Proxy = null;
                    bytes = client.DownloadData(absoluteUrl);
                    if (containsContentType != null)
                    {
                        string contenttype = client.ResponseHeaders["content-type"];
                        if (string.IsNullOrEmpty(contenttype) || !contenttype.ToLower().Contains(containsContentType.ToLower()))
                        {
                            downloadok = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                downloadok = false;
            }

            if (downloadok && bytes != null && bytes.Length > 0)
            {
                return bytes;
            }
            else
            {
                return null;
            }
        }

        public static async Task<byte[]> DownloadFileAsync(string absoluteUrl, System.Net.CookieContainer cookiecontainer = null,  string contenttype=null)
        {
            if (contenttype !=null)
            {
                contenttype = contenttype.ToLower(); 
            }

            var download = await DownloadUrlAsync(absoluteUrl, cookiecontainer); 

           if (download == null)
            {
                return null; 
            }

           if (!string.IsNullOrEmpty(contenttype))
            {
                if (download.ContentType !=null && download.ContentType.ToLower().Contains(contenttype))
                {
                    if (download.DataBytes !=null)
                    {
                        return download.DataBytes;  
                    }
                }
                else
                {
                    return new byte[0]; 
                }
            }
           else
            {
                return download.DataBytes; 
            }
            return null;
        }

        public static DownloadContent DownloadUrl(string fullUrl)
        {
            try
            {
                HttpWebResponse response;
                response = RequestGet(fullUrl);

                if (response == null)
                {
                    return null;
                }

                if (IsRedirect(response))
                {
                    var url = RedirectUrl(response);
                    if (!string.IsNullOrEmpty(url) && !url.ToLower().StartsWith("http"))
                    {
                        url = Lib.Helper.UrlHelper.Combine(fullUrl, url);
                    }
                    return DownloadUrl(url);
                }

                return ProcessResponse(response);
            }
            catch (Exception ex)
            {
                // throw ex;
            }

            return null;

        }

        public static async Task<DownloadContent> DownloadUrlAsync(string fullUrl, CookieContainer cookieContainer = null)
        {
            try
            {
                HttpClient client = HttpClientHelper.Client;

                HttpClientHelper.SetCookieContainer(cookieContainer, fullUrl);

                var response = await client.GetAsync(fullUrl);

                if (response == null)
                {
                    return null;
                }

                var statuscode = (int)response.StatusCode;
                if (statuscode >= 300 && statuscode <= 399)
                {
                    var url = response.Headers.GetValues("Location").FirstOrDefault();
                    if (!string.IsNullOrEmpty(url) && !url.ToLower().StartsWith("http"))
                    {
                        url = Lib.Helper.UrlHelper.Combine(fullUrl, url);
                    }
                    return await DownloadUrlAsync(url, cookieContainer);
                }
                return await ProcessResponse1(response);
            }
            catch (Exception ex)
            {
                // throw ex;
            }
            return null;
        }

        internal static async Task<DownloadContent> ProcessResponse1(HttpResponseMessage response)
        {
            DownloadContent downcontent = new DownloadContent();

            downcontent.ResponseHeader = response.Headers; 
       
            if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Moved && response.StatusCode != HttpStatusCode.Found)
            {
                downcontent.StatusCode = 0;
                return null;
            }
            else
            {
                downcontent.StatusCode = 200;
            }

            var contentType = response.Content.Headers.ContentType?.ToString();

            if (!string.IsNullOrEmpty(contentType))
            {
                downcontent.ContentType = contentType.ToLower();
            }
            
            if (string.IsNullOrEmpty(downcontent.ContentType) || IOHelper.IsStringType(downcontent.ContentType))
            {
                downcontent.isString = true;

                var databytes =await GetDataBytes(response);
                if (databytes == null) return downcontent;
                downcontent.DataBytes = databytes;

                var encoding = EncodingDetector.GetEncoding(ref databytes, contentType);
                if (encoding == null) return downcontent;

                downcontent.ContentString = encoding.GetString(databytes);
                downcontent.Encoding = encoding.WebName;

            }
            else
            { 
                downcontent.isString = false;
            }

            return downcontent;
        }

        private static async Task<byte[]> GetDataBytes(HttpResponseMessage response)
        {
            var responseStream = await response.Content.ReadAsStreamAsync();
            Stream stream = responseStream;
            try
            {
                //support common compression methods:gzip and deflate
                if (response.Content.Headers.ContentEncoding.Contains("gzip"))
                {
                    stream = new GZipStream(responseStream, CompressionMode.Decompress);
                }
                else if (response.Content.Headers.ContentEncoding.Contains("deflate"))
                {
                    stream = new DeflateStream(responseStream, CompressionMode.Decompress);
                }
                using (var memory=new MemoryStream())
                {
                    await stream.CopyToAsync(memory);
                    return memory.ToArray();
                }
            }
            catch
            {

            }
            finally
            {
                stream.Close();
                responseStream.Close();
            }

            return null;
            
        }

        private static DownloadContent ProcessResponse(HttpWebResponse httpWebResponse)
        {
            using (var responseStream = httpWebResponse.GetResponseStream())
            {
                DownloadContent downcontent = new DownloadContent();
                MemoryStream memorystream = new MemoryStream();
                responseStream.CopyTo(memorystream);
                byte[] databytes = memorystream.ToArray();

                if (httpWebResponse.StatusCode != HttpStatusCode.OK && httpWebResponse.StatusCode != HttpStatusCode.Moved && httpWebResponse.StatusCode != HttpStatusCode.Found)
                {
                    downcontent.StatusCode = 0;
                    return null;
                }
                else
                {
                    downcontent.StatusCode = 200;
                }

                var contentType = httpWebResponse.Headers["content-type"];

                if (!string.IsNullOrEmpty(contentType))
                {
                    downcontent.ContentType = contentType.ToLower();
                }

                if (string.IsNullOrEmpty(downcontent.ContentType) || IOHelper.IsStringType(downcontent.ContentType))
                {
                    downcontent.isString = true;

                    var encoding = EncodingDetector.GetEncoding(ref databytes, contentType);
                    if (encoding != null)
                    {
                        downcontent.ContentString = encoding.GetString(databytes);
                        downcontent.Encoding = encoding.WebName;
                    }
                }
                else
                {
                    downcontent.DataBytes = databytes;
                    downcontent.isString = false;
                }

                return downcontent;

            }
        }

        public static HttpWebResponse RequestGet(string url)
        {
            var httpWebRequest = (HttpWebRequest)HttpWebRequest.Create(url);

            httpWebRequest.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;

            httpWebRequest.Method = "GET";
            httpWebRequest.AllowAutoRedirect = false;
            httpWebRequest.ProtocolVersion = HttpVersion.Version10;
            httpWebRequest.Proxy = null;
            httpWebRequest.Accept = "*/*";
            httpWebRequest.Timeout = 15000;
            httpWebRequest.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/52.0.2743.116 Safari/537.36";
            httpWebRequest.Headers.Add("Upgrade-Insecure-Requests", "1");
            var webResponse = httpWebRequest.GetResponse();

            if (webResponse is HttpWebResponse)
            {
                return (HttpWebResponse)webResponse;
            }
            return null;
        }
         
        public static HttpWebResponse RequestHeader(Uri uri)
        {
            ServicePointManager.DefaultConnectionLimit = 512;
            var httpWebRequest = (HttpWebRequest)HttpWebRequest.Create(uri);

            httpWebRequest.Method = "HEAD";
            httpWebRequest.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/52.0.2743.116 Safari/537.36";
            httpWebRequest.ProtocolVersion = HttpVersion.Version10;
            httpWebRequest.Proxy = null;
            httpWebRequest.AllowAutoRedirect = false;
            httpWebRequest.Timeout = 20000;

            var webResponse = httpWebRequest.GetResponse();

            if (webResponse is HttpWebResponse)
            {
                return (HttpWebResponse)webResponse;
            }
            return null;
        }
         
        public static string RedirectUrl(HttpWebResponse webresponse)
        {
            return webresponse.Headers["Location"];
        }

        public static bool IsRedirect(HttpWebResponse webresponse)
        {
            int statuscode = (int)webresponse.StatusCode;
            if (statuscode >= 300 && statuscode <= 399)
            {
                return true;
            }
            return false;
        }
    }

    public class DownloadContent
    { 
    

        public bool isString { get; set; }

        public string ContentType { get; set; }

        public string ContentString { get; set; }

        public int StatusCode { get; set; }

        public byte[] DataBytes { get; set; }

        public string GetString()
        {
            if (isString && !string.IsNullOrEmpty(ContentString))
            {
                return ContentString;
            }
            else
            {
                if (DataBytes != null)
                {
                    ContentString = System.Text.Encoding.UTF8.GetString(DataBytes);
                    return ContentString;
                }
            }

            return string.Empty;
        }

        public string Encoding { get; set; }

        public HttpResponseHeaders ResponseHeader { get; set; }

    }

    public class MyWebClient : WebClient
    {
        protected override WebRequest GetWebRequest(Uri uri)
        {
            WebRequest w = base.GetWebRequest(uri);
            w.Timeout = 30000;
            return w;
        }
    }
}
