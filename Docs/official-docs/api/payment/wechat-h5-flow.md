# WeChat H5 payment process

> WeChat **H5** payment guide for AI and developers, covering the full flow from creating a payment request to callback verification.

## Choose the right API first

| scene | API | `nextAction` |
|------|-----|----------------|
| PC scan code / web QR code | `k.payment.wechat` | `renderHtml` (including QR code and polling) |
| **Mobile Browser H5** | **`k.payment.weChatH5`** | **`redirectUrl`** |
| WeChat built-in browser | `k.payment.wechatJsApi` | `responseData` (requires `openId`) |
| Native App | `k.payment.wechatApp` | `responseData` |

::: warning common errors
Do not use `k.payment.wechat` (Native)'s `renderHtml` process instead of H5. H5 must go to **`k.payment.weChatH5`** and make a jump to **`redirectUrl`**.
:::

The runtime attribute name is based on `kooboo.d.ts`: `weChatH5` (`get('WeChatH5')` is also acceptable).

## Configure before running

Enable WeChat Payment V3 on the site CMS and fill in:

| Configuration items | illustrate |
|--------|------|
| `appId` | WeChat App ID |
| `merchantId` | Merchant number |
| `aPIV3Key` | API V3 key |
| `certificatePrivateKey` | Merchant API certificate private key |
| `certificate` | Merchant API Certificate |

You also need to activate **H5 payment** on the WeChat merchant platform and configure the payment domain name and callback domain name. When the configuration is not completed, the following code is only for structural reference.

WeChat Pay only supports **CNY**. Multi-currency sites require CNY when placing an order or selecting a payment method, see [k.payment Overview](./index.md).

## Minimum closed loop: creating H5 payment on the server side

```ts
k.api.get(() => {
    const orderId = k.request.queryString.get("orderId")
    const order = k.commerce.order.get(orderId)

    const charge = k.payment.weChatH5.charge({
        order: orderId,
        totalAmount: order.totalAmount,
        currency: "CNY",
        name: `Order ${orderId}`,
        description: `Payment for order ${orderId}`,
        redirectUrl: `/order/${orderId}`
    })

    return k.response.redirect(charge.nextAction.redirectUrl)
})
```

Key points:

- **`order`, `currency`, `totalAmount` are required** (H5 runtime constraints).
- `redirectUrl` is the business bounce page; WeChat **notify** is automatically spliced ​​by Kooboo and should not be confused with `returnUrl`.
- After passing `order`, if the payment is successful, the Commerce order status will be updated through the built-in callback.

## Status query

If the front-end jumps or the user returns **not equal** the payment is successful, you must use `requestId` to query:

```ts
k.api.get(() => {
    const requestId = k.request.queryString.get("requestId")
    const request = k.payment.getRequest(requestId)
    const status = k.payment.weChatH5.checkStatus(requestId)
    return { request, status }
})
```

The business success logic will only be executed when `status.paid === true` (or the built-in order has been marked as paid).

## Works with Commerce orders

```ts
const order = k.commerce.order.create(cartId, { address: addressInfo })

const charge = k.payment.weChatH5.charge({
    order: order.id,
    totalAmount: order.totalAmount,
    currency: "CNY",
    name: product.title,
    description: "Order payment"
})

k.logger.information("Payment.WeChatH5", `requestId: ${charge.requestId}`)
k.response.redirect(charge.nextAction.redirectUrl)
```

## Manual Verification

The test site provides an H5 verification page and API endpoints for manually checking payment creation and callback behavior:

| use | path |
|------|------|
| H5 verification page | `/api-check/payment/wechat-h5` |
| List the instructions for each payment method on WeChat | `GET /api/ai-check/payment/wechat-h5/methods` |
| Create H5 payment request | `POST /api/ai-check/payment/wechat-h5/h5Charge` |
| Check status by requestId | `GET /api/ai-check/payment/wechat-h5/status?requestId=...` |

Verification steps:

1. Open the verification page in your mobile browser.
2. Call `h5Charge` and confirm that `requestId` and `nextAction.redirectUrl` are returned.
3. Open `redirectUrl` to complete payment.
4. Use `requestId` to adjust `status` and confirm that `paid` is consistent with `PaymentRequest`.

## Callbacks and returnUrl

`returnUrl` / `redirectUrl` (charge parameter) is the **business address** that the browser jumps back to after the user pays. Provider asynchronous notification is handled by Kooboo; if the business side wants to confirm twice on the bounce page:

```ts
k.api.get(() => {
    const orderId = k.request.queryString.get("orderId")
    const requestId = getRequestIdByOrderId(orderId)

    if (!requestId) {
        k.response.json({ error: "requestId not found" })
        return k.api.httpCode(400)
    }

    const status = k.payment.weChatH5.checkStatus(requestId)
    if (status.paid) {
        return k.response.redirect(`/order/${orderId}`)
    }
    return k.response.redirect("/payment-failed")
})
```

## AI generated checklist

1. First confirm the CMS and WeChat merchant platform H5 configuration.
2. By default, Native uses `k.payment.wechat`; **only H5 scenes** use `k.payment.weChatH5`.
3. Consume `redirectUrl`, don’t copy Native’s `renderHtml`.
4. Reuse the built-in `PaymentRequest` and do not build your own parallel payment tables (unless there are additional audit requirements).
5. The success condition is based on `checkStatus` / `getRequest`, not page jump.

## Related Docs

- [WeChat Pay API](./wechat.md)
- [k.payment Overview](./index.md)
