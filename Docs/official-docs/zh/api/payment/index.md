# k.payment

> 第三方支付集成。

## 概述

`k.payment` 提供支付请求创建、支付状态查询、支付请求记录读取等能力。使用前需要先在站点 [CMS 服务集成 · Payment](../../cms/settings/integrations/payment.md) 中启用并配置对应支付方式。

`charge()` 会写入站点内置 **PaymentRequest**；若传入 Commerce `order`，支付成功后由内建 callback 更新订单。不要默认自建一套平行支付表。

## 如何选择支付方式

| 用户目标 | 推荐 API | `nextAction` 形态 |
|----------|----------|-------------------|
| 微信扫码 / PC 二维码 | `k.payment.wechat` | `renderHtml` |
| 微信手机浏览器 H5 | `k.payment.weChatH5` | `redirectUrl` → 见 [微信 H5 流程](./wechat-h5-flow.md) |
| 微信内置浏览器 | `k.payment.wechatJsApi` | `responseData`（需 `openId`） |
| 支付宝网页收银台 | `k.payment.alipayForm` | `renderHtml` |
| 支付宝 H5 | `k.payment.alipayH5` | `renderHtml` |
| PayPal Checkout | `k.payment.paypalCheckout` | `redirect` / `renderHtml` |
| Stripe 托管页 | `k.payment.stripeCheckout` | `redirect` / `renderHtml` |
| Stripe 站内卡支付 | `k.payment.stripe` | `paid` 或 3DS `redirectUrl` |

## 支付方式文档

| 文档 | API |
|------|-----|
| [支付宝](./alipay.md) | `k.payment.alipayApp`、`k.payment.alipayForm`、`k.payment.alipayH5` |
| [微信支付](./wechat.md) | `k.payment.wechat`、`k.payment.wechatApp`、`k.payment.weChatH5`、`k.payment.wechatJsApi` |
| [PayPal](./paypal.md) | `k.payment.paypalCheckout`、`k.payment.paypalForm` |
| [Stripe](./stripe.md) | `k.payment.stripeCheckout`、`k.payment.stripe` |
| [Square](./square.md) | `k.payment.square` |
| [Pay.NL](./paynl.md) | `k.payment.paynlCheckout` |
| [TwoCheckout](./two-checkout.md) | `k.payment.twoCheckout` |
| [MoneyBoxs](./moneyboxs.md) | `k.payment.moneyBoxs` |

## 多货币注意

`k.commerce.currency.list()` 返回站点已配置货币；每个 provider 还有 `supportedCurrency`。发起支付前应取两者交集。微信 / 支付宝通常仅 **CNY**。

## getRequest()

根据 `requestId` 获取支付请求记录。

```ts
k.api.get(() => {
    const request = k.payment.getRequest('request-id')
    return {
        id: request.id,
        paid: request.paid,
        failed: request.failed,
        paymentMethod: request.paymentMethod,
        order: request.order
    }
})
```

**参数：**

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| requestId | `string` | 是 | `charge()` 返回的支付请求 ID |

**返回：** `PaymentRequest`

## 通用属性

每个支付方式对象都包含以下基础属性。

| 属性 | 类型 | 说明 |
|------|------|------|
| name | `string` | 支付方式内部名称 |
| displayName | `string` | 展示名称 |
| icon | `string` | 图标地址或内容 |
| iconType | `string` | 图标类型 |
| supportedCurrency | `string[]` | 支持的货币 |
| context | `object` | 当前支付上下文 |
| setting | `object` | 该支付方式的站点配置 |

## 通用流程

```ts
k.api.get(() => {
    const product = k.commerce.product.get('test-product')
    const cartId = k.commerce.cart.create()

    k.commerce.cart.addOrUpdateLine(cartId, product.variants[0].id, 1)
    k.commerce.cart.updateContact(cartId, 'customer@example.com')

    const order = k.commerce.order.create(cartId, {
        address: { firstName: 'Test', phone: '13800138000' } as any
    })

    const charge = k.payment.alipayForm.charge({
        name: product.title,
        description: 'Order payment',
        totalAmount: order.totalAmount,
        currency: 'CNY',
        order: order.id,
        returnUrl: `/__paymentCallback?orderId=${order.id}`,
        callbackCodeName: 'PaymentCallback'
    })

    k.logger.information('Payment.Info', `requestId: ${charge.requestId}, orderId: ${order.id}`)
    return k.response.renderView(charge.nextAction.renderHtml)
})
```

## 支付回调示例

`returnUrl` 是支付完成后的跳转地址。可以在地址上携带订单 ID，再用已保存的 `requestId` 查询真实支付状态。

```ts
k.api.get(() => {
    const orderId = k.request.queryString.get("orderId")
    if (!orderId) {
        k.response.json({ code: -1, msg: 'order ID is not found' })
        return k.api.httpCode(400)
    }

    const requestId = getRequestIdByOrderId(orderId)
    if (!requestId) {
        k.response.json({ code: -1, msg: 'payment request ID is not found' })
        return k.api.httpCode(400)
    }

    const status = k.payment.alipayForm.checkStatus(requestId)
    if (status?.paid) {
        return k.response.redirect(`/order/${orderId}`)
    }

    return k.response.redirect('/payment-failed')
})
```

## ChargeResponse

| 属性 | 类型 | 说明 |
|------|------|------|
| paid | `boolean` | 是否已支付 |
| requestId | `any` | 支付请求 ID，后续查询状态时使用 |
| paymentMethodReferenceId | `string` | 第三方支付侧引用 ID |
| nextAction | `NextAction` | 下一步动作 |

## NextAction

| 属性 | 类型 | 说明 |
|------|------|------|
| type | `string` | 动作类型，如跳转、渲染 HTML、二维码等 |
| redirectUrl | `string` | 需要重定向时的目标地址 |
| renderHtml | `string` | 需要渲染到页面上的 HTML |
| responseData | `string` | 第三方返回的原始响应数据，常用于二维码或客户端 SDK 参数 |

## PaymentStatusResponse

| 属性 | 类型 | 说明 |
|------|------|------|
| hasResult | `boolean` | 是否已有查询结果 |
| paid | `boolean` | 是否支付成功 |
| failed | `boolean` | 是否支付失败 |
| status | `PaymentStatus` | 支付状态 |
| message | `string` | 状态消息 |

`PaymentStatus` 可为：`NotAvailable`、`Authorized`、`Pending`、`Paid`、`Cancelled`、`Rejected`。

## PaymentRequest

| 属性 | 类型 | 说明 |
|------|------|------|
| id | `any` | 支付请求 ID |
| description | `string` | 支付描述 |
| totalAmount | `number` | 金额 |
| currency | `string` | 货币 |
| order | `string` | 关联的 Commerce 订单 ID |
| paymentMethod | `string` | 支付方式 |
| paid | `boolean` | 是否已支付 |
| failed | `boolean` | 是否失败 |
| callbackCodeName | `string` | 支付完成后执行的回调代码名称 |
| referenceId | `string` | 第三方引用 ID |
| returnUrl | `string` | 支付完成跳转地址 |
| cancelUrl | `string` | 取消支付跳转地址 |
| additional | `Record<string, any>` | 支付方式附加数据 |
| card | `Card` | 卡信息 |
| creationDate | `Date` | 创建时间 |
| lastModified | `Date` | 最后修改时间 |
| name | `string` | 支付名称 |
