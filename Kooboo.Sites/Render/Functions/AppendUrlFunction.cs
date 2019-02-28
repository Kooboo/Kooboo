//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq; 
using Kooboo.Data.Context;

namespace Kooboo.Sites.Render.Functions
{
    public class AppendUrlFunction : IFunction
    {
        public string Name
        {
            get
            {
                return "AppendUrl"; 
            }
        }

        public List<IFunction> Parameters
        {
            get;set;
        }

        public object Render(RenderContext context)
        {
            string relative = context.Request.RelativeUrl;
            if (relative.IndexOf("?") > -1)
            {
                relative = relative.Substring(0, relative.IndexOf("?"));
            }

            var paras = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            foreach (var item in context.Request.QueryString.AllKeys)
            {
                var value = context.Request.QueryString[item];

                if (!paras.ContainsKey(item))
                {
                    paras.Add(item, value);
                }
            }
             
            var providedParas = FunctionHelper.RenderParameter(context, this.Parameters);

            var dict = ToDictionary(providedParas);

            foreach (var item in dict)
            {
                paras[item.Key] = item.Value;
            }
            if (paras.Count()>0)
            {
                return Lib.Helper.UrlHelper.AppendQueryString(relative, paras); 
            }
           else
            {
                return relative; 
            }
        }

       private Dictionary<string, string> ToDictionary(List<object> input)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            string name = null;
            string value = null; 
            foreach (var item in input)
            {
                if (item != null)
                {
                    if (string.IsNullOrEmpty(name))
                    {
                        name = item.ToString();
                    }
                    else if (string.IsNullOrEmpty(value))
                    {
                        value = item.ToString(); 
                    }

                    if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(value))
                    {
                        dict[name] = value;
                        name = null;
                        value = null;
                    }

                }
                else
                {
                    if (!string.IsNullOrEmpty(name))
                    {
                        dict[name] = value;
                        name = null;
                        value = null; 
                    }
                    else 
                    {
                        value = null; 
                    }
                }
            }

            return dict; 
        }
    }
}
