using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kooboo.Dom;

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
                    string koobooid = element.getAttribute(SiteConstants.KoobooIdAttributeName); 
                    if (!string.IsNullOrEmpty(koobooid))
                    {  
                        BindingContentRenderTask binding = new BindingContentRenderTask() { ObjectType = "content", BindingValue = value, KoobooId = koobooid };
                        List<IRenderTask> bindings = new List<IRenderTask>();
                        bindings.Add(binding);
                        response.BindingTask = bindings;  
                    }
                    else 
                    {
                        string boundary = Kooboo.Lib.Helper.StringHelper.GetUniqueBoundary();
                        BindingContentRenderTask binding = new BindingContentRenderTask() { ObjectType = "content", BindingValue = value, Boundary = boundary, KoobooId = koobooid };
                        List<IRenderTask> bindings = new List<IRenderTask>();
                        bindings.Add(binding);
                        response.BindingTask = bindings;
                        
                        BindingContentRenderTask bindingend = new BindingContentRenderTask() { ObjectType = "content", BindingValue = value, Boundary = boundary, KoobooId = koobooid, IsEndBinding = true };
                        List<IRenderTask> bindingsend = new List<IRenderTask>();
                        bindingsend.Add(bindingend);
                        response.EndBindingTask = bindingsend; 

                    } 

                }

                return response;
            }
            return null;
        }
    }
}
