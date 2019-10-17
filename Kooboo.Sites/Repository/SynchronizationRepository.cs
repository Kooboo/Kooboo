//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.IndexedDB;
using Kooboo.Sites.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Kooboo.Extensions;
using Kooboo.IndexedDB.Btree.Comparer;
using Kooboo.Data.Interface;

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
        public List<LogEntry> GetPushItems(Guid SyncSettingId)
        {
           // var currentlogid = GetLastOut(SyncSettingId);
            var currentlogid = -1;
            List<LogEntry> result = new List<LogEntry>(); 
            Dictionary<Guid, long> ObjectLastSyncVersion = new Dictionary<Guid, long>();

            long LastVersion = -1;

            var alllogs = this.SiteDb.Log.Store.Where(o => o.Id > currentlogid).SelectAll();
            var allversion = this.SiteDb.Synchronization.Query.Where(o => o.SyncSettingId == SyncSettingId).SelectAll();
           var pastversion =  allversion.Select(o => o.Version).Distinct().ToList();

            alllogs = alllogs.Where(o=> o !=null && !pastversion.Contains(o.Id)).ToList(); 

            foreach (var item in alllogs.OrderByDescending(o => o.Id))
            { 
                if (item.KeyBytes == null)
                {
                    continue; 
                } 
                Guid key = this.Store.KeyConverter.FromByte(item.KeyBytes);

                if (!ObjectLastSyncVersion.ContainsKey(key))
                {
                    LastVersion = GetLastSyncVersion(SyncSettingId, item.StoreName, key);
                    ObjectLastSyncVersion[key] = LastVersion;
                }
                else
                {
                    LastVersion = ObjectLastSyncVersion[key];
                }

                if (LastVersion >= item.Id)
                { 
                    continue;
                }
    
                var currentrecord = result.Find(o =>o.KeyHash == item.KeyHash);

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

        public List<Synchronization> GetRecords(Guid SyncSettingId, bool In, int skip, int take)
        {
            if (!In && SyncSettingId != default(Guid))
            {
                return this.Query.Where(o => o.SyncSettingId == SyncSettingId && o.In == false).OrderByAscending(o=>o.Version).Skip(skip).Take(take);
            }
            else
            {
                return this.Query.Where(o => o.SyncSettingId == default(Guid) && o.In == true).OrderByAscending(o => o.Version).Skip(skip).Take(take);
            }
        }

        // used in the grid view to show how many items to be be published. 
        public int QueueCount(Guid SyncSettingId)
        {
            return GetPushItems(SyncSettingId).Count;
        }

        public long GetLastOut(Guid SyncSettingId)
        {
            Synchronization item = new Synchronization();
            item.SyncSettingId = SyncSettingId;
            item.ObjectId = default(Guid);
            var log = this.Get(item.Id);
             
            long generallog = -1;

            if (log != null)
            {
                generallog = log.Version;
            }

            var logitem = this.Query.Where(o => o.SyncSettingId == SyncSettingId && o.In == false).OrderByDescending(o => o.Version).FirstOrDefault();

            if (logitem != null && logitem.Version > generallog)
            {
                generallog = logitem.Version;
            }

            return generallog; 
        }
         
        public long GetLastIn(Guid SyncSettingId)
        {
            Synchronization item = new Synchronization();
            item.SyncSettingId = SyncSettingId;
            item.ObjectId = default(Guid);
            item.In = true; 
            var log = this.Get(item.Id); 
            long generallog = -1; 

            if (log != null)
            {
                generallog = log.RemoteVersion; 
            }

            var logitem = this.Query.Where(o => o.SyncSettingId == SyncSettingId && o.In == true).OrderByDescending(o => o.RemoteVersion).FirstOrDefault(); 

            if (logitem != null && logitem.RemoteVersion > generallog)
            {
                generallog = logitem.RemoteVersion; 
            }

            return generallog; 
        }

        public long GetLastSyncVersion(Guid SyncSettingId, string StoreName, Guid ObjectId)
        {
            long lastid = -1;

            var logs = this.Query.Where(o => o.SyncSettingId == SyncSettingId && o.ObjectId == ObjectId).SelectAll();

            foreach (var item in logs)
            {
                if (Kooboo.Lib.Helper.StringHelper.IsSameValue(StoreName, item.StoreName))
                {
                    if (lastid < item.Version)
                    {
                        lastid = item.Version;
                    }
                }
            }

            return lastid;
        }
          
        public bool CanSyncToDisk(ISiteObject value, string StoreName)
        {
            /// verify if this item has been sync to disk....
            /// this is a double, should not happen... 
            if (value == null)
            {
                return false; 
            }

            ICoreObject coreobject = value as ICoreObject; 
            if (coreobject == null)
            { return false;  }
    
            var logs = this.Query.Where(o => o.SyncSettingId == this.DiskSyncSettingId && o.ObjectId == value.Id).SelectAll();

            foreach (var item in logs)
            {
                if (Kooboo.Lib.Helper.StringHelper.IsSameValue(StoreName, item.StoreName))
                {
                   if (item.Version >= coreobject.Version)
                    {
                        return false;
                    } 
                }
            }

            this.AddOrUpdate(new Synchronization { SyncSettingId = this.DiskSyncSettingId, ObjectId = value.Id, Version = coreobject.Version, StoreName = StoreName });

            return true;  
        }
    }
}
