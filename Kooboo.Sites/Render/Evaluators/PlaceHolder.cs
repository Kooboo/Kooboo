//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Dom;
using System.Collections.Generic;

namespace Kooboo.Sites.Render
{
    public class PlaceHolderEvaluator : IEvaluator
    {
        public EvaluatorResponse Evaluate(Node node, EvaluatorOption options)
        {
            if (options.IgnoreEvaluators.HasFlag(EnumEvaluator.PlaceHolder))
            {
                return null;
            }

            if (node.nodeType != enumNodeType.ELEMENT)
            {
                return null;
            }
            var element = node as Element;

            //"tal-placeholder", "position", "placeholder"
            string positionName = null;
            foreach (var item in element.attributes)
            {
                var lower = item.name.ToLower();
                if (lower == "tal-placeholder" || lower == "k-position" || lower == "tal-position" || lower == "k-placeholder")
                {
                    positionName = item.name;
                    break;
                }
            }

            if (!string.IsNullOrEmpty(positionName))
            {
                var response = new EvaluatorResponse();
                List<IRenderTask> result = new List<IRenderTask>();
                string positionValue = element.getAttribute(positionName);
                element.removeAttribute(positionName);
                result.Add(new PlaceHolderRenderTask(positionValue));
                response.ContentTask = result;
                return response;
            }
            return null;
        }
    }
}