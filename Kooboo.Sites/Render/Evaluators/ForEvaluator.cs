//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Dom;
using Kooboo.Sites.DataTraceAndModify.CustomTraces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kooboo.Sites.Render.Evaluators
{

    public class ForEvaluator : IEvaluator
    {
        public EvaluatorResponse Evaluate(Node node, EvaluatorOption options)
        {
            if (options.IgnoreEvaluators.HasFlag(EnumEvaluator.For))
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
                if (lower == "tal-for" || lower == "k-for")
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

            if (string.IsNullOrEmpty(repeatitems))
            {
                return null;
            }

            element.removeAttribute(attName);

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

            var para = PrasePara(repeatitems);

            if (para == null)
            {
                return null;
            }

            ForRenderTask task = new ForRenderTask(para.DataKey, para.LowBound, para.HighBound, repeatself, element, options);

            var response = new EvaluatorResponse();
            var result = new List<IRenderTask>();
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

        private ForPara PrasePara(string input)
        {
            string[] items = input.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            if (items == null || items.Count() < 2)
            {
                return null;
            }

            if (items.Count() == 2)
            {
                ForPara para = new ForPara();
                para.LowBound = items[0].Trim();
                para.HighBound = items[1].Trim();
                return para;
            }
            else
            {
                ForPara para = new ForPara();
                para.LowBound = items[0].Trim();
                para.HighBound = items[1].Trim();
                para.DataKey = items[2].Trim();
                return para;
            }
        }

        private class ForPara
        {
            public string LowBound { get; set; }

            public string HighBound { get; set; }

            public string DataKey { get; set; }
        }

    }
}
