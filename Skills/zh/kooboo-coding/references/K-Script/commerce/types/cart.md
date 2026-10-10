# Commerce Cart Types

## Cart

```ts
interface Cart {
    id: string;
    customerId: string;
    contact: string;
    country: string;
    discountCodes: string[];
    note: string;
    lines: CartLine[];
    shippingId: string;
    digitalShippingId: string;
    insuranceAmount: number;
    extensionButton: { text: string; url: string } | null;
    redeemPoints: boolean;
    createdAt: Date;
    updatedAt: Date;
}
```

## CartDetail

```ts
interface CartDetail {
    id: string;
    contact: string;
    country: string;
    currency: { code: string; symbol: string; rate: number };
    discountCodes: string[];
    activeDiscountCodes: string[];
    note: string;
    discountAllocations: DiscountAllocation[];
    shippingAllocations: ShippingAllocation[];
    shippingAmount: number;
    insuranceAmount: number;
    subtotalAmount: number;
    totalAmount: number;
    taxAmount: number;
    totalQuantity: number;
    redeemPoints: number;
    pointsDeductionAmount: number;
    earnPoints: number;
    shipping: Shipping | null;
    lines: CartDetailLine[];
}
```

## CartLine

```ts
interface CartLine {
    variantId: string;
    quantity: number;
    groupName: string;
    isMain: boolean;
    note: string;
    extensionButton: { text: string; url: string } | null;
}
```

## CartDetailLine

```ts
interface CartDetailLine {
    variantId: string;
    productId: string;
    title: string;
    options: { name: string; value: string }[];
    sku: string;
    image: string;
    originalPrice: number;
    price: number;
    discountAllocations: DiscountAllocation[];
    quantity: number;
    totalQuantity: number;
    amount: number;
    taxAmount: number;
    inventory: number;
    groupName: string;
    isMain: boolean;
    note: string;
}
```

## ShippingAllocation

```ts
interface ShippingAllocation {
    cost: number;
    title: string;
    isAdditional: boolean;
}
```
