# Alipay Payment Guide

> 面向 AI 生成代码的支付宝支付参考。Kooboo 当前提供 `k.payment.alipayForm`、`k.payment.alipayH5`、`k.payment.alipayApp` 三种入口。

## 何时阅读本文

- 用户要求接入支付宝网页支付、H5 支付或 App 支付；
- 需要明确 `renderHtml` 和 `responseData` 分别该怎么消费；
- 需要 AI 输出“只使用 Kooboo 内建支付能力”的最小闭环代码。

## 模式选择

| 场景 | 推荐方法 | 关键前提 | 返回重点 |
| --- | --- | --- | --- |
| 站点网页收银台、PC 浏览器支付 | `k.payment.alipayForm` | 页面可直接输出 HTML | `nextAction.renderHtml` |
| 手机浏览器 H5 支付 | `k.payment.alipayH5` | 运行在手机 H5 场景 | `nextAction.renderHtml` |
| 原生 App 内发起支付宝 SDK 支付 | `k.payment.alipayApp` | App 端能消费签名字符串 | `nextAction.responseData` |

默认建议：

- 纯站点页面支付，优先选 `alipayForm`；
- 明确是手机浏览器场景，再选 `alipayH5`；
- 用户明确说“App 支付”时，才输出 `alipayApp`。

## 运行前配置说明

支付宝运行前至少要准备这些站点设置：

| 配置项 | 说明 |
| --- | --- |
| `appId` | 支付宝应用 ID |
| `appPrivateKey` | 商户应用私钥 |
| `alipayPublicKey` | 支付宝公钥 |
| `useSandBox` | 是否使用沙箱；该开关会派生 runtime 使用的 `serverUrl` |

当前 runtime 将 `signType` 固定为 `RSA2`、将 `charset` 固定为 `UTF-8`；它们只是只读 runtime 属性，不是独立的站点配置项。

`alipay*` 当前仅支持 `CNY`。这是支付宝 provider 的限制，不是 Kooboo commerce 多货币能力的限制。

AI 不得猜测 Payment 后台 URL，也不得使用通用 `show_action_button` 伪装成支付宝配置快捷入口。没有专用且已验证的配置入口时，只说明用户需要在后台服务集成 Payment 中人工配置，并把该项保留为未完成证据。

如果站点已经配置了多货币，建议先做三件事：

1. 用 `k.commerce.currency.list()` 确认后台真正启用的货币；
2. 用 `k.payment.alipayForm.supportedCurrency` 或 `k.payment.alipayH5.supportedCurrency` 确认 provider 支持范围；
3. 如果当前订单不是 `CNY`，决定是改走别的 provider，还是在下单阶段创建 `CNY` 订单。

## Kooboo 内建链路

`k.payment.alipay*.charge(...)` 会先创建站点内置 `PaymentRequest`。若传入 `order`：

- 支付成功后，Kooboo 会通过 callback worker 自动把 commerce 订单标记为已支付；
- 若传入 `callbackCodeName`，会在回调阶段执行对应 CodeBlock；
- 后续排障可以用 `k.payment.getRequest(requestId)` 和 `alipayForm.checkStatus(requestId)` / `alipayH5.checkStatus(requestId)` 查看状态。

不要把“自己落库一张支付请求表”当成默认路径。

## 最小闭环：网页表单支付端点

适用于 PC 或普通网页收银台。Checkout 成功后让浏览器直接导航到这个 Payment API/CodeBlock；服务端直接输出 provider 表单，不在 View 中使用 `<script env="server">`，也不返回 JSON 给客户端做 `innerHTML` 注入。

```ts
k.api.get((orderId) => {
    const customerId = k.commerce.customer.currentId || ''
    if (!customerId || !orderId) return k.api.notFound()

    const order = k.commerce.order.get(orderId)
    if (!order?.customer?.id || order.customer.id !== customerId) {
        return k.api.notFound()
    }
    if (String(order.currency || '').toUpperCase() !== 'CNY') {
        return k.api.httpCode(400, { error: 'Alipay requires a CNY order' })
    }

    const response = k.payment.alipayForm.charge({
        order: order.id,
        name: `Kooboo order:${order.id}`,
        totalAmount: order.totalAmount,
        currency: 'CNY',
        returnUrl: `/order-result?orderId=${encodeURIComponent(order.id)}`
    })
    if (!response?.requestId || !response?.nextAction?.renderHtml) {
        return k.api.httpCode(502, { error: 'Payment provider did not return a form' })
    }

    k.cookie.set(`payment_request_${order.id}`, response.requestId, 1)
    k.response.write(response.nextAction.renderHtml)
    return null
})
```

要点：

- 浏览器直接打开支付端点，端点通过 `k.response.write` 输出 `renderHtml`；
- `returnUrl` 写业务回到页，真正 provider 回调仍由 Kooboo 内建 callback URL 处理；
- commerce 场景下记得传 `order`。
- 支付前验证订单属于 `customer.currentId`，不能只信任 URL/body 的 orderId；
- `response.requestId` 必须保留，用于 pending 排障或补偿查询；
- `renderHtml` 必须在服务端响应中直接输出。不要通过客户端 `innerHTML` 注入后期待其中脚本自动执行；
- 若 `order.currency` 不是 `CNY`，不要直接把该订单拿来走支付宝；先处理币种路由。

