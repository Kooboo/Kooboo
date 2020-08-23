//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data;
using Kooboo.Data.Interface;
using Kooboo.Data.Models;
using Kooboo.Lib.Helper;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Repository;
using Kooboo.Sites.Service;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Kooboo.Sites.Contents.Models;
using Kooboo.Sites.Models;
using Kooboo.IndexedDB;
using Kooboo.Sites.Sync.Disk;

namespace Kooboo.Sites.Sync
{
    public static class ImportExport
    {
        private static Dictionary<string, int> InnerImportOrder = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        static ImportExport()
        {
            //-Route, Image, Style, Script, menu, form, and then others. 
            InnerImportOrder.Add(typeof(Kooboo.Sites.Routing.Route).Name, 1);
            InnerImportOrder.Add(typeof(Kooboo.Sites.Models.Image).Name, 2);
            InnerImportOrder.Add(typeof(Kooboo.Sites.Models.View).Name, 3);
            InnerImportOrder.Add(typeof(Kooboo.Sites.Models.Style).Name, 4);
            InnerImportOrder.Add(typeof(Kooboo.Sites.Models.Script).Name, 5);
            InnerImportOrder.Add(typeof(Kooboo.Sites.Models.Menu).Name, 6);
            InnerImportOrder.Add(typeof(Kooboo.Sites.Models.Form).Name, 7);
        }

        #region "NotUsedNow"


        public static string ExportSourceZip(SiteDb SiteDb)
        {
            string strguid = System.Guid.NewGuid().ToString();
            string DiskPath = AppSettings.TempDataPath;
            DiskPath = System.IO.Path.Combine(DiskPath, strguid);

            IOHelper.EnsureDirectoryExists(DiskPath);

            var newsitedb = Copy(SiteDb, DiskPath);
            newsitedb.DatabaseDb.Close();

            var zipFile = System.IO.Path.Combine(AppSettings.TempDataPath, strguid + ".zip");
            if (File.Exists(zipFile))
            {
                File.Delete(zipFile);
            }

            var targetFolder = newsitedb.DatabaseDb.AbsolutePath;

            if (Directory.Exists(targetFolder))
            {
                var newstream = new FileStream(zipFile, FileMode.OpenOrCreate);
                var newarchive = new ZipArchive(newstream, ZipArchiveMode.Create, false);

                var files = Directory.GetFiles(targetFolder, "*.*", SearchOption.AllDirectories);
                var folderLength = targetFolder.Length;
                foreach (var path in files)
                {
                    if (SkipExport(path))
                    {
                        continue;
                    }
                    newarchive.CreateEntryFromFile(path, path.Replace(DiskPath, "").Trim('\\').Trim('/'));
                }

                newarchive.Dispose();

                newstream.Dispose();

                return zipFile;
            }

            return null;
        }

        public static SiteDb Copy(SiteDb SiteDb, string DiskPath = null)
        {
            //Database db 
            if (DiskPath == null)
            {
                DiskPath = Kooboo.Data.AppSettings.TempDataPath;
                DiskPath = System.IO.Path.Combine(DiskPath, System.Guid.NewGuid().ToString());
            }

            SiteDb newdb = new SiteDb(new Data.Models.WebSite() { Name = "___temp" });
            newdb.DatabaseDb = DB.GetDatabase(DiskPath);

            foreach (var repo in SiteDb.ActiveRepositories())
            {
                var storename = repo.StoreName;
                string lower = storename.ToLower();

                if (lower.Contains("log") || lower.Contains("syncsetting") || lower.Contains("thumbnail") || lower.Contains("synchronization") || lower.Contains("transferpage") || lower.Contains("downloadfailtrack"))
                {
                    continue;
                }
                var newrepo = newdb.GetRepository(repo.StoreName);

                foreach (var dbitem in repo.All())
                {
                    if (dbitem.Id != default(Guid))
                    {
                        newrepo.Store.add(dbitem.Id, dbitem);
                    }
                }
                repo.Store.Close();
            }
            return newdb;
        }

        private static bool SkipExport(string FilePath)
        {
            var slash = Kooboo.Lib.Compatible.CompatibleManager.Instance.System.GetSlash();
            if (FilePath.Contains(slash + "EventRules")
                  || FilePath.Contains(slash + "_koobooeditlog")
                  || FilePath.Contains(slash + "SyncSetting")
                  || FilePath.Contains(slash + "Thumbnail")
                  || FilePath.Contains(slash + "TransferPage")
                  || FilePath.Contains(slash + "Synchronization")
                //|| FilePath.Contains(slash+"TransferTask")
                || FilePath.Contains(slash + "DownloadFailTrack")
                  )
            {
                return true;
            }
            return false;
        }


        public static List<string> FindCommonPath(List<string> paths)
        {
            Func<List<string>, bool> HasSameValue = (list) =>
            {
                string current = null;

                foreach (var item in list)
                {
                    if (current == null)
                    {
                        current = item;
                        if (current.ToLower().Contains("_kooboo"))
                        {
                            return false;
                        }
                    }
                    else
                    {
                        if (!Kooboo.Lib.Helper.StringHelper.IsSameValue(current, item))
                        {
                            return false;
                        }
                    }
                }
                return true;
            };

            Func<string, List<string>> ToSegments = Kooboo.Lib.Compatible.CompatibleManager.Instance.System.GetSegments; ; ;


            if (paths == null || paths.Count() == 1)
            {
                return new List<string>();
            }

            List<List<string>> AllSegments = new List<List<string>>();

            foreach (var item in paths)
            {
                AllSegments.Add(ToSegments(item));
            }

            List<string> common = new List<string>();

            int i = 0;

            while (i < 999)
            {
                List<string> indexitem = new List<string>();
                foreach (var item in AllSegments)
                {
                    if (i > item.Count() - 1)
                    {
                        break;
                    }
                    else
                    {
                        indexitem.Add(item[i]);
                    }
                }
                if (indexitem.Count() > 0 && HasSameValue(indexitem))
                {
                    common.Add(indexitem[0]);
                    i += 1;
                }
                else
                {
                    break;
                }
            }

            return common;

        }



