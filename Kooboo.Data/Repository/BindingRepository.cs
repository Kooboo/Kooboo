//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using Kooboo.IndexedDB;
using Kooboo.Data.Models;
using Kooboo.Extensions;
using Kooboo.Data.Hosts;

namespace Kooboo.Data.Repository
{
    public class BindingRepository : RepositoryBase<Binding>
    {
        private readonly object writelock = new object();

        protected override ObjectStoreParameters StoreParameters
        {
            get
            {
                ObjectStoreParameters paras = new ObjectStoreParameters();
                paras.AddIndex<Binding>(o => o.WebSiteId);
                paras.AddColumn<Binding>(o => o.DomainId);
                paras.AddColumn<Binding>(o => o.OrganizationId);
                paras.AddColumn<Binding>(o => o.IpAddress);
                paras.SetPrimaryKeyField<Binding>(o => o.Id);
                return paras;
            }
        }


        private Dictionary<Guid, Binding> _localcache;
        public Dictionary<Guid, Binding> LocalCache
        {
            get
            {
                if (_localcache == null)
                {
                    lock (writelock)
                    {
                        if (_localcache == null)
                        {
                            _localcache = new Dictionary<Guid, Binding>();
                            foreach (Binding item in GlobalDb.Bindings.All())
                            {
                                _localcache[item.Id] = item;
                            }
                        }
                    }
                }
                return _localcache;
            }
        }

        public override Binding Get(Guid id, bool getColumnDataOnly = false)
        {
            return LocalCache.GetValueOrDefault(id);
        }

        public override bool AddOrUpdate(Binding value)
        {
            var ok = base.AddOrUpdate(value);
            if (ok)
            {
                lock (writelock)
                {
                    LocalCache[value.Id] = value;
                    if (!AppSettings.IsOnlineServer)
                    {
                        var fullname = value.FullName;
                        WindowsHost.AddOrUpdate(fullname, "127.0.0.1");
                    }
                }
            }
            return ok;
        }

        public override void Delete(Binding value)
        {
            Delete(value.Id);
        }

        public override void Delete(Guid id)
        {
            base.Delete(id);
            var find = LocalCache.GetValueOrDefault(id);
            if (find != null)
            {
                LocalCache.Remove(id);
                if (!AppSettings.IsOnlineServer)
                {
                    WindowsHost.Delete(find.FullName);
                }
            }
        }

        public void AddOrUpdate(string rootDomain, string subDomain, Guid webSiteId, Guid organizationId, bool defaultbinding = false, int port = 0)
        {
            Guid domainid = default(Guid);

            if (!string.IsNullOrEmpty(rootDomain))
            {
                domainid = IDGenerator.GetDomainId(rootDomain);
            }

            if (!AppSettings.IsOnlineServer && domainid != default(Guid))
            {
                var domain = GlobalDb.Domains.Get(domainid);
                if (domain == null)
                {
                    WindowsHost.AddOrUpdate(rootDomain, "127.0.0.1");
                    GlobalDb.Domains.AddOrUpdate(new Domain()
                    {
                        DomainName = rootDomain,
                        OrganizationId = organizationId
                    });
                }
            }

            Binding binding = new Binding
            {
                DomainId = domainid,
                OrganizationId = organizationId,
                SubDomain = string.IsNullOrEmpty(subDomain) ? null : subDomain.ToLower(),
                WebSiteId = webSiteId,
                DefaultPortBinding = defaultbinding,
                Port = port
            };

            var old = this.Get(binding.Id);
            if (old != null)
            {
                throw new Exception("binding already exists");
            }
            else
            {
                AddOrUpdate(binding);
            }
        }

        public void AddOrUpdate(string FullHostName, Guid WebSiteId, Guid OrgId)
        {
            if (FullHostName.StartsWith("."))
            {
                FullHostName = FullHostName.Substring(1);
            }
            var domainresult = Kooboo.Data.Helper.DomainHelper.Parse(FullHostName);
            AddOrUpdate(domainresult.Domain, domainresult.SubDomain, WebSiteId, OrgId);
        }

        public List<Binding> GetByWebSite(Guid webSiteId)
        {
            return LocalCache
                .Values
                .Where(o => o.WebSiteId == webSiteId)
                .ToList();
        }

        public List<Binding> GetByDomain(string topDomain)
        {
            Guid domainid;
            if (!Guid.TryParse(topDomain, out domainid))
            {
                domainid = IDGenerator.GetDomainId(topDomain);
            }
            return GetByDomain(domainid);
        }

        public List<Binding> GetByDomain(Guid domainId)
        {
            return LocalCache.Values.Where(o => o.DomainId == domainId).ToList();
        }

        /// <summary>
        /// Get all the bindings that belong to the full domains. 
        /// </summary>
        /// <param name="fullDomain"></param>
        /// <returns></returns>
        public List<Binding> GetByFullDomain(string fullDomain)
        {
            var domainresult = Kooboo.Data.Helper.DomainHelper.Parse(fullDomain);

            return GetByDomainResult(domainresult);
        }

        public List<Binding> GetByDomainResult(Helper.DomainResult domainresult)
        {
            List<Binding> result = new List<Binding>();

            if (string.IsNullOrEmpty(domainresult.Domain))
            {
                return result;
            }

            if (!string.IsNullOrEmpty(domainresult.SubDomain))
            {
                domainresult.SubDomain = domainresult.SubDomain.ToLower();
            }

            var allresults = GetByDomain(domainresult.Domain);

            foreach (var item in allresults)
            {
                if (Lib.Helper.StringHelper.IsSameValue(item.SubDomain, domainresult.SubDomain))
                {
                    result.Add(item);
                }
            }

            if (result.Count() == 0 && allresults.Count() > 0 && string.IsNullOrEmpty(domainresult.SubDomain))
            {
                foreach (var item in allresults)
                {
                    if (item.SubDomain.ToLower() == "www")
                    {
                        result.Add(item);
                    }
                }
            }
            return result;
        }

        public void EnsureLocalBinding()
        {
            if (!AppSettings.IsOnlineServer)
            {
                var allbindings = GlobalDb.Bindings.All();
                foreach (var item in allbindings)
                {
                    var domain = GlobalDb.Domains.Get(item.DomainId);
                    if (domain != null)
                    {
                        string fullname = domain.DomainName;
                        if (!string.IsNullOrEmpty(item.SubDomain))
                        {
                            fullname = item.SubDomain + "." + fullname;
                        }
                        WindowsHost.AddOrUpdate(fullname, "127.0.0.1");
                    }
                }

            }

            WindowsHost.AddOrUpdate(AppSettings.DefaultLocalHost, "127.0.0.1");
            WindowsHost.AddOrUpdate(AppSettings.StartHost, "127.0.0.1");
        }
    }
}
