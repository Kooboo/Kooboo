# Stripe Payment Guide

> Stripe payment reference for AI-generated code. Kooboo currently provides `k.payment.stripeCheckout` and `k.payment.stripe`.

## When to read this guide

- The user asks to implement Stripe hosted checkout or on-site card payment;
- you need to distinguish `stripeCheckout` from `stripe`;
- you need AI to generate code that integrates 3DS, webhooks, or saved cards.

## Mode selection

| Scenario | Recommended method | Return focus | Notes |
| --- | --- | --- | --- |
| Hosted Checkout redirect page | `k.payment.stripeCheckout` | `redirectUrl` / `renderHtml` | Default choice with the lowest integration cost |
| On-site card payment, saved cards, 3DS | `k.payment.stripe` | `paid` or `nextAction.redirectUrl` | Requires handling the form and 3DS return yourself |

Default recommendation:

- Prefer `stripeCheckout` when the user only says “integrate Stripe payment”;
- choose `stripe` when on-site payment collection, card reuse, or saving customer cards is required.

## Pre-run configuration

Stripe is a multi-currency provider, but `stripeCheckout`'s `supportedCurrency` is not a fixed constant list. The implementation starts with Stripe's default currency set and narrows it by intersecting the site's `paymentMethods` settings.

This means:

- currencies not enabled in the admin should still not appear in the site's payment flow;
- enabling a specific payment method in Stripe Checkout settings may narrow the available currencies further;
- use the runtime `k.payment.stripeCheckout.supportedCurrency` for provider lists or currency switching instead of hand-writing a static array.

Confirm the available currencies first:

```ts
const configured = k.commerce.currency.list().map(item => item.code.toUpperCase())
const provider = k.payment.stripeCheckout.supportedCurrency.map(code => code.toUpperCase())
const available = provider.filter(code => configured.includes(code))
```

### `stripeCheckout`

| Configuration | Description |
| --- | --- |
| `secretKey` | Stripe secret key |
| `webhookSigningSecret` | Webhook signing secret |
| `paymentMethods` | Comma-separated list of allowed payment methods |

### `stripe`

| Configuration | Description |
| --- | --- |
| `secretKey` | Stripe secret key |
| `webhookSigningSecret` | Webhook signing secret |

## Kooboo built-in flow

Both Stripe methods first create a built-in `PaymentRequest`. In addition:

- `stripeCheckout` success and cancel pages return to the Kooboo callback URL before the order is updated;
- `stripe` triggers the callback immediately when `paid === true`; when 3DS is required, it proceeds through `nextAction.redirectUrl` for further authentication;
- when `order` is provided, the commerce order automatically calls `OrderService.Pay(...)` after payment completes.

## Minimal end-to-end flow: Stripe Hosted Checkout

This flow matches the live example in `kooboo_commerce_2/src/api/stripe-checkout.ts`.

```ts
//Api: /stripe-checkout/{orderid}
import { getCustomerId } from "code/accountService"

const orderId = k.request.orderid
const order = k.commerce.order.get(orderId)
const customerId = getCustomerId()

const response = k.payment.stripeCheckout.charge({
    returnUrl: `/order/${orderId}`,
    cancelUrl: `/pay/${orderId}`,
    order: orderId,
    name: `kooboo order:${orderId}`,
    totalAmount: order.totalAmount,
    currency: order.currency,
    customer: customerId
})

k.response.write(response.nextAction.renderHtml)
```

## Minimal end-to-end flow: on-site card payment

This flow matches the live example in `kooboo_commerce_2/src/view/pay/stripe.html`.

```ts
k.api.post((body) => {
    const response = k.payment.stripe.charge({
        order: body.orderId,
        customer: body.customerId,
        paymentMethodId: body.paymentMethodId,
        currency: body.currency,
        totalAmount: Number(body.totalAmount),
        returnUrl: `/order/${body.orderId}`
    })

    if (response.paid) {
        return { paid: true }
    }

    return {
        paid: false,
        nextAction: response.nextAction
    }
})
```

If no `paymentMethodId` is available, pass a `card` object and let Kooboo create the payment method. To save cards, prefer these helpers:

- `k.payment.stripe.getCardList(customerId)`
- `k.payment.stripe.createCard(customerId, card)`
- `k.payment.stripe.removeCard(cardId)`

Customer handling differs between the two Stripe methods:

- `stripeCheckout.customer` is a Kooboo customer key. Checkout creates or reuses the corresponding Stripe customer.
- `stripe.customer` uses an existing Kooboo-to-Stripe customer mapping. `createCard(customerId, card)` creates that mapping and attaches the card; passing `customer` together with a raw `card` to `stripe.charge(...)` does not save the new card for later reuse.

## 3DS / additional authentication

`k.payment.stripe.charge(...)` has two core return branches:

- `response.paid === true`: this request has completed payment;
- `response.nextAction?.redirectUrl`: redirect to the 3DS / provider next-step page.

Do not ignore the second case in the frontend.

```ts
if (response.paid) {
    location.href = `/order/${orderId}`
} else if (response.nextAction?.redirectUrl) {
    location.href = response.nextAction.redirectUrl
}
```

For a commerce order payment, keep `currency` consistent with `order.currency`. To change currency, rebuild the order during order creation instead.

## Status-query example

```ts
k.api.get(() => {
    const requestId = k.request.queryString.get("requestId")
    const request = requestId ? k.payment.getRequest(requestId) : null
    const status = requestId ? k.payment.stripeCheckout.checkStatus(requestId) : null
    return { request, status }
})
```

## Common AI mistakes

1. Generating the complex `k.payment.stripe` form when the user did not request on-site card payment; in most cases, prefer `stripeCheckout`.
2. Ignoring `nextAction.redirectUrl` from `k.payment.stripe`, which breaks the 3DS flow.
3. Forgetting `webhookSigningSecret`, preventing Stripe callbacks from being verified.
4. Treating `stripeCheckout`'s `renderHtml` as an HTML fragment that must be parsed; it is usually just a redirect script.
5. Treating `customer` plus a raw `card` charge as a card-saving flow. Use `createCard(customerId, card)` when the card must be attached for later reuse.
6. Looking only at Stripe's documented currencies without intersecting them with the site's configured currencies and runtime `supportedCurrency`.
