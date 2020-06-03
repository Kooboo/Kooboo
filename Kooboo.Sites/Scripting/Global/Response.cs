//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Context;
using Kooboo.Sites.Render;
using Kooboo.Sites.Extensions;
using System.ComponentModel;
using Kooboo.Sites.Scripting.Global;
using Kooboo.Sites.Service;
using System;

namespace KScript
{
    public class Response
    {
        private RenderContext context { get; set; }
        public Response(RenderContext context)
        {
            this.context = context;
        }
        [Description(@"Print on output page. Will convert Non-Value type to Json format
      k.response.write(""hello world"");
        k.response.write(1234);
        var obj = {name: ""myname""};
    k.response.write(obj);")]
        public void write(object value)
        {
            if (value == null)
            {
                return;
            }
            string output = ToJson(value);

            var item = this.context.GetItem<string>(Kooboo.Sites.Scripting.Constants.OutputName);
            if (item == null)
            {
                this.context.SetItem<string>(output, Kooboo.Sites.Scripting.Constants.OutputName);
            }
            else
            {
                item += output;
                this.context.SetItem<string>(item, Kooboo.Sites.Scripting.Constants.OutputName);
            }
        }

        private string ToJson(object value)
        {
            string output;
            if (!(value is string) && value.GetType().IsClass)
            {
                output = Kooboo.Lib.Helper.JsonHelper.SerializeCaseSensitive(value, new Kooboo.Lib.Helper.IntJsonConvert());
            }
            else
            {
                output = value.ToString();
            }
            return output;
        }

        [Description(@"set header value on output html page.
k.response.setHeader(""ServerTwo"", ""powerful kooboo server"");
        k.response.setHeader(""Access-Control-Allow-Origin"", ""*""")]
        public void setHeader(string key, string value)
        {
            this.context.Response.Headers[key] = value;
        }

        [Description(@"Redirect user to another url, url can be relative or absolute
        k.response.redirect(""/relativepath""); 
        k.response.redirect(""http://www.kooboo.com"");
        k.response.statusCode(301); 
")]
        public void redirect(string url)
        {
            this.context.Response.Redirect(302, url);
        }

        [Description(@"Print object in Json format
 var obj = {fieldone:""valueone"", fieldtwo:""valuetwo""};
        k.response.json(obj);
")]
        public void Json(object value)
        {
            // write method default is Json already...
            write(value);
        }


        public void RenderView(string ViewBody)
        { 
            var options = RenderOptionHelper.GetViewOption(context, default(Guid));

            var renderplan = RenderEvaluator.Evaluate(ViewBody, options);


            var result = RenderHelper.Render(renderplan, this.context);

            if (!string.IsNullOrWhiteSpace(result))
            {
                write(result);
            } 
        }



        public void binary(string contentType, byte[] bytes, string filename = null)
        {
            if (!string.IsNullOrWhiteSpace(filename))
            {
                this.context.Response.Headers.Add("Content-Disposition", $"attachment;filename=" + filename);
            }
            this.context.Response.ContentType = contentType;
            this.context.Response.Body = bytes;
        }

        [Description(@"Set the status code
  k.response.statusCode(301);")]
        public void StatusCode(int code)
        {
            this.context.Response.StatusCode = code;
            this.context.SetItem<CustomStatusCode>(new CustomStatusCode() { IsCustomSet = true, Code = code });
        }

        [Description(@"Excute another Url, and write the response within current context
 k.response.execute(""/anotherpage"");")]
        public void Execute(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                return;
            }

            string value = null;

            if (url.ToLower().StartsWith("https://") || url.ToLower().StartsWith("http://"))
            {
                Curl curl = new Curl(context);
                value = curl.get(url);
            }

            else
            {
                var route = Kooboo.Sites.Routing.ObjectRoute.GetRoute(context.WebSite.SiteDb(), url);
                if (route != null)
                {
                    RenderContext newcontext = new RenderContext();
                    newcontext.Request = context.Request;
                    newcontext.User = context.User;
                    newcontext.WebSite = context.WebSite;
                    newcontext.Culture = context.Culture;

                    FrontContext kooboocontext = new FrontContext(newcontext);
                 
                    kooboocontext.Route = route;

                    Kooboo.Sites.Render.RouteRenderers.RenderAsync(kooboocontext);

                    if (newcontext.Response.Body != null && newcontext.Response.Body.Length > 0)
                    {
                        value = System.Text.Encoding.UTF8.GetString(newcontext.Response.Body);
                    }
                }
            }


            var item = this.context.GetItem<string>(Kooboo.Sites.Scripting.Constants.OutputName);
            if (item == null)
            {
                this.context.SetItem<string>(value, Kooboo.Sites.Scripting.Constants.OutputName);
            }
            else
            {
                item += value;
                this.context.SetItem<string>(item, Kooboo.Sites.Scripting.Constants.OutputName);
            }
        }
    }
}
