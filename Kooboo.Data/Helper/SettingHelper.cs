//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.

namespace Kooboo.Data.Helper
{
    public static class SettingHelper
    {
        public static KoobooSetting GetKoobooSetting()
        {
            KoobooSetting setting = new KoobooSetting {IsLocal = !AppSettings.IsOnlineServer};

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