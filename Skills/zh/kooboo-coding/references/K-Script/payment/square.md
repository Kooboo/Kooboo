# Square Payment Guide

> 面向 AI 生成代码的 Square 支付参考。Kooboo 当前入口是 `k.payment.square`。

## 何时阅读本文

- 用户要求接入 Square Checkout；
- 需要明确 `k.payment.square` 的返回方式和货币限制；
- 需要 AI 生成最小的跳转支付代码。

## 适用场景

`k.payment.square` 对应的是 Square checkout link / redirect 流程，适合：

- 用户点击后跳转到 Square 支付页；
- 支付完成后回到站点订单页；
- 复用 Kooboo 内建 callback 与订单更新。

当前实现只支持 `USD`。这是 Square provider 的限制，不是 Kooboo commerce 不能做多货币。

如果站点已经配置了多货币：

- 可以继续用其他币种展示商品、购物车和订单；
- 但只有在订单最终币种是 `USD` 时，才能走 `k.payment.square`；
- 如果业务要保留多 provider，建议先用 `k.payment.square.supportedCurrency` 和 `k.commerce.currency.list()` 做币种路由。

## 运行前配置说明

| 配置项 | 说明 |
| --- | --- |
| `applicationId` | 保存于站点设置中的 Square application ID；当前 Checkout 客户端实际使用 `accessToken` 鉴权 |
| `accessToken` | Checkout 和订单状态查询使用的 Square access token |
| `locationId` | 创建支付链接和查询订单时使用的 Square location id |
| `useSandBox` | 是否使用沙箱；该开关会派生 runtime 使用的 `baseURL` |

## Kooboo 内建链路

`k.payment.square.charge(...)` 会先创建内建 `PaymentRequest`，然后返回一个 checkout link。若传入 `order`：

- provider 成功后会通过回调与状态查询更新订单；
- commerce 场景下仍建议传 `order`，不要只靠页面 query string 自己做已支付标记。

## 最小闭环示例

```ts
//@k-url /square-checkout/{orderid}
const orderId = k.request.orderid
const order = k.commerce.order.get(orderId)

const response = k.payment.square.charge({
    order: orderId,
    totalAmount: order.totalAmount,
    currency: "USD",
    name: `kooboo order:${orderId}`,
    returnUrl: `/order/${orderId}`
})

k.response.redirect(response.nextAction.redirectUrl)
```

也可以直接输出 `response.nextAction.renderHtml`，但 Square 这里最自然的写法是直接跳转。

## 状态查询示例

```ts
k.api.get(() => {
    const requestId = k.request.queryString.get("requestId")
    const request = requestId ? k.payment.getRequest(requestId) : null
    const status = requestId ? k.payment.square.checkStatus(requestId) : null
    return { request, status }
})
```

## AI 易错点

1. 把属性名写成 `returnurl`。建议统一写 `returnUrl`。
2. 继续沿用别的 provider 的货币，忘了 Square 当前只支持 `USD`。
3. 看到 `renderHtml` 就强行包进一个复杂页面。Square 这里多数场景直接跳转更清晰。
4. commerce 订单支付时漏传 `order`。
5. 把 Square 只支持 `USD` 误写成 Kooboo commerce 只能使用 `USD`。
