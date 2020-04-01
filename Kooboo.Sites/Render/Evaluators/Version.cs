using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kooboo.Dom;
using Kooboo.Sites.Render.RenderTask;

namespace Kooboo.Sites.Render.Evaluators
{
    public class VersionEvaluator : IEvaluator
    {
        public EvaluatorResponse Evaluate(Node node, EvaluatorOption options)
        {
            if (options.IgnoreEvaluators.HasFlag(EnumEvaluator.Version))
            {
                return null;
            }

            Dictionary<string, string> appendValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            if (node.nodeType != enumNodeType.ELEMENT)
            {
                return null;
            }
            var element = node as Element;

            if (element.tagName != "script" && element.tagName != "link")
            {
                return null;
            }

            if (element.tagName == "link")
            {
                var rel = element.getAttribute("rel");
                if (rel == null || rel.ToLower() != "stylesheet")
                {
                    return null;
                }
            }

            if (!element.hasAttribute("k-version"))
            {
                return null;
            }

            element.removeAttribute("k-version");

            string attname = null;

            bool IsStyle = false;

            if (element.tagName == "script")
            {
                attname = "src";
            }
            else if (element.tagName == "link")
            {
                attname = "href";
                IsStyle = true;
            }

            string value = element.getAttribute(attname);

            element.removeAttribute(attname);

            EvaluatorResponse response = new EvaluatorResponse();

            List<IRenderTask> tasks = new List<IRenderTask>();
            tasks.Add(new ContentRenderTask(" " + attname + "=\""));

            tasks.Add(new VersionRenderTask() { IsStyle = IsStyle, Url = value }); 
 
            tasks.Add(new ContentRenderTask("\""));

            if (response.AttributeTask == null)
            {
                response.AttributeTask = tasks;
            }
            else
            {
                response.AttributeTask.AddRange(tasks);
            }


            if (response.AttributeTask == null || response.AttributeTask.Count() == 0)
            {
                return null;
            }
            else
            {
                return response;
            }
        } 
    }


}
