using System;
using System.Collections.Generic;
using System.Linq;
using Kooboo.Dom;
using Kooboo.Sites.Render;
using Kooboo.Sites.Render.RenderTask;

namespace Kooboo.Render.Customized
{

    public class AdminVersionEvaluator : IEvaluator
    {
        public EvaluatorResponse Evaluate(Node node, EvaluatorOption options)
        {
            if (options.IgnoreEvaluators.HasFlag(EnumEvaluator.Version))
            {
                return null;
            }

            if (!options.EnableImageBrowserCache && !options.EnableJsCssBrowserCache && !options.EnableResourceCDN)
            {
                return null;
            }


            if (node.nodeType != enumNodeType.ELEMENT)
            {
                return null;
            }
            var element = node as Element;

            // only for style, script and image. 
            if (element.tagName != "script" && element.tagName != "link" && element.tagName != "img")
            {
                return null;
            }

            if (element.tagName == "img")
            {
                if (!options.EnableImageBrowserCache && !options.EnableResourceCDN)
                {
                    return null;
                }
            }

            if (element.tagName == "script" || Sites.Service.DomService.IsCssLinkElement(element))
            {
                if (!options.EnableJsCssBrowserCache && !options.EnableResourceCDN)
                {
                    return null;
                }
            }

            Dictionary<string, string> appendValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            string attname = null;

            bool IsStyle = false;
            bool IsImage = false;

            if (element.tagName == "script")
            {
                attname = "src";
            }
            else if (element.tagName == "link")
            {
                attname = "href";
                IsStyle = true;
            }
            else if (element.tagName == "img")
            {
                attname = "src";
                IsImage = true;
            }

            string value = element.getAttribute(attname);

            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            element.removeAttribute(attname);

            EvaluatorResponse response = new EvaluatorResponse();

            List<IRenderTask> tasks = new List<IRenderTask>();
            tasks.Add(new StaticContentRenderTask(" " + attname + "=\""));


            if (options.EnableImageBrowserCache || options.EnableJsCssBrowserCache)
            {
                var versiontask = new AdminVersionRenderTask(value, IsStyle, IsImage);
                if (options.EnableResourceCDN)
                {
                    var cdnTask = new ResourceCDNRenderTask(versiontask);
                    tasks.Add(cdnTask);
                }
                else
                {
                    tasks.Add(versiontask);
                }
            }
            else
            {
                if (options.EnableResourceCDN)
                {
                    var cdnTask = new ResourceCDNRenderTask(value);
                    tasks.Add(cdnTask);
                }
                else
                {
                    // should not do anything. ignore work above.  Should not reach here. 
                    return null;
                }
            }


            tasks.Add(new StaticContentRenderTask("\""));

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


    }



}
