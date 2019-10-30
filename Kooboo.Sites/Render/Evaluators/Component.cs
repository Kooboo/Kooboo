//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Dom;
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
                List<IRenderTask> result = new List<IRenderTask> {new ComponentRenderTask(element)};
                response.ContentTask = result;
                response.OmitTag = true;
                response.StopNextEvaluator = true;

                if (options.RequireBindingInfo)
                {
                    string boundary = Kooboo.Lib.Helper.StringHelper.GetUniqueBoundary();
                    var startbinding = new BindingObjectRenderTask()
                    { ObjectType = element.tagName, Boundary = boundary, NameOrId = element.id };
                    List<IRenderTask> bindingstarts = new List<IRenderTask> {startbinding};
                    response.BindingTask = bindingstarts;

                    var endbinding = new BindingObjectRenderTask()
                    { ObjectType = element.tagName, IsEndBinding = true, Boundary = boundary, NameOrId = element.id };

                    List<IRenderTask> bindingends = new List<IRenderTask> {endbinding};
                    response.EndBindingTask = bindingends;
                }
                return response;
            }

            return null;
        }
    }
}