        #endregion


        private static string KoobooSettingFileName { get; set; } = "kooboosetting.json";

        private static string TableSettingFileName { get; set; } = "koobootablesetting.json";

        private static string InterExtension { get; set; } = ".kbinary";

        private static string BatchSettingFileName { get; set; } = "kooboobacth.kbbatch.config";


        public static WebSite ImportZip(Stream zipFile, WebSite newsite, Guid UserId = default(Guid))
        {
            using (var archive = new ZipArchive(zipFile, ZipArchiveMode.Read))
            {
                if (archive.Entries.Count > 0)
                {
                    var SiteDb = newsite.SiteDb();

                    if (IsKoobooInterSite(archive))
                    {
                        ImportInter(archive, SiteDb, UserId);
                    }
                    else if (IsKoobooBatch(archive))
                    {
                        ImportBatch(archive, SiteDb, UserId);
                    }
                    else if (IsKoobooSite(archive))
                    {
                        ImportKoobooSite(archive, SiteDb);
                    }
                    else
                    {
                        ImportHtmlZip(archive, SiteDb);
                    }
                }
            }
            WebSiteService.InitSiteCultureAfterImport(newsite);

            return newsite;
        }

        public static WebSite ImportZip(Stream zipFile, Guid organizationId, string SiteName, string FullDomain, Guid UserId)
        {
            var newsite = Kooboo.Sites.Service.WebSiteService.AddNewSite(organizationId, SiteName, FullDomain, UserId);
            return ImportZip(zipFile, newsite, UserId);
        }

        public static bool IsKoobooSite(ZipArchive archive)
        {
            int blockcount = 0;
            int indexcount = 0;
            int settingcount = 0;
            int totalcount = 0;
            foreach (var item in archive.Entries)
            {
                if (item.Name == "Data.block")
                {
                    blockcount += 1;
                    totalcount += 1;
                    if (blockcount > 0 && indexcount > 0 && settingcount > 0)
                    {
                        return true;
                    }
                    if (totalcount > 10)
                    {
                        return true;
                    }
                }

                if (item.Name == "Id.index")
                {
                    indexcount += 1;
                    totalcount += 1;
                    if (blockcount > 0 && indexcount > 0 && settingcount > 0)
                    {
                        return true;
                    }
                    if (totalcount > 10)
                    {
                        return true;
                    }
                }

                if (item.Name == "setting.config" || item.Name == "store.config")
                {
                    settingcount += 1;
                    totalcount += 1;
                    if (blockcount > 0 && indexcount > 0 && settingcount > 0)
                    {
                        return true;
                    }
                    if (totalcount > 10)
                    {
                        return true;
                    }
                }
            }

            if (blockcount > 0 && indexcount > 0 && settingcount > 0)
            {
                return true;
            }
            if (totalcount > 5)
            {
                return true;
            }

            bool IsKoobooSite = true;

            foreach (var item in archive.Entries)
            {
                string name = item.Name;
                if (!string.IsNullOrEmpty(name))
                {
                    string lower = name.ToLower();
                    if (lower.EndsWith(".index") || lower.EndsWith(".block") || lower.EndsWith(".config"))
                    {
                        continue;
                    }
                    var minetype = UrlHelper.GetFileType(lower);
                    if (minetype == UrlHelper.UrlFileType.Image || minetype == UrlHelper.UrlFileType.PageOrView || minetype == UrlHelper.UrlFileType.JavaScript || minetype == UrlHelper.UrlFileType.Style)
                    {
                        IsKoobooSite = false;
                        break;
                    }
                }
            }

            return IsKoobooSite;
        }

        public static bool IsKoobooInterSite(ZipArchive archive)
        {
            foreach (var item in archive.Entries)
            {
                if (item.Name.EndsWith(InterExtension) || item.Name.EndsWith(KoobooSettingFileName) || item.Name.EndsWith(TableSettingFileName))
                {
                    return true;
                }
            }
            return false;
        }


        public static WebSite ImportKoobooSite(ZipArchive archive, SiteDb SiteDb)
        {
            var baseDir = SiteDb.DatabaseDb.AbsolutePath;
            IOHelper.EnsureDirectoryExists(baseDir);

            foreach (var entry in archive.Entries)
            {
                var path = System.IO.Path.Combine(baseDir, entry.FullName);

                if (string.IsNullOrEmpty(entry.Name))
                {
                    IOHelper.EnsureDirectoryExists(path);
                }
                else
                {
                    IOHelper.EnsureFileDirectoryExists(path);
                    entry.ExtractToFile(path, true);
                }
            }
            return SiteDb.WebSite;
        }

        public static WebSite ImportHtmlZip(ZipArchive archive, SiteDb siteDb)
        {
            var manager = new  SyncManager(siteDb.WebSite.Id);
            foreach (var entry in archive.Entries)
            {
                var target = entry.FullName;

                if (!string.IsNullOrEmpty(entry.Name))
                {
                    MemoryStream memory = new MemoryStream();
                    entry.Open().CopyTo(memory);
                    var bytes = memory.ToArray();
                    if (bytes != null && bytes.Length > 0)
                    {
                        manager.SyncToDb(target, siteDb, bytes);
                    }
                }
            }
            return siteDb.WebSite;
        }

