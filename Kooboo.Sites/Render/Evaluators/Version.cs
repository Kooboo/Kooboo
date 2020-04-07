using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kooboo.Dom;
using Kooboo.Sites.Render.RenderTask;

namespace Kooboo.Sites.Render.Evaluators
{
    public class VersionEvaluator : IEvaluator
    {
        public EvaluatorResponse Evaluate(Node node, EvaluatorOption options)
        {
            //Kooboo.Lib.Utilities.DataUriService.isDataUri
            if (options.IgnoreEvaluators.HasFlag(EnumEvaluator.Version))
            {
                return null;
            }

            if (!options.EnableImageBrowserCache && !options.EnableJsCssBrowserCache)
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
                if (!options.EnableImageBrowserCache)
                {
                    return null;
                }
            }

            if (element.tagName == "script" || element.tagName == "style")
            {
                if (!options.EnableJsCssBrowserCache)
                {
                    return null;
                }
            } 

            if (element.tagName == "link")
            {
                var rel = element.getAttribute("rel");
                if (rel == null || rel.ToLower() != "stylesheet")
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
            tasks.Add(new ContentRenderTask(" " + attname + "=\""));

            tasks.Add(new VersionRenderTask(value, IsStyle, IsImage));

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
    }


}
