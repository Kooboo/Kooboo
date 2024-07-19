using Kooboo.Data.Context;
using Kooboo.Sites.Render;
using Kooboo.Sites.Render.Functions;


namespace Kooboo.Sites.ScriptModules.Render
{
    //Src need to append with {ModuleUrl}
    public class MailModuleUrlRenderTask : IRenderTask
    {
        private string srcValue { get; set; }

        private ValueRenderTask RenderValueTask { get; set; }

        private bool IsExternalLink { get; set; }

        // special, no change. 
        private bool IsSpecial { get; set; }

        public bool ClearBefore
        {
            get
            {
                return false;
            }
        }

        public MailModuleUrlRenderTask(string srcValue)
        {
            this.srcValue = srcValue;

            this.IsSpecial = Kooboo.Sites.Service.DomUrlService.IsSpecialUrl(this.srcValue);

            if (!IsSpecial)
            {
                if (Sites.Render.Functions.FunctionHelper.TryParseFunction(this.srcValue, out IFunction funcOUt))
                {
                    // might be a function
                    this.RenderValueTask = new ValueRenderTask(this.srcValue);
                }

                if (Service.DomUrlService.IsExternalLink(this.srcValue) || this.srcValue == "#")
                {
                    this.IsExternalLink = true;
                }
            }
        }

        public string Render(RenderContext context)
        {
            var result = GetSrc(context);
            // merge with module baseurl
            var moduleContext = context.GetItem<ModuleContext>();
            if (moduleContext != null)
            {
                return moduleContext.MakeModuleUrl(result);
            }
            return result;
        }

        private string GetSrc(RenderContext context)
        {
            string result = string.Empty;

            if (IsSpecial)
            {
                return this.srcValue;
            }
            else if (this.RenderValueTask != null)
            {
                result = RenderValueTask.Render(context);
            }
            else
            {
                result = this.srcValue;
            }
            result = this.PraseBracket(context, result);

            return result;
        }

        // get the value to repalce {key}
        private string PraseBracket(RenderContext context, string result)
        {
            if (result == null)
            {
                return null;
            }
            int start = result.IndexOf("{");
            if (start == -1)
            {
                return result;
            }

            int end = result.IndexOf("}");

            if (end > start && start > -1)
            {
                string key = result.Substring(start + 1, end - start - 1);

                object value = context.DataContext.GetValue(key);

                string strvalue = null;
                if (value != null)
                {
                    strvalue = value.ToString();
                }

                if (!string.IsNullOrEmpty(strvalue) && !strvalue.Contains("{") && !strvalue.Contains("}"))
                {
                    string replace = "{" + key + "}";

                    string newvalue = result.Replace(replace, strvalue);

                    return PraseBracket(context, newvalue);
                }
                else
                {
                    return result;
                }

            }
            else
            {
                return result;
            }
        }

        public void AppendResult(RenderContext context, List<RenderResult> result)
        {
            result.Add(new RenderResult() { Value = Render(context) });
        }

    }

}
