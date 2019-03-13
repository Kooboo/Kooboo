//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Sites.Models;
using System;
using Kooboo.IndexedDB;
using System.Collections.Generic;
using System.Collections;

namespace Kooboo.Sites.Repository
{
    public class SiteClusterRepository : SiteRepositoryBase<SiteCluster>
    {
        public override ObjectStoreParameters StoreParameters
        {
            get
            {
                ObjectStoreParameters paras = new ObjectStoreParameters();
                paras.AddColumn<SiteCluster>(o => o.ServerIp);
                paras.AddColumn<SiteCluster>(o => o.Version);
                paras.SetPrimaryKeyField<SiteCluster>(o => o.Id);
                return paras;
            }
        }

        public SiteCluster GetByIp(string ip)
        {
            var all = this.All();
            foreach (var item in all)
            {
                if (item.ServerIp == ip)
                {
                    return item;
                }
            }

            foreach (var item in all)
            {
                if (Lib.Helper.IPHelper.IsInSameCClass(item.ServerIp, ip))
                {
                    return item;
                }
            }

            return null;
        }

        public void UpdateVersion(Guid SiteClusterId, long newversion)
        {
            this.Store.UpdateColumn<long>(SiteClusterId, o => o.Version, newversion);
        }

        public override bool AddOrUpdate(SiteCluster value)
        {
            var exists = this.Get(value.Id);
            if (exists == null)
            {
                return Add(value);
            }
            else
            {
                return Update(exists, value);
            }
        }

        public bool Add(SiteCluster newcluster)
        { 
            var ok =  base.AddOrUpdate(newcluster);

            if (ok)
            {
                // TODO: should notfify DNS for this change.  
                 this.SiteDb.ClusterManager.InitStart(); 
            }
            return ok; 
        }
         
        public bool Update(SiteCluster oldvalue, SiteCluster newvalue)
        {
            oldvalue.Name = newvalue.Name;
            oldvalue.DataCenter = newvalue.DataCenter;

            return base.AddOrUpdate(oldvalue);
        }
         
        public override void Delete(Guid id)
        {
            //TODO: 
            // Should notify the remote Server to remove it, and also if online, notify DNS change. 
            base.Delete(id);
        }

    }

}
