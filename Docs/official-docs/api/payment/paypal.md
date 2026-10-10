# paypal

> PayPal payment.

## Overview

PayPal payment includes Checkout and Form. PayPal account or application information needs to be configured in the site CMS before use.

| API | illustrate |
|-----|------|
| `k.payment.paypalCheckout` | PayPal Checkout |
| `k.payment.paypalForm` | PayPal form payment |

## paypalCheckout.charge()

Create a PayPal Checkout payment request.

```ts
k.api.get(() => {
    const result = k.payment.paypalCheckout.charge({
        name: 'T-shirt',
        description: 'Order payment',
        totalAmount: 1.5,
        currency: 'USD',
        order: 'order-id',
        returnUrl: '/payment/success?orderId=order-id',
        cancelUrl: '/payment/cancel?orderId=order-id',
        brandName: 'Kooboo Store',
        customer: 'customer@example.com',
        callbackCodeName: 'PaymentCallback'
    })

    return k.response.renderView(result.nextAction.renderHtml)
})
```

**Parameter:** `PaypalCheckoutParams`

| parameter | type | Required | illustrate |
|------|------|------|------|
| cancelUrl | `string` | yes | Return address when canceling payment |
| returnUrl | `string` | yes | Return address after successful payment |
| brandName | `string` | no | The brand name displayed on the PayPal payment page |
| customer | `string` | no | Customer ID |
| totalAmount | `number` | yes | Order amount |
| name | `string` | yes | Order name |
| description | `string` | yes | Order description |
| currency | `string` | yes | Currency, such as `USD` |
| order | `string` | no | Commerce order ID. Once the payment is successful, the order payment status will be updated. |
| callbackCodeName | `string` | no | The name of the callback code executed after payment is completed |

**Return:** `ChargeResponse`

## paypalForm.charge()

Create a PayPal form payment request.

```ts
k.api.get(() => {
    const result = k.payment.paypalForm.charge({
        name: 'T-shirt',
        description: 'Order payment',
        totalAmount: 1.5,
        currency: 'USD',
        order: 'order-id',
        returnUrl: '/payment/success?orderId=order-id',
        cancelUrl: '/payment/cancel?orderId=order-id'
    })

    return k.response.renderView(result.nextAction.renderHtml)
})
```

**Parameter:** `PaypalFormParams`

| parameter | type | Required | illustrate |
|------|------|------|------|
| cancelUrl | `string` | yes | Return address when canceling payment |
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

```ts
k.api.get(() => {
    const status = k.payment.paypalCheckout.checkStatus('request-id')
    return { paid: status.paid, failed: status.failed, status: status.status }
})
```

**parameter:**

| parameter | type | Required | illustrate |
|------|------|------|------|
| requestId | `string` | yes | `charge()` returns `requestId` |

**Return:** `PaymentStatusResponse`

## updateOrder()

Handle payment callbacks and update orders. `paypalCheckout` provides this method.

**Parameters:**

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `context` | `object` | Yes | Current payment callback context, usually `k.payment.paypalCheckout.context`. |

```ts
k.api.post(() => {
    const callback = k.payment.paypalCheckout.updateOrder(k.payment.paypalCheckout.context)
    return { paid: callback.paid, status: callback.status }
})
```

**Return:** `PaymentCallback`

## nofityUrl()

Handle PayPal Form notification callbacks. The method name is `nofityUrl` in the current type definition.

**Parameters:**

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `context` | `object` | Yes | Current PayPal Form notification context, usually `k.payment.paypalForm.context`. |

```ts
k.api.post(() => {
    const callback = k.payment.paypalForm.nofityUrl(k.payment.paypalForm.context)
    return { paid: callback.paid, status: callback.status }
})
```

**Return:** `PaymentCallback`

## Configuration structure

### PaypalSetting

| property | type | illustrate |
|------|------|------|
| paypalUrl | `string` | PayPal interface address |
| useSandBox | `boolean` | Whether to use sandbox |
| clientId | `string` | PayPal Client ID |
| secret | `string` | PayPal Secret |

### PaypalFormSetting

| property | type | illustrate |
|------|------|------|
| paypalUrl | `string` | PayPal interface address |
| iPNUrl | `string` | IPN notification address |
| emailAddress | `string` | Payment account email |
| logoImage | `string` | Payment page logo |
| useSandBox | `boolean` | Whether to use sandbox |