        public static string ExportInter(SiteDb SiteDb)
        {
            string strguid = System.Guid.NewGuid().ToString();

            string DiskPath = InterCopy(SiteDb);

            var zipFile = System.IO.Path.Combine(AppSettings.TempDataPath, strguid + ".zip");
            if (File.Exists(zipFile))
            {
                File.Delete(zipFile);
            }

            var newstream = new FileStream(zipFile, FileMode.OpenOrCreate);
            var newarchive = new ZipArchive(newstream, ZipArchiveMode.Create, false);

            var files = Directory.GetFiles(DiskPath, "*.*", SearchOption.AllDirectories);

            foreach (var path in files)
            {
                newarchive.CreateEntryFromFile(path, path.Replace(DiskPath, "").Trim('\\').Trim('/'));
            }

            newarchive.Dispose();

            newstream.Dispose();

            return zipFile;
        }

        public static string ExportInterSelected(SiteDb SiteDb, List<string> StoreNames)
        {
            string strguid = System.Guid.NewGuid().ToString();

            string DiskPath = InterCopySelected(SiteDb, StoreNames);

            var zipFile = System.IO.Path.Combine(AppSettings.TempDataPath, strguid + ".zip");
            if (File.Exists(zipFile))
            {
                File.Delete(zipFile);
            }

            var newstream = new FileStream(zipFile, FileMode.OpenOrCreate);
            var newarchive = new ZipArchive(newstream, ZipArchiveMode.Create, false);

            var files = Directory.GetFiles(DiskPath, "*.*", SearchOption.AllDirectories);

            foreach (var path in files)
            {
                newarchive.CreateEntryFromFile(path, path.Replace(DiskPath, "").Trim('\\').Trim('/'));
            }

            newarchive.Dispose();
            newstream.Dispose();
            return zipFile;
        }

        // copy as the intermediate format of Kooboo db... 
        public static string InterCopy(SiteDb SiteDb)
        {
            string DiskPath = Kooboo.Data.AppSettings.TempDataPath;
            DiskPath = System.IO.Path.Combine(DiskPath, System.Guid.NewGuid().ToString());
            IOHelper.EnsureDirectoryExists(DiskPath);

            foreach (var repo in SiteDb.ActiveRepositories())
            {
                if (!Kooboo.Attributes.AttributeHelper.IsCoreObject(repo.ModelType))
                {
                    continue;
                }
                var storename = repo.StoreName;
                string path = System.IO.Path.Combine(DiskPath, storename);
                var serializer = new Kooboo.IndexedDB.Serializer.Simple.SimpleConverter(repo.ModelType);

                foreach (var dbitem in repo.All())
                {
                    if (dbitem.Id != default(Guid))
                    {
                        var bytes = serializer.ToBytes(dbitem);
                        string fullpath = System.IO.Path.Combine(path, dbitem.Id.ToString() + InterExtension);
                        IOHelper.EnsureFileDirectoryExists(fullpath);
                        File.WriteAllBytes(fullpath, bytes);
                    }
                }
                repo.Store.Close();
            }

            var setting = GetSiteSetting(SiteDb.WebSite);
            var json = JsonHelper.SerializeCaseSensitive(setting);
            string settingfile = System.IO.Path.Combine(DiskPath, KoobooSettingFileName);
            File.WriteAllText(settingfile, json);

            InnerCopyDynamicTable(SiteDb.WebSite, DiskPath);

            return DiskPath;
        }

        public static void InnerCopyDynamicTable(WebSite website, string diskpath)
        {
            var db = Kooboo.Data.DB.GetKDatabase(website);
            var tablesetting = Kooboo.IndexedDB.Dynamic.Sync.GetTableSetting(db);
            var json = JsonHelper.SerializeCaseSensitive(tablesetting);

            string settingfile = System.IO.Path.Combine(diskpath, TableSettingFileName);
            File.WriteAllText(settingfile, json);

            var alltables = db.GetTables();

            List<string> Tables = new List<string>();

            foreach (var item in alltables)
            {
                var lower = item.ToLower();

                if (lower == "_sys_keyvalues" || !lower.StartsWith("_sys_"))
                {
                    Tables.Add(item);
                }
            }

            foreach (var item in Tables)
            {
                var table = Data.DB.GetOrCreateTable(db, item);

                if (table != null)
                {
                    string path = System.IO.Path.Combine(diskpath, "_table_" + table.Name);

                    IOHelper.EnsureDirectoryExists(path);

                    var all = table.All();

                    foreach (var data in all)
                    {
                        var jsondata = Kooboo.Lib.Helper.JsonHelper.SerializeCaseSensitive(data);

                        if (data.ContainsKey("_id"))
                        {
                            var id = data["_id"];

                            string filename = System.IO.Path.Combine(path, id.ToString() + InterExtension);

                            var bytes = System.Text.Encoding.UTF8.GetBytes(jsondata);

                            File.WriteAllBytes(filename, bytes);
                        }

                    }

                }
            }

            InnerCopyFileIO(website, diskpath);
        }

        public static void InnerCopyFileIO(WebSite website, string diskpath)
        {
            string fileiopath = Lib.Helper.IOHelper.CombinePath(diskpath, "__fileio__");

            string currentPath = Kooboo.Data.AppSettings.GetFileIORoot(website);
             
            Kooboo.Lib.Helper.IOHelper.DirectoryCopy(currentPath, fileiopath, true);  
        } 

