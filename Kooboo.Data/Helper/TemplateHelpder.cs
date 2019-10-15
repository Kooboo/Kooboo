//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using System;
using System.Collections.Generic;

namespace Kooboo.Data.Helper
{
    public class TemplateHelpder
    {
        private static List<string> _syncServerIps;
        private static DateTime _lastUpdateServerIp = DateTime.Now;

        public static List<string> SyncServerIps
        {
            get
            {
                if (_syncServerIps == null || _lastUpdateServerIp < DateTime.Now.AddMinutes(-30))
                {
                    _syncServerIps = GetNewSnycServers();
                    _lastUpdateServerIp = DateTime.Now;
                }
                return _syncServerIps;
            }
        }

        private static List<string> GetNewSnycServers()
        {
            string ipurl = Data.Helper.AccountUrlHelper.Template("ServerIps");
            return Lib.Helper.HttpHelper.Get<List<string>>(ipurl);
        }
    }
}