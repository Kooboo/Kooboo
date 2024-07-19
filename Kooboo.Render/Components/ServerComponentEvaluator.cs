using System.Collections.Generic;
using Kooboo.Dom;
using Kooboo.Sites.Render;

namespace Kooboo.Render.Components
{
    public class ServerComponentEvaluator : IEvaluator
    {
        public EvaluatorResponse Evaluate(Node node, EvaluatorOption options)
        {
            if (node.nodeType != enumNodeType.ELEMENT)
            {
                return null;
            }
            var element = node as Element;

            if (ComponentService.IsServerComponent(element.tagName))
            {
                var response = new EvaluatorResponse();
                List<IRenderTask> result = new List<IRenderTask>();
                result.Add(new ServerComponentRenderTask(element));
                response.ContentTask = result;
                response.OmitTag = true;
                response.StopNextEvaluator = true;
                return response;

            }
            return null;
        }
    }


}
