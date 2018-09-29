//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using Kooboo.Data.Context;
using Kooboo.Data.Models;
using Kooboo.Sites.Extensions;

namespace Kooboo.Sites.HtmlForm
{
    public class ValueSubmitter : Data.Interface.IFormSubmitter
    {
        public string Name
        {
            get { return "FormValue"; }
        }

        public string CustomActionUrl(RenderContext context, Dictionary<string, string> settings)
        {
            return null;
        }

        public List<SimpleSetting> Settings(RenderContext context)
        {
            return new List<SimpleSetting>();
        }

        public bool Submit(RenderContext context, Guid FormId, Dictionary<string, string> settings)
        {

            Models.FormValue value = new Models.FormValue();

            Dictionary<string, string> SubmittedValue = new Dictionary<string, string>();
              
            if (context.Request.Forms.Count > 0)
            {
                foreach (var item in context.Request.Forms.AllKeys)
                {
                    if (!item.StartsWith("_kooboo"))
                    {
                        string key = item.ToString();
                        string keyvalue = context.Request.Forms.Get(key);
                        SubmittedValue[key] = keyvalue;
                    }
                }
            }
            else if (context.Request.QueryString.Count > 0)
            {
                foreach (var item in context.Request.QueryString.AllKeys)
                {
                    if (!item.StartsWith("_kooboo"))
                    {
                        string key = item.ToString();
                        string keyvalue = context.Request.QueryString.Get(key);
                        SubmittedValue[key] = keyvalue;
                    }
                }
            }
            
            if (SubmittedValue.Count() == 0)
            {
                return true;
            }
            value.Values = SubmittedValue;
            value.FormId = FormId;
            return context.WebSite.SiteDb().FormValues.AddOrUpdate(value);

        }
    }
}
