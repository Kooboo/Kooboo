# k.payment

> Third-party payment integration.

## Overview

`k.payment` provides capabilities such as payment request creation, payment status query, and payment request record reading. Before using it, you need to enable and configure the corresponding payment method in the site [CMS Service Integrations -> Payment](../../cms/settings/integrations/payment.md).

`charge()` will be written to the site's built-in **PaymentRequest**; if Commerce `order` is passed in, the built-in callback will update the order after the payment is successful. Don’t default to building your own set of parallel pay tables.

## Choosing a Payment Method

| User goal | Recommended API | `nextAction` form |
|-----------|-----------------|-------------------|
| WeChat scan code/PC QR code | `k.payment.wechat` | `renderHtml` |
| WeChat mobile browser H5 | `k.payment.weChatH5` | `redirectUrl` -> see [WeChat H5 Flow](./wechat-h5-flow.md) |
| WeChat built-in browser | `k.payment.wechatJsApi` | `responseData` (requires `openId`) |
| Alipay web checkout | `k.payment.alipayForm` | `renderHtml` |
| Alipay H5 | `k.payment.alipayH5` | `renderHtml` |
| PayPal Checkout | `k.payment.paypalCheckout` | `redirect` / `renderHtml` |
| Stripe hosting page | `k.payment.stripeCheckout` | `redirect` / `renderHtml` |
| Stripe on-site card payment | `k.payment.stripe` | `paid` or 3DS `redirectUrl` |

## Payment Method Pages

| Doc | API |
|-----|-----|
| [Alipay](./alipay.md) | `k.payment.alipayApp`, `k.payment.alipayForm`, `k.payment.alipayH5` |
| [WeChat Pay](./wechat.md) | `k.payment.wechat`, `k.payment.wechatApp`, `k.payment.weChatH5`, `k.payment.wechatJsApi` |
| [PayPal](./paypal.md) | `k.payment.paypalCheckout`, `k.payment.paypalForm` |
| [Stripe](./stripe.md) | `k.payment.stripeCheckout`, `k.payment.stripe` |
| [Square](./square.md) | `k.payment.square` |
| [Pay.NL](./paynl.md) | `k.payment.paynlCheckout` |
| [TwoCheckout](./two-checkout.md) | `k.payment.twoCheckout` |
| [MoneyBoxs](./moneyboxs.md) | `k.payment.moneyBoxs` |

## Multi-Currency Note

`k.commerce.currency.list()` returns the site's configured currency; each provider also has `supportedCurrency`. The intersection of the two should be taken before initiating payment. WeChat/Alipay usually only **CNY**.

## getRequest()

Get the payment request record based on `requestId`.

```ts
k.api.get(() => {
    const request = k.payment.getRequest('request-id')
    return {
        id: request.id,
        paid: request.paid,
        failed: request.failed,
        paymentMethod: request.paymentMethod,
        order: request.order
    }
})
```

**Parameters**

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| requestId | `string` | Yes | Payment request ID returned by `charge()` |

**Return:** `PaymentRequest`

## Common Properties

Each payment method object contains the following basic properties.

| Property | Type | Description |
|----------|------|-------------|
| name | `string` | Internal payment method name |
| displayName | `string` | Display name |
| icon | `string` | Icon URL or inline content |
| iconType | `string` | Icon type |
| supportedCurrency | `string[]` | Supported currencies |
| context | `object` | Current payment context |
| setting | `object` | Site configuration for this payment method |

## General process

```ts
k.api.get(() => {
    const product = k.commerce.product.get('test-product')
    const cartId = k.commerce.cart.create()

    k.commerce.cart.addOrUpdateLine(cartId, product.variants[0].id, 1)
    k.commerce.cart.updateContact(cartId, 'customer@example.com')

    const order = k.commerce.order.create(cartId, {
        address: { firstName: 'Test', phone: '13800138000' } as any
    })

    const charge = k.payment.alipayForm.charge({
        name: product.title,
        description: 'Order payment',
        totalAmount: order.totalAmount,
        currency: 'CNY',
        order: order.id,
        returnUrl: `/__paymentCallback?orderId=${order.id}`,
        callbackCodeName: 'PaymentCallback'
    })

    k.logger.information('Payment.Info', `requestId: ${charge.requestId}, orderId: ${order.id}`)
    return k.response.renderView(charge.nextAction.renderHtml)
})
```

## Payment callback example

`returnUrl` is the jump address after payment is completed. You can carry the order ID in the address, and then use the saved `requestId` to query the real payment status.

```ts
k.api.get(() => {
    const orderId = k.request.queryString.get("orderId")
    if (!orderId) {
        k.response.json({ code: -1, msg: 'order ID is not found' })
        return k.api.httpCode(400)
    }

    const requestId = getRequestIdByOrderId(orderId)
    if (!requestId) {
        k.response.json({ code: -1, msg: 'payment request ID is not found' })
        return k.api.httpCode(400)
    }

    const status = k.payment.alipayForm.checkStatus(requestId)
    if (status?.paid) {
        return k.response.redirect(`/order/${orderId}`)
    }

    return k.response.redirect('/payment-failed')
})
```

## ChargeResponse

| property | type | illustrate |
|------|------|------|
| paid | `boolean` | Has it been paid? |
| requestId | `any` | Payment request ID, used when querying the status later. |
| paymentMethodReferenceId | `string` | Third-party payment side reference ID |
| nextAction | `NextAction` | Next action |

## NextAction

| property | type | illustrate |
|------|------|------|
| type | `string` | Action type, such as jump, render HTML, QR code, etc. |
| redirectUrl | `string` | Target address when redirection is required |
| renderHtml | `string` | The HTML that needs to be rendered to the page |
| responseData | `string` | Original response data returned by a third party, often used in QR codes or client SDK parameters |

## PaymentStatusResponse

| property | type | illustrate |
|------|------|------|
| hasResult | `boolean` | Are there any query results? |
| paid | `boolean` | Whether the payment was successful |
| failed | `boolean` | Whether payment failed |
| status | `PaymentStatus` | Payment status |
| message | `string` | status message |

`PaymentStatus` can be: `NotAvailable`, `Authorized`, `Pending`, `Paid`, `Cancelled`, `Rejected`.

## PaymentRequest

| property | type | illustrate |
|------|------|------|
| ID | `any` | Payment request ID |
| description | `string` | Payment description |
| totalAmount | `number` | Amount |
| currency | `string` | currency |
| order | `string` | Associated Commerce order ID |
| paymentMethod | `string` | Payment method |
| paid | `boolean` | Has it been paid? |
| failed | `boolean` | whether failed |
| callbackCodeName | `string` | The name of the callback code executed after payment is completed |
| referenceId | `string` | Third Party Reference ID |
| returnUrl | `string` | Payment completion jump address |
| cancelUrl | `string` | Cancel payment redirect address |
| additional | `Record<string, any>` | Payment method additional data |
| card | `Card` | Card information |
| creationDate | `Date` | creation time |
| lastModified | `Date` | last modified time |
| name | `string` | Payment name |
