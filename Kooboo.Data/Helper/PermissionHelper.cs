//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Data.Helper
{
    public class PermissionHelper
    {
        public static bool IsAllowed(string ip)
        {
            var allServers = OnlineServerHelper.AllServers();
            foreach (var item in allServers)
            {
                if (Lib.Helper.IPHelper.IsInSameCClass(ip, item.PrimaryIp))
                {
                    return true;
                }
            }

            if (Lib.Helper.IPHelper.IsLocalIp(ip))
            {
                return true;
            }
            return false;
        }
    }
}
