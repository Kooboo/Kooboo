//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Text;

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
#if NETSTANDARD2_0
            return System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows);
#else
            return true;
#endif

        }
    }
}
