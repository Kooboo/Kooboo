using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Kooboo.Lib.Helper;

namespace Kooboo.Lib.Utilities.UAParser
{
    public class GoogleIPList
    {
        public GoogleIPList()
        {
        }

        public int CacheDays = 30;

        private string _filename;
        public string FileName
        {
            get
            {
                if (_filename == null)
                {
                    string BaseFolder = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AppData");
                    if (!Directory.Exists(BaseFolder)) BaseFolder = IOHelper.KoobooAppData;
                    BaseFolder = System.IO.Path.Combine(BaseFolder, "GoogleIPList");

                    Lib.Helper.IOHelper.EnsureDirectoryExists(BaseFolder);

                    string FileName = System.IO.Path.Combine(BaseFolder, "googleiplist.json");

                    _filename = FileName;
                }

                return _filename;
            }
        }

        public IPList ReadAllLines()
        {
            if (!File.Exists(FileName))
                RenewFile();
            else
            {
            }

            IPList result = null;

            if (File.Exists(this.FileName))
            {
                var googleIPListStr = File.ReadAllText(this.FileName);
                result = JsonSerializer.Deserialize<IPList>(googleIPListStr, new JsonSerializerOptions()
                {
                    PropertyNameCaseInsensitive = true
                });

                var info = new FileInfo(FileName);
                if (info.LastWriteTime < DateTime.Now.AddDays(-this.CacheDays))
                    Task.Run(() => RenewFile());
            }



            return result;
        }

        private void RenewFile()
        {
            var bytes = DownloadFromRemote();
            if (bytes != null && bytes.Length > 1000)
                System.IO.File.WriteAllBytes(this.FileName, bytes);
        }

        private static byte[] DownloadFromRemote()
        {
            string url = "https://www.gstatic.com/ipranges/goog.json";
            var bytes = Lib.Helper.DownloadHelper.DownloadFile(url);
            if (bytes == null || bytes.Length < 1000)
            {
                bytes = Lib.Helper.DownloadHelper.DownloadFile(url);
            }
            return bytes;
        }
    }

    public class IPList
    {
        public string SyncToken { get; set; }
        public DateTime CreationTime { get; set; }
        public List<PrefixesItem> Prefixes { get; set; }
    }

    public class PrefixesItem
    {
        public string Ipv4Prefix { get; set; }
        public string Ipv6Prefix { get; set; }

        //Because this attribute only one of these has a value
        public override string ToString() => string.IsNullOrEmpty(Ipv4Prefix) ? Ipv6Prefix : Ipv4Prefix;
    }
}

