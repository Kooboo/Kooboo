//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Attributes;
using Kooboo.Data.Context;
using Kooboo.Data.Interface;
using Kooboo.Sites.Contents.Models;
using Kooboo.Sites.DataSources;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Repository;
using Kooboo.Sites.Scripting.Global;
using Kooboo.Sites.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;


namespace KScript.Sites
{

    public class TextContentObjectRepository
    {
        [KIgnore]
        public RenderContext context { get; set; }
        [KIgnore]
        public IRepository repo { get; set; }
        public TextContentObjectRepository(IRepository repo, RenderContext context)
        {
            this.context = context;
            this.repo = repo;
        }

        [Description(@"Add a text content into content repository. Folder is a required.
        var obj = {fieldone: ""value one"", fieldtwo: ""value two""};
        obj.folder = ""blogfolder""; 
        k.site.textContents.add(obj); 

        //To add relation data. For example, comment is embedded by blog.
         var commentItem = {content: ""very nice article""};
    commentItem.folder = ""comment""; 
        commentItem.blog = ""blogArticlekey"";  
        k.site.textContents.add(commentItem);

        //To add relation data. For example,  blog has a catgory.
        var obj = { fieldone: ""value one"", fieldtwo: ""value two"" };
    obj.folder = ""blogfolder""; 
        obj.catalias= ""categorykey""; 
        k.site.textContents.add(obj);")] 
        public void Add(object SiteObject)
        {
            var sitedb = this.context.WebSite.SiteDb();

            var data = kHelper.GetData(SiteObject);

            var culture = this.context.Culture;

            Kooboo.Sites.Contents.Models.TextContent content = new Kooboo.Sites.Contents.Models.TextContent();

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
                if (type.Properties.Find(o => Kooboo.Lib.Helper.StringHelper.IsSameValue(o.Name, item.Key)) != null)
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
                Guid ValueId = Kooboo.Lib.Helper.IDHelper.GetOrParseKey(item.Value);

                var category = GetCatFolder(item.Key);
                if (category != null)
                {
                    var catitem = siteDb.TextContent.Get(ValueId);
                    if (catitem != null)
                    {
                        siteDb.ContentCategories.AddOrUpdate(new ContentCategory() { ContentId = content.Id, CategoryFolder = category.FolderId, CategoryId = ValueId });
                    }
                }
                else
                {
                    var by = GetBY();

                    if (by.Any())
                    {
                        var beEmbedded = by.Find(o => Kooboo.Lib.Helper.StringHelper.IsSameValue(o.FolderName, item.Key));

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
                                    if (list == null)
                                    {
                                        list = new List<Guid>();
                                    }
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
                var category = folder.Category.Find(o => Kooboo.Lib.Helper.StringHelper.IsSameValue(o.Alias, foldername));
                if (category != null)
                {
                    return category;
                }
                else
                {
                    foreach (var item in folder.Category)
                    {
                        var catfolder = siteDb.ContentFolders.Get(item.FolderId);
                        if (catfolder != null)
                        {
                            if (Kooboo.Lib.Helper.StringHelper.IsSameValue(catfolder.Name, foldername))
                            {
                                return item;
                            }
                        }
                    }
                }
                return null;
            }
        }


        [Description(@"update a text content values.
          var item = k.site.textContents.get(""titletwo"");
          item.title = ""new value""; 
          k.site.textContents.update(item); ")] 
        public void Update(object textContent)
        {
            if (textContent is TextContentObject)
            {
                var obj = textContent as TextContentObject;
                var sitedb = this.context.WebSite.SiteDb();

                sitedb.TextContent.AddOrUpdate(obj.TextContent);

                if (obj.AdditionalValues.Any())
                {
                    // handle additioanl values... like blog...but how to remove then?  
                    ContentFolder folder = sitedb.ContentFolders.Get(obj.TextContent.FolderId);
                    if (folder != null)
                    {
                        UpdateAdditional(obj.AdditionalValues, obj.TextContent, folder, sitedb);
                    }
                }
            }
        }


        private void UpdateAdditional(Dictionary<string, string> additionalData, TextContent content, ContentFolder folder, SiteDb siteDb)
        {
            List<EmbeddedBy> embeddedby = null;

            foreach (var item in additionalData)
            {
                Guid ValueId = default(Guid);
                if (!String.IsNullOrWhiteSpace(item.Value))
                {
                    ValueId = Kooboo.Lib.Helper.IDHelper.GetOrParseKey(item.Value);
                }

                var category = GetCatFolder(item.Key);
                if (category != null)
                {
                    if (ValueId == default(Guid))
                    {
                        //To remove item..
                        var currentItems = siteDb.ContentCategories.GetCategories(category.FolderId, content.Id);
                        foreach (var cat in currentItems)
                        {
                            siteDb.ContentCategories.Delete(cat.Id);
                        }
                    }
                    else
                    {
                        var catitem = siteDb.TextContent.Get(ValueId);
                        if (catitem != null)
                        {
                            siteDb.ContentCategories.AddOrUpdate(new ContentCategory() { ContentId = content.Id, CategoryFolder = category.FolderId, CategoryId = ValueId });
                        }
                    }
                }
                else
                {
                    var by = GetBY();

                    if (by.Any())
                    {
                        var beEmbedded = by.Find(o => Kooboo.Lib.Helper.StringHelper.IsSameValue(o.FolderName, item.Key));

                        if (beEmbedded == null && (item.Key.ToLower() == "parent" || item.Key.ToLower() == "parentid"))
                        {
                            beEmbedded = by.First();
                        }
                        if (beEmbedded != null)
                        {
                            if (ValueId != default(Guid))
                            {
                                var parentcontent = siteDb.TextContent.Get(ValueId);
                                if (parentcontent != null)
                                {
                                    if (parentcontent.Embedded.ContainsKey(folder.Id))
                                    {
                                        var list = parentcontent.Embedded[folder.Id];
                                        if (list == null)
                                        {
                                            list = new List<Guid>();
                                        }
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
                            else
                            {
                                // TODO: 
                                // this is to remove one item from the embedded...This is not happening often.
                                // only we need to find out which item embed this one which is not easy... as we need to check every items. 
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
                var category = folder.Category.Find(o => Kooboo.Lib.Helper.StringHelper.IsSameValue(o.Alias, foldername));
                if (category != null)
                {
                    return category;
                }
                else
                {
                    foreach (var item in folder.Category)
                    {
                        var catfolder = siteDb.ContentFolders.Get(item.FolderId);
                        if (catfolder != null)
                        {
                            if (Kooboo.Lib.Helper.StringHelper.IsSameValue(catfolder.Name, foldername))
                            {
                                return item;
                            }
                        }
                    }
                }
                return null;
            }
        }


        [Description(@"Get a text content object based on Id or UserKey
var item = k.site.textContents.get(""titletwo"");")]
        public TextContentObject Get(object nameOrId)
        {
            var key = Kooboo.Lib.Helper.IDHelper.ParseKey(nameOrId);

            var content = this.context.WebSite.SiteDb().TextContent.Get(key);

            if (content != null)
            {
                return new TextContentObject(content, this.context);
            }
            return null;
        }
         

        [Description(@"Delete an item based on id or userkey
  k.site.textContents.delete(""userkey""); ")]
        public void Delete(object nameOrId)
        {

            var key = Kooboo.Lib.Helper.IDHelper.ParseKey(nameOrId);

            var content = this.context.WebSite.SiteDb().TextContent.Get(key);

            if (content != null)
            {
                this.context.WebSite.SiteDb().TextContent.Delete(content.Id);
            }
        }



        [Description(@"Return an array of all TextContentObjects
 var list= k.site.textContents.all();")]
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

        [Description(@"find the first matched items based on search condition
//available operators: ==,  >=,  >,  <,  <=, contains, startwith 
        var item= k.site.textContents.find(""name == 'matchedvalue'""); 
        var item= k.site.textContents.find(""number>=123""); 
        var item= k.site.textContents.find(""number >=123&&name=='matchedvalue'""); 
        var item= k.site.textContents.find(""name contains 'matchedvalue'""); 
        var item= k.site.textContents.find(""name startwith 'matchedvalue'""); 
        // you may use the condition of ""folder"", ""contenttype"" and ""category"".
        var bloglist = k.site.textContents.find(""folder ==blog"");")]
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

        [Description(@"Search text contents based on query condition
       // available operators: ==,  >=,  >,  <,  <=, contains, startwith 
        var items = k.site.textContents.findAll(""name == 'matchedvalue'""); 
        var items = k.site.textContents.findAll(""number>=123""); 
        var items = k.site.textContents.findAll(""number >=123&&name=='matchedvalue'""); 
        var items = k.site.textContents.findAll(""name contains 'matchedvalue'""); 
        var items = k.site.textContents.findAll(""name startwith 'matchedvalue'""); 
        // you may use the condition of ""folder"", ""contenttype"" and ""category"".
        var bloglist = k.site.textContents.findAll(""folder ==blog"");")]
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

        [KIgnore]
        internal List<TextContent> filterItems(List<TextContent> input, List<Kooboo.IndexedDB.Dynamic.ConditionItem> conditions, ContentType type, ContentFolder folder)
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

        [KIgnore]
        internal bool CheckItem(TextContent TextContent, List<Kooboo.IndexedDB.Dynamic.ConditionItem> conditions, ContentType contenttype, ContentFolder folder = null)
        {

            foreach (var item in conditions)
            {
                var col = contenttype.Properties.Find(o => o.Name == item.Field);

                if (col != null)
                {
                    var clrtype = Kooboo.Data.Helper.DataTypeHelper.ToClrType(col.DataType);

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
                case Kooboo.IndexedDB.Query.Comparer.EqualTo:
                    return Kooboo.Data.Definition.Comparer.EqualTo;
                case Kooboo.IndexedDB.Query.Comparer.GreaterThan:
                    return Kooboo.Data.Definition.Comparer.GreaterThan;
                case Kooboo.IndexedDB.Query.Comparer.GreaterThanOrEqual:
                    return Kooboo.Data.Definition.Comparer.GreaterThanOrEqual;
                case Kooboo.IndexedDB.Query.Comparer.LessThan:
                    return Kooboo.Data.Definition.Comparer.LessThan;
                case Kooboo.IndexedDB.Query.Comparer.LessThanOrEqual:
                    return Kooboo.Data.Definition.Comparer.LessThanOrEqual;
                case Kooboo.IndexedDB.Query.Comparer.NotEqualTo:
                    return Kooboo.Data.Definition.Comparer.NotEqualTo;
                case Kooboo.IndexedDB.Query.Comparer.StartWith:
                    return Kooboo.Data.Definition.Comparer.StartWith;
                case Kooboo.IndexedDB.Query.Comparer.Contains:
                    return Kooboo.Data.Definition.Comparer.Contains;
                default:
                    return Kooboo.Data.Definition.Comparer.EqualTo;
            }
        }
         
        [KIgnore]
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

        [Description(@"Return the query object for further operations like paging.
use the same query syntax as find or findAll")]
        public TextContentQuery Query(string query)
        {
            var result = new TextContentQuery(this);
            result.Where(query);
            return result;
        }
    }

    public class FindCondition
    {
        public Guid FolderId { get; set; }

        public Guid ContentTypeId { get; set; }

        public Guid CategoryId { get; set; }

        public List<Kooboo.IndexedDB.Dynamic.ConditionItem> Conditions { get; set; }
    } 

}
