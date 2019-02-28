//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kooboo.Data.Context;
using Kooboo.Dom;
using Kooboo.Sites.Extensions;

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
                List<IRenderTask> result = new List<IRenderTask>();
                string value = element.getAttribute(LabelName); 

                if (!options.RequireBindingInfo)
                {
                    element.removeAttribute(LabelName); 
                }
                else
                {
                    string koobooid = element.getAttribute("kooboo-id");
                    BindingObjectRenderTask binding = new BindingObjectRenderTask() { ObjectType = "Label", AttributeName = LabelName, BindingValue = value, KoobooId = koobooid };
                    List<IRenderTask> bindings = new List<IRenderTask>();
                    bindings.Add(binding);
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
