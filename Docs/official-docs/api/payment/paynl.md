# paynl

> Pay.NL Pay.

## Overview

`k.payment.paynlCheckout` is used to create a Pay.NL Checkout payment request. Pay.NL point of sale and Token information need to be configured in the site CMS before use.

## paynlCheckout.charge()

Create a Pay.NL Checkout payment request.

```ts
k.api.get(() => {
    const result = k.payment.paynlCheckout.charge({
        name: 'T-shirt',
        description: 'Order payment',
        totalAmount: 1.5,
        currency: 'EUR',
        order: 'order-id',
        returnUrl: '/payment/success?orderId=order-id',
        exchangeUrl: '/payment/paynl/exchange',
        statsData: {
            extra1: 'order-id',
            extra2: '',
            extra3: ''
        },
        callbackCodeName: 'PaymentCallback'
    })

    return k.response.renderView(result.nextAction.renderHtml)
})
```

**Parameter:** `PaynlCheckoutParams`

| Parameter | Type | Required | Description |
|------|------|------|------|
| returnUrl | `string` | yes | Return address after successful payment |
| exchangeUrl | `string` | yes | Pay.NL The address used to exchange transaction status |
| statsData | `StatsData` | no | Statistics extended data |
| totalAmount | `number` | yes | Order amount |
| name | `string` | yes | Order name |
| description | `string` | yes | Order description |
| currency | `string` | yes | Currency, such as `EUR` |
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
    const status = k.payment.paynlCheckout.checkStatus('request-id')
    return { paid: status.paid, failed: status.failed, status: status.status }
})
```

**Return:** `PaymentStatusResponse`

## updateOrder()

Handle Pay.NL callbacks and update orders.

**Parameters:**

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `context` | `object` | Yes | Current payment callback context, usually `k.payment.paynlCheckout.context`. |

```ts
k.api.post(() => {
    const callback = k.payment.paynlCheckout.updateOrder(k.payment.paynlCheckout.context)
    return { paid: callback.paid, status: callback.status }
})
```

**Return:** `PaymentCallback`

## StatsData

| Property | Type | Description |
|------|------|------|
| extra1 | `string` | Extended field 1 |
| extra2 | `string` | Extended field 2 |
| extra3 | `string` | Extended field 3 |

## PaynlSetting

| Property | Type | Description |
|------|------|------|
| paynlUrl | `string` | Pay.NL interface address |
| testMode | `boolean` | Whether to test mode |
| salesLocationCode | `string` | Point of sale code |
| salesLocationSecret | `string` | Point of sale Secret |
| code | `string` | Configure Code |
| token | `string` | Token |
