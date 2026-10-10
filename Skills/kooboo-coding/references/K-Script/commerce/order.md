# Commerce Order

## list()

Get the order list.

```ts
k.api.get(() => {
    const result = k.commerce.order.list({
        startDate: new Date('2025-01-01T00:00:00Z'),
        endDate: new Date('2025-12-31T23:59:59Z'),
        paid: false,
        delivered: false,
        canceled: false,
        pageIndex: 1,
        pageSize: 10
    })
    return result
})
```
**Parameters:**

| Parameter | Type | Description |
|------|------|------|
| query.customerId | `string` | Customer ID (optional) |
| query.paid | `boolean` | Whether paid (optional) |
| query.delivered | `boolean` | Whether delivered (optional) |
| query.canceled | `boolean` | Whether canceled (optional) |
| query.startDate | `Date` | Start time (optional) |
| query.endDate | `Date` | End time (optional) |
| query.pageIndex | `number` | Page number, default 1 |
| query.pageSize | `number` | Items per page, default 10 |

**Returns:** `{ list: OrderDetail[], count: number, pageIndex: number, pageSize: number }`

### Storefront order-list authorization

Public Account/Orders APIs must not accept an arbitrary `customerId`. Identity must come from the current Commerce session:

```ts
k.api.get(() => {
    const customerId = k.commerce.customer.currentId || ''
    if (!customerId) return { authenticated: false, list: [], count: 0 }
    return k.commerce.order.list({ customerId, pageIndex: 1, pageSize: 10 })
})
```

## get()

Get the details of a specified order.

```ts
k.api.get(() => {
    const order = k.commerce.order.get('order-id')
    if (!order) return { success: false, error: 'Order not found' }
    return { success: true, id: order.id, totalAmount: order.totalAmount }
})
```
**Parameter:** `orderId: string` | **Returns:** `OrderDetail | null`

### Order-line display fields

- Unit price: `line.price`
- Line subtotal: `line.totalAmount`
- Variant/options: `line.options`

Cart lines use `line.amount`, while OrderLine uses `line.totalAmount`; these are different display contracts. Do not reuse `amount` from the Cart template on the order page or replace the runtime-calculated line subtotal with `price * quantity`.

### Browser time serialization

Times such as the order entity's `createdAt` are created in UTC and emitted by the order ViewModel. Browser APIs must output values with an explicit timezone, preferably ISO 8601 UTC (ending in `Z`) or epoch values, before passing them to `new Date(...)`. Do not output timezone-less strings such as `2026/8/19 05:23:00`; browsers may interpret them as local time or fail to parse them.

Do not assume that one KScript date-string format remains fixed across every serialization boundary; the API layer should serialize UTC explicitly.

Public order-detail, continue-payment, and cancellation APIs must verify ownership:

```ts
k.api.get(() => {
    const customerId = k.commerce.customer.currentId || ''
    const orderId = k.request.queryString.get('orderId') || ''
    const order = orderId ? k.commerce.order.get(orderId) : null
    if (!customerId || !order?.customer?.id || order.customer.id !== customerId) {
        return { success: false, error: 'Order not found' }
    }
    return { success: true, order }
})
```

Do not return an order based on a customer ID from `body.customerId`, `query.customerId`, or the URL. Return not found uniformly to unauthorized callers to avoid revealing whether an order exists.

## create()

Create an order (a cart is required first).

```ts
k.api.post((body) => {
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
            isDefault: false
        },
        note: 'test order',
        currency: body.currency
    })
    return { orderId: order.id }
})
```
**Parameters:**

| Parameter | Type | Description |
|------|------|------|
| cartId | `string` | Cart ID |
| options.address | `Address` | Shipping address (required) |
| options.note | `string` | Note (optional) |
| options.currency | `string` | Target order currency; falls back to the site default when unconfigured (optional) |
| options.extensionFields | `KeyValue[]` | Extension fields (optional) |
| options.scheduledDeliveryTime | `Date` | Scheduled delivery time (optional) |

**Returns:** `OrderDetail`

`options.address` follows [Commerce customer and shipping-address contract](../../Scenario/commerce-customer-address-flow.md). Write the Checkout contact phone to `address.phone`; when reusing a saved customer address, copy the complete address object instead of trimming it to fields visible in the current form.

::: warning Prerequisite
`k.commerce.order.create(cartId, ...)` is expected to run after a successful `k.commerce.customer.login(email, password)` flow.

:::

Recommended example:

```ts
k.api.post((body) => {
    const customerId = k.commerce.customer.currentId || ''
    if (!customerId) {
        return { success: false, error: 'Commerce customer login is required' }
    }

    const cartId = k.commerce.cart.currentId || ''
    if (!cartId) {
        return { success: false, error: 'Commerce cart is required' }
    }

    const cart = k.commerce.cart.get(cartId)
    if (!cart) {
        return { success: false, error: 'Cart not found' }
    }
    if (!cart.customerId || cart.customerId !== customerId) {
        return { success: false, error: 'Cart is not bound to the current Commerce customer' }
    }

    const order = k.commerce.order.create(cartId, {
        address: body.address,
        note: body.note,
        currency: body.currency
    })

    if (!order?.id || !order?.customer?.id || order.customer.id !== customerId) {
        return { success: false, error: 'Order ownership missing; do not start payment' }
    }

    return {
        success: true,
        orderId: order.id,
        customerId: order.customer.id,
        currency: order.currency
    }
})
```

