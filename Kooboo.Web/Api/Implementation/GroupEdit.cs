using System.Linq;
using Kooboo.Api;
using Kooboo.Attributes;
using Kooboo.Data.Permission;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.GroupEdit;
using Kooboo.Sites.GroupEdit.Task;
using Kooboo.Sites.GroupEdit.Task.Impl;
using Kooboo.Sites.Service;

namespace Kooboo.Web.Api.Implementation
{
    public class GroupEditAPI : IApi
    {
        public string ModelName => "GroupEdit";

        public bool RequireSite => true;

        public bool RequireUser => true;


        [Permission(Feature.PAGES, Action = Data.Permission.Action.EDIT)]
        public NodeGroup Tree(ApiCall call)
        {
            bool IncludeScript = call.GetValue<bool>("includeScript", false);

            NodeGroup group = new NodeGroup();
            group.SkipScript = !IncludeScript;
            var pages = call.Context.WebSite.SiteDb().Pages.All();

            foreach (var item in pages)
            {
                if (item.Dom != null)
                {
                    group.AddPage(item.Id, item.Dom.documentElement);
                }
            }

            group.CalculateSameSub();

            return group;
        }


        [Permission(Feature.PAGES, Action = Data.Permission.Action.EDIT)]
        public NodeGroup Search(List<SearchPara> SearchParams, bool includeScript, ApiCall call)
        {
            return SearchNode.Search(call.Context.WebSite.SiteDb(), SearchParams, includeScript);
        }

        [Permission(Feature.PAGES, Action = Data.Permission.Action.EDIT)]
        public GroupTaskStatus GetTaskStatus(Guid TaskId, ApiCall call)
        {
            return GroupTaskContainer.GetStatus(TaskId);
        }

        [Permission(Feature.PAGES, Action = Data.Permission.Action.EDIT)]
        public IGroupTask GetSiteRunningTask(ApiCall call)
        {
            var task = GroupTaskContainer.GetSiteRunningTask(call.Context.WebSite.Id);
            return task != null ? task : new GroupTaskBase() { IsFinish = true };
        }

        [Permission(Feature.PAGES, Action = Data.Permission.Action.EDIT)]
        public Guid ToView(List<LinkedPage> pages, string viewName, ApiCall call)
        {
            EnsurePageId(pages);

            bool UseExisting = call.GetBoolValue("UseExisting");
            var viewBody = call.GetValue("viewbody", "body");
            var sitedb = call.Context.WebSite.SiteDb();

            var view = call.Context.WebSite.SiteDb().Views.Get(viewName);

            if (UseExisting)
            {
                if (view == null)
                {
                    throw new Exception("View Not Found");
                }
            }
            else
            {
                if (view != null)
                {
                    throw new Exception("Name already exists");
                }
            }

            ConvertToView Converter = new ConvertToView(sitedb, pages, UseExisting, viewName, viewBody);
            return GroupTaskContainer.AddTask(sitedb, Converter);
        }

        public List<string> ViewNames(ApiCall call)
        {
            var viewNames = call.Context.WebSite.SiteDb().Views.All().Select(o => o.Name).ToList();
            return viewNames;
        }

        public List<PageName> PageNames(ApiCall call)
        {
            List<PageName> result = new List<PageName>();
            var sitedb = call.Context.WebSite.SiteDb();

            var allPages = call.Context.WebSite.SiteDb().Pages.All();

            foreach (var item in allPages)
            {
                PageName page = new PageName();
                page.PageId = item.Id;
                page.Name = item.Name;
                page.Url = Sites.Service.ObjectService.GetObjectRelativeUrl(sitedb, item);
                page.PreviewUrl = PageService.GetPreviewUrl(sitedb, item);
                result.Add(page);
            }
            return result;
        }

        public List<string> BlockNames(ApiCall call)
        {
            var names = call.Context.WebSite.SiteDb().HtmlBlocks.All().Select(o => o.Name).ToList();
            return names;
        }

