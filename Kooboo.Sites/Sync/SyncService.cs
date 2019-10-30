//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Data.Interface;
using Kooboo.IndexedDB;
using Kooboo.Lib.Helper;
using Kooboo.Sites.Models;
using Kooboo.Sites.Repository;
using Kooboo.Sites.Routing;
using Kooboo.Sites.TaskQueue.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Kooboo.Sites.Sync
{
    public class SyncService
    {
        public static void DeleteDiskFolder(string fullPath, SiteDb siteDb)
        {
            string nonroutable = DiskPathService.GetNonRoutableFolder(fullPath);
            if (!string.IsNullOrEmpty(nonroutable))
            {
                // delete all routable..
                var repo = siteDb.GetRepository(nonroutable);
                if (repo != null)
                {
                    var all = repo.All();
                    foreach (var item in all)
                    {
                        repo.Delete(item.Id);
                    }
                }
            }
            else
            {
                // delete all.
                var relative = DiskPathService.GetRelativeUrl(siteDb.WebSite, fullPath);

                if (!string.IsNullOrEmpty(relative))
                {
                    relative = relative.ToLower();
                    if (!relative.EndsWith("/"))
                    {
                        relative = relative + "/";
                    }
                    List<Route> routesToRemove = new List<Route>();
                    foreach (var item in siteDb.Routes.All())
                    {
                        if (item.Name.ToLower().StartsWith(relative))
                        {
                            routesToRemove.Add(item);
                        }
                    }

                    foreach (var item in routesToRemove)
                    {
                        if (item.objectId != default(Guid))
                        {
                            var repo = siteDb.GetRepository(item.DestinationConstType);
                            repo.Delete(item.objectId);
                            siteDb.Routes.Delete(item.Id);
                        }
                    }
                }
            }
        }

        public static void DiskFileRename(SiteDb siteDb, string oldPath, string newPath)
        {
            NonRoutableObject nonRoutable = null;
            IRepository repo = null;

            nonRoutable = DiskPathService.GetNonRoutableObject(oldPath);

            if (nonRoutable != null)
            {
                var newNonRoutable = DiskPathService.GetNonRoutableObject(newPath);

                if (newNonRoutable != null && !string.IsNullOrEmpty(nonRoutable.Name) && !string.IsNullOrEmpty(newNonRoutable.Name))
                {
                    repo = siteDb.GetRepository(nonRoutable.StoreName);
                    var siteobject = repo?.GetByNameOrId(nonRoutable.Name);
                    if (siteobject == null)
                    {
                        return;
                    }

                    var oldid = siteobject.Id;

                    siteobject.Name = newNonRoutable.Name;
                    siteobject.Id = default(Guid);

                    repo.Delete(oldid);
                    repo.AddOrUpdate(siteobject);
                }
            }
            else
            {
                string oldRelative = DiskPathService.GetRelativeUrl(siteDb.WebSite, oldPath);
                string newRelative = DiskPathService.GetRelativeUrl(siteDb.WebSite, newPath);
                if (!string.IsNullOrEmpty(oldRelative) && !string.IsNullOrEmpty(newRelative))
                {
                    siteDb.Routes.ChangeRoute(oldRelative, newRelative);
                }
            }
        }

        public static void DiskFolderRename(SiteDb sitedb, string oldFolder, string newFolder)
        {
            string nonroutable = DiskPathService.GetNonRoutableFolder(newFolder);
            if (!string.IsNullOrEmpty(nonroutable))
            {
                return;
            }
            string oldrelative = DiskPathService.GetRelativeUrl(sitedb.WebSite, oldFolder);
            string newrelative = DiskPathService.GetRelativeUrl(sitedb.WebSite, newFolder);

            if (!oldrelative.EndsWith("/"))
            {
                oldrelative = oldrelative + "/";
            }
            if (!newrelative.EndsWith("/"))
            {
                newrelative = newrelative + "/";
            }
            var loweroldrelative = oldrelative.ToLower();
            var lowernewrelative = newrelative.ToLower();

            List<Route> changeRoutes = new List<Route>();
            foreach (var item in sitedb.Routes.All())
            {
                if (item.Name.StartsWith(loweroldrelative, StringComparison.OrdinalIgnoreCase))
                {
                    changeRoutes.Add(item);
                }
            }

            int oldlen = loweroldrelative.Length;
            foreach (var item in changeRoutes)
            {
                string newroute = newrelative + item.Name.Substring(oldlen);
                sitedb.Routes.ChangeRoute(item.Name, newroute);
            }
        }

        public static bool WriteToDisk(SiteDb siteDb, WriteToDisk task)
        {
            if (string.IsNullOrEmpty(task.FullPath))
            {
                return true;
            }

            try
            {
                if (task.IsDelete)
                {
                    if (File.Exists(task.FullPath))
                    {
                        File.Delete(task.FullPath);
                    }
                }
                else
                {
                    IOHelper.EnsureFileDirectoryExists(task.FullPath);

                    if (task.IsBinary)
                    {
                        if (System.IO.File.Exists(task.FullPath))
                        {
                            var bytes = IOHelper.ReadAllBytes(task.FullPath);
                            if (IOHelper.IsEqualBytes(bytes, task.BinaryBytes))
                            {
                                return true;
                            }
                        }

                        File.WriteAllBytes(task.FullPath, task.BinaryBytes);
                    }
                    else
                    {
                        if (System.IO.File.Exists(task.FullPath))
                        {
                            var currentext = IOHelper.ReadAllText(task.FullPath);

                            if (Kooboo.Lib.Helper.StringHelper.IsSameValue(currentext, task.TextBody))
                            {
                                return true;
                            }
                        }
                        System.IO.File.WriteAllText(task.FullPath, task.TextBody);
                    }
                }

                return true;
            }
            catch (Exception)
            {
                //throw;
                return false;
            }
        }

        public static byte[] GetObjectBytes(ISiteObject siteobject)
        {
            if (siteobject == null)
            {
                return null;
            }
            var disktype = Kooboo.Attributes.AttributeHelper.GetDiskType(siteobject);

            if (disktype == Attributes.DiskType.Text && siteobject is ITextObject textobject)
            {
                if (!string.IsNullOrEmpty(textobject.Body))
                {
                    return System.Text.Encoding.UTF8.GetBytes(textobject.Body);
                }
            }
            else if (disktype == Attributes.DiskType.Binary && siteobject is IBinaryFile binaryfile)
            {
                return binaryfile.ContentBytes;
            }
            else
            {
                string textbody = JsonHelper.Serialize(siteobject);
                if (!string.IsNullOrEmpty(textbody))
                {
                    return System.Text.Encoding.UTF8.GetBytes(textbody);
                }
            }
            return null;
        }

        #region "remote publish"

        public static void Receive(SiteDb siteDb, Kooboo.Sites.Sync.SyncObject syncObject, SyncSetting setting = null, Guid userId = default(Guid))
        {
            if (syncObject == null)
            { return; }

            if (syncObject.IsTable)
            {
                ReceiveTableData(siteDb, syncObject, setting, userId);
            }
            else
            {
                ReceiveSiteObject(siteDb, syncObject, setting, userId);
            }
        }

        private static void ReceiveSiteObject(SiteDb SiteDb, SyncObject SyncObject, SyncSetting setting, Guid UserId)
        {
            var repo = GetRepository(SiteDb, SyncObject);
            if (repo != null)
            {
                if (SyncObject.IsDelete)
                {
                    var obj = repo.Get(SyncObject.ObjectId);
                    if (obj != null)
                    {
                        repo.Delete(SyncObject.ObjectId, UserId);

                        if (setting != null && Attributes.AttributeHelper.IsCoreObject(repo.ModelType))
                        {
                            var logid = GetJustDeletedVersion(SiteDb, repo, SyncObject.ObjectId);
                            if (logid > -1)
                            {
                                SiteDb.Synchronization.AddOrUpdate(new Synchronization { SyncSettingId = setting.Id, StoreName = repo.StoreName, ObjectId = SyncObject.ObjectId, In = true, Version = logid, RemoteVersion = SyncObject.SenderVersion });
                            }
                        }
                    }
                }
                else
                {
                    var siteobject = Kooboo.Sites.Sync.SyncObjectConvertor.FromSyncObject(SyncObject);
                    if (siteobject is ICoreObject core)
                    {
                        core.Version = -1;

                        bool ok = repo.AddOrUpdate(core, UserId);

                        if (ok && setting != null)
                        {
                            var localversion = core.Version;
                            if (localversion == -1)
                            {
                                if (core is SiteObject currentsiteobject)
                                {
                                    var dbobject = repo.Get(currentsiteobject.Id);
                                    if (dbobject != null)
                                    {
                                        if (dbobject is CoreObject dbcoreobject) localversion = dbcoreobject.Version;
                                    }
                                }
                            }
                            SiteDb.Synchronization.AddOrUpdate(new Synchronization { SyncSettingId = setting.Id, StoreName = repo.StoreName, ObjectId = siteobject.Id, Version = localversion, RemoteVersion = SyncObject.SenderVersion, In = true });
                        }
                    }
                    else
                    {
                        repo.AddOrUpdate(siteobject);
                    }
                }
            }
        }

        private static void ReceiveTableData(SiteDb siteDb, SyncObject syncObject, SyncSetting setting, Guid userId)
        {
            var table = Data.DB.GetOrCreateTable(siteDb.WebSite, syncObject.TableName);

            if (table != null)
            {
                if (syncObject.IsDelete)
                {
                    bool deleteOk = table.Delete(syncObject.ObjectId);

                    if (deleteOk && setting != null)
                    {
                        var logid = GetJustDeletedVersion(siteDb, table.Name, syncObject.ObjectId);
                        if (logid > -1)
                        {
                            siteDb.Synchronization.AddOrUpdate(new Synchronization { SyncSettingId = setting.Id, TableName = table.Name, ObjectId = syncObject.ObjectId, In = true, Version = logid, RemoteVersion = syncObject.SenderVersion });
                        }
                    }
                }
                else
                {
                    var data = Kooboo.Sites.Sync.SyncObjectConvertor.FromTableSyncObject(syncObject);

                    if (data == null || (data.Count == 1 && data.ContainsKey("_id")))
                    {
                        Kooboo.Data.Log.Instance.Exception.Write("null pull table data/r/n" + Lib.Helper.JsonHelper.Serialize(setting) + Lib.Helper.JsonHelper.Serialize(syncObject));
                        return;
                    }

                    Guid tableItemKey = syncObject.ObjectId;
                    var item = table.Get(tableItemKey);

                    bool updateok = false;
                    if (item != null)
                    {
                        if (!string.IsNullOrWhiteSpace(syncObject.TableColName))
                        {
                            object value = null;
                            if (data.ContainsKey(syncObject.TableColName))
                            {
                                value = data[syncObject.TableColName];
                            }
                            updateok = table.UpdateColumn(syncObject.ObjectId, syncObject.TableColName, value);
                        }
                        else
                        {
                            updateok = table.Update(syncObject.ObjectId, data);
                        }
                    }
                    else
                    {
                        tableItemKey = table.Add(data, true);
                        updateok = tableItemKey != default(Guid);
                    }

                    if (updateok && setting != null)
                    {
                        var addLog = table.OwnerDatabase.Log.GetLastLogByTableNameAndKey(table.Name, tableItemKey);

                        if (addLog != null)
                        {
                            siteDb.Synchronization.AddOrUpdate(new Synchronization { SyncSettingId = setting.Id, TableName = table.Name, ObjectId = tableItemKey, Version = addLog.Id, RemoteVersion = syncObject.SenderVersion, In = true });
                        }
                    }
                }
            }
        }

        internal static long GetJustDeletedVersion(SiteDb siteDb, IRepository repo, Guid objectId)
        {
            byte[] key = Service.ObjectService.KeyConverter.ToByte(objectId);
            var oldlogs = siteDb.Log.GetByStoreNameAndKey(repo.StoreName, key, 1);
            if (oldlogs == null || oldlogs.Count == 0)
            { return -1; }
            var log = oldlogs.First();
            if (log.EditType == IndexedDB.EditType.Delete)
            {
                return log.Id;
            }

            var logs = siteDb.Log.GetByStoreNameAndKey(repo.StoreName, key, 10);
            foreach (var item in logs.OrderByDescending(o => o.Id))
            {
                if (item.EditType == IndexedDB.EditType.Delete)
                {
                    return item.Id;
                }
            }
            return -1;
        }

        internal static long GetJustDeletedVersion(SiteDb siteDb, string tableName, Guid objectId)
        {
            byte[] key = Service.ObjectService.KeyConverter.ToByte(objectId);
            var oldlogs = siteDb.Log.GetByTableNameAndKey(tableName, key, 1);
            if (oldlogs == null || oldlogs.Count == 0)
            { return -1; }
            var log = oldlogs.First();
            if (log.EditType == IndexedDB.EditType.Delete)
            {
                return log.Id;
            }

            var logs = siteDb.Log.GetByStoreNameAndKey(tableName, key, 10);
            foreach (var item in logs.OrderByDescending(o => o.Id))
            {
                if (item.EditType == IndexedDB.EditType.Delete)
                {
                    return item.Id;
                }
            }
            return -1;
        }

        internal static IRepository GetRepository(SiteDb siteDb, SyncObject syncObject)
        {
            if (!string.IsNullOrEmpty(syncObject.StoreName))
            {
                var repo = siteDb.GetRepository(syncObject.StoreName);
                if (repo != null)
                {
                    return repo;
                }
            }

            var modeltype = Service.ConstTypeService.GetModelType(syncObject.ObjectConstType);

            if (modeltype != null)
            {
                var repo = siteDb.GetRepository(modeltype);
                return repo;
            }
            return null;
        }

        internal static SyncObject Prepare(ISiteObject siteObject, string storeName = null, bool isDelete = false)
        {
            SyncObject syncobject = null;

            if (isDelete)
            {
                syncobject = new SyncObject
                {
                    IsDelete = true, ObjectConstType = siteObject.ConstType, ObjectId = siteObject.Id
                };
            }
            else
            {
                syncobject = SyncObjectConvertor.ToSyncObject(siteObject);
            }

            if (string.IsNullOrEmpty(storeName))
            {
                storeName = siteObject.GetType().Name;
            }
            syncobject.StoreName = storeName;

            if (siteObject is ICoreObject coreobject)
            {
                syncobject.SenderVersion = coreobject.Version;
            }

            return syncobject;
        }

        internal static SyncObject Prepare(Guid id, Dictionary<string, object> data, string tableName, string colName, long version, bool isDelete = false)
        {
            SyncObject syncobject = null;

            if (isDelete)
            {
                syncobject = new SyncObject {IsDelete = true, TableName = tableName, ObjectId = id};
            }
            else
            {
                syncobject = SyncObjectConvertor.ToTableSyncObject(tableName, id, colName, data);
            }

            syncobject.SenderVersion = version;

            return syncobject;
        }

        public static SyncObject Prepare(SiteDb siteDb, long logVersionId)
        {
            return Prepare(siteDb, siteDb.Log.Get(logVersionId));
        }

        public static SyncObject Prepare(SiteDb siteDb, LogEntry log)
        {
            if (log.IsTable)
            {
                var key = Kooboo.IndexedDB.ObjectContainer.GuidConverter.FromByte(log.KeyBytes);
                bool isDelete = log.EditType == EditType.Delete;

                var kdb = Kooboo.Data.DB.GetKDatabase(siteDb.WebSite);
                var ktable = Kooboo.Data.DB.GetOrCreateTable(kdb, log.TableName);
                if (ktable != null)
                {
                    var data = ktable.GetLogData(log);
                    return Prepare(key, data, log.TableName, log.TableColName, log.Id, isDelete);
                }
            }
            else
            {
                var repo = siteDb.GetRepository(log.StoreName);
                var siteobject = repo.GetByLog(log);
                return Prepare(siteobject, log.StoreName, log.EditType == IndexedDB.EditType.Delete);
            }
            return null;
        }

        #endregion "remote publish"
    }
}