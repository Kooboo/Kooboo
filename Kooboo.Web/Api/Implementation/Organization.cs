using System.Collections.Generic;
using Kooboo.Api;
using Kooboo.Data;
using Kooboo.Data.Models; 
using System;
using Kooboo.Web.ViewModel;

namespace Kooboo.Web.Api
{
    public class OrganizationApi: IApi
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
            return GlobalDb.Users.Organizations(call.Context.User.Id); 
        }
             
        public List<User> GetUsers(ApiCall call)
        {
            var org =  GlobalDb.Organization.GetByUser(call.Context.User.Id);
            if (org !=null)
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

            result.redirectUrl = Kooboo.Web.Service.UserService.GetLoginRedirectUrl(call.Context, user, call.Context.Request.Url, redirecturl); 

            return result;
        }

        [Attributes.RequireParameters("organizationId", "userName")]
        public string AddUser(ApiCall call)
        {
            var userName = call.GetValue("userName");
            var organizationId = call.GetGuidValue("organizationId");
            string message = GlobalDb.Organization.AddUser(userName, organizationId);

            return message??"";
        }

        [Attributes.RequireParameters("organizationId", "userName")]
        public bool DeleteUser(ApiCall call)
        {
            var userName = call.GetValue("userName");
            var organizationId = call.GetGuidValue("organizationId");
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
         
        [Attributes.RequireParameters("organizationId")] 
        // TODO: should move to user api. 
        public bool IsAdminOfOrganization(ApiCall call)
        {
            string userName = call.Context.User.UserName;
            var organizationId = call.GetGuidValue("organizationId");
            return GlobalDb.Users.IsAdmin(organizationId, call.Context.User.Id); 
        }
         
          
    }
}
