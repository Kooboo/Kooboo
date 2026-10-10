# currency

> currency management

## Overview

`k.commerce.currency` provides currency-related operations for obtaining currency lists and exchange rate conversions.

## default

Get the default currency code.

```ts
k.api.get(() => {
    return k.commerce.currency.default
})
```

**Returns:** `string` - Default currency code (e.g. "CNY")

## list()

Get a list of supported currencies.

**Parameters**: none.

```ts
k.api.get(() => {
    return k.commerce.currency.list()
})
```

**Return:** [CurrencyItem](#currencyitem-property)[]

## convert()

Currency exchange rate conversion.

```ts
k.api.get(() => {
    const usdAmount = 2
    const cnyAmount = k.commerce.currency.convert("USD", "CNY", usdAmount)
    return { usdAmount, cnyAmount }
})
```

**parameter:**

| Parameter | Type | Required | Description |
|------|------|------|------|
| from | `string` | yes | Source currency code |
| to | `string` | yes | target currency code |
| value | `number` | yes | Conversion amount |

**Returns:** `number` - converted amount

## structure

### CurrencyItem Property

| Property | Description | Type |
|------|------|------|
| name | currency name | `string` |
| nameNative | local currency name | `string` |
| code | currency code | `string` |
| symbol | currency symbol | `string` |
| exchangeRate | exchange rate | `number` |

## Related Docs

- [k.commerce](./commerce.md) - E-commerce module overview
- [settings](./settings.md) - E-commerce settings
