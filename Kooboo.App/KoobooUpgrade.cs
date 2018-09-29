using System;
using Kooboo.Lib.Helper;
using System.IO.Compression;
using Kooboo.Data;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Linq;
using System.Threading;

namespace Kooboo.App
{
    public class KoobooUpgrade
    {
        static KoobooUpgrade()
        {
            string root = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

            string path = System.IO.Path.Combine(root, "upgradePackage");
            KoobooZipFullName = System.IO.Path.Combine(path, "Kooboo.zip");
            UpgradeExeFullName = System.IO.Path.Combine(root, "Kooboo.Upgrade.exe");
        }

        public static bool IsRunning { get; set; } = false;
        private static object _locker = new object();

        private static int LastCheckDay { get; set; }

        public static bool IsAutoUpgrade
        {
            get
            { 
                return Data.AppSettings.GetBool("AutoUpgrade");
            } 
        }

        public static void SetAutoUpgrade(bool auto)
        {
            AppSettings.SetConfigValue("AutoUpgrade", auto.ToString());
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
