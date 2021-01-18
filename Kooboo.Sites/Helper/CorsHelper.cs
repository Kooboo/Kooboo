using Kooboo.Sites.Render;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Helper
{
    public static class CorsHelper
    {
        public static void HandleHeaders(FrontContext context)
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

        public static bool IsOptionsRequest(FrontContext context)
        {
            return context?.RenderContext?.Request?.Method?.ToUpper() == "OPTIONS";
        }
    }
}
