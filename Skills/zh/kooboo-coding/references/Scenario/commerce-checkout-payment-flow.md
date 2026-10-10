# Commerce 结算与支付黄金路径

适用于 `commerceModel = online-checkout` 的 Cart → Checkout → Payment → Order Result → Account/Orders。目标是把「已登录」接到「能下单并看到真实支付状态」，避免三站实测中的订单不存在、支付页 NRE、回跳空白、后台仍待支付等问题。

支付宝细节见 [Alipay Payment Guide](../K-Script/payment/alipay.md)；订单 API 见 [Commerce Order](../K-Script/commerce/order.md)；Checkout 的收货地址字段、保存地址复用和隐藏字段保留规则见 [Commerce 顾客地址与收货地址契约](commerce-customer-address-flow.md)。

## 1. 端到端步骤（必须按序）

1. **已登录**：`k.commerce.customer.currentId` 非空；Header/Account 已能恢复同一会话。
2. **购物车属于当前顾客**：`cart.currentId` 非空，且该 cart 的 `customerId === currentId`。
3. **Checkout 创建订单**：只用 `cart.currentId` 下单；创建后读回 `order.customer.id`，必须等于 `currentId` 才能进入支付。
4. **跳转支付端点**：例如 `/api/payment/alipay?orderId=...`；浏览器直接导航到该端点，由 KScript `charge({ order })` 并通过 `k.response.write(renderHtml)` 输出支付表单。
5. **保留 `requestId`**：用于 pending 排障 / `checkStatus`。
6. **returnUrl 只展示状态**：重新读取订单或 PaymentRequest；不得把回跳当成已支付，不得调用 `order.pay`。
7. **Account/Orders**：用 `order.list({ customerId: currentId })` 列单，并提供订单详情入口。

## 2. 购物车归属

`order.create(cartId, ...)` 的 `CustomerId` **来自购物车**，不是下单时临时传入。`customer.login(...)` 会把当前 `ECommerceState.CartId` 对应的购物车绑定到登录顾客，因此正常登录路径不需要默认重建或迁移 guest cart。

Checkout 仍必须验证 ownership，因为客户端旧 cartId、会话/cookie 漂移、登录发生在另一个浏览器上下文，或登录后切换 cart 都可能造成当前购物车与顾客不一致。

Checkout 创建订单前必须同时满足：

```ts
const customerId = k.commerce.customer.currentId || ''
const cartId = k.commerce.cart.currentId || ''
if (!customerId) return { success: false, error: 'Login required' }
if (!cartId) return { success: false, error: 'Cart required' }

const cart = k.commerce.cart.get(cartId)
if (!cart || !cart.customerId || cart.customerId !== customerId) {
    return {
        success: false,
        error: 'Cart is not bound to the current Commerce customer. Refresh auth/cart state and retry checkout.'
    }
}
```

排查顺序：

- 确认登录、Cart 和 Checkout 都在同一个浏览器会话中，并都使用 `kb_commerce`；
- 登录后重新读取 `customer.currentId`、`cart.currentId` 和 `cart.get(cartId)`，不要继续使用客户端缓存的旧 cartId；
- 确认登录流程确实调用了 `customer.login(...)`；`updateContact` 不是登录，不能替代它；
- 只有产品明确要求跨会话合并购物车时，才设计可审计的迁移策略。不要把重建/迁移当成每次登录的默认动作。
创建订单成功后立刻校验：

```ts
const order = k.commerce.order.create(cartId, { address: body.address, note: body.note, currency: 'CNY' })
if (!order?.customer?.id || order.customer.id !== customerId) {
    return { success: false, error: 'Order was created without customer ownership; do not start payment' }
}
return { success: true, orderId: order.id, customerId: order.customer.id }
```

## 3. Checkout API（客户端可调用）

Checkout 页用 API 创建订单即可，不必为表单页引入 `env="server"`。

`body.address` 必须遵循 [Commerce 顾客地址与收货地址契约](commerce-customer-address-flow.md)：Contact Phone 映射到 `address.phone`；选择已保存地址时复制完整地址；不能从 Checkout 当前可见字段反推或裁剪 `AddressModel`。

```ts
k.api.post((body) => {
    const customerId = k.commerce.customer.currentId || ''
    if (!customerId) return { success: false, error: 'Login required' }

    const cartId = k.commerce.cart.currentId || ''
    if (!cartId) return { success: false, error: 'Cart required' }

    const cart = k.commerce.cart.get(cartId)
    if (!cart) return { success: false, error: 'Cart not found' }
    if (!cart.customerId || cart.customerId !== customerId) {
        return { success: false, error: 'Cart customer mismatch' }
    }
    if (!cart.lines || cart.lines.length === 0) {
        return { success: false, error: 'Cart is empty' }
    }

    const order = k.commerce.order.create(cartId, {
        address: body.address,
        note: body.note,
        currency: 'CNY'
    })

    if (!order?.id || !order?.customer?.id || order.customer.id !== customerId) {
        return { success: false, error: 'Order ownership missing' }
    }

    return { success: true, orderId: order.id }
})
```

