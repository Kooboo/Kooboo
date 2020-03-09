//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Dom;
using Kooboo.Sites.DataTraceAndModify.CustomTraces;
using System;
using System.Collections.Generic;

namespace Kooboo.Sites.Render
{
    public class ComponentEvaluator : IEvaluator
    {
        public EvaluatorResponse Evaluate(Node node, EvaluatorOption options)
        {
            if (options.IgnoreEvaluators.HasFlag(EnumEvaluator.Component))
            {
                return null;
            }
            if (node.nodeType != enumNodeType.ELEMENT)
            {
                return null;
            }
            var element = node as Element;

            if (Components.Manager.IsComponent(element))
            {
                var response = new EvaluatorResponse();
                var result = new List<IRenderTask>();
                result.Add(new ComponentRenderTask(element));
                response.ContentTask = result;
                response.OmitTag = true;
                response.StopNextEvaluator = true;

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
