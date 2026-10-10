# wechat

> 微信支付。

## 概述

微信支付包含 Native、App、H5 和 JsApi 四种方式。使用前需要在站点 CMS 中配置微信支付 V3 参数。

如果要让 AI 直接生成微信 **H5** 支付代码，请先阅读 [微信 H5 支付流程](./wechat-h5-flow.md)。H5 使用 `k.payment.weChatH5` 与 **`redirectUrl`**，不要与 Native 扫码的 `renderHtml` 混用。

| API | 说明 |
|-----|------|
| `k.payment.wechat` | Native 扫码支付 |
| `k.payment.wechatApp` | App 支付 |
| `k.payment.weChatH5` | H5 支付 |
| `k.payment.wechatJsApi` | 公众号或小程序内 JSAPI 支付 |

## 支付方式区别

| API | 微信支付产品 | 使用场景 | 用户完成支付的位置 | 当前文档验证范围 |
|-----|-------------|----------|-------------------|----------------|
| `k.payment.wechat` | Native 支付 | PC 网站或收银台生成二维码 | 用户使用微信扫码支付 | 可验证 API 返回结构；本轮不重点测试 |
| `k.payment.weChatH5` | H5 支付 | 手机浏览器网页支付，不依赖商户 App | 手机浏览器跳转到微信支付页 | 本轮重点手动验证 |
| `k.payment.wechatApp` | App 支付 | 商户自己的 iOS/Android App 调起微信 SDK | 原生 App 内调起微信支付 | 需要 App 环境，本轮不测试 |
| `k.payment.wechatJsApi` | JSAPI 支付 | 微信内置浏览器、公众号网页或小程序 | 微信运行环境内调起支付 | 需要真实 `openId` 和微信环境，本轮不测试 |

H5 支付验证时，前端跳转或返回结果不能作为最终支付成功依据。完成支付后，应使用 `charge()` 返回的 `requestId` 调用 `checkStatus()` 或 `k.payment.getRequest()` 查询真实状态。

## wechat.charge()

创建微信 Native 扫码支付请求。

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

**参数：** `WeChatNativeParams`

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| redirectUrl | `string` | 是 | 使用 `nextAction.renderHtml` 时支付后的跳转地址 |
| totalAmount | `number` | 是 | 订单金额 |
| name | `string` | 是 | 订单名称 |
| description | `string` | 是 | 订单描述 |
| currency | `string` | 是 | 货币，如 `CNY` |
| order | `string` | 否 | Commerce 订单 ID。提供后支付成功会更新订单支付状态 |
| callbackCodeName | `string` | 否 | 支付完成后执行的回调代码名称 |

**返回：** `ChargeResponse`

## wechatApp.charge()

创建微信 App 支付请求。

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

**参数：** `ChargeParams`

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| totalAmount | `number` | 是 | 订单金额 |
| name | `string` | 是 | 订单名称 |
| description | `string` | 是 | 订单描述 |
| currency | `string` | 是 | 货币，如 `CNY` |
| order | `string` | 否 | Commerce 订单 ID。提供后支付成功会更新订单支付状态 |
| callbackCodeName | `string` | 否 | 支付完成后执行的回调代码名称 |

**返回：** `ChargeResponse`

## weChatH5.charge()

创建微信 H5 支付请求。

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

**参数：** `ChargeParams`

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| totalAmount | `number` | 是 | 订单金额 |
| currency | `string` | 是 | 货币，如 `CNY` |
| order | `string` | 是 | Commerce 订单 ID。H5 运行时要求该字段存在，支付成功后会更新订单支付状态 |
| name | `string` | 是 | 订单名称 |
| description | `string` | 是 | 订单描述 |
| callbackCodeName | `string` | 否 | 支付完成后执行的回调代码名称 |

**返回：** `ChargeResponse`

H5 支付运行时要求 `order`、`currency`、`totalAmount` 必须同时提供。缺少这些字段时，微信下单可能返回参数错误。

### H5 手动验证

测试站点提供了一个 H5 验证页面：

```text
/api-check/payment/wechat-h5
```

对应测试 API：

```text
POST /api/ai-check/payment/wechat-h5/h5Charge
GET  /api/ai-check/payment/wechat-h5/status?requestId=<request-id>
GET  /api/ai-check/payment/wechat-h5/methods
```

验证流程：

1. 在手机浏览器中打开 `/api-check/payment/wechat-h5`。
2. 点击创建 H5 支付请求。
3. 页面返回 `requestId` 和 `nextAction.redirectUrl`。
4. 打开 `redirectUrl` 完成微信支付。
5. 回到验证页面，使用 `requestId` 查询支付状态。

`wechatApp` 和 `wechatJsApi` 需要原生 App、微信内置浏览器或小程序环境，本页面不验证这两类支付方式。

## wechatJsApi.charge()

创建微信 JSAPI 支付请求。需要传入微信用户 `openId`。

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

客户端可将 `nextAction.responseData` 作为 `WeixinJSBridge.invoke('getBrandWCPayRequest', ...)` 的参数。

**参数：** `WeChatJsApiChargeParams`

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| openId | `string` | 是 | 微信用户 OpenID |
| totalAmount | `number` | 是 | 订单金额 |
| name | `string` | 是 | 订单名称 |
| description | `string` | 是 | 订单描述 |
| currency | `string` | 是 | 货币，如 `CNY` |
| order | `string` | 否 | Commerce 订单 ID。提供后支付成功会更新订单支付状态 |
| callbackCodeName | `string` | 否 | 支付完成后执行的回调代码名称 |

**返回：** `ChargeResponse`

## checkStatus()

检查支付状态。

```ts
k.api.get(() => {
    const status = k.payment.wechat.checkStatus('request-id')
    return { paid: status.paid, failed: status.failed, status: status.status }
})
```

**参数：**

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| requestId | `string` | 是 | `charge()` 返回的 `requestId` |

**返回：** `PaymentStatusResponse`

## WeChatV3Setting

| 属性 | 类型 | 说明 |
|------|------|------|
| appId | `string` | 微信应用 ID |
| merchantId | `string` | 商户号 |
| aPIV3Key | `string` | API V3 密钥 |
| certificatePrivateKey | `SettingFile` | 商户证书私钥 |
| certificate | `SettingFile` | 商户证书 |
