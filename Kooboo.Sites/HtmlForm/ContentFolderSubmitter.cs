//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kooboo.Data.Context;
using Kooboo.Data.Models;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Contents.Models;

namespace Kooboo.Sites.HtmlForm
{
    public class ContentFolderSubmitter : Data.Interface.IFormSubmitter
    {
        public string Name
        {
            get
            {
                return "ContentFolder"; 
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
            foldersetting.Name = "ContentFolder";
            foldersetting.ControlType = Data.ControlType.Selection; 
            var folders = context.WebSite.SiteDb().ContentFolders.All();
            foreach (var item in folders)
            {
                foldersetting.SelectionValues.Add(item.Id.ToString(), item.Name); 
            }
            setting.Add(foldersetting); 
            return setting; 
        }

        public bool Submit(RenderContext context, Guid FormId, Dictionary<string, string> settings)
        {
           var sitedb = context.WebSite.SiteDb();
             
            Guid ContentFolderId = default(Guid);
            foreach (var item in settings)
            {
                if (item.Key.ToLower().Contains("folder"))
                {
                    var value = item.Value; 
                    if (System.Guid.TryParse(value, out ContentFolderId))
                    {
                        break;
                    }
                }
            }

            if (ContentFolderId != default(Guid))
            {
                string culture = context.Culture; 

                TextContent content = new TextContent();
                content.FolderId = ContentFolderId;

                 if (context.Request.Forms.Count>0)
                {
                    foreach (var item in context.Request.Forms.AllKeys)
                    {
                        if (!item.StartsWith("_kooboo"))
                        {
                            var itemvalue = context.Request.Forms[item];
                            content.SetValue(item, itemvalue, culture);
                        }
                    }
                }
                 else if (context.Request.QueryString.Count>0)
                {
                    foreach (var item in context.Request.QueryString.AllKeys)
                    {
                        if (!item.StartsWith("_kooboo"))
                        {
                            var itemvalue = context.Request.QueryString[item];
                            content.SetValue(item, itemvalue, culture);
                        }
                    }
                }

                sitedb.TextContent.AddOrUpdate(content);
                return true;
            }

            return false; 
        
        }
    }
}