        public static string InterCopySelected(SiteDb SiteDb, List<string> StoretNames)
        {
            string DiskPath = Kooboo.Data.AppSettings.TempDataPath;
            DiskPath = System.IO.Path.Combine(DiskPath, System.Guid.NewGuid().ToString());
            IOHelper.EnsureDirectoryExists(DiskPath);

            var routeserializer = new Kooboo.IndexedDB.Serializer.Simple.SimpleConverter(SiteDb.Routes.SiteObjectType);

            string RoutePath = System.IO.Path.Combine(DiskPath, SiteDb.Routes.StoreName);

            foreach (var item in StoretNames)
            {

                if (item.ToLower() == "storage")
                {
                    InnerCopyDynamicTable(SiteDb.WebSite, DiskPath);
                }
                else
                {
                    var repo = SiteDb.GetRepository(item);
                    
                    if (repo== null)
                    {
                        continue; 
                    }

                    if (!Attributes.AttributeHelper.IsCoreObject(repo.ModelType))
                    {
                        continue;
                    }
                    var storename = repo.StoreName;
                    string path = System.IO.Path.Combine(DiskPath, storename);
                    IOHelper.EnsureDirectoryExists(path);

                    var serializer = new Kooboo.IndexedDB.Serializer.Simple.SimpleConverter(repo.ModelType);

                    foreach (var dbitem in repo.All())
                    {
                        if (dbitem.Id != default(Guid))
                        {
                            var bytes = serializer.ToBytes(dbitem);
                            string fullpath = System.IO.Path.Combine(path, dbitem.Id.ToString() + InterExtension);
                            File.WriteAllBytes(fullpath, bytes);

                            // routable object....  
                            if (Kooboo.Attributes.AttributeHelper.IsRoutable(dbitem))
                            {
                                var route = SiteDb.Routes.GetByObjectId(dbitem.Id);
                                if (route != null)
                                {
                                    var routebytes = routeserializer.ToBytes(route);
                                    string RouteFullPath = System.IO.Path.Combine(RoutePath, route.Id.ToString() + InterExtension);
                                    IOHelper.EnsureFileDirectoryExists(RouteFullPath);
                                    File.WriteAllBytes(RouteFullPath, routebytes);
                                }
                            }

                            if (dbitem is TextContent)
                            {
                                // need to have the content type and content folder. 
                                var content = dbitem as TextContent;
                                var folder = SiteDb.ContentFolders.Get(content.FolderId);
                                var FolderSerilizer = new Kooboo.IndexedDB.Serializer.Simple.SimpleConverter(SiteDb.ContentFolders.SiteObjectType);
                                var FolderBytes = FolderSerilizer.ToBytes(folder);
                                string FolderPath = System.IO.Path.Combine(DiskPath, SiteDb.ContentFolders.StoreName);
                                IOHelper.EnsureDirectoryExists(FolderPath);
                                FolderPath = System.IO.Path.Combine(FolderPath, folder.Id.ToString() + InterExtension);
                                File.WriteAllBytes(FolderPath, FolderBytes);


                                var contenttype = SiteDb.ContentTypes.Get(folder.ContentTypeId);
                                var TypeSerilizer = new Kooboo.IndexedDB.Serializer.Simple.SimpleConverter(SiteDb.ContentTypes.SiteObjectType);
                                var typeBytes = TypeSerilizer.ToBytes(contenttype);
                                string TypePath = System.IO.Path.Combine(DiskPath, SiteDb.ContentTypes.StoreName);
                                IOHelper.EnsureDirectoryExists(TypePath);
                                TypePath = System.IO.Path.Combine(TypePath, contenttype.Id.ToString() + InterExtension);
                                File.WriteAllBytes(TypePath, typeBytes);
                            }

                            if (dbitem is ContentFolder)
                            {
                                var folder = dbitem as ContentFolder;
                                var contenttype = SiteDb.ContentTypes.Get(folder.ContentTypeId);
                                var TypeSerilizer = new Kooboo.IndexedDB.Serializer.Simple.SimpleConverter(SiteDb.ContentTypes.SiteObjectType);
                                var typeBytes = TypeSerilizer.ToBytes(contenttype);
                                string TypePath = System.IO.Path.Combine(DiskPath, SiteDb.ContentTypes.StoreName);
                                IOHelper.EnsureDirectoryExists(TypePath);
                                TypePath = System.IO.Path.Combine(TypePath, contenttype.Id.ToString() + InterExtension);
                                File.WriteAllBytes(TypePath, typeBytes);
                            }

                            if (dbitem is View)
                            {
                                var view = dbitem as View;
                                var allmethods = SiteDb.ViewDataMethods.Query.Where(o => o.ViewId == view.Id).SelectAll();

                                if (allmethods != null && allmethods.Count() > 0)
                                {
                                    var MethodSerializer = new Kooboo.IndexedDB.Serializer.Simple.SimpleConverter(SiteDb.ViewDataMethods.SiteObjectType);

                                    foreach (var method in allmethods)
                                    {
                                        var methodbytes = MethodSerializer.ToBytes(method);
                                        string methodPath = System.IO.Path.Combine(DiskPath, SiteDb.ViewDataMethods.StoreName);
                                        IOHelper.EnsureDirectoryExists(methodPath);
                                        methodPath = System.IO.Path.Combine(methodPath, method.Id.ToString() + InterExtension);
                                        File.WriteAllBytes(methodPath, methodbytes);
                                        // get the datamethod setting. 

                                        var datamethod = SiteDb.DataMethodSettings.Get(method.MethodId);

                                        if (datamethod != null)
                                        {
                                            var dataMethodSerializer = new Kooboo.IndexedDB.Serializer.Simple.SimpleConverter(SiteDb.DataMethodSettings.SiteObjectType);

                                            var datamethodBytes = dataMethodSerializer.ToBytes(datamethod);

                                            string DataMethodPath = System.IO.Path.Combine(DiskPath, SiteDb.DataMethodSettings.StoreName);
                                            IOHelper.EnsureDirectoryExists(DataMethodPath);
                                            DataMethodPath = System.IO.Path.Combine(DataMethodPath, datamethod.Id.ToString() + InterExtension);

                                            File.WriteAllBytes(DataMethodPath, datamethodBytes);

                                        }

                                    }
                                }
                            }
                        }
                    }
                    repo.Store.Close();
                }
            }

            return DiskPath;
        }
         
