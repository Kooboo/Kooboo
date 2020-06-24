//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Context;
using System.Threading.Tasks;
using Kooboo.Data.Server; 
using System.Collections.Generic;
using Kooboo.Render.Response;

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

            if (this.options.InitData != null)
            {
                foreach (var item in this.options.InitData)
                {
                    context.DataContext.Push(item.Key, item.Value);
                }
            }

            Kooboo.Data.Models.User currentUser = null;

            if (context.User != null)
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
                context.WebSite = null; 

                //var website = new Kooboo.Data.Models.WebSite();
                //website.Name = "_____kooboorendertempspecialsitename";
                //context.WebSite = website;
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
                        returnurl.Add("returnurl", relative);
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

            if (context.Response.End)
            {
                return;
            }

            if (!string.IsNullOrEmpty(this.options.LoginPage) && context.Request.RelativeUrl.StartsWith(this.options.LoginPage, System.StringComparison.OrdinalIgnoreCase))
            {
                if (context.User != null)
                {
                    string afterlogin = Kooboo.Data.Service.StartService.AfterLoginPage(context);
                    context.Response.Redirect(302, afterlogin);
                    return;
                }
            }

            if ((context.User != null) && !string.IsNullOrWhiteSpace(context.User.Language))
            {
                context.Culture = context.User.Language;
            }

            var Response = RenderEngine.Render(context, this.options, RenderHelper.GetRelativeUrl(context.Request.RawRelativeUrl, options));

            // Set System version. 
            context.Response.AppendCookie("_system_version_", Data.AppSettings.Version.ToString());  
            if (Response != null)
            {
                if (this.options.Log != null)
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

            else
            {
                // try render controller... 
                if (this.options.Render != null)
                {
                    string relativeurl = RenderHelper.GetRelativeUrl(context.Request.RawRelativeUrl, options);
                    var resposne = this.options.Render(context, relativeurl);
                    if (resposne != null)
                    {
                        SetResponse(context, resposne);
                        return;
                    }
                } 
            }

            // Render controller here...
            //  context.Request.RelativeUrl = null; //restore change if any.
            if (this.options.RequireSpeicalSite)
            {
                context.WebSite = currentwebsite;  // restore...
            }
            await Next.Invoke(context);
        }

        public void SetResponse(RenderContext context, ResponseBase response)
        {
            foreach (var item in response.Headers)
            {
                context.Response.Headers.Add(item.Key, string.Join(null, item.Value));
            }

            foreach (var item in response.DeletedCookieNames)
            {
                context.Response.DeleteCookie(item);
            }
            foreach (var item in response.AppendedCookies)
            {
                context.Response.AddCookie(item);
            }
            response.DeletedCookieNames.Clear();
            response.AppendedCookies.Clear();
            response.Headers.Clear();

            context.Response.StatusCode = response.StatusCode;
            context.Response.ContentType = response.ContentType;
            if (string.IsNullOrWhiteSpace(context.Response.ContentType))
            {
                context.Response.ContentType = "text/html";
            }

            if (response is RedirectResponse)
            {
                var redirect = response as RedirectResponse;
                if (!string.IsNullOrWhiteSpace(redirect.RedirectUrl))
                {
                    context.Response.Redirect(302, redirect.RedirectUrl);
                    return;
                }
            }
             
            if (response is BinaryResponse)
            {
                var binary = response as BinaryResponse;
                context.Response.Body = binary.BinaryBytes;
            }
            else if (response is StringResponse)
            {
                var strres = response as StringResponse;
                context.Response.Body = System.Text.Encoding.UTF8.GetBytes(strres.Content);
                // do nothing. 
            }

        }
         
    }
}
