//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.Text;

namespace Kooboo.Sites.Render
{
    public class CodeRenderer
    {
        public static void Render(FrontContext context)
        {
            var start = System.DateTime.Now;

            var code = context.SiteDb.Code.Get(context.Route.objectId);

            if (code != null)
            {
                string result = string.Empty;
                var enableCORS = context?.WebSite?.EnableCORS ?? false;

                if (code.IsJson)
                {
                    result = code.Body;
                }
                else if (context?.RenderContext?.Request?.Method?.ToUpper() == "OPTIONS" && enableCORS)
                {
                    result = "";
                }
                else
                {
                    result = Scripting.Manager.ExecuteCode(context.RenderContext, code.Body, code.Id);
                }

                //context.RenderContext.Response.ContentType = "application/javascript";

                if (code.Cors || enableCORS)
                {
                    var origin = context?.RenderContext?.Request?.Headers?.Get("Origin");
                    var method = context?.RenderContext?.Request?.Headers?.Get("Access-Control-Request-Method");
                    var headers = context?.RenderContext?.Request?.Headers?.Get("Access-Control-Request-Headers");
                    context?.RenderContext?.Response?.Headers?.Add("Access-Control-Allow-Origin", origin ?? "*");
                    context?.RenderContext?.Response?.Headers?.Add("Access-Control-Allow-Methods", method ?? "*");
                    context?.RenderContext?.Response?.Headers?.Add("Access-Control-Allow-Headers", headers ?? "*");
                    context?.RenderContext?.Response?.Headers?.Add("Access-Control-Allow-Credentials", "true");
                }

                if (!string.IsNullOrEmpty(result))
                {
                    context.RenderContext.Response.Body = Encoding.UTF8.GetBytes(result);
                }
            }
            else
            {
                context.RenderContext.Response.StatusCode = 404;
            }

            if (context.WebSite.EnableVisitorLog)
            {
                string detail = "";
                if (context.RenderContext.Request.Body != null)
                {
                    detail = context.RenderContext.Request.Body;
                }

                context.Log.AddEntry("API call", context.RenderContext.Request.RawRelativeUrl, start, System.DateTime.Now, (short)context.RenderContext.Response.StatusCode, detail);

                context.Page = new Models.Page() { Name = "system api page" };
            }
        }
    }
}
