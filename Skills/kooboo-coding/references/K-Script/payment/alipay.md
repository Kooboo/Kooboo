# Alipay Payment Guide

> Alipay payment reference for AI-generated code. Kooboo currently provides `k.payment.alipayForm`, `k.payment.alipayH5`, and `k.payment.alipayApp`.

## When to read this guide

- The user asks to integrate Alipay web, H5, or App payment;
- you need to know how to consume `renderHtml` and `responseData`;
- you need AI to output a minimal end-to-end implementation using only Kooboo's built-in payment capabilities.

## Mode selection

| Scenario | Recommended method | Key prerequisite | Return focus |
| --- | --- | --- | --- |
| Site web checkout, PC browser payment | `k.payment.alipayForm` | The page can output HTML directly | `nextAction.renderHtml` |
| Mobile-browser H5 payment | `k.payment.alipayH5` | Running in a mobile H5 scenario | `nextAction.renderHtml` |
| Alipay SDK payment from a native App | `k.payment.alipayApp` | The App can consume the signed string | `nextAction.responseData` |

Default recommendation:

- For payment on a regular site page, prefer `alipayForm`;
- choose `alipayH5` only when the scenario is explicitly a mobile browser;
- output `alipayApp` only when the user explicitly says “App payment.”

## Pre-run configuration

Alipay requires at least these site settings before running:

| Configuration | Description |
| --- | --- |
| `appId` | Alipay application ID |
| `appPrivateKey` | Merchant application private key |
| `alipayPublicKey` | Alipay public key |
| `useSandBox` | Whether to use the sandbox; this derives the runtime `serverUrl` |

The current runtime fixes `signType` to `RSA2` and `charset` to `UTF-8`; they are exposed as read-only runtime properties rather than independent site settings.

`alipay*` currently supports only `CNY`. This is an Alipay provider limitation, not a limitation of Kooboo commerce's multi-currency capability.

AI must not guess the Payment admin URL or use generic `show_action_button` to fake an Alipay configuration shortcut. When there is no dedicated, verified configuration entry point, state that the user must configure Payment manually under the admin service integration and retain the item as incomplete evidence.

If the site already has multiple currencies configured, do three things first:

1. use `k.commerce.currency.list()` to confirm the currencies actually enabled in the admin;
2. use `k.payment.alipayForm.supportedCurrency` or `k.payment.alipayH5.supportedCurrency` to confirm the provider's supported range;
3. if the current order is not `CNY`, decide whether to use another provider or create a `CNY` order during order creation.

## Kooboo built-in flow

`k.payment.alipay*.charge(...)` first creates a built-in site `PaymentRequest`. When `order` is provided:

- after successful payment, Kooboo automatically marks the commerce order paid through the callback worker;
- when `callbackCodeName` is provided, the corresponding CodeBlock runs during the callback;
- troubleshoot later with `k.payment.getRequest(requestId)` and `alipayForm.checkStatus(requestId)` / `alipayH5.checkStatus(requestId)`.

Do not treat “store a separate payment-request table” as the default path.

## Minimal end-to-end flow: web form payment endpoint

For a PC or ordinary web checkout. After checkout succeeds, navigate the browser directly to this Payment API/CodeBlock. The server writes the provider form directly; do not use `<script env="server">` in a View or return JSON for client-side `innerHTML` injection.

```ts
k.api.get((orderId) => {
    const customerId = k.commerce.customer.currentId || ''
    if (!customerId || !orderId) return k.api.notFound()

    const order = k.commerce.order.get(orderId)
    if (!order?.customer?.id || order.customer.id !== customerId) {
        return k.api.notFound()
    }
    if (String(order.currency || '').toUpperCase() !== 'CNY') {
        return k.api.httpCode(400, { error: 'Alipay requires a CNY order' })
    }

    const response = k.payment.alipayForm.charge({
        order: order.id,
        name: `Kooboo order:${order.id}`,
        totalAmount: order.totalAmount,
        currency: 'CNY',
        returnUrl: `/order-result?orderId=${encodeURIComponent(order.id)}`
    })
    if (!response?.requestId || !response?.nextAction?.renderHtml) {
        return k.api.httpCode(502, { error: 'Payment provider did not return a form' })
    }

    k.cookie.set(`payment_request_${order.id}`, response.requestId, 1)
    k.response.write(response.nextAction.renderHtml)
    return null
})
```

Key points:

- The browser opens the payment endpoint directly, and the endpoint writes `renderHtml` through `k.response.write`;
- `returnUrl` is the business return page; the actual provider callback is still handled by Kooboo's built-in callback URL;
- remember to pass `order` for commerce scenarios.
- verify before payment that the order belongs to `customer.currentId`; do not trust only the URL/body orderId;
- retain `response.requestId` for pending troubleshooting or compensation queries;
- write `renderHtml` directly in the server response. Do not inject it with client-side `innerHTML` and expect embedded scripts to execute;
- if `order.currency` is not `CNY`, do not send the order directly to Alipay; route the currency first.

