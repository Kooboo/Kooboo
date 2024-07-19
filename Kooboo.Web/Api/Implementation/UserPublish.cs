//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Api;
using Kooboo.Sites.Service;
using Kooboo.Sites.ViewModel;

namespace Kooboo.Web.Api.Implementation
{
    public class UserPublishApi : IApi
    {
        public string ModelName
        {
            get
            {
                return "UserPublish";
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

        public List<UserPublishServer> List(ApiCall call)
        {
            List<UserPublishServer> result = UserPublishService.GetServers(call.Context);
            return result;
        }

        public List<Data.Models.Domain> RemoteDomains(string serverUrl, Guid orgId, ApiCall call)
        {
            if (!serverUrl.ToLower().StartsWith("http"))
            {
                serverUrl = "http://" + serverUrl;
            }

            string url = Lib.Helper.UrlHelper.Combine(serverUrl, "/_api/Domain/Available");
            url += "?organizationid=" + orgId.ToString();

            return Lib.Helper.HttpHelper.Post<List<Data.Models.Domain>>(
                url,
                null,
                Data.Helper.ApiHelper.GetAuthHeaders(call.Context),
                true
            );
        }

        public List<Data.Models.Domain> AvailableDomains(ApiCall call)
        {
            Guid orgid = call.GetValue<Guid>("organizationid");
            if (orgid == default(Guid))
            {
                orgid = call.Context.User.CurrentOrgId;
            }

            return Kooboo.Data.GlobalDb.Domains.ListByOrg(orgid);
        }


        [Kooboo.Attributes.RequireParameters("id")]
        public void DeleteServer(ApiCall call)
        {
            // need to verify the owner ship here... 
            string url = Kooboo.Data.Helper.AccountUrlHelper.UserPublish("Delete");
            Dictionary<string, string> para = new Dictionary<string, string>();
            para.Add("Id", call.ObjectId.ToString());

            Lib.Helper.HttpHelper.Post<bool>(
                url,
                para,
                Data.Helper.ApiHelper.GetAuthHeaders(call.Context),
                true
            );
        }

        // add or update
        [Kooboo.Attributes.RequireParameters("Id", "Name", "ServerUrl")]
        public Guid UpdateServer(ApiCall call)
        {
            string url = Kooboo.Data.Helper.AccountUrlHelper.UserPublish("Update");

            Guid id = call.ObjectId;
            string serverurl = call.GetValue("ServerUrl");

            if (call.ObjectId == default(Guid))
            {
                string unique = call.Context.User.Id.ToString() + serverurl;
                id = Lib.Security.Hash.ComputeGuidIgnoreCase(unique);
                // url = Kooboo.Data.Helper.AccountUrlHelper.UserPublish("Add");
            }

            Dictionary<string, string> para = new Dictionary<string, string>();
            para.Add("UserId", call.Context.User.Id.ToString());
            para.Add("Name", call.GetValue("Name"));
            para.Add("Id", id.ToString());
            para.Add("ServerUrl", call.GetValue("ServerUrl"));
            para.Add("OrgId", call.GetValue("OrgId"));

            bool ok = Lib.Helper.HttpHelper.Post<bool>(
                url,
                para,
                Data.Helper.ApiHelper.GetAuthHeaders(call.Context),
                true
            );

            if (ok)
            {
                return id;
            }
            else
            {
                return default(Guid);
            }
        }

        public string CreateCooperationBearer(ApiCall call)
        {
            if (!call.Context.User.IsAdmin)
            {
                throw new Exception("This api call limit by admin user");
            }
            var siteId = call.Context.WebSite.Id;
            return WebSiteService.CreateCooperationBearer(call.Context, siteId, call.GetValue("role"), call.GetValue("expire"));
        }
    }
}
