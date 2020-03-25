using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using Kooboo.Lib.Helper;
using Kooboo.Lib.VirtualFile.Zip;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace VirtualFile.Zip
{
    public static class VirtualResourcesZipExtensions
    {
        static ConcurrentDictionary<string, ICSharpCode.SharpZipLib.Zip.ZipFile> _zipArchives = new ConcurrentDictionary<string, ICSharpCode.SharpZipLib.Zip.ZipFile>();

        public static void LoadZip(this VirtualResources virtualResources, string zipPath, string rootPath, ZipOption zipOption)
        {
            if (!File.Exists(zipPath)) throw new FileNotFoundException();
            var zipArchive = new ICSharpCode.SharpZipLib.Zip.ZipFile(zipPath);
            zipPath = Helper.NormalizePath(zipPath);
            _zipArchives[zipPath] = zipArchive;

            var dir = GetZipVirtualPath(zipPath);
            var fileMaps = GetFileMaps(zipArchive);

            foreach (ZipEntry item in zipArchive)
            {
                var path = Path.Combine(dir, item.Name);
                path = Helper.NormalizePath(path);

                if (item.IsDirectory)
                {
                    var virtualDirectory = new VirtualDirectory(path, "zip");
                    virtualResources._entries[path] = virtualDirectory;
                }
                else
                {
                    var fileMap = fileMaps.FirstOrDefault(f => f.To == item.Name);

                    if (fileMap != null)
                    {
                        fileMap.From = fileMap.From.Trim();
                        if (fileMap.From.StartsWith("/")) fileMap.From = fileMap.From.Substring(1);
                        var fromPath = Path.Combine(rootPath, fileMap.From);
                        var fileMapFrom = Helper.NormalizePath(fromPath);
                        var virtualFile = new ZipFile(item, zipArchive, fileMapFrom, zipPath, zipOption);
                        virtualResources._fileMaps[fileMapFrom] = virtualFile;
                    }

                    virtualResources._entries[path] = new ZipFile(item, zipArchive, path, zipPath, zipOption);

                    while (true)
                    {
                        if (path == dir) break;
                        path = Path.GetDirectoryName(path);
                        if (virtualResources._entries.Where(w => w.Value is VirtualDirectory).All(a => a.Key != path))
                        {
                            virtualResources._entries[path] = new VirtualDirectory(path, "zip");
                        }
                    }
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
                zipArchive.Close();
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

        static FileMapping[] GetFileMaps(ICSharpCode.SharpZipLib.Zip.ZipFile zipArchive)
        {
            try
            {
                var entry = zipArchive.Cast<ZipEntry>().FirstOrDefault(f => f.Name.ToLower() == "config.json");

                if (entry != null)
                {
                    var buffer = new byte[entry.Size];

                    using (var stream = zipArchive.GetInputStream(entry))
                    {
                        StreamUtils.ReadFully(stream, buffer);
                    }

                    var json = Encoding.UTF8.GetString(buffer);
                    var jObject = JsonHelper.DeserializeJObject(json);

                    if (jObject.TryGetValue("mappings", out var obj))
                    {
                        return obj.ToObject<FileMapping[]>();
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
