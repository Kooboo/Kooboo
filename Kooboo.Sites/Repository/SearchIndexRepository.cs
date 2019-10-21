//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Interface;
using Kooboo.Sites.Contents.Models;
using System.Collections.Generic;
using System;
using System.Linq;
using Kooboo.Data.Context;
using Kooboo.Sites.Models;
using Kooboo.Sites.Extensions;
using Kooboo.Data.Models;
using Kooboo.IndexedDB;
using Kooboo.Search;
using Kooboo.Sites.ViewModel;
using System.Text;

namespace Kooboo.Sites.Repository
{
    public class SearchIndexRepository
    {
        private object _locker = new object();

        private object _loglocker = new object();

        private List<byte> _IndexType;
        private List<byte> IndexType
        {
            get
            {
                if (_IndexType == null)
                {
                    _IndexType = new List<byte>();
                    _IndexType.Add(ConstObjectType.Page);
                    //_IndexType.Add(ConstObjectType.View);
                    _IndexType.Add(ConstObjectType.Layout);
                    _IndexType.Add(ConstObjectType.HtmlBlock);
                    _IndexType.Add(ConstObjectType.TextContent);
                }
                return _IndexType;
            }
        }

        public string Folder { get; set; }

        public SiteDb SiteDb { get; set; }

        public Kooboo.Search.Index IndexData { get; set; }

        public string LogFolder { get; set; }

        public SearchIndexRepository(SiteDb sitedb)
        {
            var orgfolder = Data.AppSettings.GetOrganizationFolder(sitedb.WebSite.OrganizationId);

            string sitename = sitedb.WebSite.Name;
            if (string.IsNullOrEmpty(sitename))
            {
                sitename = sitedb.Id.ToString();
            }
            string sitefolder = System.IO.Path.Combine(orgfolder, sitename);
            this.Folder = System.IO.Path.Combine(sitefolder, "searchindex");
            this.LogFolder = System.IO.Path.Combine(sitefolder, "searchlog");
            Lib.Helper.IOHelper.EnsureDirectoryExists(this.LogFolder);
            Lib.Helper.IOHelper.EnsureDirectoryExists(this.Folder);
            this.IndexData = new Search.Index(this.Folder);
            this.SiteDb = sitedb;
        }

        #region "log" 

        private Sequence<SearchLog> _Log;
        private DateTime _logtime;

        public Sequence<SearchLog> Log
        {
            get
            {
                if (_logtime.DayOfYear != DateTime.Now.DayOfYear)
                {
                    _logtime = default(DateTime);
                    _Log = null;
                }

                if (_Log == null)
                {
                    lock (_locker)
                    {
                        if (_Log == null)
                        {
                            _logtime = DateTime.Now;
                            string logname = System.IO.Path.Combine(this.LogFolder, Lib.Helper.MiscHelper.GetWeekName(_logtime) + ".log");
                            _Log = this.SiteDb.DatabaseDb.GetSequence<SearchLog>(logname);
                        }
                    }
                }
                return _Log;
            }
        }

        public Sequence<SearchLog> GetLog(DateTime weektime)
        {
            var weekname = Lib.Helper.MiscHelper.GetWeekName(weektime);
            return this.GetLog(weekname);
        }

        public Sequence<SearchLog> GetLog(string weekname)
        {
            lock (_loglocker)
            {
                if (string.IsNullOrEmpty(weekname))
                {
                    return this.Log;
                }
                string logname = System.IO.Path.Combine(this.LogFolder, weekname + ".log");
                return this.SiteDb.DatabaseDb.GetSequence<SearchLog>(logname);
            }
        }

        public List<string> GetWeekNames()
        {
            lock (_loglocker)
            {
                return Lib.Helper.MiscHelper.GetFolderWeekFileNames(this.LogFolder);
            }
        }

        public Dictionary<string, int> SearchCount(string WeekName)
        {
            lock (_loglocker)
            {
                Sequence<SearchLog> log = GetLog(WeekName);
                Dictionary<string, int> set = new Dictionary<string, int>();

                foreach (var item in log.GetCollection())
                {
                    if (set.ContainsKey(item.Keywords))
                    {
                        set[item.Keywords] = set[item.Keywords] + 1;
                    }
                    else
                    {
                        set.Add(item.Keywords, 1);
                    }
                }
                Dictionary<string, int> result = new Dictionary<string, int>();
                foreach (var item in set.OrderByDescending(o => o.Value).Take(100))
                {
                    result.Add(item.Key, item.Value);
                }

                return result;
            }
        }

