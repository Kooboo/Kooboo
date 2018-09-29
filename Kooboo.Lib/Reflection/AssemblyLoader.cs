using System;
using System.Collections.Generic;
using System.Reflection;
using System.IO;

namespace Kooboo.Lib.Reflection
{
    public static class AssemblyLoader
    { 
        static AssemblyLoader()
        {
            AllAssemblies = new List<Assembly>();

            var allassembs = AppDomain.CurrentDomain.GetAssemblies();

            foreach (var item in allassembs)
            {
                if (!item.GlobalAssemblyCache)
                {
                    if (!IsIgnoredName(item.FullName))
                    {
                        AllAssemblies.Add(item);
                    }
                }
            }
            var otherAssemblies = LoadOtherAssemblies();

            AllAssemblies.AddRange(otherAssemblies);

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
            if (lower.StartsWith("mscorlib") || lower.StartsWith("microsoft") || lower.StartsWith("kooboo.dom") || lower.StartsWith("kooboo.indexeddb") || lower.StartsWith("newtonsoft") || lower.StartsWith("vshost"))
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

    }
}
