//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Data.Context;
using Kooboo.Data.Interface;
using Kooboo.Sites.Contents.Models;
using Kooboo.Sites.Extensions;
using System.Collections.Generic;
using System.Linq;

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

            foreach (var item in updatelist.GroupBy(o => o.NameOrId))
            {
                ExecuteObject(context, null, item.Key, item.ToList());
            }
        }

        public void ExecuteObject(RenderContext context, IRepository repo, string nameOrId, List<IInlineModel> updates)
        {
            var sitedb = context.WebSite.SiteDb();
            string culture = context.Culture;
            if (string.IsNullOrEmpty(culture))
            {
                culture = context.WebSite.DefaultCulture;
            }
            //  var repo = context.WebSite.SiteDb().TextContents(culture);
            var contentmodels = updates.Cast<Model.ContentModel>().ToList();
            if (contentmodels.Any(o => o.Action == ActionType.Delete))
            {
                if (sitedb.TextContent.GetByNameOrId(nameOrId) is TextContent textcontent)
                {
                    sitedb.TextContent.Delete(textcontent.Id);
                }
                return;
            }
            else if (contentmodels.Any(o => o.Action == ActionType.Copy))
            {
                var orgitem = contentmodels.Find(o => !string.IsNullOrEmpty(o.OrgNameOrId));
                if (orgitem != null)
                {
                    string oldnameorid = orgitem.OrgNameOrId;
                    var oldcontent = sitedb.TextContent.GetByNameOrId(oldnameorid);
                    if (oldcontent != null)
                    {
                        TextContent newcontent = new TextContent
                        {
                            FolderId = oldcontent.FolderId,
                            ContentTypeId = oldcontent.ContentTypeId,
                            Contents = oldcontent.Contents,
                            UserKey = nameOrId
                        };
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
                            var newcat = new ContentCategory
                            {
                                CategoryFolder = item.CategoryFolder,
                                CategoryId = item.CategoryId,
                                ContentId = newcontent.Id
                            };
                            sitedb.ContentCategories.AddOrUpdate(newcat, context.User.Id);
                        }
                    }
                }
            }
            else if (contentmodels.Any(o => o.Action == ActionType.Add))
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
                var content = sitedb.TextContent.GetByNameOrId(nameOrId);
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