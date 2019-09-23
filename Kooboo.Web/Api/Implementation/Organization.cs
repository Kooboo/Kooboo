//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.Collections.Generic;
using Kooboo.Api;
using Kooboo.Data;
using Kooboo.Data.Models;
using System;
using Kooboo.Web.ViewModel;

namespace Kooboo.Web.Api
{
    public class OrganizationApi : IApi
    {
        public string ModelName
        {
            get
            {
                return "organization";
            }
        }

        public bool RequireSite
        {
            get
            {
                return false;
            }
        }

        public bool RequireUser
        {
            get
            {
                return true;
            }
        }

        public List<Organization> GetOrganizations(ApiCall call)
        {
            var user = call.Context.User;

            if (GlobalDb.Users.IsDefaultUser(call.Context.User))
            {
                List<Organization> org = new List<Organization>();
                org.Add(new Organization() { AdminUser = user.Id, Name = user.UserName });
                return org;
            }

            return GlobalDb.Users.Organizations(call.Context.User.Id);
        }

        public List<User> GetUsers(ApiCall call)
        {
            if (GlobalDb.Users.IsDefaultUser(call.Context.User))
            {
                List<User> users = new List<User>();
                users.Add(call.Context.User);
            }

            var org = GlobalDb.Organization.GetByUser(call.Context.User.Id);
            if (org != null)
            {
                return GlobalDb.Organization.Users(org.Id);
            }
            return null;
        }


        public SimpleUser ChangeUserOrg(Guid organizationId, ApiCall call)
        {
            var user = GlobalDb.Users.ChangeOrg(call.Context.User.Id, organizationId);
            if (user != null)
            {
                call.Context.User = user;
            }

            call.Context.Response.DeleteCookie(DataConstants.UserApiSessionKey);

            SimpleUser result = new SimpleUser(user);

            string redirecturl = "/_Admin/Account/Profile";

            result.redirectUrl = Kooboo.Web.Service.UserService.GetLoginRedirectUrl(call.Context, user, call.Context.Request.Url, redirecturl, false);

            return result;
        }

        public string AddUser(string userName, Guid organizationId, ApiCall call)
        {
            if (GlobalDb.Users.IsDefaultUser(call.Context.User))
            {
                throw new Exception(Data.Language.Hardcoded.GetValue("Default account can not add user"));
            }

            string message = GlobalDb.Organization.AddUser(userName, organizationId);

            return message ?? "";
        }

        public bool DeleteUser(string userName, Guid organizationId, ApiCall call)
        {
            return GlobalDb.Organization.DeleteUser(userName, organizationId);
        }


        public Organization GetOrg(ApiCall call)
        {
            var organizationId = call.Context.User.CurrentOrgId;
            return GlobalDb.Organization.Get(organizationId);
        }

        public Organization GetOwnOrg(ApiCall call)
        {
            return GlobalDb.Organization.GetByUser(call.Context.User.Id);
        }


        // TODO: should move to user api. 
        public bool IsAdminOfOrganization(Guid organizationId, ApiCall call)
        {
            return GlobalDb.Users.IsAdmin(organizationId, call.Context.User.Id);
        }

        public bool RemoveLocal(string name, ApiCall call)
        {
            Guid id = Lib.Helper.IDHelper.ParseKey(name);

            GlobalDb.Organization.RemoveOrgCache(id);
            return true; 
        }
    }
}
