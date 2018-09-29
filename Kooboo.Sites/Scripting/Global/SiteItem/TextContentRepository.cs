//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Context;
using Kooboo.Data.Interface;
using Kooboo.Sites.Contents.Models;
using Kooboo.Sites.DataSources;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Sites.Scripting.Global.SiteItem
{

    public class TextContentObjectRepository 
    {
        public RenderContext context { get; set; }

        public IRepository repo { get; set; }
        public TextContentObjectRepository(IRepository repo, RenderContext context)
        {
            this.context = context;
            this.repo = repo;
        }

        public   void Add(object SiteObject)
        {
            var sitedb = this.context.WebSite.SiteDb();

            var data = kHelper.GetData(SiteObject);

            var culture = this.context.Culture;

            Kooboo.Sites.Contents.Models.TextContent content = new Contents.Models.TextContent();

            foreach (var item in data)
            {
                var lower = item.Key.ToLower();

                if (lower == "userkey")
                {
                    content.UserKey = item.Value.ToString();
                    continue;
                }
                else if (lower == "id")
                {
                    if (Guid.TryParse(lower, out Guid Id))
                    {
                        content.Id = Id;
                        continue;
                    }
                }
                else if (lower == "Order" || lower == "sequence")
                {
                    int value;
                    if (int.TryParse(item.Value.ToString(), out value))
                    {
                        content.Order = value;
                        continue;
                    }
                }
                else if (lower == "contenttype")
                {
                    var type = sitedb.ContentTypes.Get(lower);
                    if (type != null)
                    {
                        content.ContentTypeId = type.Id;
                        var folder = sitedb.ContentFolders.All().Find(o => o.ContentTypeId == type.Id);
                        if (folder != null)
                        {
                            content.FolderId = folder.Id;
                            continue;
                        }
                    }
                }
                else if (lower == "contentfolder" || lower == "folder")
                {
                    var folder = sitedb.ContentFolders.Get(item.Value.ToString());
                    if (folder != null)
                    {
                        content.FolderId = folder.Id;
                        content.ContentTypeId = folder.ContentTypeId;
                        continue;
                    }
                }

                else if (lower == "online")
                {
                    if (bool.TryParse(lower, out bool isonline))
                    {
                        content.Online = isonline;
                        continue;
                    }
                }

                content.SetValue(item.Key, item.Value.ToString(), culture);

            }

            if (content.ContentTypeId == default(Guid) && content.FolderId == default(Guid))
            {
                throw new Exception(Kooboo.Data.Language.Hardcoded.GetValue("Content folder undefined"));
            }

            if (content.FolderId == default(Guid))
            {
                var folders = sitedb.ContentFolders.Query.Where(o => o.ContentTypeId == content.ContentTypeId).FirstOrDefault();
                if (folders != null)
                {
                    content.ContentTypeId = folders.ContentTypeId;
                }
                else
                {
                    throw new Exception(Kooboo.Data.Language.Hardcoded.GetValue("Content folder not found"));
                }
            }

            sitedb.TextContent.AddOrUpdate(content);
        }

        public   void Update(object SiteObject)
        {
            if (SiteObject is TextContentObject)
            {
                var obj = SiteObject as TextContentObject;
                this.context.WebSite.SiteDb().TextContent.AddOrUpdate(obj.TextContent);
            }
        }

        public TextContentObject Get(object nameOrId)
        {
            var key = Lib.Helper.IDHelper.ParseKey(nameOrId);

            var content = this.context.WebSite.SiteDb().TextContent.Get(key);

            if (content != null)
            {
                return new TextContentObject(content, this.context);
            }
            return null;
        }

        public   void Delete(object nameOrId)
        {

            var key = Lib.Helper.IDHelper.ParseKey(nameOrId);

            var content = this.context.WebSite.SiteDb().TextContent.Get(key);

            if (content != null)
            {
                this.context.WebSite.SiteDb().TextContent.Delete(content.Id);
            }

        }


        public   List<TextContentObject> All()
        {
            List<TextContentObject> result = new List<TextContentObject>();

            var allcontents = this.context.WebSite.SiteDb().TextContent.All();

            foreach (var item in allcontents)
            {
                TextContentObject model = new TextContentObject(item, this.context);
                result.Add(model);
            }
            return result;
        }

        public TextContentObject Find(string query)
        {
            // todo: improve the performance.
            var all = FindAll(query);
            if (all != null && all.Count() > 0)
            {
                return all.First();
            }
            return null;
        }

        public List<TextContentObject> FindAll(string query)
        {
            var all = _findAll(query);
            List<TextContentObject> result = new List<TextContentObject>();
            foreach (var item in all)
            {
                var txtObj = new TextContentObject(item, this.context);
                result.Add(txtObj);
            }
            return result.OrderByDescending(o => o.TextContent.LastModified).ToList();
        }

        private List<TextContent> _findAll(string queryCondition)
        {
            var sitedb = this.context.WebSite.SiteDb();

            var allContentTypes = sitedb.ContentTypes.All();

            ContentType onlyType = null;

            var condition = ParseCondition(queryCondition);

            var tablequery = sitedb.TextContent.Query.Where(o => o.Online == true);

            if (condition.FolderId != default(Guid))
            {
                tablequery.Where(o => o.FolderId == condition.FolderId);
                onlyType = sitedb.ContentTypes.GetByFolder(condition.FolderId);
            }

            else if (condition.ContentTypeId != default(Guid))
            {
                tablequery.Where(o => o.ContentTypeId == condition.ContentTypeId);
                var type = sitedb.ContentTypes.Get(condition.ContentTypeId);
                if (type != null)
                {
                    onlyType = type;
                }
            }

            // var allcontentids = this.Context.SiteDb.ContentCategories.Query.Where(o => o.CategoryId == CategoryId).SelectAll().Select(o => o.ContentId).ToList();    
            //    var categoryContentquery = this.Context.SiteDb.TextContent.Query.Where(o => o.FolderId == ContentFolderId).WhereIn("Id", allcontentids);

            if (condition.CategoryId != default(Guid))
            {
                var allcontentids = sitedb.ContentCategories.Query.Where(o => o.CategoryId == condition.CategoryId).SelectAll().Select(o => o.ContentId).ToList();

                tablequery.WhereIn("Id", allcontentids);        
            }       
            
            var all = tablequery.SelectAll();
            return filterItems(all, condition.Conditions, onlyType);
        }

        internal List<TextContent> filterItems(List<TextContent> input, List<IndexedDB.Dynamic.ConditionItem> conditions, ContentType type = null)
        {
            List<TextContent> result = new List<TextContent>();

            List<ContentType> alltypes = null;
            if (type == null)
            {
                alltypes = this.context.WebSite.SiteDb().ContentTypes.All();
            }

            foreach (var item in input)
            {
                ContentType contenttype = type;
                if (contenttype == null)
                {
                    contenttype = alltypes.Find(o => o.Id == item.ContentTypeId);
                }

                if (contenttype != null)
                {
                    if (CheckItem(item, conditions, contenttype))
                    {
                        result.Add(item);
                    }
                }
            }

            return result;
        }

        public bool CheckItem(TextContent TextContent, List<IndexedDB.Dynamic.ConditionItem> conditions, ContentType contenttype)
        {

            foreach (var item in conditions)
            {
                var col = contenttype.Properties.Find(o => o.Name == item.Field);

                if (col != null)
                {
                    var clrtype = Data.Helper.DataTypeHelper.ToClrType(col.DataType);

                    if (col.MultipleLanguage)
                    {
                        foreach (var cul in this.context.WebSite.Culture)
                        {
                            var value = TextContent.GetValue(col.Name, cul.Key);

                            if (value != null && FilterHelper.Check(value.ToString(), ToFilterCompare(item.Comparer), item.Value, clrtype))
                            {
                                continue;
                            }
                        }

                        return false;
                    }
                    else
                    {
                        var value = TextContent.GetValue(col.Name);
                        if (value != null)
                        {
                            if (FilterHelper.Check(value.ToString(), ToFilterCompare(item.Comparer), item.Value, clrtype))
                            {
                                continue;
                            }
                            else
                            {
                                return false;
                            }
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    return false;
                }
            }

            return true;

        }


        private Kooboo.Data.Definition.Comparer ToFilterCompare(Kooboo.IndexedDB.Query.Comparer input)
        {
            switch (input)
            {
                case IndexedDB.Query.Comparer.EqualTo:
                    return Data.Definition.Comparer.EqualTo;
                case IndexedDB.Query.Comparer.GreaterThan:
                    return Data.Definition.Comparer.GreaterThan;
                case IndexedDB.Query.Comparer.GreaterThanOrEqual:
                    return Data.Definition.Comparer.GreaterThanOrEqual;
                case IndexedDB.Query.Comparer.LessThan:
                    return Data.Definition.Comparer.LessThan;
                case IndexedDB.Query.Comparer.LessThanOrEqual:
                    return Data.Definition.Comparer.LessThanOrEqual;
                case IndexedDB.Query.Comparer.NotEqualTo:
                    return Data.Definition.Comparer.NotEqualTo;
                case IndexedDB.Query.Comparer.StartWith:
                    return Data.Definition.Comparer.StartWith;
                case IndexedDB.Query.Comparer.Contains:
                    return Data.Definition.Comparer.Contains;
                default:
                    return Data.Definition.Comparer.EqualTo;
            }
        }


        public FindCondition ParseCondition(string query)
        {
            var conditions = Kooboo.IndexedDB.Dynamic.QueryPraser.ParseConditoin(query);

            Guid FolderId = default(Guid);
            Guid ContentTypeId = default(Guid);
            Guid CategoryId = default(Guid);


            var folderidcon = conditions.Find(o => o.Field.ToLower() == "folderid");

            if (folderidcon != null)
            {
                conditions.Remove(folderidcon);

                Guid.TryParse(folderidcon.Value, out FolderId);
            }

            if (FolderId == default(Guid))
            {
                var foldername = conditions.Find(o => o.Field.ToLower() == "folder");
                if (foldername != null)
                {
                    var folder = this.context.WebSite.SiteDb().ContentFolders.Get(foldername.Value);
                    if (folder != null)
                    {
                        conditions.Remove(foldername);
                        FolderId = folder.Id;
                    }
                }
            }

            if (FolderId == default(Guid))
            {
                var typecon = conditions.Find(o => o.Field.ToLower() == "contenttypeid");

                if (typecon != null)
                {
                    conditions.Remove(typecon);
                    Guid.TryParse(typecon.Value, out ContentTypeId);
                }

                if (ContentTypeId == default(Guid))
                {
                    var typename = conditions.Find(o => o.Field.ToLower() == "contenttype");
                    if (typename != null)
                    {
                        var type = this.context.WebSite.SiteDb().ContentTypes.Get(typename.Value);
                        if (type != null)
                        {
                            conditions.Remove(typename);

                            ContentTypeId = type.Id;
                        }
                    }
                }
            }



            var catname = conditions.Find(o => o.Field.ToLower() == "category");
            if (catname != null)
            {
                var cat = this.context.WebSite.SiteDb().TextContent.Get(catname.Value);
                if (cat != null)
                {
                    conditions.Remove(catname);
                    CategoryId = cat.Id;
                }
            }



            FindCondition result = new FindCondition();
            result.FolderId = FolderId;
            result.ContentTypeId = ContentTypeId;
            result.CategoryId = CategoryId; 
            result.Conditions = conditions;
            return result;
        }


        public TextContentQuery Query(string searchCondition)
        {
            var result = new TextContentQuery(this);
            result.Where(searchCondition);
            return result;
        }

        //internal List<TextContentViewModel> ByCategory(Guid CategoryId, Guid ContentFolderId, List<FilterDefinition> Filters, string SortField, Boolean IsAscending)
        //{
        //    var allcontentids = this.Context.SiteDb.ContentCategories.Query.Where(o => o.CategoryId == CategoryId).SelectAll().Select(o => o.ContentId).ToList();

        //    var categoryContentquery = this.Context.SiteDb.TextContent.Query.Where(o => o.FolderId == ContentFolderId).WhereIn("Id", allcontentids);

        //    if (this.IsDefault)
        //    {
        //        categoryContentquery.Where(o => o.Online == true);
        //    }

        //    var allorgcontents = categoryContentquery.SelectAll();

        //    var props = Context.SiteDb.ContentTypes.GetPropertiesByFolder(ContentFolderId);

        //    SetSortField(ref SortField, props);

        //    var allcontents = Helper.ContentHelper.ToViews(allorgcontents, this.Context.RenderContext.Culture, props);

        //    return SortFilterContentViews(allcontents, ContentFolderId, Filters, SortField, IsAscending); 
        //}


    }

    public class FindCondition
    {
        public Guid FolderId { get; set; }

        public Guid ContentTypeId { get; set; }

        public Guid CategoryId { get; set; }

        public List<IndexedDB.Dynamic.ConditionItem> Conditions { get; set; }
    }






}
