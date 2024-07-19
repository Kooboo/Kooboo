//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.IO;
using System.Runtime.InteropServices;

namespace Kooboo.IndexedDB.Helper
{
    public class PathHelper
    {
        public static int GetLastSlash(string path)
        {
            if (IsWindow())
            {
                return path.LastIndexOf('\\');
            }
            return path.LastIndexOf('/');
        }

        public static bool IsWindow()
        {
            return RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        }


        public static void EnsureDirectoryExists(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        public static void EnsureFileDirectoryExists(string filePath)
        {
            var dir = Path.GetDirectoryName(filePath);
            EnsureDirectoryExists(dir);
        }
    }
}
