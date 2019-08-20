//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.Collections.Generic;
using System.Text;
using Kooboo.Dom;
using Kooboo.Data.Context;
using Kooboo.Sites.DataSources;

namespace Kooboo.Sites.Render
{
    /// <summary>
    /// conditional render task first check the condition, and if true, render the sub tasks, otherwise do nothing. 
    /// </summary>
    public class ConditionRenderTask : IRenderTask
    {
        private string ConditionText { get; set; }

        private bool IsRepeatCondition { get; set; }

        private FilterDefinition Filter { get; set; }

        private ValueRenderTask ValueRenderTask { get; set; }

        private ValueRenderTask CompareValueRenderTask { get; set; }

        public ConditionRenderTask(Element element, string ConditionText, EvaluatorOption options)
        {
            if (ConditionText.ToLower().StartsWith("repeat"))
            {
                this.IsRepeatCondition = true;
                this.ConditionText = GetConditionText(ConditionText);
            }
            else
            {
                this.Filter = FilterHelper.GetFilter(ConditionText);
                 FilterHelper.CheckValueType(this.Filter); 
            }
            string NewElementString = Service.DomService.ReSerializeElement(element);

            this.SubTasks = RenderEvaluator.Evaluate(NewElementString, options);
        }
         

        public string Render(RenderContext context)
        {
            if (EvaluateCondition(context))
            {
                StringBuilder sb = new StringBuilder();

                foreach (var item in this.SubTasks)
                {
                    sb.Append(item.Render(context));
                }

                return sb.ToString();
            }
            else
            {
                return null;
            }
        }

        public List<IRenderTask> SubTasks { get; set; }

        public bool ClearBefore
        {
            get
            {
                return false;
            }
        }

        public bool EvaluateCondition(RenderContext context)
        {
            if (this.IsRepeatCondition)
            {
                return context.DataContext.RepeatCounter.Check(this.ConditionText);
            }
            else
            {
                if (this.Filter != null)
                {
                    string value = null; 
                    if (this.Filter.IsNameValueType)
                    {
                        value = this.Filter.FieldName; 
                    }
                    else
                    {
                        if (this.ValueRenderTask == null)
                        {
                            this.ValueRenderTask = new ValueRenderTask(this.Filter.FieldName);
                        }

                       value = this.ValueRenderTask.Render(context);
                    }
                   

                    if (value == null)
                    {
                        // TODO: add more k-system fields. 
                        if (this.Filter.FieldName == "k-index")
                        {
                            value = context.DataContext.RepeatCounter.CurrentCounter.Current.ToString();
                        }
                        else
                        {
                            return false;
                        }
                    }


                    string comparevalue = null;
                    if (this.Filter.IsValueValueType)
                    {
                        comparevalue = this.Filter.FieldValue;
                    }
                    else
                    {
                        if (this.CompareValueRenderTask == null)
                        {
                            this.CompareValueRenderTask = new ValueRenderTask(this.Filter.FieldValue);
                        }

                        var contextcomparevalue = this.CompareValueRenderTask.Render(context); 

                        if (!string.IsNullOrWhiteSpace(contextcomparevalue))
                        {
                            comparevalue = contextcomparevalue; 
                        } 
                        else
                        {
                            comparevalue = this.Filter.FieldValue; 
                        }
                    }


                    return FilterHelper.Check(value.ToString(), this.Filter.Comparer, comparevalue);
                }
            }

            return false;
        }

        private string GetConditionText(string input)
        {
            int index = input.IndexOf("\\");
            if (index > 0)
            {
                return input.Substring(index + 1).ToLower().Trim();
            }

            index = input.IndexOf("/");
            if (index > 0)
            {
                return input.Substring(index + 1).ToLower().Trim();
            }

            return input;

        }

        public void AppendResult(RenderContext context, List<RenderResult> result)
        {
            if (EvaluateCondition(context))
            {
                foreach (var item in this.SubTasks)
                {
                    result.Add(new RenderResult() { Value = item.Render(context) });
                }
            }
        }
    }

    public class ConditionFilter
    {
        public string KeyOrExpression { get; set; }
    }
}