        public static string InterCopyTime(SiteDb SiteDb, Int64 timetick)
        {
            string DiskPath = Kooboo.Data.AppSettings.TempDataPath;
            DiskPath = System.IO.Path.Combine(DiskPath, System.Guid.NewGuid().ToString());
            IOHelper.EnsureDirectoryExists(DiskPath);

            var routeserializer = new Kooboo.IndexedDB.Serializer.Simple.SimpleConverter(SiteDb.Routes.SiteObjectType);

            string RoutePath = System.IO.Path.Combine(DiskPath, SiteDb.Routes.StoreName);

            var logs = SiteDb.Log.Store.Where(o => o.TimeTick >= timetick).SelectAll();

            foreach (var item in logs)
            {
                var repo = SiteDb.GetRepository(item.StoreName);
                if (!Attributes.AttributeHelper.IsCoreObject(repo.ModelType))
                {
                    continue;
                }

                string path = System.IO.Path.Combine(DiskPath, item.StoreName);
                IOHelper.EnsureDirectoryExists(path);

                var serializer = new Kooboo.IndexedDB.Serializer.Simple.SimpleConverter(repo.ModelType);

                if (item.EditType == IndexedDB.EditType.Delete)
                {

                }

                foreach (var dbitem in repo.All())
                {
                    if (dbitem.Id != default(Guid))
                    {
                        var bytes = serializer.ToBytes(dbitem);
                        string fullpath = System.IO.Path.Combine(path, dbitem.Id.ToString() + InterExtension);
                        File.WriteAllBytes(fullpath, bytes);

                        // routable object....  
                        if (Kooboo.Attributes.AttributeHelper.IsRoutable(dbitem))
                        {
                            var route = SiteDb.Routes.GetByObjectId(dbitem.Id);
                            if (route != null)
                            {
                                var routebytes = routeserializer.ToBytes(route);
                                string RouteFullPath = System.IO.Path.Combine(RoutePath, route.Id.ToString() + InterExtension);
                                IOHelper.EnsureFileDirectoryExists(RouteFullPath);
                                File.WriteAllBytes(RouteFullPath, routebytes);
                            }
                        }

                        if (dbitem is TextContent)
                        {
                            // need to have the content type and content folder. 
                            var content = dbitem as TextContent;
                            var folder = SiteDb.ContentFolders.Get(content.FolderId);
                            var FolderSerilizer = new Kooboo.IndexedDB.Serializer.Simple.SimpleConverter(SiteDb.ContentFolders.SiteObjectType);
                            var FolderBytes = FolderSerilizer.ToBytes(folder);
                            string FolderPath = System.IO.Path.Combine(DiskPath, SiteDb.ContentFolders.StoreName);
                            IOHelper.EnsureDirectoryExists(FolderPath);
                            FolderPath = System.IO.Path.Combine(FolderPath, folder.Id.ToString() + InterExtension);
                            File.WriteAllBytes(FolderPath, FolderBytes);


                            var contenttype = SiteDb.ContentTypes.Get(folder.ContentTypeId);
                            var TypeSerilizer = new Kooboo.IndexedDB.Serializer.Simple.SimpleConverter(SiteDb.ContentTypes.SiteObjectType);
                            var typeBytes = TypeSerilizer.ToBytes(contenttype);
                            string TypePath = System.IO.Path.Combine(DiskPath, SiteDb.ContentTypes.StoreName);
                            IOHelper.EnsureDirectoryExists(TypePath);
                            TypePath = System.IO.Path.Combine(TypePath, contenttype.Id.ToString() + InterExtension);
                            File.WriteAllBytes(TypePath, typeBytes);
                        }

                        if (dbitem is ContentFolder)
                        {
                            var folder = dbitem as ContentFolder;
                            var contenttype = SiteDb.ContentTypes.Get(folder.ContentTypeId);
                            var TypeSerilizer = new Kooboo.IndexedDB.Serializer.Simple.SimpleConverter(SiteDb.ContentTypes.SiteObjectType);
                            var typeBytes = TypeSerilizer.ToBytes(contenttype);
                            string TypePath = System.IO.Path.Combine(DiskPath, SiteDb.ContentTypes.StoreName);
                            IOHelper.EnsureDirectoryExists(TypePath);
                            TypePath = System.IO.Path.Combine(TypePath, contenttype.Id.ToString() + InterExtension);
                            File.WriteAllBytes(TypePath, typeBytes);
                        }

                        if (dbitem is View)
                        {
                            var view = dbitem as View;
                            var allmethods = SiteDb.ViewDataMethods.Query.Where(o => o.ViewId == view.Id).SelectAll();

                            if (allmethods != null && allmethods.Count() > 0)
                            {
                                var MethodSerializer = new Kooboo.IndexedDB.Serializer.Simple.SimpleConverter(SiteDb.ViewDataMethods.SiteObjectType);

                                foreach (var method in allmethods)
                                {
                                    var methodbytes = MethodSerializer.ToBytes(method);
                                    string methodPath = System.IO.Path.Combine(DiskPath, SiteDb.ViewDataMethods.StoreName);
                                    IOHelper.EnsureDirectoryExists(methodPath);
                                    methodPath = System.IO.Path.Combine(methodPath, method.Id.ToString() + InterExtension);
                                    File.WriteAllBytes(methodPath, methodbytes);
                                    // get the datamethod setting. 

                                    var datamethod = SiteDb.DataMethodSettings.Get(method.MethodId);

                                    if (datamethod != null)
                                    {
                                        var dataMethodSerializer = new Kooboo.IndexedDB.Serializer.Simple.SimpleConverter(SiteDb.DataMethodSettings.SiteObjectType);

                                        var datamethodBytes = dataMethodSerializer.ToBytes(datamethod);

                                        string DataMethodPath = System.IO.Path.Combine(DiskPath, SiteDb.DataMethodSettings.StoreName);
                                        IOHelper.EnsureDirectoryExists(DataMethodPath);
                                        DataMethodPath = System.IO.Path.Combine(DataMethodPath, datamethod.Id.ToString() + InterExtension);

                                        File.WriteAllBytes(DataMethodPath, datamethodBytes);

                                    }

                                }
                            }
                        }
                    }
                }
                repo.Store.Close();


            }



            return DiskPath;
        }

