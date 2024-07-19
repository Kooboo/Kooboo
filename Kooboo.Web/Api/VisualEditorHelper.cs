using System.IO;
using Kooboo.Api;
using Kooboo.Sites.Helper;

namespace Kooboo.Web.Api
{
    internal class VisualEditorHelper
    {
        internal static string GetInjects(ApiCall call)
        {
            var isDev = call.GetValue("env") == "development";
            var baseUrl = call.Context.Request.Headers.Get("Referer");
            var unoCss = UnoCssHelper.GetUnoCss(call.WebSite);

            if (isDev)
            {
                var url = $"/_Admin/src/components/visual-editor/inject.ts?t={DateTime.Now.ToUnixEpochTicks()}";
                return $"{unoCss}<script type=\"module\" src=\"{ToAbsoluteUrl(url, baseUrl)}\"></script>";
            }

            var target = Path.Combine(Environment.CurrentDirectory, "_Admin/page-editor-inject.html");
            if (!File.Exists(target))
            {
                return unoCss;
            }
            return unoCss + File.ReadAllText(target);
        }

        private static string ToAbsoluteUrl(string url, string baseUrl)
        {
            if (!string.IsNullOrEmpty(baseUrl))
            {
                return new Uri(new Uri(baseUrl), url).AbsoluteUri;
            }

            return url;
        }
    }
}
