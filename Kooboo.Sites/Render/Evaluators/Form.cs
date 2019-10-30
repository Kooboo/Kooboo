//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Dom;
using System;
using System.Collections.Generic;

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

                Guid ownerObjectId = options.OwnerObjectId;
                string koobooId = Service.DomService.GetKoobooId(element);
                Guid formId = Kooboo.Data.IDGenerator.GetFormId(ownerObjectId, koobooId);

                result.Add(new FormRenderTask(element, ownerObjectId, formId, options));

                response.ContentTask = result;
                response.StopNextEvaluator = true;
                response.OmitTag = true;

                if (options.RequireBindingInfo)
                {
                    string boundary = Kooboo.Lib.Helper.StringHelper.GetUniqueBoundary();

                    var startbinding = new BindingObjectRenderTask()
                    { ObjectType = "form", Boundary = boundary, NameOrId = formId.ToString() };
                    List<IRenderTask> bindingstarts = new List<IRenderTask> {startbinding};
                    response.BindingTask = bindingstarts;

                    var endbinding = new BindingObjectRenderTask()
                    { ObjectType = "form", IsEndBinding = true, Boundary = boundary, NameOrId = formId.ToString() };

                    List<IRenderTask> bindingends = new List<IRenderTask> {endbinding};
                    response.EndBindingTask = bindingends;
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
            return !Service.DomService.IsAspNetWebForm(el);
        }
    }
}