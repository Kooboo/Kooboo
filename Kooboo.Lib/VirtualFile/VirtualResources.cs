using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace VirtualFile
{
    public class VirtualResources
    {
        static readonly VirtualResources _instance = new Lazy<VirtualResources>(() => new VirtualResources(), true).Value;
        internal readonly ConcurrentDictionary<string, IEntry> _entries = new ConcurrentDictionary<string, IEntry>();
        internal readonly ConcurrentDictionary<string, VirtualFile> _fileMaps = new ConcurrentDictionary<string, VirtualFile>();

        public bool IncludePhysical { get; set; } = true;

        public static ConcurrentDictionary<string, IEntry> Entries => _instance._entries;
        public static ConcurrentDictionary<string, VirtualFile> Mappings => _instance._fileMaps;

        public static void Setup(Action<VirtualResources> action)
        {
            action(_instance);
        }

        public static bool FileExists(string path)
        {
            var normalPath = Helper.NormalizePath(path);

            if (_instance._fileMaps.TryGetValue(normalPath, out var _))
            {
                return true;
            }

            if (_instance.IncludePhysical && File.Exists(path)) return true;
            return _instance._entries.TryGetValue(normalPath, out var entry) && entry is VirtualFile;
        }

        public static bool DirectoryExists(string path)
        {

            if (_instance.IncludePhysical && Directory.Exists(path)) return true;

            var normalPath = Helper.NormalizePath(path);
            return _instance._entries.TryGetValue(normalPath, out var entry) && entry is VirtualDirectory;
        }

        public static byte[] ReadAllBytes(string path)
        {
            var normalPath = Helper.NormalizePath(path);

            if (_instance._fileMaps.TryGetValue(normalPath, out var mapFile))
            {
                return mapFile.ReadAllBytes();
            }

            if (_instance.IncludePhysical && File.Exists(path))
            {
                return File.ReadAllBytes(path);
            }

            if (!_instance._entries.TryGetValue(normalPath, out var entry) || entry is VirtualDirectory)
            {
                throw new FileNotFoundException();
            }

            return (entry as VirtualFile).ReadAllBytes();
        }

        public Stream Open(string path, FileMode fileMode = default(FileMode))
        {
            var normalPath = Helper.NormalizePath(path);

            if (_instance._fileMaps.TryGetValue(normalPath, out var mapFile))
            {
                return mapFile.Open();
            }

            if (IncludePhysical && File.Exists(path))
            {
                return File.Open(path, fileMode);
            }

            if (!_instance._entries.TryGetValue(normalPath, out var entry) || entry is VirtualDirectory)
            {
                throw new FileNotFoundException();
            }

            return (entry as VirtualFile).Open();
        }

        public static string ReadAllText(string path) => ReadAllText(path, Encoding.UTF8);

        public static string ReadAllText(string path, Encoding encoding)
        {
            var normalPath = Helper.NormalizePath(path);

            if (_instance._fileMaps.TryGetValue(normalPath, out var mapFile))
            {
                return mapFile.ReadAllText(encoding);
            }

            if (_instance.IncludePhysical && File.Exists(path))
            {
                return File.ReadAllText(path);
            }

            if (!_instance._entries.TryGetValue(normalPath, out var entry) || entry is VirtualDirectory)
            {
                throw new FileNotFoundException();
            }

            return (entry as VirtualFile).ReadAllText(encoding);
        }

        public static string[] GetFiles(string path)
        {
            var paths = new List<string>();
            var normalPath = Helper.NormalizePath(path);

            var virtualMapPaths = _instance._fileMaps
                .Where(w => w.Value.Directory == normalPath)
                .Select(s => s.Value.Path)
                .ToArray();

            paths.AddRange(virtualMapPaths);

            if (_instance.IncludePhysical && Directory.Exists(path))
            {
                var physicalPaths = Directory.GetFiles(path);

                foreach (var item in physicalPaths)
                {
                    if (paths.Contains(Helper.NormalizePath(item))) continue;
                    paths.Add(item);
                }
            }

            var virtualPaths = _instance._entries
                .Where(w => w.Value.Directory == normalPath && w.Value is VirtualFile)
                .Select(s => s.Value.Path)
                .ToArray();

            paths.AddRange(virtualPaths);

            return paths.ToArray();
        }

        public static string[] GetFiles(string path, string searchPattern)
        {
            var paths = new List<string>();
            var normalPath = Helper.NormalizePath(path);
            var reg = Helper.GetWildcardRegexString(searchPattern);

            var virtualMapPaths = _instance._fileMaps
                .Where(w => w.Value.Directory == normalPath && Regex.IsMatch(w.Key, reg, RegexOptions.IgnoreCase))
                .Select(s => s.Value.Path)
                .ToArray();

            paths.AddRange(virtualMapPaths);

            if (_instance.IncludePhysical && Directory.Exists(path))
            {
                var physicalPaths = Directory.GetFiles(path, searchPattern);

                foreach (var item in physicalPaths)
                {
                    if (paths.Contains(Helper.NormalizePath(item))) continue;
                    paths.Add(item);
                }
            }

            var virtualPaths = _instance._entries
                .Where(w => w.Value.Directory == normalPath && Regex.IsMatch(w.Key, reg, RegexOptions.IgnoreCase) && w.Value is VirtualFile)
                .Select(s => s.Value.Path)
                .ToArray();

            paths.AddRange(virtualPaths);
            return paths.ToArray();
        }

        public static string[] GetFiles(string path, string searchPattern, SearchOption searchOption)
        {
            var paths = new List<string>();
            var normalPath = Helper.NormalizePath(path);
            var reg = Helper.GetWildcardRegexString(searchPattern);

            IEnumerable<KeyValuePair<string, VirtualFile>> fileMaps = _instance._fileMaps;

            if (searchOption == SearchOption.TopDirectoryOnly)
            {
                fileMaps = fileMaps.Where(w => w.Value.Directory == normalPath);
            }
            else
            {
                fileMaps = fileMaps.Where(w => w.Key.StartsWith(normalPath));
            }

            var virtualMapPaths = fileMaps
                .Where(w => Regex.IsMatch(w.Key, reg, RegexOptions.IgnoreCase))
                .Select(s => s.Value.Path)
                .ToArray();

            paths.AddRange(virtualMapPaths);

            if (_instance.IncludePhysical && Directory.Exists(path))
            {
                var physicalPaths = Directory.GetFiles(path, searchPattern, searchOption);

                foreach (var item in physicalPaths)
                {
                    if (paths.Contains(Helper.NormalizePath(item))) continue;
                    paths.Add(item);
                }
            }

            IEnumerable<KeyValuePair<string, IEntry>> entries = _instance._entries;

            if (searchOption == SearchOption.TopDirectoryOnly)
            {
                entries = entries.Where(w => w.Value.Directory == normalPath);
            }
            else
            {
                entries = entries.Where(w => w.Key.StartsWith(normalPath));
            }

            var virtualPaths = entries
                .Where(w => Regex.IsMatch(w.Key, reg, RegexOptions.IgnoreCase) && w.Value is VirtualFile)
                .Select(s => s.Value.Path)
                .ToArray();

            paths.AddRange(virtualPaths);
            return paths.ToArray();
        }


        public static string[] GetDirectories(string path)
        {
            var paths = new List<string>();

            if (_instance.IncludePhysical && Directory.Exists(path))
            {
                paths.AddRange(Directory.GetDirectories(path));
            }

            var normalPath = Helper.NormalizePath(path);

            var virtualPaths = _instance._entries
                    .Where(w => w.Value.Directory == normalPath && w.Value is VirtualDirectory)
                    .Select(s => s.Value.Path)
                    .ToArray();

            paths.AddRange(virtualPaths);
            return paths.ToArray();
        }
    }
}