        public string GetViewBody(string name, ApiCall call)
        {
            var view = call.Context.WebSite.SiteDb().Views.Get(name);
            if (view != null)
            {
                return view.Body;
            }
            return " ";
        }

        public string GetHtmlBlockBody(string name, ApiCall call)
        {
            var block = call.Context.WebSite.SiteDb().HtmlBlocks.Get(name);
            if (block != null)
            {
                return block.Body;
            }
            return " ";
        }

        public string GetInnerHtml(List<LinkedPage> pages, ApiCall call)
        {
            EnsurePageId(pages);
            foreach (var item in pages)
            {
                var page = call.Context.WebSite.SiteDb().Pages.Get(item.PageId);
                var el = Kooboo.Sites.Service.DomService.GetElementByKoobooId(page.Dom, item.KoobooId);
                var value = el.InnerHtml;

                if (!string.IsNullOrWhiteSpace(value))
                {
                    return value;
                }
            }
            return " ";
        }

        public string GetOuterHtml(List<LinkedPage> pages, ApiCall call)
        {
            EnsurePageId(pages);

            foreach (var item in pages)
            {
                var page = call.Context.WebSite.SiteDb().Pages.Get(item.PageId);
                var el = Kooboo.Sites.Service.DomService.GetElementByKoobooId(page.Dom, item.KoobooId);
                var value = el.OuterHtml;

                if (!string.IsNullOrWhiteSpace(value))
                {
                    return value;
                }
            }

            return " ";
        }


        [Permission(Feature.PAGES, Action = Data.Permission.Action.EDIT)]
        public Guid EditOuterHtml(List<LinkedPage> pages, string NewBody, ApiCall call)
        {
            EnsurePageId(pages);

            var sitedb = call.Context.WebSite.SiteDb();
            EditDom edit = new EditDom(sitedb, pages, NewBody, true);
            return GroupTaskContainer.AddTask(sitedb, edit);
        }

        [Permission(Feature.PAGES, Action = Data.Permission.Action.EDIT)]
        public Guid EditInnerHtml(List<LinkedPage> pages, string NewBody, ApiCall call)
        {
            EnsurePageId(pages);
            var sitedb = call.Context.WebSite.SiteDb();
            EditDom edit = new EditDom(sitedb, pages, NewBody, false);
            return GroupTaskContainer.AddTask(sitedb, edit);
        }

        [Permission(Feature.PAGES, Action = Data.Permission.Action.EDIT)]
        public Guid DeleteDom(List<LinkedPage> pages, ApiCall call)
        {
            EnsurePageId(pages);

            var sitedb = call.Context.WebSite.SiteDb();
            DeleteDom delete = new DeleteDom(sitedb, pages);
            return GroupTaskContainer.AddTask(sitedb, delete);
        }

        [RequireModel(typeof(List<LinkedPage>))]
        [Permission(Feature.HTML_BLOCK, Action = Data.Permission.Action.EDIT)]
        public Guid ToHtmlBlock(List<LinkedPage> pages, string name, string body, bool UseExisting, ApiCall call)
        {
            EnsurePageId(pages);

            var sitedb = call.Context.WebSite.SiteDb();

            var block = sitedb.HtmlBlocks.Get(name);

            if (UseExisting)
            {
                if (block == null)
                {
                    throw new Exception("HtmlBlock Not Found");
                }
            }
            else
            {
                if (block != null)
                {
                    throw new Exception("Name already exists");
                }
            }

            ConvertToHtmlBlock Converter = new ConvertToHtmlBlock(sitedb, pages, UseExisting, name, body);
            return GroupTaskContainer.AddTask(sitedb, Converter);
        }


        private void EnsurePageId(List<LinkedPage> pages)
        {
            if (pages != null)
            {

                var nullKooboo = pages.FindAll(o => o.KoobooId == "0");

                if (nullKooboo != null && nullKooboo.Any())
                {
                    throw new Exception("strange, Kooboo Id not generated");
                }
            }

        }

    }


    public class PageName
    {
        public Guid PageId { get; set; }

        public string Name { get; set; }

        public string Url { get; set; }

        public string PreviewUrl { get; set; }
    }


}