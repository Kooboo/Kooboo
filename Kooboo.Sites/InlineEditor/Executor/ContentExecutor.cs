using System;
using System.Collections.Generic;
using System.Linq;
using Kooboo.Data.Context;
using Kooboo.Sites.Extensions;
using Kooboo.Data.Interface;
using Kooboo.Sites.Contents.Models;

namespace Kooboo.Sites.InlineEditor.Executor
{
    public class ContentExecutor : IInlineExecutor
    {
        public string EditorType
        {
            get
            {
                return "content"; 
            }
        }

        public void Execute(RenderContext context, List<IInlineModel> updatelist)
        { 
           // string culture = context.Request.Culture;
            //var repo = context.WebSite.SiteDb().TextContents(culture); 

            foreach (var item in updatelist.GroupBy(o=>o.NameOrId))
            {
                ExecuteObject(context, null, item.Key, item.ToList()); 
            } 
        }

        public void ExecuteObject(RenderContext context, IRepository repo, string NameOrId, List<IInlineModel> updates)
        {
            var sitedb = context.WebSite.SiteDb();
            string culture = context.Culture; 
            if (string.IsNullOrEmpty(culture))
            {
                culture = context.WebSite.DefaultCulture; 
            }
          //  var repo = context.WebSite.SiteDb().TextContents(culture); 
            var contentmodels = updates.Cast<Model.ContentModel>().ToList();
            if (contentmodels.Where(o => o.Action == ActionType.Delete).Any())
            {
                var textcontent = sitedb.TextContent.GetByNameOrId(NameOrId) as TextContent;
                if (textcontent != null)
                {
                    sitedb.TextContent.Delete(textcontent.Id);
                }
                return;
            }

            else if (contentmodels.Where(o => o.Action == ActionType.Copy).Any())
            {
                var orgitem = contentmodels.Find(o => !string.IsNullOrEmpty(o.OrgNameOrId));
                if (orgitem != null)
                {
                    string oldnameorid = orgitem.OrgNameOrId;
                    var oldcontent = sitedb.TextContent.GetByNameOrId(oldnameorid);
                    if (oldcontent != null)
                    {
                        TextContent newcontent = new TextContent();
                        newcontent.FolderId = oldcontent.FolderId;
                        newcontent.ContentTypeId = oldcontent.ContentTypeId;
                        newcontent.Contents = oldcontent.Contents;
                       //newcontent.Embedded = oldcontent.Embedded;  
                        foreach (var item in contentmodels)
                        {
                            if (!string.IsNullOrEmpty((item.FieldName)))
                            {
                                newcontent.SetValue(item.FieldName, item.Value, culture);
                            }
                        }
                        
                        sitedb.TextContent.AddOrUpdate(newcontent, context.User.Id);

                        // category.
                        var allcats = sitedb.ContentCategories.Query.Where(o => o.ContentId == oldcontent.Id).SelectAll();
                        foreach (var item in allcats)
                        {
                            var newcat = new ContentCategory();
                            newcat.CategoryFolder = item.CategoryFolder;
                            newcat.CategoryId = item.CategoryId;
                            newcat.ContentId = newcontent.Id;
                            sitedb.ContentCategories.AddOrUpdate(newcat, context.User.Id);
                        }

                    }
                }
            }

            else if (contentmodels.Where(o => o.Action == ActionType.Add).Any())
            {
                var newcontent = new TextContent();
                foreach (var item in contentmodels)
                {
                    newcontent.SetValue(item.FieldName, item.Value, culture);
                }
                sitedb.TextContent.AddOrUpdate(newcontent, context.User.Id);
            }
            else
            {
                var content = sitedb.TextContent.GetByNameOrId(NameOrId);
                if (content != null)
                {
                    foreach (var item in contentmodels)
                    {
                        content.SetValue(item.FieldName, item.Value, culture);
                    }
                    sitedb.TextContent.AddOrUpdate(content, context.User.Id);
                }
            }
        }
    }
}
