//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Sites.Render
{
    /// <summary>
    ///  This is used when request the view by itself without putting into a page.
    /// </summary>
    class ViewRenderer
    {
        public async static Task Render(FrontContext context)
        {
          Components.ComponentSetting setting = new Components.ComponentSetting();  
            setting.TagName = "view";
            setting.NameOrId = context.Route.objectId.ToString();

            var ViewComponent = Components.Container.Get("view");
            string viewresult = null;
            if (ViewComponent != null)
            {
                viewresult = await ViewComponent.RenderAsync(context.RenderContext, setting);
            }

            if (!string.IsNullOrEmpty(viewresult))
            { 
                context.RenderContext.Response.Body = Encoding.UTF8.GetBytes(viewresult);
            }
        }
    }
}