## Minimal end-to-end flow: H5 payment endpoint

For mobile browsers.

```ts
k.api.get((orderId) => {
    const customerId = k.commerce.customer.currentId || ''
    const order = orderId ? k.commerce.order.get(orderId) : null
    if (!customerId || !order?.customer?.id || order.customer.id !== customerId) {
        return k.api.notFound()
    }
    if (String(order.currency || '').toUpperCase() !== 'CNY') {
        return k.api.httpCode(400, { error: 'Alipay requires a CNY order' })
    }

    const response = k.payment.alipayH5.charge({
        order: order.id,
        name: `Kooboo order:${order.id}`,
        totalAmount: order.totalAmount,
        currency: 'CNY',
        returnUrl: `/order-result?orderId=${encodeURIComponent(order.id)}`
    })
    if (!response?.requestId || !response?.nextAction?.renderHtml) {
        return k.api.httpCode(502, { error: 'Payment provider did not return a form' })
    }

    k.cookie.set(`payment_request_${order.id}`, response.requestId, 1)
    k.response.write(response.nextAction.renderHtml)
    return null
})
```

`alipayH5` still returns an HTML form, not `redirectUrl`.

## Minimal end-to-end flow: App payment

For native Apps. Do not copy this section directly into a web client.

```ts
k.api.post((body) => {
    const response = k.payment.alipayApp.charge({
        order: body.orderId,
        name: body.name,
        totalAmount: Number(body.totalAmount),
        currency: "CNY",
        description: body.description
    })

    return {
        requestId: response.requestId,
        orderString: response.nextAction?.responseData
    }
})
```

The Alipay SDK on the App must consume `orderString` next.

## Status-query example

```ts
k.api.get(() => {
    const requestId = k.request.queryString.get("requestId")
    const request = requestId ? k.payment.getRequest(requestId) : null
    const status = requestId ? k.payment.alipayForm.checkStatus(requestId) : null
    return { request, status }
})
```

Use `checkStatus(...)` for troubleshooting and compensation; final order updates still rely on the provider callback and Kooboo callback worker.

If the provider shows payment success but the Commerce Order is still pending, inspect `PaymentRequest`, the provider callback/notify, and `checkStatus` using the same `requestId`. Do not continue to explain the problem as “the key is not configured” without evidence.

Current runtime flow:

1. `charge` creates and saves `PaymentRequest`, returning `requestId` and `renderHtml`;
2. provider notify enters `AlipayBase.Notify` and verifies the Alipay signature. The current implementation accepts the callback when either the amount matches the stored request or the subject matches the stored name; this is an `OR` condition, not strict validation of both fields. Do not describe it as dual validation or treat it as the only fulfillment authorization boundary;
3. `alipayForm` provider return enters `AlipayForm.UpdateOrder`, calls `CheckStatus` by `requestId`, and then redirects to the business `returnUrl`; `alipayH5` returns to its supplied business URL, while `alipayApp` has no browser return path and relies on provider notify;
4. the callback worker updates `PaymentRequest` and idempotently updates the Commerce Order when the request includes `order`;
5. KScript `alipayForm.checkStatus(requestId)` / `alipayH5.checkStatus(requestId)` uses the same callback worker; do not create a separate “payment status table.”

For web and H5 flows, `returnUrl` only indicates that the browser returned to the site. The result page must read the real order state again:

- `paid`: show success;
- provider/query still pending: show processing and a retry-query entry point;
- failed/canceled: show failure or cancellation and allow a new valid payment attempt;
- no requestId or order not found: show that the result cannot be confirmed; never default to success.

## Common AI mistakes

1. Treating `alipayForm` or `alipayH5` as a `redirectUrl` API; both primarily return `renderHtml`.
2. Using `alipayApp` on a website page; `alipayApp` returns a signed string for the App SDK.
3. Omitting `order`, preventing the commerce order from being updated automatically by the built-in callback.
4. Treating `returnUrl` as the provider callback address; Kooboo builds the actual provider notify URL automatically.
5. Using a non-`CNY` currency for Alipay or mistakenly interpreting this as a single-currency limitation of Kooboo commerce.
6. Returning `renderHtml` from a client API and injecting it with `innerHTML`; scripts inside the form usually will not execute as expected. Navigate to a server-side payment page that writes it directly.
7. Using an old cartId during checkout or allowing session/cart drift, causing cart/order ownership mismatch. `customer.login()` normally binds the current cart; do not rebuild the cart by default. First trace state sources using [Commerce checkout and payment golden path](../../Scenario/commerce-checkout-payment-flow.md).
8. Claiming payment completed when the key is not configured; tell the user to configure the Payment integration and retain incomplete evidence.
9. Guessing that the key is missing when the provider succeeded but Commerce is still pending; inspect the same `requestId`'s PaymentRequest, callback/notify, and `checkStatus`.
10. Describing Alipay callback validation as requiring both amount and subject when the current runtime accepts either one; keep fulfillment authorization stricter than this provider callback condition.
