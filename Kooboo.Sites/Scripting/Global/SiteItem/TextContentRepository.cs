//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Context;
using Kooboo.Data.Interface;
using Kooboo.Sites.Contents.Models;
using Kooboo.Sites.DataSources;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Repository;
using Kooboo.Sites.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;

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

        public void Add(object SiteObject)
        {
            var sitedb = this.context.WebSite.SiteDb();

            var data = kHelper.GetData(SiteObject);

            var culture = this.context.Culture;

            Kooboo.Sites.Contents.Models.TextContent content = new Contents.Models.TextContent();

            ContentType type = null;
            ContentFolder folder = null;
            Dictionary<string, string> initValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);


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
                    type = sitedb.ContentTypes.Get(lower);
                    if (type != null)
                    {
                        content.ContentTypeId = type.Id;
                        folder = sitedb.ContentFolders.All().Find(o => o.ContentTypeId == type.Id);
                        if (folder != null)
                        {
                            content.FolderId = folder.Id;
                            continue;
                        }
                    }
                }
                else if (lower == "contentfolder" || lower == "folder")
                {
                    folder = sitedb.ContentFolders.Get(item.Value.ToString());
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

                // content.SetValue(item.Key, item.Value.ToString(), culture);

                initValues.Add(item.Key, item.Value.ToString());

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

            // prepare and set values... 
            if (type == null && folder != null)
            {
                type = sitedb.ContentTypes.Get(folder.ContentTypeId);
            } 

            PrepareSetValues(initValues, content, culture, type, folder, sitedb);

            sitedb.TextContent.AddOrUpdate(content);
        }

        private void PrepareSetValues(Dictionary<string, string> initdata, TextContent content, string culture, ContentType type, ContentFolder folder, SiteDb siteDb)
        {
            List<EmbeddedBy> embeddedby = null;

            Dictionary<string, string> nonfields = new Dictionary<string, string>(); 

            foreach (var item in initdata)
            {
                if (type.Properties.Find(o => Lib.Helper.StringHelper.IsSameValue(o.Name, item.Key)) != null)
                {
                    content.SetValue(item.Key, item.Value, culture);
                }
                else
                {
                    nonfields.Add(item.Key, item.Value);  
                }
            }

            siteDb.TextContent.EnsureUserKey(content); 

            foreach (var item in nonfields)
            {
                Guid ValueId = Lib.Helper.IDHelper.GetOrParseKey(item.Value);

                var category = GetCatFolder(item.Key); 
                if (category != null)
                {
                    var catitem = siteDb.TextContent.Get(ValueId); 
                    if (catitem !=null)
                    {
                        siteDb.ContentCategories.AddOrUpdate(new ContentCategory() { ContentId = content.Id, CategoryFolder = category.FolderId, CategoryId = ValueId });
                    } 
                }
                else
                {
                    var by = GetBY();

                    if (by.Any())
                    {
                        var beEmbedded = by.Find(o => Lib.Helper.StringHelper.IsSameValue(o.FolderName, item.Key));

                        if (beEmbedded == null && (item.Key.ToLower() == "parent" || item.Key.ToLower() == "parentid"))
                        {
                            beEmbedded = by.First();
                        }
                        if (beEmbedded != null)
                        {
                            var parentcontent = siteDb.TextContent.Get(ValueId);
                            if (parentcontent != null)
                            {
                                if (parentcontent.Embedded.ContainsKey(folder.Id))
                                {
                                    var list = parentcontent.Embedded[folder.Id];
                                    if (!list.Contains(content.Id))
                                    {
                                        list.Add(content.Id);
                                    }
                                }
                                else
                                {
                                    List<Guid> ids = new List<Guid>();
                                    ids.Add(content.Id);
                                    parentcontent.Embedded[folder.Id] = ids;
                                }

                                siteDb.TextContent.AddOrUpdate(parentcontent);
                            }
                        }
                    }
                }
            }


            List<EmbeddedBy> GetBY()
            {
                if (embeddedby == null)
                {
                    embeddedby = siteDb.ContentFolders.GetEmbeddedBy(folder.Id);
                }
                return embeddedby;
            } 

            CategoryFolder GetCatFolder(string foldername)
            {
                var category = folder.Category.Find(o => Lib.Helper.StringHelper.IsSameValue(o.Alias, foldername));
                if (category !=null)
                {
                    return category; 
                }
                else
                {
                    foreach (var item in folder.Category)
                    {
                        var catfolder = siteDb.ContentFolders.Get(item.FolderId); 
                        if (catfolder !=null)
                        {
                            if (Lib.Helper.StringHelper.IsSameValue(catfolder.Name, foldername))
                            {
                                return item; 
                            }
                        }
                    }
                }
                return null; 
            }
        }

        public void Update(object SiteObject)
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

        public void Delete(object nameOrId)
        {

            var key = Lib.Helper.IDHelper.ParseKey(nameOrId);

            var content = this.context.WebSite.SiteDb().TextContent.Get(key);

            if (content != null)
            {
                this.context.WebSite.SiteDb().TextContent.Delete(content.Id);
            }

        }


        public List<TextContentObject> All()
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
            ContentFolder onlyFolder = null;

            var condition = ParseCondition(queryCondition);

            var tablequery = sitedb.TextContent.Query.Where(o => o.Online == true);

            if (condition.FolderId != default(Guid))
            {
                tablequery.Where(o => o.FolderId == condition.FolderId);

                var folder = sitedb.ContentFolders.Get(condition.FolderId);
                if (folder != null)
                {
                    onlyFolder = folder;
                    onlyType = sitedb.ContentTypes.Get(folder.ContentTypeId);
                }

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
            return filterItems(all, condition.Conditions, onlyType, onlyFolder);
        }

        internal List<TextContent> filterItems(List<TextContent> input, List<IndexedDB.Dynamic.ConditionItem> conditions, ContentType type, ContentFolder folder)
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
                    if (CheckItem(item, conditions, contenttype, folder))
                    {
                        result.Add(item);
                    }
                }
            }

            return result;
        }

        public bool CheckItem(TextContent TextContent, List<IndexedDB.Dynamic.ConditionItem> conditions, ContentType contenttype, ContentFolder folder = null)
        {

            foreach (var item in conditions)
            {
                var col = contenttype.Properties.Find(o => o.Name == item.Field);

                if (col != null)
                {
                    var clrtype = Data.Helper.DataTypeHelper.ToClrType(col.DataType);

                    if (col.MultipleLanguage)
                    {
                        bool isPassed = false;
                        foreach (var cul in this.context.WebSite.Culture)
                        {
                            var value = TextContent.GetValue(col.Name, cul.Key);

                            if (value != null && FilterHelper.Check(value.ToString(), ToFilterCompare(item.Comparer), item.Value, clrtype))
                            {
                                isPassed = true;
                                break;
                            }
                        }
                        return isPassed;
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
                    // TODO: Should check for category fields...   
                    ///----------------------------------------- 
                    if (folder != null)
                    {
                        //check category.  
                        string fieldname = item.Field;
                        string subfieldname = null;
                        var category = folder.Category.Find(o => o.Alias == fieldname);
                        if (category == null && fieldname.IndexOf(".") > -1)
                        {
                            var partone = fieldname.Substring(0, fieldname.IndexOf("."));
                            category = folder.Category.Find(o => o.Alias == partone);
                            if (category != null)
                            {
                                subfieldname = fieldname.Substring(fieldname.IndexOf(".") + 1);
                            }
                        }

                        if (category != null)
                        {
                            var ids = this.context.WebSite.SiteDb().ContentCategories.Query.Where(o => o.ContentId == TextContent.Id && o.CategoryFolder == category.FolderId).SelectAll().Select(o => o.CategoryId).ToList();

                            foreach (var catid in ids)
                            {
                                var catcontent = this.context.WebSite.SiteDb().TextContent.Get(catid);

                                if (catcontent != null)
                                {
                                    if (subfieldname != null)
                                    {
                                        var value = catcontent.GetValue(subfieldname);
                                        if (value != null)
                                        {
                                            if (FilterHelper.Check(value.ToString(), ToFilterCompare(item.Comparer), item.Value))
                                            {
                                                return true;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        var value = catcontent.GetValue("UserKey");
                                        if (value != null)
                                        {
                                            if (FilterHelper.Check(value.ToString(), ToFilterCompare(item.Comparer), item.Value, typeof(string)))
                                            {
                                                return true;
                                            }
                                        }

                                        value = catcontent.GetValue("Id");
                                        if (value != null)
                                        {
                                            if (FilterHelper.Check(value.ToString(), ToFilterCompare(item.Comparer), item.Value, typeof(Guid)))
                                            {
                                                return true;
                                            }
                                        }


                                        value = catcontent.GetValue("name");
                                        if (value != null)
                                        {
                                            if (FilterHelper.Check(value.ToString(), ToFilterCompare(item.Comparer), item.Value, typeof(Guid)))
                                            {
                                                return true;
                                            }
                                        }



                                    }

                                }

                            }

                        }

                    }

                    ///----------------


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
