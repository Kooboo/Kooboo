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

            if (ids != null && ids.Count() > 0)
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
            string RootDomain = call.GetValue("rootdomain");
            Guid SiteId = call.GetGuidValue("SiteId");
            int port = (int)call.GetLongValue("Port");

            if (string.IsNullOrEmpty(RootDomain))
            {
                return;
            }

            bool DefaultBinding = call.GetBoolValue("DefaultBinding");

            if (SiteId != default(Guid))
            {

                if (port <= 0)
                {
                    DefaultBinding = false;
                }


                if (!DefaultBinding)
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
                  

                if (DefaultBinding && port > 0)
                {

                    GlobalDb.Bindings.AddOrUpdate(null, null, SiteId, call.Context.User.CurrentOrgId, DefaultBinding, port);
                }
                else
                {

                    var domain = GlobalDb.Domains.Get(RootDomain);
                    if (domain.OrganizationId != default(Guid) && AppSettings.IsOnlineServer && domain.OrganizationId != call.Context.User.CurrentOrgId)
                    {
                        throw new Exception(Data.Language.Hardcoded.GetValue("Domain does not owned by current user", call.Context));
                    }

                    GlobalDb.Bindings.AddOrUpdate(RootDomain, subdomain, SiteId, call.Context.User.CurrentOrgId, DefaultBinding, port);

                } 

            }
        }

        [Kooboo.Attributes.RequireParameters("siteid")]
        public List<Binding> ListBySite(ApiCall call)
        {
            Guid siteid = call.GetGuidValue("SiteId");
            if (siteid != default(Guid))
            {
                return GlobalDb.Bindings.GetByWebSite(siteid);
            }
            return null;
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
                    BindingInfo info = new BindingInfo();
                    info.Id = item.Id;
                    info.FullName = item.FullName;
                    info.SubDomain = item.SubDomain;
                    info.OrganizationId = item.OrganizationId;

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

    }
}
