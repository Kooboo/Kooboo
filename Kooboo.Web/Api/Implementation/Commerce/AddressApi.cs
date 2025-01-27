using Kooboo.Api;
using Kooboo.Sites.Commerce.Entities;
using Kooboo.Sites.Commerce.Services;
using KScript.Commerce;
using KScript.Commerce.Models;

namespace Kooboo.Web.Api.Implementation.Commerce;

public class AddressApi : CommerceApi
{
    public override string ModelName => "Address";

    public Country[] Countries()
    {
        return AddressService.Countries();
    }

    public Province[] Provinces(string country)
    {
        return AddressService.Provinces(country);
    }

    public City[] Cities(string country, ApiCall call)
    {
        var province = call.GetValue("province");
        return AddressService.Cites(country, province);
    }

    public object Regions(string country, ApiCall call)
    {
        var provinces = Provinces(country);
        if (provinces.Length != 0) return provinces;
        return Cities(country, call);
    }

    public AddressDetail[] Details(Address[] addresses)
    {
        var result = new List<AddressDetail>();
        foreach (var item in addresses)
        {
            result.Add(AddressService.AddressDetail(item));
        }

        return [.. result];
    }
}