using Kooboo.Api;
using Kooboo.Sites.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Kooboo.Sites.Authorization.Model;

namespace Kooboo.Web.Api.Implementation
{
    public class RoleApi : IApi
    {
        public string ModelName => "Role";

        public bool RequireSite => true;

        public bool RequireUser => true;


        public List<Kooboo.Sites.Authorization.Model.PermissionViewModel> List(ApiCall call)
        {
            var db = call.WebSite.SiteDb();

            var items = db.GetSiteRepository<Kooboo.Sites.Authorization.Model.RolePermissionRepository>().All();

            return items.Select(o => Sites.Authorization.PermissionService.ToViewModel(o, call.Context)).ToList();
        }


        public Kooboo.Sites.Authorization.Model.PermissionViewModel GetEdit(ApiCall call)
        {
            RolePermission permission=null;
            var db = call.WebSite.SiteDb();
            var repo = db.GetSiteRepository<Kooboo.Sites.Authorization.Model.RolePermissionRepository>();

            string name = call.GetValue("name");
            if (!string.IsNullOrWhiteSpace(name))
            {
                permission = repo.Get(name);
            }
            else
            {
                if (call.ObjectId != default(Guid))
                {
                    permission = repo.Get(call.ObjectId);
                }
                else
                {
                    var id = call.GetValue<Guid>("id");
                    if (id != default(Guid))
                    {
                        permission = repo.Get(id);
                    }
                }
            }


            if (permission != null)
            {
                return Kooboo.Sites.Authorization.PermissionService.ToViewModel(permission, call.Context);
            }
            else
            {
                return Kooboo.Sites.Authorization.DefaultData.Available;
            } 
        }
         
        public void Post(ApiCall call, Sites.Authorization.Model.PermissionViewModel model)
        {
            var permission = Kooboo.Sites.Authorization.PermissionService.ExtractPermissionFromModel(model);
            Kooboo.Sites.Authorization.Model.RolePermission role = new Sites.Authorization.Model.RolePermission();
            role.Name = model.Name;
            role.Permission = permission;

            var sitedb = call.WebSite.SiteDb();
            sitedb.GetSiteRepository<Kooboo.Sites.Authorization.Model.RolePermissionRepository>().AddOrUpdate(role, call.Context.User.Id);
        }

        public bool IsUniqueName(ApiCall call, string name)
        {
            name = name.ToLower();
            if (name == "master" || name == "developer" || name == "contentmanager")
            {
                return false;
            }

            var sitedb = call.WebSite.SiteDb();
            var item = sitedb.GetSiteRepository<Kooboo.Sites.Authorization.Model.RolePermissionRepository>().Get(name);

            return item == null;
        }


        public bool Deletes(ApiCall call)
        {
            var sitedb = call.WebSite.SiteDb();
            var repo = sitedb.GetSiteRepository<Kooboo.Sites.Authorization.Model.RolePermissionRepository>();

            string json = call.GetValue("ids");
            if (string.IsNullOrEmpty(json))
            {
                json = call.Context.Request.Body;
            }
            List<Guid> ids = Lib.Helper.JsonHelper.Deserialize<List<Guid>>(json);

            if (ids != null && ids.Count() > 0)
            {
                foreach (var item in ids)
                {
                    repo.Delete(item, call.Context.User.Id);
                }
                return true;
            }
            return false;
        }


    }
}
