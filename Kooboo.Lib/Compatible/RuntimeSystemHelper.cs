using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Lib.Helper
{
    public class RuntimeSystemHelper
    {
        public static bool IsLinux()
        {
#if NETSTANDARD
            return System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Linux);
#else
             return false;
#endif

        }

        public static bool IsWindow()
        {
#if NETSTANDARD
            return System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows);
#else
            return true;
#endif

        }
    }
}
