//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Api;
using Kooboo.Data.Models;
using Kooboo.Web.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            List<UserPublishServer> result = GetServers(call);

            return result;
        }

        private static List<UserPublishServer> GetServers(ApiCall call)
        {
            string url = Kooboo.Data.Helper.AccountUrlHelper.UserPublish("List");
            Dictionary<string, string> para = new Dictionary<string, string>();

            var servers = Lib.Helper.HttpHelper.Post<List<UserPublish>>(url, para, call.Context.User.UserName, call.Context.User.PasswordHash.ToString());

            List<UserPublishServer> result = new List<UserPublishServer>();

            if (servers != null)
            {
                foreach (var item in servers)
                {
                    result.Add(new UserPublishServer() { Id = item.Id, Name = item.Name, ServerUrl = item.ServerUrl, Reserved = item.IsReserved });
                }
            }

            result.Add(new UserPublishServer() { Name = Data.Language.Hardcoded.GetValue("LocalHost", call.Context), ServerUrl = "http://127.0.0.1", Reserved = true });
            return result;
        }


        public List<Data.Models.Domain> RemoteDomains(ApiCall call)
        {
            // request to remote for domains...    
            var serverurl = call.GetValue("ServerUrl"); 

            if (serverurl != null)
            {
                var orgid = Service.UserService.GuessOrgId(call.Context.User, serverurl);

                string url = Lib.Helper.UrlHelper.Combine(serverurl, "/_api/UserPublish/AvailableDomains");

                if (orgid != default(Guid))
                {
                    url += "?organizationid=" + orgid.ToString();
                }

                return Lib.Helper.HttpHelper.Get<List<Domain>>(url, null, call.Context.User.UserName, call.Context.User.PasswordHash.ToString());
            }
            return null;
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
            Lib.Helper.HttpHelper.Post<bool>(url, para, call.Context.User.UserName, call.Context.User.PasswordHash.ToString());
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

            bool ok = Lib.Helper.HttpHelper.Post<bool>(url, para, call.Context.User.UserName, call.Context.User.PasswordHash.ToString());

            if (ok)
            {
                return id;
            }
            else
            {
                return default(Guid);
            }
        }

    }
}
