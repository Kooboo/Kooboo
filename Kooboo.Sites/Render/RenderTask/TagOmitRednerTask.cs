using Kooboo.Data.Context;
using Kooboo.Dom;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Render
{
    /// <summary>
    /// conditional render task first check the condition, and if true, render the sub tasks, otherwise do nothing. 
    /// </summary>
    public class TagOmitRenderTask : IRenderTask
    {

        public TagOmitRenderTask(Element element, EvaluatorOption options)
        {
            //string NewElementString = Service.DomService.ReSerializeElement(element); 
            this.SubTasks = RenderEvaluator.Evaluate(element.InnerHtml, options);
        }


        public string Render(RenderContext context)
        {
            if (this.SubTasks != null)
            {
                StringBuilder sb = new StringBuilder();
                foreach (var item in this.SubTasks)
                {
                    sb.Append(item.Render(context));
                }
                return sb.ToString();
            }
            return null;
        }

        public List<IRenderTask> SubTasks { get; set; }

        public bool ClearBefore
        {
            get
            {
                return false;
            }
        }

        public void AppendResult(RenderContext context, List<RenderResult> result)
        {
            if (this.SubTasks != null)
            {
                foreach (var item in this.SubTasks)
                {
                    result.Add(new RenderResult() { Value = item.Render(context) });
                }
            }
        }
    }
}

