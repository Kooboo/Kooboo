#if NETSTANDARD2_0

using System;
using System.Net;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Server.Kestrel.Transport.Sockets;
using Microsoft.AspNetCore.Http.Features;
using Kooboo.Data.Context;
using Microsoft.Extensions.Logging;
using System.Collections.Specialized;
using Microsoft.AspNetCore.Http;
using System.Text;
using Kooboo.Lib.Helper;

namespace Kooboo.Data.Server
{
    public class KestrelWebServer : IWebServer
    {
        private KestrelServer _server { get; set; }

        private KoobooHttpApplication _application { get; set; }

        private List<IKoobooMiddleWare> MiddleWares { get; set; } = new List<IKoobooMiddleWare>();

        private IKoobooMiddleWare StartWare { get; set; } = null;

        public KestrelWebServer(int port, List<IKoobooMiddleWare> middlewares, bool forcessl = false)
        {
            var life = new ApplicationLifetime(null);

            var log = new Microsoft.Extensions.Logging.Abstractions.NullLoggerFactory();

            SocketTransportFactory usesocket = new SocketTransportFactory(new koobooSocketOption(), life, log);

            _server = new KestrelServer(new KoobooServerOption(port, forcessl), usesocket, log);
            _server.Options.Limits.MaxRequestBodySize = 1024 * 1024 * 800;
            SetMiddleWares(middlewares);
        }

        public void Start()
        {
            _application = new KoobooHttpApplication(StartWare);

            _server.StartAsync(_application, CancellationToken.None);
        }

        public void Stop()
        {
            _server.StopAsync(CancellationToken.None);
        }

        public void SetMiddleWares(List<IKoobooMiddleWare> middlewares)
        {
            this.MiddleWares = middlewares;
            OrganizeChain();
        }

        private void OrganizeChain()
        {
            bool HasEnd = false;
            foreach (var item in this.MiddleWares)
            {
                if (item.GetType() == typeof(EndMiddleWare))
                {
                    HasEnd = true;
                }
            }
            if (!HasEnd)
            {
                this.MiddleWares.Add(new EndMiddleWare());
            }

            int count = this.MiddleWares.Count;
            for (int i = 0; i < count; i++)
            {
                if (i < count - 1)
                {
                    this.MiddleWares[i].Next = this.MiddleWares[i + 1];
                }
            }
            this.StartWare = this.MiddleWares[0];
        }

        public class koobooSocketOption : IOptions<SocketTransportOptions>
        {
            public SocketTransportOptions Value
            {
                get
                {
                    var op = new SocketTransportOptions();
                    op.IOQueueCount = 0;
                    return op;
                }
            }
        }

        public class KoobooServerOption : IOptions<KestrelServerOptions>
        {
            int port { get; set; }
            bool forcessl { get; set; }

            public KoobooServerOption(int port, bool forcessl)
            {
                this.port = port;
                this.forcessl = forcessl;
            }

            public KestrelServerOptions op { get; set; }

            public KestrelServerOptions Value
            {
                get
                {
                    if (op == null)
                    {
                        op = new KestrelServerOptions();
                        op.Limits.MaxRequestBodySize = 1024 * 1024 * 128;
                        op.Listen(IPAddress.Any, port, Configure);
                    }
                    return op;
                }
            }

            public void Configure(ListenOptions lisOption)
            {
                lisOption.Protocols = HttpProtocols.Http1AndHttp2;

                var services = new ServiceCollection();
                var log = new Microsoft.Extensions.Logging.Abstractions.NullLoggerFactory();
                services.AddSingleton<ILoggerFactory>(log);

                lisOption.KestrelServerOptions.ApplicationServices = services.BuildServiceProvider();

                if (port == 443 || forcessl)
                {
                    // use https
                    Microsoft.AspNetCore.Server.Kestrel.Https.HttpsConnectionAdapterOptions ssloption = new Microsoft.AspNetCore.Server.Kestrel.Https.HttpsConnectionAdapterOptions();

                    ssloption.ServerCertificateSelector = SelectSsl;
                    ssloption.ClientCertificateMode = Microsoft.AspNetCore.Server.Kestrel.Https.ClientCertificateMode.NoCertificate;
                    ssloption.ClientCertificateValidation = (x, y, z) => { return true; };
                    ssloption.HandshakeTimeout = new TimeSpan(0, 0, 30);

                    ssloption.SslProtocols = System.Security.Authentication.SslProtocols.Tls12 | System.Security.Authentication.SslProtocols.Tls11 | System.Security.Authentication.SslProtocols.Tls;

                    lisOption.UseHttps(ssloption);
                }

            }

