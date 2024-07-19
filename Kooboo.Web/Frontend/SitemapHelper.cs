using System.Text;
using Kooboo.Data.Context;
using Kooboo.Data.Sitemap;
using Kooboo.Data.Typescript;
using Kooboo.Sites;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Helper;
using Kooboo.Sites.Scripting;
using Kooboo.Sites.Service;

namespace Kooboo.Web.Frontend
{
    public static class SitemapHelper
    {
        public static void HandleSitemap(RenderContext context, SitemapSettings settings)
        {
            CorsHelper.HandleHeaders(context);


            if (settings.AutoGenerate)
            {
                var result = AutoGenerateSitemap(context);
                context.Response.ContentType = "text/xml";
                context.Response.Body = Encoding.UTF8.GetBytes(result.ToString());
            }
            else
            {
                var engine = Manager.GetJsEngine(context);

                try
                {
                    var cache = TypescriptCache.Instance.GetOrCreate(
                        context.WebSite.Id,
                        context.WebSite.Id,
                        settings.Code,
                        "sitemap");

                    var exports = engine.ExecuteCode(cache, Data.ScriptType.Module);
                    var result = exports.Get("default").ToObject();
                    context.Response.ContentType = "text/xml";
                    context.Response.Body = Encoding.UTF8.GetBytes(result.ToString());
                }
                catch (System.Exception ex)
                {
                    context.Response.ContentType = "text/html";
                    context.Response.Body = Encoding.UTF8.GetBytes(ex.Message);
                }
            }
        }

        private static string AutoGenerateSitemap(RenderContext context)
        {
            var sitemapBuilder = new SitemapBuilder();
            var siteDb = context.WebSite.SiteDb();
            var pages = siteDb.Pages.All();

            foreach (var page in pages)
            {
                var url = PageService.GetPreviewUrl(siteDb, page);
                sitemapBuilder.Append(url, page.LastModified, SitemapBuilder.Changefreq.always, 1);
            }

            return sitemapBuilder.Build();
        }
    }
}