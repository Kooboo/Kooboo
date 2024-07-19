//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved. 
using System.Linq;
using Kooboo.Api;
using Kooboo.Data;
using Kooboo.Lib.Helper;
using Kooboo.Sites.Service;
using Kooboo.Web.ViewModel;

namespace Kooboo.Web.Api.Implementation
{
    public class DomainApi : IApi
    {
        public string ModelName
        {
            get { return "Domain"; }
        }

        public bool RequireSite
        {
            get { return false; }
        }

        public bool RequireUser
        {
            get { return true; }
        }

        public Data.OpenProvider.Models.CheckResult Search(ApiCall call, string keyword, List<string> tlds)
        {
            var domains = tlds.Select(s => new Kooboo.Data.OpenProvider.Models.Domain
            {
                name = keyword,
                extension = s
            }).ToArray();

            var url = Data.Helper.AccountUrlHelper.Order("SearchDomain", true);

            Dictionary<string, string> header = new Dictionary<string, string>();
            header.Add("orgid", call.Context.User.Id.ToString());

            return Lib.Helper.HttpHelper.Post<Data.OpenProvider.Models.CheckResult>(url, JsonHelper.Serialize(domains), header);
        }

        public virtual DomainInfo ServerInfo(ApiCall call)
        {
            DomainInfo info = new DomainInfo();
            info.DnsServers = new List<string>();
            if (!string.IsNullOrEmpty(AppSettings.ServerSetting.Ns1))
            {
                info.DnsServers.Add(AppSettings.ServerSetting.Ns1);
            }

            if (AppSettings.ServerSetting.Ns2 != null)
            {
                info.DnsServers.Add(AppSettings.ServerSetting.Ns2);
            }

            if (info.DnsServers.Count < 2)
            {
                info.DnsServers.Add("ns1.kooboodns.com");
                info.DnsServers.Add("ns2.kooboodns.com");
            }

            var orgname = GlobalDb.Organization.GetName(call.Context.User.Id);

            string subname = null;
            if (orgname != null)
            {
                subname = orgname;
            }
            else
            {
                subname = AppSettings.ServerSetting.ServerId.ToString();
            }

            info.CName = subname + "." + AppSettings.ServerSetting.HostDomain;

            info.IPAddress = AppSettings.ServerSetting.MyIP;

            return info;
        }

        public class DomainInfo
        {
            public List<string> DnsServers { get; set; }
            public string IPAddress { get; set; }

            public string CName { get; set; }
        }

        public IEnumerable<Data.Models.Domain> Available(ApiCall apiCall)
        {
            var list = Kooboo.Data.GlobalDb.Domains.Available(apiCall.Context.User.CurrentOrgId).Where(o => o.MailOnly == false).ToList();

            if (list == null)
            {
                list = new List<Data.Models.Domain>();
            }

            var stagingService = new Kooboo.Data.Service.ShareStagingDomainService();

            if (!string.IsNullOrEmpty(stagingService.Domain))
            {
                list.Add(new Data.Models.Domain() { DomainName = stagingService.Domain, IsKoobooDns = false, OrganizationId = apiCall.Context.User.CurrentOrgId });
            }

            return list;
        }

        public List<DomainSummaryViewModel> List(ApiCall call)
        {
            List<DomainSummaryViewModel> result = new List<DomainSummaryViewModel>();

            var user = call.Context.User;

            foreach (var item in GlobalDb.Domains.ListByUser(user))
            {
                if (item.MailOnly)
                {
                    continue;
                }

                DomainSummaryViewModel model = new DomainSummaryViewModel();
                model.Id = item.Id;
                model.DomainName = item.DomainName;
                model.NameServer = item.NameServer;

                if (AppSettings.IsOnlineServer)
                {
                    if (item.Expiration != default(DateTime))
                    {
                        model.Expires = item.Expiration.ToLongDateString();
                    }
                }
                else
                {
                    model.Expires = Data.Language.Hardcoded.GetValue("Never", call.Context);
                }

                var bindings = Data.Config.AppHost.BindingService.ListDomainBindings(item.DomainName);

                if (bindings != null)
                {
                    bindings = bindings.Where(o => WebSiteService.UserHasRight(user, o.WebSiteId)).ToList();
                }

                model.Records = bindings.Count(o => o.OrganizationId == call.Context.User.CurrentOrgId);

                model.Sites = bindings.Select(o => o.WebSiteId).Distinct().Count();

                model.EnableDns = Kooboo.Data.Helper.DomainHelper.hasKoobooDns(item.NameServer);

                model.EnableEmail = true;
                if (AppSettings.IsOnlineServer)
                {
                    if (!model.EnableDns)
                    {
                        model.EnableEmail = false;
                    }
                }

                //model.DataCenter = item.DataCenter;
                model.DataCenter = Kooboo.Data.Service.DataCenterService.GetLangDescByName(item.DataCenter, call.Context);

                result.Add(model);
            }

            return result;

        }

        public Data.Models.Domain Get(Guid Id, ApiCall call)
        {
            var domain = GlobalDb.Domains.Get(Id, call.Context.User.CurrentOrgId);
            if (domain == null)
            {
                if (Id == Kooboo.Data.IDGenerator.GetDomainId(Data.AppSettings.DefaultLocalHost))
                {
                    return new Data.Models.Domain()
                    {
                        DomainName = Data.AppSettings.DefaultLocalHost,
                        OrganizationId = call.Context.User.CurrentOrgId
                    };
                }
            }

            return domain;
        }

