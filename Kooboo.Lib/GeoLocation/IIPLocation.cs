namespace Kooboo.Lib.GeoLocation
{
    public interface IIPLocation
    {
        int Priority { get; set; }

        IPViewModel GetIpCity(string ClientIp);
        IPViewModel GetIpCountry(string ClientIp);

        // try city first, if not found,try country.
        IPViewModel GetIpCityOrCountry(string ClientIp);

        string GetCountryCode(string clientIP);
    }
}
