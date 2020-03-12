using Kooboo.Lib.CrossPlatform;
using Kooboo.Lib.Helper;
using Kooboo.Lib.VirtualFile.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace VirtualFile.Zip
{
    public static class VirtualResourcesZipExtensions
    {
        public static void LoadZip(this VirtualResources virtualResources, string zipPath, string rootPath, bool cache = false)
        {
            if (!File.Exists(zipPath)) throw new FileNotFoundException();
            zipPath = Helper.NormalizePath(zipPath);
            var file = File.OpenRead(zipPath);
            var zipArchive = new ZipArchive(file);
            var fullPath = Path.GetFullPath(zipPath);
            var dir = Path.Combine(Path.GetDirectoryName(fullPath), Path.GetFileNameWithoutExtension(fullPath));
            var fileMaps = GetFileMaps(zipArchive);

            foreach (var item in zipArchive.Entries)
            {
                var path = Path.Combine(dir, item.FullName);
                path = Helper.NormalizePath(path);

                if (item.Name == string.Empty)
                {
                    var virtualDirectory = new VirtualDirectory(path, "zip");
                    virtualResources._entries[path] = virtualDirectory;
                }
                else
                {
                    var fileMap = fileMaps.FirstOrDefault(f => f.To == item.FullName);

                    if (fileMap != null)
                    {
                        var fromPath = Path.Combine(rootPath, fileMap.From);
                        var fileMapFrom = Helper.NormalizePath(fromPath);
                        var virtualFile = new ZipFile(item, fileMapFrom, zipPath, cache);
                        virtualResources._fileMaps[fileMapFrom] = virtualFile;
                    }
                    else
                    {
                        virtualResources._entries[path] = new ZipFile(item, path, zipPath, cache); ;
                    }
                }
            }
        }

        static FileMapping[] GetFileMaps(ZipArchive zipArchive)
        {
            try
            {
                var entry = zipArchive.Entries.FirstOrDefault(f => f.FullName.ToLower() == "mapping.json");
                if (entry != null)
                {
                    using (var stream = entry.Open())
                    {
                        using (var sr = new StreamReader(stream))
                        {
                            var json = sr.ReadToEnd();
                            return JsonHelper.Deserialize<FileMapping[]>(json);
                        }
                    }
                }
            }
            catch (Exception)
            {
            }

            return new FileMapping[0];
        }
    }
}
