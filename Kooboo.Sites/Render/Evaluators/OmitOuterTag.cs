using Kooboo.Dom;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Render
{ 
    public class OmitOuterTagEvaluator : IEvaluator
    {
        public EvaluatorResponse Evaluate(Node node, EvaluatorOption options)
        {
            if (options.IgnoreEvaluators.HasFlag(EnumEvaluator.OmitTag))
            {
                return null;
            }

            if (node.nodeType != enumNodeType.ELEMENT)
            {
                return null;
            }
            var element = node as Element;

            bool shouldomit = false;

            foreach (var item in element.attributes)
            {
                if (item.name == "k-omitouter" || item.name == "k-omitoutertag")
                {
                    shouldomit = true;
                    break;
                }
            }

            if (shouldomit)
            {
                TagOmitRenderTask task = new TagOmitRenderTask(element, options);

                var response = new EvaluatorResponse();
                List<IRenderTask> result = new List<IRenderTask>();
                result.Add(task);
                response.ContentTask = result;
                response.OmitTag = true;
                response.StopNextEvaluator = true; 
                return response;
            }
            else
            {
                return null;
            }
        }
    } 
}
