# PayPal Payment Guide

> 面向 AI 生成代码的 PayPal 支付参考。Kooboo 当前提供 `k.payment.paypalCheckout` 和 `k.payment.paypalForm` 两种入口。

## 何时阅读本文

- 用户要求接入 PayPal Checkout 或 PayPal 表单支付；
- 需要判断应使用重定向 checkout 还是自动提交 form；
- 需要 AI 按 Kooboo 现有 contract 输出最小闭环代码。

## 模式选择

| 场景                            | 推荐方法                   | 返回重点                     | 说明                      |
| ------------------------------- | -------------------------- | ---------------------------- | ------------------------- |
| 使用 PayPal Checkout 跳转支付页 | `k.payment.paypalCheckout` | `redirectUrl` / `renderHtml` |                           |
| 使用 HTML form 自动提交         | `k.payment.paypalForm`     | `renderHtml`                 | 偏传统的 PayPal form 集成 |

默认建议：

- 未特别说明时，优先 `paypalCheckout`；
- 用户明确要 form 提交或兼容旧表单集成时，再用 `paypalForm`。

## 运行前配置说明

PayPal 是多币种 provider，但最终可用币种仍应以“站点已配置货币”与 `supportedCurrency` 的交集为准。

推荐在接入前先确认：

```ts
const configured = k.commerce.currency.list().map(item => item.code.toUpperCase())
const provider = k.payment.paypalCheckout.supportedCurrency.map(code => code.toUpperCase())
const available = provider.filter(code => configured.includes(code))
```

如果订单要按某个非默认币种支付，优先在 `k.commerce.order.create(...)` 时把 `currency` 设成目标币种，而不是等到支付阶段再临时改金额。

### `paypalCheckout`

| 配置项       | 说明                           |
| ------------ | ------------------------------ |
| `clientId`   | PayPal client id               |
| `secret`     | PayPal secret                  |
| `useSandBox` | 是否使用沙箱；该开关同时决定 runtime 使用的 API 地址 |

### `paypalForm`

| 配置项         | 说明                |
| -------------- | ------------------- |
| `emailAddress` | PayPal 收款邮箱     |
| `logoImage`    | 可选的品牌图        |
| `useSandBox`   | 是否使用沙箱；该开关同时决定表单提交地址和 IPN 验证地址 |

## Kooboo 内建链路

`paypalCheckout` / `paypalForm` 都会先创建内建 `PaymentRequest`。如果传入 `order`：

- 成功回调后会自动更新 commerce 订单；
- `callbackCodeName` 可用于支付完成后的业务补充逻辑；
- 可通过 `k.payment.getRequest(requestId)` 和 `paypalCheckout.checkStatus(requestId)` 做排障。`paypalForm` 依赖已验证的 IPN 回调，当前不支持 `checkStatus`。

## 最小闭环：PayPal Checkout

这个形态与 `kooboo_commerce_2/src/api/paypal-checkout.ts` 的真实示例一致。

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

说明：

- `renderHtml` 实际上是一个跳转脚本；
- 也可以直接 `k.response.redirect(response.nextAction.redirectUrl)`；
- 不要把 `customer` 加入这个示例。当前 PayPal Checkout 实现虽然在 schema 中保留了这个旧参数，但创建 provider 订单时并不会使用它。
- `currency` 建议直接使用 `order.currency`，不要在支付阶段再手写一个与订单不一致的币种。

## 最小闭环：PayPal Form

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

`paypalForm` 会输出一段自动提交的 HTML form，由 PayPal IPN 回调负责异步确认。

## 状态查询示例

```ts
k.api.get(() => {
    const requestId = k.request.queryString.get("requestId")
    const request = requestId ? k.payment.getRequest(requestId) : null
    const status = requestId ? k.payment.paypalCheckout.checkStatus(requestId) : null
    return { request, status }
})
```

## AI 易错点

1. 把 `paypalCheckout` 和 `paypalForm` 当成同一种返回结构。前者有 `redirectUrl`，后者以 form HTML 为主。
2. 忘记传 `cancelUrl`，导致用户取消支付后无法回到业务页。
3. commerce 订单支付时没传 `order`。
4. 把 provider 成功跳转误当作订单已经支付完成。订单确认仍应以 Kooboo callback / status 为准。
5. 机械照抄 Stripe 的字段或注释到 PayPal 示例里。
6. 站点没在后台启用某个币种，却在 PayPal 里直接硬写该币种。
7. 调用 `paypalForm.checkStatus(...)`；当前表单支付实现不支持该方法。
