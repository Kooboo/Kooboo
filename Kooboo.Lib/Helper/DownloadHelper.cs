//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;


namespace Kooboo.Lib.Helper
{
    public static class DownloadHelper
    {
        public const string DEFAULT_UA = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/52.0.2743.116 Safari/537.36";
        static DownloadHelper()
        {
            // ServicePointManager.ServerCertificateValidationCallback += CheckValidationResult;
            //turn on tls12 and tls11,default is ssl3 and tls
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls13;
            HttpHelper.SetCustomSslChecker();
        }


        public static byte[] DownloadFile(string absoluteUrl, string containsContentType = null)
        {
            return DownloadFile(absoluteUrl, out _, containsContentType);
        }

        public static byte[] DownloadFile(string absoluteUrl, out string responseContentType, string containsContentType = null)
        {
            byte[] bytes = null;
            bool downloadOK = true;
            responseContentType = null;
            try
            {
                using MyWebClient client = new MyWebClient();
                bytes = client.DownloadData(absoluteUrl);
                responseContentType = client.ResponseHeaders["content-type"];
                if (containsContentType != null)
                {
                    if (string.IsNullOrEmpty(responseContentType) || !responseContentType.ToLower().Contains(containsContentType.ToLower()))
                    {
                        downloadOK = false;
                    }
                }
            }
            catch (Exception)
            {
                downloadOK = false;
            }

            if (downloadOK && bytes != null && bytes.Length > 0)
            {
                return bytes;
            }
            else
            {
                return null;
            }
        }


        public static async Task<byte[]> DownloadFileAsync(string absoluteUrl, System.Net.CookieContainer cookieContainer = null, Dictionary<string, string> headers = null)
        {

            var download = await DownloadUrlAsync(absoluteUrl, cookieContainer, "GET", headers, null);

            return download == null ? null : download.DataBytes;
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
            catch (Exception)
            {
                // throw ex;
            }

            return null;

        }

        public static async Task<DownloadContent> DownloadUrlAsync(string fullUrl, CookieContainer cookieContainer, string method, Dictionary<string, string> headers, string PutPostBoy)
        {
            if (string.IsNullOrWhiteSpace(method))
            {
                method = "GET";
            }
            method = method.ToUpper();

            try
            {
                HttpClient client = HttpClientHelper.Client;
                //Check null inside the method. 
                HttpClientHelper.SetCookieContainer(cookieContainer, fullUrl);

                HttpResponseMessage response = null;

                if (headers != null || PutPostBoy != null)
                {
                    HttpRequestMessage request = new HttpRequestMessage(new HttpMethod(method), fullUrl);
                    string contentType = null;
                    var existContentType = headers?.TryGetValue("Content-Type", out contentType) ?? false;

                    if (headers != null)
                    {
                        if (existContentType) headers.Remove("Content-Type");

                        foreach (var item in headers)
                        {
                            if (!request.Headers.TryGetValues(item.Key, out var _))
                            {
                                request.Headers.Add(item.Key, item.Value);
                            }
                        }
                    }
                    if (PutPostBoy != null)
                    {
                        if (existContentType)
                        {
                            ContentType contentTypeInstance = new ContentType(contentType);
                            var encoding = Encoding.Default;

                            try
                            {
                                encoding = Encoding.GetEncoding(contentTypeInstance.CharSet);
                            }
                            catch (Exception)
                            {
                            }

                            var mediaType = contentTypeInstance.MediaType;
                            if (string.IsNullOrWhiteSpace(mediaType)) mediaType = "application/json";
                            request.Content = new StringContent(PutPostBoy, encoding, mediaType);
                        }
                        else
                        {
                            request.Content = new StringContent(PutPostBoy);

                        }
                    }

                    response = await client.SendAsync(request);
                }
                else
                {
                    if (method == "GET")
                    {
                        response = await client.GetAsync(fullUrl, HttpCompletionOption.ResponseHeadersRead);
                    }
                    else if (method == "POST")
                    {
                        response = await client.PostAsync(fullUrl, new StringContent(PutPostBoy));
                    }
                    else if (method == "PUT")
                    {
                        response = await client.PutAsync(fullUrl, new StringContent(PutPostBoy));
                    }
                    else if (method == "DELETE")
                    {
                        response = await client.DeleteAsync(fullUrl);
                    }
                }


                if (response == null)
                {
                    return null;
                }

                var StatusCode = (int)response.StatusCode;
                if (StatusCode >= 300 && StatusCode <= 399)
                {
                    if (response.Headers.TryGetValues("Location", out var urllist))
                    {
                        var url = urllist.FirstOrDefault();
                        if (!string.IsNullOrEmpty(url) && !url.ToLower().StartsWith("http"))
                        {
                            url = Lib.Helper.UrlHelper.Combine(fullUrl, url);
                        }

                        return await DownloadUrlAsync(url, cookieContainer, method, headers, PutPostBoy);
                    }


                }
                return await ProcessResponse1(response);
            }
            catch (Exception)
            {
                // throw ex;
            }
            return null;
        }



        internal static async Task<DownloadContent> ProcessResponse1(HttpResponseMessage response)
        {
            DownloadContent downContent = new DownloadContent();

            downContent.ResponseHeader = response.Headers;

            if (response.Content.Headers.Expires != default(DateTimeOffset) && response.Content.Headers.Expires.HasValue)
            {
                downContent.Expires = response.Content.Headers.Expires.Value.DateTime;
            }

            if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Moved && response.StatusCode != HttpStatusCode.Found)
            {
                downContent.StatusCode = 0;
                return null;
            }
            else
            {
                downContent.StatusCode = 200;
            }

            var contentType = response.Content.Headers.ContentType?.ToString();

            downContent.FileName = response.Content.Headers.ContentDisposition?.FileName;

            if (!string.IsNullOrEmpty(contentType))
            {
                downContent.ContentType = contentType.ToLower();
            }

            var dataBytes = await GetDataBytes(response);
            downContent.DataBytes = dataBytes;

            if (string.IsNullOrEmpty(downContent.ContentType) || IOHelper.IsStringType(downContent.ContentType))
            {
                downContent.isString = true;

                if (dataBytes != null)
                {
                    var encoding = EncodingDetector.GetEncoding(ref dataBytes, contentType);
                    if (encoding == null) return downContent;
                    downContent.ContentString = encoding.GetString(dataBytes);
                    downContent.Encoding = encoding.WebName;
                }
            }

            return downContent;
        }

        private static async Task<byte[]> GetDataBytes(HttpResponseMessage response)
        {
            var responseStream = await response.Content.ReadAsStreamAsync();
            Stream stream = responseStream;
            try
            {
                //support common compression methods:gzip and deflate
                if (response.Content.Headers.ContentEncoding != null)
                {
                    if (response.Content.Headers.ContentEncoding.Contains("gzip"))
                    {
                        stream = new GZipStream(responseStream, CompressionMode.Decompress);
                    }
                    else if (response.Content.Headers.ContentEncoding.Contains("deflate"))
                    {
                        stream = new DeflateStream(responseStream, CompressionMode.Decompress);
                    }
                }

                using (var memory = new MemoryStream())
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

            httpWebRequest.ServerCertificateValidationCallback = (a, b, c, d) => { return true; };
            httpWebRequest.UserAgent = DEFAULT_UA;
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
            httpWebRequest.UserAgent = DEFAULT_UA;
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

        public DateTime Expires { get; set; }

        public string FileName { get; set; }

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
