//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Api;
using Kooboo.Data;
using Kooboo.Data.Models;
using Kooboo.Web.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kooboo.Web.Api.Implementation
{
    public class BindingApi : IApi
    {
        public string ModelName
        {
            get
            {
                return "binding";
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

        [Kooboo.Attributes.RequireParameters("id")]
        public void Delete(ApiCall call)
        {
            GlobalDb.Bindings.Delete(call.ObjectId);
        }

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

            if (ids != null && ids.Any())
            {
                foreach (var item in ids)
                {
                    GlobalDb.Bindings.Delete(item);
                }
                return true;
            }
            return false;
        }

        [Kooboo.Attributes.RequireParameters("SiteId")]
        public virtual void Post(ApiCall call)
        {
            string subdomain = call.GetValue("subdomain");
            string rootDomain = call.GetValue("rootdomain");
            Guid siteId = call.GetGuidValue("SiteId");
            int port = (int)call.GetLongValue("Port");

            if (string.IsNullOrEmpty(rootDomain))
            {
                return;
            }

            bool defaultBinding = call.GetBoolValue("DefaultBinding");

            if (siteId != default(Guid))
            {
                if (port <= 0)
                {
                    defaultBinding = false;
                }

                if (!defaultBinding)
                {
                    port = 0;
                }

                if (port > 0)
                {
                    if (!SystemStart.WebServers.ContainsKey(port) && Lib.Helper.NetworkHelper.IsPortInUse(port))
                    {
                        throw new Exception(Data.Language.Hardcoded.GetValue("port in use", call.Context) + ": " + port.ToString());
                    }
                }

                if (defaultBinding && port > 0)
                {
                    GlobalDb.Bindings.AddOrUpdate(null, null, siteId, call.Context.User.CurrentOrgId, defaultBinding, port);
                }
                else
                {
                    var domain = GlobalDb.Domains.Get(rootDomain);
                    if (domain.OrganizationId != default(Guid) && AppSettings.IsOnlineServer && domain.OrganizationId != call.Context.User.CurrentOrgId)
                    {
                        throw new Exception(Data.Language.Hardcoded.GetValue("Domain does not owned by current user", call.Context));
                    }

                    GlobalDb.Bindings.AddOrUpdate(rootDomain, subdomain, siteId, call.Context.User.CurrentOrgId, defaultBinding, port);
                }
            }
        }

        public List<BindingViewModel> ListBySite(Guid siteId, ApiCall call)
        {
            List<BindingViewModel> result = new List<BindingViewModel>();

            var list = GlobalDb.Bindings.GetByWebSite(siteId);

            foreach (var item in list)
            {
                BindingViewModel model = new BindingViewModel(item);
                model.EnableSsl = HasSsl(model.FullName);
                result.Add(model);
            }

            return result;
        }

        [Kooboo.Attributes.RequireParameters("domainid")]
        public List<BindingInfo> ListByDomain(ApiCall call)
        {
            Guid domainid = call.GetGuidValue("domainid");
            if (domainid != default(Guid))
            {
                var bindings = GlobalDb.Bindings.GetByDomain(domainid);
                List<BindingInfo> bindinginfos = new List<BindingInfo>();
                foreach (var item in bindings)
                {
                    BindingInfo info = new BindingInfo
                    {
                        Id = item.Id,
                        FullName = item.FullName,
                        SubDomain = item.SubDomain,
                        OrganizationId = item.OrganizationId
                    };

                    if (item.Port > 0 && item.Port != 80)
                    {
                        info.Port = item.Port;
                    }

                    var site = Kooboo.Data.GlobalDb.WebSites.Get(item.WebSiteId);
                    info.SiteName = site != null ? site.Name : "";
                    info.Device = item.Device;
                    info.DomainId = item.DomainId;
                    info.WebSiteId = item.WebSiteId;
                    bindinginfos.Add(info);
                }
                return bindinginfos;
            }

            return null;
        }

        public bool VerifySsl(string rootDomain, string subdomain, ApiCall call)
        {
            string fullname = null;
            if (rootDomain.StartsWith("."))
            {
                fullname = subdomain + rootDomain;
            }
            else
            {
                fullname = subdomain + "." + rootDomain;
            }

            var ok = Kooboo.Data.SSL.SslService.EnsureCheck(fullname);
            if (ok)
            {
                return true;
            }
            else
            {
                throw new Exception(Data.Language.Hardcoded.GetValue("Only internet enabled domain that use Kooboo DNS or hosted by Kooboo can generate SSL certificates"));
            }
        }

        public void SetSsl(string rootDomain, string subdomain, ApiCall call)
        {
            string fullname = null;
            if (rootDomain.StartsWith("."))
            {
                fullname = subdomain + rootDomain;
            }
            else
            {
                fullname = subdomain + "." + rootDomain;
            }

            Guid orgid = default(Guid);

            if (call.Context.WebSite != null)
            {
                orgid = call.Context.WebSite.OrganizationId;
            }
            else
            {
                if (call.Context.User != null)
                {
                    orgid = call.Context.User.CurrentOrgId;
                }
            }
            Kooboo.Data.SSL.SslService.SetSsl(fullname, orgid);
        }

        public List<SiteBindingViewModel> SiteBinding(ApiCall call)
        {
            var user = call.Context.User;
            if (user.CurrentOrgId == default(Guid))
            {
                return null;
            }

            var sites = Data.GlobalDb.WebSites.ListByOrg(user.CurrentOrgId);

            List<SiteBindingViewModel> result = new List<SiteBindingViewModel>();

            foreach (var item in sites)
            {
                var bindings = Data.GlobalDb.Bindings.GetByWebSite(item.Id);
                int count = 0;
                if (bindings != null)
                {
                    count = bindings.Count();
                }

                result.Add(new SiteBindingViewModel() { Name = item.Name, Id = item.Id, BindingCount = count });
            }

            return result;
        }

        private bool HasSsl(string fullDomain)
        {
            var item = Kooboo.Data.GlobalDb.SslCertificate.GetByDomain(fullDomain);
            return (item != null);
        }

        public class BindingViewModel
        {
            public BindingViewModel(Binding binding)
            {
                this.Id = binding.Id;
                this.OrganizationId = binding.OrganizationId;
                this.WebSiteId = binding.WebSiteId;
                this.SubDomain = binding.SubDomain;
                this.IpAddress = binding.IpAddress;
                this.DefaultPortBinding = binding.DefaultPortBinding;
                this.Device = binding.Device;
                this.DomainId = binding.DomainId;
                this.FullName = binding.FullName;
            }

            public bool EnableSsl { get; set; }

            public Guid Id
            {
                get; set;
            }

            public Guid OrganizationId { get; set; }

            public Guid WebSiteId;

            public string SubDomain { get; set; }

            public Guid DomainId { get; set; }

            public string FullName
            {
                get; set;
            }

            public string Device { get; set; }

            public string IpAddress
            {
                get; set;
            }

            // for default port binding..
            public int Port { get; set; } = 0;

            public bool DefaultPortBinding { get; set; }
        }
    }
}