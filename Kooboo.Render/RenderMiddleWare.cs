using Kooboo.Data.Context;
using System.Threading.Tasks;
using Kooboo.Data.Server;
using System.IO;
using System.Collections.Generic; 

namespace Kooboo.Render
{
    public class RenderMiddleWare : IKoobooMiddleWare
    {
        public RenderOption options { get; set; }

        public RenderMiddleWare(RenderOption options)
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
            if (!options.ShouldTryHandle(context, this.options))
            {
                await Next.Invoke(context); return;
            }

            if (this.options.InitData!=null)
            {
                foreach (var item in this.options.InitData)
                {
                    context.DataContext.Push(item.Key, item.Value); 
                }
            }

            Kooboo.Data.Models.User currentUser = null; 

            if (context.User !=null)
            {
                currentUser = context.User; 
            }
            else
            {
                currentUser = new Data.Models.User(); 
            }

            context.DataContext.Push("User", context.User);

            var currentwebsite = context.WebSite;
            if (this.options.RequireSpeicalSite)
            {
                var website = new Kooboo.Data.Models.WebSite();
                website.Name = "_____kooboorendertempspecialsitename";
                context.WebSite = website;
            }

            if (this.options.RequireUser && !IsIgnorePath(context.Request.RelativeUrl))
            {
                var user = context.User;
                if (user == null)
                {
                    string relative = context.Request.RelativeUrl;

                    if (!string.IsNullOrEmpty(this.options.LoginPage))
                    {
                        int index = relative.IndexOf("&accesstoken"); 
                        if (index > -1)
                        {
                            relative = relative.Substring(0, index); 
                        }
                        
                        Dictionary<string, string> returnurl = new Dictionary<string, string>();
                        returnurl.Add("returnurl", System.Net.WebUtility.UrlEncode(relative));
                        string fullurl = Kooboo.Lib.Helper.UrlHelper.AppendQueryString(this.options.LoginPage, returnurl);
                        context.Response.Redirect(503, fullurl);
                        return;
                    }
                    else
                    {
                        context.Response.StatusCode = 503;
                        return;
                    }

                }
            }

            if (!string.IsNullOrEmpty(this.options.LoginPage) && context.Request.RelativeUrl.StartsWith(this.options.LoginPage, System.StringComparison.OrdinalIgnoreCase))
            {
                if (context.User != null)
                {
                    string afterlogin = Kooboo.Sites.Service.StartService.AfterLoginPage(context); 
                    context.Response.Redirect(302, afterlogin);
                    return;
                }
            }

            // only for this render task. 
           // context.Request.RelativeUrl = RenderHelper.GetRelativeUrl(context.Request.RawRelativeUrl, options);
            
            // var Response = RenderEngine.Render(siteContext);

            if ((context.User !=null) && !string.IsNullOrWhiteSpace(context.User.Language))
            {
                context.Culture = context.User.Language; 
            }

            var Response = RenderEngine.Render(context, this.options, RenderHelper.GetRelativeUrl(context.Request.RawRelativeUrl, options));

            if (Response != null)
            {  
                if (this.options.Log !=null)
                {
                    this.options.Log(context, Response); 
                }

                context.Response.ContentType = Response.ContentType;

                context.Response.StatusCode = 200;
                if (Response.Stream != null)
                {
                    context.Response.Stream = Response.Stream; 
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

            //  context.Request.RelativeUrl = null; //restore change if any.

            if (this.options.RequireSpeicalSite)
            {
                context.WebSite = currentwebsite;  // restore...
            } 
            await Next.Invoke(context);
        }
    }
}
