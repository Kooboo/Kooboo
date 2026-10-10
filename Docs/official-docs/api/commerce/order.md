# order

> Order

## Overview

`k.commerce.order` provides order-related operations, including query, creation, payment, delivery, cancellation and other functions.

## list()

Get the order list.

```ts
k.api.get(() => {
    const result = k.commerce.order.list({
        startDate: new Date('2025-01-01'),
        endDate: new Date('2025-12-31'),
        paid: false,
        delivered: false,
        canceled: false,
        pageIndex: 1,
        pageSize: 10
    })
    return result
})
```

**parameter:**

| Parameter | Type | Required | Description |
|------|------|------|------|
| query.customerId | `string` | no | Customer ID |
| query.paid | `boolean` | no | Has it been paid? |
| query.delivered | `boolean` | no | Has it been shipped? |
| query.canceled | `boolean` | no | Has it been cancelled? |
| query.startDate | `Date` | no | start time |
| query.endDate | `Date` | no | Deadline |
| query.pageIndex | `number` | no | Page number, default 1 |
| query.pageSize | `number` | no | Number of items per page, default 10 |

**return:**

```ts
{
    list: OrderDetail[];  // Order list
    count: number;        // Total count
    pageIndex: number;    // Current page number
    pageSize: number;     // Items per page
}
```

## get()

Get the specified order details.

```ts
k.api.get(() => {
    const order = k.commerce.order.get('order-id')
    return { id: order.id, totalAmount: order.totalAmount }
})
```

**parameter:**

| Parameter | Type | Required | Description |
|------|------|------|------|
| orderId | `string` | yes | Order ID |

**Return:** `OrderDetail`

## create()

Create an order.

```ts
k.api.post(() => {
    // Create a cart and add a product
    const cartId = k.commerce.cart.create()
    const product = k.commerce.product.create({
        title: 'Order Test Product',
        description: 'Test',
        price: 99.9,
        active: true,
        seoName: 'order-test-' + Date.now(),
        featuredImage: ''
    } as any)
    k.commerce.product.createVariant(product.id, {
        sku: 'ORD-SKU-001',
        price: 99.9,
        inventory: 100,
        barcode: '',
        active: true,
        selectedOptions: []
    } as any)
    const refreshed = k.commerce.product.get(product.id)
    k.commerce.cart.addOrUpdateLine(cartId, refreshed.variants[0].id, 1)

    // Create the order
    const order = k.commerce.order.create(cartId, {
        address: {
            country: 'China',
            province: 'Fujian',
            city: 'Xiamen',
            address1: 'address1',
            address2: '',
            zip: '361000',
            firstName: 'firstName',
            lastName: 'lastName',
            phone: '13800138000',
            isDefault: true
        },
        note: 'test order'
    })
    return { orderId: order.id }
})
```

**parameter:**

| Parameter | Type | Required | Description |
|------|------|------|------|
| cartId | `string` | yes | Cart ID |
| options | `object` | yes | Order options |
| options.address | `Address` | yes | Shipping address |
| options.extensionFields | `KeyValue[]` | no | extension fields |
| options.note | `string` | no | Remark |
| options.scheduledDeliveryTime | `Date` | no | Estimated delivery time |

**Return:** `OrderDetail`

## cancel()

Cancel order.

```ts
k.api.post(() => {
    k.commerce.order.cancel('order-id', 'cancel reason')
    return 'success'
})
```

**parameter:**

| Parameter | Type | Required | Description |
|------|------|------|------|
| orderId | `string` | yes | Order ID |
| reason | `string` | yes | Reason for cancellation |

**Return:** `void`

## delete()

Delete order.

```ts
k.api.post(() => {
    k.commerce.order.delete('order-id')
    return 'success'
})
```

**parameter:**

| Parameter | Type | Required | Description |
|------|------|------|------|
| orderId | `string` | yes | Order ID |

**Return:** `void`

## pay()

Pay for your order.

```ts
k.api.post(() => {
    k.commerce.order.pay('order-id', 'alipay')
    return 'success'
})
```

**parameter:**

| Parameter | Type | Required | Description |
|------|------|------|------|
| orderId | `string` | yes | Order ID |
| paymentMethod | `string` | yes | Payment method |

**Return:** `void`

## delivery()

Order shipped.

```ts
k.api.post(() => {
    const shipping = k.commerce.shipping.list()[0]
    k.commerce.order.delivery('order-id', shipping.id, 'SF123456789')
    return 'success'
})
```

**parameter:**

| Parameter | Type | Required | Description |
|------|------|------|------|
| orderId | `string` | yes | Order ID |
| shippingCarrier | `string` | yes | Logistics company |
| trackingNumber | `string` | yes | Logistics order number |

**Return:** `void`

## reopen()

Reopen a canceled order.

```ts
k.api.post(() => {
    k.commerce.order.reopen('order-id')
    return 'success'
})
```

**parameter:**

| Parameter | Type | Required | Description |
|------|------|------|------|
| orderId | `string` | yes | Order ID |

**Return:** `void`

## updateAddress()

Update the order shipping address.

```ts
k.api.post(() => {
    k.commerce.order.updateAddress('order-id', {
        country: 'China',
        province: 'Fujian',
        city: 'Xiamen',
        address1: 'address1',
        address2: '',
        zip: '361000',
        firstName: 'firstName',
        lastName: 'lastName',
        phone: '13800138000',
        isDefault: true
    })
    return 'success'
})
```

**parameter:**

| Parameter | Type | Required | Description |
|------|------|------|------|
| orderId | `string` | yes | Order ID |
| address | `Address` | yes | Shipping address |

**Return:** `void`

## updateShippingInfo()

Update order shipping information.