        public List<SearchLog> LastestSearch(int count)
        {
            lock (_loglocker)
            {
                List<SearchLog> result = new List<SearchLog>();

                var currentlog = this.Log;
                var currentweekname = Lib.Helper.MiscHelper.GetWeekName(_logtime);
                var AllWeekNames = this.GetWeekNames();

                int i = 0;

                while (i <= count || currentlog == null)
                {
                    foreach (var item in currentlog.GetCollection())
                    {
                        result.Add(item);
                        i += 1;
                        if (i >= count)
                        {
                            break;
                        }
                    }

                    if (i >= count)
                    {
                        break;
                    }
                    else
                    {
                        string nextweek = null;
                        bool found = false;
                        foreach (var item in AllWeekNames.OrderByDescending(o => o))
                        {
                            if (found)
                            {
                                nextweek = item;
                                break;
                            }

                            if (item == currentweekname)
                            {
                                found = true;
                            }
                        }

                        if (nextweek != null)
                        {
                            currentlog = this.GetLog(nextweek);
                            currentweekname = nextweek;
                        }
                        else
                        {
                            currentlog = null;
                            break;
                        }
                    }

                }

                return result;
            }
        }

        #endregion


        internal string GetMetaKey(ISiteObject siteobject)
        {
            string metaname = Service.ConstTypeService.GetModelType(siteobject.ConstType).Name;

            return new MetaIndex(metaname, siteobject.Id).ToString();
        }

        private string GetBody(ISiteObject siteobject)
        {
            string body = null;

            if (siteobject is ITextObject)
            {
                var textobject = siteobject as ITextObject;
                body = textobject.Body;
                return Kooboo.Search.Utility.RemoveHtml(body);
            }
            return body;
        }

        private bool ShouldIndex(ISiteObject siteobject)
        {
            return this.IndexType.Contains(siteobject.ConstType);
        }

        public void AddOrUpdate(ISiteObject siteobject)
        {
            if (this.IndexType.Contains(siteobject.ConstType))
            {
                string meta = GetMetaKey(siteobject);
                string body = GetBody(siteobject);
                if (!string.IsNullOrWhiteSpace(body))
                {
                    this.IndexData.AddOrUpdate(meta, body);
                }
                else
                {
                    this.Delete(siteobject);
                }
            }
        }

        public void Delete(ISiteObject siteobject)
        {
            if (this.IndexType.Contains(siteobject.ConstType))
            {
                string meta = GetMetaKey(siteobject);
                IndexData.Delete(meta);
            }
        }

        public List<SearchResult> Search(string keywords, int Top = 20, string HighLightAttr = null, RenderContext context = null)
        {
            if (context == null)
            {
                context = new RenderContext() { WebSite = this.SiteDb.WebSite, Culture = this.SiteDb.WebSite.DefaultCulture };
            }

            if (string.IsNullOrEmpty(keywords))
            {
                return new List<SearchResult>();
            }

            var ids = this.IndexData.FindAll(keywords);

            List<SearchResult> result = new List<SearchResult>();

            int icount = 0;

            foreach (var item in ids)
            {
                var meta = this.IndexData.GetMeta(item);
                if (meta != null)
                {
                    var searchresult = SearchResultConverter.ConvertTo(context, meta);
                    if (searchresult != null)
                    {
                        var find = result.Find(o => o.Id == searchresult.Id);
                        if (find != null)
                        {
                            find.Found.AddRange(searchresult.Found);
                        }
                        else
                        {
                            result.Add(searchresult);
                            icount += 1;
                        }
                    }

                    if (icount >= Top)
                    {
                        break;
                    }

                }
            }

            SetData(result, keywords, context.Culture, HighLightAttr);

            this.Log.Add(new SearchLog() { IP = context.Request.IP, Keywords = keywords, Time = DateTime.Now, DocFound = ids.Count, ResultCount = result.Count(), Skip = 0 });

            return result;
        }

