//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Data.Interface;
using Kooboo.Sites.Models;
using Kooboo.Sites.Repository;
using System;
using System.Linq;

namespace Kooboo.Sites.Sync.Cluster
{
    public static class Integrity
    {
        /// <summary>
        /// Add the object sender information... this information should be append to all synchronization...
        /// If this object received from remote, appender the remote sender and tick, otherwise append local.
        /// </summary>
        /// <param name="siteDb"></param>
        /// <param name="syncObject"></param>
        /// <param name="localVersion"></param>
        public static void Generate(SiteDb siteDb, SyncObject syncObject, long localVersion)
        {
            // Check and use the local version number.
            if (siteDb != null && syncObject != null)
            {
                // first check log to see if there are others..

                var store = Stores.ClusterUpdateHistory(siteDb);
                var items = store.Where(o => o.LocalVersion == localVersion).SelectAll();

                if (items != null && items.Any())
                {
                    if (items.Count() > 1)
                    {
                        throw new Exception(Data.Language.Hardcoded.GetValue("cluster should not have two source for the same item"));
                    }
                    else
                    {
                        var item = items[0];
                        syncObject.Sender = item.Sender;
                        syncObject.SenderTick = item.SenderTick;
                        return;
                    }
                }
                else
                {
                    syncObject.Sender = Lib.Security.Hash.ComputeIntCaseSensitive(siteDb.WebSite.Id.ToString());
                    syncObject.SenderTick = DateTime.UtcNow.Ticks;
                }
            }
        }

        /// <summary>
        /// verify whether this item can be should be added into the local repository or not.
        /// Rule: if local contains a newer version, ignore, otherwise add, update or delete.
        /// </summary>
        /// <param name="siteDb"></param>
        /// <param name="syncObject"></param>
        /// <returns></returns>
        public static bool Verify(SiteDb siteDb, SyncObject syncObject)
        {
            var modeltype = Kooboo.Sites.Service.ConstTypeService.GetModelType(syncObject.ObjectConstType);

            var repo = siteDb.GetRepository(modeltype);

            var historystore = Stores.ClusterUpdateHistory(siteDb);

            var logs = historystore.Where(it => it.ObjectId == syncObject.ObjectId).SelectAll();

            if (logs == null || logs.Count == 0)
            {
                return true;
            }

            var lastlog = logs.OrderByDescending(o => o.SenderTick).First();

            return lastlog.SenderTick <= syncObject.SenderTick;
        }

        public static void AddHistory(SiteDb siteDb, SyncObject syncObject, ISiteObject siteObject)
        {
            NodeUpdate update = new NodeUpdate
            {
                ObjectId = syncObject.ObjectId,
                ObjectConstType = syncObject.ObjectConstType,
                IsDelete = syncObject.IsDelete,
                Language = syncObject.Language,
                Sender = syncObject.Sender,
                SenderTick = syncObject.SenderTick
            };

            if (siteObject is CoreObject core)
            {
                update.LocalVersion = core.Version;
            }

            var store = Stores.ClusterUpdateHistory(siteDb);

            var old = store.get(update.Id);
            if (old == null)
            {
                store.add(update.Id, update);
            }
        }
    }
}