        public List<DomainBindingViewModel> DomainSiteBindings(ApiCall call)
        {
            List<DomainBindingViewModel> result = new List<DomainBindingViewModel>();
            var domain = GlobalDb.Domains.Get(call.ObjectId);
            if (domain == null)
            {
                return null;
            }

            var bindingrecords = Kooboo.Data.Config.AppHost.BindingService.GetByRootDomain(domain.DomainName);

            if (bindingrecords == null)
            {
                return new List<DomainBindingViewModel>();
            }

            foreach (var item in bindingrecords)
            {
                var site = Data.Config.AppHost.SiteRepo.Get(item.WebSiteId);
                if (site != null)
                {
                    DomainBindingViewModel model = new DomainBindingViewModel();
                    model.Id = item.Id;
                    model.SubDomain = item.GetSubDomain();
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
                var bindings = Data.Config.AppHost.BindingService.GetByRootDomain(domain.DomainName);
                if (bindings != null && bindings.Count > 0)
                {
                    foreach (var binding in bindings)
                    {
                        Data.Config.AppHost.BindingRepo.Delete(binding.Id);
                    }
                }

                GlobalDb.Domains.Delete(domain, call.Context);
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

            var domain = new Data.Models.Domain()
            {
                DomainName = rootdomain,
                OrganizationId = call.Context.User.CurrentOrgId
            };

            var addok = GlobalDb.Domains.AddOrUpdate(domain, call.Context);
            if (!addok)
            {
                throw new Exception("Domain needs to use our DNS servers or A records point to our server");
            }
        }

        //TODO, move to online servers...Local version does not have rights...

        #region DNS

        public List<string> DnsType(ApiCall call)
        {
            List<string> type = new List<string>();
            type.Add("A");
            type.Add("MX");
            type.Add("AAAA");
            type.Add("TXT");
            type.Add("CNAME");
            return type;
        }

        public List<Sites.Store.Model.NameValue> TTl(ApiCall call)
        {
            List<Sites.Store.Model.NameValue> ttl = new List<Sites.Store.Model.NameValue>();

            ttl.Add(new Sites.Store.Model.NameValue { Name = "10 mins", Value = 10 * 60 });
            ttl.Add(new Sites.Store.Model.NameValue() { Name = "30 mins", Value = 30 * 60 });
            ttl.Add(new Sites.Store.Model.NameValue() { Name = "1 hours", Value = 60 * 60 });
            ttl.Add(new Sites.Store.Model.NameValue() { Name = "12 hours", Value = 12 * 60 * 60 });
            return ttl;
        }


        public List<Data.Models.DNSRecord> DnsRecords(string domain, ApiCall call)
        {
            EnsureDnsRight(call);

            var url = Data.Helper.AccountUrlHelper.Domain("Records");

            Dictionary<string, string> para = new Dictionary<string, string>();
            para.Add("domain", domain);
            para.Add("UserId", call.Context.User.Id.ToString());

            var records = Lib.Helper.HttpHelper.Get<List<Data.Models.DNSRecord>>(url, para);

            foreach (var item in records)
            {
                if (item.Type == "MX")
                {
                    item.Value = item.Value + " " + item.Priority.ToString();
                }
            }

            return records;
        }


        public List<DomainDataCenterViewModel> DataCenterList(string domain, ApiCall call)
        {
            DataCenterApi api = new DataCenterApi();
            var list = api.List(call);


            List<DomainDataCenterViewModel> result = new List<DomainDataCenterViewModel>();

            foreach (var item in list)
            {
                DomainDataCenterViewModel model = new DomainDataCenterViewModel();
                model.DataCenter = item.Name;
                model.Description = Data.Language.Hardcoded.GetValue(item.Description, call.Context);
                model.Name = item.Name;
                result.Add(model);
            }

            return result;
        }

        public bool AssignDataCenter(string Domain, string DataCenter, ApiCall call)
        {
            EnsureDnsRight(call);
            var url = Kooboo.Data.Helper.AccountUrlHelper.Domain("AssignDataCenter");
            var para = new Dictionary<string, string>();
            para.Add("domain", Domain);
            para.Add("DataCenter", DataCenter);

            return Lib.Helper.HttpHelper.Get2<bool>(
                url,
                para,
                Data.Helper.ApiHelper.GetAuthHeaders(call.Context)
            );
        }


        public void AddDns(DNSRecordViewModel model, ApiCall call)
        {
            EnsureDnsRight(call);
            var url = Kooboo.Data.Helper.AccountUrlHelper.Domain("AddDns");
            var para = new Dictionary<string, string>();
            para.Add("UserId", call.Context.User.Id.ToString());

            var json = System.Text.Json.JsonSerializer.Serialize(model);

            var ok = Lib.Helper.HttpHelper.Post<bool>(
                url,
                json,
                Data.Helper.ApiHelper.GetAuthHeaders(call.Context)
            );
        }

        public void DeleteDns(List<Guid> Ids, ApiCall call)
        {
            EnsureDnsRight(call);
            var url = Kooboo.Data.Helper.AccountUrlHelper.Domain("DeleteDNS");

            var json = System.Text.Json.JsonSerializer.Serialize(Ids);

            var ok = Lib.Helper.HttpHelper.Post<bool>(
                url,
                json,
                Data.Helper.ApiHelper.GetAuthHeaders(call.Context)
            );
        }


        private void EnsureDnsRight(ApiCall call)
        {
#if DEBUG
            return;
#endif

            if (!Kooboo.Data.AppSettings.IsOnlineServer)
            {
                throw new Exception("DNS Records only available on online version");
            }

            if (!call.Context.User.IsAdmin)
            {
                throw new Exception("you must be an administrator to view/change dns records");
            }
        }

        #endregion
    }


    public class DomainSearchResult
    {
        public string DomainName { get; set; }

        public bool IsAvailable { get; set; }

        public Dictionary<int, double> PriceByYears { get; set; } = new Dictionary<int, double>();
    }
}