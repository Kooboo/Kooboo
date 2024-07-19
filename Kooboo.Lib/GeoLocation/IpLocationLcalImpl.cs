using System.Linq;
using Kooboo.Lib.GeoLocation;
using MaxMind.GeoIP2.Responses;

namespace Kooboo.ShareInfo.GeoLocation
{
    public class IpLocationLcalImpl : Kooboo.Lib.GeoLocation.IIPLocation
    {
        public int Priority { get; set; } = 0;

        public string GetCountryCode(string clientIP)
        {
            var res = this.GetIpCountry(clientIP);
            return res != null ? res.CountryCode : null;
        }

        public IPViewModel GetIpCity(string ClientIp)
        {
            var res = LocalMaxMindDb.instance.ReadCity(ClientIp);
            return res == null ? null : FromCityResponse(res);
        }

        public IPViewModel GetIpCountry(string ClientIp)
        {
            var res = LocalMaxMindDb.instance.ReadCountry(ClientIp);

            if (res != null)
            {
                return FromCountryResponse(res);
            }
            var cityRes = LocalMaxMindDb.instance.ReadCity(ClientIp);
            return cityRes != null ? FromCityResponse(cityRes) : null;

        }

        public IPViewModel GetIpCityOrCountry(string ClientIp)
        {
            var res = LocalMaxMindDb.instance.ReadCity(ClientIp);
            if (res != null)
            {
                return FromCityResponse(res);
            }
            var countryRes = LocalMaxMindDb.instance.ReadCountry(ClientIp);
            return countryRes != null ? FromCountryResponse(countryRes) : null;
        }


        public IPViewModel FromCityResponse(CityResponse res)
        {
            if (res == null)
            {
                return null;
            }

            IPViewModel model = new IPViewModel();
            model.City = res.City.Name;
            model.CountryName = res.Country.Name;
            model.CountryCode = res.Country.IsoCode;
            if (res.Location != null)
            {
                model.Latitude = res.Location.Latitude ?? 0;
                model.Longitude = res.Location.Longitude ?? 0;
            }

            if (res.Subdivisions != null)
            {
                var state = res.Subdivisions.FirstOrDefault();
                if (state != null)
                {
                    model.State = state.Name;
                }
            }

            return model;

        }


        public IPViewModel FromCountryResponse(CountryResponse res)
        {
            if (res == null)
            {
                return null;
            }

            IPViewModel model = new IPViewModel();

            model.CountryName = res.Country.Name;
            model.CountryCode = res.Country.IsoCode;

            var location = CountryLocation.FindCountryLocation(res.Country.IsoCode);

            if (location != null)
            {
                model.Latitude = location.Latitude;
                model.Longitude = location.Longtitude;
            }
            return model;
        }


    }
}
