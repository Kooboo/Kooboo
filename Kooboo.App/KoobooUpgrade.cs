//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using Kooboo.Lib.Helper;
using System.IO.Compression;
using Kooboo.Data;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Linq;
using System.Threading;
using System.Reflection;
using System.IO;

namespace Kooboo.App
{
    public class KoobooUpgrade
    {
        static KoobooUpgrade()
        {
            string root = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

            string path = System.IO.Path.Combine(root, "upgradePackage");
            KoobooZipFullName = System.IO.Path.Combine(path, "Kooboo.zip");
#if DEBUG
            UpgradeExeFullName = System.IO.Path.Combine(root, "Kooboo.Upgrade.exe");
#else
            UpgradeExeFullName = System.IO.Path.Combine(root,"Upgrade", "Kooboo.Upgrade.exe");
#endif
        }

        public static bool IsRunning { get; set; } = false;
        private static object _locker = new object();

        private static int LastCheckDay { get; set; }

        public static bool IsAutoUpgrade
        {
            get
            {
                var autoUpgradePath = GetAutoUpgradeSettingPath();
                if (!File.Exists(autoUpgradePath))
                {
                    return false;
                }

                var autoStart = false;
                var result = File.ReadAllText(autoUpgradePath);
                bool.TryParse(result, out autoStart);

                return autoStart;
            } 
        }

        public static void SetAutoUpgrade(bool auto)
        {
            var autoUpgradePath = GetAutoUpgradeSettingPath();
            var dir = System.IO.Path.GetDirectoryName(autoUpgradePath);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            File.WriteAllText(autoUpgradePath, auto.ToString());
        }

        private static string GetAutoUpgradeSettingPath()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var dir = System.IO.Path.GetDirectoryName(assembly.Location);
            dir = System.IO.Path.Combine(dir, "_Admin");
            var autoStartPath = System.IO.Path.Combine(dir, "AutoUpgrade.txt");

            return autoStartPath;
        }
        private static string KoobooZipFullName { get; set; }

        public static string UpgradeExeFullName { get; set; }

        public static async Task<Version> GetLatestVersion()
        {
            var version = new Version("0.0.0.0");
            try
            {
                string serverUrl = AppSettings.ConvertApiUrl + "/_api/converter/GetLatestVersion";
                var versionStr = await HttpHelper.GetAsync<string>(serverUrl);
                if (!string.IsNullOrWhiteSpace(versionStr))
                {
                    version = new Version(versionStr);
                }
            }
            catch (Exception ex)
            { 
            }
            return version;
        }
        private static async Task<byte[]> DownloadKoobooZip(System.Net.DownloadProgressChangedEventHandler downloadProgressChanged)
        {
            string serverUrl = AppSettings.ConvertApiUrl + "/_api/converter/DownloadUpgradePackage";
            var client = new System.Net.WebClient();
            if(downloadProgressChanged!=null)
                client.DownloadProgressChanged += downloadProgressChanged;

            Uri uri = new Uri(serverUrl);
            var bytes= await client.DownloadDataTaskAsync(uri);
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
            return null;
        }

        public static async Task Upgrade(System.Net.DownloadProgressChangedEventHandler downloadProgressChanged=null)
        {
            if (LastCheckDay == DateTime.Now.DayOfYear || IsRunning)
            {
                return;
            }

            bool hasupgrade = false; 

            Monitor.Enter(_locker);

            if (LastCheckDay == DateTime.Now.DayOfYear || IsRunning)
            {
                return;
            }

            if (!IsRunning)
            {
                IsRunning = true;
                LastCheckDay = DateTime.Now.DayOfYear; 
                try
                {
                    var newVersion = await GetLatestVersion();

                    if (newVersion > AppSettings.Version)
                    {  
                        var package = await DownloadKoobooZip(downloadProgressChanged);
                        if (package != null)
                        {
                            hasupgrade = true;

                            Lib.Helper.IOHelper.EnsureFileDirectoryExists(KoobooZipFullName); 
                            System.IO.File.WriteAllBytes(KoobooZipFullName, package);
                             
                            using (var archive = ZipFile.Open(KoobooZipFullName, ZipArchiveMode.Read))
                            {
                                foreach (var entry in archive.Entries)
                                {
                                    if (entry.FullName.IndexOf("Kooboo.Upgrade.exe", StringComparison.OrdinalIgnoreCase) > -1)
                                    {
                                        entry.ExtractToFile(UpgradeExeFullName, true);
                                        break;
                                    }
                                }
                            }

                        }

                    }

                }
                catch (Exception ex)
                {
                }
            }

            IsRunning = false;

            Monitor.Exit(_locker);

            if (hasupgrade)
            { 
                Process.Start(UpgradeExeFullName);
            }

        }

    }

}
