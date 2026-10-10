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

订单 `Address` 与顾客 `AddressModel` 使用同一字段语义。Checkout 组装、保存地址复用和字段保留规则见 [Commerce 顾客地址与收货地址契约](../../../Scenario/commerce-customer-address-flow.md)。

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

展示订单行时使用 `price` 作为单价、`totalAmount` 作为行小计、`options` 作为变体/规格。Cart 行的金额字段是 `amount`，不要把 Cart 字段名复制到 OrderLine。`createdAt`、`paidAt` 等时间进入浏览器 API 时应显式序列化为带 `Z` 的 UTC ISO 8601 字符串或 epoch，避免无时区日期字符串。

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

## 查询参数

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