        public static void ImportInter(ZipArchive archive, SiteDb siteDb, Guid UserId = default(Guid))
        {
            SiteSetting setting = null;

            Kooboo.IndexedDB.Dynamic.TableSetting tablesetting = null;
            List<TableData> tableData = new List<TableData>();

            List<InterObject> interObjectList = new List<InterObject>();

            foreach (var entry in archive.Entries)
            {
                var target = entry.FullName;

                if (!string.IsNullOrEmpty(entry.Name))
                {
                    MemoryStream memory = new MemoryStream();
                    entry.Open().CopyTo(memory);
                    var bytes = memory.ToArray();

                    if (bytes != null && bytes.Length > 0)
                    {
                        if (entry.Name == KoobooSettingFileName)
                        {
                            string value = System.Text.Encoding.UTF8.GetString(bytes);
                            setting = Lib.Helper.JsonHelper.Deserialize<SiteSetting>(value);
                        }
                        else if (entry.Name == TableSettingFileName)
                        {
                            string value = System.Text.Encoding.UTF8.GetString(bytes);
                            tablesetting = JsonHelper.Deserialize<Kooboo.IndexedDB.Dynamic.TableSetting>(value);
                        }
                        else
                        {
                            var interobject = GetInterObject(target);

                            if (interobject.StoreName == null)
                            {
                                if (target.StartsWith("__fileio__"))
                                {
                                    var filename = target.Replace("__fileio__", "");
                                    var folder = Kooboo.Data.AppSettings.GetFileIORoot(siteDb.WebSite);
                                    string fullfilepath = Lib.Helper.IOHelper.CombinePath(folder, filename);

                                    Lib.Helper.IOHelper.WriteAllBytes(fullfilepath, bytes);
                                }
                            }

                            else if (interobject.StoreName.StartsWith("_table_"))
                            {
                                string tablename = interobject.StoreName.Replace("_table_", "");

                                string datastring = System.Text.Encoding.UTF8.GetString(bytes);

                                if (!string.IsNullOrEmpty(tablename) && !string.IsNullOrWhiteSpace(datastring) && interobject.ObjectId != default(Guid))
                                {
                                    tableData.Add(new TableData() { Id = interobject.ObjectId, DataString = datastring, TableName = tablename });
                                }
                            }


                            else
                            {
                                interobject.Binary = bytes;
                                interObjectList.Add(interobject);

                                //var repo = siteDb.GetRepository(interobject.StoreName);
                                //if (repo != null)
                                //{
                                //    if (interobject.IsDelete)
                                //    {
                                //        repo.Delete(interobject.ObjectId);
                                //    }
                                //    else
                                //    {
                                //        var back = Kooboo.Data.Helper.SimpleSerializerHelper.Deserialize(bytes, repo.ModelType) as ISiteObject;
                                //        if (back != null)
                                //        {
                                //            back.Id = interobject.ObjectId;
                                //            repo.AddOrUpdate(back, UserId);
                                //        }
                                //    }
                                //}
                            }

                        }
                    }
                }
            }


            foreach (var item in interObjectList.OrderBy(o => o.ImportOrder))
            {
                var repo = siteDb.GetRepository(item.StoreName);
                if (repo != null)
                {
                    if (item.IsDelete)
                    {
                        repo.Delete(item.ObjectId);
                    }
                    else
                    {
                        var back = Kooboo.Data.Helper.SimpleSerializerHelper.Deserialize(item.Binary, repo.ModelType) as ISiteObject;
                        if (back != null)
                        {
                            back.Id = item.ObjectId;
                            repo.AddOrUpdate(back, UserId);
                        }
                    }
                }
            }


            if (setting != null)
            {
                SetSiteSetting(siteDb.WebSite, setting);
                GlobalDb.WebSites.AddOrUpdate(siteDb.WebSite);

                var version = Kooboo.Sites.Upgrade.UpgradeManager.ParseVersion(setting.KoobooVersion);
                var siteupgrader = Kooboo.Sites.Upgrade.UpgradeManager.GetSiteUpgraderList(version);
                if (siteupgrader != null && siteupgrader.Count() > 0)
                {
                    foreach (var item in siteupgrader)
                    {
                        item.Do(siteDb.WebSite);
                    }
                }

                if (setting.EnableFullTextSearch)
                {
                    siteDb.SearchIndex.Rebuild();
                }
            }

            if (tablesetting != null)
            {
                var kdatabase = Kooboo.Data.DB.GetKDatabase(siteDb.WebSite);

                Kooboo.IndexedDB.Dynamic.Sync.SetTableSetting(kdatabase, tablesetting);

                foreach (var item in tableData.GroupBy(o => o.TableName))
                {
                    var tablename = item.Key;

                    if (!string.IsNullOrEmpty(tablename))
                    {
                        var table = Data.DB.GetOrCreateTable(kdatabase, tablename);

                        foreach (var data in item.ToList())
                        {
                            var dict = Lib.Helper.JsonHelper.Deserialize<Dictionary<string, object>>(data.DataString);

                            var old = table.Get(data.Id);
                            if (old == null)
                            {
                                table.Add(dict, true);
                            }
                            else
                            {
                                // should not be.... 
                                table.Update(data.Id, dict);
                            }
                        }
                    }

                }

            }

        }

