using Kooboo.Lib.CrossPlatform;
using Kooboo.Lib.Helper;
using Kooboo.Lib.VirtualFile.Zip;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace VirtualFile.Zip
{
    public static class VirtualResourcesZipExtensions
    {
        static ConcurrentDictionary<string, Ionic.Zip.ZipFile> _zipArchives = new ConcurrentDictionary<string, Ionic.Zip.ZipFile>();

        public static void LoadZip(this VirtualResources virtualResources, string zipPath, string rootPath, ZipOption zipOption)
        {
            if (!File.Exists(zipPath)) throw new FileNotFoundException();
            var file = File.OpenRead(zipPath);
            var zipArchive = Ionic.Zip.ZipFile.Read(file);
            zipPath = Helper.NormalizePath(zipPath);
            _zipArchives[zipPath] = zipArchive;

            var dir = GetZipVirtualPath(zipPath);
            var fileMaps = GetFileMaps(zipArchive);

            foreach (var item in zipArchive.Entries)
            {
                var path = Path.Combine(dir, item.FileName);
                path = Helper.NormalizePath(path);

                if (item.IsDirectory)
                {
                    var virtualDirectory = new VirtualDirectory(path, "zip");
                    virtualResources._entries[path] = virtualDirectory;
                }
                else
                {
                    var fileMap = fileMaps.FirstOrDefault(f => f.To == item.FileName);

                    if (fileMap != null)
                    {
                        var fromPath = Path.Combine(rootPath, fileMap.From);
                        var fileMapFrom = Helper.NormalizePath(fromPath);
                        var virtualFile = new ZipFile(item, fileMapFrom, zipPath, zipOption);
                        virtualResources._fileMaps[fileMapFrom] = virtualFile;
                    }

                    virtualResources._entries[path] = new ZipFile(item, path, zipPath, zipOption); ;
                }
            }
        }

        private static string GetZipVirtualPath(string zipPath)
        {
            var fullPath = Path.GetFullPath(zipPath);
            return Path.Combine(Path.GetDirectoryName(fullPath), Path.GetFileNameWithoutExtension(fullPath));
        }

        public static void UnloadZip(this VirtualResources virtualResources, string zipPath)
        {
            zipPath = Helper.NormalizePath(zipPath);
            var dir = GetZipVirtualPath(zipPath);
            dir = Helper.NormalizePath(dir);
            RemoveEntries(virtualResources, dir);
            RemoveFileMaps(virtualResources, zipPath);

            if (_zipArchives.TryGetValue(zipPath, out var zipArchive))
            {
                zipArchive.Dispose();
                _zipArchives.TryRemove(zipPath, out _);
            }
        }

        private static void RemoveFileMaps(VirtualResources virtualResources, string zipPath)
        {
            var removed = new List<KeyValuePair<string, VirtualFile>>();

            foreach (var item in virtualResources._fileMaps)
            {
                if (!(item.Value is ZipFile)) continue;
                if ((item.Value as ZipFile).ZipPath == zipPath) removed.Add(item);
            }

            foreach (var item in removed)
            {
                virtualResources._fileMaps.TryRemove(item.Key, out _);
            }
        }

        private static void RemoveEntries(VirtualResources virtualResources, string dir)
        {
            var removed = new List<KeyValuePair<string, IEntry>>();

            foreach (var item in virtualResources._entries)
            {
                if (item.Value.Source != "zip") continue;
                if (item.Value.Path.StartsWith(dir)) removed.Add(item);
            }

            foreach (var item in removed)
            {
                virtualResources._entries.TryRemove(item.Key, out _);
            }
        }

        static FileMapping[] GetFileMaps(Ionic.Zip.ZipFile zipArchive)
        {
            try
            {
                var entry = zipArchive.Entries.FirstOrDefault(f => f.FileName.ToLower() == "config.json");

                if (entry != null)
                {
                    using (var stream = entry.OpenReader())
                    {
                        using (var sr = new StreamReader(stream))
                        {
                            var json = sr.ReadToEnd();
                            var jObject = JsonHelper.DeserializeJObject(json);

                            if (jObject.TryGetValue("mappings", out var obj))
                            {
                                return obj.ToObject<FileMapping[]>();
                            }
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
