using Kooboo.Data.Context;

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
