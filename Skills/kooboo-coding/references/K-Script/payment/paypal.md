# PayPal Payment Guide

> PayPal payment reference for AI-generated code. Kooboo currently provides `k.payment.paypalCheckout` and `k.payment.paypalForm`.

## When to read this guide

- The user asks to integrate PayPal Checkout or PayPal form payment;
- you need to choose between a redirect checkout and an auto-submitting form;
- you need AI to output a minimal end-to-end implementation using the current Kooboo contract.

## Mode selection

| Scenario                       | Recommended method        | Return focus                 | Notes                     |
| ------------------------------- | -------------------------- | ---------------------------- | ------------------------- |
| Redirect to a PayPal Checkout page | `k.payment.paypalCheckout` | `redirectUrl` / `renderHtml` | |
| Auto-submit an HTML form            | `k.payment.paypalForm`     | `renderHtml`                 | Traditional PayPal form integration |

Default recommendation:

- Prefer `paypalCheckout` unless the user says otherwise;
- use `paypalForm` when the user explicitly requests form submission or compatibility with a legacy form integration.

## Pre-run configuration

PayPal is a multi-currency provider, but the final available currencies must still be the intersection of the site's configured currencies and `supportedCurrency`.

Confirm this before integration:

```ts
const configured = k.commerce.currency.list().map(item => item.code.toUpperCase())
const provider = k.payment.paypalCheckout.supportedCurrency.map(code => code.toUpperCase())
const available = provider.filter(code => configured.includes(code))
```

If an order must be paid in a non-default currency, set `currency` to the target currency when calling `k.commerce.order.create(...)` instead of changing the amount temporarily during payment.

### `paypalCheckout`

| Configuration | Description                  |
| ------------ | ------------------------------ |
| `clientId`   | PayPal client ID              |
| `secret`     | PayPal secret                 |
| `useSandBox` | Whether to use the sandbox; this also selects the runtime API URL |

### `paypalForm`

| Configuration  | Description         |
| -------------- | ------------------- |
| `emailAddress` | PayPal payment email       |
| `logoImage`    | Optional brand image       |
| `useSandBox`   | Whether to use the sandbox; this selects the form and IPN URLs |

## Kooboo built-in flow

Both `paypalCheckout` and `paypalForm` first create a built-in `PaymentRequest`. When `order` is provided:

- the commerce order is updated automatically after a successful callback;
- `callbackCodeName` can add business logic after payment completes;
- use `k.payment.getRequest(requestId)` and `paypalCheckout.checkStatus(requestId)` for troubleshooting. `paypalForm` relies on the verified IPN callback and does not support `checkStatus`.

## Minimal end-to-end flow: PayPal Checkout

This flow matches the live example in `kooboo_commerce_2/src/api/paypal-checkout.ts`.

```ts
//Api: /paypal-checkout/{orderid}
const orderId = k.request.orderid
const order = k.commerce.order.get(orderId)

const response = k.payment.paypalCheckout.charge({
    returnUrl: `/order/${orderId}`,
    cancelUrl: `/pay/${orderId}`,
    order: orderId,
    name: `kooboo order:${orderId}`,
    totalAmount: order.totalAmount,
    currency: order.currency,
    brandName: "Kooboo"
})

k.response.write(response.nextAction.renderHtml)
```

Notes:

- `renderHtml` is effectively a redirect script;
- you can also call `k.response.redirect(response.nextAction.redirectUrl)` directly;
- Do not add `customer` to this example. The current PayPal Checkout implementation exposes that legacy parameter in its schema but does not use it when creating the provider order.
- Prefer `order.currency` for `currency`; do not hand-write a currency that differs from the order during payment.

## Minimal end-to-end flow: PayPal Form

```html
<script env="server">
const orderId = k.request.orderid
const order = k.commerce.order.get(orderId)
const response = k.payment.paypalForm.charge({
    order: orderId,
    name: `kooboo order:${orderId}`,
    totalAmount: order.totalAmount,
    currency: order.currency,
    returnUrl: `/order/${orderId}`,
    cancelUrl: `/pay/${orderId}`
})
</script>

<div k-content="response.nextAction.renderHtml"></div>
```

`paypalForm` outputs an auto-submitting HTML form; the PayPal IPN callback performs asynchronous confirmation.

## Status-query example

```ts
k.api.get(() => {
    const requestId = k.request.queryString.get("requestId")
    const request = requestId ? k.payment.getRequest(requestId) : null
    const status = requestId ? k.payment.paypalCheckout.checkStatus(requestId) : null
    return { request, status }
})
```

## Common AI mistakes

1. Treating `paypalCheckout` and `paypalForm` as the same return shape. The former has `redirectUrl`; the latter is primarily form HTML.
2. Omitting `cancelUrl`, leaving users with no way back to the business page after canceling payment.
3. Omitting `order` when paying for a commerce order.
4. Treating a successful provider redirect as proof that the order is paid. Order confirmation must still come from the Kooboo callback / status.
5. Mechanically copying Stripe fields or comments into the PayPal example.
6. Hard-coding a currency in PayPal when that currency is not enabled for the site in the admin.
7. Calling `paypalForm.checkStatus(...)`; that method is not supported by the current form integration.