::: danger Order ownership comes from the cart
`order.CustomerId` is copied from `cart.CustomerId`. Normally, `customer.login()` binds the current Commerce cart to the logged-in customer; Checkout must still explicitly compare `cart.customerId === customer.currentId`, and after creation compare `order.customer.id === customer.currentId`. If they differ, investigate session/cart drift, a stale cartId, or a missing login binding; do not remove the ownership check to avoid “order not found.” See [Commerce checkout and payment golden path](../../Scenario/commerce-checkout-payment-flow.md) for the complete path.
:::

::: tip Single Commerce state
Login, cart, and order placement use `customer.currentId`, `cart.currentId`, and `kb_commerce` consistently by default. Do not ask for email and password again in the order API, create `commerce_token` by default, or let Checkout and Header judge different login states independently.
:::

::: tip Placing an order in another currency
If the order will ultimately use third-party payment in a specific currency, pass `currency` to `order.create(...)` first. This keeps the order amount, line amounts, and later payment currency consistent.
:::

## pay()

Mark an order as paid directly.

**Parameters:**

| Parameter | Type | Description |
|------|------|------|
| orderId | `string` | Order ID |
| paymentMethod | `string` | Payment method |

::: danger Direct storefront calls are prohibited
`k.commerce.order.pay(...)` immediately marks the order paid, increases product sales, and triggers the paid event; it does not verify the payment-provider signature and does not represent actual funds received.

Do not place it in a public GET/POST API, payment return URL, frontend confirmation button, order-result page, or polling endpoint. A normal storefront should call `k.payment.xxx.charge({ order: orderId, ... })` and let the built-in provider callback / `checkStatus(...)` update the order after real payment is confirmed. Use this method explicitly only in trusted internal migrations, manual order corrections, or controlled admin flows that have completed independent signature verification.
:::

## delivery()

Deliver an order.

```ts
k.api.post(() => {
    const shipping = k.commerce.shipping.list()[0]
    k.commerce.order.delivery('order-id', shipping.id, 'SF123456789')
})
```
**Parameters:**

| Parameter | Type | Description |
|------|------|------|
| orderId | `string` | Order ID |
| shippingCarrier | `string` | Shipping-carrier ID |
| trackingNumber | `string` | Tracking number |

## cancel()

Cancel an order.

```ts
k.api.post(() => {
    k.commerce.order.cancel('order-id', 'cancel reason')
})
```
**Parameters:**

| Parameter | Type | Description |
|------|------|------|
| orderId | `string` | Order ID |
| reason | `string` | Cancellation reason |

## reopen()

Reopen a canceled order.

```ts
k.api.post(() => {
    k.commerce.order.reopen('order-id')
})
```

## updateAddress()

Update an order's shipping address.

```ts
k.api.post(() => {
    k.commerce.order.updateAddress('order-id', {
        country: 'China',
        province: 'Fujian',
        city: 'Xiamen',
        address1: 'address1',
        address2: 'Room 1201',
        zip: '361000',
        firstName: 'firstName',
        lastName: 'lastName',
        phone: '13800138000',
        isDefault: true
    })
})
```

## updateShippingInfo()

Update order delivery information (including tracking numbers and digital-product delivery).

```ts
k.api.post(() => {
    const order = k.commerce.order.get('order-id')
    const lineId = order.lines[0].id
    const shipping = k.commerce.shipping.list()[0]
    k.commerce.order.updateShippingInfo('order-id', lineId, {
        shippingCarrier: shipping.id,
        trackingNumber: 'SF123456789'
    })
})
```
**Parameters:**

| Parameter | Type | Description |
|------|------|------|
| orderId | `string` | Order ID |
| orderLineId | `string` | Order-item ID |
| options.shippingCarrier | `string` | Shipping carrier (optional) |
| options.trackingNumber | `string` | Tracking number (optional) |
| options.digitalItems | `DigitalOrderItem[]` | Digital products (optional) |

## delete()

Delete an order.

```ts
k.api.post(() => {
    k.commerce.order.delete('order-id')
})
```

## Type hint

If you are unsure about the fields of Order / OrderLine / Address, use [Order Types](types/order.md) for the complete type definitions.

## Boundaries

- A cart is required before order creation ([Cart Guide](cart.md))
- `pay()`, `delivery()`, `cancel()`, `reopen()`, address/shipping updates, and `delete()` do not perform caller authorization; public endpoints must enforce customer ownership or the appropriate admin authorization before invoking them.
- Customer address book and order shipping-address fields → [Commerce customer and shipping-address contract](../../Scenario/commerce-customer-address-flow.md)
- Main commerce customer login and ordering flow → [Commerce customer login and ordering flow](../../Scenario/commerce-customer-login-flow.md)
- This guide does not cover cart-operation details