        public void SetData(List<SearchResult> recordSet, string keywords, string culture, string HightLightTag = null)
        {
            List<string> words = ToWordList(keywords);
            string baseurl = this.SiteDb.WebSite.BaseUrl();

            foreach (var item in recordSet)
            {
                if (!item.Url.ToLower().StartsWith("http://") && !item.Url.ToLower().StartsWith("https://"))
                {
                    item.Url = Lib.Helper.UrlHelper.Combine(baseurl, item.Url);
                }
                // first text content.
                var find = item.Found.Find(o => o.ObjectType.ToLower() == "textcontent");
                if (find != null)
                {
                    var content = this.SiteDb.TextContent.Get(find.ObjectId);
                    if (content != null)
                    {
                        var contentview = Sites.Helper.ContentHelper.ToView(content, culture, SiteDb.ContentTypes.GetColumns(content.ContentTypeId));
                        item.Title = this.HighLight(Helper.ContentHelper.GetTitle(this.SiteDb, contentview), words, HightLightTag);

                        var fulltext = string.Join(" ", contentview.TextValues.Values);

                        item.Summary = this.GetSummary(fulltext, words, HightLightTag, 250);

                        continue;
                    }
                }


                find = item.Found.Find(o => o.ObjectType.ToLower() == "htmlblock");
                if (find != null)
                {
                    var block = this.SiteDb.HtmlBlocks.Get(find.ObjectId);
                    if (block != null)
                    {
                        var textvalue = block.GetValue(culture).ToString();

                        textvalue = Lib.Helper.StringHelper.StripHTML(textvalue);

                        if (!string.IsNullOrWhiteSpace(textvalue))
                        {
                            var textvaluepart = Lib.Helper.StringHelper.SementicSubString(textvalue, 0, 150);

                            item.Title = this.HighLight(textvaluepart, words, HightLightTag);

                            item.Summary = this.GetSummary(textvalue, words, HightLightTag, 280);
                            continue;    
                        }   
                    }
                }

                find = item.Found.Find(o => o.ObjectType.ToLower() == "page");
                if (find != null)
                {
                    var page = this.SiteDb.Pages.Get(find.ObjectId);
                    if (page != null)
                    {
                        string body = Lib.Helper.StringHelper.StripHTML(page.Body);

                        item.Summary = this.GetSummary(body, words, HightLightTag, 280);

                        var title = GetTitle(culture, page);
                        if (string.IsNullOrEmpty(title))
                        {
                            title = Lib.Helper.StringHelper.SementicSubString(item.Summary, 0, 150);
                        }
                        item.Title = HighLight(title, words, HightLightTag);
                        continue;
                    }
                }

                find = item.Found.Find(o => o.ObjectType.ToLower() == "view");
                if (find != null)
                {
                    var view = this.SiteDb.Views.Get(find.ObjectId);

                    if (view != null)
                    {
                        String body = Lib.Helper.StringHelper.StripHTML(view.Body);
                        item.Summary = this.GetSummary(body, words, HightLightTag, 280);
                        string title = Lib.Helper.StringHelper.SementicSubString(body, 0, 150);
                        item.Title = HighLight(title, words, HightLightTag);
                        continue;
                    }
                }

                find = item.Found.Find(o => o.ObjectType.ToLower() == "layout");
                if (find != null)
                {
                    var layout = this.SiteDb.Layouts.Get(find.ObjectId); 

                    if (layout !=null)
                    {
                        String body = Lib.Helper.StringHelper.StripHTML(layout.Body);
                        item.Summary = this.GetSummary(body, words, HightLightTag, 280);
                        string title = Lib.Helper.StringHelper.SementicSubString(body, 0, 150);
                        item.Title = HighLight(title, words, HightLightTag);
                        continue;  
                    }      

                }

            }
        }

        public PagedResult SearchWithPaging(string keywords, int pagesize = 20, int pagenumber = 1, string HighLightAttr = null, RenderContext context = null)
        {
            if (context == null)
            {
                context = new RenderContext() { WebSite = this.SiteDb.WebSite, Culture = this.SiteDb.WebSite.DefaultCulture };
            }
            PagedResult pageresult = new PagedResult();

            if (pagesize <= 0)
            {
                pagesize = 20;
            }

            int skip = (pagenumber - 1) * pagesize;
            if (skip < 0) { skip = 0; }

            var ids = this.IndexData.FindAll(keywords);

            pageresult.PageNumber = pagenumber;
            pageresult.PageSize = pagesize;
            pageresult.TotalCount = ids.Count();
            pageresult.TotalPages = Lib.Helper.CalculationHelper.GetPageCount(pageresult.TotalCount, pagesize);

            List<SearchResult> result = new List<SearchResult>();

            int totalcount = skip + pagesize;
            int icount = 0;

            foreach (var item in ids)
            {
                var meta = this.IndexData.GetMeta(item);
                if (meta != null)
                {
                    var searchresult = SearchResultConverter.ConvertTo(context, meta);
                    if (searchresult != null)
                    {
                        var find = result.Find(o => o.Id == searchresult.Id);
                        if (find != null)
                        {
                            find.Found.AddRange(searchresult.Found);
                        }
                        else
                        {
                            result.Add(searchresult);
                            icount += 1;
                        }
                    }
                }

                if (icount >= totalcount)
                {
                    break;
                }
            }

            var resultset = result.Skip(skip).Take(pagesize).ToList();

            // set summary.     
            SetData(resultset, keywords, context.Culture, HighLightAttr);


            this.Log.Add(new SearchLog() { IP = context.Request.IP, Keywords = keywords, Time = DateTime.Now, DocFound = ids.Count, ResultCount = resultset.Count(), Skip = skip });

            pageresult.DataList = resultset.ToList<object>();
            return pageresult;
        }

