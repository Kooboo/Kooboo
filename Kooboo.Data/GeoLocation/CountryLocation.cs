//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Kooboo.Data.GeoLocation
{
    public static class CountryLocation
    {
        static CountryLocation()
        {
            Items = new Dictionary<short, CountryLocationModel>();

            StreamReader reader = Embedded.EmbeddedHelper.GetStreamReader("CountryLocation", typeof(CountryLocationModel));

            var line = reader.ReadLine();

            while (line != null)
            {
                string[] segs = line.Split(',');

                if (segs.Length == 4)
                {
                    CountryLocationModel location = new CountryLocationModel
                    {
                        CountryCode = segs[0], Continent = segs[1]
                    };
                    if (double.TryParse(segs[2], out var latitude))
                    {
                        location.Latitude = latitude;
                    }

                    if (double.TryParse(segs[3], out var longtitude))
                    {
                        location.Longtitude = longtitude;
                    }

                    short intcode = Kooboo.Data.GeoLocation.CountryCode.ToShort(location.CountryCode);

                    Items[intcode] = location;
                }
                else if (segs.Length > 2)
                {
                    CountryLocationModel location = new CountryLocationModel
                    {
                        CountryCode = segs[0], Continent = segs[1]
                    };

                    short intcode = Kooboo.Data.GeoLocation.CountryCode.ToShort(location.CountryCode);

                    Items[intcode] = location;
                }

                line = reader.ReadLine();
            }
        }

        // int is the hash of countrycode.
        public static Dictionary<short, CountryLocationModel> Items
        {
            get; set;
        }

        public static CountryLocationModel FindCountryLocation(string countryCode)
        {
            var intcode = Kooboo.Data.GeoLocation.CountryCode.ToShort(countryCode);

            return Items.ContainsKey(intcode) ? Items[intcode] : null;
        }

        public static string FindNearByCountry(string targetCode, List<string> available)
        {
            var target = FindCountryLocation(targetCode);
            if (target == null)
            {
                return available.First();
            }

            double distance = double.MaxValue;
            string result = null;

            foreach (var item in available)
            {
                var dest = FindCountryLocation(item);

                if (dest != null)
                {
                    var newdistance = GetDistance(target.Latitude, target.Longtitude, dest.Latitude, dest.Longtitude);

                    if (newdistance < distance)
                    {
                        distance = newdistance;
                        result = item;
                    }
                }
            }

            return result;
        }

        public static double GetDistance(double xLa, double xLong, double yLa, double yLong)
        {
            return Kooboo.Lib.Compatible.CompatibleManager.Instance.Framework.GetDistance(xLa, xLong, yLa, yLong);
        }
    }

    public class CountryLocationModel
    {
        public string CountryCode { get; set; }
        public string Continent { get; set; }
        public double Latitude { get; set; }

        public double Longtitude { get; set; }
    }
}