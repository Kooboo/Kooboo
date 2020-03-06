//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Context;
using Kooboo.Dom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Sites.Render
{
    public static class RenderHelper
    {
        public static string Render(this List<IRenderTask> task, RenderContext context)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var item in task)
            {
                if (item.ClearBefore)
                {
                    string currentvalue = sb.ToString();
                    sb.Clear();
                    context.AddPlaceHolderContent("", currentvalue);
                }

                sb.Append(item.Render(context));
            }
            return sb.ToString();
        }

        public static void OptimizeTask(List<IRenderTask> task)
        {
            int count = task.Count();
            bool IsPreviousContent = false;
            int lastContentI = -1;
            List<int> ToRemoved = new List<int>();
            for (int i = 0; i < count; i++)
            {
                var item = task[i];

                if (item is ContentRenderTask)
                {
                    if (IsPreviousContent && lastContentI != -1)
                    {
                        var contenttask = item as ContentRenderTask;

                        var lastcontenttask = task[lastContentI] as ContentRenderTask;
                        lastcontenttask.Content += contenttask.Content;
                        ToRemoved.Add(i);
                    }
                    IsPreviousContent = true;
                    if (lastContentI == -1)
                    {
                        lastContentI = i;
                    }
                }

                else
                {
                    IsPreviousContent = false;
                    lastContentI = -1;
                }
            }

            ToRemoved.Reverse();
            foreach (var item in ToRemoved)
            {
                task.RemoveAt(item);
            }
        }

        public static string GetHalfOpenTag(Element element)
        {
            string ehtml = string.Empty;
            ehtml = "<" + element.tagName;
            foreach (var item in element.attributes)
            {
                ehtml += " " + item.name;
                if (!string.IsNullOrEmpty(item.value))
                {
                    ehtml += "=\"" + item.value + "\"";
                }
            }
            return ehtml;
        }

        public static string LazyRender(List<IRenderTask> task, RenderContext context)
        {
            List<RenderResult> result = new List<RenderResult>();
            foreach (var item in task)
            {
                if (item.ClearBefore)
                {
                    result.Add(new RenderResult() { ClearBefore = true });
                }
                item.AppendResult(context, result);
            }

            StringBuilder sb = new StringBuilder();

            foreach (var item in result)
            {
                if (item.ClearBefore)
                {
                    string currentvalue = sb.ToString();
                    sb.Clear();
                    context.AddPlaceHolderContent("", currentvalue);
                }
                else
                {
                    sb.Append(item.Value);
                }
            }
            return sb.ToString();
        }

        public static string ModelBind(object Model, string Html, string ModelName = "model")
        {
            if (Model == null)
            {
                return Html;
            }
            RenderContext context = new RenderContext();
            context.DataContext.Push(ModelName, Model);

            var plans = RenderEvaluator.Evaluate(Html, new EvaluatorOption());
            return Render(plans, context);
        }
    }
}
