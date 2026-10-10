# square

> Square Payments.

## Overview

`k.payment.square` is used to create Square Checkout payment requests. Square application and Location information need to be configured in the site CMS before use.

## square.charge()

Create a Square Checkout payment request.

```ts
k.api.get(() => {
    const result = k.payment.square.charge({
        name: 'Order summary',
        description: 'Order payment',
        totalAmount: 1.5,
        currency: 'USD',
        order: 'order-id',
        returnUrl: '/payment/success?orderId=order-id',
        callbackCodeName: 'PaymentCallback'
    })

    return k.response.redirect(result.nextAction.redirectUrl)
})
```

**Parameter:** `SquareCheckoutParams`

| Parameter | Type | Required | Description |
|------|------|------|------|
| returnUrl | `string` | yes | Return address after successful payment |
| totalAmount | `number` | yes | Order amount |
| name | `string` | yes | Order name |
| description | `string` | yes | Order description |
| currency | `string` | yes | Currency, such as `USD` |
| order | `string` | no | Commerce order ID. Once the payment is successful, the order payment status will be updated. |
| callbackCodeName | `string` | no | The name of the callback code executed after payment is completed |

**Return:** `ChargeResponse`

## checkStatus()

Check payment status.

**Parameters:**

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `requestId` | `string` | Yes | `requestId` returned by `charge()`. |

```ts
k.api.get(() => {
    const status = k.payment.square.checkStatus('request-id')
    return { paid: status.paid, failed: status.failed, status: status.status }
})
```

**Return:** `PaymentStatusResponse`

## notify()

Handle Square notification callbacks.

**Parameters:**

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `context` | `object` | Yes | Current payment notification context, usually `k.payment.square.context`. |

```ts
k.api.post(() => {
    const callback = k.payment.square.notify(k.payment.square.context)
    return { paid: callback.paid, status: callback.status }
})
```

**Return:** `PaymentCallback`

## updateOrder()

Handle Square callbacks and update orders.

**Parameters:**

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `context` | `object` | Yes | Current payment callback context, usually `k.payment.square.context`. |

```ts
k.api.post(() => {
    const callback = k.payment.square.updateOrder(k.payment.square.context)
    return { paid: callback.paid, status: callback.status }
})
```

**Return:** `PaymentCallback`

## SquareSetting

| Property | Type | Description |
|------|------|------|
| useSandBox | `boolean` | Whether to use sandbox |
| applicationId | `string` | Square Application ID |
| accessToken | `string` | Square Access Token |
| locationId | `string` | Square Location ID |
| baseURL | `string` | Square API address |
