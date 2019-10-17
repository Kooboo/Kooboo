using System;
using System.Collections.Generic;
using Kooboo.Dom;
using Kooboo.Sites.Render.RenderTask;

namespace Kooboo.Sites.Render.Evaluators
{
    public class kExternalCacheEvaluator : IEvaluator
    {
        public EvaluatorResponse Evaluate(Node node, EvaluatorOption options)
        {
            if (options.IgnoreEvaluators.HasFlag(EnumEvaluator.kCache))
            {
                return null;
            }

            if (node.nodeType != enumNodeType.ELEMENT)
            {
                return null;
            }
            var element = node as Element;

            int interval = 60 * 60;

            string LabelName = null;
            foreach (var item in element.attributes)
            {
                var lower = item.name.ToLower();
                if (lower == "k-externalcache")
                {
                    LabelName = item.name;
                    break;
                }
            }
            if (!string.IsNullOrEmpty(LabelName))
            {
                var response = new EvaluatorResponse();
             
                string value = element.getAttribute(LabelName);

                element.removeAttribute(LabelName);

                string strinternval = element.getAttribute("k-interval");
                if (!string.IsNullOrWhiteSpace(strinternval))
                {
                    int.TryParse(strinternval, out interval);

                    element.removeAttribute("k-interval");
                }
                element.removeAttribute("href");

                List<IRenderTask> result = new List<IRenderTask>();
                result.Add(new ContentRenderTask(" href=\""));
                result.Add(new ExternalCacheRenderTask(value, interval));
                result.Add(new ContentRenderTask("\""));
                response.AttributeTask = result; 
                 
                return response;
            }
            return null;
        }
    }







}
