//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Context;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Models;
using Newtonsoft.Json.Linq;
 

namespace Kooboo.Sites.Converter
{
    public class HtmlBlockConverter : IConverter
    { 
        public string Type
        {
            get
            {
                return "htmlblock"; 
            }
        } 
        public ConvertResponse Convert(RenderContext context, JObject ConvertResult)
        {
            var page = context.GetItem<Page>(); 

            if (page == null || string.IsNullOrEmpty(page.Body) || page.Dom == null)
            {
                return null; 
            }
            string convertname = Lib.Helper.JsonHelper.GetString(ConvertResult, "name"); 
            string converttype = Lib.Helper.JsonHelper.GetString(ConvertResult, "ConvertToType");

            var name = ConvertManager.GetUniqueName(context, converttype, convertname);

            Sites.Contents.Models.HtmlBlock block = new Contents.Models.HtmlBlock();
             block.Name = name;
            string convertvalue = Lib.Helper.JsonHelper.GetString(ConvertResult, "HtmlBody");
            var culture = string.IsNullOrEmpty(context.Culture) ? string.Empty : context.Culture;
            block.SetValue(culture, convertvalue);

            context.WebSite.SiteDb().HtmlBlocks.AddOrUpdate(block);

            return new ConvertResponse()
            {
                 IsSuccess = true,
                 ComponentNameOrId = block.Name,
                  Tag = "<htmlblock id='" + block.Name.ToString() + "'></htmlblock>"
            };  
             
        }

  
 
    }
}
