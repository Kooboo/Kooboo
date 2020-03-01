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
        [STAThread]
        public static void Main()
        {
#if !DEBUG
            var dir = new System.IO.DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
            var files = dir.GetFiles("Kooboo.*.dll", System.IO.SearchOption.TopDirectoryOnly);
            var isDebugMode = files.Length > 0;

            if (!isDebugMode)
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
                        catch (Exception ex)
                        {

                        }
                    }
                }

                AppDomain.CurrentDomain.AssemblyResolve += (s, ev) =>
                {
                    var assemblyName = new AssemblyName(ev.Name);

                    var path = string.Format("{0}.dll", assemblyName.Name);

                    if (assemblies.ContainsKey(path))
                    {
                        return assemblies[path];
                    }
                    return null;
                };
            }

#endif
            App.Main();
        }
    }
}
