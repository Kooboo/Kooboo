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
                    var requestHeaders = context?.RenderContext?.Request?.Headers;
                    var origin = requestHeaders?.Get("Origin");
                    var method = requestHeaders?.Get("Access-Control-Request-Method");
                    var headers = requestHeaders?.Get("Access-Control-Request-Headers");
                    var responseHeaders = context?.RenderContext?.Response?.Headers;
                    if (!responseHeaders.ContainsKey("Access-Control-Allow-Origin")) responseHeaders?.Add("Access-Control-Allow-Origin", origin ?? "*");
                    if (!responseHeaders.ContainsKey("Access-Control-Allow-Methods")) responseHeaders?.Add("Access-Control-Allow-Methods", method ?? "*");
                    if (!responseHeaders.ContainsKey("Access-Control-Allow-Headers")) responseHeaders?.Add("Access-Control-Allow-Headers", headers ?? "*");
                    if (!responseHeaders.ContainsKey("Access-Control-Allow-Credentials")) responseHeaders?.Add("Access-Control-Allow-Credentials", "true");
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
