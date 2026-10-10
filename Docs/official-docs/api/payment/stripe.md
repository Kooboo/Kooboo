# stripe

> Stripe payments.

## Overview

Stripe payment includes Checkout and PaymentIntent. The Stripe key and Webhook signing key need to be configured in the site CMS before use.

| API | illustrate |
|-----|------|
| `k.payment.stripeCheckout` | Stripe Checkout hosted payment page |
| `k.payment.stripe` | Stripe PaymentIntent and card management |

## stripeCheckout.charge()

Create a Stripe Checkout payment request.

```ts
k.api.get(() => {
    const result = k.payment.stripeCheckout.charge({
        name: 'T-shirt',
        description: 'Order payment',
        totalAmount: 1.5,
        currency: 'USD',
        order: 'order-id',
        returnUrl: '/payment/success?orderId=order-id',
        cancelUrl: '/payment/cancel?orderId=order-id',
        customer: 'customer@example.com',
        callbackCodeName: 'PaymentCallback'
    })

    return k.response.renderView(result.nextAction.renderHtml)
})
```

**Parameter:** `StripeCheckoutParams`

| parameter | type | Required | illustrate |
|------|------|------|------|
| cancelUrl | `string` | yes | Checkout return button or cancel payment address |
| returnUrl | `string` | yes | Return address after successful payment |
| customer | `string` | no | Customer ID. Provided to automatically create a Stripe Customer |
| totalAmount | `number` | yes | Order amount |
| name | `string` | yes | Order name |
| description | `string` | yes | Order description |
| currency | `string` | yes | Currency, such as `USD`, `EUR` |
| order | `string` | no | Commerce order ID. Once the payment is successful, the order payment status will be updated. |
| callbackCodeName | `string` | no | The name of the callback code executed after payment is completed |

**Return:** `ChargeResponse`

## stripe.charge()

Create a Stripe PaymentIntent payment request.

```ts
k.api.post(() => {
    const result = k.payment.stripe.charge({
        name: 'T-shirt',
        description: 'Order payment',
        totalAmount: 1.5,
        currency: 'USD',
        order: 'order-id',
        returnUrl: '/payment/success?orderId=order-id',
        customer: 'customer-id',
        paymentMethodId: 'payment-method-id',
        card: {
            number: '4242424242424242',
            expMonth: '12',
            expYear: '2030',
            cvc: '123',
            name: 'John Doe'
        }
    })

    return { requestId: result.requestId, nextAction: result.nextAction }
})
```

**Parameter:** `StripePaymentIntentParams`

| parameter | type | Required | illustrate |
|------|------|------|------|
| returnUrl | `string` | yes | Return address after payment is completed |
| card | `Card` | no | Card information |
| paymentMethodId | `string` | no | Saved Stripe PaymentMethod ID |
| customer | `string` | no | Stripe Customer ID or customer identification |
| totalAmount | `number` | yes | Order amount |
| name | `string` | yes | Order name |
| description | `string` | yes | Order description |
| currency | `string` | yes | Currency, such as `USD`, `EUR` |
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
    const status = k.payment.stripeCheckout.checkStatus('request-id')
    return { paid: status.paid, failed: status.failed, status: status.status }
})
```

**Return:** `PaymentStatusResponse`

## getCardList()

Get the list of cards saved by the specified customer.

```ts
k.api.get(() => {
    return k.payment.stripe.getCardList('customer-id')
})
```

**parameter:**

| parameter | type | Required | illustrate |
|------|------|------|------|
| customer | `string` | yes | Stripe Customer ID |

**Return:** `any`

## createCard()

Create cards for customers.

**Parameters:**

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `customer` | `string` | Yes | Stripe Customer ID. |
| `card` | `Card` | Yes | Card information to save. |

```ts
k.api.post(() => {
    return k.payment.stripe.createCard('customer-id', {
        number: '4242424242424242',
        expMonth: '12',
        expYear: '2030',
        cvc: '123',
        name: 'John Doe'
    })
})
```

**Return:** `string`

## removeCard()

Remove the card from the customer.

**Parameters:**

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `cardId` | `string` | Yes | Stripe card ID to remove. |

```ts
k.api.post(() => {
    k.payment.stripe.removeCard('card-id')
    return 'success'
})
```

**Return:** `void`

## updateOrder()

Handle Stripe Checkout callbacks and update orders.

**Parameters:**

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `context` | `object` | Yes | Current payment callback context, usually `k.payment.stripeCheckout.context`. |

```ts
k.api.post(() => {
    const callback = k.payment.stripeCheckout.updateOrder(k.payment.stripeCheckout.context)
    return { paid: callback.paid, status: callback.status }
})
```

**Return:** `PaymentCallback`

## Card

| property | type | illustrate |
|------|------|------|
| number | `string` | card number |
| cvc | `string` | CVC |
| expMonth | `string` | Expiration month |
| expYear | `string` | Expiration year |
| name | `string` | Cardholder name |

## StripeSetting

| property | type | illustrate |
|------|------|------|
| secretKey | `string` | Stripe Secret Key |
| webhookSigningSecret | `string` | Webhook signing key |
