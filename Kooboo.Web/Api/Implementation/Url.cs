//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Api;
using Kooboo.Data.Interface;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Models;
using Kooboo.Sites.Routing;
using Kooboo.Web.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kooboo.Web.Api.Implementation
{
    public class UrlApi : IApi
    {
        public string ModelName
        {
            get
            {
                return "Url";
            }
        }

        public bool RequireSite
        {
            get
            {
                return true;
            }
        }

        public bool RequireUser
        {
            get
            {
                return true;
            }
        }

        public PagedListViewModel<RouteItemViewModel> Internal(ApiCall apiCall)
        {
            var sitedb = apiCall.WebSite.SiteDb();
            var pager = ApiHelper.GetPager(apiCall, 30);

            PagedListViewModel<RouteItemViewModel> result = new PagedListViewModel<RouteItemViewModel>();

            var allroutes = sitedb.Routes.All();

            var total = allroutes.Count;
            result.TotalCount = total;
            result.TotalPages = ApiHelper.GetPageCount(total, pager.PageSize);
            result.PageNr = pager.PageNr;

            List<RouteItemViewModel> list = new List<RouteItemViewModel>();

            string baseurl = sitedb.WebSite.BaseUrl();

            var items = allroutes.OrderBy(o => o.Name).Skip(pager.SkipCount).Take(pager.PageSize);

            foreach (var item in items)
            {   
                RouteItemViewModel model = new RouteItemViewModel();
                model.Id = item.Id;
                model.Name = item.Name;
                model.ResourceType = ConstTypeContainer.GetName(item.DestinationConstType);
                model.ObjectId = item.objectId;
                model.LastModified = item.LastModified;
                model.Relations = Sites.Helper.RelationHelper.Sum(sitedb.Routes.GetUsedBy(item.Id));
                model.PreviewUrl = Kooboo.Lib.Helper.UrlHelper.Combine(baseurl, model.Name);
                list.Add(model);
            }

            result.List = list;

            return result;

        }

        public PagedListViewModel<ExternalResourceItemViewModel> External(ApiCall call)
        {
            var sitedb = call.WebSite.SiteDb();
            var pager = ApiHelper.GetPager(call, 50);

            PagedListViewModel<ExternalResourceItemViewModel> result = new PagedListViewModel<ExternalResourceItemViewModel>();

            var alllinks = sitedb.ExternalResource.All();

            var total = alllinks.Count;
            result.TotalCount = total;
            result.TotalPages = ApiHelper.GetPageCount(total, pager.PageSize);
            result.PageNr = pager.PageNr;

            List<ExternalResourceItemViewModel> list = new List<ExternalResourceItemViewModel>();

            var items = alllinks.OrderBy(o => o.Name).Skip(pager.SkipCount).Take(pager.PageSize);

            foreach (var item in items)
            {
                ExternalResourceItemViewModel model = new ExternalResourceItemViewModel();
                model.Id = item.Id;
                model.FullUrl = item.FullUrl;
                model.ResourceType = ConstTypeContainer.GetName(item.DestinationObjectType);
                model.LastModified = item.LastModified;

                model.Relations = Sites.Helper.RelationHelper.Sum(sitedb.ExternalResource.GetUsedBy(item.Id));

                list.Add(model);
            }
            result.List = list;

            return result;
        }

        [Kooboo.Attributes.RequireParameters("type", "value", "id")]
        public RouteUpdate UpdateUrl(ApiCall call)
        {
            // type = route or external. 
            var type = call.GetValue("type");
            if (type.ToLower() == "route" || type.ToLower() == "internal")
            {
                return UpdateRoute(call);
            }
            else if (type.ToLower() == "external")
            {
                return UpdateExternal(call);
            }
            return new RouteUpdate();
        }

        [Kooboo.Attributes.RequireParameters("ids", "type")]
        public void Deletes(ApiCall call)
        {
            string strids = call.GetValue("ids");
            var sitedb = call.WebSite.SiteDb();
            List<Guid> objectids = null;
            if (!string.IsNullOrEmpty(strids))
            {
                objectids = Lib.Helper.JsonHelper.Deserialize<List<Guid>>(strids);
            }
            var type = call.GetValue("type");
            IRepository repo = null;
            if (type.ToLower() == "route" || type.ToLower() == "internal")
            {
                repo = sitedb.Routes;
            }
            else if (type.ToLower() == "external")
            {
                repo = sitedb.ExternalResource;
            }

            foreach (var id in objectids)
            {
                var siteobject = repo.Get(id);
                if (siteobject != null)
                {
                    string oldurl = null;
                    if (siteobject is Route)
                    {
                        var routeobject = siteobject as Route;
                        oldurl = routeobject.Name;
                    }
                    else
                    {
                        var external = siteobject as ExternalResource;
                        oldurl = external.FullUrl;
                    }

                    var referredby = sitedb.Relations.GetReferredByRelations(id);
                    
                    foreach (var by in referredby)
                    {
                        var repofrom = sitedb.GetRepository(by.ConstTypeX);
                        if (repofrom != null)
                        {
                            Sites.Helper.ChangeHelper.DeleteUrl(sitedb, repofrom, by.objectXId, oldurl);
                        }
                    }

                    if (siteobject is Route)
                    {
                        var route = siteobject as Route; 
                        if (route.objectId == default(Guid))
                        {
                            repo.Delete(siteobject.Id); 
                        }
                    }
                    else
                    {
                        repo.Delete(siteobject.Id);
                    }
                }

            }
        }

        private RouteUpdate UpdateRoute(ApiCall call)
        {
            string url = call.GetValue("value", "url");
  
            url = url.Replace("\\", "/"); 
            if (!url.StartsWith("/"))
            {
                url = "/" + url; 
            } 
            RouteUpdate result = new RouteUpdate();

            var sitedb = call.WebSite.SiteDb();
            var route = sitedb.Routes.Get(call.ObjectId);
            if (route != null)
            {
                result.OldId = route.Id;
                var referredby = sitedb.Relations.GetReferredByRelations(call.ObjectId);
                sitedb.Routes.ChangeRoute(route.Name, url);
                foreach (var by in referredby)
                {
                    var repofrom = sitedb.GetRepository(by.ConstTypeX);
                    if (repofrom != null)
                    {
                        Sites.Helper.ChangeHelper.ChangeUrl(sitedb, repofrom, by.objectXId, route.Name, url);
                    }
                }
            }
            result.NewId = Kooboo.Data.IDGenerator.GetRouteId(url);
            var newroute = sitedb.Routes.Get(result.NewId); 
             result.PreviewUrl  = sitedb.WebSite.BaseUrl(newroute.Name);  
            return result;
        }

        private RouteUpdate UpdateExternal(ApiCall call)
        {
            RouteUpdate result = new RouteUpdate();

            string value = call.GetValue("value", "url");
            var sitedb = call.WebSite.SiteDb();
            var external = sitedb.ExternalResource.Get(call.ObjectId);
            if (external != null)
            {
                result.OldId = external.Id;
                var referredby = sitedb.Relations.GetReferredByRelations(call.ObjectId);

                foreach (var by in referredby)
                {
                    var repofrom = sitedb.GetRepository(by.ConstTypeX);
                    if (repofrom != null)
                    {
                        Sites.Helper.ChangeHelper.ChangeUrl(sitedb, repofrom, by.objectXId, external.FullUrl, value);
                    }
                }
            }
            result.NewId = Data.IDGenerator.Generate(value, ConstObjectType.ExternalResource);
            result.PreviewUrl = value; 
            return result;
        }

        public class RouteUpdate
        {
            public Guid OldId { get; set; }
            public Guid NewId { get; set; }

            public string PreviewUrl { get; set; }
        }
    }
}
