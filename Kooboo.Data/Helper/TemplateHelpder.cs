//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Data.Helper
{
 public  class TemplateHelpder
    { 
        private static List<string> _SyncServerIps;
        private static DateTime LastUpdateServerIp = DateTime.Now; 

        public static List<string> SyncServerIps
        {
            get
            {
                if (_SyncServerIps == null || LastUpdateServerIp < DateTime.Now.AddMinutes(-30))
                {
                    _SyncServerIps = GetNewSnycServers();
                    LastUpdateServerIp = DateTime.Now; 
                } 
                return _SyncServerIps; 
            }
        }

        private static List<string> GetNewSnycServers()
        {
            string ipurl = Data.Helper.AccountUrlHelper.Template("ServerIps"); 
            return Lib.Helper.HttpHelper.Get<List<string>>(ipurl); 
        }
         
         
    }
}
