//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Data.Interface;
using Kooboo.Extensions;
using Kooboo.IndexedDB;
using Kooboo.Sites.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kooboo.Sites.Repository
{
    public class SynchronizationRepository : SiteRepositoryBase<Synchronization>
    {
        public Guid DiskSyncSettingId = ("___kooboodisksync" + Environment.MachineName).ToHashGuid();

        public Guid GlobalObjectId = default(Guid);

        public override ObjectStoreParameters StoreParameters
        {
            get
            {
                ObjectStoreParameters paras = new ObjectStoreParameters();
                paras.AddColumn<Synchronization>(it => it.ObjectId);
                paras.AddColumn<Synchronization>(o => o.SyncSettingId);
                paras.AddColumn<Synchronization>(o => o.In);
                paras.AddIndex<Synchronization>(o => o.Version);
                paras.AddIndex<Synchronization>(o => o.RemoteVersion);
                paras.SetPrimaryKeyField<Synchronization>(o => o.Id);

                return paras;
            }
        }

        //Get items to push to remote..
        public List<LogEntry> GetPushItems(Guid syncSettingId)
        {
            // var currentlogid = GetLastOut(SyncSettingId);
            var currentlogid = -1;
            List<LogEntry> result = new List<LogEntry>();
            Dictionary<Guid, long> objectLastSyncVersion = new Dictionary<Guid, long>();

            long lastVersion = -1;

            var alllogs = this.SiteDb.Log.Store.Where(o => o.Id > currentlogid).SelectAll();
            var allversion = this.SiteDb.Synchronization.Query.Where(o => o.SyncSettingId == syncSettingId).SelectAll();
            var pastversion = allversion.Select(o => o.Version).Distinct().ToList();

            alllogs = alllogs.Where(o => o != null && !pastversion.Contains(o.Id)).ToList();

            foreach (var item in alllogs.OrderByDescending(o => o.Id))
            {
                if (item.KeyBytes == null)
                {
                    continue;
                }
                Guid key = this.Store.KeyConverter.FromByte(item.KeyBytes);

                if (!objectLastSyncVersion.ContainsKey(key))
                {
                    lastVersion = GetLastSyncVersion(syncSettingId, item.StoreName, key);
                    objectLastSyncVersion[key] = lastVersion;
                }
                else
                {
                    lastVersion = objectLastSyncVersion[key];
                }

                if (lastVersion >= item.Id)
                {
                    continue;
                }

                var currentrecord = result.Find(o => o.KeyHash == item.KeyHash);

                if (currentrecord == null)
                {
                    result.Add(item);
                }
                else
                {
                    if (item.EditType == EditType.Add)
                    {
                        if (currentrecord.EditType == EditType.Delete)
                        {
                            result.Remove(currentrecord);
                        }
                        else if (currentrecord.EditType == EditType.Update)
                        {
                            currentrecord.EditType = EditType.Add;
                        }
                    }
                }
            }
            return result;
        }

        public List<Synchronization> GetRecords(Guid syncSettingId, bool In, int skip, int take)
        {
            if (!In && syncSettingId != default(Guid))
            {
                return this.Query.Where(o => o.SyncSettingId == syncSettingId && o.In == false).OrderByAscending(o => o.Version).Skip(skip).Take(take);
            }
            else
            {
                return this.Query.Where(o => o.SyncSettingId == default(Guid) && o.In == true).OrderByAscending(o => o.Version).Skip(skip).Take(take);
            }
        }

        // used in the grid view to show how many items to be be published.
        public int QueueCount(Guid syncSettingId)
        {
            return GetPushItems(syncSettingId).Count;
        }

        public long GetLastOut(Guid syncSettingId)
        {
            Synchronization item = new Synchronization();
            item.SyncSettingId = syncSettingId;
            item.ObjectId = default(Guid);
            var log = this.Get(item.Id);

            long generallog = -1;

            if (log != null)
            {
                generallog = log.Version;
            }

            var logitem = this.Query.Where(o => o.SyncSettingId == syncSettingId && o.In == false).OrderByDescending(o => o.Version).FirstOrDefault();

            if (logitem != null && logitem.Version > generallog)
            {
                generallog = logitem.Version;
            }

            return generallog;
        }

        public long GetLastIn(Guid syncSettingId)
        {
            Synchronization item = new Synchronization
            {
                SyncSettingId = syncSettingId, ObjectId = default(Guid), In = true
            };
            var log = this.Get(item.Id);
            long generallog = -1;

            if (log != null)
            {
                generallog = log.RemoteVersion;
            }

            var logitem = this.Query.Where(o => o.SyncSettingId == syncSettingId && o.In == true).OrderByDescending(o => o.RemoteVersion).FirstOrDefault();

            if (logitem != null && logitem.RemoteVersion > generallog)
            {
                generallog = logitem.RemoteVersion;
            }

            return generallog;
        }

        public long GetLastSyncVersion(Guid syncSettingId, string storeName, Guid objectId)
        {
            long lastid = -1;

            var logs = this.Query.Where(o => o.SyncSettingId == syncSettingId && o.ObjectId == objectId).SelectAll();

            foreach (var item in logs)
            {
                if (Kooboo.Lib.Helper.StringHelper.IsSameValue(storeName, item.StoreName))
                {
                    if (lastid < item.Version)
                    {
                        lastid = item.Version;
                    }
                }
            }

            return lastid;
        }

        public bool CanSyncToDisk(ISiteObject value, string storeName)
        {
            // verify if this item has been sync to disk....
            // this is a double, should not happen...

            if (!(value is ICoreObject coreobject))
            { return false; }

            var logs = this.Query.Where(o => o.SyncSettingId == this.DiskSyncSettingId && o.ObjectId == value.Id).SelectAll();

            foreach (var item in logs)
            {
                if (Kooboo.Lib.Helper.StringHelper.IsSameValue(storeName, item.StoreName))
                {
                    if (item.Version >= coreobject.Version)
                    {
                        return false;
                    }
                }
            }

            this.AddOrUpdate(new Synchronization { SyncSettingId = this.DiskSyncSettingId, ObjectId = value.Id, Version = coreobject.Version, StoreName = storeName });

            return true;
        }
    }
}