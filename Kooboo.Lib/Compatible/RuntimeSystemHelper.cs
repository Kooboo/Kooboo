//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.Runtime.InteropServices;

namespace Kooboo.Lib.Helper
{
    public class RuntimeSystemHelper
    {
        public static bool IsLinux()
        {
            return !RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        }

        public static bool IsWindow()
        {
            return RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        }
    }
}
