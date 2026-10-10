# Commerce Discount

## list()

Get the list of discounts.

```ts
k.api.get(() => {
    const discounts = k.commerce.discount.list()
    return { count: discounts.length, items: discounts.slice(0, 3) }
})
```
**Returns:** `Discount[]`

## Discount type definition

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

## Boundaries

- Applying coupon codes → `updateDiscountCodes()` in [Cart Guide](cart.md)
