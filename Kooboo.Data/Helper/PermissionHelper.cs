//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.

using System.Linq;

namespace Kooboo.Data.Helper
{
    public class PermissionHelper
    {
        public static bool IsAllowed(string ip)
        {
            var allServers = OnlineServerHelper.AllServers();
            if (allServers.Any(item => Lib.Helper.IPHelper.IsInSameCClass(ip, item.PrimaryIp)))
            {
                return true;
            }

            return Lib.Helper.IPHelper.IsLocalIp(ip);
        }
    }
}