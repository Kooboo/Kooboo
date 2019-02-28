//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

                if (segs != null)
                {
                    if (segs.Length == 4)
                    {
                        CountryLocationModel location = new CountryLocationModel();
                        location.CountryCode = segs[0];
                        location.Continent = segs[1];
                        double latitude;
                        if (double.TryParse(segs[2], out latitude))
                        {
                            location.Latitude = latitude;
                        }

                        double longtitude;
                        if (double.TryParse(segs[3], out longtitude))
                        {
                            location.Longtitude = longtitude;
                        }

                        short intcode = Kooboo.Data.GeoLocation.CountryCode.ToShort(location.CountryCode);

                        Items[intcode] = location;

                    }
                    else if (segs.Length > 2)
                    {
                        CountryLocationModel location = new CountryLocationModel();
                        location.CountryCode = segs[0];
                        location.Continent = segs[1];

                        short intcode = Kooboo.Data.GeoLocation.CountryCode.ToShort(location.CountryCode);

                        Items[intcode] = location;

                    }
                }

                line = reader.ReadLine();
            }

        }
         
        // int is the hash of countrycode.
        public static Dictionary<short, CountryLocationModel> Items
        {
            get; set;
        }

        public static CountryLocationModel FindCountryLocation(string CountryCode)
        {
            var intcode = Kooboo.Data.GeoLocation.CountryCode.ToShort(CountryCode);

            if (Items.ContainsKey(intcode))
            {
                return Items[intcode];
            }
            return null;
        }

        public static string FindNearByCountry(string TargetCode, List<string> Available)
        {
            var target = FindCountryLocation(TargetCode);
            if (target == null)
            {
                return Available.First();
            }

            double distance = double.MaxValue;
            string result = null;

            foreach (var item in Available)
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
