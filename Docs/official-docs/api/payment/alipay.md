# alipay

> Alipay payment.

## Overview

Alipay payment includes three methods: App, web form and H5. Alipay application information needs to be configured in the site CMS before use.

| API | illustrate |
|-----|------|
| `k.payment.alipayApp` | Alipay App Payment |
| `k.payment.alipayForm` | Alipay web payment, usually rendered `nextAction.renderHtml` |
| `k.payment.alipayH5` | Alipay H5 payment |

## alipayApp.charge()

Create an Alipay App payment request.

```ts
k.api.post(() => {
    const result = k.payment.alipayApp.charge({
        name: 'Test Alipay App',
        description: 'Order payment',
        totalAmount: 1.5,
        currency: 'CNY',
        order: 'order-id',
        callbackCodeName: 'PaymentCallback'
    })

    return { requestId: result.requestId, paid: result.paid }
})
```

**Parameter:** `ChargeParams`

| parameter | type | Required | illustrate |
|------|------|------|------|
| totalAmount | `number` | yes | Order amount |
| name | `string` | yes | Order name |
| description | `string` | yes | Order description |
| currency | `string` | yes | Currency, such as `CNY` |
| order | `string` | no | Commerce order ID. Once the payment is successful, the order payment status will be updated. |
| callbackCodeName | `string` | no | The name of the callback code executed after payment is completed |

**Return:** `ChargeResponse`

## alipayForm.charge()

Create an Alipay web payment request.

```ts
k.api.get(() => {
    const result = k.payment.alipayForm.charge({
        name: 'Test Alipay Form',
        description: 'Order payment',
        totalAmount: 1.5,
        currency: 'CNY',
        order: 'order-id',
        returnUrl: '/payment/success?orderId=order-id',
        callbackCodeName: 'PaymentCallback'
    })

    return k.response.renderView(result.nextAction.renderHtml)
})
```

**Parameter:** `AlipayFormParams`

| parameter | type | Required | illustrate |
|------|------|------|------|
| returnUrl | `string` | yes | Jump address after payment is completed |
| totalAmount | `number` | yes | Order amount |
| name | `string` | yes | Order name |
| description | `string` | yes | Order description |
| currency | `string` | yes | Currency, such as `CNY` |
| order | `string` | no | Commerce order ID. Once the payment is successful, the order payment status will be updated. |
| callbackCodeName | `string` | no | The name of the callback code executed after payment is completed |

**Return:** `ChargeResponse`

## alipayH5.charge()

Create Alipay H5 payment request.

```ts
k.api.get(() => {
    const result = k.payment.alipayH5.charge({
        name: 'Test Alipay H5',
        description: 'Order payment',
        totalAmount: 1.5,
        currency: 'CNY',
        order: 'order-id',
        returnUrl: '/payment/success?orderId=order-id'
    })

    return k.response.renderView(result.nextAction.renderHtml)
})
```

**Parameter:** `AlipayFormParams`

| parameter | type | Required | illustrate |
|------|------|------|------|
| returnUrl | `string` | yes | Jump address after payment is completed |
| totalAmount | `number` | yes | Order amount |
| name | `string` | yes | Order name |
| description | `string` | yes | Order description |
| currency | `string` | yes | Currency, such as `CNY` |
| order | `string` | no | Commerce order ID. Once the payment is successful, the order payment status will be updated. |
| callbackCodeName | `string` | no | The name of the callback code executed after payment is completed |

**Return:** `ChargeResponse`

## checkStatus()

Check payment status.

```ts
k.api.get(() => {
    const status = k.payment.alipayForm.checkStatus('request-id')
    return { paid: status.paid, failed: status.failed, status: status.status }
})
```

**parameter:**

| parameter | type | Required | illustrate |
|------|------|------|------|
| requestId | `string` | yes | `charge()` returns `requestId` |

**Return:** `PaymentStatusResponse`

## updateOrder()

Handle payment callbacks and update orders. This method is only provided by `alipayForm`.

```ts
k.api.post(() => {
    const callback = k.payment.alipayForm.updateOrder(k.payment.alipayForm.context)
    return { paid: callback.paid, status: callback.status }
})
```

**parameter:**

| parameter | type | Required | illustrate |
|------|------|------|------|
| context | `object` | yes | Current payment callback context |

**Return:** `PaymentCallback`

## AlipaySetting

| property | type | illustrate |
|------|------|------|
| useSandBox | `boolean` | Whether to use a sandbox environment |
| appId | `string` | Alipay application ID |
| appPrivateKey | `string` | Apply private key |
| alipayPublicKey | `string` | Alipay public key |
| signType | `string` | signature type |
| charset | `string` | coding |
| serverUrl | `string` | Alipay gateway address |
