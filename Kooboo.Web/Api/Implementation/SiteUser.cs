using Kooboo.Api;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Web.Api.Implementation
{
    public class SiteUserApi : SiteObjectApi<SiteUser>
    {

        public List<SiteUserViewModel> AvailableUsers(ApiCall call)
        {
            var sitedb = call.Context.WebSite.SiteDb();

            var allusers = Kooboo.Data.GlobalDb.Organization.Users(call.Context.User.CurrentOrgId);


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

                        result.Add(model);
                    }

                }
            }

            return result;
        }

        public List<SiteUserViewModel> CurrentUsers(ApiCall call)
        {
            var sitedb = call.Context.WebSite.SiteDb();
            var users = sitedb.SiteUser.All();

            List<SiteUserViewModel> result = new List<SiteUserViewModel>();

            foreach (var item in users)
            {
                SiteUserViewModel model = new SiteUserViewModel();
                model.UserId = item.Id;
                model.UserName = item.Name;
                model.Role = item.Role;
                result.Add(model);
            }

            return result;
        }


        public List<string> Roles(ApiCall call)
        {
            var roles = Enum.GetNames(typeof(Kooboo.Sites.Authorization.EnumUserRole)).ToList();

            var admin = Enum.GetName(typeof(Kooboo.Sites.Authorization.EnumUserRole), Sites.Authorization.EnumUserRole.Administrator);

            roles.Remove(admin);

            return roles;
        }

        public void AddUser(Guid UserId, string role, ApiCall call)
        {
            var userrole = Lib.Helper.EnumHelper.GetEnum<Kooboo.Sites.Authorization.EnumUserRole>(role);

            var sitedb = call.Context.WebSite.SiteDb();

            var user = Kooboo.Data.GlobalDb.Users.Get(UserId);

            if (user != null)
            {
                sitedb.SiteUser.AddOrUpdate(new Sites.Models.SiteUser() { UserId = UserId, Role = userrole, Name = user.UserName });
            }

        }

        public void DeleteUsers(Guid UserId, ApiCall call)
        {
            var sitedb = call.Context.WebSite.SiteDb();

            sitedb.SiteUser.Delete(UserId);
        }

    }


    public class SiteUserViewModel
    {
        public string UserName { get; set; }

        public Guid UserId { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public Kooboo.Sites.Authorization.EnumUserRole Role { get; set; }

    }


}
