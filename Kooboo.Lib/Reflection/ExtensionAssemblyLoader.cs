using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Linq;
using VirtualFile;
using Kooboo.Lib.Utilities;
using VirtualFile.Zip;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace Kooboo.Lib.Reflection
{
    public class ExtensionAssemblyLoader
    {
        private static readonly List<string> extensionFolders = new List<string>();

        static ExtensionAssemblyLoader()
        {
            var path = AppDomain.CurrentDomain.BaseDirectory;
            extensionFolders.Add(Path.Combine(path, "modules"));
            extensionFolders.Add(Path.Combine(path, "dll"));
            extensionFolders.Add(Path.Combine(path, "packages"));
            LoadModuleZip();

        }
        public static void AddExtensionFolder(string path)
        {
            extensionFolders.Add(path);
        }

        public List<Assembly> Assemblies { get; private set; } = new List<Assembly>();


        private static object _lockObj = new object();
        private static ExtensionAssemblyLoader _instance;

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
            Assemblies = LoadDlls();
        }

        private List<Assembly> LoadDlls()
        {
            var dlls = new List<Assembly>();
            bool isNetFramework = false;  // = RuntimeInformation.FrameworkDescription.StartsWith(".NET Framework");

#if NET461
            {
            isNetFramework = true; 
            }
#endif

            foreach (var folder in extensionFolders)
            {
                if (!Directory.Exists(folder)) continue;

                var allsubdlls = VirtualResources.GetFiles(folder, "*.dll", SearchOption.AllDirectories);

                foreach (var filename in allsubdlls)
                {
#if DEBUG

                    if (isNetFramework && File.Exists("ignoremodules.txt"))
                    {
                        var modules = File.ReadAllText("ignoremodules.txt")
                            .Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                            .Select(s =>
                            {
                                var module = s.ToUpper().Trim();
                                if (module.EndsWith(".ZIP")) module = module.Substring(0, module.Length - 4);
                                return module;
                            });

                        if (modules.Any(a => filename.Contains(a))) continue;
                    }
#endif

                    try
                    {
                        var otherAssembly = Assembly.Load(VirtualResources.ReadAllBytes(filename));

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
                return assemblyName.Name == a.GetName().Name;
            });
            if (assembly != null)
            {
                return assembly;
            }

            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var path = extensionFolders.Union(new[] { baseDirectory }).Select(folder =>
             {
                 var dllpath = Path.Combine(folder, string.Format("{0}.dll", name));
                 if (VirtualResources.FileExists(dllpath)) return dllpath;

                 return string.Empty;
             }).FirstOrDefault(f => f.Length > 0);

            if (!string.IsNullOrEmpty(path))
            {
                assembly = Assembly.Load(VirtualResources.ReadAllBytes(path));
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

        private static void LoadModuleZip()
        {
            var rootPath = PathUtility.TryRootPath();

            VirtualResources.Setup(v =>
            {
                foreach (var item in Directory.GetFiles(Path.Combine(AppContext.BaseDirectory, "modules"), "*.zip"))
                {
                    v.LoadZip(item, rootPath, new Lib.VirtualFile.Zip.ZipOption
                    {
                        Cache = true
                    });
                }
            });
        }
    }
}
