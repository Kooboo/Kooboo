//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public static int GetInt(string name, int defaultValue = 0)
        {
            if (name == null)
            {
                return defaultValue;
            }

            var val = System.Configuration.ConfigurationManager.AppSettings.Get(name);
            if (string.IsNullOrWhiteSpace(val))
            {
                val = System.Configuration.ConfigurationManager.AppSettings.Get(name.ToLower());
            }
             
            if (!string.IsNullOrWhiteSpace(val))
            {
                if (int.TryParse(val, out int result))
                {
                    return result; 
                }
            }

            return defaultValue; 
        }


    }
}
