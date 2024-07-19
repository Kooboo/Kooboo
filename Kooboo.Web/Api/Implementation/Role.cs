using System.Linq;
using Kooboo.Api;
using Kooboo.Data.Permission;
using Kooboo.Sites.Authorization;
using Kooboo.Sites.Authorization.Model;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Permission;

namespace Kooboo.Web.Api.Implementation
{
    public class RoleApi : IApi
    {
        public string ModelName => "Role";

        public bool RequireSite => true;

        public bool RequireUser => true;

        [Permission(Feature.ROLE, Action = Data.Permission.Action.VIEW)]
        public IEnumerable<RolePermission> List(ApiCall call)
        {
            var db = call.WebSite.SiteDb();
            var roles = db.GetSiteRepository<RolePermissionRepository>().All();

            foreach (var item in roles)
            {
                Migrator.run(item, PermissionService.GetList());
            }

            foreach (var item in PermissionService.EmbeddedRoles)
            {
                if (roles.Any(a => a.Name == item.Name)) continue;
                roles.Add(item);
            }

            return roles.OrderByDescending(it => it.CreationDate);
        }

        public record RolePermissionViewModel(string Name, List<PermissionItem> Permissions);
        [Permission(Feature.ROLE, Action = Data.Permission.Action.VIEW)]
        public RolePermissionViewModel GetEdit(ApiCall call)
        {
            var db = call.WebSite.SiteDb();
            var repo = db.GetSiteRepository<RolePermissionRepository>();
            string name = call.GetValue("name");
            var permissionList = PermissionService.GetList();

            if (!string.IsNullOrWhiteSpace(name))
            {
                var permission = repo.Get(name);
                if (permission == null)
                {
                    permission = PermissionService.EmbeddedRoles.FirstOrDefault(f => f.Name == name);
                }

                Migrator.run(permission, PermissionService.GetList());

                var permissions = new List<PermissionItem>();

                foreach (var item in permissionList)
                {
                    var exist = permission.Permissions.FirstOrDefault(f =>
                        f.Feature == item.Feature && f.Action == item.Action);

                    if (exist == null)
                    {
                        permissions.Add(item);
                    }
                    else
                    {
                        permissions.Add(exist);
                    }
                }

                return new RolePermissionViewModel(permission.Name, permissions);
            }
            else
            {
                return new RolePermissionViewModel("", permissionList);
            }
        }

        [Kooboo.Attributes.RequireModel(typeof(RolePermissionViewModel))]
        [Permission(Feature.ROLE, Action = Data.Permission.Action.EDIT)]
        public void Post(ApiCall call, RolePermission role)
        {
            var db = call.WebSite.SiteDb();
            var repo = db.GetSiteRepository<RolePermissionRepository>();
            var model = repo.Get(role.Name);

            if (model == null)
            {
                model = new RolePermission();
                model.Name = role.Name;
            }

            model.Permissions = role.Permissions;
            repo.AddOrUpdate(model, call.Context.User.Id);
        }

        public bool IsUniqueName(ApiCall call, string name)
        {
            name = name.ToLower();
            if (PermissionService.EmbeddedRoles.Any(a => a.Name == name))
            {
                return false;
            }

            var sitedb = call.WebSite.SiteDb();
            var item = sitedb.GetSiteRepository<RolePermissionRepository>().Get(name);

            return item == null;
        }

        [Permission(Feature.ROLE, Action = Data.Permission.Action.DELETE)]
        public bool Deletes(ApiCall call)
        {
            var sitedb = call.WebSite.SiteDb();
            var repo = sitedb.GetSiteRepository<RolePermissionRepository>();

            string json = call.GetValue("ids");
            if (string.IsNullOrEmpty(json))
            {
                json = call.Context.Request.Body;
            }
            List<Guid> ids = Lib.Helper.JsonHelper.Deserialize<List<Guid>>(json);

            if (ids != null && ids.Count() > 0)
            {
                var users = sitedb.SiteUser.All();
                var roles = ids.Select(s => repo.Get(s).Name).ToArray();

                if (users.Any(a => roles.Contains(a.SiteRole)))
                {
                    throw new Exception("The role is in use and cannot be deleted");
                }

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