        public PagedResult SearchByFolders(List<Guid> FolderIds, string keywords, int pagesize = 20, int pagenumber = 1, string HighLightAttr = null, RenderContext context = null)
        {
            if (context == null)
            {
                context = new RenderContext() { WebSite = this.SiteDb.WebSite, Culture = this.SiteDb.WebSite.DefaultCulture };
            }
            PagedResult pageresult = new PagedResult();

            List<string> words = ToWordList(keywords);
            string baseurl = this.SiteDb.WebSite.BaseUrl();

            if (pagesize <= 0)
            {
                pagesize = 20;
            }

            int skip = (pagenumber - 1) * pagesize;
            if (skip < 0) { skip = 0; }

            var ids = this.IndexData.FindAll(keywords);

            pageresult.PageNumber = pagenumber;
            pageresult.PageSize = pagesize;
            pageresult.TotalCount = ids.Count();
            pageresult.TotalPages = Lib.Helper.CalculationHelper.GetPageCount(pageresult.TotalCount, pagesize);

            List<SearchResult> result = new List<SearchResult>();
            int totalcount = skip + pagesize;
            int icount = 0;

            foreach (var item in ids)
            {
                var meta = this.IndexData.GetMeta(item);
                if (meta != null)
                {
                    var searchresult = SearchResultConverter.ConvertTo(context, meta);

                    if (searchresult != null)
                    {
                        // if already set, 
                        var resultfind = result.Find(o => o.Id == searchresult.Id);
                        if (resultfind != null)
                        {
                            continue;
                        }

                        // try to find textcontent..... 
                        var contentresults = searchresult.Found.FindAll(o => o.ObjectType.ToLower() == "textcontent");
                        if (contentresults == null || !contentresults.Any())
                        {
                            continue;
                        }

                        foreach (var contentresult in contentresults)
                        {
                            var content = this.SiteDb.TextContent.Get(contentresult.ObjectId);

                            if (content != null && FolderIds.Contains(content.FolderId))
                            {
                                var contentview = Sites.Helper.ContentHelper.ToView(content, context.Culture, null);
                                searchresult.Title = this.HighLight(Helper.ContentHelper.GetTitle(this.SiteDb, contentview), words, HighLightAttr);

                                var fulltext = string.Join(" ", contentview.TextValues.Values);

                                searchresult.Summary = this.GetSummary(fulltext, words, HighLightAttr, 250);

                                if (!searchresult.Url.ToLower().StartsWith("http://") && !searchresult.Url.ToLower().StartsWith("https://"))
                                {
                                    searchresult.Url = Lib.Helper.UrlHelper.Combine(baseurl, searchresult.Url);
                                }
                                result.Add(searchresult);

                            }

                        }
                    }
                }

                if (icount >= totalcount)
                {
                    break;
                }
            }

            var resultset = result.Skip(skip).Take(pagesize).ToList();

            this.Log.Add(new SearchLog() { IP = context.Request.IP, Keywords = keywords, Time = DateTime.Now, DocFound = ids.Count, ResultCount = resultset.Count(), Skip = skip });

            pageresult.DataList = resultset.ToList<object>();
            return pageresult;
        }

        private string GetLangTexContentBody(TextContentViewModel model)
        {
            return string.Join(" ", model.TextValues);
        }

        public void Sync(SiteDb SiteDb, ISiteObject Value, ChangeType ChangeType, string StoreName)
        {
            if (ChangeType == ChangeType.Add || ChangeType == ChangeType.Update)
            {
                this.AddOrUpdate(Value);
            }
            else
            {
                this.Delete(Value);
            }
            this.IndexData.Close();
        }

        public void Rebuild()
        {
            //rebuild all index.
            this.DelSelf();
            this.InitIndex();
            this.IndexData.Close();
        }

        internal void InitIndex()
        {
            // all pages. 
            foreach (var item in this.IndexType)
            {
                var repo = this.SiteDb.GetRepository(item);
                foreach (var model in repo.All())
                {
                    this.AddOrUpdate(model);
                }
            }
        }

