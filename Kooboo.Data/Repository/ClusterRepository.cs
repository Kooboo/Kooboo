//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Models;
using Kooboo.IndexedDB;
using System.Collections.Generic;
using System;

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

        private object _locker = new object();

        internal Dictionary<Guid, List<Cluster>> cache = new Dictionary<Guid, List<Cluster>>();

        internal void AddUpdateCache(Cluster newCluster)
        {
            if (cache.ContainsKey(newCluster.WebSiteId))
            {
                var list = cache[newCluster.WebSiteId];

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
                var currentresult = this.Query.Where(o => o.WebSiteId == newCluster.WebSiteId).SelectAll(); 

                if (currentresult == null)
                {
                    currentresult = new List<Cluster>(); 
                }

                currentresult.Add(newCluster);
                cache[newCluster.WebSiteId] = currentresult;
            }
        }

        public List<Cluster> ListByWebSite(Guid websiteid)
        {
           lock(_locker)
            {
                if (this.cache.ContainsKey(websiteid))
                {
                    return this.cache[websiteid]; 
                }
                else
                {
                    var result = this.Query.Where(o => o.WebSiteId == websiteid).SelectAll();
                    if (result == null)
                    {
                        result = new List<Cluster>(); 
                    }
                    this.cache[websiteid] = result; 
                    return result; 
                }  
            }
        }

        internal void Removecache(Guid WebSiteId, Guid CluserId)
        {
            if (cache.ContainsKey(WebSiteId))
            {
                var list = cache[WebSiteId];

                var item = list.Find(o => o.Id == CluserId);
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
            else
            {
                if (!IsEqual(old, value))
                {
                    AddUpdateCache(value);

                    Store.update(value.Id, value);
                    RaiseEvent(value, ChangeType.Update, old);
                     
                    Store.Close();
                    return true;
                }
            }

            Store.Close();
            return false;
        }

        public override void Delete(Cluster value)
        {
            base.Delete(value);
            Removecache(value.WebSiteId, value.Id);
        }

        public void Delete(Guid WebSiteId, Guid ClusterId)
        {
            base.Delete(ClusterId);
            Removecache(WebSiteId, ClusterId);
        }

        public override void Delete(Guid id)
        {
            throw new Exception("not allowed");
        }
    }
}
