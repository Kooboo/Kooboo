//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Events.Cms;
using Kooboo.Sites.Contents.Models;
using Kooboo.Sites.Models;
using Kooboo.Sites.Relation;
using Kooboo.Sites.SiteTransfer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kooboo.Sites.Events
{
    public static class Handler
    {
        private static Dictionary<Type, List<object>> _handlers = new Dictionary<Type, List<object>>();

        private static object _locker = new object();

        static Handler()
        {
            AddHandler<Routing.Route>(HandleRoute);
            AddHandler<Page>(HandlePageChange);
            AddHandler<View>(HandleViewChange);
            AddHandler<Layout>(HandleLayoutChange);
            AddHandler<Style>(HandleStyleChange);
            AddHandler<Kooboo.Data.Models.DataMethodSetting>(HandleDataMethodSettingChange);
            AddHandler<ViewDataMethod>(HandleViewDataMethodChange);
            AddHandler<CmsCssRule>(HandleCssRuleChange);
            AddHandler<Form>(HandleFormChange);
            //AddHandler<TextContent>(HandleTextContentChange);
            //AddHandler<HtmlBlock>(HandleHtmlBlockChange);
            AddHandler<ContentType>(HandleContentTypeChange);
            AddHandler<ContentFolder>(HandleContentFolderChange);
            AddHandler<Image>(HandleImageChange);
            AddHandler<ObjectRelation>(HandleObjectRelationChange);

            AddHandler<Code>(HandleCodeChange);

            AddHandler<TransferTask>(HandleTransferTask);
        }

        public static void AddHandler<T>(Action<SiteObjectEvent<T>> handle)
            where T : class, Data.Interface.ISiteObject
        {
            lock (_locker)
            {
                if (!_handlers.ContainsKey(typeof(T)))
                {
                    List<object> target = new List<object> {handle};
                    _handlers.Add(typeof(T), target);
                }
                else
                {
                    var target = _handlers[typeof(T)];
                    target.Add(handle);
                }
            }
        }

        public static void HandleChange<T>(SiteObjectEvent<T> e)
            where T : class, Data.Interface.ISiteObject
        {
            List<object> handler;
            if (!_handlers.TryGetValue(typeof(T), out handler))
                return;

            foreach (var item in handler)
            {
                if (item is Action<SiteObjectEvent<T>> handleItem)
                {
                    handleItem(e);
                }
            }
        }

        private static void HandleRoute(SiteObjectEvent<Routing.Route> changeEvent)
        {
            if (changeEvent.ChangeType == ChangeType.Add)
            {
                Cache.RouteTreeCache.RouteTree(changeEvent.SiteDb).AddOrUpdate(changeEvent.Value);
                Cache.RouteTreeCache.RouteTree(changeEvent.SiteDb, changeEvent.Value.DestinationConstType).AddOrUpdate(changeEvent.Value);
            }
            else if (changeEvent.ChangeType == ChangeType.Update)
            {
                if (changeEvent.OldValue.Name.ToLower() != changeEvent.Value.Name.ToLower())
                {
                    Cache.RouteTreeCache.RouteTree(changeEvent.SiteDb).Del(changeEvent.OldValue.Name);
                    Cache.RouteTreeCache.RouteTree(changeEvent.SiteDb, changeEvent.OldValue.DestinationConstType).Del(changeEvent.OldValue.Name);
                }

                Cache.RouteTreeCache.RouteTree(changeEvent.SiteDb).AddOrUpdate(changeEvent.Value);
                Cache.RouteTreeCache.RouteTree(changeEvent.SiteDb, changeEvent.Value.DestinationConstType).AddOrUpdate(changeEvent.Value);
            }
            else
            {
                // delete an route..
                Cache.RouteTreeCache.RouteTree(changeEvent.SiteDb).Del(changeEvent.Value.Name);
                Cache.RouteTreeCache.RouteTree(changeEvent.SiteDb, changeEvent.Value.DestinationConstType).Del(changeEvent.Value.Name);
            }
        }

        private static void HandlePageChange(SiteObjectEvent<Page> pageEvent)
        {
            switch (pageEvent.ChangeType)
            {
                case ChangeType.Add:
                    //PageEvent.SiteDb.DomElements.AddOrUpdateDom(PageEvent.Value.Dom, PageEvent.Value.Id, PageEvent.Value.ConstType);
                    break;
                case ChangeType.Update:
                    //if (PageEvent.OldValue.Body != PageEvent.Value.Body)
                    //{
                    //   PageEvent.SiteDb.DomElements.AddOrUpdateDom(PageEvent.Value.Dom, PageEvent.Value.Id, PageEvent.Value.ConstType);
                    //}
                    Cache.RenderPlan.RemovePlan(pageEvent.SiteDb, pageEvent.OldValue.Id);
                    break;
                default:
                    Cache.RenderPlan.RemovePlan(pageEvent.SiteDb, pageEvent.Value.Id);
                    //PageEvent.SiteDb.DomElements.CleanObject(PageEvent.Value.Id, PageEvent.Value.ConstType);
                    break;
            }

            Routing.PageRoute.UpdatePageRouteParameter(pageEvent.SiteDb, pageEvent.Value.Id);
        }

        private static void HandleTransferTask(SiteObjectEvent<SiteTransfer.TransferTask> taskEvent)
        {
            // check whether need to continue download or not.

            var sitedb = taskEvent.SiteDb;
            var taskcount = sitedb.TransferTasks.Count();
            if (taskcount > 0)
            {
                if (sitedb.WebSite.ContinueDownload == false)
                {
                    sitedb.WebSite.ContinueDownload = true;
                    Kooboo.Data.GlobalDb.WebSites.AddOrUpdate(sitedb.WebSite);
                }
            }
            else if (taskcount == 0)
            {
                if (sitedb.WebSite.ContinueDownload == true)
                {
                    sitedb.WebSite.ContinueDownload = false;
                    Kooboo.Data.GlobalDb.WebSites.AddOrUpdate(sitedb.WebSite);
                }
            }
        }

        private static void HandleViewChange(SiteObjectEvent<View> viewEvent)
        {
            switch (viewEvent.ChangeType)
            {
                case ChangeType.Add:
                    break;
                case ChangeType.Update:
                {
                    if (viewEvent.Value.Body != viewEvent.OldValue.Body)
                    {
                        Cache.RenderPlan.RemovePlan(viewEvent.SiteDb, viewEvent.Value.Id);

                        var pageviewrelation = viewEvent.SiteDb.Relations.GetReferredBy(viewEvent.Value, ConstObjectType.Page);
                        foreach (var item in pageviewrelation)
                        {
                            Cache.RenderPlan.RemovePlan(viewEvent.SiteDb, item.objectXId);

                            // also the route parameter.
                            Routing.PageRoute.UpdatePageRouteParameter(viewEvent.SiteDb, item.objectXId);
                        }
                    }

                    break;
                }
                default:
                {
                    Cache.RenderPlan.RemovePlan(viewEvent.SiteDb, viewEvent.Value.Id);

                    var pageviewrelation = viewEvent.SiteDb.Relations.GetReferredBy(viewEvent.Value);
                    foreach (var item in pageviewrelation)
                    {
                        Cache.RenderPlan.RemovePlan(viewEvent.SiteDb, item.objectXId);

                        // also the route parameter.
                        Routing.PageRoute.UpdatePageRouteParameter(viewEvent.SiteDb, item.objectXId);
                    }

                    // delete view date method.
                    var viewmethods = viewEvent.SiteDb.ViewDataMethods.Query.Where(o => o.ViewId == viewEvent.Value.Id).SelectAll();

                    foreach (var item in viewmethods)
                    {
                        viewEvent.SiteDb.ViewDataMethods.Delete(item.Id);
                    }

                    break;
                }
            }
        }

        private static void HandleLayoutChange(SiteObjectEvent<Layout> layoutEvent)
        {
            switch (layoutEvent.ChangeType)
            {
                case ChangeType.Add:
                    break;
                case ChangeType.Update:
                {
                    if (layoutEvent.Value.Body != layoutEvent.OldValue.Body)
                    {
                        Kooboo.Sites.Cache.RenderPlan.RemovePlan(layoutEvent.SiteDb, layoutEvent.Value.Id);

                        var allpages = layoutEvent.SiteDb.Pages.TableScan.Where(o => o.LayoutName == layoutEvent.Value.Name).SelectAll();

                        foreach (var item in allpages)
                        {
                            Kooboo.Sites.Cache.RenderPlan.RemovePlan(layoutEvent.SiteDb, item.Id);
                        }
                    }

                    break;
                }
                default:
                {
                    Kooboo.Sites.Cache.RenderPlan.RemovePlan(layoutEvent.SiteDb, layoutEvent.Value.Id);

                    var allpages = layoutEvent.SiteDb.Pages.TableScan.Where(o => o.LayoutName == layoutEvent.Value.Name).SelectAll();

                    foreach (var item in allpages)
                    {
                        Kooboo.Sites.Cache.RenderPlan.RemovePlan(layoutEvent.SiteDb, item.Id);
                    }

                    break;
                }
            }
        }

        private static void HandleContentTypeChange(SiteObjectEvent<ContentType> contenttypeevent)
        {
            switch (contenttypeevent.ChangeType)
            {
                case ChangeType.Add:
                {
                    var folder = contenttypeevent.SiteDb.ContentFolders.Query.Where(it => it.Name == contenttypeevent.Value.Name).FirstOrDefault();
                    if (folder == null)
                    {
                        folder = new ContentFolder
                        {
                            Name = contenttypeevent.Value.Name,
                            ContentTypeId = contenttypeevent.Value.Id,
                        };
                        contenttypeevent.SiteDb.ContentFolders.AddOrUpdate(folder);
                    }

                    break;
                }
                case ChangeType.Delete:
                {
                    var allfolders = contenttypeevent.SiteDb.ContentFolders.Query.Where(o => o.ContentTypeId == contenttypeevent.Value.Id).SelectAll();

                    foreach (var item in allfolders)
                    {
                        contenttypeevent.SiteDb.ContentFolders.Delete(item.Id);
                    }

                    break;
                }
            }
        }

        private static void HandleContentFolderChange(SiteObjectEvent<ContentFolder> theEvent)
        {
            var sitedb = theEvent.SiteDb;
            if (theEvent.Value == null || theEvent.Value.Id == default(Guid))
            {
                return;
            }

            if (theEvent.ChangeType == ChangeType.Delete)
            {
                // delete content folder, delete all content.
                var foldercontent = sitedb.TextContent.Query.Where(o => o.FolderId == theEvent.Value.Id).UseColumnData().SelectAll();

                foreach (var item in foldercontent)
                {
                    theEvent.SiteDb.TextContent.Delete(item.Id);
                }
                // delete all  categories.

                var allAssignedCategories = sitedb.ContentCategories.Query.Where(o => o.CategoryFolder == theEvent.Value.Id).SelectAll();
                foreach (var item in allAssignedCategories)
                {
                    sitedb.ContentCategories.Delete(item.Id);
                }

                var allfolder = sitedb.ContentFolders.All().ToList().Where(o => o.Id != theEvent.Value.Id).ToList();

                foreach (var folder in allfolder)
                {
                    var catteds = folder.Category.FindAll(o => o.FolderId == theEvent.Value.Id).ToList();
                    if (catteds != null && catteds.Any())
                    {
                        foreach (var item in catteds)
                        {
                            folder.Category.Remove(item);
                        }
                        sitedb.ContentFolders.AddOrUpdate(folder);
                    }
                }

                // remove embedded.

                foreach (var folder in allfolder)
                {
                    var embedfolders = folder.Embedded.FindAll(o => o.FolderId == theEvent.Value.Id).ToList();

                    if (embedfolders != null && embedfolders.Any())
                    {
                        foreach (var embedfolder in embedfolders)
                        {
                            folder.Embedded.Remove(embedfolder);
                        }

                        sitedb.ContentFolders.AddOrUpdate(folder);

                        var allTextContents = sitedb.TextContent.Query.Where(o => o.FolderId == folder.Id).SelectAll();
                        foreach (var item in allTextContents)
                        {
                            if (item.Embedded.ContainsKey(theEvent.Value.Id))
                            {
                                item.Embedded.Remove(theEvent.Value.Id);
                                sitedb.TextContent.AddOrUpdate(item);
                            }
                        }
                    }
                }
            }
        }

        private static void HandleStyleChange(Kooboo.Events.Cms.SiteObjectEvent<Style> StyleEvent)
        {
            //if (StyleEvent.ChangeType == ChangeType.Add)
            //{
            //    Kooboo.Sites.Relation.StyleRelation.Compute(StyleEvent.Value, StyleEvent.SiteDb);
            //}
            //else if (StyleEvent.ChangeType == ChangeType.Update)
            //{
            //    if (StyleEvent.OldValue.Body != StyleEvent.Value.Body)
            //    {
            //        Kooboo.Sites.Relation.StyleRelation.Compute(StyleEvent.Value, StyleEvent.SiteDb);
            //    }
            //}
            //else
            //{
            //    List<CmsCssRule> rules = StyleEvent.SiteDb.CssRules.Query.Where(o => o.ParentStyleId == StyleEvent.Value.Id && o.ParentCssRuleId == default(Guid)).SelectAll();

            //    foreach (var item in rules)
            //    {
            //        StyleEvent.SiteDb.CssRules.Delete(item.Id);
            //    }

            //    StyleEvent.SiteDb.Relations.CleanObjectRelation(StyleEvent.Value.Id);

            //    if (StyleEvent.Value.IsEmbedded)
            //    {
            //        StyleEvent.SiteDb.Styles.RemoveEmbedded(StyleEvent.Value);

            //    }

            //}
        }

        private static void HandleDataMethodSettingChange(Kooboo.Events.Cms.SiteObjectEvent<Kooboo.Data.Models.DataMethodSetting> methodEvent)
        {
            var sitedb = methodEvent.SiteDb;

            Cache.CompileMethodCache.Remove(sitedb, methodEvent.Value.Id);

            var allviewmethods = sitedb.ViewDataMethods.Query.Where(o => o.MethodId == methodEvent.Value.Id).SelectAll();

            if (methodEvent.ChangeType == ChangeType.Delete)
            {
                // when delete viewdatamethod, it will update the route parameters as well.
                foreach (var item in allviewmethods)
                {
                    sitedb.ViewDataMethods.Delete(item.Id);
                }
            }
            else
            {
                List<Guid> affectedPageId = new List<Guid>();

                foreach (var item in allviewmethods)
                {
                    var pagerelations = sitedb.Relations.GetReferredByRelations(item.ViewId, ConstObjectType.Page);
                    foreach (var relation in pagerelations)
                    {
                        var pageid = relation.objectXId;
                        if (!affectedPageId.Contains(pageid))
                        {
                            affectedPageId.Add(pageid);
                        }
                    }
                }

                foreach (var item in affectedPageId)
                {
                    Sites.Routing.PageRoute.UpdatePageRouteParameter(sitedb, item);
                }
            }
        }

        private static void HandleViewDataMethodChange(Kooboo.Events.Cms.SiteObjectEvent<ViewDataMethod> viewDataMethodEvent)
        {
            var viewid = viewDataMethodEvent.Value.ViewId;
            var pages = viewDataMethodEvent.SiteDb.Relations.GetReferredByRelations(viewid, ConstObjectType.Page);

            foreach (var item in pages)
            {
                Sites.Routing.PageRoute.UpdatePageRouteParameter(viewDataMethodEvent.SiteDb, item.objectXId);
            }

            if (viewDataMethodEvent.ChangeType == ChangeType.Delete)
            {
                var methodid = viewDataMethodEvent.Value.MethodId;
                var sitemethod = viewDataMethodEvent.SiteDb.DataMethodSettings.Get(methodid);
                if (sitemethod != null && !sitemethod.IsPublic)
                {
                    // check being used or not.
                    var viewdatamethod = viewDataMethodEvent.SiteDb.ViewDataMethods.Query.Where(o => o.MethodId == sitemethod.Id).SelectAll();
                    if (viewdatamethod == null || viewdatamethod.Count == 0)
                    {
                        viewDataMethodEvent.SiteDb.DataMethodSettings.Delete(sitemethod.Id);
                    }
                }
            }
        }

        private static void HandleCssRuleChange(Kooboo.Events.Cms.SiteObjectEvent<CmsCssRule> cssRuleEvent)
        {
            //if (CssRuleEvent.ChangeType == ChangeType.Add)
            //{
            //    Kooboo.Sites.Relation.CmsCssRuleRelation.Compute(CssRuleEvent.Value, CssRuleEvent.SiteDb);

            //}
            //else if (CssRuleEvent.ChangeType == ChangeType.Update)
            //{
            //    if (CssRuleEvent.OldValue.CssText != CssRuleEvent.Value.CssText)
            //    {
            //        Kooboo.Sites.Relation.CmsCssRuleRelation.Compute(CssRuleEvent.Value, CssRuleEvent.SiteDb);
            //    }
            //}
            //else
            //{
            //    List<CmsCssRule> subrules = CssRuleEvent.SiteDb.CssRules.Query.Where(o => o.ParentCssRuleId == CssRuleEvent.Value.Id).UseColumnData().SelectAll();

            //    foreach (var item in subrules)
            //    {
            //        CssRuleEvent.SiteDb.CssRules.Delete(item.Id);
            //    }

            //    // clean all relations.
            //    CssRuleEvent.SiteDb.Relations.CleanObjectRelation(CssRuleEvent.Value.Id);

            //    // clean all sub css declarations.
            //    List<CmsCssDeclaration> declarations = CssRuleEvent.SiteDb.CssDeclarations.Query.Where(o => o.CmsCssRuleId == CssRuleEvent.Value.Id).UseColumnData().SelectAll();

            //    foreach (var item in declarations)
            //    {
            //        CssRuleEvent.SiteDb.CssDeclarations.Delete(item.Id);
            //    }
            //}
        }

        private static void HandleFormChange(Kooboo.Events.Cms.SiteObjectEvent<Form> formEvent)
        {
            if (formEvent.ChangeType == ChangeType.Delete)
            {
                foreach (var item in formEvent.SiteDb.FormValues.Query.Where(o => o.FormId == formEvent.Value.Id).SelectAll())
                {
                    formEvent.SiteDb.FormValues.Delete(item.Id);
                }

                formEvent.SiteDb.Relations.CleanObjectRelation(formEvent.Value.Id);
            }
        }

        private static void HandleImageChange(Kooboo.Events.Cms.SiteObjectEvent<Image> imageEvent)
        {
            Guid imageid = default(Guid);
            imageid = imageEvent.Value != null ? imageEvent.Value.Id : imageEvent.OldValue.Id;

            imageEvent.SiteDb.Thumbnails.DeleteByImageId(imageid);
        }

        private static void HandleCodeChange(Kooboo.Events.Cms.SiteObjectEvent<Code> codeEvent)
        {
            if (codeEvent.ChangeType == ChangeType.Delete)
            {
                if (codeEvent.Value.CodeType == CodeType.Api && codeEvent.Value != null)
                {
                    // for api, also need to remove the url.
                    var route = codeEvent.SiteDb.Routes.GetByObjectId(codeEvent.Value.Id);
                    if (route != null)
                    {
                        codeEvent.SiteDb.Routes.Delete(route.Id);
                    }
                }
            }

            if (codeEvent.Value.CodeType == CodeType.Event)
            {
                var website = codeEvent.SiteDb.WebSite;

                if (codeEvent.ChangeType == ChangeType.Delete)
                {
                    // check if there is any event....
                    if (website.EnableFrontEvents)
                    {
                        var events = codeEvent.SiteDb.Code.Query.Where(o => o.CodeType == CodeType.Event).Count();
                        if (events == 0)
                        {
                            website.EnableFrontEvents = false;
                            Kooboo.Data.GlobalDb.WebSites.AddOrUpdate(website);
                        }
                    }
                }
                else
                {
                    codeEvent.SiteDb.WebSite.EnableFrontEvents = true;
                    Kooboo.Data.GlobalDb.WebSites.AddOrUpdate(website);
                }
            }
        }

        private static void HandleObjectRelationChange(SiteObjectEvent<ObjectRelation> relationEvent)
        {
            if (relationEvent.ChangeType == ChangeType.Delete)
            {
                if (relationEvent.Value != null)
                {
                    var item = relationEvent.Value;

                    if (item.ConstTypeY == ConstObjectType.Route)
                    {
                        var route = relationEvent.SiteDb.Routes.Get(item.objectYId);
                        if (route != null && route.objectId == default(Guid))
                        {
                            var stillusedby = relationEvent.SiteDb.Relations.Query.Where(o => o.objectYId == item.objectYId).SelectAll();
                            if ((stillusedby == null || stillusedby.Count == 0 || (stillusedby.Count == 1 && stillusedby[0].objectXId == item.objectXId)))
                            {
                                relationEvent.SiteDb.Routes.Delete(item.objectYId);
                            }
                        }
                    }
                    else if (item.ConstTypeY == ConstObjectType.ExternalResource)
                    {
                        var stillusedby = relationEvent.SiteDb.Relations.Query.Where(o => o.objectYId == item.objectYId).SelectAll();
                        if (stillusedby == null || stillusedby.Count == 0 || (stillusedby.Count == 1 && stillusedby[0].objectXId == item.objectXId))
                        {
                            relationEvent.SiteDb.ExternalResource.Delete(item.objectYId);
                        }
                    }
                }
            }
        }
    }
}