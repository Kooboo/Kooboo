//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Kooboo.Web.ViewModel;
using Kooboo.Data;
using Kooboo.Api;
using Kooboo.Data.ViewModel;
using Kooboo.Mail;

namespace Kooboo.Web.Api.Implementation
{
    public class DomainApi : IApi
    {
        public string ModelName
        {
            get
            {
                return "Domain";
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

        public virtual DomainInfo ServerInfo(ApiCall call)
        {
           DomainInfo info = new DomainInfo();
           // info.DnsServers = new List<string>(); 
           // if (!string.IsNullOrEmpty(AppSettings.ServerSetting.Ns1))
           // {
           //     info.DnsServers.Add(Kooboo.Data.AppSettings.ServerSetting.Ns1);
           // }
           // if (Kooboo.Data.AppSettings.ServerSetting.Ns2 != null)
           // {
           //     info.DnsServers.Add(Kooboo.Data.AppSettings.ServerSetting.Ns2);
           // }

           // if (info.DnsServers.Count < 2)
           // {
           //     info.DnsServers.Add("ns1.dnscall.org");
           //     info.DnsServers.Add("ns2.dnscall.org");
           // }

           // var orgname = Kooboo.Data.GlobalDb.Organization.GetName(call.Context.User.Id);

           // string subname = null; 
           // if (orgname !=null)
           // {
           //     subname = orgname; 
           // }
           // else
           // {
           //     subname = AppSettings.ServerSetting.ServerId.ToString(); 
           // } 
           // info.CName = subname + "." + AppSettings.ServerSetting.HostDomain; 
           //if (Data.AppSettings.ServerSetting.Ips != null && Data.AppSettings.ServerSetting.Ips.Count > 0)
           // {
           //     info.IPAddress = AppSettings.ServerSetting.Ips.First();
           // }

            return info;
        }

        public class DomainInfo
        {
            public List<string> DnsServers { get; set; }
            public string IPAddress { get; set; }

            public string CName { get; set; }
        }

        public List<Domain> Available(ApiCall apiCall)
        {
            return Kooboo.Data.GlobalDb.Domains.ListByUser(apiCall.Context.User);
        }

        public List<DomainSummaryViewModel> List(ApiCall call)
        {
            List<DomainSummaryViewModel> result = new List<DomainSummaryViewModel>();

            var orgdb = Kooboo.Mail.Factory.DBFactory.OrgDb(call.Context.User.CurrentOrgId);

            List<EmailAddress> alladdress = null; 

            if (orgdb !=null)
            {
                alladdress = orgdb.EmailAddress.All(); 
            }


            foreach (var item in GlobalDb.Domains.ListByUser(call.Context.User))
            {
                DomainSummaryViewModel model = new DomainSummaryViewModel();
                model.Id = item.Id;
                model.DomainName = item.DomainName;

                if (AppSettings.IsOnlineServer)
                {
                    model.Expires = item.ExpirationDate.ToLongDateString();
                }
                else
                {
                    model.Expires = Data.Language.Hardcoded.GetValue("Never", call.Context);
                }
                var bindings = GlobalDb.Bindings.GetByDomain(item.Id);
                model.Records = bindings.Count(o => o.OrganizationId == call.Context.User.CurrentOrgId); model.Sites = bindings.Select(o => o.WebSiteId).Distinct().Count();


                if (alladdress != null && model.DomainName !=null)
                {
                    string domainname = model.DomainName.ToLower();

                    model.Emails = alladdress.FindAll(o => o.Address.ToLower().EndsWith(domainname)).Count();  
                }

                result.Add(model);
            }

            return result;
        }

        public Domain Get(ApiCall call)
        {
            return Kooboo.Data.GlobalDb.Domains.Get(call.ObjectId);
        }

        public List<DomainBindingViewModel> DomainSiteBindings(ApiCall call)
        {
            List<DomainBindingViewModel> result = new List<DomainBindingViewModel>();
            Domain domain = GlobalDb.Domains.Get(call.ObjectId);
            if (domain == null) { return null; }
            var bindingrecords = GlobalDb.Bindings.GetByDomain(domain.DomainName);

            foreach (var item in bindingrecords)
            {
                var site = Kooboo.Data.GlobalDb.WebSites.Get(item.WebSiteId);
                if (site != null)
                {
                    DomainBindingViewModel model = new DomainBindingViewModel();
                    model.Id = item.Id;
                    model.SubDomain = item.SubDomain;
                    model.WebSiteName = site.Name;
                    result.Add(model);
                }
            }
            return result;
        }

        [Kooboo.Attributes.RequireParameters("id")]
        public void Delete(ApiCall call)
        {
            DeleteDomain(call, call.ObjectId);
        }

        public virtual void DeleteDomain(ApiCall call, Guid Id)
        {
            var domain = GlobalDb.Domains.Get(Id);
            if (domain != null)
            {
                var bindings = GlobalDb.Bindings.GetByDomain(domain.DomainName);
                foreach (var binding in bindings)
                {
                    GlobalDb.Bindings.Delete(binding.Id);
                }
                GlobalDb.Domains.Delete(domain);
            }
        }

        [Kooboo.Attributes.RequireParameters("ids")]
        public virtual bool Deletes(ApiCall call)
        {
            string json = call.GetValue("ids");
            if (string.IsNullOrEmpty(json))
            {
                json = call.Context.Request.Body;
            }

            List<Guid> ids = new List<Guid>();

            try
            {
                ids = Lib.Helper.JsonHelper.Deserialize<List<Guid>>(json);
            }
            catch (Exception)
            {
                //throw;
            }

            if (ids != null && ids.Count() > 0)
            {
                foreach (var item in ids)
                {
                    DeleteDomain(call, item);
                }
                return true;
            }
            return false;
        }

        public void Create(string domainname, ApiCall call)
        {
            var rootdomain = Kooboo.Data.Helper.DomainHelper.GetRootDomain(domainname);

            if (string.IsNullOrEmpty(rootdomain))
            {
                throw new Exception(Data.Language.Hardcoded.GetValue("invalid domain", call.Context));
            }

            Domain domain = new Domain()
            {
                DomainName = rootdomain,
                OrganizationId = call.Context.User.CurrentOrgId
            };

            GlobalDb.Domains.AddOrUpdate(domain);
        }
        
    }


}
