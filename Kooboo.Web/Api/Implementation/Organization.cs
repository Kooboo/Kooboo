//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.Linq;
using Kooboo.Api;
using Kooboo.Data;
using Kooboo.Data.Helper;
using Kooboo.Data.Language;
using Kooboo.Data.Models;
using Kooboo.Data.ViewModel;
using Kooboo.Lib.Helper;
using Kooboo.Web.Api.Implementation;
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

        public List<OrganizationListItemViewModel> GetOrganizations(ApiCall call)
        {
            var user = call.Context.User;

            if (GlobalDb.Users.IsDefaultUser(call.Context.User))
            {
                List<OrganizationListItemViewModel> org = new List<OrganizationListItemViewModel>();
                org.Add(new OrganizationListItemViewModel() { AdminUser = user.Id, Name = user.UserName });
                return org;
            }

            return GlobalDb.Users.OrganizationView(call.Context.User.Id);
        }

        public List<OrgUserViewModel> GetUsers(ApiCall call)
        {
            if (GlobalDb.Users.IsDefaultUser(call.Context.User))
            {
                List<User> users = new List<User>();
                users.Add(call.Context.User);
            }

            var list = GlobalDb.Organization.OrgUserView(call.Context.User.CurrentOrgId);

            return list.OrderByDescending(o => o.JoinDate).ToList();
        }


        public SimpleUser ChangeUserOrg(Guid organizationId, ApiCall call)
        {
            var token = call.Context.Request.GetValue("jwt_token");

            var user = GlobalDb.Users.ChangeOrg(call.Context, organizationId);
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

        public string AddUser(string userName, ApiCall call)
        {
            if (GlobalDb.Users.IsDefaultUser(call.Context.User))
            {
                throw new Exception("Default account can not add user");
            }

            string message = GlobalDb.Organization.AddUser(userName, call.Context);

            return message ?? "";
        }

        public bool DeleteUser(string userName, ApiCall call)
        {
            return GlobalDb.Organization.DeleteUser(userName, call.Context);
        }


        public Organization GetOrg(ApiCall call)
        {
            var organizationId = call.Context.User.CurrentOrgId;
            return GlobalDb.Organization.Get(organizationId);
        }


        public UserOrgViewModel GetUserOrg(ApiCall call)
        {
            var organizationId = call.Context.User.CurrentOrgId;
            var org = GlobalDb.Organization.Get(organizationId);
            UserOrgViewModel model = new UserOrgViewModel();
            model.Id = org.Id;
            model.Name = org.Name;
            model.DisplayName = org.DisplayName;
            model.UserName = call.Context.User.UserName;
            model.UserId = call.Context.User.Id;
            model.ServiceLevel = org.ServiceLevel;
            model.IsPartner = org.IsPartner;
            model.HasImap = org.ServiceLevel >= 2;

            if (org.AdminUser == call.Context.User.Id || org.Id == call.Context.User.Id)
            {
                model.IsAdmin = true;
            }
            return model;
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

        public bool CreateOrgUser(string username, string password, string email, ApiCall call)
        {
            var user = call.Context.User;
            //string username, string password, string Email, string Tel,
            var tel = call.GetValue("Tel", "Phone");

            var IsAdmin = GlobalDb.Users.IsAdmin(user.CurrentOrgId, user.Id);
            if (!IsAdmin)
            {
                var msg = Hardcoded.GetValue("Not Authorized", call.Context);
                throw new Exception(msg);
            }

            var result = GlobalDb.Organization.CreateOrgUser(user.CurrentOrgId, username, password, email, tel, call.Context);
            return true;
        }


        //  public bool DepartOrg(Guid OrgId, ApiCall call)
        public bool UserLeaveOrg(Guid OrgId, ApiCall call)
        {
            var para = new Dictionary<string, string>();
            para.Add("OrgId", OrgId.ToString());
            var AuthHeader = Kooboo.Data.Helper.ApiHelper.GetAuthHeaders(call.Context);
            return HttpHelper.Post<bool>(AccountUrlHelper.Org("DepartOrg"), para, AuthHeader);
        }

        //  public void CreateSoleOrg(ApiCall call)
        public bool CreateOwnOrg(ApiCall call)
        {
            var para = new Dictionary<string, string>();

            var AuthHeader = Kooboo.Data.Helper.ApiHelper.GetAuthHeaders(call.Context);
            return HttpHelper.Post<bool>(AccountUrlHelper.Org("CreateSoleOrg"), para, AuthHeader);
        }


        [Obsolete]
        public List<DataCenterViewModelOld> DataCenterList(ApiCall call)
        {
            DataCenterApi api = new DataCenterApi();

            var list = api.List(call);

            List<DataCenterViewModelOld> result = new List<DataCenterViewModelOld>();


            if (list != null && list.Count > 0)
            {
                foreach (var item in list)
                {
                    DataCenterViewModelOld model = new DataCenterViewModelOld();
                    model.Selected = item.Default;
                    string displayname = item.Name;

                    model.Name = displayname;
                    result.Add(model);
                }
            }
            return result;
        }

    }



    public class DataCenterViewModelOld
    {
        public string Name { get; set; }

        public string Country { get; set; }

        public int Id { get; set; }

        public bool Selected { get; set; }

    }
}
