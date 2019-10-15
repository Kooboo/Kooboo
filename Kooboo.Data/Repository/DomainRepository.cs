//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Data.Helper;
using Kooboo.Data.Hosts;
using Kooboo.Data.Models;
using Kooboo.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kooboo.Data.Repository
{
    public interface IDomainRepository
    {
        List<Domain> ListByUser(User user);

        List<Domain> ListByOrg(Guid organizationId);

        List<Domain> ListForEmail(User user);

        Domain Get(Guid id);

        Domain Get(string nameOrGuid);

        bool AddOrUpdate(Domain domain);

        bool Delete(Domain domain);
    }

    public class LocalDomainRepository : IDomainRepository
    {
        public LocalDomainRepository()
        {
            this.Localcache = new Dictionary<Guid, Domain>();
            DomainRepositoryHelper.InitGlobal(this.Localcache);
        }

        private object locker = new object();

        internal Dictionary<Guid, Domain> Localcache { get; set; }

        private RepositoryBase<Domain> _domainstore;

        internal RepositoryBase<Domain> Domainstore => _domainstore ?? (_domainstore = new RepositoryBase<Domain>());

        public bool AddOrUpdate(Domain domain)
        {
            string localip = "127.0.0.1";
            WindowsHost.AddOrUpdate(domain.DomainName, localip);
            this.Domainstore.AddOrUpdate(domain);
            return true;
        }

        public bool Delete(Domain domain)
        {
            try
            {
                var records = WindowsHost.GetList().Where(it => it.Domain.EndsWith(domain.DomainName));
                foreach (var item in records)
                {
                    WindowsHost.Delete(item.Domain);
                    var domainid = IDGenerator.GetDomainId(item.Domain);
                    this.Domainstore.Delete(domainid);
                }

                var id = IDGenerator.GetDomainId(domain.DomainName);
                this.Domainstore.Delete(id);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public Domain Get(string nameOrGuid)
        {
            if (!System.Guid.TryParse(nameOrGuid, out var key))
            {
                key = IDGenerator.GetDomainId(nameOrGuid);
            }
            return Get(key);
        }

        public Domain Get(Guid id)
        {
            if (!Localcache.ContainsKey(id))
            {
                lock (locker)
                {
                    if (!Localcache.ContainsKey(id))
                    {
                        var domain = this.Domainstore.Get(id);
                        if (domain != null)
                        {
                            Localcache[id] = domain;
                        }
                        else
                        {
                            var alllocals = LocalHostDomains();

                            if (alllocals.ContainsKey(id))
                            {
                                Localcache[id] = new Domain() { DomainName = alllocals[id] };
                            }
                        }
                    }
                }
            }
            if (Localcache.ContainsKey(id))
            {
                return Localcache[id];
            }
            return null;
        }

        public List<Domain> ListByUser(User user)
        {
            List<Domain> result = new List<Domain>
            {
                new Domain()
                {
                    Id = IDGenerator.GetDomainId(AppSettings.DefaultLocalHost),
                    DomainName = AppSettings.DefaultLocalHost
                }
            };


            var all = this.Domainstore.All();
            result.AddRange(all.Where(o => o.OrganizationId == user.CurrentOrgId).Where(item => !string.IsNullOrEmpty(item.DomainName) && !item.DomainName.ToLower().EndsWith(AppSettings.DefaultLocalHost)));

            return result;
        }

        private Dictionary<Guid, string> LocalHostDomains()
        {
            var localDomains = new Dictionary<Guid, string>();

            var domains = Kooboo.Data.Hosts.WindowsHost.GetList();
            foreach (var item in domains)
            {
                var domainresult = Kooboo.Data.Helper.DomainHelper.Parse(item.Domain);

                if (!string.IsNullOrEmpty(domainresult.Domain))
                {
                    Guid id = domainresult.Domain.ToHashGuid();
                    if (!localDomains.ContainsKey(id))
                    {
                        localDomains.Add(id, domainresult.Domain);
                    }
                }
            }

            var localhostid = AppSettings.DefaultLocalHost.ToHashGuid();
            if (!localDomains.ContainsKey(localhostid))
            {
                localDomains.Add(localhostid, AppSettings.DefaultLocalHost);
            }

            return localDomains;
        }

        public List<Domain> ListForEmail(User user)
        {
            return this.Domainstore.All().Where(o => o.OrganizationId == user.CurrentOrgId).ToList();
        }

        public List<Domain> ListByOrg(Guid organizationId)
        {
            List<Domain> result = new List<Domain>
            {
                new Domain()
                {
                    Id = IDGenerator.GetDomainId(AppSettings.DefaultLocalHost),
                    DomainName = AppSettings.DefaultLocalHost
                }
            };


            var all = this.Domainstore.All();
            result.AddRange(all.Where(o => o.OrganizationId == organizationId).Where(item => !string.IsNullOrEmpty(item.DomainName) && !item.DomainName.ToLower().EndsWith(AppSettings.DefaultLocalHost)));

            return result;
        }
    }

    public static class DomainRepositoryHelper
    {
        public static void InitGlobal(Dictionary<Guid, Domain> localcachelist)
        {
            string name = AppSettings.ThemeDomain;
            if (name != null && name.Contains("."))
            {
                name = DomainHelper.GetRootDomain(name);
            }

            if (!string.IsNullOrEmpty(name))
            {
                Guid hash = Lib.Security.Hash.ComputeGuidIgnoreCase(name);
                Domain domain = new Domain() { DomainName = name, ExpirationDate = DateTime.Now.AddYears(100) };
                localcachelist[hash] = domain;
            }

            name = AppSettings.HostDomain;
            if (name != null && name.Contains("."))
            {
                name = DomainHelper.GetRootDomain(name);
            }

            if (!string.IsNullOrEmpty(name))
            {
                Guid hash = Lib.Security.Hash.ComputeGuidIgnoreCase(name);
                Domain domain = new Domain() { DomainName = name, ExpirationDate = DateTime.Now.AddYears(100) };
                localcachelist[hash] = domain;
            }
        }
    }
}