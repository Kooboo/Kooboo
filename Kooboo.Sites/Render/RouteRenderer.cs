//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.Threading.Tasks;

namespace Kooboo.Sites.Render
{
    public static class RouteRenderers
    {
        public static async Task RenderAsync(FrontContext context)
        {
            if (context.RenderContext.Response.End)
            {
                return;
            }

            switch (context.Route.DestinationConstType)
            {
                case ConstObjectType.Page:
                    {
                        await PageRenderer.RenderAsync(context);
                        break;
                    }
                case ConstObjectType.CmsFile:
                    {
                        FileRenderer.Render(context);
                        break;
                    }
                case ConstObjectType.Image:
                    {
                        ImageRenderer.Render(context);
                        break;
                    }
                case ConstObjectType.Script:
                    {
                        ScriptRenderer.Render(context);
                        break;
                    }
 
                case ConstObjectType.Style:
                    {
                        StyleRenderer.Render(context);
                        break;
                    }
                case ConstObjectType.View:
                    {
                        ViewRenderer.Render(context);
                        break;
                    }
                case ConstObjectType.KoobooSystem:
                    {
                        Systems.SystemRender.Render(context);
                        break;
                    }
                case ConstObjectType.ResourceGroup:
                    {
                        Systems.SystemRender.ResourceGroupRender(context, context.Route.objectId.ToString());
                        break;
                    }
                case ConstObjectType.Code: 
                    {
                        CodeRenderer.Render(context);
                        break; 
                    }
                default:
                    break;
            }
        }
    }
}