```ts
k.api.post(() => {
    const order = k.commerce.order.get('order-id')
    const lineId = order.lines[0].id
    const shipping = k.commerce.shipping.list()[0]

    k.commerce.order.updateShippingInfo('order-id', lineId, {
        shippingCarrier: shipping.id,
        trackingNumber: 'SF123456789'
    })
    return 'success'
})
```

**parameter:**

| Parameter | Type | Required | Description |
|------|------|------|------|
| orderId | `string` | yes | Order ID |
| orderLineId | `string` | yes | Order item ID |
| options | `object` | yes | Shipping options |
| options.shippingCarrier | `string` | no | Logistics company |
| options.trackingNumber | `string` | no | Logistics order number |
| options.digitalItems | `DigitalOrderItem[]` | no | digital products |

**Return:** `void`

## structure

### OrderDetail Property

| Property | Description | Type |
|------|------|------|
| ID | Order ID | `string` |
| customer | customer information | [Customer](#customer-attribute) |
| pointsDeductionAmount | Points deduction amount | `number` |
| earnPoints | earn points | `number` |
| redeemPoints | Redeem points | `number` |
| totalAmount | Total order amount | `number` |
| taxAmount | tax | `number` |
| originalAmount | original amount | `number` |
| shippingAmount | Delivery amount | `number` |
| insuranceAmount | Insurance amount | `number` |
| subtotalAmount | Subtotal amount | `number` |
| originalSubtotalAmount | Original subtotal amount | `number` |
| totalQuantity | total quantity | `number` |
| currency | currency | `string` |
| paid | Has it been paid? | `boolean` |
| paidAt | payment time | `Date` |
| paymentMethod | Payment method | `string` |
| delivered | Has it been shipped? | `boolean` |
| partialDelivered | Whether to partially ship | `boolean` |
| trackingNumber | Logistics order number | `string` |
| shippingCarrier | Logistics company | `string` |
| shippingAt | Shipping time | `Date` |
| scheduledDeliveryTime | Estimated delivery time | `Date` |
| canceled canceled | Has it been cancelled? | `boolean` |
| cancelReason | Reason for cancellation | `string` |
| cancelAt | Cancellation time | `Date` |
| createdAt | creation time | `Date` |
| updatedAt | Update time | `Date` |
| shippingAddress | Shipping address | [Address](#address-property) |
| note | Remark | `string` |
| lines | Order items | [OrderLine](#orderline-property) |
| ip | IP address | `string` |
| country | nation | `string` |
| source | source | `string` |
| clientInfo | Client information | [ClientInfo](#clientinfo-property) |
| extensionButton | Extension button | `{ text: string, url: string }` |
| discountAllocations | Automatic discount information | `DiscountAllocation[]` |
| shippingAllocations | Shipping method list | `ShippingAllocation[]` |
| extensionFields | extension fields | `KeyValue[]` |

### Customer attribute

| Property | Description | Type |
|------|------|------|
| ID | Customer ID | `string` |
| email | Customer email | `string` |
| firstName | Customer name | `string` |
| lastName | Customer last name | `string` |
| phone | Customer phone number | `string` |

### Address property

| Property | Description | Type |
|------|------|------|
| ID | Address ID | `string` |
| isDefault | Whether the default address | `boolean` |
| country | nation | `string` |
| province | province | `string` |
| city | City | `string` |
| address1 | Address 1 | `string` |
| address2 | Address 2 | `string` |
| firstName | Consignee name | `string` |
| lastName | Consignee's last name | `string` |
| phone | Telephone | `string` |
| zip | post code | `string` |

### OrderLine Property

| Property | Description | Type |
|------|------|------|
| ID | Order item ID | `string` |
| totalAmount | Total amount of order items | `number` |
| taxAmount | Order product tax | `number` |
| originalAmount | Original amount of order items | `number` |
| quantity | Order quantity | `number` |
| totalQuantity | Total quantity of items ordered | `number` |
| price | Order item price | `number` |
| originalPrice | Original price of order items | `number` |
| title | Order product name | `string` |
| image | Order product pictures | `string` |
| productId | Product ID | `string` |
| variantId | Variant ID | `string` |
| sku | Product SKU | `string` |
| orderId | Order ID | `string` |
| options | Product options | `{ name: string, value: string }[]` |
| discountAllocations | Automatic discount information | `DiscountAllocation[]` |
| groupName | Product group name | `string` |
| isMain | Is it the main product? | `boolean` |
| note | Notes on order items | `string` |
| extensionButton | Extension button | `{ text: string, url: string }` |
| trackingNumber | Logistics order number | `string` |
| shippingCarrier | Logistics company | `string` |
| shippingAt | Shipping time | `Date` |
| delivered | Has it been shipped? | `boolean` |
| digitalItems | Digital product list | `DigitalOrderItem[]` |
| autoDelivery | Whether to ship automatically | `boolean` |
| isDigital | Is it a digital product? | `boolean` |
| maxDownloadCount | Maximum number of downloads | `number` |
| maxDownloadDay | Maximum download days | `number` |
| errorMessage | error message | `string` |

### ClientInfo Property

| Property | Description | Type |
|------|------|------|
| platform | platform | `string` |
| oS | operating system | `string` |
| device | equipment | `string` |
| application | Application information | [ApplicationInfo](#applicationinfo-property) |

### ApplicationInfo Property

| Property | Description | Type |
|------|------|------|
| isWebBrowser | Is it a web browser? | `boolean` |
| name | Application name | `string` |
| version | Application version | `string` |

## Related Docs

- [k.commerce](./commerce.md) - E-commerce module overview
- [cart](./cart.md) - Shopping Cart
- [product](./product.md) - Product Management
