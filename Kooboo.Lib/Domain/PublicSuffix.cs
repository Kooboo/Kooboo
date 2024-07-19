using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Kooboo.Lib.Helper;

namespace Kooboo.Lib.Domain
{
    public class PublicSuffix
    {

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
                    BaseFolder = System.IO.Path.Combine(BaseFolder, "PublicSuffix");

                    Lib.Helper.IOHelper.EnsureDirectoryExists(BaseFolder);

                    string FileName = System.IO.Path.Combine(BaseFolder, "publicsuffix.dat");

                    _filename = FileName;
                }

                return _filename;
            }
        }

        public List<string> ReadAllLines()
        {
            if (!File.Exists(FileName))
            {
                RenewFile();
            }
            else
            {
                var info = new FileInfo(FileName);
                if (info.LastWriteTime < DateTime.Now.AddDays(-this.CacheDays))
                {
                    Task.Run(() => RenewFile());
                }
            }

            if (File.Exists(this.FileName))
            {
                using (StreamReader reader = new StreamReader(this.FileName))
                {
                    List<string> lines = new List<string>();

                    var line = reader.ReadLine();

                    while (line != null)
                    {
                        var item = line.Trim();

                        if (!string.IsNullOrWhiteSpace(item))
                        {
                            if (!item.StartsWith("/"))
                            {
                                lines.Add(item);
                            }
                        }
                        line = reader.ReadLine();
                    }
                    return lines;
                }
            }
            return null;
        }





        public byte[] DownloadFromRemote()
        {
            string url = "https://publicsuffix.org/list/public_suffix_list.dat";
            var bytes = Lib.Helper.DownloadHelper.DownloadFile(url);
            if (bytes == null || bytes.Length < 1000)
            {
                bytes = Lib.Helper.DownloadHelper.DownloadFile(url);
            }
            return bytes;
        }

        public void RenewFile()
        {
            var bytes = DownloadFromRemote();
            if (bytes != null && bytes.Length > 1000)
            {
                System.IO.File.WriteAllBytes(this.FileName, bytes);
            }
        }
    }
}