        public static InterObject GetInterObject(string FullPath)
        {
            FullPath = FullPath.Replace("/", "\\");

            if (!FullPath.Contains("\\"))
            {
                return null;
            }

            string[] segs = FullPath.Split('\\');

            InterObject inter = new InterObject();

            foreach (var item in segs.Reverse())
            {
                if (string.IsNullOrEmpty(item))
                {
                    continue;
                }

                if (inter.ObjectId == default(Guid))
                {
                    if (item.EndsWith(InterExtension))
                    {
                        string strguid = item.Substring(0, item.Length - InterExtension.Length);

                        Guid ObjectId;
                        if (System.Guid.TryParse(strguid, out ObjectId))
                        {
                            inter.ObjectId = ObjectId;
                        }
                        else
                        {
                            return null;
                        }
                    }
                }

                else if (string.IsNullOrEmpty(inter.StoreName))
                {
                    inter.StoreName = item;
                }
                else
                {
                    if (item == "delete")
                    {
                        inter.IsDelete = true;
                    }

                    break;
                }
            }

            if (inter.StoreName != null)
            {
                if (InnerImportOrder.ContainsKey(inter.StoreName))
                {
                    inter.ImportOrder = InnerImportOrder[inter.StoreName];
                }
                else
                {
                    inter.ImportOrder = 999;
                }
            }

            return inter;
        }

        public static SiteSetting GetSiteSetting(Data.Models.WebSite site)
        {
            SiteSetting setting = new SiteSetting();
            setting.Culture = site.Culture;
            setting.CustomErrors = site.CustomErrors;
            setting.CustomSettings = site.CustomSettings;
            setting.DefaultCulture = site.DefaultCulture;
            setting.EnableMultilingual = site.EnableMultilingual;
            setting.EnableSitePath = site.EnableSitePath;
            setting.SitePath = site.SitePath;
            setting.AutoDetectCulture = site.AutoDetectCulture;
            setting.ForceSsl = site.ForceSSL;
            setting.KoobooVersion = AppSettings.Version.ToString();

            setting.EnableFullTextSearch = site.EnableFullTextSearch;
            setting.EnableFrontEvents = site.EnableFrontEvents;
            setting.EnableConstraintChecker = site.EnableConstraintChecker;
            setting.EnableConstraintFixOnSave = site.EnableConstraintFixOnSave;
            setting.EnableECommerce = site.EnableECommerce;
            setting.EnableCache = site.EnableCache;

            setting.IsApp = site.IsApp;
            setting.SiteType = site.SiteType;


            return setting;
        }

        public static void SetSiteSetting(WebSite site, SiteSetting setting)
        {
            foreach (var item in setting.CustomErrors)
            {
                site.CustomErrors[item.Key] = item.Value;
            }

            foreach (var item in setting.CustomSettings)
            {
                site.CustomSettings[item.Key] = item.Value;
            }

            if (!string.IsNullOrEmpty(setting.DefaultCulture))
            {
                site.DefaultCulture = setting.DefaultCulture;
            }

            if (setting.EnableMultilingual)
            {
                site.EnableMultilingual = true;
            }

            if (setting.EnableSitePath)
            {
                site.EnableSitePath = true;

                foreach (var item in setting.SitePath)
                {
                    site.SitePath[item.Key] = item.Value;
                }
            }

            site.AutoDetectCulture = setting.AutoDetectCulture;
            site.ForceSSL = setting.ForceSsl;


            site.EnableFullTextSearch = setting.EnableFullTextSearch;
            site.EnableFrontEvents = setting.EnableFrontEvents;
            site.EnableConstraintChecker = setting.EnableConstraintChecker;
            site.EnableConstraintFixOnSave = setting.EnableConstraintFixOnSave;
            site.EnableECommerce = setting.EnableECommerce;
            site.EnableCache = setting.EnableCache;


            site.Culture.Clear();
            foreach (var item in setting.Culture)
            {
                site.Culture[item.Key] = item.Value;
            }

            var notfoundculture = site.Culture.Where(o => !setting.Culture.ContainsKey(o.Key)).ToList();

            if (notfoundculture != null)
            {
                foreach (var item in notfoundculture)
                {
                    site.Culture.Remove(item.Key);
                }
            }

            site.IsApp = setting.IsApp;
            site.SiteType = setting.SiteType;

            // site.KoobooVersion = setting.KoobooVersion; 
        }


