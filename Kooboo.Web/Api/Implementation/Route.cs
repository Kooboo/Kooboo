//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.Linq;
using Kooboo.Api;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Models;
using Kooboo.Sites.Render;
using Kooboo.Sites.Routing;
using Kooboo.Sites.Service;
using Kooboo.Web.ViewModel;


namespace Kooboo.Web.Api.Implementation
{
    public class RouteApi : SiteObjectApi<Route>
    {
        public override List<object> List(ApiCall call)
        {
            List<RouteItemViewModel> result = new List<RouteItemViewModel>();
            var sitedb = call.WebSite.SiteDb();

            foreach (var item in sitedb.Routes.All())
            {
                RouteItemViewModel model = new RouteItemViewModel();
                model.Id = item.Id;
                model.Name = item.Name;
                model.ResourceType = ConstTypeContainer.GetName(item.DestinationConstType);
                model.ObjectId = item.objectId;

                model.LastModified = item.LastModified;

                model.Relations = Sites.Helper.RelationHelper.Sum(sitedb.Routes.GetUsedBy(item.Id));

                result.Add(model);
            }

            return result.ToList<object>();
        }

        public List<object> ListByType(ApiCall call, string type)
        {
            List<RouteItemViewModel> result = new List<RouteItemViewModel>();
            var sitedb = call.WebSite.SiteDb();
            var all = sitedb.Routes.All();

            foreach (var item in all)
            {
                var constType = ConstTypeContainer.GetName(item.DestinationConstType);
                if (constType != type && constType != nameof(ConstObjectType.Route)) continue;

                if (constType == nameof(ConstObjectType.Route)) //check alias
                {
                    var aliasFrom = all.FirstOrDefault(f => f.Id == item.objectId);
                    if (ConstTypeContainer.GetName(aliasFrom.DestinationConstType) != type)
                    {
                        continue;
                    }
                }

                var model = new RouteItemViewModel
                {
                    Id = item.Id,
                    Name = item.Name,
                    ResourceType = constType,
                    ObjectId = item.objectId,

                    LastModified = item.LastModified,

                    Relations = Sites.Helper.RelationHelper.Sum(sitedb.Routes.GetUsedBy(item.Id))
                };

                result.Add(model);
            }

            return result.ToList<object>();
        }

        public override bool IsUniqueName(ApiCall call)
        {
            string name = call.NameOrId;
            if (string.IsNullOrEmpty(name))
            {
                return true;
            }

            Guid id = call.ObjectId;

            if (id != default)
            {
                var repo = call.WebSite.SiteDb().Routes;

                var value = repo.Get(id);
                if (value != null)
                {
                    if (value.Name == name)
                    {
                        return true;
                    }
                }
            }

            if (!string.IsNullOrEmpty(name))
            {
                var repo = call.WebSite.SiteDb().Routes;

                var oldName = call.GetValue("oldName");
                var value = repo.GetByNameOrId(name);
                if (value != null && value.objectId != Guid.Empty && value.Name != oldName)
                {
                    return false;
                }
            }

            return true;
        }

        [Kooboo.Attributes.RequireParameters("url")]
        public RouteResolvedViewModel Resolve(ApiCall call)
        {
            var url = call.GetValue("url");
            var task = new UrlRenderTask(url);
            var routeName = task.Render(call.Context);
            if (string.IsNullOrEmpty(routeName))
            {
                return null;
            }

            var siteDb = call.WebSite.SiteDb();
            var route = ObjectRoute.GetRoute(siteDb, routeName);
            if (route == null)
            {
                // try get from code
                var name = System.IO.Path.GetFileNameWithoutExtension(routeName);
                var code = siteDb.Code.GetByNameOrId(name);
                if (code != null)
                {
                    route = new Route
                    {
                        objectId = code.Id,
                        DestinationConstType = code.ConstType
                    };
                }
                else
                {
                    return null;
                }
            }

            var objectInfo = ObjectService.GetSiteObject(siteDb, route.objectId, route.DestinationConstType);
            if (objectInfo == null)
            {
                return null;
            }

            var model = new RouteResolvedViewModel
            {
                Id = objectInfo.Id,
                Name = objectInfo.Name,
                Type = ConstTypeContainer.GetName(route.DestinationConstType),
            };
            if (objectInfo is Page page)
            {
                model.Params = new Dictionary<string, object>
                {
                    ["type"] = page.Type.ToString()
                };
            }
            else if (objectInfo is Script script)
            {
                model.Params = new Dictionary<string, object>
                {
                    ["ownerObjectId"] = script.OwnerObjectId
                };
            }
            else if (objectInfo is Style style)
            {
                model.Params = new Dictionary<string, object>
                {
                    ["ownerObjectId"] = style.OwnerObjectId
                };
            }
            else if (objectInfo is Code code)
            {
                model.Params = new Dictionary<string, object>
                {
                    ["codeType"] = code.CodeType.ToString()
                };
            }

            return model;
        }
    }
}
