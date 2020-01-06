using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Render
{
    public static class TextBodyRender
    {
        public static void SetBody(FrontContext context, string body)
        {
            if (body == null)
            {
                return;
            }

            var bytes = Encoding.UTF8.GetBytes(body);

            if (context.RenderContext.EnableTextGZip)
            {
                bool gzipSupported = false;
                var acceptEncoding = context.RenderContext.Request.Headers.Get("Accept-Encoding");
                if (acceptEncoding != null)
                {
                    gzipSupported = acceptEncoding.Contains("gzip") || acceptEncoding.Contains("deflate");
                }

                if (gzipSupported)
                {
                    byte[] gzipBytes;
                    using (var stream = new System.IO.MemoryStream())
                    {
                        using (System.IO.Compression.GZipStream gZipStream = new System.IO.Compression.GZipStream(stream, System.IO.Compression.CompressionLevel.Fastest))
                        {
                            gZipStream.Write(bytes, 0, bytes.Length);
                        }
                        gzipBytes = stream.ToArray();
                    }
                    context.RenderContext.Response.Body = gzipBytes;
                    context.RenderContext.Response.Headers.Add("Content-encoding", "gzip");

                    return;
                }
            } 

            context.RenderContext.Response.Body = bytes;  
        }
    }
}
