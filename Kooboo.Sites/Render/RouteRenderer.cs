//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.Threading.Tasks;

namespace Kooboo.Sites.Render
{
    public static class RouteRenderers
    {
        public static async Task RenderAsync(FrontContext context)
        {
            try
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
                           await FileRenderer.RenderAsync(context);
                            break;
                        }
                    case ConstObjectType.Image:
                        {
                           await ImageRenderer.RenderAsync(context);
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
                            await ViewRenderer.Render(context);
                            break;
                        }
                    case ConstObjectType.KoobooSystem:
                        {
                            await Systems.SystemRender.Render(context);
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
            catch (System.Exception ex)
            {
                Kooboo.Data.Log.Instance.Exception.Write(ex.Message + " " + ex.Source + ex.StackTrace);

                context.RenderContext.Response.AppendString(ex.Message+ " " + ex.Source);
      
            }

        }
    }
}
