//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Data.Context;
using Kooboo.Dom;
using System.Collections.Generic;

namespace Kooboo.Sites.Render
{
    public class SiteLayoutRenderTask : IRenderTask
    {
        public string LayoutName { get; set; }

        private Dictionary<string, string> Settings { get; set; }

        public bool ClearBefore
        {
            get
            {
                return false;
            }
        }

        public Dictionary<string, List<IRenderTask>> Components = new Dictionary<string, List<IRenderTask>>();

        public SiteLayoutRenderTask(Element layoutElement, EvaluatorOption option)
        {
            if (layoutElement.hasAttribute("id"))
            {
                this.LayoutName = layoutElement.getAttribute("id");
            }
            else if (layoutElement.hasAttribute("name"))
            {
                this.LayoutName = layoutElement.getAttribute("name");
            }

            LoadComponents(layoutElement, option);
        }

        private void LoadComponents(Element element, EvaluatorOption option)
        {
            foreach (var item in element.childNodes.item)
            {
                if (item.nodeType == enumNodeType.ELEMENT)
                {
                    Element child = item as Element;

                    if (child.tagName == "position" || child.tagName == "placeholder")
                    {
                        string positionname = child.id;
                        if (string.IsNullOrEmpty(positionname))
                        {
                            positionname = child.getAttribute("name");
                        }

                        if (string.IsNullOrEmpty(positionname))
                        {
                            continue;
                        }
                        List<IRenderTask> comTask = new List<IRenderTask>();

                        string innerhtml = child.InnerHtml;

                        if (!string.IsNullOrWhiteSpace(innerhtml))
                        {
                            var newoption = option.Clone();
                            newoption.RenderHeader = false;
                            comTask = RenderEvaluator.Evaluate(innerhtml, option);
                        }

                        if (Components.ContainsKey(positionname))
                        {
                            var current = Components[positionname];
                            current.AddRange(comTask);
                            Components[positionname] = current;
                        }
                        else
                        {
                            Components[positionname] = comTask;
                        }
                    }
                }
            }
        }

        public string Render(RenderContext context)
        {
            RenderComponent(context);
            var plans = RenderPlanManager.GetLayoutPlan(this.LayoutName, context);
            var result = plans.Render(context);
            return result;
        }

        private void RenderComponent(RenderContext context)
        {
            foreach (var item in Components)
            {
                string componentContent = item.Value.Render(context);

                if (context.PlaceholderContents.ContainsKey(item.Key))
                {
                    var current = context.PlaceholderContents[item.Key];
                    context.PlaceholderContents[item.Key] = current + componentContent;
                }
                else
                {
                    context.PlaceholderContents[item.Key] = componentContent;
                }
            }
        }

        public void AppendResult(RenderContext context, List<RenderResult> result)
        {
            result.Add(new RenderResult() { Value = this.Render(context) });
        }
    }
}