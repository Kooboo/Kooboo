//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using Kooboo.Dom;
using Kooboo.Sites.DataTraceAndModify.CustomTraces;

namespace Kooboo.Sites.Render
{
    public class FormEvaluator : IEvaluator
    {
        public EvaluatorResponse Evaluate(Node node, EvaluatorOption options)
        {
            if (options.IgnoreEvaluators.HasFlag(EnumEvaluator.Form))
            {
                return null;
            }

            if (node.nodeType != enumNodeType.ELEMENT)
            {
                return null;
            }
            var element = node as Element;

            if (ShouldEvaluate(element))
            {
                var response = new EvaluatorResponse();
                List<IRenderTask> result = new List<IRenderTask>();

                Guid OwnerObjectId = options.OwnerObjectId;
                string KoobooId = Service.DomService.GetKoobooId(element);
                Guid FormId = Kooboo.Data.IDGenerator.GetFormId(OwnerObjectId, KoobooId);

                result.Add(new FormRenderTask(element, OwnerObjectId, FormId, options));

                response.ContentTask = result;
                response.StopNextEvaluator = true;
                response.OmitTag = true;

                if (options.RequireBindingInfo)
                {

                    if (response.BindingTask == null) response.BindingTask = new List<IRenderTask>();
                    var traceability = new ComponentTrace(FormId.ToString(), "innerform");
                    var bindingTask = new BindingRenderTask(traceability);
                    response.BindingTask.Add(bindingTask);
                    if (response.EndBindingTask == null) response.EndBindingTask = new List<IRenderTask>();
                    response.EndBindingTask.Add(bindingTask.BindingEndRenderTask);
                }
                return response;
            }

            return null;
        }

        public bool ShouldEvaluate(Element el)
        {
            if (el.tagName != "form")
            {
                return false;
            }
            if (Service.DomService.IsAspNetWebForm(el))
            {
                return false;
            }

            return true;
        }
    }
}
