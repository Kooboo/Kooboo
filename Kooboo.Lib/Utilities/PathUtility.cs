using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Kooboo.Lib.Utilities
{
    public static class PathUtility
    {
        public static string TryRootPath()
        {
            var basefolder = AppDomain.CurrentDomain.BaseDirectory;
            if (IsKoobooDiskRoot(basefolder))
            {
                return basefolder;
            }

            List<string> trypaths = Kooboo.Lib.Compatible.CompatibleManager.Instance.System.GetTryPaths();

            foreach (var item in trypaths)
            {
                basefolder = System.IO.Path.GetFullPath(item);
                if (basefolder != null && IsKoobooDiskRoot(basefolder))
                {
                    return basefolder;
                }
            }

            return AppDomain.CurrentDomain.BaseDirectory;
        }

        private static bool IsKoobooDiskRoot(string FullPath)
        {
            string ScriptFolder = System.IO.Path.Combine(FullPath, "_Admin", "Scripts");
            if (!Directory.Exists(ScriptFolder))
            {
                return false;
            }
            string ViewFolder = System.IO.Path.Combine(FullPath, "_Admin", "View");
            if (!Directory.Exists(ViewFolder))
            {
                return false;
            }
            return true;
        }

    }
}