            public X509Certificate2 SelectSsl(Microsoft.AspNetCore.Connections.ConnectionContext context, string host)
            {
                return Kooboo.Data.Server.SslCertificateProvider.SelectCertificate2(host);
            }
        }

        public class KoobooHttpApplication : IHttpApplication<RenderContext>
        {
            private IKoobooMiddleWare start { get; set; }

            public KoobooHttpApplication(IKoobooMiddleWare start)
            {
                this.start = start;
            }

            public RenderContext CreateContext(IFeatureCollection contextFeatures)
            {
                var context = GetRenderContext(contextFeatures);
                context.SetItem<IFeatureCollection>(contextFeatures);
                return context;
            }

            public void DisposeContext(RenderContext context, Exception exception)
            {
                context = null;
            }

            public async Task ProcessRequestAsync(RenderContext context)
            {
                // process the context....
                await this.start.Invoke(context);
                await SetResponse(context);
            }

            public static RenderContext GetRenderContext(IFeatureCollection Feature)
            {
                RenderContext context = new RenderContext();
                context.Request = GetRequest(Feature);
                return context;
            }

            private static Context.HttpRequest GetRequest(IFeatureCollection requestFeature)
            {
                Context.HttpRequest httprequest = new Context.HttpRequest();
                var req = requestFeature.Get<IHttpRequestFeature>();

                var connection = requestFeature.Get<IHttpConnectionFeature>();
                httprequest.Port = connection.LocalPort;
                httprequest.IP = connection.RemoteIpAddress.ToString();

                Microsoft.Extensions.Primitives.StringValues forwardip;
                if (req.Headers.TryGetValue("X-Forwarded-For", out forwardip))
                {
                    httprequest.IP = forwardip.First();
                }

                Microsoft.Extensions.Primitives.StringValues host;
                req.Headers.TryGetValue("Host", out host);


                string domainhost = host.First().ToString();
                int delimiterIndex = domainhost.IndexOf(":");
                if (delimiterIndex > 0)
                {
                    domainhost = domainhost.Substring(0, delimiterIndex);
                }

                httprequest.Host = domainhost;


                foreach (var item in req.Headers)
                {
                    httprequest.Headers.Add(item.Key, item.Value);
                }

                httprequest.Path = req.Path;
                httprequest.Url = GetUrl(req.RawTarget);
                httprequest.RawRelativeUrl = httprequest.Url;
                httprequest.Method = req.Method;

                httprequest.Scheme = req.Scheme;


                httprequest.QueryString = GetQueryString(requestFeature);

                httprequest.Cookies = GetCookie(requestFeature);

                if (httprequest.Method != "GET")
                {
                    if (req.Body != null && req.Body.CanRead)
                    {
                        MemoryStream ms = new MemoryStream();
                        var body = req.Body;
                        req.Body.CopyTo(ms);
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

            private static Dictionary<string, string> GetCookie(IFeatureCollection feature)
            {
                Dictionary<string, string> cookies = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

                var cookiefeture = feature.Get<IRequestCookiesFeature>();
                if (cookiefeture == null)
                {
                    cookiefeture = new RequestCookiesFeature(feature);
                }

                if (cookiefeture.Cookies != null && cookiefeture.Cookies.Any())
                {
                    foreach (var item in cookiefeture.Cookies)
                    {
                        if (!cookies.ContainsKey(item.Key))
                        {
                            cookies.Add(item.Key, item.Value);
                        }
                    }
                }
                return cookies;
            }

            private static NameValueCollection GetQueryString(IFeatureCollection feature)
            {
                NameValueCollection query = new NameValueCollection();

                var requestfeature = feature.Get<IQueryFeature>();
                if (requestfeature == null)
                {
                    requestfeature = new QueryFeature(feature);
                }

                if (requestfeature.Query != null && requestfeature.Query.Any())
                {
                    foreach (var item in requestfeature.Query)
                    {
                        query.Add(item.Key, item.Value);
                    }
                }
                return query;
            }


            public static async Task SetResponse(RenderContext renderContext)
            {
                var feature = renderContext.GetItem<IFeatureCollection>();
                var response = renderContext.Response;
                var res = feature.Get<IHttpResponseFeature>();

                var request = feature.Get<IHttpRequestFeature>();

                res.Headers.Add("server", "http://www.kooboo.com");
                res.StatusCode = response.StatusCode;
                res.Headers.Add("Content-Type", response.ContentType);

                foreach (var item in response.Headers)
                {
                    res.Headers[item.Key] = item.Value;
                }

                #region cookie

                if (response.DeletedCookieNames.Any() || response.AppendedCookies.Any())
                {
                    var cookieres = feature.Get<IResponseCookiesFeature>();
                    if (cookieres == null)
                    {
                        cookieres = new ResponseCookiesFeature(feature);
                    }

                    foreach (var item in response.DeletedCookieNames)
                    {
                        var options = new CookieOptions()
                        {
                            Domain = renderContext.Request.Host,
                            Path = "/",
                            Expires = DateTime.Now.AddDays(-30)
                        };

                        cookieres.Cookies.Append(item, "", options);
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

                        cookieres.Cookies.Append(item.Name, item.Value, options);

                    }

                }
                #endregion


                if (response.StatusCode == 200)
                {

                    if (response.Body != null && renderContext.Request.Method != "HEAD")
                    {
                        try
                        {
                            res.Headers["Content-Length"] = response.Body.Length.ToString();
                            await res.Body.WriteAsync(response.Body, 0, response.Body.Length).ConfigureAwait(false);
                        }
                        catch (Exception)
                        {
                            res.Body.Close();
                        }
                    }
                    else
                    {
                        if (response.Stream != null)
                        {
                            await response.Stream.CopyToAsync(res.Body);
                        }

                        else
                        {
                            // 404.   
                            //string filename = Lib.Helper.IOHelper.CombinePath(AppSettings.RootPath, Kooboo.DataConstants.Default404Page) + ".html";
                            //if (System.IO.File.Exists(filename))
                            //{
                            //    string text = System.IO.File.ReadAllText(filename);
                            //    var bytes = System.Text.Encoding.UTF8.GetBytes(text);
                            //    res.Headers["Content-Length"] = bytes.Length.ToString();
                            //    res.StatusCode = 404;
                            //    await res.Body.WriteAsync(bytes, 0, bytes.Length);
                            //}
                        }
                    }
                }
                else
                {
                    string location = response.RedirectLocation;

                    if (!string.IsNullOrEmpty(location))
                    {
                        var host = renderContext.Request.Port == 80 || renderContext.Request.Port == 443
                            ? renderContext.Request.Host
                            : string.Format("{0}:{1}", renderContext.Request.Host, renderContext.Request.Port);
                        string BaseUrl = renderContext.Request.Scheme + "://" + host + renderContext.Request.Path;
                        var newUrl = UrlHelper.Combine(BaseUrl, location);
                        if (response.StatusCode != 200)
                        {
                            res.StatusCode = response.StatusCode;
                        }
                        //status code doesn't start with 3xx,it'will not redirect.
                        if (!response.StatusCode.ToString().StartsWith("3"))
                        {
                            res.StatusCode = StatusCodes.Status302Found;
                        }

                        res.Headers["location"] = newUrl;

                        res.Body.Dispose();

                        Log(renderContext);
                        return;
                    }





                    if (response.Body != null && response.Body.Length > 0)
                    {
                        res.StatusCode = response.StatusCode;
                        await res.Body.WriteAsync(response.Body, 0, response.Body.Length).ConfigureAwait(false);

                        res.Body.Dispose();
                        Log(renderContext);
                        return;
                    }

                    res.StatusCode = response.StatusCode;
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
                    await res.Body.WriteAsync(bytes, 0, bytes.Length).ConfigureAwait(false);
                }


                res.Body.Dispose();
                Log(renderContext);
                res = null;
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
}

#endif
