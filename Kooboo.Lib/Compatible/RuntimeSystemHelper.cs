//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Lib.Helper
{
    public class RuntimeSystemHelper
    {
        public static bool IsLinux()
        {
#if NETSTANDARD2_0
            return System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Linux);
#else
             return false;
#endif

        }

        public static bool IsWindow()
        {
#if NETSTANDARD2_0
            return System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows);
#else
            return true;
#endif

        }
    }
}
