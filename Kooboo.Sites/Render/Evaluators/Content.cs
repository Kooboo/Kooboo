//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kooboo.Dom;
using Kooboo.Sites.DataTrace;

namespace Kooboo.Sites.Render
{
    public class ContentEvaluator : IEvaluator
    {
        public EvaluatorResponse Evaluate(Node node, EvaluatorOption options)
        {
            if (options.IgnoreEvaluators.HasFlag(EnumEvaluator.Content))
            {
                return null;
            }

            if (node.nodeType != enumNodeType.ELEMENT)
            {
                return null;
            }
            var element = node as Element;

            string attName = null;
            foreach (var item in element.attributes)
            {
                if (item.name == "tal-content" || item.name == "k-content" || item.name == "tal-replace" || item.name == "k-replace")
                {
                    attName = item.name;
                    break;
                }
            }
            if (!string.IsNullOrEmpty(attName))
            {
                var response = new EvaluatorResponse();
                List<IRenderTask> result = new List<IRenderTask>();
                string value = element.getAttribute(attName);
                element.removeAttribute(attName);
                result.Add(new ValueRenderTask(value));
                response.ContentTask = result;
                if (attName == "tal-replace" || attName == "k-replace")
                {
                    response.OmitTag = true;
                }

                if (options.RequireBindingInfo)
                {
                    if (response.BindingTask == null) response.BindingTask = new List<IRenderTask>();
                    var bindingTask = new BindingRenderTask(value, ActOn.content);
                    response.BindingTask.Add(bindingTask);
                    if (response.EndBindingTask == null) response.EndBindingTask = new List<IRenderTask>();
                    response.EndBindingTask.Add(bindingTask.BindingEndRenderTask);
                }

                return response;
            }
            return null;
        }
    }
}
