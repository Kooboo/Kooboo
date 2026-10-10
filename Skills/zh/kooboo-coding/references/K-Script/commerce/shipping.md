# Commerce Shipping

## list()

获取实体产品配送方式列表。

```ts
k.api.get(() => {
    const shippings = k.commerce.shipping.list()
    return { count: shippings.length, items: shippings }
})
```
**参数：**

| 参数 | 类型 | 说明 |
|------|------|------|
| options.cartId | `string` | 用于按购物车计算配送费用的购物车 ID（可选） |
| options.currency | `string` | 返回费用使用的货币代码（可选） |

**返回：** `ShippingItem[]`

不传 `cartId` 时，`currentCost` 是按货币汇率换算后的基础费用；传入 `cartId` 后，运行时会根据购物车行和配送方式重新计算 `currentCost`。

## digitalList()

获取数字产品配送方式列表。

```ts
k.api.get(() => {
    const digitalShippings = k.commerce.shipping.digitalList()
    return { count: digitalShippings.length }
})
```
**返回：** `DigitalShipping[]`

## ShippingItem 类型定义

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

## DigitalShipping 类型定义

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

## 边界说明

- 购物车设置配送方式 → [Cart Guide](cart.md) 的 `updateShipping()`
- 订单发货 → [Order Guide](order.md) 的 `delivery()`
