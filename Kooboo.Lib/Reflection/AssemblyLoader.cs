//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace Kooboo.Lib.Reflection
{
    public static class AssemblyLoader
    {
        static AssemblyLoader()
        {
            AllAssemblies = LoadAllDlls();
        }

        private static List<Assembly> LoadAllDlls()
        {
            var dlls = new List<Assembly>();

            var allassembs = AppDomain.CurrentDomain.GetAssemblies();

            foreach (var item in allassembs)
            {
                if (!item.GlobalAssemblyCache)
                {
                    if (!IsIgnoredName(item.FullName))
                    {
                        dlls.Add(item);
                    }
                }
            }

            var path =  AppDomain.CurrentDomain.BaseDirectory;
            dlls = LoadKoobooDlls(dlls, path);
            // load dll from modules or dll. 
            List<string> subfolders = new List<string>();
            subfolders.Add("dll");
            subfolders.Add("modules");
            subfolders.Add("packages"); 

            foreach (var item in subfolders)
            {
                string folder = System.IO.Path.Combine(path, item); 
                if (System.IO.Directory.Exists(folder))
                {
                    var allsubdlls = System.IO.Directory.GetFiles(folder, "*.dll", SearchOption.AllDirectories);

                    foreach (var filename in allsubdlls)
                    {
                        try
                        {
                            var otherAssembly = Assembly.LoadFile(filename);
                            if (otherAssembly !=null)
                            {
                                dlls.Add(otherAssembly);
                            } 
                        }
                        catch (Exception ex)
                        {
 
                        } 
                    } 

                }
            } 
            return dlls; 
        }

        public static List<Assembly> LoadExtensionDll(string extensiondlls)
        {
            var path = AppDomain.CurrentDomain.BaseDirectory;

            if (!string.IsNullOrEmpty(extensiondlls))
            {
                var extensionList= extensiondlls.Split(',').ToList().Distinct().ToList();
                foreach (var dll in extensionList)
                {
                    try
                    {
                        var filename = dll.EndsWith(".dll", StringComparison.OrdinalIgnoreCase)
                            ? dll
                            : string.Format("{0}.dll", dll);

                        var filepath = Path.Combine(path, filename);
                        if(File.Exists(filepath))
                        {
                            var assembly = Assembly.LoadFile(filepath);
                            if (assembly != null && !AllAssemblies.Exists(ass=>ass.FullName.Equals(assembly.FullName)))
                            {
                                AllAssemblies.Add(assembly);
                            }
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
            return AllAssemblies;
        }

        public static List<Assembly> LoadKoobooDlls(List<Assembly> dlls,string path)
        {
            if (dlls == null)
            {
                dlls = new List<Assembly>();
            }
            var alldlls = System.IO.Directory.GetFiles(path, "*.dll", SearchOption.TopDirectoryOnly);

            foreach (var name in alldlls)
            {
                string dllname = name.Substring(path.Length);

                if (string.IsNullOrWhiteSpace(dllname))
                {
                    continue;
                }

                dllname = dllname.Trim('\\').Trim('/');

                if (dllname.StartsWith("Kooboo.") && dllname.EndsWith(".dll"))
                {
                    var index = dllname.LastIndexOf(".");
                    if (index > -1)
                    {
                        string koobooname = dllname.Substring(0, index);
                        var find = dlls.Find(o => o.FullName.StartsWith(koobooname));
                        if (find == null && !IsIgnoredName(koobooname))
                        {
                            var otherAssembly = Assembly.LoadFile(name);
                            dlls.Add(otherAssembly);
                        }
                    }
                }
            }
            return dlls;
        }

        public static List<Assembly> AllAssemblies
        {
            get; set;
        }

        private static List<Assembly> LoadOtherAssemblies()
        {
            List<Assembly> otherAssemblies = new List<Assembly>();
            List<string> otherAssembliesName = new List<string>();
            otherAssembliesName.Add("Kooboo.Account.dll");
            otherAssembliesName.Add("Kooboo.Converter.dll");
            otherAssembliesName.Add("Kooboo.Engines.dll");
            foreach (var assemblyname in otherAssembliesName)
            {
                var path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, assemblyname);
                if (File.Exists(path))
                {
                    var otherAssembly = Assembly.LoadFile(path);
                    otherAssemblies.Add(otherAssembly);
                }
            }

            return otherAssemblies;
        }

        private static bool IsIgnoredName(string FullName)
        {
            if (string.IsNullOrEmpty(FullName))
            {
                return true;
            }

            string lower = FullName.ToLower();
            if (lower.StartsWith("mscorlib") || lower.StartsWith("microsoft") || lower.StartsWith("anonymously") || lower.StartsWith("kooboo.dom") || lower.StartsWith("kooboo.indexeddb") || lower.StartsWith("newtonsoft") || lower.StartsWith("vshost") || lower.StartsWith("kooboo.httpserver"))
            {
                return true;
            }

            return false;

        }

        public static List<Type> LoadTypeByInterface(Type interfaceType)
        {
            List<Type> typelist = new List<Type>();

            foreach (var item in AllAssemblies)
            {
                foreach (var type in item.GetTypes())
                {
                    if (!type.IsAbstract && !type.IsInterface && !type.IsGenericType && interfaceType.IsAssignableFrom(type))
                    {
                        typelist.Add(type);
                    }
                }

            }

            return typelist;
        }
 
        public static List<Type> LoadTypeByGenericInterface(Type GenericInterface)
        {
            List<Type> typelist = new List<Type>();

            foreach (var item in AllAssemblies)
            {
                foreach (var type in item.GetTypes())
                {
                    if (!type.IsAbstract && !type.IsInterface && !type.IsGenericType && Lib.Reflection.TypeHelper.HasGenericInterface(type, GenericInterface))
                    {
                        typelist.Add(type);
                    }
                }
            }

            return typelist;
        }

        public static Type LoadTypeByFullClassName(string fullClassName)
        {
            foreach (var item in AllAssemblies)
            {
                var type = item.GetType(fullClassName);
                if (type != null)
                {
                    return type;
                }
            }

            return null;
        }

    }
}
