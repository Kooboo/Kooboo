//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Context;
using Kooboo.Dom;
using System.Linq;
using System.Collections.Generic;
using System;

namespace Kooboo.Sites.Render
{
    public class PlaceHolderEvaluator : IEvaluator
    {
        public EvaluatorResponse Evaluate(Node node, EvaluatorOption options)
        { 
            if (options.IgnoreEvaluators.HasFlag(EnumEvaluator.PlaceHolder))
            {
                return null;
            }

            if (node.nodeType != enumNodeType.ELEMENT)
            {
                return null;
            }
            var element = node as Element;

            //"tal-placeholder", "position", "placeholder" 
            string PositionName = null;
            foreach (var item in element.attributes)
            {
                var lower = item.name.ToLower();
                if (lower == "tal-placeholder" || lower == "k-position" || lower == "tal-position" || lower == "k-placeholder")
                {
                    PositionName = item.name;
                    break;
                }
            }

            if (!string.IsNullOrEmpty(PositionName))
            {
               var response = new EvaluatorResponse(); 
                List<IRenderTask> result = new List<IRenderTask>();
                string PositionValue = element.getAttribute(PositionName);
                element.removeAttribute(PositionName); 
                result.Add(new PlaceHolderRenderTask(PositionValue)); 
                response.ContentTask = result;
                return response; 
            }
            return null; 
        }
    }

    
}
