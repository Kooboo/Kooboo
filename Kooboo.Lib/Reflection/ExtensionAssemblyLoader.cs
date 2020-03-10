using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Linq;

namespace Kooboo.Lib.Reflection
{
    public class ExtensionAssemblyLoader
    {
        private static List<string> extensionFolders = new List<string> { "modules", "dll", "packages" };

        public List<Assembly> Assemblies { get; private set; } = new List<Assembly>();

        private static List<FileSystemWatcher> watchers = new List<FileSystemWatcher>();

        private static object _lockObj = new object();
        private static ExtensionAssemblyLoader _instance;

        public static Action AssemblyChangeAction;
        public static ExtensionAssemblyLoader Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lockObj)
                    {
                        if (_instance == null)
                            _instance = new ExtensionAssemblyLoader();
                    }
                }

                return _instance;
            }
        }

        private ExtensionAssemblyLoader()
        {
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
            foreach (var folder in extensionFolders)
            {
                var dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, folder);
                if (!Directory.Exists(dir)) continue;



                var watcher = new FileSystemWatcher(dir);
                watcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite;
                watcher.Changed += new FileSystemEventHandler(OnFileChanged);
                watcher.Created += new FileSystemEventHandler(OnFileChanged);
                watcher.Deleted += new FileSystemEventHandler(OnFileChanged);
                watcher.EnableRaisingEvents = true;
                watchers.Add(watcher);
            }
            Assemblies = LoadDlls();
        }

        public Type LoadTypeByName(string name)
        {
            foreach (var item in Assemblies)
            {
                var type = item.GetTypes().ToList().Find(i => i.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
                if (type != null)
                    return type;
            }

            return null;
        }

        public void OnFileChanged(object sender, FileSystemEventArgs e)
        {
            Assembly assembly = null;
            if (File.Exists(e.FullPath))
            {
                try
                {
                    //file can be replaced。
                    //Now can't unload assembly from current domain,so we still need to restart server
                    assembly = Assembly.Load(File.ReadAllBytes(e.FullPath));
                }
                catch (Exception ex)
                {

                }

            }

            lock (_lockObj)
            {
                Assemblies.RemoveAll(a =>
                {
                    if (assembly == null)
                    {
                        var assemblyName = a.GetName();
                        return e.Name.StartsWith(assemblyName.Name);
                    }
                    return a.FullName == assembly.FullName;
                });

                if (assembly != null && (e.ChangeType == WatcherChangeTypes.Changed || e.ChangeType == WatcherChangeTypes.Created))
                {
                    Assemblies.Add(assembly);
                }

                AssemblyChangeAction?.Invoke();

            }

        }

        private List<Assembly> LoadDlls()
        {
            var dlls = new List<Assembly>();
            var path = AppDomain.CurrentDomain.BaseDirectory;

            foreach (var item in extensionFolders)
            {
                string folder = Path.Combine(path, item);
                if (!Directory.Exists(folder)) continue;

                var allsubdlls = Directory.GetFiles(folder, "*.dll", SearchOption.AllDirectories);

                foreach (var filename in allsubdlls)
                {
                    try
                    {
                        var otherAssembly = Assembly.Load(File.ReadAllBytes(filename));

                        if (otherAssembly != null)
                        {
                            dlls.Add(otherAssembly);
                        }
                    }
                    catch (Exception)
                    {

                    }
                }
            }
            return dlls;
        }

        /// <summary>
        /// load dll which is in root path,and not kooboo dll
        /// </summary>
        /// <param name="extensiondlls"></param>
        public void LoadSpecificDlls(string extensiondlls)
        {
            var path = AppDomain.CurrentDomain.BaseDirectory;

            if (!string.IsNullOrEmpty(extensiondlls))
            {
                var extensionList = extensiondlls.Split(',').Distinct().ToList();
                foreach (var dll in extensionList)
                {
                    try
                    {
                        var filename = dll.EndsWith(".dll", StringComparison.OrdinalIgnoreCase)
                            ? dll
                            : string.Format("{0}.dll", dll);

                        var filepath = Path.Combine(path, filename);
                        if (File.Exists(filepath))
                        {
                            var assembly = Assembly.Load(File.ReadAllBytes(filepath));

                            if (assembly != null && !Assemblies.Exists(ass => ass.FullName.Equals(assembly.FullName)))
                            {
                                Assemblies.Add(assembly);
                            }
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
        }

        public Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            var assemblyName = new AssemblyName(args.Name);
            var name = assemblyName.Name;

            var assembly = Assemblies.Find(a =>
            {
                return assemblyName.FullName == a.FullName;
            });
            if (assembly != null)
            {
                return assembly;
            }

            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var path = extensionFolders.Select(folder =>
            {
                var dllpath = Path.Combine(baseDirectory, folder, string.Format("{0}.dll", name));
                if (File.Exists(dllpath)) return dllpath;

                return string.Empty;
            }).FirstOrDefault();

            if (!string.IsNullOrEmpty(path))
            {
                assembly = Assembly.Load(File.ReadAllBytes(path));
                lock (_lockObj)
                {
                    if (!Assemblies.Exists(a => a.FullName == assemblyName.FullName))
                    {
                        Assemblies.Add(assembly);
                    }
                }
                return assembly;
            }

            return null;
        }
    }

}
