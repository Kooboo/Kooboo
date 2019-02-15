using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Render.Controller
{
    //TODO: to be refactor....  
    public static class ModuleFile
    {
        static ModuleFile()
        {
            ModuleRoot = System.IO.Path.Combine(Data.AppSettings.RootPath, "modules");
        }

        public static string ModuleRoot { get; set; }

        public static string AdminPath { get; set; } = "_admin";

        public static char[] seps { get; set; } = "/\\".ToCharArray();

        public static string FindFile(string FullFilePath)
        {
            var root = Data.AppSettings.RootPath;
            if (FullFilePath.StartsWith(root))
            {
                string relative = FullFilePath.Substring(root.Length);
                if (string.IsNullOrWhiteSpace(relative))
                {
                    return null; 
                }

                if (relative.StartsWith("/") || relative.StartsWith("\\"))
                {
                    relative = relative.Substring(1);
                }

                if (string.IsNullOrWhiteSpace(relative))
                {
                    return null; 
                }

                if (relative.ToLower().StartsWith(AdminPath))
                {
                    relative = relative.Substring(AdminPath.Length);
                }

                if (string.IsNullOrWhiteSpace(relative))
                {
                    return null; 
                }

                var paths = relative.Split(seps, StringSplitOptions.RemoveEmptyEntries).ToList();
                paths.Insert(0, ModuleRoot);  

                var fullpath = System.IO.Path.Combine(paths.ToArray());
                 
                if (System.IO.File.Exists(fullpath))
                {
                    return fullpath; 
                }

            }

            return null; 
        }
    }
}