        #region BatchPackage


        public static bool IsKoobooBatch(ZipArchive archive)
        {
            foreach (var item in archive.Entries)
            {
                if (item.Name.EndsWith(BatchSettingFileName))
                {
                    return true;
                }
            }
            return false;
        }

        public static List<LogEntry> BatchGetFromIdLogs(SiteDb sitedb, long fromid)
        {
            var alllogs = sitedb.Log.Store.Where(o => o.Id >= fromid).SelectAll();
            return CleanLogItems(sitedb, alllogs);
        }

        public static List<LogEntry> BatchGetLogs(SiteDb sitedb, List<long> Ids)
        {
            List<LogEntry> result = new List<LogEntry>();
            foreach (var id in Ids)
            {
                var item = sitedb.Log.Get(id);
                if (item != null && item.KeyBytes != null)
                {
                    result.Add(item);
                }
            }
            return result;
        }


        public static string ExportBatch(SiteDb sitedb, long byfromId)
        {
            var logs = BatchGetFromIdLogs(sitedb, byfromId);
            if (logs == null || logs.Count() == 0)
            {
                return null;
            }

            return BatchCopy(sitedb, logs);
        }

        public static string ExportBatch(SiteDb sitedb, List<long> Ids)
        {
            var logs = BatchGetLogs(sitedb, Ids);
            if (logs == null || logs.Count() == 0)
            {
                return null;
            }
            return BatchCopy(sitedb, logs);
        }

        // return zip file. 
        public static string BatchCopy(SiteDb SiteDb, List<LogEntry> items)
        {
            string DiskPath = Kooboo.Data.AppSettings.TempDataPath;
            DiskPath = System.IO.Path.Combine(DiskPath, System.Guid.NewGuid().ToString());
            IOHelper.EnsureDirectoryExists(DiskPath);

            var serializer = new Kooboo.IndexedDB.Serializer.Simple.SimpleConverter<SyncObject>();

            foreach (var item in items)
            {
                var obj = Kooboo.Sites.Sync.SyncService.Prepare(SiteDb, item);
                obj.SenderVersion = item.Id;
                var bytes = serializer.ToBytes(obj);
                string fullpath = System.IO.Path.Combine(DiskPath, item.Id.ToString() + ".kbbatch");

                File.WriteAllBytes(fullpath, bytes);
            }

            string checkfile = System.IO.Path.Combine(DiskPath, BatchSettingFileName);

            File.WriteAllText(checkfile, "kooboo batch file identifier, do not modify");

            string strguid = System.Guid.NewGuid().ToString();

            var zipFile = System.IO.Path.Combine(AppSettings.TempDataPath, strguid + ".zip");
            if (File.Exists(zipFile))
            {
                File.Delete(zipFile);
            }

            var newstream = new FileStream(zipFile, FileMode.OpenOrCreate);

            var newarchive = new ZipArchive(newstream, ZipArchiveMode.Create, false);

            var files = Directory.GetFiles(DiskPath, "*.*", SearchOption.TopDirectoryOnly);

            foreach (var path in files)
            {
                newarchive.CreateEntryFromFile(path, path.Replace(DiskPath, "").Trim('\\'));
            }

            newarchive.Dispose();

            newstream.Dispose();

            return zipFile;

        }

        public static List<LogEntry> CleanLogItems(SiteDb siteDb, List<LogEntry> items)
        {
            List<LogEntry> result = new List<LogEntry>();
            Dictionary<Guid, long> ObjectLastSyncVersion = new Dictionary<Guid, long>();

            long LastVersion = -1;

            foreach (var item in items.OrderByDescending(o => o.Id))
            {
                if (item == null || item.KeyBytes == null)
                {
                    continue;
                }

                Guid key = siteDb.Pages.Store.KeyConverter.FromByte(item.KeyBytes);

                if (!ObjectLastSyncVersion.ContainsKey(key))
                {
                    ObjectLastSyncVersion[key] = item.Id;
                    LastVersion = item.Id;
                }
                else
                {
                    LastVersion = ObjectLastSyncVersion[key];
                    if (LastVersion > item.Id)
                    {
                        continue;
                    }
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

        public static void ImportBatch(ZipArchive archive, SiteDb siteDb, Guid UserId = default(Guid))
        {
            var serializer = new Kooboo.IndexedDB.Serializer.Simple.SimpleConverter<SyncObject>();

            List<SyncObject> result = new List<SyncObject>();

            foreach (var entry in archive.Entries)
            {
                var target = entry.FullName;

                if (!string.IsNullOrEmpty(entry.Name) && entry.Name.EndsWith(".kbbatch"))
                {
                    MemoryStream memory = new MemoryStream();
                    entry.Open().CopyTo(memory);
                    var bytes = memory.ToArray();

                    var obj = serializer.FromBytes(bytes);

                    if (obj != null)
                    {
                        result.Add(obj);
                    }
                }
            }


            foreach (var item in result.OrderBy(o => o.SenderVersion))
            {
                Kooboo.Sites.Sync.SyncService.Receive(siteDb, item, null, UserId);
            }
        }

        #endregion
    }


    public class TableData
    {
        public string TableName { get; set; }
        public string DataString { get; set; }

        public Guid Id { get; set; }
    }

    public class InterObject
    {
        public string StoreName { get; set; }

        public Guid ObjectId { get; set; }

        public bool IsDelete { get; set; }

        public int ImportOrder { get; set; }

        public byte[] Binary { get; set; }

    }
}
