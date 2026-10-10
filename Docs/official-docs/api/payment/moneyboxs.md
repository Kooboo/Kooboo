# moneyboxes

> MoneyBoxes payments.

## Overview

`k.payment.moneyBoxs` is used to create MoneyBoxes payment requests, supporting card information, billing address, shipping address and item details. MoneyBoxs merchant information needs to be configured in the site CMS before use.

## moneyBoxes.charge()

Create a MoneyBoxs payment request.

```ts
k.api.post(() => {
    const result = k.payment.moneyBoxs.charge({
        name: 'TEST order',
        description: 'Order payment',
        totalAmount: 100,
        currency: 'USD',
        order: 'order-id',
        returnUrl: '/payment/success?orderId=order-id',
        card: {
            number: '378282246310005',
            expMonth: '03',
            expYear: '2030',
            cvc: '123',
            name: 'John Doe'
        },
        billingAddress: {
            firstName: 'Customer',
            lastName: 'MoneyBoxs',
            countryCode: 'US',
            state: 'California',
            city: 'LA',
            address1: 'Test Address',
            zip: '12345',
            email: 'testcustomer@example.com',
            phone: '13800138000'
        },
        shipAddress: {
            firstName: 'Customer',
            lastName: 'MoneyBoxs',
            countryCode: 'US',
            state: 'California',
            city: 'LA',
            address1: 'Test Address',
            zip: '12345',
            email: 'testcustomer@example.com',
            phone: '13800138000'
        },
        goods: [
            { goodsName: 'T-shirt', quantity: '1', goodsPrice: '100' }
        ]
    })

    if (result.nextAction?.redirectUrl) {
        return k.response.redirect(result.nextAction.redirectUrl)
    }

    return { paid: result.paid, requestId: result.requestId }
})
```

**Parameter:** `MoneyBoxsParams`

| parameter | type | Required | illustrate |
|------|------|------|------|
| returnUrl | `string` | yes | Jump address after successful 3D verification |
| card | `Card` | yes | Card information |
| billingAddress | `Address` | yes | billing address |
| shipAddress | `Address` | yes | Shipping address |
| goods | `Good[]` | yes | Product details |
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
    const status = k.payment.moneyBoxs.checkStatus('request-id')
    return { paid: status.paid, failed: status.failed, status: status.status }
})
```

**Return:** `PaymentStatusResponse`

## updateOrder()

Handle MoneyBoxs callbacks and update orders.

**Parameters:**

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `context` | `object` | Yes | Current payment callback context, usually `k.payment.moneyBoxs.context`. |

```ts
k.api.post(() => {
    const callback = k.payment.moneyBoxs.updateOrder(k.payment.moneyBoxs.context)
    return { paid: callback.paid, status: callback.status }
})
```

**Return:** `PaymentCallback`

## verify3DAuthCallback()

Verify 3D Auth callback.

```ts
k.api.post(() => {
    k.payment.moneyBoxs.verify3DAuthCallback(
        'orderNo',
        'merchantNo',
        'terminalNo',
        '100',
        'hashCode',
        '00'
    )
    return 'success'
})
```

**parameter:**

| parameter | type | Required | illustrate |
|------|------|------|------|
| orderNo | `string` | yes | Order number |
| merchantNo | `string` | yes | Merchant number |
| terminalNo | `string` | yes | terminal number |
| amount | `string` | yes | Amount |
| hashCode | `string` | yes | Callback Hash |
| respCode | `string` | yes | response code |

**Return:** `void`

## Good

| property | type | illustrate |
|------|------|------|
| goodsName | `string` | Product name |
| quantity | `string` | quantity |
| goodsPrice | `string` | Product price |

## MoneyBoxesSetting

| property | type | illustrate |
|------|------|------|
| baseUrl | `string` | MoneyBoxs API address |
| merchantNo | `string` | Merchant number |
| terminalNo | `string` | terminal number |
| key | `string` | key |
| transactionURL | `string` | Transaction address |
| type | `string` | type |