前端成功后：

```js
location.href = `/api/payment/alipay?orderId=${encodeURIComponent(orderId)}`
```

禁止：

- 把 `alipayForm.charge` 放进 Checkout API，再把 `renderHtml` 用客户端 `innerHTML` 注入；
- 使用页面本地缓存的过期 `cartId`，而不读 `cart.currentId`；
- 未登录或 cart 无归属时仍跳转支付页。

## 4. 支付端点（服务端直接输出表单）

支付宝网页支付需要在**服务端响应**中输出 `renderHtml`。推荐创建独立 Payment API/CodeBlock，让浏览器直接导航到它；不要在 View 中加入 `<script env="server">`，也不要先 fetch JSON 再用客户端 `innerHTML` 注入。

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

    // requestId is a recovery hint only. Result/status APIs must still verify order ownership.
    k.cookie.set(`payment_request_${order.id}`, response.requestId, 1)
    k.response.write(response.nextAction.renderHtml)
    return null
})
```

完整结构见 [Alipay Payment Guide](../K-Script/payment/alipay.md)。强制点：

- 校验 `order.customer.id === customer.currentId`；
- `charge({ order: orderId, ... })` 必须带 `order`；
- 保留 `response.requestId`；
- `returnUrl` 指向订单结果页，例如 `/order-result?orderId=...`；
- 未配置支付宝密钥时，向用户说明需在后台 Payment 集成中配置，并标记未完成；不要伪造成功。

常见运行时错误对照：

| 现象 | 优先排查 |
| --- | --- |
| Order not found | 未登录、orderId 错、或 `order.customer.id` 为空/不匹配 |
| Cart not found | 使用了过期 cartId，或未读 `cart.currentId` |
| Object reference / 非 JSON | 支付页对空 order/null charge 未判空，或 API 抛错被前端当 JSON 解析 |
| provider 显示成功但 Commerce 仍待支付 | 按 `requestId` 检查 `PaymentRequest`、provider callback/notify 与 `checkStatus`；不要在已有 provider 成功证据时继续猜测「缺少密钥」 |
| 回跳页空白 | 结果页未按 orderId 重新读订单；或强制登录导致回跳会话判断失败 |

## 5. 订单结果页

`returnUrl` 只表示浏览器回来了。结果页必须重新读取真实状态：

- 用 query 中的 `orderId` 调订单详情/状态 API；
- 展示 `paid` / `pending` / `failed` / `canceled`；
- pending 时可保留 `requestId` 做 `checkStatus`，但仍不得调用 `order.pay`；
- 提供「查看订单详情」「继续支付」（仅未支付且校验所有权后）等合法入口。

订单结果页读取订单也必须验证 `order.customer.id === customer.currentId`。不得仅凭公开 `orderId` 向未登录用户返回订单摘要。若业务确实需要无登录回跳展示，必须另行设计短期、签名、绑定 order/request 且可过期的 return credential；这不是默认实现，也不能削弱继续支付、取消、账户订单列表等操作的 ownership。

## 6. Account / Orders（首交付必含）

online-checkout 首交付不得缺少：

- Account 最近订单列表：`order.list({ customerId: currentId })`；
- 每条订单的支付/履约状态；
- 进入订单详情的入口（`/order-detail?orderId=`）；
- 详情页商品行、金额、地址、状态与合法恢复操作。

不要用未按顾客过滤的泛化 `order.list()`，也不要接受请求体里的任意 `customerId`。

## 7. 配置入口与完成表述

- 支付密钥位于后台服务集成 Payment。没有专用且已验证的跳转能力时，只说明人工配置位置并保留未完成证据；不要猜测后台 URL，不要把通用 `show_action_button` 当成 Payment 配置快捷入口。
- Preflight PASS / `structurally verified` ≠ 支付完成。
- 只有同路径走通「下单 → 支付页出表单 →（沙箱/真实）回调或 checkStatus → 前台与后台订单均为已支付，且 Account 能看到该单」才可表述 `runtime verified` / `business verified`。
- provider 已显示成功但 Commerce Order 仍 pending 时，优先检查同一 `requestId` 的 `PaymentRequest`、callback/notify 和 `checkStatus` 结果；不要退回到没有证据的「密钥缺失」猜测。

## 相关参考

- 登录态：[Commerce 顾客登录与下单流程](commerce-customer-login-flow.md)
- 顾客地址与收货地址：[Commerce 顾客地址与收货地址契约](commerce-customer-address-flow.md)
- 动态化总契约：[Commerce 站点动态化契约](site-building/commerce-dynamicization.md)
- 支付总览：[Payment Guide](../K-Script/payment/index.md)
- 支付宝：[Alipay Payment Guide](../K-Script/payment/alipay.md)
- 订单：[Commerce Order](../K-Script/commerce/order.md)
- 购物车：[Commerce Cart](../K-Script/commerce/cart.md)
