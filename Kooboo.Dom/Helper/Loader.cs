//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace Kooboo.Dom
{

    public class Loader
    {


        public static string DownloadCss(string fullurl)
        {
            DownloadContent content = downloadUrl(fullurl);

            if (content == null)
            {
                return string.Empty;
            }

            if (content.ContentType.Contains("css"))
            {
                return content.getString();
            }
            else
            {
                return string.Empty;
            }
        }


        /// <summary>
        /// load css from internet or local file. 
        /// </summary>
        /// <param name="FullUrlOrPath"></param>
        /// <returns></returns>
        public static string LoadCss(string FullUrlOrPath)
        {
            string csstext = string.Empty;

            if (FullUrlOrPath.ToLower().StartsWith("http://") || FullUrlOrPath.ToLower().StartsWith("https://"))
            {
                csstext = Loader.DownloadCss(FullUrlOrPath);
            }
            else
            {
                if (FullUrlOrPath.Contains(":"))
                {
                    if (System.IO.File.Exists(FullUrlOrPath))
                    {
                        try
                        {
                            csstext = System.IO.File.ReadAllText(FullUrlOrPath);
                        }
                        catch (Exception)
                        {
                            //Error handling TODO:
                        }

                    }

                }
            }
            return csstext;
        }


        public static string DownloadHtml(string fullurl)
        {
            DownloadContent content = downloadUrl(fullurl);

            if (content == null)
            {
                return string.Empty;
            }
            else
            {
                return content.getString();

            }
        }


        public static string LoadHtml(string FullUrlOrPath)
        {

            string text = string.Empty;

            if (FullUrlOrPath.ToLower().StartsWith("http://") || FullUrlOrPath.ToLower().StartsWith("https://"))
            {
                text = DownloadHtml(FullUrlOrPath);
            }
            else
            {
                if (FullUrlOrPath.Contains(":"))
                {
                    if (System.IO.File.Exists(FullUrlOrPath))
                    {
                        try
                        {
                            text = System.IO.File.ReadAllText(FullUrlOrPath);
                        }
                        catch (Exception)
                        {
                            //Error handling TODO:
                        }

                    }

                }
            }
            return text;

        }


        private static Encoding ParseEncoding(byte[] databyes, string contentType)
        {
            String charset = string.Empty;
            if (!string.IsNullOrEmpty(contentType))
            {
                string[] sb = contentType.Split(';');
                foreach (var item in sb)
                {
                    if (item.IndexOf("charset") != -1)
                    {
                        int ind = item.IndexOf("=");
                        if (ind != -1)
                        {
                            charset = item.Substring(ind + 1).Trim();
                        }
                    }
                }
            }
            else
            {
                return Encoding.UTF8;
            }

            //
            // if ContentType is null, or did not contain charset, we search in body
            //
            if (string.IsNullOrEmpty(charset) && !contentType.Contains("css") && !contentType.Contains("script"))
            {

                string meta = System.Text.Encoding.ASCII.GetString(databyes);

                if (!string.IsNullOrEmpty(meta))
                {
                    string pattern = "charset\\s*=\\s*[\"']?(?<charset>.*?)[\"'\\s]";

                    Match matchCharset = Regex.Match(meta, pattern);
                    if (matchCharset.Success)
                    {
                        charset = matchCharset.Groups["charset"].Value;
                    }
                }
            }

            Encoding e = null;
            if (string.IsNullOrEmpty(charset))
            {
                e = System.Text.Encoding.UTF8; //default encoding
            }
            else
            {
                try
                {
                    e = Encoding.GetEncoding(charset);
                }
                catch (Exception)
                {
                    e = Encoding.UTF8;
                }
            }

            return e;
        }


        public static byte[] downloadFile(string absoluteUrl)
        {
            byte[] imagebytes = null;
            bool downloadok = true;

            try
            {
                imagebytes = new System.Net.WebClient().DownloadData(absoluteUrl);
            }
            catch (Exception)
            {
                downloadok = false;
            }

            if (downloadok)
            {
                return imagebytes;
            }
            else
            {
                return null;
            }

        }


        public static DownloadContent downloadUrl(string fullurl)
        {
            try
            {
                Uri uri = new Uri(fullurl);
                return downloadUrl(uri);
            }
            catch (Exception)
            {
                // TODO: handle exception.
            }
            return null;

        }


        public static DownloadContent downloadUrl(Uri uri)
        {
            DownloadContent down = null;

            try
            {
                var httpWebRequest = (HttpWebRequest)HttpWebRequest.Create(uri);

                httpWebRequest.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;

                httpWebRequest.Method = "GET";

                httpWebRequest.AllowAutoRedirect = true;

                var webResponse = httpWebRequest.GetResponse();

                if (webResponse is HttpWebResponse)
                {
                    down = ProcessResponse((HttpWebResponse)webResponse);
                    if (down != null)
                    {

                        return down;
                    }
                }
            }
            catch (Exception)
            {

            }

            return down;
        }

        protected static DownloadContent ProcessResponse(HttpWebResponse httpWebResponse)
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

                downcontent.ContentType = contentType.ToLower();

                if (string.IsNullOrEmpty(downcontent.ContentType) || downcontent.ContentType.Contains("text"))
                {

                    string text = string.Empty;
                    Encoding encoding = Encoding.Default;

                    encoding = ParseEncoding(databytes, contentType);

                    text = encoding.GetString(databytes).Trim(new[] { '\uFEFF', '\u200B' });

                    downcontent.isString = true;
                    downcontent.ContentString = text;

                }

                else
                {

                    downcontent.DataBytes = databytes;
                    downcontent.isString = false;
                }

                return downcontent;

            }
        }


        public class DownloadContent
        {
            public DownloadContent()
            {
                this.isString = true;
            }

            /// <summary>
            ///  the full url. 
            /// </summary>
            public string Url;

            public bool isString;

            public string ContentType;

            public string ContentString;

            public int StatusCode;

            public byte[] DataBytes;

            public string getString()
            {
                if (isString && !string.IsNullOrEmpty(ContentString))
                {
                    return ContentString;
                }
                else
                {
                    if (DataBytes != null)
                    {
                        return System.Text.Encoding.ASCII.GetString(DataBytes);
                    }
                }

                return string.Empty;
            }

        }


    }











}