        public void DelSelf()
        {
            this.IndexData.DelSelf();
        }

        public IndexStat GetIndexStat()
        {
            return this.IndexData.GetIndexStat();
        }


        public string GetSummary(string body, List<string> words, int summaryLen = 250)
        {
            if (body == null)
            {
                return null;
            }

            body = Lib.Helper.StringHelper.StripHTML(body);

            int len = body.Length;
            if (len <= summaryLen)
            {
                return body;
            }

            int i = 0;

            string part = null;

            while (i < len)
            {
                part = Lib.Helper.StringHelper.SementicSubString(body, i, summaryLen);
                foreach (var item in words)
                {
                    if (part.IndexOf(item, StringComparison.OrdinalIgnoreCase) > -1)
                    {
                        goto goout;
                    }
                }
                i += part.Length;
                while (i < len && IsSeperator(body[i]))
                {
                    i = i + 1;
                }
                part = null;
            }

            goout:

            if (part == null)
            {
                part = Lib.Helper.StringHelper.SementicSubString(body, 0, summaryLen);
            }

            return part;
        }



        public string GetSummary(string body, List<string> words, string HighLightAttribute = null, int summaryLen = 250)
        {
            if (body == null)
            {
                return null;
            }

            body = Lib.Helper.StringHelper.StripHTML(body);

            int len = body.Length;
            if (len <= summaryLen)
            {
                return HighLight(body, words, HighLightAttribute);
            }

            int i = 0;

            string part = null;

            while (i < len)
            {
                part = Lib.Helper.StringHelper.SementicSubString(body, i, summaryLen);
                foreach (var item in words)
                {
                    if (part.IndexOf(item, StringComparison.OrdinalIgnoreCase) > -1)
                    {
                        goto goout;
                    }
                }
                i += part.Length;
                while (i < len && IsSeperator(body[i]))
                {
                    i = i + 1;
                }
                part = null;
            }

            goout:

            if (part == null)
            {
                part = Lib.Helper.StringHelper.SementicSubString(body, 0, summaryLen);
            }

            return HighLight(part, words, HighLightAttribute);
        }

        public string HighLight(string text, List<string> Keywords, string tagAttribute)
        {
            if (string.IsNullOrEmpty(tagAttribute) || string.IsNullOrEmpty(text))
            {
                return text;
            }

            if (Keywords == null || Keywords.Count() == 0)
            {
                return text;
            }

            string starttag = "<span " + tagAttribute + ">";
            string endtag = "</span>";

            foreach (var item in Keywords)
            {
                text = Lib.Helper.StringHelper.ReplaceIgnoreCase(text, item, starttag + item + endtag);
            }
            return text;
        }

        private List<string> ToWordList(string keywords)
        {
            if (string.IsNullOrEmpty(keywords))
            {
                return new List<string>();
            }
            Search.Scanner.DefaultTokenizer tokenizer = new Search.Scanner.DefaultTokenizer();
            tokenizer.SetDoc(keywords);
            List<string> result = new List<string>();
            var token = tokenizer.ConsumeNext();
            while (token != null)
            {
                if (!string.IsNullOrEmpty(token.Value))
                {
                    result.Add(token.Value);
                }
                token = tokenizer.ConsumeNext();
            }
            return result;
        }

        private string GetTitle(string culture, Page page)
        {
            var title = page.Headers.GetTitle(culture);
            if (!string.IsNullOrEmpty(title))
            {
                return title;
            }

            var titletag = page.Dom.head.getElementsByTagName("title");
            if (titletag != null && titletag.length > 0)
            {
                return titletag.item.First().InnerHtml;
            }
            return null;
        }

        private bool IsSeperator(char currentchar)
        {
            return this.IndexData.Tokenizer.IsSeperator(currentchar);
        }
    }

    public class MetaIndex
    {
        public MetaIndex()
        {

        }
        public MetaIndex(string meta)
        {
            Parse(meta);
        }

        public MetaIndex(string model, Guid ObjectId)
        {
            this.Model = model;
            this.ObjectId = ObjectId;
        }

        public string Model { get; set; }

        public Guid ObjectId { get; set; }

        public override string ToString()
        {
            return this.Model + "__" + this.ObjectId.ToString();
        }

