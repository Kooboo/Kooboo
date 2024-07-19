using Kooboo.Api;
using Kooboo.Data.Models;
using Kooboo.Data.Models.Partner;

namespace Kooboo.Web.Api.V2
{
    public class Partner : IApi
    {
        public string ModelName => "Partner";

        public bool RequireSite => false;

        public bool RequireUser => true;


        private Organization GetOrg(ApiCall call)
        {
            var user = call.Context.User;
            if (user != null)
            {
                // admin org 
                var org = Kooboo.Data.GlobalDb.Organization.Get(user.Id);
                if (org != null && org.IsPartner)
                {
                    return org;
                }
            }
            return null;
        }

        //  public List<PartnerServer> Servers(string OrgName, ApiCall call)
        public List<PartnerServer> Servers(ApiCall call)
        {
            var org = GetOrg(call);

            if (org != null)
            {
                var url = Kooboo.Data.Helper.AccountUrlHelper.Partner("Servers");

                Dictionary<string, string> data = new Dictionary<string, string>();
                data.Add("OrgName", org.Name);
                return Kooboo.Lib.Helper.HttpHelper.Get<List<PartnerServer>>(url, data, null);
            }

            return null;
        }

        //   public List<DNSServer> DNS(string OrgName, ApiCall call)
        public List<DNSServer> DNS(ApiCall call)
        {
            var org = GetOrg(call);

            if (org != null)
            {
                var url = Kooboo.Data.Helper.AccountUrlHelper.Partner("DNS");

                Dictionary<string, string> data = new Dictionary<string, string>();
                data.Add("OrgName", org.Name);
                return Kooboo.Lib.Helper.HttpHelper.Get<List<DNSServer>>(url, data, null);
            }

            return null;
        }

        //  public bool AddNewUser(string username, string password, Guid serverid, string remark, string PartnerOrgName, ApiCall call)
        // public bool AddNewUserInfo(string username, string password, string Email, string Tel,  Guid serverid, string remark, string PartnerOrgName, ApiCall call)
        public bool AddNewUser(string username, string password, Guid serverid, ApiCall call)
        {
            var org = GetOrg(call);

            if (org != null)
            {
                string remark = call.GetValue("remark");
                string Tel = call.GetValue("Phone");
                string email = call.GetValue("Email");

                var url = Kooboo.Data.Helper.AccountUrlHelper.Partner("AddNewUserInfo");

                Dictionary<string, string> data = new Dictionary<string, string>();
                data.Add("username", username);
                data.Add("password", password);
                data.Add("serverid", serverid.ToString());
                data.Add("PartnerOrgName", org.Name);

                if (!string.IsNullOrWhiteSpace(email))
                {
                    data.Add("Email", email);
                }

                if (!string.IsNullOrWhiteSpace(remark))
                {
                    data.Add("remark", remark);
                }
                if (!string.IsNullOrWhiteSpace(Tel))
                {
                    data.Add("Tel", Tel);
                }
                return Kooboo.Lib.Helper.HttpHelper.Post<bool>(url, data, null, false);
            }

            return false;
        }

        // public bool AddExisting(string username, string password, Guid serverid, string remark, string PartnerOrgName, ApiCall call)
        public bool AddExisting(string username, string password, Guid serverid, ApiCall call)
        {
            var org = GetOrg(call);

            if (org != null)
            {
                string remark = call.Context.Request.GetValue("remark");

                var url = Kooboo.Data.Helper.AccountUrlHelper.Partner("AddExisting");

                Dictionary<string, string> data = new Dictionary<string, string>();
                data.Add("username", username);
                data.Add("password", password);
                data.Add("serverid", serverid.ToString());
                if (!string.IsNullOrEmpty(remark))
                {
                    data.Add("remark", remark);
                }

                data.Add("PartnerOrgName", org.Name);
                return Kooboo.Lib.Helper.HttpHelper.Post<bool>(url, data, null, false);
            }

            return false;
        }


        /// public IEnumerable<PartnerUser> Users(string OrgName, ApiCall call)
        public List<PartnerUser> Users(ApiCall call)
        {

            var org = GetOrg(call);

            if (org != null)
            {
                var url = Kooboo.Data.Helper.AccountUrlHelper.Partner("Users");

                Dictionary<string, string> data = new Dictionary<string, string>();
                data.Add("OrgName", org.Name);
                return Kooboo.Lib.Helper.HttpHelper.Get<List<PartnerUser>>(url, data, null);
            }

            return null;
        }


        // public bool RemoveUser(string username, string PartnerOrgName, ApiCall call)
        public bool RemoveUser(string username, ApiCall call)
        {
            var org = GetOrg(call);

            if (org != null)
            {
                var url = Kooboo.Data.Helper.AccountUrlHelper.Partner("RemoveUser");

                Dictionary<string, string> data = new Dictionary<string, string>();
                data.Add("username", username);
                data.Add("PartnerOrgName", org.Name);
                return Kooboo.Lib.Helper.HttpHelper.Get<bool>(url, data);
            }

            return false;
        }

        // public bool ResetPassword(string username, string password, string PartnerOrgName, ApiCall call)
        public bool ResetPassword(string username, string password, ApiCall call)
        {
            var org = GetOrg(call);

            if (org != null)
            {
                var url = Kooboo.Data.Helper.AccountUrlHelper.Partner("ResetPassword");

                Dictionary<string, string> data = new Dictionary<string, string>();
                data.Add("username", username);
                data.Add("password", password);
                data.Add("PartnerOrgName", org.Name);
                return Kooboo.Lib.Helper.HttpHelper.Get<bool>(url, data);
            }
            return false;
        }

        // public string Impersonate(string username, string PartnerOrgName, ApiCall call)
        public string InPerson(string username, ApiCall call)
        {

            //string url = "http://www.baidu.com/link?url=LQmaa373Cmyo7Kl3gX9Fi_Ui0DX9Dv6fYu2fRFBHGMqzIZ517dppd6xTrDtejFHTRCfjONP_Y2roV4zT1Ln1F8tBZryid8ccNCpbTeGBI-3";

            //return url;

            return Impersonate(username, call);
        }

        public string Impersonate(string username, ApiCall call)
        {
            var org = GetOrg(call);

            if (org != null)
            {
                var url = Kooboo.Data.Helper.AccountUrlHelper.Partner("Impersonate");

                Dictionary<string, string> data = new Dictionary<string, string>();
                data.Add("username", username);
                data.Add("PartnerOrgName", org.Name);
                return Kooboo.Lib.Helper.HttpHelper.Get<string>(url, data);
            }
            return null;
        }

    }


}
