//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.IO;
using System.Threading.Tasks;
using Kooboo.Data;
using Kooboo.Data.Context;
using Kooboo.Render;
using Kooboo.Sites.Helper;
using Kooboo.Sites.Service;

namespace Kooboo.Web.Frontend
{

    public class DefaultStartMiddleWare : IKoobooMiddleWare
    {
        private readonly RenderOption _options;

        public DefaultStartMiddleWare(RenderOption options)
        {
            _options = options;
        }

        public IKoobooMiddleWare Next { get; set; }

        public async Task Invoke(RenderContext context)
        {
            var relative = context.Request.RelativeUrl;

            if (context.Request.RelativeUrl == "/" || string.IsNullOrEmpty(context.Request.RelativeUrl))
            {
                relative = _options.StartPath;
            }

            if (relative.StartsWith(_options.StartPath, System.StringComparison.CurrentCultureIgnoreCase))
            {
                CorsHelper.HandleHeaders(context);
                //Temp fix. 
                //string host = context.Request.Host.ToLower();
                //if (host == "kooboo.cn" || host == "www.kooboo.cn" || host == "www.kooboo.com" || host == "kooboo.com" || host == "kooboo.eu" || host == "www.kooboo.eu")
                //{
                //    await Next.Invoke(context);
                //    return;
                //}
                if (relative.Equals(_options.StartPath, System.StringComparison.CurrentCultureIgnoreCase))
                {
                    relative += "/index.html";
                }
                else if (relative.Equals(_options.StartPath + '/', System.StringComparison.CurrentCultureIgnoreCase))
                {
                    relative += "index.html";
                }

                if (relative.StartsWith(_options.StartPath, System.StringComparison.CurrentCultureIgnoreCase))
                {
                    relative = relative[_options.StartPath.Length..];
                }

                if (relative.Trim('/').StartsWith("__cover?"))
                {
                    var cover = SiteCoverService.GetSiteCover(context);

                    if (cover == null)
                    {
                        relative = "/site_default_cover.svg";
                    }
                    else
                    {
                        context.Response.ContentType = "image/png";
                        context.Response.StatusCode = 200;
                        context.Response.Body = cover;
                        return;
                    }
                }

                var Response = RenderEngine.Render(context, this._options, relative);

                if (Response.ContentType == "text/html" && Response.Body == null && Response.Stream == null && Response.BinaryBytes == null)
                {
                    relative = _options.StartPath + "/index.html";
                    Response = RenderEngine.Render(context, _options, relative);
                }

                if (Response != null)
                {
                    context.Response.ContentType = Response.ContentType;

                    if (relative.StartsWith("/assets"))
                    {
                        context.Response.Headers["Expires"] = DateTime.UtcNow.AddDays(388).ToString("r");
                    }

                    context.Response.StatusCode = 200;

                    byte[] ReseultBytes = null;
                    if (Response.Stream != null)
                    {
                        MemoryStream memory = new MemoryStream();
                        Response.Stream.CopyTo(memory);
                        ReseultBytes = memory.ToArray();

                    }
                    else if (Response.BinaryBytes != null)
                    {
                        ReseultBytes = Response.BinaryBytes;
                    }
                    else if (!string.IsNullOrEmpty(Response.Body))
                    {
                        ReseultBytes = System.Text.Encoding.UTF8.GetBytes(Response.Body);
                    }

                    if (ReseultBytes != null)
                    {

                        //Check if client support GZIP. 
                        bool gzipSupported = false;
                        var acceptEncoding = context.Request.Headers.Get("Accept-Encoding");
                        if (acceptEncoding != null)
                        {
                            gzipSupported = acceptEncoding.Contains("gzip") || acceptEncoding.Contains("deflate");
                        }

                        if (!gzipSupported)
                        {
                            context.Response.Body = ReseultBytes;
                            return;
                        }
                        else
                        {
                            // handle GZIP. 
                            byte[] gzipBytes;
                            using (var stream = new System.IO.MemoryStream())
                            {
                                using (System.IO.Compression.GZipStream gZipStream = new System.IO.Compression.GZipStream(stream, System.IO.Compression.CompressionLevel.Fastest))
                                {
                                    gZipStream.Write(ReseultBytes, 0, ReseultBytes.Length);
                                }
                                gzipBytes = stream.ToArray();
                            }
                            context.Response.Body = gzipBytes;
                            context.Response.Headers.Add("Content-encoding", "gzip");

                            return;
                        }

                    }
                }

                await Next.Invoke(context);
            }
            else
            {
                await Next.Invoke(context); return;
            }
        }
    }

}


