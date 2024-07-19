//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.IO;
using System.Threading.Tasks;
using Kooboo.Data;
using Kooboo.Data.Context;

namespace Kooboo.Web.Spa
{
    public class SpaMiddleWare : IKoobooMiddleWare
    {
        private SpaRenderOption options;

        public SpaMiddleWare(SpaRenderOption options)
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

            if (this.options.InitData != null)
            {
                foreach (var item in this.options.InitData)
                {
                    context.DataContext.Push(item.Key, item.Value);
                }
            }

            if (string.IsNullOrWhiteSpace(context.Culture))
            {
                context.Culture = context.Request.GetValue("lang");
            }

            var Response = RenderEngine.Render(context, this.options, RenderHelper.GetRelativeUrl(context.Request.RawRelativeUrl, options));

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
