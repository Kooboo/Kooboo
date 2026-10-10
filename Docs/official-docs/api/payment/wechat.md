# wechat

> WeChat Pay.

## Overview

WeChat payment includes four methods: Native, App, H5 and JsApi. WeChat Payment V3 parameters need to be configured in the site CMS before use.

If you want AI to directly generate WeChat **H5** payment code, please read [WeChat H5 payment process](./wechat-h5-flow.md) first. H5 uses `k.payment.weChatH5` and **`redirectUrl`**, so do not mix it with Native QR-code `renderHtml`.

| API | Description |
|-----|------|
| `k.payment.wechat` | Native Scan QR code to pay |
| `k.payment.wechatApp` | App payment |
| `k.payment.weChatH5` | H5 payment |
| `k.payment.wechatJsApi` | JSAPI payment within official account or mini program |

## Differences in payment methods

| API | WeChat payment products | Usage scenarios | Where the user completes payment | Current document validation scope |
|-----|-------------|----------|-------------------|----------------|
| `k.payment.wechat` | Native payment | Generate QR code on PC website or checkout counter | Users use WeChat to scan QR codes to pay | Verifiable API return structure; this round does not focus on testing |
| `k.payment.weChatH5` | H5 payment | Mobile browser web payment, does not rely on merchant apps | Mobile browser jumps to WeChat payment page | This round focuses on manual verification |
| `k.payment.wechatApp` | App payment | The merchant's own iOS/Android App calls up the WeChat SDK | Launch WeChat payment within the native app | App environment is required and will not be tested in this round. |
| `k.payment.wechatJsApi` | JSAPI payment | WeChat built-in browser, official account web page or mini program | Enable payment within the WeChat operating environment | Requires real `openId` and WeChat environment, not tested in this round |

During H5 payment verification, the front-end jump or return results cannot be used as the basis for final payment success. After completing the payment, `checkStatus()` or `k.payment.getRequest()` should be called using `requestId` returned by `charge()` to query the true status.

## wechat.charge()

Create a WeChat Native scan code payment request.

```ts
k.api.get(() => {
    const result = k.payment.wechat.charge({
        name: 'Kooboo order',
        description: 'Order payment',
        totalAmount: 1.5,
        currency: 'CNY',
        order: 'order-id',
        redirectUrl: '/payment/success?orderId=order-id',
        callbackCodeName: 'PaymentCallback'
    })

    return k.response.renderView(result.nextAction.renderHtml)
})
```

**Parameter:** `WeChatNativeParams`

| Parameter | Type | Required | Description |
|------|------|------|------|
| redirectUrl | `string` | yes | Jump address after payment when using `nextAction.renderHtml` |
| totalAmount | `number` | yes | Order amount |
| name | `string` | yes | Order name |
| description | `string` | yes | Order description |
| currency | `string` | yes | Currency, such as `CNY` |
| order | `string` | no | Commerce order ID. Once the payment is successful, the order payment status will be updated. |
| callbackCodeName | `string` | no | The name of the callback code executed after payment is completed |

**Return:** `ChargeResponse`

## wechatApp.charge()

Create a WeChat App payment request.

```ts
k.api.post(() => {
    const result = k.payment.wechatApp.charge({
        name: 'Tea',
        description: 'Tea description',
        totalAmount: 0.01,
        currency: 'CNY',
        order: 'order-id',
        callbackCodeName: 'PaymentCallback'
    })

    return result.nextAction.responseData
})
```

**Parameter:** `ChargeParams`

| Parameter | Type | Required | Description |
|------|------|------|------|
| totalAmount | `number` | yes | Order amount |
| name | `string` | yes | Order name |
| description | `string` | yes | Order description |
| currency | `string` | yes | Currency, such as `CNY` |
| order | `string` | no | Commerce order ID. Once the payment is successful, the order payment status will be updated. |
| callbackCodeName | `string` | no | The name of the callback code executed after payment is completed |

**Return:** `ChargeResponse`

## weChatH5.charge()

Create WeChat H5 payment request.

```ts
k.api.get(() => {
    const result = k.payment.weChatH5.charge({
        name: 'Tea',
        description: 'Tea description',
        totalAmount: 0.01,
        currency: 'CNY',
        order: 'order-id'
    })

    return k.response.redirect(result.nextAction.redirectUrl)
})
```

**Parameter:** `ChargeParams`

| Parameter | Type | Required | Description |
|------|------|------|------|
| totalAmount | `number` | yes | Order amount |
| currency | `string` | yes | Currency, such as `CNY` |
| order | `string` | yes | Commerce order ID. H5 requires this field to exist when running, and the order payment status will be updated after the payment is successful. |
| name | `string` | yes | Order name |
| description | `string` | yes | Order description |
| callbackCodeName | `string` | no | The name of the callback code executed after payment is completed |

**Return:** `ChargeResponse`

H5 payment runtime requirements `order`, `currency`, `totalAmount` must be provided at the same time. When these fields are missing, WeChat orders may return parameter errors.

### H5 manual verification

The test site provides an H5 verification page:

```text
/api-check/payment/wechat-h5
```

Corresponding test API:

```text
POST /api/ai-check/payment/wechat-h5/h5Charge
GET  /api/ai-check/payment/wechat-h5/status?requestId=<request-id>
GET  /api/ai-check/payment/wechat-h5/methods
```

Verification process:

1. Open `/api-check/payment/wechat-h5` in your mobile browser.
2. Click Create H5 Payment Request.
3. The page returns `requestId` and `nextAction.redirectUrl`.
4. Open `redirectUrl` to complete WeChat payment.
5. Return to the verification page and use `requestId` to check the payment status.

`wechatApp` and `wechatJsApi` require native App, WeChat built-in browser or mini program environment. This page does not verify these two types of payment methods.

## wechatJsApi.charge()

Create a WeChat JSAPI payment request. WeChat user `openId` needs to be passed in.

```ts
k.api.post(() => {
    const result = k.payment.wechatJsApi.charge({
        name: 'Tea',
        description: 'Tea description',
        totalAmount: 0.01,
        currency: 'CNY',
        order: 'order-id',
        openId: '<wechat-openid>'
    })

    return result.nextAction.responseData
})
```

Clients can pass `nextAction.responseData` as a parameter to `WeixinJSBridge.invoke('getBrandWCPayRequest', ...)`.

**Parameter:** `WeChatJsApiChargeParams`

| Parameter | Type | Required | Description |
|------|------|------|------|
| openId | `string` | yes | WeChat User OpenID |
| totalAmount | `number` | yes | Order amount |
| name | `string` | yes | Order name |
| description | `string` | yes | Order description |
| currency | `string` | yes | Currency, such as `CNY` |
| order | `string` | no | Commerce order ID. Once the payment is successful, the order payment status will be updated. |
| callbackCodeName | `string` | no | The name of the callback code executed after payment is completed |

**Return:** `ChargeResponse`

## checkStatus()

Check payment status.

```ts
k.api.get(() => {
    const status = k.payment.wechat.checkStatus('request-id')
    return { paid: status.paid, failed: status.failed, status: status.status }
})
```

**parameter:**

| Parameter | Type | Required | Description |
|------|------|------|------|
| requestId | `string` | yes | `charge()` returns `requestId` |

**Return:** `PaymentStatusResponse`

## WeChatV3Setting

| Property | Type | Description |
|------|------|------|
| appId | `string` | WeChat App ID |
| merchantId | `string` | Merchant number |
| aPIV3Key | `string` | API V3 key |
| certificatePrivateKey | `SettingFile` | Merchant certificate private key |
| certificate | `SettingFile` | Merchant certificate |
