//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Kooboo.Data;
using Kooboo.Lib.Helper;
namespace Kooboo.App
{
    public class UpgradeHelper
    {
        public static string UpgradeName;
        public static string UpgradeFullPath;

        private static string KoobooZipFullName { get; set; }

        public static string RootPath { get; set; }

        private static string UpgradeLogPath { get; set; }

        static UpgradeHelper()
        {
            RootPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            UpgradeName = "Kooboo.App.Upgrade.dll";

            UpgradeFullPath = System.IO.Path.Combine(RootPath, UpgradeName);

            string path = System.IO.Path.Combine(RootPath, "upgradePackage");
            KoobooZipFullName = System.IO.Path.Combine(path, "Kooboo.zip");

            UpgradeLogPath = System.IO.Path.Combine(RootPath, "upgradePackage", "upgradeLog.txt");
        }
        public static void Log(string content)
        {
            var log = string.Format("{0}-----{1}{2}", DateTime.Now.ToString(), content, Environment.NewLine);
            Lib.Helper.IOHelper.EnsureFileDirectoryExists(UpgradeLogPath);
            System.IO.File.AppendAllText(UpgradeLogPath, log);
        }

        public static bool HasNewVersion()
        {
            var version = new Version("0.0.0.0");
            try
            {
                string url = AppSettings.ConvertApiUrl + "/_api/converter/GetLatestVersion?isLinux=true";
                var task = HttpHelper.GetAsync<string>(url);

                var versionStr = task.Result;
                if (!string.IsNullOrWhiteSpace(versionStr))
                {
                    version = new Version(versionStr);
                }
            }
            catch (Exception ex)
            {
            }
            Log(string.Format("new version:{0},oldersion:{1}", version, AppSettings.Version));
            return version > AppSettings.Version;
        }
        public static void Download()
        {
            try
            {
                Log("download begin");
                var url = AppSettings.ConvertApiUrl + "/_api/converter/DownloadUpgradePackage?isLinux=true";
                DownloadZip(url);
                Log("download successfully");
            }
            catch (Exception ex)
            {
                Log("download failed" + ex.Message);

            }
            UpdateKoobooUpgradeDll();
        }

        private static void UpdateKoobooUpgradeDll()
        {
            var zipBytes = System.IO.File.ReadAllBytes(KoobooZipFullName);
            byte[] upgradebytes = Kooboo.Data.Upgrade.UpgradeHelper.ExtractFileFromZip(zipBytes, UpgradeName);
            if (upgradebytes != null)
            {
                System.IO.File.WriteAllBytes(UpgradeFullPath, upgradebytes);
            }
        }

        public static void DownloadZip(string url)
        {
            var task = DownloadKoobooZip(url);
            var package = task.Result;
            if (package != null)
            {
                Lib.Helper.IOHelper.EnsureFileDirectoryExists(KoobooZipFullName);
                if (System.IO.File.Exists(KoobooZipFullName))
                    System.IO.File.Delete(KoobooZipFullName);
                if (package.Length > 0)
                    System.IO.File.WriteAllBytes(KoobooZipFullName, package);
            }
        }
        private static async Task<byte[]> DownloadKoobooZip(string url)
        {
            var client = new System.Net.WebClient();

            Uri uri = new Uri(url);
            var bytes = await client.DownloadDataTaskAsync(uri);
            if (client.ResponseHeaders.AllKeys.Contains("filehash"))
            {
                var hash = client.ResponseHeaders["filehash"];
                if (hash != null)
                {
                    Guid hashguid = default(Guid);

                    if (Guid.TryParse(hash, out hashguid))
                    {
                        var newhash = Lib.Security.Hash.ComputeGuid(bytes);
                        if (hashguid == newhash)
                        {
                            return bytes;
                        }
                    }
                }
            }
            return bytes;
        }
    }
}
