//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Kooboo.Data.Hosts;
using Kooboo.Extensions;
using Kooboo.Lib.Helper;
using Kooboo.Data.Helper;

namespace Kooboo.Data.Repository
{
    public interface IDomainRepository
    {
        List<Domain> ListByUser(User user);

        List<Domain> ListByOrg(Guid OrganizationId); 

        List<Domain> ListForEmail(User user);

        Domain Get(Guid Id);

        Domain Get(string NameOrGuid);

        bool AddOrUpdate(Domain Domain);

        bool Delete(Domain Domain);

    }

    public class LocalDomainRepository : IDomainRepository
    {
        public LocalDomainRepository()
        {
            this.localcache = new Dictionary<Guid, Domain>();
            DomainRepositoryHelper.InitGlobal(this.localcache);
        }

        private object locker = new object();

        internal Dictionary<Guid, Domain> localcache { get; set; }

        private RepositoryBase<Domain> _domainstore;
        internal RepositoryBase<Domain> domainstore
        {
            get
            {
                if (_domainstore == null)
                {
                    _domainstore = new RepositoryBase<Domain>();
                }
                return _domainstore;
            }
        }

        public bool AddOrUpdate(Domain Domain)
        {
            string localip = "127.0.0.1";
            WindowsHost.AddOrUpdate(Domain.DomainName, localip);
            this.domainstore.AddOrUpdate(Domain);
            return true;
        }

        public bool Delete(Domain Domain)
        {
            try
            {
                var records = WindowsHost.GetList().Where(it => it.Domain.EndsWith(Domain.DomainName));
                foreach (var item in records)
                {
                    WindowsHost.Delete(item.Domain);
                    var domainid = IDGenerator.GetDomainId(item.Domain);
                    this.domainstore.Delete(domainid);
                }

                var id = IDGenerator.GetDomainId(Domain.DomainName);
                this.domainstore.Delete(id);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public Domain Get(string NameOrGuid)
        {
            Guid key;

            if (!System.Guid.TryParse(NameOrGuid, out key))
            {
                key = IDGenerator.GetDomainId(NameOrGuid);
            }
            return Get(key);
        }

        public Domain Get(Guid Id)
        {
            if (!localcache.ContainsKey(Id))
            {
                lock (locker)
                {
                    if (!localcache.ContainsKey(Id))
                    {
                        var domain = this.domainstore.Get(Id);
                        if (domain != null)
                        {
                            localcache[Id] = domain;
                        }
                        else
                        {
                            var alllocals = LocalHostDomains();

                            if (alllocals.ContainsKey(Id))
                            {
                                localcache[Id] = new Domain() { DomainName = alllocals[Id] };
                            }
                        }
                    }
                }
            }
            if (localcache.ContainsKey(Id))
            {
                return localcache[Id];
            }
            return null;
        }

        public List<Domain> ListByUser(User user)
        {

            List<Domain> result = new List<Domain>();

            result.Add(new Domain() { Id = IDGenerator.GetDomainId(AppSettings.DefaultLocalHost), DomainName = AppSettings.DefaultLocalHost }); 
              

            var all = this.domainstore.All();
            foreach (var item in all.Where(o=>o.OrganizationId == user.CurrentOrgId))
            {
                if (!string.IsNullOrEmpty(item.DomainName) && !item.DomainName.ToLower().EndsWith(AppSettings.DefaultLocalHost))
                {
                    result.Add(item); 
                     
                }
            }

            return result; 
        }

        private Dictionary<Guid, string> LocalHostDomains()
        {
            var LocalDomains = new Dictionary<Guid, string>();

            var domains = Kooboo.Data.Hosts.WindowsHost.GetList();
            foreach (var item in domains)
            {
                var domainresult = Kooboo.Data.Helper.DomainHelper.Parse(item.Domain);

                if (!string.IsNullOrEmpty(domainresult.Domain))
                {
                    Guid id = domainresult.Domain.ToHashGuid();
                    if (!LocalDomains.ContainsKey(id))
                    {
                        LocalDomains.Add(id, domainresult.Domain);
                    }
                }
            }

            var localhostid = AppSettings.DefaultLocalHost.ToHashGuid();
            if (!LocalDomains.ContainsKey(localhostid))
            {
                LocalDomains.Add(localhostid, AppSettings.DefaultLocalHost);
            }

            return LocalDomains;
        }

        public List<Domain> ListForEmail(User user)
        {
            return this.domainstore.All().Where(o => o.OrganizationId == user.CurrentOrgId).ToList();
        }

        public List<Domain> ListByOrg(Guid OrganizationId)
        {

            List<Domain> result = new List<Domain>();

            result.Add(new Domain() { Id = IDGenerator.GetDomainId(AppSettings.DefaultLocalHost), DomainName = AppSettings.DefaultLocalHost });


            var all = this.domainstore.All();
            foreach (var item in all.Where(o => o.OrganizationId == OrganizationId))
            {
                if (!string.IsNullOrEmpty(item.DomainName) && !item.DomainName.ToLower().EndsWith(AppSettings.DefaultLocalHost))
                {
                    result.Add(item);

                }
            }

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
