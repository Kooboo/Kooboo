//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Sites.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kooboo.Extensions;
using Kooboo.Sites.Models;
using Kooboo.Data.Interface;

namespace Kooboo.Sites.Sync.Cluster
{
    public static class Integrity
    {
        /// <summary>
        /// Add the object sender information... this information should be append to all synchronization... 
        /// If this object received from remote, appender the remote sender and tick, otherwise append local. 
        /// </summary>
        /// <param name="SiteDb"></param>
        /// <param name="SyncObject"></param>
        /// <param name="LocalVersion"></param>
        public static void Generate(SiteDb SiteDb, SyncObject SyncObject, long LocalVersion)
        {
            /// Check and use the local version number. 
            if (SiteDb != null && SyncObject != null)
            {
                // first check log to see if there are others..  
                 
                var store = Stores.ClusterUpdateHistory(SiteDb);
                var items = store.Where(o => o.LocalVersion == LocalVersion).SelectAll();

                if (items != null && items.Count() > 0)
                {
                    if (items.Count() > 1)
                    {
                        throw new Exception(Data.Language.Hardcoded.GetValue("cluster should not have two source for the same item"));
                    }
                    else
                    {
                        var item = items[0];
                        SyncObject.Sender = item.Sender;
                        SyncObject.SenderTick = item.SenderTick;
                        return;
                    }

                }
                else
                {
                    SyncObject.Sender  = Lib.Security.Hash.ComputeIntCaseSensitive(SiteDb.WebSite.Id.ToString());
                    SyncObject.SenderTick = DateTime.UtcNow.Ticks; 
                }

            }
        }

        /// <summary>
        /// verify whether this item can be should be added into the local repository or not.
        /// Rule: if local contains a newer version, ignore, otherwise add, update or delete. 
        /// </summary>
        /// <param name="SiteDb"></param>
        /// <param name="SyncObject"></param>
        /// <returns></returns>
        public static bool Verify(SiteDb SiteDb, SyncObject SyncObject)
        {

            var modeltype = Kooboo.Sites.Service.ConstTypeService.GetModelType(SyncObject.ObjectConstType);

            var repo = SiteDb.GetRepository(modeltype);

            var historystore = Stores.ClusterUpdateHistory(SiteDb);

            var logs = historystore.Where(it => it.ObjectId == SyncObject.ObjectId).SelectAll();

            if (logs == null || logs.Count == 0)
            {
                return true;
            }

            var lastlog = logs.OrderByDescending(o => o.SenderTick).First();

            if (lastlog.SenderTick > SyncObject.SenderTick)
            {
                return false;
            }

            return true;
        }

        public static void AddHistory(SiteDb SiteDb, SyncObject SyncObject, ISiteObject SiteObject)
        {

            NodeUpdate update = new NodeUpdate();
            update.ObjectId = SyncObject.ObjectId;
            update.ObjectConstType = SyncObject.ObjectConstType;
            update.IsDelete = SyncObject.IsDelete;
            update.Language = SyncObject.Language;
            update.Sender = SyncObject.Sender;
            update.SenderTick = SyncObject.SenderTick;

            if (SiteObject is CoreObject)
            {
                var core = SiteObject as CoreObject;
                update.LocalVersion = core.Version;
            }

            var store = Stores.ClusterUpdateHistory(SiteDb);

            var old = store.get(update.Id);
            if (old == null)
            {
                store.add(update.Id, update);
            } 
        }
    }
}
