//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Context;
using Kooboo.Data.Models;
using Kooboo.Sites.Extensions;
using System;
using System.Collections.Generic;
using Kooboo.Sites.Models;

namespace Kooboo.Sites.HtmlForm
{
    public class ApiSubmitter : Data.Interface.IFormSubmitter
    {
        public string Name
        {
            get
            {
                return "Api";
            }
        }

        public string CustomActionUrl(RenderContext context, Dictionary<string, string> settings)
        {
            return null;
        }

        public List<SimpleSetting> Settings(RenderContext context)
        {
            List<SimpleSetting> setting = new List<SimpleSetting>();
            SimpleSetting foldersetting = new SimpleSetting();
            foldersetting.Name = "Api";
            foldersetting.ControlType = Data.ControlType.Selection;

            var codes = context.WebSite.SiteDb().Code.ListByCodeType(Models.CodeType.Api);
            foreach (var item in codes)
            {
                foldersetting.SelectionValues.Add(item.Id.ToString(), item.Name);
            }
            setting.Add(foldersetting);

            return setting;
        }

        public bool Submit(RenderContext context, Guid FormId, Dictionary<string, string> settings)
        {
            var sitedb = context.WebSite.SiteDb();
            // right now, only one setting.. the code id. 
            Guid codeid = default(Guid);
            string codename = null;
            foreach (var item in settings)
            {
                if (item.Value != null)
                {
                    if (System.Guid.TryParse(item.Value, out codeid))
                    {
                        break;
                    }
                }

                if (item.Key == "Api")
                {
                    codename = item.Value;
                }
            }

            Code code = null;
            if (codeid != default(Guid))
            {
                code = sitedb.Code.Get(codeid);
            }
            else if (codename != null)
            {
                code = sitedb.Code.GetByNameOrId(codename);
            }
            else
            {
                return false;
            }

            if (code != null && !string.IsNullOrEmpty(code.Body))
            {


                string result = Kooboo.Sites.Scripting.Manager.ExecuteCode(context, code.Body, code.Id);
                if (!string.IsNullOrWhiteSpace(result))
                {
                    context.Response.ContentType = "application/json";
                    context.Response.Body = System.Text.Encoding.UTF8.GetBytes(result);
                    context.Response.End = true; 
                }
                return true;
            }

            return false;
        }
    }
}
