using Kooboo.Lib.Reflection;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using VirtualFile;

namespace Kooboo.Data.Helper
{
    public static class ModuleHelper
    {
        static string _koobooWebModulePath = Path.Combine(AppSettings.RootPath, "modules");
        static bool _needLoadKoobooWebModules =
            (VirtualFile.Helper.NormalizePath(_koobooWebModulePath) != VirtualFile.Helper.NormalizePath(AppSettings.ModulePath))
            && Directory.Exists(_koobooWebModulePath);

        static ModuleHelper()
        {
            if (_needLoadKoobooWebModules)
            {
                ExtensionAssemblyLoader.AddExtensionFolder(_koobooWebModulePath);
            }
        }

        public static string[] GetConfigs()
        {
            var configs = VirtualResources.GetFiles(AppSettings.ModulePath, "*config.json", SearchOption.AllDirectories);

            if (_needLoadKoobooWebModules)
            {
                var webDirConfigs = VirtualResources.GetFiles(_koobooWebModulePath, "*config.json", SearchOption.AllDirectories);
                configs = configs.Union(webDirConfigs).ToArray();
            }

            return configs;
        }

        public static string[] GetModuleZips()
        {
            var zipFiles = new List<string>();

            if (Directory.Exists(AppSettings.ModulePath))
            {
                zipFiles.AddRange(Directory.GetFiles(AppSettings.ModulePath, "*.zip"));
            }

            if (_needLoadKoobooWebModules)
            {
                zipFiles.AddRange(Directory.GetFiles(_koobooWebModulePath, "*.zip"));
            }

            return zipFiles.ToArray();
        }
    }
}
