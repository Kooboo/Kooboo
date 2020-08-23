using System;
using System.Collections.Generic;
using System.Linq;
using Kooboo.Data.Interface;
using Kooboo.Data.Models;
using Kooboo.Lib.Helper;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Models;
using Kooboo.Sites.Repository;
using Kooboo.Sites.Sync.DiskSyncLog;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Kooboo.Sites.Sync.Disk
{ 

    public class SyncManager
    {  
        public SyncManager(Guid WebSiteId)
        {
            this.WebSiteId = WebSiteId; 
        }

        private Guid WebSiteId { get; set; }

        internal void ProcessDiskEvent(WebSite website, DiskChangeEvent task)
        {
            var sitedb = website.SiteDb();

            if (task.ChangeType == DiskChangeType.Deleted)
            {
                this.DeleteFromDb(task.FullPath, sitedb);
                DiskSyncLog.DiskLogManager.Delete(task.FullPath, sitedb.WebSite.Id);
            }
            else
            {
                this.SyncToDb(task.FullPath, sitedb, null);
                DiskSyncLog.DiskLogManager.Add(task.FullPath, sitedb.WebSite.Id);
            }
        }

        public void DeleteFromDb(string DiskFullPath, SiteDb sitedb)
        {
            var NonRoutable = DiskPathService.GetNonRoutableObject(DiskFullPath);

            if (NonRoutable != null)
            {
                var repo = sitedb.GetRepository(NonRoutable.StoreName);
                string name = string.IsNullOrWhiteSpace(NonRoutable.Name) ? NonRoutable.Id.ToString() : NonRoutable.Name;
                var result = repo.GetByNameOrId(name) as ISiteObject;
                if (result != null)
                {
                    repo.Delete(result.Id);
                }
            }
            else
            {
                var RelativeUrl = DiskPathService.GetRelativeUrl(sitedb.WebSite, DiskFullPath);
                var route = sitedb.Routes.GetByUrl(RelativeUrl);
                if (route != null)
                {
                    var repo = sitedb.GetRepository(route.DestinationConstType);
                    if (repo != null)
                    {
                        var result = repo.Get(route.objectId) as ISiteObject;
                        if (result != null)
                        {
                            repo.Delete(result.Id);
                        }
                    }
                }
            }
        }

        public void SyncToDb(string FullPath, SiteDb SiteDb, byte[] diskbytes = null)
        {
            if (diskbytes == null)
            {
                diskbytes = this.ReadAllBytes(FullPath);
            }
            if (diskbytes == null)
            {
                return;
            }

            string OldRelativeUrl = null;
            string RelativeUrl = null;

            IRepository repo = null;
            ISiteObject result = null;
            Routing.Route route = null;
            string NameFromFile = null;
            string extension = UrlHelper.FileExtension(FullPath);
            if (!string.IsNullOrEmpty(extension) && !extension.StartsWith("."))
            {
                extension = "." + extension;
            }
            if (!string.IsNullOrEmpty(extension))
            {
                extension = extension.ToLower();
            }

            var NonRoutable = DiskPathService.GetNonRoutableObject(FullPath);

            if (NonRoutable != null)
            {
                repo = SiteDb.GetRepository(NonRoutable.StoreName);
                NameFromFile = NonRoutable.Name;
                string name = string.IsNullOrWhiteSpace(NonRoutable.Name) ? NonRoutable.Id.ToString() : NonRoutable.Name;
                if (!string.IsNullOrEmpty(NonRoutable.Extension))
                {
                    extension = NonRoutable.Extension.ToLower();
                    if (!extension.StartsWith("."))
                    {
                        extension = "." + extension;
                    }
                }

                result = repo.GetByNameOrId(name) as ISiteObject;

                if (result == null)
                {
                    if (name.ToLower().EndsWith(extension))
                    {
                        name = name.Substring(0, name.Length - extension.Length);
                        result = repo.GetByNameOrId(name);
                    }
                    else
                    {
                        name = name + extension;
                        result = repo.GetByNameOrId(name);
                    }
                }
            }
            else
            {
                OldRelativeUrl = DiskPathService.GetRelativeUrl(SiteDb.WebSite, FullPath);
                RelativeUrl = Kooboo.Sites.Helper.RouteHelper.ToValidRoute(OldRelativeUrl);

                route = SiteDb.Routes.GetByUrl(RelativeUrl);
                if (route != null)
                {
                    repo = SiteDb.GetRepository(route.DestinationConstType);
                    result = repo.Get(route.objectId) as ISiteObject;
                }
                else
                {
                    var ModelType = Service.ConstTypeService.GetModelTypeByUrl(RelativeUrl);
                    if (ModelType == null) { return; }
                    repo = SiteDb.GetRepository(ModelType);
                }
                NameFromFile = UrlHelper.FileName(RelativeUrl);
            }

            if (result == null)
            {
                result = Activator.CreateInstance(repo.ModelType) as ISiteObject;
            }

            if (!DiskObjectConverter.FromBytes(ref result, diskbytes))
            {
                return;
            }

            if (result is IExtensionable)
            {
                var extensionfile = result as IExtensionable;
                extensionfile.Extension = extension;
            }

            if (string.IsNullOrEmpty(result.Name))
            {
                result.Name = Lib.Helper.StringHelper.ToValidFileName(NameFromFile);
            }

            #region "Routing"

            if (!string.IsNullOrEmpty(RelativeUrl))
            {
                SiteDb.Routes.AddOrUpdate(RelativeUrl, result as SiteObject);
            }
            else
            {
                // # Rule1, only the API is different...
                if (result is Kooboo.Sites.Models.Code)
                {
                    var code = result as Code;
                    if (code.CodeType == CodeType.Api)
                    {
                        bool shouldUpdateCodeRouteText = false;

                        var diskroute = DiskObjectConverter.GetRouteFromCodeBytes(diskbytes);
                        if (string.IsNullOrWhiteSpace(diskroute))
                        {
                            // # Rule2, Api must have a route defined, otherwise it is a new api. 
                            var newroute = DiskObjectConverter.GetNewRoute(SiteDb, code.Name);
                            SiteDb.Routes.AddOrUpdate(newroute, code);
                            shouldUpdateCodeRouteText = true;
                        }
                        else
                        {
                            // # Rule 3, Check if this is its own route, or someelse routes. 
                            // Own rule, do nothing. 
                            var coderoute = SiteDb.Routes.Get(diskroute);
                            if (coderoute == null)
                            {
                                //#Rule 4, If route does not exists yet. Add and end. 
                                SiteDb.Routes.AddOrUpdate(diskroute, code);
                            }
                            else
                            {
                                if (coderoute.objectId != default(Guid) && coderoute.objectId != code.Id)
                                {
                                    // #Rule 5, This is route for others... get a new route.
                                    var newcoderoute = DiskObjectConverter.GetNewRoute(SiteDb, diskroute);
                                    SiteDb.Routes.AddOrUpdate(newcoderoute, code);
                                    shouldUpdateCodeRouteText = true;
                                }
                            }

                        }

                        if (shouldUpdateCodeRouteText)
                        {
                            this.SyncToDisk(SiteDb, code, ChangeType.Update, SiteDb.Code.StoreName);
                        }

                    }

                }
            }

            #endregion

            if (!isSameName(result.Name, NameFromFile, extension) || OldRelativeUrl != RelativeUrl)
            {
                if (File.Exists(FullPath))
                {
                    File.Delete(FullPath);
                }
                repo.AddOrUpdate(result);
            }

            else
            {

                repo.AddOrUpdate(result);

            }
        }

        private bool isSameName(string x, string y, string extension)
        {
            extension = extension.ToLower();
            if (!extension.StartsWith("."))
            {
                extension = "." + extension;
            }

            if (x == null || y == null)
            {
                return true;
            }
            if (string.IsNullOrWhiteSpace(extension))
            {
                return x.ToLower() == y.ToLower();
            }
            else
            {
                if (x.ToLower().EndsWith(extension))
                {
                    x = x.Substring(0, x.Length - extension.Length);
                }

                if (y.ToLower().EndsWith(extension))
                {
                    y = y.Substring(0, y.Length - extension.Length);
                }

                return x.ToLower() == y.ToLower();

            }
        }

        public string SyncToDisk(SiteDb SiteDb, ISiteObject Value, ChangeType ChangeType, string StoreName)
        {
            if (Attributes.AttributeHelper.IsDiskable(Value) && !IsEmbedded(Value) && !string.IsNullOrEmpty(StoreName))
            {
                var value = Value as ISiteObject;
                string relativeurl = DiskPathService.GetObjectRelativeUrl(value, SiteDb, StoreName);

                if (!string.IsNullOrEmpty(relativeurl))
                {
                    string fullpath = DiskPathService.GetFullDiskPath(SiteDb.WebSite, relativeurl);

                    if (ChangeType == ChangeType.Delete)
                    {
                        if (File.Exists(fullpath))
                        {
                            this.Delete(fullpath);
                            DiskSyncLog.DiskLogManager.Delete(fullpath, SiteDb.Id);
                            return fullpath;
                        }
                    }

                    else
                    {
                        var coreobject = value as ICoreObject;

                        if (coreobject != null)
                        {
                            var contentbytes = DiskObjectConverter.ToBytes(SiteDb, value);

                            this.WriteBytes(fullpath, contentbytes);

                            DiskSyncLog.DiskLogManager.Add(fullpath, SiteDb.Id);

                            return fullpath;
                        }
                    }
                }

            }

            return null;
        }
         
        public void InitSyncToDisk()
        {
            var website = Kooboo.Data.GlobalDb.WebSites.Get(this.WebSiteId);
            var sitedb = website.SiteDb();

            var allrepos = sitedb.ActiveRepositories();
            foreach (var repo in allrepos)
            {
                if (Kooboo.Attributes.AttributeHelper.IsDiskable(repo.ModelType))
                {
                    var allitems = repo.All();

                    foreach (var item in allitems)
                    {
                        SyncToDisk(sitedb, item, ChangeType.Add, repo.StoreName);
                    }
                }
            }
        }

        private bool IsEmbedded(ISiteObject Value)
        {
            if (Value is IEmbeddable)
            {
                IEmbeddable embedded = Value as IEmbeddable;
                return embedded.IsEmbedded;
            }
            return false;
        }

        private object _IOLocker = new object();

        private byte[] ReadAllBytes(string FilePath)
        {
            lock (_IOLocker)
            {
                if (File.Exists(FilePath))
                {
                    int i = 0;
                    System.Threading.Thread.Sleep(10);  //TODO:  this is very strange action, otherwise, file will be being used.  
                    while (i < 10)
                    {
                        try
                        {
                            if (File.Exists(FilePath))
                            {
                                var bytes = File.ReadAllBytes(FilePath);
                                return bytes;
                            }
                        }
                        catch (Exception)
                        {
                            System.Threading.Thread.Sleep(100);
                        }
                        i += 1;
                    }
                }
                return null;
            }
        }

        internal void WriteBytes(string FullPath, byte[] Value)
        {
            if (Value == null)
            {
                Value = new byte[0];
            }
            lock (_IOLocker)
            {
                IOHelper.EnsureFileDirectoryExists(FullPath);

                int i = 0;
                while (i < 10)
                {
                    try
                    {
                        System.IO.FileStream stream = new FileStream(FullPath, FileMode.Create);

                        stream.Write(Value, 0, Value.Length);
                        stream.Close();
                        stream.Dispose();

                    }
                    catch (Exception)
                    {
                        System.Threading.Thread.Sleep(10);
                    }
                    i = i + 1;
                }

            }
        }

        internal void Delete(string fullpath)
        {
            try
            {
                System.IO.File.Delete(fullpath);
            }
            catch (Exception)
            {

            }
        }
    }
}
