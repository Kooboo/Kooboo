//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Dom;
using Kooboo.Sites.DataTraceAndModify.CustomTraces;
using System.Collections.Generic;

namespace Kooboo.Sites.Render
{
    public class RepeaterEvaluator : IEvaluator
    {
        public EvaluatorResponse Evaluate(Node node, EvaluatorOption options)
        {
            if (options.IgnoreEvaluators.HasFlag(EnumEvaluator.Repeater))
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
                var lower = item.name.ToLower();
                if (lower == "tal-repeat" || lower == "k-repeat" || lower == "k-foreach")
                {
                    attName = item.name;
                    break;
                } 
            }

            if (string.IsNullOrEmpty(attName))
            {
                return null; 
            } 
            string repeatitems = element.getAttribute(attName).Trim();

            if(string.IsNullOrEmpty(repeatitems))
            {
                return null; 
            }

            //if (!options.RequireBindingInfo)
           // {
                element.removeAttribute(attName);
            //}

            bool repeatself = false;  
            if (element.hasAttribute("repeat-self"))
            {
                repeatself = true; 
                element.removeAttribute("repeat-self"); 
            } 
           else if (element.hasAttribute("tal-repeat-self"))
            {
                repeatself = true; 
                 element.removeAttribute("tal-repeat-self"); 
            }
            else if (element.hasAttribute("k-repeat-self"))
            {
                repeatself = true; 
                 element.removeAttribute("k-repeat-self");
            }
              
                string datakey;
                string alias = string.Empty;
                // the repeat item can be with or without an alias. it can be like articles or "item articles"; 
                string[] items = repeatitems.Split(' ');
                if (items.Length == 1)
                {
                    datakey = items[0];
                }
                else if (items.Length == 2)
                {
                    datakey = items[1];
                    alias = items[0];
                }
                else
                { 
                    return null;
                }
              
            RepeatRenderTask task = new RepeatRenderTask(datakey, alias, repeatself, element, options);
              
            var response = new EvaluatorResponse();
            List<IRenderTask> result = new List<IRenderTask>(); 
            result.Add(task);
            response.ContentTask = result;
            response.OmitTag = true;
            response.StopNextEvaluator = true;

            if (options.RequireBindingInfo)
            {
                if (response.BindingTask == null) response.BindingTask = new List<IRenderTask>();
                var traceability = new RepeatTrace();
                var bindingTask = new BindingRenderTask(traceability);
                response.BindingTask.Add(bindingTask);
                if (response.EndBindingTask == null) response.EndBindingTask = new List<IRenderTask>();
                response.EndBindingTask.Add(bindingTask.BindingEndRenderTask);
            }

            return response;

        }
    } 
    
}
