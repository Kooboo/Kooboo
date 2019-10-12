//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.

namespace Kooboo.Lib
{
    public class AppSettingsUtility
    {
        public static string Get(string name, string defaultValue = null)
        {
            var val = System.Configuration.ConfigurationManager.AppSettings.Get(name);
            if (string.IsNullOrEmpty(val))
            {
                return defaultValue;
            }
            return val;
        }

        public static bool GetBool(string name)
        {
            var val = System.Configuration.ConfigurationManager.AppSettings.Get(name);
            if (string.IsNullOrEmpty(val))
            {
                return false;
            }

            bool result;

            bool.TryParse(val, out result);

            return result;
        }
    }
}