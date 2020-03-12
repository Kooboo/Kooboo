//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.Collections.Generic;
using Kooboo.Dom;
using Kooboo.Sites.DataTraceAndModify.CustomTraces;

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

            string LabelName = null;
            foreach (var item in element.attributes)
            {
                var lower = item.name.ToLower();
                if (lower == "tal-label" || lower == "k-label")
                {
                    LabelName = item.name;
                    break;
                }
            }
            if (!string.IsNullOrEmpty(LabelName))
            {
                var response = new EvaluatorResponse();
                var result = new List<IRenderTask>();
                string value = element.getAttribute(LabelName);
                element.removeAttribute(LabelName);

                if (options.RequireBindingInfo)
                {
                    if (response.BindingTask == null) response.BindingTask = new List<IRenderTask>();
                    var traceability = new ComponentTrace(value, "label");
                    var bindingTask = new BindingRenderTask(traceability);
                    response.BindingTask.Add(bindingTask);
                    if (response.EndBindingTask == null) response.EndBindingTask = new List<IRenderTask>();
                    response.EndBindingTask.Add(bindingTask.BindingEndRenderTask);
                }

                result.Add(new LabelRenderTask(value));
                response.ContentTask = result;
                return response;
            }
            return null;
        }
    }

}
