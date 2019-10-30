//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using System.Text;

namespace Kooboo.Sites.Render
{
    public class ScriptRenderer
    {
        public static void Render(FrontContext context)
        {
            var script = context.SiteDb.Scripts.Get(context.Route.objectId);
            context.RenderContext.Response.ContentType = "application/javascript;charset=utf-8";

            if (script?.Body != null)
            {
                if (script.Extension == null || script.Extension == "js" || script.Extension == ".js")
                {
                    context.RenderContext.Response.Body = Encoding.UTF8.GetBytes(script.Body);
                }
                else
                {
                    var engines = Kooboo.Sites.Engine.Manager.GetScript();

                    var find = engines.Find(o => o.Extension == script.Extension);
                    if (find != null)
                    {
                        var code = find.Execute(context.RenderContext, script.Body);
                        context.RenderContext.Response.Body = Encoding.UTF8.GetBytes(code);
                    }
                    else
                    {
                        context.RenderContext.Response.Body = Encoding.UTF8.GetBytes(script.Body);
                    }
                }
            }
        }
    }
}