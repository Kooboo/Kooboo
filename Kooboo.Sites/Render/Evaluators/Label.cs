//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Dom;
using System.Collections.Generic;

namespace Kooboo.Sites.Render
{
    public class LabelEvaluator : IEvaluator
    {
        public EvaluatorResponse Evaluate(Node node, EvaluatorOption options)
        {
            if (options.IgnoreEvaluators.HasFlag(EnumEvaluator.Label))
            {
                return null;
            }

            if (node.nodeType != enumNodeType.ELEMENT)
            {
                return null;
            }
            var element = node as Element;

            string labelName = null;
            foreach (var item in element.attributes)
            {
                var lower = item.name.ToLower();
                if (lower == "tal-label" || lower == "k-label")
                {
                    labelName = item.name;
                    break;
                }
            }
            if (!string.IsNullOrEmpty(labelName))
            {
                var response = new EvaluatorResponse();
                List<IRenderTask> result = new List<IRenderTask>();
                string value = element.getAttribute(labelName);

                if (!options.RequireBindingInfo)
                {
                    element.removeAttribute(labelName);
                }
                else
                {
                    string koobooid = element.getAttribute("kooboo-id");
                    BindingObjectRenderTask binding = new BindingObjectRenderTask() { ObjectType = "Label", AttributeName = labelName, BindingValue = value, KoobooId = koobooid };
                    List<IRenderTask> bindings = new List<IRenderTask> {binding};
                    response.BindingTask = bindings;
                }

                result.Add(new LabelRenderTask(value));
                response.ContentTask = result;
                return response;
            }
            return null;
        }
    }
}