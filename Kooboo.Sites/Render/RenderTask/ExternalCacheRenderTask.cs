using Kooboo.Data.Context;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Render.RenderTask
{

    public class ExternalCacheRenderTask : IRenderTask
    {
        private string Url { get; set; }
        private int Interval { get; set; }

        public string cacheid { get; set; }

        public bool ClearBefore
        {
            get
            {
                return false;
            }
        }

        public ExternalCacheRenderTask(string Url, int interval)
        {
            this.Url = Url;
            this.Interval = interval;
            // Start the new thread to download... 
            this.cacheid  =  Kooboo.Sites.Render.Renderers.ExternalCacheRender.AddNew(this.Url, interval);  
        }


        public string Render(RenderContext context)
        {
            return "/__kb/" + "KExternalCache/" + this.cacheid; 
        }

        public void AppendResult(RenderContext context, List<RenderResult> result)
        {
            result.Add(new RenderResult() { Value = Render(context) });
        }
    }




}
