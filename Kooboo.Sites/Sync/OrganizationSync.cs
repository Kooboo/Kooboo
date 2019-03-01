//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data;
using Kooboo.Data.Models;
using Kooboo.Lib.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;


namespace Kooboo.Sites.Sync
{
    public static class OrganizationSync
    {
        public static byte[] ReadZip(Guid PackageId)
        {
            string filename = System.IO.Path.Combine(AppSettings.TempDataPath, PackageId.ToString("N") + ".zip");
            if (System.IO.File.Exists(filename))
            {
                return System.IO.File.ReadAllBytes(filename); 
            }
            return null; 
        }


        public static Guid GeneratePackage(Guid OrganizationId)
        {
            var sites = Kooboo.Data.GlobalDb.WebSites.AllSites.Values.Where(o => o.OrganizationId == OrganizationId).ToList();

            foreach (var item in sites)
            {
                item.Published = false;
                Kooboo.Sites.Cache.WebSiteCache.SetNull(item.Id);
                Kooboo.Mail.Factory.DBFactory.SetNull(item.OrganizationId);
            }

            // Next we pack. 
            Guid guid = System.Guid.NewGuid();

            string filename = System.IO.Path.Combine(AppSettings.TempDataPath, guid.ToString("N") + ".zip");

            Lib.Helper.IOHelper.EnsureFileDirectoryExists(filename);

            var orgfolder = Kooboo.Data.AppSettings.GetOrganizationFolder(OrganizationId);

            var newstream = new FileStream(filename, FileMode.OpenOrCreate);
            var newarchive = new ZipArchive(newstream, ZipArchiveMode.Create, false);

            string newtempfolder = System.IO.Path.Combine(AppSettings.TempDataPath, System.Guid.NewGuid().ToString());
            Lib.Helper.IOHelper.EnsureDirectoryExists(newtempfolder);

            Lib.Helper.IOHelper.DirectoryCopy(orgfolder, newtempfolder, true); 
             
            var files = Directory.GetFiles(newtempfolder, "*.*", SearchOption.AllDirectories);

            foreach (var path in files)
            {
                newarchive.CreateEntryFromFile(path, path.Replace(newtempfolder, "").Trim('\\').Trim('/'));
            }

            List<OrgSetting> settings = new List<OrgSetting>();
            foreach (var item in sites)
            {
                item.Published = true;
                Kooboo.Sites.Cache.WebSiteCache.RemoveNull(item.Id);
                Kooboo.Mail.Factory.DBFactory.RemoveNull(item.OrganizationId);

                OrgSetting setting = new OrgSetting();
                setting.Site = item;
                setting.Bindings = Kooboo.Data.GlobalDb.Bindings.GetByWebSite(item.Id);

                settings.Add(setting);
            }

            var json = Lib.Helper.JsonHelper.Serialize(settings);

            string settingfilename = System.IO.Path.Combine(AppSettings.TempDataPath, System.Guid.NewGuid().ToString() + ".json");

            System.IO.File.WriteAllText(settingfilename, json);

            newarchive.CreateEntryFromFile(settingfilename, "sites.json");

            //sssl. 
            var ssls = Kooboo.Data.GlobalDb.SslCertificate.ListByOrganization(OrganizationId);
            if (ssls !=null && ssls.Any())
            {
                var sslJson = Lib.Helper.JsonHelper.Serialize(ssls);

                string sslFilename = System.IO.Path.Combine(AppSettings.TempDataPath, System.Guid.NewGuid().ToString() + ".ssl");

                System.IO.File.WriteAllText(sslFilename, sslJson);

                newarchive.CreateEntryFromFile(sslFilename, "ssl.json");

            }

            newarchive.Dispose();
            newstream.Dispose();
            return guid;
        }


        private static bool EqualName(string path, string target)
        {
            if(path == null)
            {
                return false; 
            }
            if (path.StartsWith("\\")|| path.StartsWith("/"))
            {
                path = path.Substring(1); 
            }

            return path == target; 
        }


        public static bool ImportOrg(ZipArchive archive, Guid OrganizationId)
        { 

            var folder = Kooboo.Data.AppSettings.GetOrganizationFolder(OrganizationId);
            if (System.IO.Directory.Exists(folder))
            { 
                try
                {
                    var websites = Kooboo.Data.GlobalDb.WebSites.ListByOrg(OrganizationId);
                    foreach (var item in websites)
                    {
                        item.Published = false; 
                    }

                    foreach (var item in websites)
                    {
                        Kooboo.Sites.Service.WebSiteService.Delete(item.Id); 
                    }

                    var dirs = System.IO.Directory.GetDirectories(folder); 
                    if (dirs !=null && dirs.Any())
                    {
                        System.IO.Directory.Delete(folder, true);
                    } 
                }
                catch (Exception)
                {
                    return false; 
                } 
            }

            IOHelper.EnsureDirectoryExists(folder);

            List<OrgSetting> settings = null;
            List<SslCertificate> ssls = null; 

            foreach (var entry in archive.Entries)
            {  
                if (EqualName(entry.Name, "sites.json"))
                {
                    using (var stream = entry.Open())
                    {
                        StreamReader reader = new StreamReader(stream);
                        var alltext = reader.ReadToEnd();
                        settings = Lib.Helper.JsonHelper.Deserialize<List<OrgSetting>>(alltext);
                    }  
                }
                else if (EqualName(entry.Name, "ssl.json"))
                {
                    using (var stream = entry.Open())
                    {
                        StreamReader reader = new StreamReader(stream);
                        var alltext = reader.ReadToEnd();
                        ssls = Lib.Helper.JsonHelper.Deserialize<List<SslCertificate>>(alltext);
                    }
                }
                else
                {
                    var path =  Lib.Compatible.CompatibleManager.Instance.System.CombinePath(folder, entry.FullName);

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


            }

            if (settings !=null)
            {
                foreach (var item in settings)
                {
                    item.Site.OrganizationId = OrganizationId;
                    item.Site.Id = default(Guid);  // reset id.  
                    item.Site.DiskSyncFolder = null;
                    item.Site.EnableDiskSync = false; 
                    Kooboo.Data.GlobalDb.WebSites.AddOrUpdate(item.Site);
                    foreach (var binding in item.Bindings)
                    {
                        binding.WebSiteId = item.Site.Id;
                        binding.OrganizationId = OrganizationId; 
                        GlobalDb.Bindings.AddOrUpdate(binding); 
                    }
                }
            }

            if (ssls !=null)
            {
                foreach (var item in ssls)
                {
                    item.OrganizationId = OrganizationId;  
                    Kooboo.Data.GlobalDb.SslCertificate.AddOrUpdate(item); 
                } 
            }
            return true;
        }

        public static bool ImportOrg(byte[] contentbytes, Guid OrganizationId)
        {
            try
            {
                ZipArchive archive = new ZipArchive(new MemoryStream(contentbytes));

                return ImportOrg(archive, OrganizationId); 
            }
            catch (Exception)
            {
                 
            }

            return false; 
        }

    }


    public class OrgSetting
    {
        public WebSite Site { get; set; }

        public List<Binding> Bindings { get; set; }
    }

}
