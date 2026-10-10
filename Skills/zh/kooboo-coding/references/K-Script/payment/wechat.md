# WeChat Payment Guide

> 面向 AI 生成代码的微信支付参考。Kooboo 当前提供 `k.payment.wechat`、`k.payment.weChatH5`、`k.payment.wechatJsApi`、`k.payment.wechatApp` 四种入口。

## 何时阅读本文

- 用户要求实现微信支付；
- 需要判断该选二维码、H5、JSAPI 还是 App；
- 需要 AI 输出不会把微信支付模式混写的代码。

## 模式选择

| 场景 | 推荐方法 | 关键前提 | 返回重点 |
| --- | --- | --- | --- |
| PC 网页扫码支付、普通网页二维码支付 | `k.payment.wechat` | 页面可直接渲染二维码 | `nextAction.renderHtml` / `responseData` |
| 手机浏览器 H5 支付 | `k.payment.weChatH5` | 明确是手机 H5 环境 | `nextAction.redirectUrl` |
| 微信内置浏览器 JSAPI 支付 | `k.payment.wechatJsApi` | 已拿到微信用户 `openId` | `nextAction.responseData` |
| 原生 App 支付 | `k.payment.wechatApp` | App 端可继续消费 `prepay_id` | `nextAction.responseData` |

默认建议：

- 用户只说“微信支付”时，优先输出 `k.payment.wechat` 的 Native 二维码版本；
- 只有用户明确说明“微信浏览器内支付”并且能提供 `openId`，才输出 `wechatJsApi`；
- 只有用户明确说明“手机浏览器 H5”时，才输出 `weChatH5`；
- `wechatApp` 只用于原生 App，不适合普通网站页面直接照抄。

这也是当前 reference 的最佳实践边界：先给 AI 一个稳定、最少前提的默认路径，再根据场景升级到 H5 / JSAPI / App。

## 运行前配置说明

微信支付配置在联调前至少要准备这些站点设置：

| 配置项 | 说明 |
| --- | --- |
| `appId` | 微信应用 ID |
| `merchantId` | 微信支付商户号 |
| `aPIV3Key` | 微信支付 API v3 密钥 |
| `certificatePrivateKey` | 商户 API 证书私钥 |
| `certificate` | 商户 API 证书 |

所有微信支付模式当前都只支持 `CNY`。

这表示的是 provider 约束，不是 Kooboo commerce 只能做单币种：

- 站点仍然可以在后台配置多货币；
- 订单也仍然可以用别的币种创建；
- 只是当你决定走 `k.payment.wechat*` 时，最终支付请求必须落到 `CNY`。

如果当前订单币种不是 `CNY`，推荐先判断：

1. 是否应该改走支持该币种的其他 provider；
2. 是否应该在下单阶段就创建一张 `CNY` 订单；
3. 是否只需要用 `k.commerce.currency.convert(...)` 做展示换算，而不是直接把非 `CNY` 订单塞给微信支付。

可用下面的写法在页面上做 guard：

```ts
const configured = k.commerce.currency.list().map(item => item.code.toUpperCase())
const supported = k.payment.wechat.supportedCurrency.map(code => code.toUpperCase())
const available = supported.filter(code => configured.includes(code))
```

## Kooboo 内建链路

`k.payment.wechat*` 的核心 contract 是：

1. `charge(...)` 先写入站点内置 `PaymentRequestRepository`；
2. runtime 自动为 provider 拼接 notify callback URL；
3. 成功回调或 `checkStatus(...)` 确认支付后，Kooboo callback worker 回写支付状态；
4. 如果传入 `order`，commerce 订单会自动调用 `OrderService.Pay(...)`；
5. 如果传入 `callbackCodeName`，支付完成后会执行对应 CodeBlock。

因此，默认推荐使用 Kooboo 内建 `PaymentRequest` 流程，而不是额外落一张 `WechatPaymentRequest` 表。

## 最小闭环：Native 二维码支付

这是最稳妥、最适合作为 AI 默认输出的微信支付方案；`kooboo_commerce_2/src/view/pay/wechat.html` 也是这个方向。

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

说明：

- `renderHtml` 已经包含二维码渲染和状态轮询逻辑；
- `response.nextAction.responseData` 则是原始 `code_url`，仅在你要自己生成二维码时才用；
- 普通网页场景下，不要再自己额外写一套重复轮询。
- 如果 `order.currency` 不是 `CNY`，先不要直接照抄这段；应先处理币种路由或重新创建 `CNY` 订单。

## 最小闭环：H5 支付

适用于手机浏览器。

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

`weChatH5` 返回的是 `redirectUrl`，不是二维码 HTML。

## 最小闭环：JSAPI 支付

只适用于微信浏览器内，且必须先拿到 `openId`。

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

注意：前端 `ok` 只代表 JSAPI 返回成功，不应替代服务端支付确认。

## 最小闭环：App 支付

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

`prepayId` 需要交给原生 App SDK 继续处理；普通网页不要直接照抄这条路径。

## 状态查询示例

```ts
k.api.get(() => {
    const requestId = k.request.queryString.get("requestId")
    const request = requestId ? k.payment.getRequest(requestId) : null
    const status = requestId ? k.payment.wechat.checkStatus(requestId) : null
    return { request, status }
})
```

这个查询接口适合排障、管理后台补偿或自定义支付结果页；真正的订单更新仍以 Kooboo 回调链路为主。

## AI 易错点

1. 用户没说明场景，就直接生成 `weChatH5`、`wechatJsApi` 或 `wechatApp`。默认应先用 `k.payment.wechat`。
2. `wechatJsApi` 没有 `openId` 却继续输出代码。
3. 把 Native 二维码的 `renderHtml`、H5 的 `redirectUrl`、JSAPI 的 `responseData` 混成一种返回格式处理。
4. 误把“自己把 `paymentHtml` 落库再渲染”写成标准方案。Kooboo 内建 `PaymentRequest` 已经能承接主链路。
5. 当前订单币种不是 `CNY`，却仍然直接把订单金额原样传给 `k.payment.wechat*`。
6. 前端提示“支付成功”后，就立即把业务订单标记为已支付，没有再走 provider 回调或 `checkStatus(...)`。
