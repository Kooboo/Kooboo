# Commerce Discount

## list()

获取优惠折扣列表。

```ts
k.api.get(() => {
    const discounts = k.commerce.discount.list()
    return { count: discounts.length, items: discounts.slice(0, 3) }
})
```
**返回：** `Discount[]`

## Discount 类型定义

```ts
interface Discount {
    id: string;
    code: string;
    title: string;
    startDate: string;
    endDate: string;
    condition: DiscountCondition;
    method: "DiscountCode" | "AutomaticDiscount";
    type: "ProductAmountOff" | "OrderAmountOff" | "FreeShipping";
    value: number;
    isPercent: boolean;
    priority: number;
    isExclusion: boolean;
    codeUsageLimit: number | null;
    createdAt: string;
    updatedAt: string;
}

interface DiscountCondition {
    isAny: boolean;
    items: ConditionItem[];
}

interface ConditionItem {
    option: string;
    method: string;
    value: string;
}
```

## 边界说明

- 优惠券使用 → [Cart Guide](cart.md) 的 `updateDiscountCodes()`
