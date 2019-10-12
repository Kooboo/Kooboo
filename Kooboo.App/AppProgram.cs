//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Kooboo.App
{
    public class AppProgram
    {
        [STAThreadAttribute]
        public static void Main()
        {  
            var assemblies = new Dictionary<string, Assembly>();
            var executingAssembly = Assembly.GetExecutingAssembly();
            var resources = executingAssembly.GetManifestResourceNames().Where(n => n.EndsWith(".dll"));

            foreach (string resource in resources)
            {
                using (var stream = executingAssembly.GetManifestResourceStream(resource))
                {
                    if (stream == null)
                        continue;

                    var bytes = new byte[stream.Length];
                    stream.Read(bytes, 0, bytes.Length);
                    try
                    {
                        assemblies.Add(resource, Assembly.Load(bytes));
                    }
                    catch (Exception)
                    {
                        // ignored
                    }
                }
            } 
          

            AppDomain.CurrentDomain.AssemblyResolve += (s, ev) =>
            {
                var assemblyName = new AssemblyName(ev.Name);

                var path = $"{assemblyName.Name}.dll";

                if (assemblies.ContainsKey(path))
                {
                    return assemblies[path];
                } 
                return null;
            };
            App.Main();
        }
    }
}
