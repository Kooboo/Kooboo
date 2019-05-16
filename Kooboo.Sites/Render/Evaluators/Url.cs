//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Dom;
using System.Collections.Generic;
  
namespace Kooboo.Sites.Render
{
    public class UrlEvaluator : IEvaluator
    {
        public EvaluatorResponse Evaluate(Node node, EvaluatorOption options)
        {
            if (options.IgnoreEvaluators.HasFlag(EnumEvaluator.Url))
            {
                return null;
            }

            if (node.nodeType != enumNodeType.ELEMENT)
            {
                return null;
            }
            var element = node as Element;

            string url = string.Empty;
            string attName = null; 

            foreach (var item in element.attributes)
            {
                var lower = item.name.ToLower();
                if (lower == "tal-href" || lower == "k-href")
                {
                    attName = item.name;
                    break;
                }
            }

            if (!string.IsNullOrEmpty(attName))
            {
                url = element.getAttribute(attName);
                if (!options.RequireBindingInfo)
                {
                    element.removeAttribute(attName);
                }
            }

            if (string.IsNullOrEmpty(url))
            {
                if (options.RenderUrl && element.tagName == "a")
                {
                    string href = element.getAttribute("href");
                    if (!string.IsNullOrEmpty(href))
                    {
                        url = href.Trim();
                    }
                    if (!options.RequireBindingInfo)
                    {
                        element.removeAttribute("href");
                    }
                }
            }

            if (!string.IsNullOrEmpty(url))
            {

                if (Kooboo.Sites.Service.DomUrlService.IsSpecialUrl(url))
                {
                    return null; 
                }

                var response = new EvaluatorResponse();
                List<IRenderTask> result = new List<IRenderTask>();
                result.Add(new ContentRenderTask(" href=\""));
                result.Add(new UrlRenderTask(url));
                result.Add(new ContentRenderTask("\""));
                response.AttributeTask = result;

                if (options.RequireBindingInfo)
                {
                    string attributeName = string.IsNullOrEmpty(attName) ? "href" : attName;
                    string koobooid = element.getAttribute(SiteConstants.KoobooIdAttributeName);
                    BindingObjectRenderTask binding = new BindingObjectRenderTask() { ObjectType = "Url", AttributeName = attributeName, BindingValue = url, KoobooId = koobooid };
                    List<IRenderTask> bindings = new List<IRenderTask>();
                    bindings.Add(binding);
                    response.BindingTask = bindings;
                }

                element.removeAttribute("href"); 

                return response;
            }

            return null;
        }
    }
}
