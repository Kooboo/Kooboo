//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Context;
using Kooboo.Dom;
using Kooboo.Sites.DataTraceAndModify.CustomTraces;
using System.Collections.Generic;

namespace Kooboo.Sites.Render
{
    public class SiteLayoutEvaluator : IEvaluator
    {
        public EvaluatorResponse Evaluate(Node node, EvaluatorOption options)
        {
            if (options.IgnoreEvaluators.HasFlag(EnumEvaluator.SiteLayout))
            {
                return null;
            }

            if (node.nodeType != enumNodeType.ELEMENT)
            {
                return null;
            }
            var element = node as Element;

            if (element.tagName == "layout" && (element.hasAttribute("id") || element.hasAttribute("name")))
            {
               var response = new EvaluatorResponse();
                var result = new List<IRenderTask>(); 
                result.Add(new SiteLayoutRenderTask(element, options));
                response.ContentTask = result; 
                response.OmitTag = true;
                 
                if (options.RequireBindingInfo)
                {

                    if (response.BindingTask == null) response.BindingTask = new List<IRenderTask>();
                    var traceability = new ComponentTrace(element.id, element.tagName);
                    var bindingTask = new BindingRenderTask(traceability, new Dictionary<string, string> { { "scope", "true" } });
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
