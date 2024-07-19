//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved. 

using System.Linq;

namespace Kooboo.Lib.GeoLocation
{
    public static class IPLocation
    {
        static IPLocation()
        {
            var items = Lib.IOC.Service.GetInstances<Lib.GeoLocation.IIPLocation>();

            if (items != null && items.Any())
            {
                instance = items.OrderByDescending(o => o.Priority).FirstOrDefault();
            }
        }

        public static IIPLocation instance { get; set; }


        public static IPViewModel GetIpCity(string ClientIp)
        {
            if (ClientIp == null)
            {
                return null;
            }

            return instance?.GetIpCity(ClientIp);

        }

        public static IPViewModel GetIpCountry(string ClientIp)
        {
            if (ClientIp == null)
            {
                return null;
            }
            return instance?.GetIpCountry(ClientIp);
        }

        public static IPViewModel GetIpCityOrCountry(string clientIp)
        {
            if (clientIp == null)
            {
                return null;
            }

            return instance?.GetIpCityOrCountry(clientIp);
        }

        public static string GetCountryCode(string clientIP)
        {
            if (clientIP == null)
            {
                return null;
            }

            return instance?.GetCountryCode(clientIP);
        }

        public static double GetDistance(double xLa, double xLong, double yLa, double yLong)
        {
            return Kooboo.Lib.Compatible.CompatibleManager.Instance.Framework.GetDistance(xLa, xLong, yLa, yLong);
        }
    }
}
