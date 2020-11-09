//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.

#if !NETSTANDARD2_0


using System;
using Kooboo.Data.Context;
using Kooboo.HttpServer;
using System.Threading.Tasks;
using System.Linq;
using System.IO;
using System.Collections.Specialized;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.Text;
using Kooboo.Lib.Helper;
using Kooboo.HttpServer.Http;

namespace Kooboo.Data.Server
{

    public class ServerHandler : IHttpHandler
    {
        public Func<RenderContext, Task> _handle;

        public ServerHandler(Func<RenderContext, Task> handle)
        {
            _handle = handle;
        }

        public async Task Handle(HttpContext context)
        {
            RenderContext renderContext = await GetRenderContext(context);
            try
            {
                await _handle(renderContext);
                await SetResponse(context, renderContext);
            }
            catch (Exception ex)
            {
                renderContext.Response.StatusCode = 500;
                renderContext.Response.Body = System.Text.Encoding.UTF8.GetBytes(ex.Message);
                await SetResponse(context, renderContext);
            }
        }


        public static async Task<RenderContext> GetRenderContext(HttpContext httpContext)
        {
            RenderContext context = new RenderContext();
            context.Request = await GetRequest(httpContext);
            return context;
        }


        private static async Task<Context.HttpRequest> GetRequest(HttpContext context)
        {
            var header = context.Features.Request.Headers as Kooboo.HttpServer.Http.HttpRequestHeaders;

            Context.HttpRequest httprequest = new Context.HttpRequest();

            // httprequest.Host

            if (header != null && header.HeaderHost.Any())
            {
                string host = header.HeaderHost.First();

                int delimiterIndex = host.IndexOf(":");
                if (delimiterIndex > 0)
                {
                    host = host.Substring(0, delimiterIndex);
                }
                httprequest.Host = host;
            }

            httprequest.Path = context.Features.Request.Path;
            var url = GetUrl(context.Features.Request.RawTarget);

            httprequest.Url = url;
            httprequest.RawRelativeUrl = url;
            httprequest.Method = context.Features.Request.Method.ToUpper();
            httprequest.IP = context.Features.Connection.RemoteEndPoint.Address.ToString();


            httprequest.Port = context.Features.Connection.LocalEndPoint.Port;
            httprequest.Scheme = context.Features.Request.Scheme;

            foreach (var item in header)
            {
                httprequest.Headers.Add(item.Key, item.Value);
            }

            var headerip = httprequest.Headers.Get("X-Forwarded-For");

            if (!string.IsNullOrWhiteSpace(headerip))
            {
                httprequest.IP = headerip;
            }


            foreach (var item in context.Features.Request.Query)
            {
                httprequest.QueryString.Add(item.Key, item.Value);
            }
            httprequest.Cookies = GetCookie(context);

            if (httprequest.Method != "GET")
            {
                if (context.Features.Request.Body != null && context.Features.Request.Body.CanRead)
                {
                    MemoryStream ms = new MemoryStream();
                    var body = context.Features.Request.Body;
                    await context.Features.Request.Body.CopyToAsync(ms);
                    httprequest.PostData = ms.ToArray();
                    ms.Dispose();
                }


                var contenttype = httprequest.Headers.Get("Content-Type");
                if (contenttype != null && contenttype.ToLower().Contains("multipart"))
                {
                    var formresult = Kooboo.Lib.NETMultiplePart.FormReader.ReadForm(httprequest.PostData);

                    httprequest.Forms = new NameValueCollection();
                    if (formresult.FormData != null)
                    {
                        foreach (var item in formresult.FormData)
                        {
                            httprequest.Forms.Add(item.Key, item.Value);
                        }
                    }

                    httprequest.Files = formresult.Files;

                }
                else
                {
                    httprequest.Forms.Add(GetForm(httprequest.PostData, contenttype));
                }

            }
            return httprequest;
        }

        private static string GetUrl(string target)
        {
            if (string.IsNullOrEmpty(target))
            {
                return "/";
            }
            else
            {
                return System.Net.WebUtility.UrlDecode(target);
            }
        }


