//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Data.Context;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Render;

namespace Kooboo.Sites.Scripting.Global
{
    public class Response
    {
        private RenderContext Context { get; set; }

        public Response(RenderContext context)
        {
            this.Context = context;
        }

        public void write(object value)
        {
            if (value == null)
            {
                return;
            }
            string output = ToJson(value);

            var item = this.Context.GetItem<string>(Constants.OutputName);
            if (item == null)
            {
                this.Context.SetItem<string>(output, Constants.OutputName);
            }
            else
            {
                item += output;
                this.Context.SetItem<string>(item, Constants.OutputName);
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
                output = Lib.Helper.JsonHelper.SerializeCaseSensitive(value, new Kooboo.Lib.Helper.IntJsonConvert());
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
            this.Context.Response.Headers[key] = value;
        }

        public void redirect(string url)
        {
            this.Context.Response.Redirect(302, url);
        }

        public void Json(object value)
        {
            // write method default is Json already...
            write(value);
        }

        public void binary(string contentType, byte[] bytes)
        {
            this.Context.Response.ContentType = contentType;
            this.Context.Response.Body = bytes;
        }

        public void StatusCode(int code)
        {
            this.Context.Response.StatusCode = code;
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
                value = curl.get(url);
            }
            else
            {
                var route = Kooboo.Sites.Routing.ObjectRoute.GetRoute(Context.WebSite.SiteDb(), url);
                if (route != null)
                {
                    RenderContext newcontext = new RenderContext();
                    newcontext.Request = Context.Request;
                    newcontext.User = Context.User;
                    newcontext.WebSite = Context.WebSite;
                    newcontext.Culture = Context.Culture;

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

            var item = this.Context.GetItem<string>(Constants.OutputName);
            if (item == null)
            {
                this.Context.SetItem<string>(value, Constants.OutputName);
            }
            else
            {
                item += value;
                this.Context.SetItem<string>(item, Constants.OutputName);
            }
        }
    }
}