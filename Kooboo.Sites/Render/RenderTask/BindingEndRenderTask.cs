using Kooboo.Data.Context;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Render.RenderTask
{
    public class BindingEndRenderTask : IRenderTask
    {
        readonly string _uid;

        public BindingEndRenderTask(string uid)
        {
            _uid = uid;
        }

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
            return $@"
<!--#kooboo--end=true--uid={_uid}-->
";
        }
    }
}
