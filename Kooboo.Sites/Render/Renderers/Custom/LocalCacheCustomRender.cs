using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Sites.Render.Renderers.Custom
{
    public class LocalCacheCustomRender : ICustomRender
    {
        public string Name => "LocalCache";

        public static string GetRoute<T>(string parameter) where T : ICustomRender
        {
            return "/__kb/custom/" + typeof(T).Name + "/" + parameter;
        }

        public async Task RenderAsync(FrontContext context, string parameter)
        {
            var bytes = await LocalCache.LocalCacheManager.GetItem(context.WebSite, parameter);
            if (bytes != null)
            {
                var CmsFile = new Kooboo.Sites.Models.CmsFile();
                CmsFile.ContentBytes = bytes;
                CmsFile.Name = parameter; 
                await Kooboo.Sites.Render.FileRenderer.RenderFile(context, CmsFile); 
            }  
        }

        public static string GetRoute(string parameter)
        {
            return CustomRenderManager.GetRoute<LocalCacheCustomRender>(parameter);
        }
    }
}