        internal static NameValueCollection GetForm(byte[] inputstream, string contenttype = null)
        {
            bool hasEncoded = false;
            if (contenttype != null && contenttype.ToLower().Contains("urlencoded"))
            {
                hasEncoded = true;
            }

            // The encoding type of a form is determined by the attribute enctype.It can have three values, 
            //application / x - www - form - urlencoded - Represents an URL encoded form. This is the default value if enctype attribute is not set to anything. 
            //multipart / form - data - Represents a Multipart form.This type of form is used when the user wants to upload files 
            //text / plain - A new form type introduced in HTML5, that as the name suggests, simply sends the data without any encoding


            NameValueCollection result = new NameValueCollection();

            string text = System.Text.Encoding.UTF8.GetString(inputstream);

            if (text == null)
            {
                return result;
            }

            //if (hasEncoded && text !=null)
            //{
            //    text = System.Net.WebUtility.UrlDecode(text); 
            //}

            int textLength = text.Length;
            int equalIndex = text.IndexOf('=');
            if (equalIndex == -1)
            {
                equalIndex = textLength;
            }
            int scanIndex = 0;
            while (scanIndex < textLength)
            {
                int delimiterIndex = text.IndexOf("&", scanIndex);
                if (delimiterIndex == -1)
                {
                    delimiterIndex = textLength;
                }
                if (equalIndex < delimiterIndex)
                {
                    while (scanIndex != equalIndex && char.IsWhiteSpace(text[scanIndex]))
                    {
                        ++scanIndex;
                    }
                    string name = text.Substring(scanIndex, equalIndex - scanIndex);
                    if (hasEncoded)
                    {
                        name = System.Net.WebUtility.UrlDecode(name);
                    }
                    //   name= Uri.UnescapeDataString(name);

                    string value = text.Substring(equalIndex + 1, delimiterIndex - equalIndex - 1);
                    // value= Uri.UnescapeDataString(value);
                    if (hasEncoded)
                    {
                        value = System.Net.WebUtility.UrlDecode(value);
                    }

                    result.Add(name, value);
                    equalIndex = text.IndexOf('=', delimiterIndex);
                    if (equalIndex == -1)
                    {
                        equalIndex = textLength;
                    }
                }
                scanIndex = delimiterIndex + 1;
            }

            return result;
        }

        private static Dictionary<string, string> GetCookie(HttpContext context)
        {
            Dictionary<string, string> cookies = new Dictionary<string, string>();

            foreach (var item in context.Features.Request.Cookies)
            {
                if (!cookies.ContainsKey(item.Key))
                {
                    cookies.Add(item.Key, item.Value);
                }
            }
            return cookies;
        }

