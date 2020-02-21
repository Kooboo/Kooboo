//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Context;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Sites.Converter
{
    public class ViewConverter : IConverter
    { 

        public string Type
        {
            get
            {
                return "view"; 
            }
        }

        public ConvertResponse Convert(RenderContext context, JObject result)
        {
            var page = context.GetItem<Page>(); 
            if (page == null || string.IsNullOrEmpty(page.Body) || page.Dom == null)
            {
                return null;
            }

            string converttype = Lib.Helper.JsonHelper.GetString(result, "ConvertToType");
            string convertname = Lib.Helper.JsonHelper.GetString(result, "Name"); 

            var name = ConvertManager.GetUniqueName(context, converttype, convertname);

            View view = new View();
            view.Name = name;
            view.Body = Lib.Helper.JsonHelper.GetString(result, "HtmlBody");

            context.WebSite.SiteDb().Views.AddOrUpdate(view);

            return new ConvertResponse
            {
                 ComponentNameOrId = view.Name,
                  Tag = "<view id='" + view.Name.ToString() + "'></view>", 
            };
             
             
        }
         
    }
}
