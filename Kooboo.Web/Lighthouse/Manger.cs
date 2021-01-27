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

    }
}
