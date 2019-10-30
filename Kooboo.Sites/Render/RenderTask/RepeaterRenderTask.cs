//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Data.Context;
using Kooboo.Data.Models;
using Kooboo.Dom;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kooboo.Sites.Render
{
    public class RepeatRenderTask : IRenderTask
    {
        /// <summary>
        /// The key to access the data context.
        /// </summary>
        public string datakey { get; set; }

        /// <summary>
        /// The alias that can be used for binding, like foreach item in article.comments. item is the alias.
        /// right now, alias can not be null.
        /// </summary>
        public string alias { get; set; }

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
            get { return _containerTask ?? (_containerTask = new List<IRenderTask>()); }
            set
            {
                _containerTask = value;
            }
        }

        public string ContainerEndTag { get; set; }

        public RepeatRenderTask(string dataKey, string alias, bool repeatSelf, Element element, EvaluatorOption options)
        {
            this.datakey = dataKey;
            this.alias = alias;
            this.repeatself = repeatSelf;
            string boundary = null;
            if (options.RequireBindingInfo)
            {
                boundary = Kooboo.Lib.Helper.StringHelper.GetUniqueBoundary();
            }

            if (repeatself)
            {
                string newHtml = Service.DomService.ReSerializeElement(element, element.InnerHtml);

                if (options.RequireBindingInfo)
                {
                    this.SubTasks.Add(new BindingTextContentItemRenderTask(this.alias, boundary, false));
                }
                this.SubTasks.AddRange(RenderEvaluator.Evaluate(newHtml, options));

                if (options.RequireBindingInfo)
                {
                    this.SubTasks.Add(new BindingTextContentItemRenderTask(this.alias, boundary, true));
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
                    this.SubTasks.Add(new BindingTextContentItemRenderTask(this.alias, boundary, false));
                }

                this.SubTasks.AddRange(RenderEvaluator.Evaluate(element.InnerHtml, options));

                if (options.RequireBindingInfo)
                {
                    this.SubTasks.Add(new BindingTextContentItemRenderTask(this.alias, boundary, true));
                }
            }
        }

        public string Render(RenderContext context)
        {
            // step get the repeat object. and push to datacontext every time.
            object repeatcontainer = context.DataContext.GetValue(this.datakey);

            if (repeatcontainer == null)
            {
                return null;
            }

            StringBuilder sb = new StringBuilder();

            string container = this.ContainerTask.Render(context);
            if (!string.IsNullOrEmpty(container))
            {
                sb.Append(container);
            }

            switch (repeatcontainer)
            {
                case DataMethodResult dataMethodResult:
                    repeatcontainer = dataMethodResult.Value;
                    break;
                case string _:
                    // this is json.
                    repeatcontainer = Lib.Helper.JsonHelper.Deserialize<List<object>>(repeatcontainer.ToString());
                    break;
            }

            IList itemcollection = GetList(repeatcontainer);

            context.DataContext.RepeatCounter.Push(itemcollection.Count);
            int counter = 0;

            foreach (var item in itemcollection)
            {
                counter = counter + 1;
                context.DataContext.RepeatCounter.CurrentCounter.Current = counter;

                if (counter > Kooboo.Data.AppSettings.MaxForEachLoop)
                {
                    throw new System.Exception(Data.Language.Hardcoded.GetValue("You have reached the max loop limitation in your account", context));
                }

                context.DataContext.Push(this.alias, item);

                sb.Append(this.SubTasks.Render(context));

                context.DataContext.Pop();
            }
            context.DataContext.RepeatCounter.Pop();

            if (!string.IsNullOrEmpty(this.ContainerEndTag))
            {
                sb.Append(this.ContainerEndTag);
            }

            return sb.ToString();
        }

        public IList GetList(object container)
        {
            IList itemcollection;
            switch (container)
            {
                case DataMethodResult containerresult when containerresult.Value is PagedResult paged:
                    itemcollection = ((IEnumerable)paged.DataList).Cast<object>().ToList();
                    break;
                case DataMethodResult containerresult:
                    itemcollection = ((IEnumerable)containerresult.Value).Cast<object>().ToList();
                    break;
                case PagedResult pagedResult:
                {
                    var paged = pagedResult;
                    itemcollection = ((IEnumerable)paged.DataList).Cast<object>().ToList();
                    break;
                }
                default:
                    itemcollection = ((IEnumerable)container).Cast<object>().ToList();
                    break;
            }

            return itemcollection;
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
            // step get the repeat object. and push to datacontext every time.
            object repeatcontainer = context.DataContext.GetValue(this.datakey);

            switch (repeatcontainer)
            {
                case null:
                    return;
                case DataMethodResult dataMethodResult:
                    repeatcontainer = dataMethodResult.Value;
                    break;
            }

            foreach (var item in this.ContainerTask)
            {
                item.AppendResult(context, result);
            }

            var itemcollection = ((IEnumerable)repeatcontainer).Cast<object>().ToList();
            context.DataContext.RepeatCounter.Push(itemcollection.Count());
            int counter = 0;

            foreach (var item in itemcollection)
            {
                counter += 1;
                context.DataContext.RepeatCounter.CurrentCounter.Current = counter;

                context.DataContext.Push(this.alias, item);

                foreach (var subitem in this.SubTasks)
                {
                    subitem.AppendResult(context, result);
                }
                context.DataContext.Pop();
            }
            context.DataContext.RepeatCounter.Pop();

            if (!string.IsNullOrEmpty(this.ContainerEndTag))
            {
                result.Add(new RenderResult() { Value = this.ContainerEndTag });
            }
        }
    }
}