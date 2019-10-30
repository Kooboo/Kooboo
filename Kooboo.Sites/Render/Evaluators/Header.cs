//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Dom;
using System.Collections.Generic;

namespace Kooboo.Sites.Render
{
    public class HeaderEvaluator : IEvaluator
    {
        public EvaluatorResponse Evaluate(Node node, EvaluatorOption options)
        {
            if (options.IgnoreEvaluators.HasFlag(EnumEvaluator.Header))
            {
                return null;
            }

            if (node.nodeType != enumNodeType.ELEMENT)
            {
                return null;
            }
            var element = node as Element;

            if (element.tagName == "head" && options.RenderHeader)
            {
                var response = new EvaluatorResponse();
                var result = new List<IRenderTask> {new HeaderRenderTask(element)};
                response.ContentTask = result;
                response.OmitTag = true;
                response.StopNextEvaluator = true;
                return response;
            }
            return null;
        }
    }
}