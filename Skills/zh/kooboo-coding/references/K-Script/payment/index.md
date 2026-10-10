# Payment Guide 支付模块

## 模块导航

| 用户目标 | 支付方法 | `nextAction` 形态 | 使用的 Guide |
| --- | --- | --- | --- |
| 微信扫码支付、PC 网页二维码支付 | `k.payment.wechat` | `qrcode` + `renderHtml` / `responseData` | [WeChat Payment Guide](wechat.md) |
| 微信浏览器内支付 | `k.payment.wechatJsApi` | `responseData` | [WeChat Payment Guide](wechat.md) |
| 微信 H5 跳转支付 | `k.payment.weChatH5` | `redirectUrl` | [WeChat Payment Guide](wechat.md) |
| 微信 App 支付 | `k.payment.wechatApp` | `responseData` | [WeChat Payment Guide](wechat.md) |
| 支付宝网页收银台 / 表单跳转 | `k.payment.alipayForm` | `renderHtml` | [Alipay Payment Guide](alipay.md) |
| 支付宝 H5 支付 | `k.payment.alipayH5` | `renderHtml` | [Alipay Payment Guide](alipay.md) |
| 支付宝 App 支付 | `k.payment.alipayApp` | `responseData` | [Alipay Payment Guide](alipay.md) |
| PayPal Checkout 跳转 | `k.payment.paypalCheckout` | `redirect` + `renderHtml` | [PayPal Payment Guide](paypal.md) |
| PayPal Form 提交 | `k.payment.paypalForm` | `form` + `renderHtml` | [PayPal Payment Guide](paypal.md) |
| Stripe Hosted Checkout | `k.payment.stripeCheckout` | `redirect` + `renderHtml` | [Stripe Payment Guide](stripe.md) |
| Stripe 直连信用卡 / 3DS | `k.payment.stripe` | `paid` 或 `3dSecure` | [Stripe Payment Guide](stripe.md) |
| Square Checkout 跳转 | `k.payment.square` | `redirect` + `renderHtml` | [Square Payment Guide](square.md) |

## 多货币与 provider 货币限制

Kooboo commerce 自身支持多货币，但第三方支付是否能收某个币种，要再经过 provider 一层筛选。

1. 先在 Commerce 后台配置默认货币和附加货币，再谈多货币支付。
2. `k.commerce.currency.list()` 只会返回站点默认货币和后台已配置的附加货币。
3. `k.commerce.order.create(...)` 可以显式传 `currency` 创建该币种订单；如果币种未在后台配置，runtime 会回退到站点默认货币。
4. 每个支付方式都有 `supportedCurrency` contract。最终可用币种应该取：
   - 站点已配置货币
   - provider `supportedCurrency`
   - 业务当前允许的币种

推荐在支付页或支付 API 里先做一次交集判断：

```ts
k.api.get(() => {
    const configured = k.commerce.currency.list().map(item => item.code.toUpperCase())
    const provider = k.payment.paypalCheckout.supportedCurrency.map(code => code.toUpperCase())
    const available = provider.filter(code => configured.includes(code))
    return {
        defaultCurrency: k.commerce.currency.default,
        available
    }
})
```

补充规则：

- 单币种 provider（如 WeChat / Alipay / Square）如果没传 `currency`，Kooboo 会自动补成它唯一支持的币种；如果你显式传了别的币种，会直接报 `currency not supported by payment method`。
- 多币种 provider（如 PayPal / Stripe）应显式传 `currency`，不要依赖默认值猜测。
- `supportedCurrency` 是 provider 约束，不是 Kooboo commerce 的全局限制。

## 标准流程

1. 先按用户场景和目标币种选对支付方法，不要看到“支付”就默认写某一个 provider。
2. 调用 `k.payment.xxx.charge(...)` 时，若这是 commerce 订单支付，优先把 `order` 传进去。
3. 若订单币种需要切到 provider 支持的币种，优先在 `k.commerce.order.create(...)` 阶段就创建成目标币种订单；`k.commerce.currency.convert(...)` 更适合展示价、报价或 provider 选择前的换算。
4. 让 Kooboo 内建 `PaymentRequest` / callback / `checkStatus(...)` 机制处理支付请求状态，不要默认自己再造一张支付请求表。
5. 按 `nextAction` 的真实形态处理前端：
   - `renderHtml`：由服务端端点直接写入响应；浏览器直接导航到该端点，不要先返回 JSON 再用客户端 `innerHTML` 注入。
   - `redirectUrl`：服务端或前端跳转。
   - `responseData`：交给 provider SDK 或你自己的前端逻辑继续消费。
6. 只有在业务侧确实还要补通知、积分、开票、发货之类动作时，再使用 `callbackCodeName` 扩展。

::: danger 不要在回到页确认支付
`returnUrl`、订单结果页、前端轮询和公开 API 都不能调用 `k.commerce.order.pay(...)`。该方法会直接修改订单状态，不会验证 provider 签名。订单支付结果必须来自 Kooboo 内建 callback，或由 provider `checkStatus(...)` 确认后交给内建 callback worker 更新。
:::

## Kooboo 内建 contract

`k.payment.xxx.charge(...)` 不是单纯“向第三方发个请求”。Kooboo runtime 还会：

- 先把本次请求写进站点内置 `PaymentRequestRepository`；
- 在 provider 回调或 `checkStatus(...)` 确认成功后回写 `Paid` / `Failed`；
- 如果传入了 `order`，由内建 callback worker 调用 commerce `OrderService.Pay(...)` 更新订单支付状态；
- 如果传入了 `callbackCodeName`，支付完成后执行对应 CodeBlock。

因此，reference 默认以“复用内建支付链路”为最佳实践；只有业务确实需要额外字段或额外审计数据时，才补自己的存储层。

前端跳回 `returnUrl` 只表示用户浏览器完成了一次导航，不是可信支付证据。回到页只读取并展示订单/PaymentRequest 状态；若状态仍 pending，则继续等待 callback 或调用 provider 的状态查询能力，不得主动把订单改成 paid。

provider 已显示成功但 Commerce Order 仍 pending 时，沿同一 `requestId` 检查 `PaymentRequest`、provider callback/notify 和 `checkStatus(...)`。不要在已有 provider 成功证据后继续猜测「密钥未配置」。

## 配置入口边界

- 支付配置位于后台服务集成 Payment，但 AI 不得猜测后台 URL 或手写 SiteId。
- 通用 `show_action_button` 不是 Payment 配置入口，不能用它伪装快捷跳转。
- 没有专用且已验证的配置入口时，明确说明需要用户人工进入后台配置，并把该项保留为未完成证据。

## 何时继续读叶子文档

- 需要判断该选哪个 provider / 模式：看对应 Guide 的“模式选择”部分。
- 需要确认 provider 的配置项：看对应 Guide 的“运行前配置”部分。
- 需要 AI 输出可执行代码：直接按叶子页里的最小闭环示例生成，不要跨 provider 混写。
- 需要把支付能力接回 commerce 页面：同时参考 [Commerce Order](../commerce/order.md)、[Commerce 结算与支付黄金路径](../../Scenario/commerce-checkout-payment-flow.md) 和 [Commerce Settings](../commerce/settings.md)。

## 边界说明

- 本入口只做 provider 路由与共性 contract，不展开每个 provider 的全部参数。
- 各 provider 的支持货币不同，不能把一个示例机械替换成另一个 provider。
- 后台没有配置的货币，不应直接拿去创建订单或发起支付请求。
- `renderHtml`、`redirectUrl`、`responseData` 不是同义字段；具体如何消费，以叶子页和 runtime 返回形态为准。
