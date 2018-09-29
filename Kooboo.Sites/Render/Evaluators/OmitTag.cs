//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kooboo.Dom;

namespace Kooboo.Sites.Render
{
    public class OmitTagEvaluator : IEvaluator
    {
        public EvaluatorResponse Evaluate(Node node, EvaluatorOption options)
        {
            if (options.IgnoreEvaluators.HasFlag(EnumEvaluator.OmitTag))
            {
                return null;
            }

            if (node.nodeType != enumNodeType.ELEMENT)
            {
                return null;
            }
            var element = node as Element;

            foreach (var item in element.attributes)
            { 
                if (item.name == "k-omit" || item.name == "omittag" || item.name == "omit-tag")
                {
                    element.removeAttribute(item.name); 
                   return new EvaluatorResponse() { OmitTag = true};  
                }
            } 
            return null;
        }
    }
 
}
