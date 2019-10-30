using Kooboo.Dom;
using Kooboo.Sites.Render.RenderTask;
using System.Collections.Generic;

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

            string labelName = null;
            foreach (var item in element.attributes)
            {
                var lower = item.name.ToLower();
                if (lower == "k-externalcache")
                {
                    labelName = item.name;
                    break;
                }
            }
            if (!string.IsNullOrEmpty(labelName))
            {
                var response = new EvaluatorResponse();

                string value = element.getAttribute(labelName);

                element.removeAttribute(labelName);

                string strinternval = element.getAttribute("k-interval");
                if (!string.IsNullOrWhiteSpace(strinternval))
                {
                    int.TryParse(strinternval, out interval);

                    element.removeAttribute("k-interval");
                }
                element.removeAttribute("href");

                List<IRenderTask> result = new List<IRenderTask>
                {
                    new ContentRenderTask(" href=\""),
                    new ExternalCacheRenderTask(value, interval),
                    new ContentRenderTask("\"")
                };
                response.AttributeTask = result;

                return response;
            }
            return null;
        }
    }
}