## 最小闭环：H5 支付端点

适用于手机浏览器。

```ts
k.api.get((orderId) => {
    const customerId = k.commerce.customer.currentId || ''
    const order = orderId ? k.commerce.order.get(orderId) : null
    if (!customerId || !order?.customer?.id || order.customer.id !== customerId) {
        return k.api.notFound()
    }
    if (String(order.currency || '').toUpperCase() !== 'CNY') {
        return k.api.httpCode(400, { error: 'Alipay requires a CNY order' })
    }

    const response = k.payment.alipayH5.charge({
        order: order.id,
        name: `Kooboo order:${order.id}`,
        totalAmount: order.totalAmount,
        currency: 'CNY',
        returnUrl: `/order-result?orderId=${encodeURIComponent(order.id)}`
    })
    if (!response?.requestId || !response?.nextAction?.renderHtml) {
        return k.api.httpCode(502, { error: 'Payment provider did not return a form' })
    }

    k.cookie.set(`payment_request_${order.id}`, response.requestId, 1)
    k.response.write(response.nextAction.renderHtml)
    return null
})
```

`alipayH5` 仍然返回 HTML 表单，不是 `redirectUrl`。

## 最小闭环：App 支付

适用于原生 App。网页端不要直接照抄这一段。

```ts
k.api.post((body) => {
    const response = k.payment.alipayApp.charge({
        order: body.orderId,
        name: body.name,
        totalAmount: Number(body.totalAmount),
        currency: "CNY",
        description: body.description
    })

    return {
        requestId: response.requestId,
        orderString: response.nextAction?.responseData
    }
})
```

`orderString` 需要由 App 端支付宝 SDK 继续消费。

## 状态查询示例

```ts
k.api.get(() => {
    const requestId = k.request.queryString.get("requestId")
    const request = requestId ? k.payment.getRequest(requestId) : null
    const status = requestId ? k.payment.alipayForm.checkStatus(requestId) : null
    return { request, status }
})
```

`checkStatus(...)` 用于排障和补偿；订单是否最终更新，仍以 provider 回调与 Kooboo callback worker 为主。

provider 已显示支付成功但 Commerce Order 仍为 pending 时，必须沿同一 `requestId` 检查 `PaymentRequest`、provider callback/notify 与 `checkStatus`。此时不能继续用没有证据的「密钥未配置」解释问题。

当前 runtime 链路：

1. `charge` 创建并保存 `PaymentRequest`，返回 `requestId` 与 `renderHtml`；
2. provider notify 进入 `AlipayBase.Notify` 并验证支付宝签名。当前实现是“金额匹配或 subject 匹配”即可接受回调，这是 `OR` 条件，不是两个字段同时严格校验。不要把它描述成双重校验，也不要把它当成履约授权的唯一边界；
3. `alipayForm` 的 provider return 进入 `AlipayForm.UpdateOrder`，按 `requestId` 调用 `CheckStatus` 后再跳转业务 `returnUrl`；`alipayH5` 回到它收到的业务 URL，`alipayApp` 没有浏览器回跳路径，依赖 provider notify；
4. callback worker 更新 `PaymentRequest`，并在请求携带 `order` 时幂等更新 Commerce Order；
5. KScript `alipayForm.checkStatus(requestId)` / `alipayH5.checkStatus(requestId)` 也会走相同 callback worker，不要另建一套“支付状态表”。

对于网页和 H5 流程，`returnUrl` 只表示浏览器回到站点。结果页必须重新读取真实订单状态：

- `paid`：显示成功；
- provider/query 仍为 pending：显示处理中与重试查询入口；
- failed/canceled：显示失败或取消，并允许重新发起合法支付；
- 无 requestId 或查不到订单：显示不可确认，不得默认成功。

## AI 易错点

1. 把 `alipayForm`、`alipayH5` 当成 `redirectUrl` 型接口来写。它们都以 `renderHtml` 为主。
2. 在网站页面里误用 `alipayApp`。`alipayApp` 返回的是给 App SDK 的签名字符串。
3. 漏传 `order`，导致 commerce 订单无法通过内建回调自动更新支付状态。
4. 把 `returnUrl` 理解成 provider 回调地址。真正的 provider notify URL 由 Kooboo runtime 自动拼接。
5. 对支付宝继续使用非 `CNY` 货币，或者误以为这是 Kooboo commerce 只能单币种。
6. 用客户端 API 返回 `renderHtml` 再 `innerHTML` 注入；表单内脚本通常不会按预期执行。应跳转到服务端支付页直接输出。
7. Checkout 使用了旧 cartId 或发生会话/cart 漂移，导致 cart/order ownership 不匹配。`customer.login()` 正常会绑定当前 cart；不要默认重建购物车，先按 [结算与支付黄金路径](../../Scenario/commerce-checkout-payment-flow.md) 排查状态来源。
8. 密钥未配置仍宣称支付完成；应提示用户配置 Payment 集成，并保留未完成证据。
9. provider 已成功但 Commerce 仍 pending 时继续猜测密钥缺失；应检查同一 `requestId` 的 PaymentRequest、callback/notify 与 `checkStatus`。
10. 把支付宝回调校验描述成必须同时匹配金额和 subject；当前 runtime 实际接受任一项匹配，履约授权应比这个 provider 回调条件更严格。
