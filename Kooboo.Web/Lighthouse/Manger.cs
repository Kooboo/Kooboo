using Kooboo.Data.Context;
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
            return Kooboo.Lib.IOC.Service.GetInstances<ILightHouseItem>();
        }


        public static void EnsureLightHouse(RenderContext context)
        {
            if (context.WebSite.EnableLighthouseOptimization)
            {

            }
        }
    }
}
