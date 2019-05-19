//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using Kooboo.Extensions;
using Kooboo.Sites.Models;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Contents.Models;
using Kooboo.Events.Cms;
using Kooboo.Sites.Relation;
using System.Linq;

namespace Kooboo.Sites.Events
{ 
    public static class  Handler
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
        }

        public static void AddHandler<T>(Action<SiteObjectEvent<T>> handle)
            where T : class, Data.Interface.ISiteObject
        {

          lock(_locker)
            {
                if (!_handlers.ContainsKey(typeof(T)))
                {
                    List<object> target = new List<object>();
                    target.Add(handle);
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
               var HandleItem = item as Action<SiteObjectEvent<T>>; 
               if (HandleItem !=null)
                {
                    HandleItem(e); 
                }
            }  
        }

        private static void HandleRoute(SiteObjectEvent<Routing.Route> ChangeEvent)
        {
            if (ChangeEvent.ChangeType == ChangeType.Add)
            {
                Cache.RouteTreeCache.RouteTree(ChangeEvent.SiteDb).AddOrUpdate(ChangeEvent.Value);
                Cache.RouteTreeCache.RouteTree(ChangeEvent.SiteDb, ChangeEvent.Value.DestinationConstType).AddOrUpdate(ChangeEvent.Value);
            }

            else if (ChangeEvent.ChangeType == ChangeType.Update)
            {
                if (ChangeEvent.OldValue.Name.ToLower() != ChangeEvent.Value.Name.ToLower())
                {
                    Cache.RouteTreeCache.RouteTree(ChangeEvent.SiteDb).Del(ChangeEvent.OldValue.Name);
                    Cache.RouteTreeCache.RouteTree(ChangeEvent.SiteDb, ChangeEvent.OldValue.DestinationConstType).Del(ChangeEvent.OldValue.Name);
                }

                Cache.RouteTreeCache.RouteTree(ChangeEvent.SiteDb).AddOrUpdate(ChangeEvent.Value);
                Cache.RouteTreeCache.RouteTree(ChangeEvent.SiteDb, ChangeEvent.Value.DestinationConstType).AddOrUpdate(ChangeEvent.Value);

            }
            else
            {
                // delete an route.. 
                Cache.RouteTreeCache.RouteTree(ChangeEvent.SiteDb).Del(ChangeEvent.Value.Name);
                Cache.RouteTreeCache.RouteTree(ChangeEvent.SiteDb, ChangeEvent.Value.DestinationConstType).Del(ChangeEvent.Value.Name); 
            }
        }

        private static void HandlePageChange(SiteObjectEvent<Page> PageEvent)
        {
            if (PageEvent.ChangeType == ChangeType.Add)
            {
                //PageEvent.SiteDb.DomElements.AddOrUpdateDom(PageEvent.Value.Dom, PageEvent.Value.Id, PageEvent.Value.ConstType); 
            }
            else if (PageEvent.ChangeType == ChangeType.Update)
            {
                //if (PageEvent.OldValue.Body != PageEvent.Value.Body)
                //{ 
                //   PageEvent.SiteDb.DomElements.AddOrUpdateDom(PageEvent.Value.Dom, PageEvent.Value.Id, PageEvent.Value.ConstType); 
                //}

                Cache.RenderPlan.RemovePlan(PageEvent.SiteDb, PageEvent.OldValue.Id);

            }
            else
            {
                Cache.RenderPlan.RemovePlan(PageEvent.SiteDb, PageEvent.Value.Id);
                //PageEvent.SiteDb.DomElements.CleanObject(PageEvent.Value.Id, PageEvent.Value.ConstType); 
            }

            Routing.PageRoute.UpdatePageRouteParameter(PageEvent.SiteDb, PageEvent.Value.Id);
        }

        private static void HandleViewChange(SiteObjectEvent<View> ViewEvent)
        {
            if (ViewEvent.ChangeType == ChangeType.Add)
            {

            }
            else if (ViewEvent.ChangeType == ChangeType.Update)
            {
                if (ViewEvent.Value.Body != ViewEvent.OldValue.Body)
                {
                    Cache.RenderPlan.RemovePlan(ViewEvent.SiteDb, ViewEvent.Value.Id);

                    var pageviewrelation = ViewEvent.SiteDb.Relations.GetReferredBy(ViewEvent.Value, ConstObjectType.Page);
                    foreach (var item in pageviewrelation)
                    {
                        Cache.RenderPlan.RemovePlan(ViewEvent.SiteDb, item.objectXId);

                        // also the route parameter. 
                        Routing.PageRoute.UpdatePageRouteParameter(ViewEvent.SiteDb, item.objectXId);
                    }
                }

            }
            else
            {
                Cache.RenderPlan.RemovePlan(ViewEvent.SiteDb, ViewEvent.Value.Id);

                var pageviewrelation = ViewEvent.SiteDb.Relations.GetReferredBy(ViewEvent.Value);
                foreach (var item in pageviewrelation)
                {
                    Cache.RenderPlan.RemovePlan(ViewEvent.SiteDb, item.objectXId);
                     
                    // also the route parameter. 
                    Routing.PageRoute.UpdatePageRouteParameter(ViewEvent.SiteDb, item.objectXId);
                }

                // delete view date method. 
                var viewmethods = ViewEvent.SiteDb.ViewDataMethods.Query.Where(o => o.ViewId == ViewEvent.Value.Id).SelectAll();

                foreach (var item in viewmethods)
                {
                    ViewEvent.SiteDb.ViewDataMethods.Delete(item.Id);
                }

            }
        }

        private static void HandleLayoutChange(SiteObjectEvent<Layout> LayoutEvent)
        {
            if (LayoutEvent.ChangeType == ChangeType.Add)
            {

            }
            else if (LayoutEvent.ChangeType == ChangeType.Update)
            {
                if (LayoutEvent.Value.Body != LayoutEvent.OldValue.Body)
                {
                    Kooboo.Sites.Cache.RenderPlan.RemovePlan(LayoutEvent.SiteDb, LayoutEvent.Value.Id);

                    var allpages = LayoutEvent.SiteDb.Pages.TableScan.Where(o => o.LayoutName == LayoutEvent.Value.Name).SelectAll();

                    foreach (var item in allpages)
                    {
                        Kooboo.Sites.Cache.RenderPlan.RemovePlan(LayoutEvent.SiteDb, item.Id);
                    }
                }
            }
            else
            {
                Kooboo.Sites.Cache.RenderPlan.RemovePlan(LayoutEvent.SiteDb, LayoutEvent.Value.Id);

                var allpages = LayoutEvent.SiteDb.Pages.TableScan.Where(o => o.LayoutName == LayoutEvent.Value.Name).SelectAll();

                foreach (var item in allpages)
                {
                    Kooboo.Sites.Cache.RenderPlan.RemovePlan(LayoutEvent.SiteDb, item.Id);
                }
            }

        }

        private static void HandleContentTypeChange(SiteObjectEvent<ContentType> contenttypeevent)
        {
            if (contenttypeevent.ChangeType == ChangeType.Add)
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
            }
            else if (contenttypeevent.ChangeType == ChangeType.Delete)
            {
                var allfolders = contenttypeevent.SiteDb.ContentFolders.Query.Where(o => o.ContentTypeId == contenttypeevent.Value.Id).SelectAll();

                foreach (var item in allfolders)
                {
                    contenttypeevent.SiteDb.ContentFolders.Delete(item.Id);
                }
            }
        }

        private static void HandleContentFolderChange(SiteObjectEvent<ContentFolder> TheEvent)
        {
            var sitedb = TheEvent.SiteDb;
            if (TheEvent.Value == null || TheEvent.Value.Id == default(Guid))
            {
                return;
            }

            if (TheEvent.ChangeType == ChangeType.Delete)
            {
                // delete content folder, delete all content. 
                var foldercontent = sitedb.TextContent.Query.Where(o => o.FolderId == TheEvent.Value.Id).UseColumnData().SelectAll();

                foreach (var item in foldercontent)
                {
                    TheEvent.SiteDb.TextContent.Delete(item.Id);
                }
                // delete all  categories. 

                var AllAssignedCategories = sitedb.ContentCategories.Query.Where(o => o.CategoryFolder == TheEvent.Value.Id).SelectAll();
                foreach (var item in AllAssignedCategories)
                {
                    sitedb.ContentCategories.Delete(item.Id);
                }

                var allfolder = sitedb.ContentFolders.All().ToList().Where(o => o.Id != TheEvent.Value.Id).ToList();

                foreach (var folder in allfolder)
                {
                    var catteds = folder.Category.FindAll(o => o.FolderId == TheEvent.Value.Id).ToList();
                    if (catteds != null && catteds.Count() > 0)
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
                    var embedfolders = folder.Embedded.FindAll(o => o.FolderId == TheEvent.Value.Id).ToList();

                    if (embedfolders != null && embedfolders.Count() > 0)
                    {
                        foreach (var embedfolder in embedfolders)
                        {
                            folder.Embedded.Remove(embedfolder);
                        }

                        sitedb.ContentFolders.AddOrUpdate(folder);

                        var allTextContents = sitedb.TextContent.Query.Where(o => o.FolderId == folder.Id).SelectAll();
                        foreach (var item in allTextContents)
                        {
                            if (item.Embedded.ContainsKey(TheEvent.Value.Id))
                            {
                                item.Embedded.Remove(TheEvent.Value.Id);
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

        private static void HandleDataMethodSettingChange(Kooboo.Events.Cms.SiteObjectEvent<Kooboo.Data.Models.DataMethodSetting> MethodEvent)
        {
            var sitedb = MethodEvent.SiteDb;

            Cache.CompileMethodCache.Remove(sitedb, MethodEvent.Value.Id);

            var allviewmethods = sitedb.ViewDataMethods.Query.Where(o => o.MethodId == MethodEvent.Value.Id).SelectAll();

            if (MethodEvent.ChangeType == ChangeType.Delete)
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

        private static void HandleViewDataMethodChange(Kooboo.Events.Cms.SiteObjectEvent<ViewDataMethod> ViewDataMethodEvent)
        { 
            var viewid = ViewDataMethodEvent.Value.ViewId;
            var pages = ViewDataMethodEvent.SiteDb.Relations.GetReferredByRelations(viewid, ConstObjectType.Page);

            foreach (var item in pages)
            {
                Sites.Routing.PageRoute.UpdatePageRouteParameter(ViewDataMethodEvent.SiteDb, item.objectXId);
            }

            if (ViewDataMethodEvent.ChangeType == ChangeType.Delete)
            {
                var methodid = ViewDataMethodEvent.Value.MethodId;
                var sitemethod = ViewDataMethodEvent.SiteDb.DataMethodSettings.Get(methodid);
                if (sitemethod != null && !sitemethod.IsPublic)
                {
                    // check being used or not. 
                    var viewdatamethod = ViewDataMethodEvent.SiteDb.ViewDataMethods.Query.Where(o => o.MethodId == sitemethod.Id).SelectAll();
                    if (viewdatamethod == null || viewdatamethod.Count == 0)
                    {
                        ViewDataMethodEvent.SiteDb.DataMethodSettings.Delete(sitemethod.Id);
                    }
                }
            }
        }

        private static void HandleCssRuleChange(Kooboo.Events.Cms.SiteObjectEvent<CmsCssRule> CssRuleEvent)
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

        private static void HandleFormChange(Kooboo.Events.Cms.SiteObjectEvent<Form> FormEvent)
        {
            if (FormEvent.ChangeType == ChangeType.Delete)
            {
                foreach (var item in FormEvent.SiteDb.FormValues.Query.Where(o => o.FormId == FormEvent.Value.Id).SelectAll())
                {
                    FormEvent.SiteDb.FormValues.Delete(item.Id);
                }

                FormEvent.SiteDb.Relations.CleanObjectRelation(FormEvent.Value.Id);
            }

        }

        private static void HandleImageChange(Kooboo.Events.Cms.SiteObjectEvent<Image> ImageEvent)
        {
            Guid imageid = default(Guid);
            if (ImageEvent.Value != null)
            {
                imageid = ImageEvent.Value.Id;
            }
            else
            {
                imageid = ImageEvent.OldValue.Id;
            }

            ImageEvent.SiteDb.Thumbnails.DeleteByImageId(imageid);
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