        public static async Task SetResponse(HttpContext context, RenderContext renderContext)
        {
            var response = renderContext.Response;

            var header = context.Features.Response.Headers as Kooboo.HttpServer.Http.HttpResponseHeaders;

            context.Features.Response.StatusCode = response.StatusCode;

            if (response.StatusCode == 200)
            {
                context.Features.Response.Headers["Server"] = "http://www.kooboo.com";

                foreach (var item in response.Headers)
                {
                    context.Features.Response.Headers[item.Key] = item.Value;
                }

                foreach (var item in response.DeletedCookieNames)
                {
                    // context.Features.Response.Cookies.Delete(item);
                    var options = new CookieOptions()
                    {
                        Domain = renderContext.Request.Host,
                        Path = "/",
                        Expires = DateTime.Now.AddDays(-30)
                    };
                    context.Features.Response.Cookies.Append(item, "", options);

                    response.AppendedCookies.RemoveAll(o => o.Name == item);
                }

                foreach (var item in response.AppendedCookies)
                {
                    if (string.IsNullOrEmpty(item.Domain))
                    {
                        item.Domain = renderContext.Request.Host;
                    }
                    if (string.IsNullOrEmpty(item.Path))
                    {
                        item.Path = "/";
                    }

                    if (item.Expires == default(DateTime))
                    {
                        var time = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Local);
                        time = time.AddSeconds(-1);
                        item.Expires = time;
                    }

                    var options = new CookieOptions()
                    {
                        Domain = item.Domain,
                        Path = item.Path,
                        Expires = item.Expires
                    };

                    context.Features.Response.Cookies.Append(item.Name, item.Value, options);
                    // }   
                }

                header.HeaderContentType = response.ContentType;

                if (response.Body != null && context.Features.Request.Method.ToLower() != "head")
                {

                    try
                    {
                        header.ContentLength = response.Body.Length;
                        await context.Features.Response.Body.WriteAsync(response.Body, 0, response.Body.Length).ConfigureAwait(false);
                    }
                    catch (Exception)
                    {
                        context.Features.Response.Body.Close();
                    }

                }
                else
                {
                    if (response.Stream != null)
                    {
                        response.Stream.CopyTo(context.Features.Response.Body);
                    }
                    else if (response.FilePart != null)
                    {
                        await WritePartToResponse(response.FilePart, context.Features.Response);
                    }

                    else
                    {
                        // 404.
                        //string filename = Lib.Helper.IOHelper.CombinePath(AppSettings.RootPath, Kooboo.DataConstants.Default404Page) + ".html";
                        //if (System.IO.File.Exists(filename))
                        //{
                        //    string text = System.IO.File.ReadAllText(filename);
                        //    var bytes = System.Text.Encoding.UTF8.GetBytes(text);
                        //    header.ContentLength = bytes.Length;
                        //    context.Features.Response.StatusCode = 404;
                        //    await context.Features.Response.Body.WriteAsync(bytes, 0, bytes.Length);
                        //}

                    }
                }
            }
            else
            {

                string location = response.RedirectLocation;

                context.Features.Response.Headers["Server"] = "http://www.kooboo.com";

                foreach (var item in response.Headers)
                {
                    context.Features.Response.Headers[item.Key] = item.Value;
                }

                foreach (var item in response.DeletedCookieNames)
                {
                    var options = new CookieOptions()
                    {
                        Domain = renderContext.Request.Host,
                        Path = "/",
                        Expires = DateTime.Now.AddDays(-30)
                    };

                    context.Features.Response.Cookies.Append(item, "", options);
                }

                foreach (var item in response.AppendedCookies)
                {
                    if (string.IsNullOrEmpty(item.Domain))
                    {
                        item.Domain = renderContext.Request.Host;
                    }
                    if (string.IsNullOrEmpty(item.Path))
                    {
                        item.Path = "/";
                    }

                    var options = new CookieOptions()
                    {
                        Domain = item.Domain,
                        Path = item.Path,
                        Expires = item.Expires
                    };

                    context.Features.Response.Cookies.Append(item.Name, item.Value, options);
                }

                if (!string.IsNullOrEmpty(location))
                {

                    var host = renderContext.Request.Port == 80 || renderContext.Request.Port == 443
                        ? renderContext.Request.Host
                        : string.Format("{0}:{1}", renderContext.Request.Host, renderContext.Request.Port);
                    string BaseUrl = renderContext.Request.Scheme + "://" + host + renderContext.Request.Path;
                    var newUrl = UrlHelper.Combine(BaseUrl, location);
                    if (response.StatusCode != 200)
                    {
                        context.Features.Response.StatusCode = response.StatusCode;
                    }
                    //status code doesn't start with 3xx,it'will not redirect.
                    if (!response.StatusCode.ToString().StartsWith("3"))
                    {
                        context.Features.Response.StatusCode = StatusCodes.Status302Found;
                    }

                    header.HeaderLocation = newUrl;

                    context.Features.Response.Body.Dispose();

                    Log(renderContext);
                    return;
                }

                if (response.Body != null && response.Body.Length > 0)
                {
                    context.Features.Response.StatusCode = response.StatusCode;
                    await context.Features.Response.Body.WriteAsync(response.Body, 0, response.Body.Length).ConfigureAwait(false);

                    context.Features.Response.Body.Dispose();
                    Log(renderContext);
                    return;
                }

                context.Features.Response.StatusCode = response.StatusCode;
                string responsebody = null;
                switch (response.StatusCode)
                {
                    case 404:
                        responsebody = " The requested resource not found";
                        break;
                    case 301:
                        responsebody = " The requested resource has moved.";
                        break;
                    case 302:
                        responsebody = " The requested resource has moved.";
                        break;
                    case 401:
                        responsebody = " Unauthorized access";
                        break;
                    case 403:
                        responsebody = " Unauthorized access";
                        break;
                    case 407:
                        responsebody = "Reach Limitation";
                        break;
                    case 500:
                        responsebody = "Internal server error";
                        break;
                    case 503:
                        responsebody = " Service Unavailable";
                        break;
                    default:
                        break;
                }

                if (string.IsNullOrEmpty(responsebody))
                {
                    if (response.StatusCode >= 400 && response.StatusCode < 500)
                    {
                        responsebody = " Client error";
                    }
                    else if (response.StatusCode >= 500)
                    {
                        responsebody = " Server error";
                    }
                    else
                    {
                        responsebody = " Unknown error";
                    }
                }
                var bytes = Encoding.UTF8.GetBytes(responsebody);
                await context.Features.Response.Body.WriteAsync(bytes, 0, bytes.Length).ConfigureAwait(false);
            }


            context.Features.Response.Body.Dispose();
            Log(renderContext);
            context = null;
        }


        private static async Task WritePartToResponse(Kooboo.IndexedDB.FilePart part, HttpResponseFeature Res)
        {
            long offset = part.BlockPosition + part.RelativePosition;
            byte[] buffer = new byte[8096];
            long totalToSend = part.Length;
            int count = 0;

            Res.Headers["Content-Length"] = totalToSend.ToString();

            long bytesRemaining = totalToSend;

            var stream = Kooboo.IndexedDB.StreamManager.OpenReadStream(part.FullFileName);

            stream.Position = offset;

            while (bytesRemaining > 0)
            {
                try
                {
                    if (bytesRemaining <= buffer.Length)
                        count = await stream.ReadAsync(buffer, 0, (int)bytesRemaining);
                    else
                        count = await stream.ReadAsync(buffer, 0, buffer.Length);

                    if (count == 0)
                        return;

                    await Res.Body.WriteAsync(buffer, 0, count);

                    bytesRemaining -= count;
                }
                catch (IndexOutOfRangeException)
                {
                    await Res.Body.FlushAsync();
                    return;
                }
                finally
                {
                    await Res.Body.FlushAsync();
                }
            }
        }


        public static void Log(RenderContext context)
        {
            if (Data.AppSettings.Global.EnableLog)
            {
                string log = context.Response.StatusCode.ToString() + " " + context.Request.IP + ": " + DateTime.Now.ToLongTimeString() + " " + context.Request.Host + " " + context.Request.Method + " " + context.Request.Url;

                Kooboo.Data.Log.Instance.Http.Write(log);
            }
        }

    }

}


#endif
