//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using VirtualFile;

namespace Kooboo.Render.Controller
{
    //TODO: to be refactor....  
    public static class ModuleFile
    {
        static ModuleFile()
        {
            AddModuleRoot(Path.Combine(Data.AppSettings.RootPath, "view"));
            AddModuleRoot(Path.Combine(Data.AppSettings.RootPath, "modules"));
            AddModuleRoot(Path.Combine(AppContext.BaseDirectory, "modules"));
        }

        private static void AddModuleRoot(string path)
        {
            if (Directory.Exists(path)) ModuleRoots.Add(path);
            foreach (var item in VirtualResources.GetDirectories(path))
            {
                ModuleRoots.Add(item);
            }
        }

        public static List<string> ModuleRoots { get; set; } = new List<string>();

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
                foreach (var moduleRoot in ModuleRoots)
                {
                    paths.Insert(0, moduleRoot);

                    var fullpath = Path.Combine(paths.ToArray());

                    if (VirtualResources.FileExists(fullpath))
                    {
                        return fullpath;
                    }
                    paths.RemoveAt(0);
                }
            }

            return null;
        }
    }
}
