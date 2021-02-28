using Kooboo.Data.Context;
using Kooboo.Sites.Render.Functions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Kooboo.Sites.Render
{
    public class LocalCacheRenderTask : IRenderTask
    {
        public string KeyOrExpression;
        /// <summary>
        /// check whether it is an expression or not. 
        /// </summary>
        public bool IsExpression { get; set; }

        public bool IsFunction { get; set; }

        public bool IsString { get; set; }

        public IFunction function { get; set; }

        public string StringValue { get; set; }

        private List<string> _parameters;

        /// <summary>
        /// used for the {} embedded parameters, this can be multiple values. 
        /// </summary>
        public List<string> Parameters
        {
            get
            {
                if (_parameters == null)
                {
                    _parameters = new List<string>();
                }
                return _parameters;
            }
            set
            {
                this._parameters = value;
            }
        }

        public bool ClearBefore
        {
            get
            {
                return false;
            }
        }

        public LocalCacheRenderTask(string KeyOrExpression)
        {
            if (string.IsNullOrEmpty(KeyOrExpression))
            {
                return;
            }

            this.KeyOrExpression = KeyOrExpression; 

            KeyOrExpression = KeyOrExpression.Trim();

            if (KeyOrExpression.IndexOf("{") > -1 && KeyOrExpression.IndexOf("}") > -1)
            {
                if (KeyOrExpression.StartsWith("{") && KeyOrExpression.EndsWith("}") && KeyOrExpression.IndexOf("{", 1) == -1)
                {
                    this.KeyOrExpression = KeyOrExpression.Substring(1, KeyOrExpression.Length - 2);
                }
                else
                {
                    this.KeyOrExpression = KeyOrExpression;
                    this.IsExpression = true;
                    string regexpattern = @"\{(?<Name>.*?)\}";
                    var matches = Regex.Matches(KeyOrExpression, regexpattern);

                    int counter = 0;

                    foreach (Match item in matches)
                    {
                        string value = item.Groups["Name"].Value;
                        this.Parameters.Add(value);
                        this.KeyOrExpression = this.KeyOrExpression.Replace("{" + value + "}", "{" + counter.ToString() + "}");
                        counter += 1;
                    }
                }
            }
            else
            {

                if (FunctionHelper.IsFunction(KeyOrExpression))
                {
                    this.IsFunction = true;
                    this.function = FunctionHelper.Parse(KeyOrExpression);
                    this.KeyOrExpression = KeyOrExpression;
                }

                else if (Lib.Helper.StringHelper.IsString(KeyOrExpression))
                {
                    this.IsString = true;
                    if (KeyOrExpression.StartsWith("'"))
                    {
                        this.StringValue = KeyOrExpression.Trim('\'');

                    }
                    else if (KeyOrExpression.StartsWith("\""))
                    {
                        this.StringValue = KeyOrExpression.Trim('"');
                    }
                    else
                    {
                        this.StringValue = KeyOrExpression;
                    }
                } 
                else
                {
                    var lower = KeyOrExpression.ToLower();
                    if (lower.StartsWith("https://") || lower.StartsWith("http://") || lower.StartsWith("//") || lower.StartsWith("/"))
                    {
                        this.IsString = true;
                        this.StringValue = KeyOrExpression; 
                    }  
                } 
            }
        }

        public string Render(RenderContext context)
        {
            var url =  PraseUrl(context);

            if (url.StartsWith("//"))
            {
                if (context.Request.Port == 443)
                {
                    url = "https:" + url; 
                }
                else
                {
                    url = "http://" + url; 
                }
            } 


            var shorturl = Kooboo.Sites.Render.LocalCache.LocalCacheManager.SetItem(context.WebSite, url);

            return Kooboo.Sites.Render.LocalCache.LocalCacheManager.GetUrl(shorturl); 
        }

        private string PraseUrl(RenderContext context)
        {
            string result = null;

            if (this.IsExpression)
            {
                List<object> resultvalues = new List<object>();
                foreach (var item in this.Parameters)
                {
                    resultvalues.Add(GetValueFromContext(item, context));
                }

                try
                {
                    result = string.Format(this.KeyOrExpression, resultvalues.ToArray());
                }
                catch (Exception ex)
                {
                   
                }

                return result;
            }
            else if (this.IsFunction && function != null)
            {
                var funcresult = this.function.Render(context);
                if (funcresult != null)
                {
                    return funcresult.ToString();
                }
            }
            else if (this.IsString)
            {
                return this.StringValue;
            }
            else
            {
                result = GetValueFromContext(this.KeyOrExpression, context);
                if (!string.IsNullOrEmpty(result))
                {
                    return result.ToString();
                }
                else { return ""; }
            }
            return result;
        }

        private string GetValueFromContext(string KeyOrExpression, RenderContext context)
        {
            var value = context.DataContext.GetValue(KeyOrExpression);
            if (value != null)
            {
                return value.ToString();
            }
            return null;
        }

        public void AppendResult(RenderContext context, List<RenderResult> result)
        {
            result.Add(new RenderResult() { Value = Render(context) });
        }
    }
}





