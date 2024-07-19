//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.IO;
using System.Threading.Tasks;
using Kooboo.Data;
using Kooboo.Data.Context;

namespace Kooboo.Web.JsTest
{
    public class JsTestMiddleWare : IKoobooMiddleWare
    {
        private JsTestOption options;

        public JsTestMiddleWare(JsTestOption options)
        {
            this.options = options;
        }

        public IKoobooMiddleWare Next
        {
            get; set;
        }

        public async Task Invoke(RenderContext context)
        {
            if (!options.ShouldTryHandle(context, this.options))
            {
                await Next.Invoke(context); return;
            }

            var Response = RenderEngine.Render(context, this.options);

            if (Response != null)
            {
                context.Response.ContentType = Response.ContentType;

                context.Response.StatusCode = 200;
                if (Response.Stream != null)
                {
                    MemoryStream memory = new MemoryStream();
                    Response.Stream.CopyTo(memory);
                    context.Response.Body = memory.ToArray();
                    return;
                }
                else if (Response.BinaryBytes != null)
                {
                    context.Response.Body = Response.BinaryBytes;
                    return;
                }
                else if (!string.IsNullOrEmpty(Response.Body))
                {
                    context.Response.Body = System.Text.Encoding.UTF8.GetBytes(Response.Body);
                    return;
                }
            }

            await Next.Invoke(context);
        }
    }
}