        public bool Parse(string meta)
        {
            if (string.IsNullOrWhiteSpace(meta))
            {
                return false;
            }
            int index = meta.IndexOf("__");
            if (index > -1)
            {
                this.Model = meta.Substring(0, index);
                if (string.IsNullOrWhiteSpace(this.Model))
                {
                    return false;
                }
                string strguid = meta.Substring(index + 2);
                Guid outid;
                if (!Guid.TryParse(strguid, out outid))
                {
                    return false;
                }

                if (outid == default(Guid))
                {
                    return false;
                }
                this.ObjectId = outid;

                return true;
            }
            return false;
        }
    }

    public class SearchResult : IEqualityComparer<SearchResult>
    {
        private Guid _id;
        public Guid Id
        {
            get
            {
                if (_id == default(Guid))
                {
                    _id = Lib.Security.Hash.ComputeGuidIgnoreCase(this.Url);
                }
                return _id;
            }
            set { _id = value; }
        }

        public string Title { get; set; }

        public string Summary { get; set; }

        private string _url;
        public string Url
        {
            get { return _url; }
            set { _url = value; _id = default(Guid); }
        }

        internal List<FoundResult> Found { get; set; } = new List<SearchResult.FoundResult>();

        public void AddFound(string ObjectType, Guid ObjectId)
        {
            if (!string.IsNullOrEmpty(ObjectType))
            {
                FoundResult found = new FoundResult();
                found.ObjectId = ObjectId;
                found.ObjectType = ObjectType.ToLower();
                this.Found.Add(found);
            }
        }

        public bool Equals(SearchResult x, SearchResult y)
        {
            return x.GetHashCode() == y.GetHashCode();
        }

        public int GetHashCode(SearchResult obj)
        {
            return Lib.Security.Hash.ComputeInt(this.Url);
        }

        public class FoundResult
        {
            public string ObjectType { get; set; }
            public Guid ObjectId { get; set; }

        }

    }

    public static class SearchResultConverter
    {

        public static string HighLight(string text, List<string> Keywords, string tagAttribute)
        {
            if (string.IsNullOrEmpty(tagAttribute) || string.IsNullOrEmpty(text))
            {
                return text;
            }

            if (Keywords == null || Keywords.Count() == 0)
            {
                return text;
            }

            string starttag = "<span " + tagAttribute + ">";
            string endtag = "</span>";

            foreach (var item in Keywords)
            {
                text = Lib.Helper.StringHelper.ReplaceIgnoreCase(text, item, starttag + item + endtag);
            }
            return text;
        }


        public static SearchResult ConvertTo(RenderContext context, string meta)
        {
            var sitedb = context.WebSite.SiteDb();
            var metaindex = new MetaIndex(meta);
            if (metaindex.ObjectId == default(Guid) || string.IsNullOrEmpty(metaindex.Model))
            {
                return null;
            }
            SearchResult result = new SearchResult();
            string lower = metaindex.Model.ToLower();

            if (lower == "page")
            {
                return ConvertPage(sitedb, metaindex.ObjectId, context.Culture);
            }
            else if (lower == "view")
            {
                return ConvertView(sitedb, metaindex.ObjectId, context.Culture);
            }
            else if (lower == "layout")
            {
                return ConvertLayout(sitedb, metaindex.ObjectId, context.Culture);
            }
            else if (lower == "htmlblock")
            {
                return ConvertHtmlBlock(sitedb, metaindex.ObjectId, context.Culture);
            }
            else if (metaindex.Model.ToLower() == "textcontent")
            {
                return ConvertTextContent(sitedb, metaindex.ObjectId, context.Culture);
            }

            return null;
        }

        private static string GetTitle(string culture, Page page)
        {
            var title = page.Headers.GetTitle(culture);
            if (!string.IsNullOrEmpty(title))
            {
                return title;
            }

            var titletag = page.Dom.head.getElementsByTagName("title");
            if (titletag != null && titletag.length > 0)
            {
                return titletag.item.First().InnerHtml;
            }
            return page.Name;
        }

        public static SearchResult ConvertPage(SiteDb SiteDb, Guid ObjectId, string culture = null)
        {
            if (string.IsNullOrEmpty(culture))
            {
                culture = SiteDb.WebSite.DefaultCulture;
            }
            SearchResult result = new SearchResult();

            var page = SiteDb.Pages.Get(ObjectId);
            if (page == null)
            {
                return null;
            }
            result.Title = GetTitle(culture, page);
            ///result.Summary = GetSummary(page.Body, keywords, SpanAttribute);  
            result.Url = Kooboo.Sites.Service.ObjectService.GetObjectFullUrl(SiteDb.WebSite, ObjectId);

            result.AddFound("Page", ObjectId);
            return result;
        }

