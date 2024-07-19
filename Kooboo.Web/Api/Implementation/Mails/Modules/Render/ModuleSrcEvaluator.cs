using Kooboo.Dom;
using Kooboo.Sites.Render;

namespace Kooboo.Sites.ScriptModules.Render.View
{
    public class MailModuleSrcEvaluator : IEvaluator
    {
        List<string> _attributes = new List<string> { "href", "src" };

        public EvaluatorResponse Evaluate(Node node, EvaluatorOption options)
        {
            if (node.nodeType != enumNodeType.ELEMENT)
            {
                return null;
            }
            var element = node as Element;

            if (element.tagName.ToLower() == "a" || element.tagName.ToLower() == "area")
            {
                return null;
            }
            var response = new EvaluatorResponse();

            string srcValue = string.Empty;
            string attName = null;

            if (element.tagName.ToLower() == "link")
            {
                //< link rel = "stylesheet" type = "text/css" href = "/c8.css" />
                srcValue = element.getAttribute("href");
                if (!string.IsNullOrEmpty(srcValue))
                {
                    attName = "href";
                }
            }
            else
            {
                srcValue = element.getAttribute("src");
                if (!string.IsNullOrEmpty(srcValue))
                {
                    attName = "src";
                }
                else
                {
                    srcValue = element.getAttribute("data-src");
                    if (!string.IsNullOrEmpty(srcValue))
                    {
                        attName = "data-src";
                    }
                }
            }


            if (!string.IsNullOrEmpty(attName) && !string.IsNullOrEmpty(srcValue))
            {
                var result = new List<IRenderTask>();
                result.Add(new StaticContentRenderTask(" " + attName + "=\""));
                result.Add(new MailModuleSrcRenderTask(element.tagName, srcValue));
                result.Add(new StaticContentRenderTask("\""));
                response.AttributeTask = result;
                element.removeAttribute(attName);
                return response;

            }

            return null;
        }
    }

}
