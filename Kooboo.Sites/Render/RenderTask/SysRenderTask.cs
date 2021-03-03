using Kooboo.Data.Context;
using Kooboo.Dom;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Render
{ 
    public class SysRenderTask : IRenderTask
    {
        private string ConditionText { get; set; }

        private string AttributeName { get; set; } 

        public string KeyWord { get; set; } 
      
        public SysRenderTask(Element element,string attributeName,  string ConditionText, EvaluatorOption options)
        { 

            this.ConditionText = ConditionText.Trim();

            this.AttributeName = attributeName; 

            this.KeyWord = this.AttributeName.Replace("k-sys-", "");
            
            if (this.KeyWord !=null)
            {
                var para = ",;'\""; 
                this.KeyWord = this.KeyWord.ToLower().Trim(para.ToCharArray()); 
            }
             
            string NewElementString = Service.DomService.ReSerializeElement(element);

            this.SubTasks = RenderEvaluator.Evaluate(NewElementString, options);
        }
         
        public string Render(RenderContext context)
        {
            var testok = EvaluateCondition(context);
             
            if (testok)
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
            // now support Mobile, desktop. 
            if (this.KeyWord == "mobile")
            {
                var device = RequestManager.GetSystemValue(context.Request, "device"); 
                
                if (device.ToLower() == "mobile")
                {
                    return true; 
                } 
            }
           else if (this.KeyWord == "desktop")
            {
                var device = RequestManager.GetSystemValue(context.Request, "device");

                if (device.ToLower() == "desktop")
                {
                    return true;
                }
            }

            return false;
        }
         
        public string GetSystemValue(RenderContext context, string key)
        {
            return RequestManager.GetValue(context.Request, key);
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
     
}
