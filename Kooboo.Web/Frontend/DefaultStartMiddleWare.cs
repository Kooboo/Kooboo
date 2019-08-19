//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Context;
using Kooboo.Data.Server;
using Kooboo.Render;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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

                if (context.User !=null && !string.IsNullOrWhiteSpace(this.options.PageAfterLogin))
                {
                    relative = this.options.PageAfterLogin; 
                }

                if (relative.ToLower().StartsWith(this.options.StartPath))
                {
                    relative = relative.Substring(this.options.StartPath.Length); 
                }

                var Response = RenderEngine.Render(context, this.options,  relative);

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
            else
            {
                await Next.Invoke(context); return;
            }
              
        

        }
    }

}


