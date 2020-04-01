using System;
using System.Collections.Generic;
using System.Text;
using Kooboo.Dom;

namespace Kooboo.Sites.Render.Evaluators
{  
    public class VersionEvaluator : IEvaluator
    {  
        public EvaluatorResponse Evaluate(Node node, EvaluatorOption options)
        {
            if (options.IgnoreEvaluators.HasFlag(EnumEvaluator.Version))
            {
                return null;
            }

            Dictionary<string, string> appendValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            if (node.nodeType != enumNodeType.ELEMENT)
            {
                return null;
            }
            var element = node as Element;

            if (element.tagName !="script" && element.tagName != "link")
            {
                return null; 
            }

            if (!element.hasAttribute("k-version"))
            {
                return null; 
            } 
          
            element.removeAttribute("k-version");

            string attname = 

          
            EvaluatorResponse response = new EvaluatorResponse();
             

            foreach (var item in attributes)
            {
                var attkeyvalue = ParseAtt(item);
                if (attkeyvalue == null)
                {
                    continue;
                }

                string attributeName = attkeyvalue.Key;
                string attributeValue = attkeyvalue.Value;

                if (AppendAttributes.ContainsKey(attributeName))
                {
                    string sep = AppendAttributes[attributeName];
                    string value = element.getAttribute(attributeName);

                    if (!string.IsNullOrEmpty(value))
                    {
                        if (!value.Trim().EndsWith(sep))
                        {
                            value = value + sep;
                        }
                        if (appendValues.ContainsKey(attributeName))
                        {
                            var orgvalue = appendValues[attributeName];
                            value = orgvalue + value;
                        }
                        appendValues[attributeName] = value;
                    }
                }

                List<IRenderTask> tasks = new List<IRenderTask>();
                tasks.Add(new ContentRenderTask(" " + attributeName + "=\""));

                if (appendValues.ContainsKey(attributeName))
                {
                    tasks.Add(new ContentRenderTask(appendValues[attributeName]));
                }

                tasks.Add(new ValueRenderTask(attributeValue));
                tasks.Add(new ContentRenderTask("\""));

                if (response.AttributeTask == null)
                {
                    response.AttributeTask = tasks;
                }
                else
                {
                    response.AttributeTask.AddRange(tasks);
                } 
                 
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

        private AttKeyValue ParseAtt(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return null;
            }

            input = input.Trim();

            int spaceindex = input.IndexOf(" ");
            if (spaceindex == -1)
            {
                return null;
            }

            AttKeyValue result = new AttKeyValue();

            result.Key = input.Substring(0, spaceindex);
            result.Value = input.Substring(spaceindex).Trim();
            return result;
        }

        public class AttKeyValue
        {
            public string Key { get; set; }
            public string Value { get; set; }
        }

        public class AppendValues
        {
            public string AttName { get; set; }
            public string Value { get; set; }
        }
    }


}
