# Commerce Currency

> `k.commerce.currency` reads site commerce currency configuration and performs exchange-rate conversion. Its premise is not “which currencies Kooboo supports,” but “which currencies the current site has actually enabled in the Commerce admin.”

## Prerequisites

Before using multiple currencies on the site, configure these in the Commerce admin:

- Default currency
- Additional currency list
- The exchange rate or `AutoUpdate` setting for each additional currency

If a currency is not enabled in the admin:

- `k.commerce.currency.list()` will not return it;
- `k.commerce.order.create(..., { currency })` will not actually create the order in that currency and will fall back to the default currency;
- Even if a third-party payment provider supports the currency, you should not enable it directly on the site.

## default

Get the site's default commerce currency code.

```ts
k.api.get(() => {
    return k.commerce.currency.default
})
```

**Returns:** `string` (for example, `"CNY"`)

## list()

Get the currencies currently available for the site.

```ts
k.api.get(() => {
    return k.commerce.currency.list()
})
```

**Returns:** `CurrencyItem[]`

Runtime behavior:

- The result always includes the default currency first, and its `exchangeRate` is fixed at `1`.
- Additional currencies configured in the admin follow it.
- If an additional currency has `AutoUpdate` enabled, `list()` attempts to refresh its `exchangeRate` with the latest rate when reading.

It is recommended to first inspect which currencies the site has actually enabled:

```ts
k.api.get(() => {
    const currencies = k.commerce.currency.list()
    return {
        defaultCurrency: k.commerce.currency.default,
        currencies
    }
})
```

## convert()

Convert between currency exchange rates.

```ts
k.api.get(() => {
    const usdAmount = 2
    const cnyAmount = k.commerce.currency.convert("USD", "CNY", usdAmount)
    return { usdAmount, cnyAmount }
})
```

**Parameters:**

| Parameter | Type | Description |
|------|------|------|
| from | `string` | Source currency code |
| to | `string` | Target currency code |
| value | `number` | Amount to convert |

**Returns:** `number`

Runtime behavior:

- When `from` equals the site's default currency, Kooboo first reads the site's locally configured rate for `to`.
- If the additional currency `to` has a fixed rate configured, the fixed rate is used.
- If `to` has `AutoUpdate` enabled, the remote exchange-rate result is used.
- When `from` is not the site's default currency, Kooboo uses the general exchange-rate service directly instead of the site's local configuration table.

## Use with payment scenarios

`k.commerce.currency` is suitable for three types of tasks:

1. Determine which currencies the site should display to users;
2. Intersect with `k.payment.xxx.supportedCurrency` to filter currencies actually available from the current provider;
3. Convert amounts before displaying prices, preparing quotes, or selecting a provider.

Recommended intersection pattern:

```ts
k.api.get(() => {
    const configured = k.commerce.currency.list().map(item => item.code.toUpperCase())
    const provider = k.payment.stripeCheckout.supportedCurrency.map(code => code.toUpperCase())
    const available = provider.filter(code => configured.includes(code))
    return { available }
})
```

If the goal is for a commerce order to ultimately be paid in a specific currency, create the order in that currency during order placement instead of only calling `convert(...)`:

```ts
k.api.post((body) => {
    const order = k.commerce.order.create(body.cartId, {
        address: body.address,
        note: body.note,
        currency: body.currency
    })

    return {
        orderId: order.id,
        currency: order.currency,
        totalAmount: order.totalAmount
    }
})
```

## CurrencyItem type definition

```ts
interface CurrencyItem {
    name: string;
    nameNative: string;
    code: string;
    symbol: string;
    exchangeRate: number;
}
```
