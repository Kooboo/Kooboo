# address

> Address management

## Overview

`k.commerce.address` provides address-related operations for obtaining geographic information such as countries, provinces, and cities.

## countries()

Get all country information.

**Parameters**: none.

```ts
k.api.get(() => {
    const countries = k.commerce.address.countries()
    return { count: countries.length, items: countries.slice(0, 3) }
})
```

**Return:** `Country[]`

## provinces()

Get list of provinces based on country name.

```ts
k.api.get(() => {
    const provinces = k.commerce.address.provinces('China')
    return { count: provinces.length, items: provinces.slice(0, 3) }
})
```

**parameter:**

| Parameter | Type | Required | Description |
|------|------|------|------|
| country | `string` | yes | Country name (English) |

**Return:** `Province[]`

## cities()

Get a list of cities by country and province.

```ts
k.api.get(() => {
    const cities = k.commerce.address.cities('China', 'Fujian')
    return { count: cities.length, items: cities.slice(0, 3) }
})
```

**parameter:**

| Parameter | Type | Required | Description |
|------|------|------|------|
| country | `string` | yes | Country name (English) |
| province | `string` | yes | Province name (English) |

**Return:** `City[]`

## structure

### Country property

| Property | Description | Type |
|------|------|------|
| code | country code | `string` |
| name | Country name (English) | `string` |
| nameTranslations | country name translation | [NameTranslation](#nametranslation-property) |
| currency | currency code | `string` |
| currencySymbol | currency symbol | `string` |
| emojiFlag | flag emoji | `string` |

### Province attribute

| Property | Description | Type |
|------|------|------|
| country | Country name (English) | `string` |
| name | Province name (English) | `string` |
| nameTranslations | Province name translation | [NameTranslation](#nametranslation-property) |

### City property

| Property | Description | Type |
|------|------|------|
| country | Country name (English) | `string` |
| province | Province name (English) | `string` |
| name | City name (English) | `string` |
| nameTranslations | City name translation | [NameTranslation](#nametranslation-property) |

### NameTranslation Property

| Property | Description | Type |
|------|------|------|
| key | language code | `string` |
| value | Corresponding translation | `string` |

## Related Docs

- [k.commerce](./commerce.md) - E-commerce module overview
- [customer](./customer.md) - Customer Management
- [order](./order.md) - Order Management
