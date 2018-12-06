using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Compression;
using System.IO;

namespace Kooboo.Data.Helper
{
    public class DownloadZipHelper
    {
        public static async Task<bool> DownloadZipData(string url,string zipName)
        {
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var zipPath = System.IO.Path.Combine(baseDirectory, zipName);
            var success=false;
            try
            {
                success=await DownloadZip(url, zipPath);
                if (success)
                {
                    Unzip(baseDirectory, zipPath);
                    File.Delete(zipPath);
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                success = false;
            }

            return success;
        }

        private static async Task<bool> DownloadZip(string url, string savePath)
        {
            var client = new System.Net.WebClient();

            Uri uri = new Uri(url);
            var bytes = await client.DownloadDataTaskAsync(uri);
            if (bytes != null)
            {
                Lib.Helper.IOHelper.EnsureFileDirectoryExists(savePath);
                if (System.IO.File.Exists(savePath))
                    System.IO.File.Delete(savePath);
                if (bytes.Length > 0)
                    System.IO.File.WriteAllBytes(savePath, bytes);
                return true;
            }
            return false;
        }
        private static void Unzip(string basePath, string zipPath)
        {
            using (var archive = ZipFile.Open(zipPath, ZipArchiveMode.Read))
            {
                if (archive.Entries.Count > 0)
                {
                    foreach (var entry in archive.Entries)
                    {
                        var path = Path.Combine(basePath, entry.FullName);
                        Kooboo.Lib.Helper.IOHelper.EnsureFileDirectoryExists(path);
                        try
                        {
                            entry.ExtractToFile(path, true);
                        }
                        catch (Exception ex)
                        {
                        }
                    }
                }
            }
        }
    }
}
