# Commerce Cart

::: warning 登录前置
`cart` 属于 commerce 顾客流程的一部分。若当前需求最终会进入 `order` 或其他站点侧顾客业务链路，登录必须先走 `k.commerce.customer.login(...)`，完整流程见 [Commerce 顾客登录与下单流程](../../Scenario/commerce-customer-login-flow.md)。
:::

::: tip 状态来源
commerce 场景下，默认使用服务端维护的 `kb_commerce` 作为当前 cart/customer/order 上下文来源。Account、Header、Cart、Checkout 不要再维护第二套 `commerce_token` 或 `localStorage` 登录态；只有用户明确要求独立应用鉴权时，额外 token 才能作为附加授权层，且不能替代 `kb_commerce`。
:::

## currentId

获取 `kb_commerce` 中的当前购物车 ID。

```ts
k.api.get(() => {
    return { cartId: k.commerce.cart.currentId || '' }
})
```

Header 数量、Cart 页面和 Checkout 应从同一个当前购物车读取数据，避免各自维护不一致的 cart ID。

::: danger 顾客归属
`cart.create()` 会把当时的 `ECommerceState.CustomerId` 写入购物车；`customer.login()` 也会把当前 `ECommerceState.CartId` 对应的购物车绑定到登录顾客。Checkout 前仍必须确认 `cart.customerId === customer.currentId`。若不一致，优先排查会话/cart 漂移或客户端旧 cartId，不要默认重建购物车或发明绑定 API。见 [结算与支付黄金路径](../../Scenario/commerce-checkout-payment-flow.md)。
:::

## create()

创建购物车。

```ts
k.api.post(() => {
    const cartId = k.commerce.cart.create({
        extensionButton: { text: '测试', url: '/test' },
        insuranceAmount: 100
    })
    return { cartId }
})
```
**返回：** `string` — 购物车 ID

## get()

获取购物车基本信息（不含金额计算）。

```ts
k.api.get(() => {
    return k.commerce.cart.get(cartId)
})
```
**参数：** `cartId: string` | **返回：** `Cart | null`

## getDetail()

获取购物车详情（包含金额计算）。

```ts
k.api.get(() => {
    return k.commerce.cart.getDetail(cartId, {
        country: 'China',
        province: 'Guangdong',
        city: 'Guangzhou'
    })
})
```
**参数：**

| 参数 | 类型 | 说明 |
|------|------|------|
| cartId | `string` | 购物车 ID |
| options.currency | `string` | 货币代码（可选） |
| options.country | `string` | 国家（可选） |
| options.province | `string` | 省份（可选） |
| options.city | `string` | 城市（可选） |

**返回：** `CartDetail | null`

国家、省份和城市使用 Commerce 行政区划对象的英文 `name`；选项与显示规则见 [Address Guide](address.md)。
同时提供 `province` 和 `city` 时，配送区域使用 `province`；只有未提供 `province` 时才使用 `city`。

### CartDetail 展示字段

- Header 数量：`detail.totalQuantity`，不要用 `detail.lines.length` 冒充商品总数量。
- 行单价：`line.price`；原始单价：`line.originalPrice`。
- 行金额：`line.amount`。
- Cart 小计/总计：`detail.subtotalAmount` / `detail.totalAmount`。
- `unitPrice`、`totalPrice` 不是 `CartDetail.lines` 的字段；使用它们会出现“总额正确但商品明细价格错误”。

## addOrUpdateLine()

添加或更新购物车商品。`quantity` 是该行写入后的**最终绝对数量**，不是在现有数量上累加；运行时会直接把它赋给购物车行。

```ts
k.api.post(() => {
    const product = k.commerce.product.get('test-product')
    // 仅适用于确认只有一个默认变体的商品；多变体商品须先匹配用户选项。
    const variantId = product.variants[0].id
    k.commerce.cart.addOrUpdateLine(cartId, variantId, 2, {
        note: 'test line',
        groupName: '捆绑组1',
        isMain: true
    })
    return { success: true }
})
```
**参数：**

| 参数 | 类型 | 说明 |
|------|------|------|
| cartId | `string` | 购物车 ID |
| variantId | `string` | 商品变体 ID |
| quantity | `number` | 写入后的最终绝对数量 |
| options.note | `string` | 备注（可选） |
| options.groupName | `string` | 捆绑产品组名（可选） |
| options.isMain | `boolean` | 是否为主产品（可选） |
| options.extensionButton | `object` | 扩展按钮（可选） |

### 数量写入语义

- Cart 数量编辑器：直接传用户期望的最终数量，例如从 2 改成 3 时传 `3`。
- PDP“再加 N 件”：服务端先读取当前行数量，再传 `existingQuantity + requestedQuantity`；不要把 `N` 直接交给 `addOrUpdateLine(...)` 后期待它自动累加。
- 最终数量 `<= 0`：调用 `removeLine(...)`，不要继续写入非正数数量。
- 统一 API 解析数量时必须区分“未传/非法”和合法的 `0`。禁止用 `body.quantity || 1`，因为它会把删除语义的 `0` 改成 `1`。

PDP 前端可以继续只发送本次请求的 `quantity`，但 `/api/cart/add` 服务端端点必须明确拥有“读取现有行并累加”的语义：

