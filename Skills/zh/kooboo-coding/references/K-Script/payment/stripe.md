# Stripe Payment Guide

> 面向 AI 生成代码的 Stripe 支付参考。Kooboo 当前提供 `k.payment.stripeCheckout` 和 `k.payment.stripe` 两类入口。

## 何时阅读本文

- 用户要求实现 Stripe 托管收银台或站内信用卡支付；
- 需要判断 `stripeCheckout` 和 `stripe` 的边界；
- 需要 AI 生成可对接 3DS、Webhook、保存卡片的代码。

## 模式选择

| 场景 | 推荐方法 | 返回重点 | 说明 |
| --- | --- | --- | --- |
| Hosted checkout 跳转页 | `k.payment.stripeCheckout` | `redirectUrl` / `renderHtml` | 默认首选，集成成本最低 |
| 站内信用卡支付、保存卡片、3DS | `k.payment.stripe` | `paid` 或 `nextAction.redirectUrl` | 需要自己处理表单和 3DS 回跳 |

默认建议：

- 用户只说“接 Stripe 支付”时，优先 `stripeCheckout`；
- 需要站内收单、复用卡片、保存 customer 卡时，再选 `stripe`。

## 运行前配置说明

Stripe 是多币种 provider，但 `stripeCheckout` 的 `supportedCurrency` 不是一份固定常量列表。源码里它会先拿 Stripe 的默认币种集合，再按站点设置里的 `paymentMethods` 做交集收窄。

这意味着：

- 后台没启用的货币，仍然不应该出现在你的站点支付流程里；
- Stripe Checkout 设置里如果启用了特定 payment method，它可能进一步缩小可用币种；
- 做 provider 列表或币种切换时，应以运行时的 `k.payment.stripeCheckout.supportedCurrency` 为准，而不是手写一份静态数组。

推荐先做一次可用币种确认：

```ts
const configured = k.commerce.currency.list().map(item => item.code.toUpperCase())
const provider = k.payment.stripeCheckout.supportedCurrency.map(code => code.toUpperCase())
const available = provider.filter(code => configured.includes(code))
```

### `stripeCheckout`

| 配置项 | 说明 |
| --- | --- |
| `secretKey` | Stripe secret key |
| `webhookSigningSecret` | Webhook 签名密钥 |
| `paymentMethods` | 允许的 payment method 列表，逗号分隔 |

### `stripe`

| 配置项 | 说明 |
| --- | --- |
| `secretKey` | Stripe secret key |
| `webhookSigningSecret` | Webhook 签名密钥 |

## Kooboo 内建链路

两种 Stripe 方式都会先创建内建 `PaymentRequest`。另外：

- `stripeCheckout` 的 provider 成功页与取消页会回到 Kooboo callback URL 再更新订单；
- `stripe` 在 `paid === true` 时会立即触发 callback，若需要 3DS 则通过 `nextAction.redirectUrl` 进入后续认证；
- 传入 `order` 时，commerce 订单会在支付完成后自动调用 `OrderService.Pay(...)`。

## 最小闭环：Stripe Hosted Checkout

这个形态与 `kooboo_commerce_2/src/api/stripe-checkout.ts` 的真实示例一致。

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

## 最小闭环：站内信用卡支付

这个形态与 `kooboo_commerce_2/src/view/pay/stripe.html` 的真实示例一致。

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

如果没有现成 `paymentMethodId`，也可以传 `card` 对象让 Kooboo 代为创建支付方式。需要保存卡片时，优先配合：

- `k.payment.stripe.getCardList(customerId)`
- `k.payment.stripe.createCard(customerId, card)`
- `k.payment.stripe.removeCard(cardId)`

两个 Stripe 方法对 `customer` 的处理不同：

- `stripeCheckout.customer` 是 Kooboo customer key，Checkout 会创建或复用对应的 Stripe customer。
- `stripe.customer` 依赖已有的 Kooboo 到 Stripe customer 映射。`createCard(customerId, card)` 会创建该映射并绑定卡片；仅在 `stripe.charge(...)` 中同时传 `customer` 和原始 `card`，不会把新卡保存为以后可复用的卡片。

## 3DS / 二次认证处理

`k.payment.stripe.charge(...)` 的返回有两种核心分支：

- `response.paid === true`：本次请求已完成支付；
- `response.nextAction?.redirectUrl`：需要跳转到 3DS / provider 下一步页面。

前端不要忽略第二种情况。

```ts
if (response.paid) {
    location.href = `/order/${orderId}`
} else if (response.nextAction?.redirectUrl) {
    location.href = response.nextAction.redirectUrl
}
```

如果这是 commerce 订单支付，建议让 `currency` 与 `order.currency` 保持一致。需要换币种时，优先回到建单阶段重建订单。

## 状态查询示例

```ts
k.api.get(() => {
    const requestId = k.request.queryString.get("requestId")
    const request = requestId ? k.payment.getRequest(requestId) : null
    const status = requestId ? k.payment.stripeCheckout.checkStatus(requestId) : null
    return { request, status }
})
```

## AI 易错点

1. 用户没要求站内信用卡，就直接生成 `k.payment.stripe` 的复杂表单。多数情况下应优先 `stripeCheckout`。
2. `k.payment.stripe` 返回了 `nextAction.redirectUrl` 却没处理，导致 3DS 链路断掉。
3. 忘记配置 `webhookSigningSecret`，让 Stripe callback 无法验签。
4. 把 `stripeCheckout` 的 `renderHtml` 理解成一个要自己解析的 HTML 片段；它通常只是跳转脚本。
5. 把 `customer` 加原始 `card` 误当成保存卡片流程。需要以后复用卡片时，应使用 `createCard(customerId, card)`。
6. 只看 Stripe 官网支持币种，没有再和站点已配置货币、`supportedCurrency` 运行时结果做交集。
