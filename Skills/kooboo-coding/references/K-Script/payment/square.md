# Square Payment Guide

> Square payment reference for AI-generated code. The current Kooboo entry point is `k.payment.square`.

## When to read this guide

- The user asks to integrate Square Checkout;
- you need to confirm the return shape and currency limits of `k.payment.square`;
- you need AI to generate a minimal redirect-payment implementation.

## Applicable scenarios

`k.payment.square` implements the Square checkout-link / redirect flow and is suitable for:

- redirecting the user to the Square payment page after a click;
- returning to the site's order page after payment;
- reusing Kooboo's built-in callback and order update flow.

The current implementation supports only `USD`. This is a Square provider limitation, not a limitation preventing Kooboo commerce from supporting multiple currencies.

If the site already has multiple currencies configured:

- products, carts, and orders can still use other currencies for display;
- `k.payment.square` can be used only when the order's final currency is `USD`;
- if the business retains multiple providers, route currencies using `k.payment.square.supportedCurrency` and `k.commerce.currency.list()` first.

## Pre-run configuration

| Configuration | Description |
| --- | --- |
| `applicationId` | Square application ID stored in the site setting; the current Checkout client authenticates with `accessToken` |
| `accessToken` | Square access token used by the Checkout and order-status requests |
| `locationId` | Square location ID used to create the payment link and query the order |
| `useSandBox` | Whether to use the sandbox; this derives the runtime `baseURL` |

## Kooboo built-in flow

`k.payment.square.charge(...)` first creates a built-in `PaymentRequest`, then returns a checkout link. When `order` is provided:

- the order is updated through the callback and status-query flow after provider success;
- pass `order` in commerce scenarios rather than marking the order paid from a page query string.

## Minimal end-to-end example

```ts
//@k-url /square-checkout/{orderid}
const orderId = k.request.orderid
const order = k.commerce.order.get(orderId)

const response = k.payment.square.charge({
    order: orderId,
    totalAmount: order.totalAmount,
    currency: "USD",
    name: `kooboo order:${orderId}`,
    returnUrl: `/order/${orderId}`
})

k.response.redirect(response.nextAction.redirectUrl)
```

You can also write `response.nextAction.renderHtml` directly, but a direct redirect is the clearest Square implementation in most cases.

## Status-query example

```ts
k.api.get(() => {
    const requestId = k.request.queryString.get("requestId")
    const request = requestId ? k.payment.getRequest(requestId) : null
    const status = requestId ? k.payment.square.checkStatus(requestId) : null
    return { request, status }
})
```

## Common AI mistakes

1. Writing the property as `returnurl`; use `returnUrl` consistently.
2. Reusing another provider's currency and forgetting that Square currently supports only `USD`.
3. Forcing `renderHtml` into a complex page; a direct redirect is clearer for most Square scenarios.
4. Omitting `order` when paying for a commerce order.
5. Misstating Square's `USD`-only support as meaning Kooboo commerce can use only `USD`.
