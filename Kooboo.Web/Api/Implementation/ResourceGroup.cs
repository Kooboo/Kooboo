//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Api;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Models;
using Kooboo.Web.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kooboo.Web.Api.Implementation
{
    public class ResourceGroupApi : IApi
    {
        public string ModelName
        {
            get
            {
                return "ResourceGroup";
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

        public List<ResourceGroupViewModel> Style(ApiCall call)
        {
            var sitedb = call.WebSite.SiteDb();

            string sitebaseurl = call.WebSite.BaseUrl();

            string previewTemplate = Sites.Systems.Routes.SystemRouteTemplate.Replace("{objecttype}", ConstObjectType.ResourceGroup.ToString());

            var styleGroup = sitedb.ResourceGroups.Query.Where(o => o.Type == ConstObjectType.Style).SelectAll();

            List<ResourceGroupViewModel> result = new List<ResourceGroupViewModel>();
            foreach (var item in styleGroup)
            {
                ResourceGroupViewModel newitem = new ResourceGroupViewModel
                {
                    Name = item.Name,
                    Id = item.Id,
                    Type = item.Type,
                    LastModified = item.LastModified,
                    ChildrenCount = item.Children.Count()
                };

                var usedby = sitedb.ResourceGroups.GetUsedBy(item.Id);
                newitem.References = Sites.Helper.RelationHelper.Sum(usedby);
                var route = sitedb.Routes.GetByObjectId(item.Id);
                newitem.RelativeUrl = route != null ? route.Name : previewTemplate.Replace("{nameorid}", item.Name);
                newitem.PreviewUrl = Lib.Helper.UrlHelper.Combine(sitebaseurl, newitem.RelativeUrl);
                result.Add(newitem);
            }
            return result;
        }

        public List<ResourceGroupViewModel> Script(ApiCall apiCall)
        {
            var sitedb = apiCall.WebSite.SiteDb();

            string sitebaseurl = apiCall.WebSite.BaseUrl();

            string previewTemplate = Sites.Systems.Routes.SystemRouteTemplate.Replace("{objecttype}", ConstObjectType.ResourceGroup.ToString());

            var scriptGroup = sitedb.ResourceGroups.Query.Where(o => o.Type == ConstObjectType.Script).SelectAll();

            List<ResourceGroupViewModel> result = new List<ResourceGroupViewModel>();
            foreach (var item in scriptGroup)
            {
                ResourceGroupViewModel newitem = new ResourceGroupViewModel
                {
                    Name = item.Name,
                    Id = item.Id,
                    Type = item.Type,
                    LastModified = item.LastModified,
                    ChildrenCount = item.Children.Count()
                };

                var usedby = sitedb.ResourceGroups.GetUsedBy(item.Id);
                newitem.References = Sites.Helper.RelationHelper.Sum(usedby);

                var route = sitedb.Routes.GetByObjectId(item.Id);
                newitem.RelativeUrl = route != null ? route.Name : previewTemplate.Replace("{nameorid}", item.Name);
                newitem.PreviewUrl = Lib.Helper.UrlHelper.Combine(sitebaseurl, newitem.RelativeUrl);
                result.Add(newitem);
            }
            return result;
        }

        public ResourceGroupViewModel Get(ApiCall apicall)
        {
            var sitedb = apicall.WebSite.SiteDb();

            Guid groupid = apicall.ObjectId;
            var group = sitedb.ResourceGroups.Query.Where(o => o.Id == groupid).FirstOrDefault();
            if (group != null)
            {
                ResourceGroupViewModel newitem = new ResourceGroupViewModel
                {
                    Name = @group.Name,
                    Id = @group.Id,
                    Type = @group.Type,
                    TypeName = @group.Type == ConstObjectType.Script ? "Script" : "Style",
                    LastModified = @group.LastModified,
                    ChildrenCount = @group.Children.Count()
                };
                foreach (var subitem in group.Children)
                {
                    var sub = new ResourceGroupItem
                    {
                        RouteId = subitem.Key,
                        Order = subitem.Value,
                        Name = sitedb.Routes.Get(subitem.Key)?.Name
                    };
                    newitem.Children.Add(sub);
                }
                return newitem;
            }
            return null;
        }

        public void Update(ApiCall call)
        {
            string json = call.Context.Request.Body;
            var viewmodel = Lib.Helper.JsonHelper.Deserialize<ResourceGroupViewModel>(json);

            var model = new ResourceGroup
            {
                Name = viewmodel.Name,
            };

            if (string.IsNullOrEmpty(viewmodel.TypeName))
            {
                model.Type = ConstObjectType.Style;
            }
            else
            {
                model.Type = viewmodel.TypeName.ToLower().Trim() == "style" ? ConstObjectType.Style : ConstObjectType.Script;
            }

            int i = 0;
            foreach (var item in viewmodel.Children)
            {
                model.Children.Add(item.RouteId, i);
                i++;
            }

            var url = Sites.Service.GroupService.GetUrl(model.Name, model.Type);
            var sitedb = call.WebSite.SiteDb();

            var exisitingitem = sitedb.ResourceGroups.Get(model.Id);
            if (exisitingitem == null)
            {
                sitedb.Routes.AddOrUpdate(url, model, call.Context.User.Id);
                sitedb.ResourceGroups.AddOrUpdate(model, call.Context.User.Id);
            }
            else
            {
                var route = sitedb.Routes.GetByUrl(url);
                if (route.objectId != model.Id)
                {
                    throw new Exception("name conflict");
                }
                sitedb.ResourceGroups.AddOrUpdate(model, call.Context.User.Id);
            }
        }

        public bool IsUniqueName(string name, string type, ApiCall call)
        {
            byte consttype = 0;

            if (type.ToLower() == "style")
            {
                consttype = ConstObjectType.Style;
            }
            else if (type.ToLower() == "script")
            {
                consttype = ConstObjectType.Script;
            }
            var sitedb = call.WebSite.SiteDb();

            return sitedb.ResourceGroups.IsUniqueName(name, consttype);
        }

        public void Deletes(ApiCall call)
        {
            string json = call.GetValue("ids");
            if (string.IsNullOrEmpty(json))
            {
                json = call.Context.Request.Body;
            }

            List<Guid> ids = new List<Guid>();

            try
            {
                ids = Lib.Helper.JsonHelper.Deserialize<List<Guid>>(json);
            }
            catch (Exception)
            {
                //throw;
            }

            if (ids != null && ids.Any())
            {
                foreach (var item in ids)
                {
                    call.WebSite.SiteDb().ResourceGroups.Delete(item, call.Context.User.Id);
                }
            }
        }
    }
}