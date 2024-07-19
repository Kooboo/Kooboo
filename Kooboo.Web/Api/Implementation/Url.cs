//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.Linq;
using Kooboo.Api;
using Kooboo.Data.Interface;
using Kooboo.Data.Language;
using Kooboo.Data.Permission;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Helper;
using Kooboo.Sites.Models;
using Kooboo.Sites.Repository;
using Kooboo.Sites.Routing;
using Kooboo.Sites.Service;
using Kooboo.Web.ViewModel;

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

        [Permission(Feature.LINK, Action = Data.Permission.Action.VIEW)]
        public Dictionary<string, IEnumerable<KeyValuePair<string, string>>> InternalOptions(ApiCall apiCall)
        {
            var sitedb = apiCall.WebSite.SiteDb();

            var all = sitedb.Routes.All();

            var resourceTypes = new HashSet<byte>();

            foreach (var item in all)
            {
                resourceTypes.Add(item.DestinationConstType);
            }

            return new Dictionary<string, IEnumerable<KeyValuePair<string, string>>>
            {
                [nameof(RouteItemViewModel.ResourceType)] = resourceTypes.ToArray()
                .Select(it => new KeyValuePair<string, string>(it.ToString(), Hardcoded.GetValue(ConstTypeContainer.GetName(it), apiCall.Context)))
                .OrderBy(it => it.Value),
            };
        }

        [Permission(Feature.LINK, Action = Data.Permission.Action.VIEW)]
        public PagedListViewModel<RouteItemViewModel> Internal(ApiCall apiCall)
        {
            var sitedb = apiCall.WebSite.SiteDb();
            var pager = ApiHelper.GetPager(apiCall, 30);

            string keyword = apiCall.GetValue("keyword");
            string resourceType = apiCall.GetValue("type");
            string hasObject = apiCall.GetValue("hasObject");

            PagedListViewModel<RouteItemViewModel> result = new PagedListViewModel<RouteItemViewModel>();

            IEnumerable<Route> allroutes = sitedb.Routes.All();
            if (!string.IsNullOrEmpty(resourceType) && byte.TryParse(resourceType, out var type))
            {
                allroutes = allroutes.Where(it => it.DestinationConstType == type);
            }
            if (!string.IsNullOrEmpty(hasObject))
            {
                if (hasObject.ToLower() == "true")
                {
                    allroutes = allroutes.Where(it => it.objectId != default);
                }
                else
                {
                    allroutes = allroutes.Where(it => it.objectId == default);
                }
            }

            if (!string.IsNullOrEmpty(keyword))
            {
                allroutes = allroutes.Where(it => it.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase));
            }

            var total = allroutes.Count();
            result.TotalCount = total;
            result.TotalPages = ApiHelper.GetPageCount(total, pager.PageSize);
            result.PageNr = pager.PageNr;

            string baseurl = sitedb.WebSite.BaseUrl();

            result.List = allroutes
                .OrderBy(o => o.Name)
                .Skip(pager.SkipCount)
                .Take(pager.PageSize)
                .Select(item => new RouteItemViewModel
                {
                    Id = item.Id,
                    Name = item.Name,
                    ResourceType = ConstTypeContainer.GetName(item.DestinationConstType),
                    ObjectId = item.objectId,
                    LastModified = item.LastModified,
                    Relations = Sites.Helper.RelationHelper.Sum(sitedb.Routes.GetUsedBy(item.Id)),
                    PreviewUrl = Kooboo.Lib.Helper.UrlHelper.Combine(baseurl, item.Name)
                })
                .ToList();

            return result;
        }

        [Permission(Feature.LINK, Action = Data.Permission.Action.VIEW)]
        public PagedListViewModel<Data.Models.SiteErrorLog> NotFound(ApiCall call)
        {
            var pager = ApiHelper.GetPager(call, 30);
            var result = new PagedListViewModel<Data.Models.SiteErrorLog>();
            var siteDb = call.WebSite.SiteDb();

            var logs = siteDb.ErrorLog.ByStatusCode(404, 5000);

            var all = new List<Data.Models.SiteErrorLog>();
            foreach (var log in logs)
            {
                var route = ObjectRoute.GetRoute(siteDb, log.Url);
                if (route != default) continue;
                all.Add(log);
            }

            result.TotalCount = all.Count;
            result.TotalPages = ApiHelper.GetPageCount(all.Count, pager.PageSize);
            result.PageNr = pager.PageNr;
            result.List = all
                .Skip(pager.SkipCount)
                .Take(pager.PageSize)
                .ToList();

            return result;
        }

        public record RouteItem(Guid Key, string Value, Dictionary<string, string> Parameters);
        [Permission(Feature.LINK, Action = Data.Permission.Action.VIEW)]
        public RouteItem[] Routes(ApiCall call)
        {
            var sitedb = call.WebSite.SiteDb();
            return sitedb.Routes.All().Select(s => new RouteItem(s.Id, s.Name, s.Parameters)).ToArray();
        }

        [Permission(Feature.LINK, Action = Data.Permission.Action.VIEW)]
        public Dictionary<string, IEnumerable<KeyValuePair<string, string>>> ExternalOptions(ApiCall apiCall)
        {
            var sitedb = apiCall.WebSite.SiteDb();

            var all = sitedb.ExternalResource.All();

            var resourceTypes = new HashSet<byte>();

            foreach (var item in all)
            {
                resourceTypes.Add(item.DestinationObjectType);
            }

            return new Dictionary<string, IEnumerable<KeyValuePair<string, string>>>
            {
                [nameof(RouteItemViewModel.ResourceType)] = resourceTypes.ToArray()
                .Select(it => new KeyValuePair<string, string>(it.ToString(), Hardcoded.GetValue(ConstTypeContainer.GetName(it), apiCall.Context)))
                .OrderBy(it => it.Value),
            };
        }

        [Permission(Feature.LINK, Action = Data.Permission.Action.VIEW)]
        public PagedListViewModel<ExternalResourceItemViewModel> External(ApiCall call)
        {
            var sitedb = call.WebSite.SiteDb();
            var pager = ApiHelper.GetPager(call, 50);
            string keyword = call.GetValue("keyword");
            string resourceType = call.GetValue("type");

            PagedListViewModel<ExternalResourceItemViewModel> result = new PagedListViewModel<ExternalResourceItemViewModel>();

            IEnumerable<ExternalResource> alllinks = sitedb.ExternalResource.All();
            if (!string.IsNullOrEmpty(resourceType) && byte.TryParse(resourceType, out var type))
            {
                alllinks = alllinks.Where(it => it.DestinationObjectType == type);
            }

            if (!string.IsNullOrEmpty(keyword))
            {
                alllinks = alllinks.Where(it => it.FullUrl.Contains(keyword, StringComparison.OrdinalIgnoreCase));
            }

            var total = alllinks.Count();
            result.TotalCount = total;
            result.TotalPages = ApiHelper.GetPageCount(total, pager.PageSize);
            result.PageNr = pager.PageNr;

            result.List = alllinks
                .OrderBy(o => o.Name)
                .Skip(pager.SkipCount)
                .Take(pager.PageSize)
                .Select(item => new ExternalResourceItemViewModel
                {
                    Id = item.Id,
                    FullUrl = item.FullUrl,
                    ResourceType = ConstTypeContainer.GetName(item.DestinationObjectType),
                    LastModified = item.LastModified,

                    Relations = Sites.Helper.RelationHelper.Sum(sitedb.ExternalResource.GetUsedBy(item.Id))
                })
                .ToList();

            return result;
        }

        [Kooboo.Attributes.RequireParameters("type", "value", "id")]
        [Permission(Feature.LINK, Action = Data.Permission.Action.EDIT)]
        public RouteUpdate UpdateUrl(ApiCall call)
        {
            var siteDb = call.WebSite.SiteDb();
            string url = call.GetValue("value", "url");

            // type = route or external. 
            var type = call.GetValue("type");
            if (type.ToLower() == "route" || type.ToLower() == "internal")
            {
                var destinationObjectId = call.GetGuidValue("destinationObjectId");
                return ChangeHelper.UpdateRoute(siteDb, url, call.ObjectId, destinationObjectId, call.Context);
            }
            else if (type.ToLower() == "external")
            {
                return ChangeHelper.UpdateExternal(siteDb, url, call.ObjectId);
            }
            return new RouteUpdate();
        }


        [Kooboo.Attributes.RequireParameters("value", "id")]
        [Permission(Feature.LINK, Action = Data.Permission.Action.EDIT)]
        public RouteUpdate MakeAlias(ApiCall call)
        {
            string url = call.GetValue("value", "url");
            var newRoute = RouteService.MakeAlias(call.Context, call.ObjectId, url);
            RouteUpdate result = new()
            {
                NewId = newRoute.Id,
                PreviewUrl = call.Context.WebSite.BaseUrl(newRoute.Name)
            };
            return result;
        }

        [Kooboo.Attributes.RequireParameters("ids", "type")]
        [Permission(Feature.LINK, Action = Data.Permission.Action.DELETE)]
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
            if (objectids != null)
            {
                foreach (var id in objectids)
                {
                    var siteobject = repo?.Get(id);
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
                        sitedb.Relations.GetRelations(id, ConstObjectType.Route);

                        foreach (var by in referredby)
                        {
                            var repofrom = sitedb.GetRepository(by.ConstTypeX);
                            if (repofrom != null)
                            {
                                Sites.Helper.ChangeHelper.DeleteUrl(sitedb, repofrom, by.objectXId, oldurl);
                            }
                        }

                        var relations = sitedb.Relations.GetRelations(id, ConstObjectType.Route);
                        foreach (var item in relations)
                        {
                            sitedb.Relations.Delete(item.Id);
                        }

                        if (siteobject is Route)
                        {
                            var route = siteobject as Route;
                            if (route.objectId == default || route.DestinationConstType == ConstObjectType.Route)
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
        }

        [Permission(Feature.LINK, Action = Data.Permission.Action.EDIT)]
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
            result.NewId = Kooboo.Data.IDGenerator.GetRouteId(url);

            if (!route.Name.Equals(url, StringComparison.CurrentCultureIgnoreCase))
            {
                var existItem = sitedb.Routes.GetByUrl(url);
                if (existItem != default && existItem.Id != call.ObjectId)
                {
                    throw new Exception(string.Format(Hardcoded.GetValue("The url '{0}' already exists. Please rename it and try again.", call.Context), url));
                }
            }

            if (route != null)
            {
                result.OldId = route.Id;
                var referredby = sitedb.Relations.GetReferredByRelations(call.ObjectId);

                Guid? objectId = default;
                if (route.DestinationConstType == ConstObjectType.Route)
                {
                    var destinationObjectId = call.GetGuidValue("destinationObjectId");
                    if (destinationObjectId != default)
                    {
                        objectId = destinationObjectId;
                    }
                }

                sitedb.Routes.ChangeRoute(route.Name, url, objectId);
                var newRouteId = Data.IDGenerator.GetRouteId(url);
                foreach (var by in referredby)
                {
                    var repofrom = sitedb.GetRepository(by.ConstTypeX);
                    if (repofrom != null)
                    {
                        Sites.Helper.ChangeHelper.ChangeUrl(sitedb, repofrom, by.objectXId, route.Name, url);
                    }

                    sitedb.Relations.Delete(by.Id);

                    if (repofrom is RouteRepository)
                    {
                        by.objectYId = newRouteId;
                        sitedb.Relations.AddOrUpdate(by);
                    }
                }

                var relations = sitedb.Relations.GetRelations(call.ObjectId, ConstObjectType.Route);
                foreach (var item in relations)
                {
                    sitedb.Relations.Delete(item.Id);
                    item.objectXId = newRouteId;
                    if (objectId.HasValue)
                    {
                        item.objectYId = objectId.Value;
                    }
                    sitedb.Relations.AddOrUpdate(item);
                }
            }
            result.NewId = Kooboo.Data.IDGenerator.GetRouteId(url);
            var newroute = sitedb.Routes.Get(result.NewId);
            result.PreviewUrl = sitedb.WebSite.BaseUrl(newroute.Name);
            return result;
        }

        public void InternalizeResource(Guid id, ApiCall call)
        {
            ResourceService.Localize(call.WebSite.SiteDb(), id, call.Context);
        }
    }
}
