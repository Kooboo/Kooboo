//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Data.Context;
using Kooboo.Data.Server;
using Kooboo.Render;
using System.IO;
using System.Threading.Tasks;

namespace Kooboo.Web.Frontend
{
    public class DefaultStartMiddleWare : IKoobooMiddleWare
    {
        private RenderOption options;

        public DefaultStartMiddleWare(RenderOption options)
        {
            this.options = options;
        }

        public IKoobooMiddleWare Next
        {
            get; set;
        }

        private bool IsIgnorePath(string relativeurl)
        {
            string relative = relativeurl.ToLower();
            foreach (var item in this.options.RequireUserIgnorePath)
            {
                if (relative.StartsWith(item))
                {
                    return true;
                }
            }
            return false;
        }

        public async Task Invoke(RenderContext context)
        {
            if (context.Request.RelativeUrl == "/" || string.IsNullOrEmpty(context.Request.RelativeUrl))
            {
                string relative = this.options.LoginPage;

                if (context.User != null && !string.IsNullOrWhiteSpace(this.options.PageAfterLogin))
                {
                    relative = this.options.PageAfterLogin;
                }

                if (relative.ToLower().StartsWith(this.options.StartPath))
                {
                    relative = relative.Substring(this.options.StartPath.Length);
                }

                var response = RenderEngine.Render(context, this.options, relative);

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
            else
            {
                await Next.Invoke(context); return;
            }
        }
    }
}