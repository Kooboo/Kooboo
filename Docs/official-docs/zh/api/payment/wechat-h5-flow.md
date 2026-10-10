# 微信 H5 支付流程

> 面向 AI 与开发者的微信 **H5** 支付指南，说明从创建支付请求到回调验证的完整接入流程。

## 先选对 API

| 场景 | API | `nextAction` |
|------|-----|----------------|
| PC 扫码 / 网页二维码 | `k.payment.wechat` | `renderHtml`（含二维码与轮询） |
| **手机浏览器 H5** | **`k.payment.weChatH5`** | **`redirectUrl`** |
| 微信内置浏览器 | `k.payment.wechatJsApi` | `responseData`（需 `openId`） |
| 原生 App | `k.payment.wechatApp` | `responseData` |

::: warning 常见错误
不要用 `k.payment.wechat`（Native）的 `renderHtml` 流程代替 H5。H5 必须走 **`k.payment.weChatH5`**，并对 **`redirectUrl`** 做跳转。
:::

运行时属性名以 `kooboo.d.ts` 为准：`weChatH5`（`get('WeChatH5')` 亦可）。

## 运行前配置

在站点 CMS 启用微信支付 V3，并填写：

| 配置项 | 说明 |
|--------|------|
| `appId` | 微信应用 ID |
| `merchantId` | 商户号 |
| `aPIV3Key` | API V3 密钥 |
| `certificatePrivateKey` | 商户 API 证书私钥 |
| `certificate` | 商户 API 证书 |

还需在微信商户平台开通 **H5 支付**，配置支付域名与回调域名。未完成配置时，下列代码仅作结构参考。

微信支付仅支持 **CNY**。多货币站点需在下单或选支付方式时落到 CNY，见 [k.payment 概述](./index.md)。

## 最小闭环：服务端创建 H5 支付

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

要点：

- **`order`、`currency`、`totalAmount` 必填**（H5 运行时约束）。
- `redirectUrl` 为业务回跳页；微信 **notify** 由 Kooboo 自动拼接，不要与 `returnUrl` 混淆。
- 传 `order` 后，支付成功会经内建 callback 更新 Commerce 订单状态。

## 状态查询

前端跳转或用户返回 **不等于** 支付成功，须用 `requestId` 查询：

```ts
k.api.get(() => {
    const requestId = k.request.queryString.get("requestId")
    const request = k.payment.getRequest(requestId)
    const status = k.payment.weChatH5.checkStatus(requestId)
    return { request, status }
})
```

仅当 `status.paid === true`（或内建订单已标记已付）再执行业务成功逻辑。

## 与 Commerce 订单配合

```ts
const order = k.commerce.order.create(cartId, { /* address 等 */ })

const charge = k.payment.weChatH5.charge({
    order: order.id,
    totalAmount: order.totalAmount,
    currency: "CNY",
    name: product.title,
    description: "Order payment"
})

k.logger.information("Payment.WeChatH5", `requestId: ${charge.requestId}`)
// 手机浏览器：redirect(charge.nextAction.redirectUrl)
```

## 手动验证

测试站提供 H5 验证页与接口，可用于手动验证支付创建与回调流程：

| 用途 | 路径 |
|------|------|
| H5 验证页 | `/api-check/payment/wechat-h5` |
| 列出微信各支付方式说明 | `GET /api/ai-check/payment/wechat-h5/methods` |
| 创建 H5 支付请求 | `POST /api/ai-check/payment/wechat-h5/h5Charge` |
| 按 requestId 查状态 | `GET /api/ai-check/payment/wechat-h5/status?requestId=...` |

验证步骤：

1. 手机浏览器打开验证页。
2. 调用 `h5Charge`，确认返回 `requestId` 与 `nextAction.redirectUrl`。
3. 打开 `redirectUrl` 完成支付。
4. 用 `requestId` 调 `status`，确认 `paid` 与 `PaymentRequest` 一致。

## 回调与 returnUrl

`returnUrl` / `redirectUrl`（charge 参数）是用户支付后浏览器跳回的**业务地址**。Provider 异步通知由 Kooboo 处理；业务侧若要在回跳页二次确认：

```ts
k.api.get(() => {
    const orderId = k.request.queryString.get("orderId")
    const requestId = getRequestIdByOrderId(orderId) // 业务自行持久化 orderId ↔ requestId

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

## AI 生成清单

1. 先确认 CMS 与微信商户平台 H5 配置。
2. 默认 Native 用 `k.payment.wechat`；**仅明确 H5 场景** 用 `k.payment.weChatH5`。
3. 消费 `redirectUrl`，不要照搬 Native 的 `renderHtml`。
4. 复用内建 `PaymentRequest`，不要自建平行支付表（除非有额外审计需求）。
5. 成功条件以 `checkStatus` / `getRequest` 为准，不以页面跳转为准。

## 相关文档

- [微信支付 API](./wechat.md)
- [k.payment 概述](./index.md)
