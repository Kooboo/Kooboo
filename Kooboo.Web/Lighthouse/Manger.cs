using Kooboo.Data.Models;
using Kooboo.Lib.Helper;
using Kooboo.Web.ViewModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Web.Lighthouse
{
    public static class Manger
    {

        public static List<ILightHouseItem> List()
        {
            var list = Kooboo.Lib.IOC.Service.GetInstances<ILightHouseItem>();

            return list;
        }

        public static LighthouseItemSetting[] GetLighthouseSettings(WebSite webSite)
        {
            List<LighthouseItemSetting> result;

            if (string.IsNullOrWhiteSpace(webSite.LighthouseSettingsJson))
            {
                result = new List<LighthouseItemSetting>();
            }
            else {
                result = JsonHelper.Deserialize<List<LighthouseItemSetting>>(webSite.LighthouseSettingsJson);
            }

            return result.ToArray();
        }
    }
}
