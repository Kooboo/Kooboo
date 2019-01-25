//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Dom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Sites.Render
{ 

    public class ConditionEvaluator : IEvaluator
    {
        public EvaluatorResponse Evaluate(Node node, EvaluatorOption options)
        {
            if (options.IgnoreEvaluators.HasFlag(EnumEvaluator.Condition))
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
                if (item.name == "tal-condition" ||  item.name == "k-if" || item.name == "k-condition")
                {
                    attName = item.name;
                    break;
                }
            }
            if (!string.IsNullOrEmpty(attName))
            {
                string conditiontext = element.getAttribute(attName);
                if (string.IsNullOrEmpty(conditiontext))
                {
                    return null; 
                } 
                element.removeAttribute(attName);  
                var response = new EvaluatorResponse();
                List<IRenderTask> result = new List<IRenderTask>();  
                result.Add(new ConditionRenderTask(element, conditiontext,  options));
                response.ContentTask = result;
                response.StopNextEvaluator = true;
                response.OmitTag = true; 
                return response;
            }
            return null;
        }
    }
     
}
