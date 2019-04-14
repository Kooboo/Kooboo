using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kooboo.Api;
using Kooboo.Data.Context;
using Kooboo.Model.Render;
namespace Kooboo.Model
{
    public class KoobooModelManager
    {
        public static string Render(string html, ModelRenderContext context)
        {
            if (!IsNeedRender(context.HttpContext))
                return html;

            html = new ViewParser(ViewParseOptions.Instance).RenderRootView(html, context);
            
            return html;
        }
        private static bool IsNeedRender(RenderContext context)
        {
            var url = context.Request.Path;
            return url.StartsWith("/_Admin/Vue", StringComparison.OrdinalIgnoreCase) ||
                url.StartsWith("/_Admin/account/Vue", StringComparison.OrdinalIgnoreCase);
           
        }
    }
}
