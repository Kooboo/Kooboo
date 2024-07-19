//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.Linq;
using Kooboo.Api;
using Kooboo.Data.Permission;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Models;


namespace Kooboo.Web.Api.Implementation
{
    public class SiteUserApi : SiteObjectApi<SiteUser>
    {
        public List<SiteUserViewModel> AvailableUsers(ApiCall call)
        {
            var sitedb = call.Context.WebSite.SiteDb();

            var allusers = Kooboo.Data.GlobalDb.Organization.Users(call.Context.User.CurrentOrgId);

            if (allusers == null)
            {
                allusers = new List<Data.Models.User>();
            }

            var org = Kooboo.Data.GlobalDb.Organization.Get(call.Context.User.CurrentOrgId);

            List<SiteUserViewModel> result = new List<SiteUserViewModel>();

            var currentusers = sitedb.SiteUser.All();

            foreach (var item in allusers)
            {
                if (item.Id != org.AdminUser)
                {
                    var find = currentusers.Find(o => o.UserId == item.Id);
                    if (find == null)
                    {
                        SiteUserViewModel model = new SiteUserViewModel();
                        model.UserId = item.Id;
                        model.UserName = item.UserName;
                        model.Email = item.EmailAddress;
                        result.Add(model);
                    }

                }
            }

            return result;
        }

        [Permission(Feature.SITE_USER, Action = Data.Permission.Action.VIEW)]
        public IEnumerable<SiteUserViewModel> CurrentUsers(ApiCall call)
        {
            var sitedb = call.Context.WebSite.SiteDb();

            var allusers = Kooboo.Data
                .GlobalDb
                .Organization
                .Users(call.Context.User.CurrentOrgId);

            return sitedb
                .SiteUser
                .All()
                .OrderBy(it => it.Name)
                .Select(item => new SiteUserViewModel
                {
                    UserId = item.Id,
                    UserName = item.Name,
                    Role = item.SiteRole,
                    Email = allusers.FirstOrDefault(f => f.Id == item.Id)?.EmailAddress
                });
        }

        [Permission(Feature.SITE_USER, Action = Data.Permission.Action.VIEW)]
        public List<string> Roles(ApiCall call)
        {
            var sitedb = call.Context.WebSite.SiteDb();

            var repo = sitedb.GetSiteRepository<Kooboo.Sites.Authorization.Model.RolePermissionRepository>();

            var all = repo.All();

            return all.Union(PermissionService.EmbeddedRoles)
                .Select(o => o.Name)
                .Distinct()
                .ToList();

        }

        [Permission(Feature.SITE_USER, Action = Data.Permission.Action.EDIT)]
        public void AddUser(Guid UserId, string role, ApiCall call)
        {
            var sitedb = call.Context.WebSite.SiteDb();

            var user = Kooboo.Data.GlobalDb.Users.Get(UserId);

            if (user != null)
            {
                sitedb.SiteUser.AddOrUpdate(new SiteUser() { UserId = UserId, SiteRole = role, Name = user.UserName });
            }
        }

        [Permission(Feature.SITE_USER, Action = Data.Permission.Action.DELETE)]
        public void DeleteUsers(Guid UserId, ApiCall call)
        {
            var sitedb = call.Context.WebSite.SiteDb();

            sitedb.SiteUser.Delete(UserId);
        }

        [Permission(Feature.SITE_USER, Action = Data.Permission.Action.EDIT)]
        public override Guid AddOrUpdate(ApiCall call)
        {
            return base.AddOrUpdate(call);
        }

        [Permission(Feature.SITE_USER, Action = Data.Permission.Action.DELETE)]
        public override bool Delete(ApiCall call)
        {
            return base.Delete(call);
        }

        [Permission(Feature.SITE_USER, Action = Data.Permission.Action.DELETE)]
        public override bool Deletes(ApiCall call)
        {
            return base.Deletes(call);
        }

        [Permission(Feature.SITE_USER, Action = Data.Permission.Action.VIEW)]
        public override object Get(ApiCall call)
        {
            return base.Get(call);
        }

        [Permission(Feature.SITE_USER, Action = Data.Permission.Action.VIEW)]
        public override List<object> List(ApiCall call)
        {
            return base.List(call);
        }

        [Permission(Feature.SITE_USER, Action = Data.Permission.Action.EDIT)]
        public override Guid Post(ApiCall call)
        {
            return base.Post(call);
        }

        [Permission(Feature.SITE_USER, Action = Data.Permission.Action.EDIT)]
        public override Guid put(ApiCall call)
        {
            return base.put(call);
        }
    }


    public class SiteUserViewModel
    {
        public string UserName { get; set; }

        public Guid UserId { get; set; }

        public string Role { get; set; }

        public string Email { get; set; }
    }
}
