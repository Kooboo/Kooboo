# Commerce Address

## countries()

Get information for all countries.

```ts
k.api.get(() => {
    const countries = k.commerce.address.countries()
    return { count: countries.length, items: countries.slice(0, 3) }
})
```
**Returns:** `Country[]`

## provinces()

Get the province list for a country name.

```ts
k.api.get(() => {
    const provinces = k.commerce.address.provinces('China')
    return { count: provinces.length, items: provinces.slice(0, 3) }
})
```
**Parameter:** `country: string` (English name) | **Returns:** `Province[]`

## cities()

Get the city list for a country and province.

```ts
k.api.get(() => {
    const cities = k.commerce.address.cities('China', 'Fujian')
    return { count: cities.length, items: cities.slice(0, 3) }
})
```
**Parameters:**

| Parameter | Type | Description |
|------|------|------|
| country | `string` | Country name (English) |
| province | `string` | Province name (English) |

**Returns:** `City[]`

## addressDetail()

Resolve an address's stored country, province, and city names to their corresponding administrative-division objects.

```ts
k.api.get(() => {
    return k.commerce.address.addressDetail({
        country: 'China',
        province: 'Fujian',
        city: 'Xiamen',
        address1: 'address1'
    })
})
```
**Parameter:** `address: Address` | **Returns:** `AddressDetail`

## Type definitions

```ts
interface Country {
    code: string;
    name: string;
    nameTranslations: Record<string, string>;
    currency: string;
    currencySymbol: string;
    emojiFlag: string;
}

interface Province {
    country: string;
    name: string;
    nameTranslations: Record<string, string>;
}

interface City {
    country: string;
    province: string;
    name: string;
    nameTranslations: Record<string, string>;
}

interface Address {
    address1: string;
    address2?: string;
    city: string;
    country: string;
    firstName?: string;
    lastName?: string;
    phone?: string;
    province: string;
    zip?: string;
}

interface AddressDetail extends Address {
    countryDetail: Country | null;
    provinceDetail: Province | null;
    cityDetail: City | null;
}
```

## Boundaries

- Forms may display `nameTranslations`; when writing `country/province/city`, use the returned object's English `name`.
- Customer address management and Checkout shipping addresses → [Commerce customer and shipping-address contract](../../Scenario/commerce-customer-address-flow.md)
- Customer address replacement writes → `updateAddresses()` in [Customer Guide](customer.md)
- Order shipping addresses → `create()` in [Order Guide](order.md)
