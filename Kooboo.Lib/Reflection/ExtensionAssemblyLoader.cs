using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Linq;
using VirtualFile;

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
            Assemblies = LoadDlls();
        }

        private List<Assembly> LoadDlls()
        {
            var dlls = new List<Assembly>();

            foreach (var folder in extensionFolders)
            {
                if (!Directory.Exists(folder)) continue;

                var allsubdlls = VirtualResources.GetFiles(folder, "*.dll", SearchOption.AllDirectories);

                foreach (var filename in allsubdlls)
                {
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
    }
}
