//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq; 
using Kooboo.Data.Context;

namespace Kooboo.Sites.Render.Functions
{
    public class PagerFunction : IFunction
    {
        public string Name
        {
            get
            {
                return "Pager"; 
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

           if (providedParas !=null && providedParas.Count()>0)
            {
                paras["PageNumber"] = providedParas[0].ToString();
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
        
    }
}
