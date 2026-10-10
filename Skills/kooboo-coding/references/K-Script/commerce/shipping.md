# Commerce Shipping

## list()

Get the shipping-method list for physical products.

```ts
k.api.get(() => {
    const shippings = k.commerce.shipping.list()
    return { count: shippings.length, items: shippings }
})
```
**Parameters:**

| Parameter | Type | Description |
|------|------|------|
| options.cartId | `string` | Cart ID used to calculate the shipping cost for that cart (optional) |
| options.currency | `string` | Currency code for returned costs (optional) |

**Returns:** `ShippingItem[]`

Without `cartId`, `currentCost` is the rate-adjusted base cost. With `cartId`, the runtime recalculates `currentCost` from the cart lines and selected shipping method.

## digitalList()

Get the shipping-method list for digital products.

```ts
k.api.get(() => {
    const digitalShippings = k.commerce.shipping.digitalList()
    return { count: digitalShippings.length }
})
```
**Returns:** `DigitalShipping[]`

## ShippingItem type definition

```ts
interface ShippingItem {
    id: string;
    name: string;
    description: string;
    baseCost: number;
    additionalCosts: AdditionalCost[];
    isDefault: boolean;
    countries: SupportCountry[];
    currentCost: number;
    code: string;
    currency: string;
    symbol: string;
}

interface AdditionalCost {
    cost: number;
    description: string;
    condition: Condition;
}

interface SupportCountry {
    name: string;
    display: string;
    estimatedDaysOfArrival: number;
}
```

## DigitalShipping type definition

```ts
interface DigitalShipping {
    id: string;
    name: string;
    description: string;
    mailServerType: string;
    koobooEmailAddress: string;
    customMailServer: MailServer;
    mailTemplate: { subject: string; body: string };
    isDefault: boolean;
    createdAt: Date;
    updatedAt: Date;
}

interface MailServer {
    server: string;
    port: number;
    ssl: boolean;
    from: string;
    userName: string;
    password: string;
}
```

## Boundaries

- Set a cart shipping method → `updateShipping()` in [Cart Guide](cart.md)
- Deliver an order → `delivery()` in [Order Guide](order.md)
