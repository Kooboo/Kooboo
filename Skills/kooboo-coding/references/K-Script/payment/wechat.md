# WeChat Payment Guide

> WeChat payment reference for AI-generated code. Kooboo currently provides `k.payment.wechat`, `k.payment.weChatH5`, `k.payment.wechatJsApi`, and `k.payment.wechatApp`.

## When to read this guide

- The user asks to implement WeChat Pay;
- you need to choose QR, H5, JSAPI, or App payment;
- you need AI to output code without mixing WeChat payment modes.

## Mode selection

| Scenario | Recommended method | Key prerequisite | Return focus |
| --- | --- | --- | --- |
| PC web QR payment, ordinary web QR payment | `k.payment.wechat` | The page can render a QR code directly | `nextAction.renderHtml` / `responseData` |
| Mobile-browser H5 payment | `k.payment.weChatH5` | The environment is explicitly mobile H5 | `nextAction.redirectUrl` |
| JSAPI payment inside the WeChat browser | `k.payment.wechatJsApi` | The WeChat user's `openId` is available | `nextAction.responseData` |
| Native App payment | `k.payment.wechatApp` | The App can consume `prepay_id` | `nextAction.responseData` |

Default recommendation:

- When the user only says “WeChat Pay,” prefer the Native QR version of `k.payment.wechat`;
- output `wechatJsApi` only when the user explicitly says “payment inside the WeChat browser” and can provide `openId`;
- output `weChatH5` only when the user explicitly says “mobile-browser H5”;
- use `wechatApp` only for a native App, not as a direct copy for an ordinary website page.

This is the current best-practice boundary for the reference: give AI a stable default path with minimal prerequisites, then move to H5 / JSAPI / App when the scenario requires it.

## Pre-run configuration

Before integration testing, WeChat Pay requires at least these site settings:

| Configuration | Description |
| --- | --- |
| `appId` | WeChat application ID |
| `merchantId` | WeChat Pay merchant ID |
| `aPIV3Key` | WeChat Pay API v3 key |
| `certificatePrivateKey` | Merchant API certificate private key |
| `certificate` | Merchant API certificate |

All WeChat Pay modes currently support only `CNY`.

This is a provider constraint, not a limitation that prevents Kooboo commerce from supporting multiple currencies:

- the site can still configure multiple currencies in the admin;
- orders can still be created in other currencies;
- when using `k.payment.wechat*`, the final payment request must use `CNY`.

If the current order currency is not `CNY`, first decide:

1. whether to use another provider that supports the currency;
2. whether to create a `CNY` order during order creation;
3. whether only display conversion through `k.commerce.currency.convert(...)` is needed instead of sending a non-`CNY` order to WeChat Pay.

Use the following guard on the page:

```ts
const configured = k.commerce.currency.list().map(item => item.code.toUpperCase())
const supported = k.payment.wechat.supportedCurrency.map(code => code.toUpperCase())
const available = supported.filter(code => configured.includes(code))
```

## Kooboo built-in flow

The core contract of `k.payment.wechat*` is:

1. `charge(...)` first writes to the site's built-in `PaymentRequestRepository`;
2. the runtime automatically builds the provider notify callback URL;
3. after a successful callback or `checkStatus(...)` confirmation, the Kooboo callback worker writes back the payment state;
4. when `order` is provided, the commerce order automatically calls `OrderService.Pay(...)`;
5. when `callbackCodeName` is provided, the corresponding CodeBlock runs after payment completes.

Therefore, the default recommendation is to use Kooboo's built-in `PaymentRequest` flow rather than creating a separate `WechatPaymentRequest` table.

## Minimal end-to-end flow: Native QR payment

This is the safest WeChat Pay flow and the best default for AI output; `kooboo_commerce_2/src/view/pay/wechat.html` follows the same approach.

```html
<script env="server">
const orderId = k.request.orderid
const order = k.commerce.order.get(orderId)
const response = k.payment.wechat.charge({
    order: orderId,
    currency: "CNY",
    name: `kooboo order:${orderId}`,
    totalAmount: order.totalAmount,
    redirectUrl: `/order/${orderId}`
})
</script>

<div class="wechat-pay-shell">
    <div k-content="response.nextAction.renderHtml"></div>
</div>
```

Notes:

- `renderHtml` already includes QR rendering and status-polling logic;
- `response.nextAction.responseData` is the raw `code_url` and should be used only when generating the QR code yourself;
- in an ordinary web scenario, do not add a second polling implementation.
- if `order.currency` is not `CNY`, do not copy this example directly; route the currency or recreate the order as `CNY` first.

## Minimal end-to-end flow: H5 payment

For mobile browsers.

```ts
k.api.get(() => {
    const orderId = k.request.queryString.get("orderId")
    const order = k.commerce.order.get(orderId)
    const response = k.payment.weChatH5.charge({
        order: orderId,
        totalAmount: order.totalAmount,
        currency: "CNY",
        name: `kooboo order:${orderId}`,
        description: `Payment for order ${orderId}`
    })

    k.response.redirect(response.nextAction.redirectUrl)
})
```

`weChatH5` returns `redirectUrl`, not QR-code HTML.

## Minimal end-to-end flow: JSAPI payment

Only for the WeChat browser, and `openId` must be available first.

```ts
// server side
const result = k.payment.wechatJsApi.charge({
    order: "1307d2c7-c5f6-4957-9766-92e485b007b2",
    totalAmount: 0.01,
    currency: "CNY",
    name: "tea",
    description: "Tea description",
    openId: "<wechat_openid>"
})
```

```html
<script>
WeixinJSBridge.invoke(
    "getBrandWCPayRequest",
    JSON.parse(result.nextAction.responseData),
    function (res) {
        if (res.err_msg === "get_brand_wcpay_request:ok") {
            location.href = "/payment-result"
        }
    }
)
</script>
```

Note: frontend `ok` only means that JSAPI returned success; it must not replace server-side payment confirmation.

## Minimal end-to-end flow: App payment

```ts
k.api.post((body) => {
    const response = k.payment.wechatApp.charge({
        order: body.orderId,
        totalAmount: Number(body.totalAmount),
        currency: "CNY",
        name: body.name,
        description: body.description
    })

    return {
        requestId: response.requestId,
        prepayId: response.nextAction?.responseData
    }
})
```

Pass `prepayId` to the native App SDK for further processing; do not copy this flow directly into an ordinary website.

## Status-query example

```ts
k.api.get(() => {
    const requestId = k.request.queryString.get("requestId")
    const request = requestId ? k.payment.getRequest(requestId) : null
    const status = requestId ? k.payment.wechat.checkStatus(requestId) : null
    return { request, status }
})
```

This query endpoint is suitable for troubleshooting, admin compensation, or a custom payment-result page; actual order updates still rely on the Kooboo callback flow.

## Common AI mistakes

1. Generating `weChatH5`, `wechatJsApi`, or `wechatApp` without a stated scenario. The default should be `k.payment.wechat`.
2. Continuing to output `wechatJsApi` code without an `openId`.
3. Treating Native QR `renderHtml`, H5 `redirectUrl`, and JSAPI `responseData` as the same return shape.
4. Presenting “store `paymentHtml` and render it later” as the standard flow. Kooboo's built-in `PaymentRequest` already carries the main flow.
5. Passing a non-`CNY` order amount directly to `k.payment.wechat*`.
6. Marking the business order paid immediately after the frontend says “payment successful,” without following the provider callback or `checkStatus(...)`.
