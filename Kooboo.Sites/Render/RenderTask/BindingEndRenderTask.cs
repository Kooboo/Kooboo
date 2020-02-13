using Kooboo.Data.Context;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Render.RenderTask
{
    public class BindingEndRenderTask : IRenderTask
    {
        public string Uid { get; set; }

        public bool ClearBefore
        {
            get
            {
                return false;
            }
        }

        public void AppendResult(RenderContext context, List<RenderResult> result)
        {
            string renderresult = this.Render(context);
            result.Add(new RenderResult() { Value = renderresult });
        }

        public virtual string Render(RenderContext context)
        {
            return $"{Environment.NewLine}<!--#kooboo--end=true--uid={Uid}-->{Environment.NewLine}";
        }
    }
}
