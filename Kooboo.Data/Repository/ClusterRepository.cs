//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Data.Models;
using Kooboo.IndexedDB;
using System;
using System.Collections.Generic;

namespace Kooboo.Data.Repository
{
    public class ClusterRepository : RepositoryBase<Cluster>
    {
        protected override ObjectStoreParameters StoreParameters
        {
            get
            {
                ObjectStoreParameters paras = new ObjectStoreParameters();
                paras.AddColumn<Cluster>(o => o.WebSiteId);
                paras.AddColumn<Cluster>(o => o.OrganizationId);
                paras.AddColumn<Cluster>(o => o.ServerId);
                paras.SetPrimaryKeyField<Cluster>(o => o.Id);
                return paras;
            }
        }

        private static object _locker = new object();

        internal Dictionary<Guid, List<Cluster>> Cache = new Dictionary<Guid, List<Cluster>>();

        internal void AddUpdateCache(Cluster newCluster)
        {
            if (Cache.ContainsKey(newCluster.WebSiteId))
            {
                var list = Cache[newCluster.WebSiteId];

                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i].Id == newCluster.Id)
                    {
                        list[i] = newCluster;
                        return;
                    }
                }

                list.Add(newCluster);
            }
            else
            {
                var currentresult = this.Query.Where(o => o.WebSiteId == newCluster.WebSiteId).SelectAll() ?? new List<Cluster>();

                currentresult.Add(newCluster);
                Cache[newCluster.WebSiteId] = currentresult;
            }
        }

        public List<Cluster> ListByWebSite(Guid websiteid)
        {
            lock (_locker)
            {
                if (this.Cache.ContainsKey(websiteid))
                {
                    return this.Cache[websiteid];
                }

                var result = this.Query.Where(o => o.WebSiteId == websiteid).SelectAll() ?? new List<Cluster>();
                this.Cache[websiteid] = result;
                return result;
            }
        }

        internal void Removecache(Guid webSiteId, Guid cluserId)
        {
            if (Cache.ContainsKey(webSiteId))
            {
                var list = Cache[webSiteId];

                var item = list.Find(o => o.Id == cluserId);
                if (item != null)
                {
                    list.Remove(item);
                }
            }
        }

        public override bool AddOrUpdate(Cluster value)
        {
            var old = Store.get(value.Id);
            if (old == null)
            {
                AddUpdateCache(value);

                Store.add(value.Id, value);
                RaiseEvent(value, ChangeType.Add, default(Cluster));

                Store.Close();
                return true;
            }

            if (!IsEqual(old, value))
            {
                AddUpdateCache(value);

                Store.update(value.Id, value);
                RaiseEvent(value, ChangeType.Update, old);

                Store.Close();
                return true;
            }

            Store.Close();
            return false;
        }

        public override void Delete(Cluster value)
        {
            base.Delete(value);
            Removecache(value.WebSiteId, value.Id);
        }

        public void Delete(Guid webSiteId, Guid clusterId)
        {
            base.Delete(clusterId);
            Removecache(webSiteId, clusterId);
        }

        public override void Delete(Guid id)
        {
            throw new Exception("not allowed");
        }
    }
}