```ts
k.api.post((body) => {
    const cartId = k.commerce.cart.currentId || k.commerce.cart.create()
    const requestedQuantity = Number(body.quantity)
    if (!Number.isFinite(requestedQuantity) || requestedQuantity <= 0) {
        return { success: false, error: 'quantity must be a positive number' }
    }

    const detail = k.commerce.cart.getDetail(cartId)
    const existingLine = detail.lines.find((line) => line.variantId === body.variantId)
    const finalQuantity = (existingLine?.quantity || 0) + requestedQuantity
    k.commerce.cart.addOrUpdateLine(cartId, body.variantId, finalQuantity)
    return { success: true, quantity: finalQuantity }
})
```

### PDP 加购前端标准路径

服务端先把默认 variant 渲染到 DOM：

```html
<button id="addToCart"
  k-attribute="data-product-id product_detail.id; data-variant-id product_detail.variants[0].id">
  Add to cart
</button>
```

客户端只读取 DOM，并在成功后通知共享 Header：

```js
const button = document.querySelector('#addToCart')

button.addEventListener('click', async () => {
    const selected = document.querySelector('[name="variantId"]:checked')
    const variantId = selected?.value || button.dataset.variantId
    if (!variantId) return

    const originalText = button.textContent
    button.disabled = true
    button.textContent = 'Adding...'
    try {
        const response = await fetch('/api/cart/add', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ variantId, quantity: 1 })
        })
        if (!response.ok) throw new Error('Add to cart failed')
        window.dispatchEvent(new CustomEvent('kooboo:cart-changed'))
    } finally {
        button.disabled = false
        button.textContent = originalText
    }
})
```

禁止：

- 客户端直接读取 `product_detail.variants[0].id` 等 k-data 服务端模板变量；
- 页面没有 radio 时仍强制读取 `input[name="variant"]:checked`；
- API 失败后按钮永久停留在 loading；
- 加购成功只修改 PDP 局部数字，不刷新共享 Header 的权威 cart 状态。

Header 监听同一个事件：

```js
async function refreshCartCount() {
    const response = await fetch('/api/cart/get')
    const detail = await response.json()
    document.querySelector('#cartCount').textContent = String(detail.totalQuantity || 0)
}

window.addEventListener('kooboo:cart-changed', refreshCartCount)
refreshCartCount()
```

## removeLine()

删除购物车商品。

```ts
k.api.post(() => {
    k.commerce.cart.removeLine(cartId, variantId)
})
```
**参数：**

| 参数 | 类型 | 说明 |
|------|------|------|
| cartId | `string` | 购物车 ID |
| variantId | `string` | 商品变体 ID |
| groupName | `string` | 捆绑产品组名（可选） |

如果删除的是捆绑组主行，运行时会删除该组的所有行；否则只删除匹配的行。

## updateContact()

更新购物车顾客联系信息。

```ts
k.api.post(() => {
    k.commerce.cart.updateContact(cartId, 'test@test.com')
})
```
::: tip 注意
如果 email 匹配 customer 记录，会绑定对应的 customerId；否则会清空购物车的 customerId，仅保留 email。
:::

::: warning 使用边界
`k.commerce.cart.updateContact(...)` 只负责补充购物车联系信息，不负责建立顾客登录态。不要把它当作 `k.commerce.customer.login(...)` 的替代方案。
:::

## updateDiscountCodes()

更新购物车优惠券代码。

```ts
k.api.post(() => {
    const discount = k.commerce.discount.list()[0]
    k.commerce.cart.updateDiscountCodes(cartId, [discount.code])
})
```
**参数：**

| 参数 | 类型 | 说明 |
|------|------|------|
| cartId | `string` | 购物车 ID |
| discountCodes | `string[]` | 优惠券代码数组 |

## updateShipping()

更新购物车配送方式。

```ts
k.api.post(() => {
    k.commerce.cart.updateShipping(cartId, shippingId)
})
```
**参数：**

| 参数 | 类型 | 说明 |
|------|------|------|
| cartId | `string` | 购物车 ID |
| shippingId | `string` | 配送方式 ID |

## updateDigitalShipping()

更新数字配送方式。

```ts
k.api.post(() => {
    k.commerce.cart.updateDigitalShipping(cartId, shippingId)
})
```
**参数：**

| 参数 | 类型 | 说明 |
|------|------|------|
| cartId | `string` | 购物车 ID |
| shippingId | `string` | 数字配送方式 ID |

## updateAdditional()

更新购物车附加信息（积分、保险、扩展按钮）。

```ts
k.api.post(() => {
    k.commerce.cart.updateAdditional(cartId, {
        redeemPoints: true,
        extensionButton: { text: '测试', url: '/test' },
        insuranceAmount: 100
    })
})
```
**参数：**

| 参数 | 类型 | 说明 |
|------|------|------|
| cartId | `string` | 购物车 ID |
| options.redeemPoints | `boolean` | 是否使用积分（可选） |
| options.extensionButton | `object` | 扩展按钮（可选） |
| options.insuranceAmount | `number` | 保险金额（可选） |

## 类型提示

如果你不确定 Cart / CartItem 的具体字段，用 [Cart Types](types/cart.md) 获取完整类型定义。

## 边界说明

- 订单创建（需要先有 cart）→ [Order Guide](order.md)
- 配送地区值 → [Address Guide](address.md)
- commerce 顾客登录与下单主链路 → [Commerce 顾客登录与下单流程](../../Scenario/commerce-customer-login-flow.md)
- 本 guide 不展开订单流程细节
