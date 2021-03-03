//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Dom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; 

namespace Kooboo.Sites.Render.Evaluators
{  
    public class SystemEvaluator : IEvaluator
    {
        public EvaluatorResponse Evaluate(Node node, EvaluatorOption options)
        {
            if (options.IgnoreEvaluators.HasFlag(EnumEvaluator.System))
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
                if (item.name.StartsWith("k-sys-"))
                {
                    attName = item.name;
                    break;
                }
            }
            if (!string.IsNullOrEmpty(attName))
            {
                string conditiontext = element.getAttribute(attName); 
                element.removeAttribute(attName);
                var response = new EvaluatorResponse();
                List<IRenderTask> result = new List<IRenderTask>();
                result.Add(new SysRenderTask(element, attName, conditiontext, options));
                response.ContentTask = result;
                response.StopNextEvaluator = true;
                response.OmitTag = true;
                return response;
            }
            return null;
        }
    }







}



