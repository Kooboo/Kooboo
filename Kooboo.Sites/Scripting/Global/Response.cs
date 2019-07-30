//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Context; 
using Kooboo.Sites.Render;
using Kooboo.Sites.Extensions;

namespace Kooboo.Sites.Scripting.Global
{
    public class Response
    {
        private RenderContext context { get; set; }
        public Response(RenderContext context)
        {
            this.context = context;
        }

        public void write(object value)
        {
            if (value == null)
            {
                return;
            }
            string output = ToJson(value); 

            var item = this.context.GetItem<string>(Constants.OutputName);
            if (item == null)
            {
                this.context.SetItem<string>(output, Constants.OutputName);
            }
            else
            {
                item += output;
                this.context.SetItem<string>(item, Constants.OutputName);
            }
        }

        public string ToJson(object value)
        {
            string output;
            if (!(value is string) && value.GetType().IsClass)
            {
                //if (value is Kooboo.Data.Interface.IDynamic)
                //{
                //    var dynamic = value as Kooboo.Data.Interface.IDynamic;
                //    output = Lib.Helper.JsonHelper.SerializeCaseSensitive(dynamic.Values);
                //}
                //else if (IsDynamicArray(value))
                //{
                //    var dynamicValues = value as ICollection;

                //    List<Dictionary<string, object>> result = new List<Dictionary<string, object>>();
                //    foreach (var v in dynamicValues)
                //    {
                //        var dynamicvalue = v as Kooboo.Data.Interface.IDynamic;
                //        if (dynamicvalue != null)
                //        {
                //            result.Add(dynamicvalue.Values);
                //        }
                //    }
                //    output = Lib.Helper.JsonHelper.SerializeCaseSensitive(result);
                //}
                //else
                //{
                    output = Lib.Helper.JsonHelper.SerializeCaseSensitive(value);
                //}
            }
            else
            {
                output = value.ToString();
            } 
            return output;
        }

        public void setHeader(string key, string value)
        {
            this.context.Response.Headers[key] = value;
        }

        public void redirect(string url)
        {
            this.context.Response.Redirect(302, url);
        } 

        public void Json(object value)
        {
            // write method default is Json already...
            write(value);
        }

        public void binary(string contentType,byte[] bytes)
        {
            this.context.Response.ContentType = contentType;
            this.context.Response.Body = bytes;
        }

        public void StatusCode(int code)
        {
            this.context.Response.StatusCode = code; 
        }
         
        public void Execute(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                return; 
            }

            string value = null; 

            if (url.ToLower().StartsWith("https://") || url.ToLower().StartsWith("http://"))
            {
                Curl curl = new Curl();
                value =   curl.get(url); 
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
                     
                    FrontContext kooboocontext = new FrontContext();
                    newcontext.SetItem<FrontContext>(kooboocontext);
                    kooboocontext.RenderContext = newcontext;

                    kooboocontext.Route = route;

                    Kooboo.Sites.Render.RouteRenderers.RenderAsync(kooboocontext);

                    if (newcontext.Response.Body != null && newcontext.Response.Body.Length > 0)
                    {
                        value = System.Text.Encoding.UTF8.GetString(newcontext.Response.Body);
                    }
                }
            } 


            var item = this.context.GetItem<string>(Constants.OutputName);
            if (item == null)
            {
                this.context.SetItem<string>(value, Constants.OutputName);
            }
            else
            {
                item += value;
                this.context.SetItem<string>(item, Constants.OutputName);
            }
        }
    }
}
