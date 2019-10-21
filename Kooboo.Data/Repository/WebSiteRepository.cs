//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Data.Models;
using Kooboo.IndexedDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Kooboo.Data.Repository
{
    public class WebSiteRepository : RepositoryBase<WebSite>
    {
        protected override ObjectStoreParameters StoreParameters
        {
            get
            {
                ObjectStoreParameters paras = new ObjectStoreParameters();
                paras.AddIndex<WebSite>(o => o.Name);
                paras.AddColumn<WebSite>(o => o.OrganizationId);
                paras.AddColumn<WebSite>(o => o.EnableCluster);
                paras.AddColumn<WebSite>(o => o.EnableFullTextSearch);
                paras.AddColumn<WebSite>(o => o.EnableDiskSync);
                paras.AddColumn<WebSite>(o => o.EnableECommerce);
                paras.SetPrimaryKeyField<WebSite>(o => o.Id);
                return paras;
            }
        }

        private object _locker = new object();

        public Dictionary<Guid, WebSite> Cachedsites;

        //cache of all local websites...
        public Dictionary<Guid, WebSite> AllSites
        {
            get
            {
                if (Cachedsites == null)
                {
                    lock (_locker)
                    {
                        if (Cachedsites == null)
                        {
                            Cachedsites = new Dictionary<Guid, WebSite>();
                            var sites = Data.GlobalDb.WebSites.All();
                            foreach (WebSite item in sites)
                            {
                                Cachedsites[item.Id] = item;
                            }
                        }
                    }
                }
                return Cachedsites;
            }
        }

        public override WebSite Get(Guid id, bool getColumnDataOnly = false)
        {
            AllSites.TryGetValue(id, out var site);
            if (site == null)
            {
                site = this.Store.get(id);
                if (site != null)
                {
                    this.AllSites[site.Id] = site;
                }
            }
            return site;
        }

        public void UpdateBoolColumn(Guid id, Expression<Func<WebSite, object>> expression, bool Value)
        {
            string fieldname = Kooboo.IndexedDB.Helper.ExpressionHelper.GetFieldName<WebSite>(expression);

            string enablecluser = IndexedDB.Helper.ExpressionHelper.GetFieldName<WebSite>(o => o.EnableCluster);
            string enablefulltext = IndexedDB.Helper.ExpressionHelper.GetFieldName<WebSite>(o => o.EnableFullTextSearch);
            string enabledisksync = IndexedDB.Helper.ExpressionHelper.GetFieldName<WebSite>(o => o.EnableDiskSync);

            if (fieldname == null)
            {
                return;
            }
            var site = this.Get(id);
            if (site != null)
            {
                this.Store.UpdateColumn<bool>(id, expression, Value);

                if (fieldname == enablecluser)
                {
                    site.EnableCluster = Value;
                }
                else if (fieldname == enabledisksync)
                {
                    site.EnableDiskSync = Value;
                }
                else if (fieldname == enablefulltext)
                {
                    site.EnableFullTextSearch = Value;
                }
            }
        }

        public override bool AddOrUpdate(WebSite value)
        {
            // check quota control.
            var maxsites = Kooboo.Data.Authorization.QuotaControl.MaxSites(value.OrganizationId);
            if (maxsites != int.MaxValue)
            {
                var counts = this.ListByOrg(value.OrganizationId).Count;
                if (counts >= maxsites)
                {
                    var found = this.Get(value.Id);

                    if (found == null)
                    {
                        throw new Exception(Kooboo.Data.Language.Hardcoded.GetValue("Max number of sites has been reached, require service level upgrade"));
                    }
                }
            }

            var ok = base.AddOrUpdate(value);

            lock (_locker)
            {
                this.AllSites[value.Id] = value;
            }
            return ok;
        }

        public override void Delete(Guid id)
        {
            lock (_locker)
            {
                base.Delete(id);

                var bindings = Data.GlobalDb.Bindings.GetByWebSite(id);
                foreach (var item in bindings)
                {
                    Data.GlobalDb.Bindings.Delete(item);
                }

                this.AllSites.Remove(id);
            }
        }

        public bool CheckNameAvailable(string webSiteName, Guid organizationId)
        {
            string lowerCaseSiteName = webSiteName.ToLower();

            return !this.AllSites.Values.ToList().Any(o => o.OrganizationId == organizationId && o.Name.ToLower() == lowerCaseSiteName);
        }

        public List<WebSite> ListByOrg(Guid orgId)
        {
            return this.AllSites.Values.Where(o => o.OrganizationId == orgId).OrderByDescending(it => it.CreationDate).ToList();
        }

        public IEnumerable<WebSite> GetLocalSites()
        {
            return TableScan.Where(o => !string.IsNullOrEmpty(o.LocalRootPath))
                .SelectAll()
                .OrderBy(it => it.Name);
        }

        public WebSite AddNewWebSite(Guid organiztionId, string websitename, string hostName)
        {
            WebSite newsite = new WebSite
            {
                Name = websitename,
                OrganizationId = organiztionId
            };

            Data.GlobalDb.WebSites.AddOrUpdate(newsite);

            Data.GlobalDb.Bindings.AddOrUpdate(hostName, newsite.Id, organiztionId);

            return newsite;
        }
    }
}