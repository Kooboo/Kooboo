using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kooboo.Dom;

namespace Kooboo.Sites.Render.Evaluators
{
    public class LocalCacheEvaluator : IEvaluator
    {
        public EvaluatorResponse Evaluate(Node node, EvaluatorOption options)
        {
            if (options.IgnoreEvaluators.HasFlag(EnumEvaluator.LocalCache))
            {
                return null;
            } 

            if (node.nodeType != enumNodeType.ELEMENT)
            {
                return null;
            }
            var element = node as Element;

            if (!element.hasAttribute("k-localcache"))
            {
                return null;
            }

            string attName = "k-localcache";
            string attvalue = element.getAttribute(attName);
            var source = GetSourceLink(element);

            if (string.IsNullOrEmpty(attvalue))
            {
                attvalue = source.AttributeValue;
            }

            element.removeAttribute(attName);

            if (attvalue == null)
            {
                return null;
            }

            element.removeAttribute(source.AttributeName);

            EvaluatorResponse response = new EvaluatorResponse();


            List<IRenderTask> tasks = new List<IRenderTask>();
            tasks.Add(new ContentRenderTask(" " + source.AttributeName + "=\""));

            var local = new LocalCacheRenderTask(attvalue);  
            tasks.Add(local);

            tasks.Add(new ContentRenderTask("\""));

            if (response.AttributeTask == null)
            {
                response.AttributeTask = tasks;
            }
            else
            {
                response.AttributeTask.AddRange(tasks);
            } 

            if (response.AttributeTask == null || response.AttributeTask.Count() == 0)
            {
                return null;
            }
            else
            {
                return response;
            }

        }

        public static SourceValue GetSourceLink(Element item)
        {
            SourceValue result = new SourceValue();
            string tagname = item.tagName.ToLower();

            if (tagname == "script" || tagname == "img" || tagname == "embed")
            {
                result.AttributeName = "src";
                result.AttributeValue = item.getAttribute("src");
            }
            else if (tagname == "a" || tagname == "link")
            {
                result.AttributeName = "href";
                result.AttributeValue = item.getAttribute("href");
            }
            else
            {
                var href = item.getAttribute("href");
                if (!string.IsNullOrWhiteSpace(href))
                {
                    result.AttributeName = "href";
                    result.AttributeValue = href;
                }
                else
                {
                    href = item.getAttribute("src");
                    if (!string.IsNullOrWhiteSpace(href))
                    {
                        result.AttributeName = "src";
                        result.AttributeValue = href;
                    }
                }

            }

            if (string.IsNullOrEmpty(result.AttributeValue))
            {
                return null;
            }

            var lowerhref = result.AttributeValue.ToLower();
            if (lowerhref == "#" || lowerhref.StartsWith("#") || lowerhref.StartsWith("javascript:"))
            {
                return null;
            }

            result.AttributeValue = result.AttributeValue.Replace("\r", "");
            result.AttributeValue = result.AttributeValue.Replace("\n", "");

            string tempwithoutBracket = result.AttributeValue.Replace("{", "");
            tempwithoutBracket = tempwithoutBracket.Replace("}", "");
            if (Lib.Helper.UrlHelper.IsValidUrl(tempwithoutBracket))
            {
                return result;
            }
            else
            {
                return null;
            }
        }
    }


    public class SourceValue
    {
        public string AttributeName { get; set; }

        public string AttributeValue { get; set; }
    }
}