using Kooboo.Data.Context;
using Kooboo.Dom;
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
                    string boundary = Kooboo.Lib.Helper.StringHelper.GetUniqueBoundary();

                    var startbinding = new BindingObjectRenderTask()
                    { ObjectType = "layout", Boundary = boundary, NameOrId = element.id };
                    List<IRenderTask> bindingstarts = new List<IRenderTask>();
                    bindingstarts.Add(startbinding);
                    response.BindingTask = bindingstarts;

                    var endbinding = new BindingObjectRenderTask()
                    { ObjectType = "layout", IsEndBinding = true, Boundary = boundary, NameOrId = element.id };

                    List<IRenderTask> bindingends = new List<IRenderTask>();
                    bindingends.Add(endbinding);
                    response.EndBindingTask = bindingends;
                }
                return response;
            }

            return null; 
        }
    } 
}
