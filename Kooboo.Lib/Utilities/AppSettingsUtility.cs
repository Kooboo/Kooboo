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

    }
}
