//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Mail.Helper
{
    public class MineTypeHelper
    {
        public static string GetMineType(string filename)
        {
#if NETSTANDARD2_0
            var contentType = MimeMapping.MimeUtility.GetMimeMapping(filename);
#else
            var contentType = System.Web.MimeMapping.GetMimeMapping(filename);
#endif
            return contentType;
        }
    }
}
