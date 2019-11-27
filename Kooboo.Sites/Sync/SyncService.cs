//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Sites.Repository;
using Kooboo.Sites.Models;
using Kooboo.Data.Interface;
using System.IO;
using System;
using Kooboo.Sites.TaskQueue.Model;
using System.Linq;
using Kooboo.Lib.Helper;
using Kooboo.Sites.Routing;
using System.Collections.Generic;
using Kooboo.IndexedDB;

namespace Kooboo.Sites.Sync
{
    public class SyncService
    {
        public static void DeleteDiskFolder(string FullPath, SiteDb SiteDb)
        {
            string nonroutable = DiskPathService.GetNonRoutableFolder(FullPath);
            if (!string.IsNullOrEmpty(nonroutable))
            {
                // delete all routable.. 
                var repo = SiteDb.GetRepository(nonroutable);
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
                var relative = DiskPathService.GetRelativeUrl(SiteDb.WebSite, FullPath);

                if (!string.IsNullOrEmpty(relative))
                {
                    relative = relative.ToLower();
                    if (!relative.EndsWith("/"))
                    {
                        relative = relative + "/";
                    }
                    List<Route> routesToRemove = new List<Route>();
                    foreach (var item in SiteDb.Routes.All())
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
                            var repo = SiteDb.GetRepository(item.DestinationConstType);
                            repo.Delete(item.objectId);
                            SiteDb.Routes.Delete(item.Id);
                        }
                    }
                }

            }
        }

        public static void DiskFileRename(SiteDb SiteDb, string OldPath, string NewPath)
        {
            NonRoutableObject NonRoutable = null;
            IRepository repo = null;

            NonRoutable = DiskPathService.GetNonRoutableObject(OldPath);

            if (NonRoutable != null)
            {
                var NewNonRoutable = DiskPathService.GetNonRoutableObject(NewPath);

                if (NewNonRoutable != null && !string.IsNullOrEmpty(NonRoutable.Name) && !string.IsNullOrEmpty(NewNonRoutable.Name))
                {
                    repo = SiteDb.GetRepository(NonRoutable.StoreName);
                    var siteobject = repo?.GetByNameOrId(NonRoutable.Name);
                    if (siteobject == null)
                    {
                        return;
                    }

                    var oldid = siteobject.Id;

                    siteobject.Name = NewNonRoutable.Name;
                    siteobject.Id = default(Guid);

                    repo.Delete(oldid);
                    repo.AddOrUpdate(siteobject);

                }
            }
            else
            {
                string OldRelative = DiskPathService.GetRelativeUrl(SiteDb.WebSite, OldPath);
                string NewRelative = DiskPathService.GetRelativeUrl(SiteDb.WebSite, NewPath);
                if (!string.IsNullOrEmpty(OldRelative) && !string.IsNullOrEmpty(NewRelative))
                {
                    SiteDb.Routes.ChangeRoute(OldRelative, NewRelative);
                }
            }
        }

        public static void DiskFolderRename(SiteDb sitedb, string oldFolder, string NewFolder)
        {
            string nonroutable = DiskPathService.GetNonRoutableFolder(NewFolder);
            if (!string.IsNullOrEmpty(nonroutable))
            {
                return;
            }
            string oldrelative = DiskPathService.GetRelativeUrl(sitedb.WebSite, oldFolder);
            string newrelative = DiskPathService.GetRelativeUrl(sitedb.WebSite, NewFolder);

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

            List<Route> ChangeRoutes = new List<Route>();
            foreach (var item in sitedb.Routes.All())
            {
                if (item.Name.StartsWith(loweroldrelative, StringComparison.OrdinalIgnoreCase))
                {
                    ChangeRoutes.Add(item);
                }
            }

            int oldlen = loweroldrelative.Length;
            foreach (var item in ChangeRoutes)
            {
                string newroute = newrelative + item.Name.Substring(oldlen);
                sitedb.Routes.ChangeRoute(item.Name, newroute);
            }
        }

        public static bool WriteToDisk(SiteDb SiteDb, WriteToDisk task)
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
            catch (Exception ex)
            {
                ///throw;
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

            if (disktype == Attributes.DiskType.Text && siteobject is ITextObject)
            {
                var textobject = siteobject as ITextObject;
                if (!string.IsNullOrEmpty(textobject.Body))
                {
                    return System.Text.Encoding.UTF8.GetBytes(textobject.Body);
                }
            }
            else if (disktype == Attributes.DiskType.Binary && siteobject is IBinaryFile)
            {
                var binaryfile = siteobject as IBinaryFile;
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

        public static void Receive(SiteDb SiteDb, Kooboo.Sites.Sync.SyncObject SyncObject, SyncSetting setting = null, Guid UserId = default(Guid))
        {
            if (SyncObject == null)
            { return; }

            if (SyncObject.IsTable)
            {
                ReceiveTableData(SiteDb, SyncObject, setting, UserId);
            }
            else
            {
                ReceiveSiteObject(SiteDb, SyncObject, setting, UserId);
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
                    if (siteobject is ICoreObject)
                    {
                        var core = siteobject as ICoreObject;
                        core.Version = -1;

                        bool ok = repo.AddOrUpdate(core, UserId);

                        if (ok && setting != null)
                        {
                            var localversion = core.Version;
                            if (localversion == -1)
                            {
                                var currentsiteobject = core as SiteObject;
                                if (currentsiteobject != null)
                                {
                                    var dbobject = repo.Get(currentsiteobject.Id);
                                    if (dbobject != null)
                                    {
                                        var dbcoreobject = dbobject as CoreObject;
                                        localversion = dbcoreobject.Version;
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

        private static void ReceiveTableData(SiteDb SiteDb, SyncObject SyncObject, SyncSetting setting, Guid UserId)
        {

            var table = Data.DB.GetOrCreateTable(SiteDb.WebSite, SyncObject.TableName);

            if (table != null)
            {
                if (SyncObject.IsDelete)
                {
                    bool deleteOk = table.Delete(SyncObject.ObjectId);

                    if (deleteOk && setting != null)
                    {
                        var logid = GetJustDeletedVersion(SiteDb, table.Name, SyncObject.ObjectId);
                        if (logid > -1)
                        {
                            SiteDb.Synchronization.AddOrUpdate(new Synchronization { SyncSettingId = setting.Id, TableName = table.Name, ObjectId = SyncObject.ObjectId, In = true, Version = logid, RemoteVersion = SyncObject.SenderVersion });
                        }
                    }
                }
                else
                {
                    var data = Kooboo.Sites.Sync.SyncObjectConvertor.FromTableSyncObject(SyncObject);

                    if (data == null || (data.Count == 1 && data.ContainsKey("_id")))
                    {
                        Kooboo.Data.Log.Instance.Exception.Write("null pull table data/r/n" + Lib.Helper.JsonHelper.Serialize(setting) + Lib.Helper.JsonHelper.Serialize(SyncObject));
                        return;
                    }

                    Guid TableItemKey = SyncObject.ObjectId;
                    var item = table.Get(TableItemKey);

                    bool updateok = false;
                    if (item != null)
                    {
                        if (!string.IsNullOrWhiteSpace(SyncObject.TableColName))
                        {
                            object value = null;
                            if (data.ContainsKey(SyncObject.TableColName))
                            {
                                value = data[SyncObject.TableColName];
                            }
                            updateok = table.UpdateColumn(SyncObject.ObjectId, SyncObject.TableColName, value);
                        }
                        else
                        {
                            updateok = table.Update(SyncObject.ObjectId, data);
                        }
                    }
                    else
                    {
                        TableItemKey = table.Add(data, true);
                        updateok = TableItemKey != default(Guid);
                    }

                    if (updateok && setting != null)
                    { 
                        var AddLog = table.OwnerDatabase.Log.GetLastLogByTableNameAndKey(table.Name, TableItemKey);

                        if (AddLog != null)
                        {
                            SiteDb.Synchronization.AddOrUpdate(new Synchronization { SyncSettingId = setting.Id, TableName = table.Name, ObjectId = TableItemKey, Version = AddLog.Id, RemoteVersion = SyncObject.SenderVersion, In = true });
                        }
                    }
                }

            }
        }

        internal static long GetJustDeletedVersion(SiteDb SiteDb, IRepository repo, Guid ObjectId)
        {
            byte[] key = Service.ObjectService.KeyConverter.ToByte(ObjectId);
            var oldlogs = SiteDb.Log.GetByStoreNameAndKey(repo.StoreName, key, 1);
            if (oldlogs == null || oldlogs.Count() == 0)
            { return -1; }
            var log = oldlogs.First();
            if (log.EditType == IndexedDB.EditType.Delete)
            {
                return log.Id;
            }

            var logs = SiteDb.Log.GetByStoreNameAndKey(repo.StoreName, key, 10);
            foreach (var item in logs.OrderByDescending(o => o.Id))
            {
                if (item.EditType == IndexedDB.EditType.Delete)
                {
                    return item.Id;
                }
            }
            return -1;
        }

        internal static long GetJustDeletedVersion(SiteDb SiteDb, string TableName, Guid ObjectId)
        {
            byte[] key = Service.ObjectService.KeyConverter.ToByte(ObjectId);
            var oldlogs = SiteDb.Log.GetByTableNameAndKey(TableName, key, 1);
            if (oldlogs == null || oldlogs.Count() == 0)
            { return -1; }
            var log = oldlogs.First();
            if (log.EditType == IndexedDB.EditType.Delete)
            {
                return log.Id;
            }

            var logs = SiteDb.Log.GetByStoreNameAndKey(TableName, key, 10);
            foreach (var item in logs.OrderByDescending(o => o.Id))
            {
                if (item.EditType == IndexedDB.EditType.Delete)
                {
                    return item.Id;
                }
            }
            return -1;
        }



        internal static IRepository GetRepository(SiteDb SiteDb, SyncObject SyncObject)
        {
            if (!string.IsNullOrEmpty(SyncObject.StoreName))
            {
                var repo = SiteDb.GetRepository(SyncObject.StoreName);
                if (repo != null)
                {
                    return repo;
                }
            }

            var modeltype = Service.ConstTypeService.GetModelType(SyncObject.ObjectConstType);

            if (modeltype != null)
            {
                var repo = SiteDb.GetRepository(modeltype);
                return repo;
            }
            return null;
        }

        internal static SyncObject Prepare(ISiteObject SiteObject, string StoreName = null, bool IsDelete = false)
        {
            SyncObject syncobject = null;

            if (IsDelete)
            {
                syncobject = new SyncObject();
                syncobject.IsDelete = true;
                syncobject.ObjectConstType = SiteObject.ConstType;
                syncobject.ObjectId = SiteObject.Id;
            }
            else
            {
                syncobject = SyncObjectConvertor.ToSyncObject(SiteObject);
            }

            if (string.IsNullOrEmpty(StoreName))
            {
                StoreName = SiteObject.GetType().Name;
            }
            syncobject.StoreName = StoreName;

            if (SiteObject is ICoreObject)
            {
                var coreobject = SiteObject as ICoreObject;
                syncobject.SenderVersion = coreobject.Version;
            }

            return syncobject;
        }

        internal static SyncObject Prepare(Guid Id, Dictionary<string, object> Data, string tableName, string colName, long version, bool IsDelete = false)
        {
            SyncObject syncobject = null;

            if (IsDelete)
            {
                syncobject = new SyncObject();
                syncobject.IsDelete = true;
                syncobject.TableName = tableName;
                syncobject.ObjectId = Id;
            }
            else
            {
                syncobject = SyncObjectConvertor.ToTableSyncObject(tableName, Id, colName, Data);
            }

            syncobject.SenderVersion = version;

            return syncobject;
        }

        public static SyncObject Prepare(SiteDb SiteDb, long LogVersionId)
        {
            return Prepare(SiteDb, SiteDb.Log.Get(LogVersionId));
        }

        public static SyncObject Prepare(SiteDb SiteDb, LogEntry log)
        {
            if (log.IsTable)
            {
                var key = Kooboo.IndexedDB.ObjectContainer.GuidConverter.FromByte(log.KeyBytes);
                bool isDelete = log.EditType == EditType.Delete;

                var kdb = Kooboo.Data.DB.GetKDatabase(SiteDb.WebSite);
                var ktable = Kooboo.Data.DB.GetTable(kdb, log.TableName);
                if (ktable != null)
                {
                    var data = ktable.GetLogData(log);
                    return Prepare(key, data, log.TableName, log.TableColName, log.Id, isDelete);
                } 
            }
            else
            {
                var repo = SiteDb.GetRepository(log.StoreName);
                var siteobject = repo.GetByLog(log);
                return Prepare(siteobject, log.StoreName, log.EditType == IndexedDB.EditType.Delete);
            }
            return null; 
        }

        #endregion
    }
}
