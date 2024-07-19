using Kooboo.Data.Context;
using Kooboo.Sites.FrontEvent.Rendering;
using System;

namespace Kooboo.Web.Lighthouse.CssMinimizer
{
    public class StyleRenderingEvent : ISiteRenderingEvent
    {

        public EnumRenderingEventType EventType => EnumRenderingEventType.StyleRendering;

        public int Priority => 1;

        public void Render(RenderContext context, Guid ObjectId, string Body, RenderingResult result)
        {
            if (context.WebSite.EnableCssSplitByMedia)
            {
                var renderResult = CssSpliter.instance.Render(context, ObjectId, Body);
                if (!string.IsNullOrWhiteSpace(renderResult))
                {
                    result.Body = renderResult;
                    result.OverWriteBody = true;
                    result.StopNext = true;
                }
            } 
        }
    }
}
