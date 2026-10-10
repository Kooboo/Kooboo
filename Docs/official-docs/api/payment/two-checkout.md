# two-checkout

> TwoCheckout payment.

## Overview

`k.payment.twoCheckout` is used to create TwoCheckout payment requests, supporting card information and 3D verification jumps. TwoCheckout merchant information needs to be configured in the site CMS before use.

## twoCheckout.charge()

Create a TwoCheckout payment request.

```ts
k.api.post(() => {
    const result = k.payment.twoCheckout.charge({
        name: 'TEST order',
        description: 'Order payment',
        totalAmount: 100,
        currency: 'USD',
        order: 'order-id',
        returnUrl: '/payment/success?orderId=order-id',
        cancelUrl: '/payment/cancel?orderId=order-id',
        card: {
            number: '378282246310005',
            expMonth: '03',
            expYear: '2030',
            cvc: '123',
            name: 'John Doe'
        },
        cardType: 'Amex',
        billingAddress: {
            firstName: 'Customer',
            lastName: '2Checkout',
            countryCode: 'US',
            state: 'California',
            city: 'LA',
            address1: 'Test Address',
            zip: '12345',
            email: 'testcustomer@2checkout.com',
            phone: '13800138000'
        }
    })

    if (result.nextAction?.redirectUrl) {
        return k.response.redirect(result.nextAction.redirectUrl)
    }

    return { paid: result.paid, requestId: result.requestId }
})
```

**Parameter:** `TwoCheckoutParams`

| Parameter | Type | Required | Description |
|------|------|------|------|
| cancelUrl | `string` | yes | Jump address after cancellation of 3D verification |
| returnUrl | `string` | yes | Jump address after successful 3D verification |
| card | `Card` | yes | Card information |
| cardType | `string` | yes | Card type, such as `Visa`, `Mastercard`, `Amex` |
| billingAddress | `Address` | yes | billing address |
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
    const status = k.payment.twoCheckout.checkStatus('request-id')
    return { paid: status.paid, failed: status.failed, status: status.status }
})
```

**Return:** `PaymentStatusResponse`

## updateOrder()

Handle the TwoCheckout callback and update the order.

**Parameters:**

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `context` | `object` | Yes | Current payment callback context, usually `k.payment.twoCheckout.context`. |

```ts
k.api.post(() => {
    const callback = k.payment.twoCheckout.updateOrder(k.payment.twoCheckout.context)
    return { paid: callback.paid, status: callback.status }
})
```

**Return:** `PaymentCallback`

## Card

| Property | Type | Description |
|------|------|------|
| number | `string` | card number |
| cvc | `string` | CVC |
| expMonth | `string` | Expiration month |
| expYear | `string` | Expiration year |
| name | `string` | Cardholder name |

## Address

| Property | Type | Description |
|------|------|------|
| firstName | `string` | name |
| lastName | `string` | surname |
| countryCode | `string` | country code |
| state | `string` | state or province |
| city | `string` | City |
| address1 | `string` | address |
| zip | `string` | post code |
| email | `string` | Mail |
| phone | `string` | Telephone |

## TwoCheckoutSetting

| Property | Type | Description |
|------|------|------|
| baseUrl | `string` | TwoCheckout interface address |
| merchantCode | `string` | Merchant Code |
| secretKey | `string` | Secret Key |
| testMode | `boolean` | Whether to test mode |
