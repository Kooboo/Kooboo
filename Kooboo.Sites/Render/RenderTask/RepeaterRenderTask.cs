//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Context;
using Kooboo.Data.Models;
using Kooboo.Dom;
using Kooboo.Sites.DataTraceAndModify.CustomTraces;
using Kooboo.Sites.Render.RenderTask;
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

        public RepeatRenderTask(string DataKey, string Alias, bool RepeatSelf, Element element, EvaluatorOption options)
        {
            this.datakey = DataKey;
            this.alias = Alias;
            this.repeatself = RepeatSelf;
            string boundary = null;
            if (options.RequireBindingInfo)
            {
                boundary = Kooboo.Lib.Helper.StringHelper.GetUniqueBoundary();
            }

            BindingEndRenderTask bindingEndRenderTask = null;

            if (options.RequireBindingInfo)
            {
                var bindingRenderTask = new BindingRenderTask(new RepeatItemTrace(Alias));
                bindingEndRenderTask = bindingRenderTask.BindingEndRenderTask;
                this.SubTasks.Add(bindingRenderTask);
            }

            if (repeatself)
            {
                string NewHtml = Service.DomService.ReSerializeElement(element, element.InnerHtml);
                this.SubTasks.AddRange(RenderEvaluator.Evaluate(NewHtml, options));
            }
            else
            {
                string opentag = string.Empty;

                this.ContainerEndTag = "</" + element.tagName + ">";

                var attributeEvaluator = new AttributeEvaluator();
                var response = attributeEvaluator.Evaluate(element, options);
                if (response != null && response.AttributeTask != null && response.AttributeTask.Count() > 0)
                {
                    opentag = RenderHelper.GetHalfOpenTag(element);
                    this.ContainerTask.Add(new ContentRenderTask(opentag));
                    this.ContainerTask.AddRange(response.AttributeTask);
                    this.ContainerTask.Add(new ContentRenderTask(">"));
                }
                else
                {
                    this.ContainerTask.Add(new ContentRenderTask(Service.DomService.ReSerializeOpenTag(element)));
                }
                this.SubTasks.AddRange(RenderEvaluator.Evaluate(element.InnerHtml, options));
            }

            if (options.RequireBindingInfo)
            {
                this.SubTasks.Add(bindingEndRenderTask);
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

            string container = RenderHelper.Render(this.ContainerTask, context);
            if (!string.IsNullOrEmpty(container))
            {
                sb.Append(container);
            }

            if (repeatcontainer is DataMethodResult)
            {
                repeatcontainer = ((DataMethodResult)repeatcontainer).Value;
            }
            else if (repeatcontainer.GetType() == typeof(string))
            {
                // this is json. 
                repeatcontainer = Lib.Helper.JsonHelper.Deserialize<List<object>>(repeatcontainer.ToString());
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

                sb.Append(RenderHelper.Render(this.SubTasks, context));

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
            if (container is DataMethodResult)
            {
                var containerresult = container as DataMethodResult;

                if (containerresult.Value is PagedResult)
                {
                    var paged = containerresult.Value as PagedResult;
                    itemcollection = ((IEnumerable)paged.DataList).Cast<object>().ToList();
                }
                else
                {
                    itemcollection = ((IEnumerable)containerresult.Value).Cast<object>().ToList();
                }
            }
            else
            {
                if (container is PagedResult)
                {
                    var paged = container as PagedResult;
                    itemcollection = ((IEnumerable)paged.DataList).Cast<object>().ToList();
                }
                else
                {
                    itemcollection = ((IEnumerable)container).Cast<object>().ToList();
                }

            }

            if (itemcollection == null)
            {
                itemcollection = new List<string>();
            }
            return itemcollection;
        }

        private List<IRenderTask> _subtasks;

        public List<IRenderTask> SubTasks
        {
            get
            {
                if (_subtasks == null)
                {
                    _subtasks = new List<IRenderTask>();
                }
                return _subtasks;
            }
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

            if (repeatcontainer == null)
            {
                return;
            }

            if (repeatcontainer is DataMethodResult)
            {
                repeatcontainer = ((DataMethodResult)repeatcontainer).Value;
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
                counter = counter + 1;
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
