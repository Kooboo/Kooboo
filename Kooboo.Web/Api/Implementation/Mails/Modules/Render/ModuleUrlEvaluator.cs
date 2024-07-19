using Kooboo.Dom;
using Kooboo.Sites.Render;

namespace Kooboo.Sites.ScriptModules.Render
{
    //All src should be replace with the relative / resource link....
    public class MailModuleUrlEvaluator : IEvaluator
    {
        List<string> _attributes = new List<string> { "href", "k-href" };

        public EvaluatorResponse Evaluate(Node node, EvaluatorOption options)
        {
            if (node.nodeType != enumNodeType.ELEMENT)
            {
                return null;
            }
            var element = node as Element;

            if (element.tagName.ToLower() != "a" && element.tagName.ToLower() != "area")
            {
                return null;
            }

            var response = new EvaluatorResponse();

            string srcValue = string.Empty;
            string attName = null;

            foreach (var item in element.attributes)
            {
                var lower = item.name.ToLower();
                if (_attributes.Contains(lower))
                {
                    attName = item.name;
                    break;
                }
            }

            if (!string.IsNullOrEmpty(attName))
            {
                srcValue = element.getAttribute(attName);
                element.removeAttribute(attName);
            }


            if (!string.IsNullOrEmpty(srcValue))
            {
                var result = new List<IRenderTask>();
                result.Add(new StaticContentRenderTask(" " + attName + "=\""));
                result.Add(new MailModuleUrlRenderTask(srcValue));
                result.Add(new StaticContentRenderTask("\""));
                response.AttributeTask = result;
                element.removeAttribute(attName);
                return response;
            }

            return null;
        }
    }

}
