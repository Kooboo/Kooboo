//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using System.Text;

namespace Kooboo.Sites.Render
{
    /// <summary>
    ///  This is used when request the view by itself without putting into a page.
    /// </summary>
    internal class ViewRenderer
    {
        public async static void Render(FrontContext context)
        {
            Components.ComponentSetting setting = new Components.ComponentSetting
            {
                TagName = "view", NameOrId = context.Route.objectId.ToString()
            };

            var viewComponent = Components.Container.Get("view");
            string viewresult = null;
            if (viewComponent != null)
            {
                viewresult = await viewComponent.RenderAsync(context.RenderContext, setting);
            }

            if (!string.IsNullOrEmpty(viewresult))
            {
                context.RenderContext.Response.Body = Encoding.UTF8.GetBytes(viewresult);
            }
        }
    }
}