        public static SearchResult ConvertView(SiteDb SiteDb, Guid ObjectId, string culture = null)
        {
            // view only need to find the related page.... 

            var view = SiteDb.Views.Get(ObjectId);
            if (view == null)
            {
                return null;
            }
            var page = GetViewPage(SiteDb, ObjectId);
            if (page != null)
            {
                SearchResult result = new SearchResult();
                result.Title = GetTitle(culture, page);
                result.Url = Service.ObjectService.GetObjectFullUrl(SiteDb.WebSite, page.Id);
                result.AddFound("View", ObjectId);
                return result;
            }
            return null;
        }

        public static SearchResult ConvertLayout(SiteDb SiteDb, Guid ObjectId, string culture = null)
        {
            // view only need to find the related page.... 
            SearchResult result = new SearchResult();
            var layout = SiteDb.Layouts.Get(ObjectId);
            if (layout == null)
            {
                return null;
            }
            var page = GetLayoutPage(SiteDb, ObjectId);
            if (page != null)
            {
                result.Title = GetTitle(culture, page);
                result.Url = Service.ObjectService.GetObjectFullUrl(SiteDb.WebSite, page.Id);
                result.AddFound("Layout", ObjectId);
                return result;
            }
            return null;

        }

        public static SearchResult ConvertHtmlBlock(SiteDb SiteDb, Guid ObjectId, string culture = null)
        {
            var htmlblock = SiteDb.HtmlBlocks.Get(ObjectId);
            if (htmlblock == null)
            {
                return null;
            }
            var page = GetHtmlBlockPage(SiteDb, ObjectId);
            if (page != null)
            {
                SearchResult result = new SearchResult();
                result.Title = GetTitle(culture, page);
                result.Url = Service.ObjectService.GetObjectFullUrl(SiteDb.WebSite, page.Id);
                result.AddFound("HtmlBlock", ObjectId);
                return result;
            }
            return null;
        }

        public static SearchResult ConvertTextContent(SiteDb SiteDb, Guid ObjectId, string culture = null)
        {
            var content = SiteDb.TextContent.Get(ObjectId);
            if (content == null) { return null; }

            var Setting = GetContentDefaultDetailMethodSetting(SiteDb, content);
            if (Setting == null)
            {
                return null;
            }
            foreach (var item in Setting.ParameterBinding)
            {
                if (item.Key.ToLower() == "id")
                {
                    return ConvertContentByKey(Setting, item.Value.Binding, content.Id.ToString(), SiteDb, content, culture);
                }
                else if (item.Key.ToLower() == "userkey")
                {
                    return ConvertContentByKey(Setting, item.Value.Binding, content.UserKey, SiteDb, content, culture);
                }
            }

            return null;
        }

        private static SearchResult ConvertContentByKey(DataMethodSetting setting, string binding, string replacevalue, SiteDb sitedb, TextContent content, string culture = null)
        {
            if (binding == null)
            { return null; }

            var viewmethods = sitedb.ViewDataMethods.Query.Where(o => o.MethodId == setting.Id).SelectAll();
            if (!viewmethods.Any())
            {
                return null;
            }

            foreach (var item in viewmethods)
            {
                var page = GetViewPage(sitedb, item.ViewId);
                if (page != null)
                {
                    SearchResult result = new SearchResult();
                    result.Title = GetTitle(culture, page);

                    var route = sitedb.Routes.GetByObjectId(page.Id);

                    if (route.Name.Contains(binding))
                    {
                        result.Url = route.Name.Replace(binding, replacevalue);
                    }
                    else
                    {
                        Dictionary<string, string> query = new Dictionary<string, string>();

                        if (binding.Contains("{") && binding.Contains("}"))
                        {
                            binding = binding.Replace("{", "");
                            binding = binding.Replace("}", "");
                            query.Add(binding, replacevalue);
                        }

                        result.Url = Lib.Helper.UrlHelper.AppendQueryString(route.Name, query);
                    }

                    result.AddFound("TextContent", content.Id);
                    return result;
                }
            }

            return null;
        }





        public static List<DataMethodSetting> GetContentDetailMethodSettings(SiteDb siteDb, TextContent content)
        {
            List<DataMethodSetting> result = new List<DataMethodSetting>();
            var folderid = content.FolderId;
            var allsetting = siteDb.DataMethodSettings.All();
            var Settings = allsetting.Where(o => o.ReturnType == typeof(ViewModel.TextContentViewModel).FullName).ToList();

            foreach (var item in Settings)
            {
                Guid itemfolderid;

                foreach (var binding in item.ParameterBinding)
                {
                    if (binding.Key.ToLower().Contains("folder"))
                    {
                        if (binding.Value.Binding != null && System.Guid.TryParse(binding.Value.Binding, out itemfolderid))
                        {
                            if (itemfolderid == folderid)
                            {
                                result.Add(item);
                            }
                        }
                    }
                }
            }

            return result;
        }

