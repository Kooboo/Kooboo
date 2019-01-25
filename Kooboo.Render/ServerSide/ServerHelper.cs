//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Render.ServerSide
{
 public static   class ServerHelper
    {

        public static string EnsureRelative(string relativeUrl, string baseRelativeUrl)
        {

            relativeUrl = relativeUrl.Replace("\\", "/");

            if (!relativeUrl.StartsWith("/") && !string.IsNullOrEmpty(baseRelativeUrl))
            {
                relativeUrl = Kooboo.Lib.Helper.UrlHelper.Combine(baseRelativeUrl, relativeUrl);
            }
            return relativeUrl; 
        }



    }
}
