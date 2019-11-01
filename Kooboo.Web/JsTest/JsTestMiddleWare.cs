//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Data.Context;
using Kooboo.Data.Server;
using System.IO;
using System.Threading.Tasks;

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

            var response = RenderEngine.Render(context, this.options);

            if (response != null)
            {
                context.Response.ContentType = response.ContentType;

                context.Response.StatusCode = 200;
                if (response.Stream != null)
                {
                    MemoryStream memory = new MemoryStream();
                    response.Stream.CopyTo(memory);
                    context.Response.Body = memory.ToArray();
                    return;
                }
                else if (response.BinaryBytes != null)
                {
                    context.Response.Body = response.BinaryBytes;
                    return;
                }
                else if (!string.IsNullOrEmpty(response.Body))
                {
                    context.Response.Body = System.Text.Encoding.UTF8.GetBytes(response.Body);
                    return;
                }
            }

            await Next.Invoke(context);
        }
    }
}