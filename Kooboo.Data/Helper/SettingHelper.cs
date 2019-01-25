//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Data.Helper
{
  public static class SettingHelper
    {
        public static KoobooSetting GetKoobooSetting()
        {
            KoobooSetting setting = new KoobooSetting();
            setting.IsLocal = !AppSettings.IsOnlineServer;

#if DEBUG
            {

                setting.IsDebug = true; 
            }
#endif

            return setting; 
        } 
    }

    public class KoobooSetting
    {
        public bool IsLocal { get; set; }  
        public bool IsDebug { get; set; }
    }
}
