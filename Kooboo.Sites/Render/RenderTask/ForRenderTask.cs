//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Data.Context;
using Kooboo.Dom;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kooboo.Sites.Render
{
    public class ForRenderTask : IRenderTask
    {
        /// <summary>
        /// The key to access the data context.
        /// </summary>
        public string datakey { get; set; }

        /// <summary>
        /// The alias that can be used for binding, like foreach item in article.comments. item is the alias.
        /// right now, alias can not be null.
        /// </summary>
        public string HighBoundKey { get; set; }

        public string LowBoundKey { get; set; }

        public long LowBound { get; set; } = long.MinValue;

        public long HighBound { get; set; } = long.MinValue;

        /// <summary>
        /// whether the self container element needs to be repeated or not.
        /// </summary>
        public bool repeatself { get; set; }

        private List<IRenderTask> _containerTask;

        /// <summary>
        /// The outside container render task when it is NOT repeatself. The most outside tag.
        /// </summary>
        public List<IRenderTask> ContainerTask
        {
            get
            {
                if (_containerTask == null)
                {
                    _containerTask = new List<IRenderTask>();
                }
                return _containerTask;
            }
            set
            {
                _containerTask = value;
            }
        }

        public string ContainerEndTag { get; set; }

        public ForRenderTask(string dataKey, string lowbound, string highbound, bool repeatSelf, Element element, EvaluatorOption options)
        {
            this.datakey = dataKey;

            if (long.TryParse(lowbound, out var low))
            {
                this.LowBound = low;
            }
            else
            {
                this.LowBoundKey = lowbound;
            }

            if (long.TryParse(highbound, out var high))
            {
                this.HighBound = high;
            }
            else
            {
                this.HighBoundKey = highbound;
            }

            this.repeatself = repeatSelf;
            if (options.RequireBindingInfo)
            {
                Kooboo.Lib.Helper.StringHelper.GetUniqueBoundary();
            }

            if (repeatself)
            {
                string newHtml = Service.DomService.ReSerializeElement(element, element.InnerHtml);

                if (options.RequireBindingInfo)
                {
                    //  this.SubTasks.Add(new BindingTextContentItemRenderTask(this.alias, boundary, false));
                }
                this.SubTasks.AddRange(RenderEvaluator.Evaluate(newHtml, options));

                if (options.RequireBindingInfo)
                {
                    //this.SubTasks.Add(new BindingTextContentItemRenderTask(this.alias, boundary, true));
                }
            }
            else
            {
                this.ContainerEndTag = "</" + element.tagName + ">";

                var attributeEvaluator = new AttributeEvaluator();
                var response = attributeEvaluator.Evaluate(element, options);
                if (response?.AttributeTask != null && response.AttributeTask.Any())
                {
                    var opentag = RenderHelper.GetHalfOpenTag(element);
                    this.ContainerTask.Add(new ContentRenderTask(opentag));
                    this.ContainerTask.AddRange(response.AttributeTask);
                    this.ContainerTask.Add(new ContentRenderTask(">"));
                }
                else
                {
                    this.ContainerTask.Add(new ContentRenderTask(Service.DomService.ReSerializeOpenTag(element)));
                }

                if (options.RequireBindingInfo)
                {
                    // this.SubTasks.Add(new BindingTextContentItemRenderTask(this.alias, boundary, false));
                }

                this.SubTasks.AddRange(RenderEvaluator.Evaluate(element.InnerHtml, options));

                if (options.RequireBindingInfo)
                {
                    // this.SubTasks.Add(new BindingTextContentItemRenderTask(this.alias, boundary, true));
                }
            }
        }

        public string Render(RenderContext context)
        {
            long low = this.LowBound;
            if (low == long.MinValue && !string.IsNullOrEmpty(this.LowBoundKey))
            {
                var lowvalue = context.DataContext.GetValue(this.LowBoundKey);

                if (lowvalue == null || !long.TryParse(lowvalue.ToString(), out low))
                {
                    return null;
                }
            }

            long high = this.HighBound;
            if (high == long.MinValue && !string.IsNullOrEmpty(this.HighBoundKey))
            {
                var highvalue = context.DataContext.GetValue(this.HighBoundKey);
                if (highvalue == null || !long.TryParse(highvalue.ToString(), out high))
                {
                    return null;
                }
            }

            if (high == long.MinValue || low == long.MinValue)
            {
                return null;
            }
            int count = (int)(high - low);

            if (count < 1)
            {
                return null;
            }

            StringBuilder sb = new StringBuilder();

            string container = this.ContainerTask.Render(context);
            if (!string.IsNullOrEmpty(container))
            {
                sb.Append(container);
            }

            context.DataContext.RepeatCounter.Push(count);
            int counter = 0;

            for (long i = low; i < high; i++)
            {
                counter = counter + 1;

                if (counter > Kooboo.Data.AppSettings.MaxForEachLoop)
                {
                    throw new System.Exception(Data.Language.Hardcoded.GetValue("You have reached the max loop limitation in your account", context));
                }

                context.DataContext.RepeatCounter.CurrentCounter.Current = counter;

                if (!string.IsNullOrEmpty(this.datakey))
                {
                    context.DataContext.Push(this.datakey, i);
                }

                sb.Append(this.SubTasks.Render(context));

                if (!string.IsNullOrEmpty(this.datakey))
                {
                    context.DataContext.Pop();
                }
            }

            context.DataContext.RepeatCounter.Pop();

            if (!string.IsNullOrEmpty(this.ContainerEndTag))
            {
                sb.Append(this.ContainerEndTag);
            }

            return sb.ToString();
        }

        private List<IRenderTask> _subtasks;

        public List<IRenderTask> SubTasks
        {
            get { return _subtasks ?? (_subtasks = new List<IRenderTask>()); }
            set
            {
                _subtasks = value;
            }
        }

        public bool ClearBefore
        {
            get
            {
                return false;
            }
        }

        public void AppendResult(RenderContext context, List<RenderResult> result)
        {
            long low = this.LowBound;
            if (low == long.MinValue && !string.IsNullOrEmpty(this.LowBoundKey))
            {
                var lowvalue = context.DataContext.GetValue(this.LowBoundKey);

                if (lowvalue == null || !long.TryParse(lowvalue.ToString(), out low))
                {
                    return;
                }
            }

            long high = this.HighBound;
            if (high == long.MinValue && !string.IsNullOrEmpty(this.HighBoundKey))
            {
                var highvalue = context.DataContext.GetValue(this.HighBoundKey);
                if (highvalue == null || !long.TryParse(highvalue.ToString(), out high))
                {
                    return;
                }
            }

            if (high == long.MinValue || low == long.MinValue)
            {
                return;
            }
            int count = (int)(high - low);

            if (count < 1)
            {
                return;
            }

            foreach (var item in this.ContainerTask)
            {
                item.AppendResult(context, result);
            }

            context.DataContext.RepeatCounter.Push(count);
            int counter = 0;

            for (long i = low; i < high; i++)
            {
                counter = counter + 1;
                context.DataContext.RepeatCounter.CurrentCounter.Current = counter;

                if (!string.IsNullOrEmpty(this.datakey))
                {
                    context.DataContext.Push(this.datakey, i);
                }

                context.DataContext.Push(this.datakey, i);

                foreach (var subitem in this.SubTasks)
                {
                    subitem.AppendResult(context, result);
                }

                if (!string.IsNullOrEmpty(this.datakey))
                {
                    context.DataContext.Pop();
                }
            }

            context.DataContext.RepeatCounter.Pop();

            if (!string.IsNullOrEmpty(this.ContainerEndTag))
            {
                result.Add(new RenderResult() { Value = this.ContainerEndTag });
            }
        }
    }
}