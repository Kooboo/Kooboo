using System;
using System.IO;

namespace Kooboo.Lib.Utilities
{
    public static class PathUtility
    {

        readonly static string _webName = "Kooboo.Web";
        readonly static string _moduleName = "SubModule";
        public static string ModulesPath { get; set; } = Path.Combine(AppContext.BaseDirectory, "modules");
        public static string TryRootPath()
        {
            var directoryInfo = new DirectoryInfo(AppContext.BaseDirectory);

            var configPath = AppSettingsUtility.Get("RootPath");
            if (!string.IsNullOrEmpty(configPath))
            {
                if (System.IO.Directory.Exists(configPath))
                {
                    if (IsKoobooDiskRoot(configPath))
                    {
                        return configPath;
                    }
                }
                else
                {
                    configPath = Path.Combine(directoryInfo.FullName, configPath);
                    if (System.IO.Directory.Exists(configPath))

                    {
                        if (IsKoobooDiskRoot(configPath))
                        {
                            return configPath;
                        }
                    }
                }

            }

            while (true)
            {
                if (directoryInfo == null) break;
                if (IsKoobooDiskRoot(directoryInfo.FullName)) return directoryInfo.FullName;
                var webDir = Path.Combine(directoryInfo.FullName, _webName);
                if (IsKoobooDiskRoot(webDir)) return webDir;
                var subModuleWebDir = Path.Combine(directoryInfo.FullName, _moduleName, _webName);
                if (IsKoobooDiskRoot(subModuleWebDir)) return subModuleWebDir;
                var srcSubModuleWebDir = Path.Combine(directoryInfo.FullName, "src", _moduleName, _webName);
                if (IsKoobooDiskRoot(srcSubModuleWebDir)) return srcSubModuleWebDir;
                directoryInfo = directoryInfo.Parent;
            }

            return System.IO.Directory.GetCurrentDirectory();
            // throw new Exception("Can not find Kooboo disk root");
        }

        private static bool IsKoobooDiskRoot(string FullPath)
        {
            string adminDir = Path.Combine(FullPath, "_Admin");
            return Directory.Exists(adminDir);
        }

        public static void ValidatePath(string FullPath)
        {
            if (FullPath.Contains("..\\")) throw new Exception("Path can not contains '..\\'");
            if (FullPath.Contains("../")) throw new Exception("Path can not contains '../'");
        }
    }
}
