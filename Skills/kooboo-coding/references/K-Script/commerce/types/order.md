# Commerce Order Types

## OrderDetail

```ts
interface OrderDetail {
    id: string;
    customer: Customer;
    pointsDeductionAmount: number;
    earnPoints: number;
    redeemPoints: number;
    totalAmount: number;
    taxAmount: number;
    originalAmount: number;
    shippingAmount: number;
    insuranceAmount: number | null;
    subtotalAmount: number;
    originalSubtotalAmount: number;
    totalQuantity: number;
    currency: string;
    paid: boolean;
    paidAt: Date | null;
    paymentMethod: string;
    delivered: boolean;
    partialDelivered: boolean;
    trackingNumber: string | null;
    shippingCarrier: string | null;
    shippingAt: Date | null;
    scheduledDeliveryTime: Date | null;
    canceled: boolean;
    cancelReason: string;
    cancelAt: Date | null;
    createdAt: Date;
    updatedAt: Date;
    shippingAddress: Address | null;
    note: string;
    lines: OrderLine[];
    ip: string;
    country: string;
    source: string;
    clientInfo: ClientInfo | null;
    extensionButton: { text: string; url: string } | null;
    discountAllocations: DiscountAllocation[];
    shippingAllocations: ShippingAllocation[];
    extensionFields: KeyValue[] | null;
}
```

## Customer

```ts
interface Customer {
    id: string;
    email: string;
    firstName: string;
    lastName: string;
    phone: string;
}
```

## Address

```ts
interface Address {
    id: string;
    isDefault: boolean;
    country: string;
    province: string;
    city: string;
    address1: string;
    address2: string;
    firstName: string;
    lastName: string;
    phone: string;
    zip: string;
}
```

Order `Address` and customer `AddressModel` use the same field semantics. See [Commerce customer and shipping-address contract](../../../Scenario/commerce-customer-address-flow.md) for Checkout assembly, saved-address reuse, and field-retention rules.

## OrderLine

```ts
interface OrderLine {
    id: string;
    totalAmount: number;
    taxAmount: number;
    originalAmount: number;
    quantity: number;
    totalQuantity: number;
    price: number;
    originalPrice: number;
    title: string;
    image: string;
    productId: string;
    variantId: string;
    sku: string;
    orderId: string;
    options: { name: string; value: string }[];
    discountAllocations: DiscountAllocation[];
    groupName: string;
    isMain: boolean;
    note: string;
    extensionButton: { text: string; url: string } | null;
    trackingNumber: string | null;
    shippingCarrier: string | null;
    shippingAt: Date | null;
    delivered: boolean;
    digitalItems: DigitalOrderItem[];
    autoDelivery: boolean;
    isDigital: boolean;
    maxDownloadCount: number | null;
    maxDownloadDay: number | null;
    errorMessage: string | null;
}
```

When displaying an order line, use `price` as the unit price, `totalAmount` as the line subtotal, and `options` as the variant/options. The amount field on a Cart line is `amount`; do not copy Cart field names to OrderLine. When times such as `createdAt` and `paidAt` enter a browser API, serialize them explicitly as UTC ISO 8601 strings with `Z` or as epoch values to avoid timezone-less date strings.

## ClientInfo

```ts
interface ClientInfo {
    platform: string;
    oS: string;
    device: string;
    application: ApplicationInfo;
}

interface ApplicationInfo {
    isWebBrowser: boolean;
    name: string;
    version: string;
}
```

## Query parameters

```ts
interface OrderQueryParams {
    customerId?: string;
    paid?: boolean;
    delivered?: boolean;
    canceled?: boolean;
    startDate?: Date;
    endDate?: Date;
    pageIndex?: number;   // default: 1
    pageSize?: number;    // default: 30
}
```