        public static DataMethodSetting GetContentDefaultDetailMethodSetting(SiteDb sitedb, TextContent content)

        {
            var list = GetContentDetailMethodSettings(sitedb, content);

            if (list != null && list.Any())
            {
                return list.First();
            }
            return null;
        }
        public static Page GetPage(SiteDb sitedb, byte constType, Guid ObjectId)
        {
            if (constType == ConstObjectType.Page)
            {
                return sitedb.Pages.Get(ObjectId);
            }
            else if (constType == ConstObjectType.View)
            {
                return GetViewPage(sitedb, ObjectId);
            }
            else if (constType == ConstObjectType.Layout)
            {
                return GetLayoutPage(sitedb, ObjectId);
            }
            return null;
        }

        internal static Page GetViewPage(SiteDb sitedb, Guid ObjectId, int loopcount = 0)
        {
            if (loopcount > 7)
            {
                return null;
            }
            var relations = sitedb.Views.GetUsedBy(ObjectId);

            if (relations == null || relations.Count() == 0)
            {
                return null;
            }

            var pages = relations.Where(o => o.ConstType == ConstObjectType.Page).ToList();
            foreach (var item in pages)
            {
                var page = sitedb.Pages.Get(item.ObjectId);
                if (page != null)
                {
                    return page;
                }
            }
            var layouts = relations.Where(o => o.ConstType == ConstObjectType.Layout).ToList();
            foreach (var item in layouts)
            {
                var page = GetLayoutPage(sitedb, item.ObjectId);
                if (page != null)
                {
                    return page;
                }
            }

            var others = relations.Where(o => o.ConstType != ConstObjectType.Page && o.ConstType != ConstObjectType.Layout).ToList();

            // only possible for view now. 
            foreach (var item in others)
            {
                if (item.ConstType == ConstObjectType.View)
                {
                    return GetViewPage(sitedb, item.ObjectId, loopcount + 1);
                }
            }
            return null;
        }

        internal static Page GetLayoutPage(SiteDb sitedb, Guid LayoutId, int loopcount = 0)
        {
            if (loopcount > 7)
            {
                return null;
            }
            var relations = sitedb.Layouts.GetUsedBy(LayoutId);

            if (relations == null || relations.Count() == 0)
            {
                return null;
            }

            var pages = relations.Where(o => o.ConstType == ConstObjectType.Page).ToList();
            foreach (var item in pages)
            {
                var page = sitedb.Pages.Get(item.ObjectId);
                if (page != null)
                {
                    return page;
                }
            }

            var layouts = relations.Where(o => o.ConstType == ConstObjectType.Layout).ToList();
            foreach (var item in layouts)
            {
                var page = GetLayoutPage(sitedb, item.ObjectId, loopcount + 1);
                if (page != null)
                {
                    return page;
                }
            }

            return null;
        }

        internal static Page GetHtmlBlockPage(SiteDb sitedb, Guid ObjectId)
        {

            var relations = sitedb.HtmlBlocks.GetUsedBy(ObjectId);

            if (relations == null || relations.Count() == 0)
            {
                return null;
            }

            var pages = relations.Where(o => o.ConstType == ConstObjectType.Page).ToList();
            foreach (var item in pages)
            {
                var page = sitedb.Pages.Get(item.ObjectId);
                if (page != null)
                {
                    return page;
                }
            }

            var layouts = relations.Where(o => o.ConstType == ConstObjectType.Layout).ToList();
            foreach (var item in layouts)
            {
                var page = GetLayoutPage(sitedb, item.ObjectId);
                if (page != null)
                {
                    return page;
                }
            }

            var views = relations.Where(o => o.ConstType == ConstObjectType.View).ToList();
            foreach (var item in views)
            {
                var page = GetViewPage(sitedb, item.ObjectId);
                if (page != null)
                {
                    return page;
                }
            }

            return null;
        }


    }

    public class SearchLog
    {
        public DateTime Time { get; set; } = DateTime.Now;

        public string IP { get; set; }

        public string Keywords { get; set; }

        public int ResultCount { get; set; }

        public int Skip { get; set; }

        public int DocFound { get; set; }

    }
}
