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

        public SiteLayoutRenderTask(Element LayoutElement, EvaluatorOption option)
        {
            if (LayoutElement.hasAttribute("id"))
            {
                this.LayoutName = LayoutElement.getAttribute("id");
            }
            else if (LayoutElement.hasAttribute("name"))
            {
                this.LayoutName = LayoutElement.getAttribute("name");
            }

            LoadComponents(LayoutElement, option);
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
                        List<IRenderTask> ComTask = new List<IRenderTask>();

                        string innerhtml = child.InnerHtml;

                        if (!string.IsNullOrWhiteSpace(innerhtml))
                        {
                            var newoption = option.Clone();
                            newoption.RenderHeader = false;
                            ComTask = RenderEvaluator.Evaluate(innerhtml, option);
                        }

                        if (Components.ContainsKey(positionname))
                        {
                            var current = Components[positionname];
                            current.AddRange(ComTask);
                            Components[positionname] = current;
                        }
                        else
                        {
                            Components[positionname] = ComTask;
                        }
                    }

                }
            }
        }

        public string Render(RenderContext context)
        {
            var plans = RenderPlanManager.GetLayoutPlan(this.LayoutName, context);
            plans = PreRenderLayout(context, plans);
            RenderComponent(context);
            var result = RenderHelper.Render(plans, context);
            return result;
        }

        private List<IRenderTask> PreRenderLayout(RenderContext context, List<IRenderTask> layoutplan)
        {
            List<IRenderTask> newPlan = new List<IRenderTask>(); 

            for (int i = 0; i < layoutplan.Count; i++)
            {
                var item = layoutplan[i];
                if (item is HeaderRenderTask || item is PlaceHolderRenderTask || item is ContentRenderTask)
                {
                    //do nothing.
                    newPlan.Add(item);
                }
                else
                {
                    var result = item.Render(context);
                    newPlan.Add(new ContentRenderTask(result));
                }
            } 
            return newPlan;
        }

        private void RenderComponent(RenderContext context)
        {
            foreach (var item in Components)
            {
                string ComponentContent = item.Value.Render(context);

                if (context.PlaceholderContents.ContainsKey(item.Key))
                {
                    var current = context.PlaceholderContents[item.Key];
                    context.PlaceholderContents[item.Key] = current + ComponentContent;
                }
                else
                {
                    context.PlaceholderContents[item.Key] = ComponentContent;
                }
            }
        }

        public void AppendResult(RenderContext context, List<RenderResult> result)
        {
            result.Add(new RenderResult() { Value = this.Render(context) });
        }
    }
}
