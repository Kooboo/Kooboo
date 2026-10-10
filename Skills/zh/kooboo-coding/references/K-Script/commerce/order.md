# Commerce Order

## list()

获取订单列表。

```ts
k.api.get(() => {
    const result = k.commerce.order.list({
        startDate: new Date('2025-01-01T00:00:00Z'),
        endDate: new Date('2025-12-31T23:59:59Z'),
        paid: false,
        delivered: false,
        canceled: false,
        pageIndex: 1,
        pageSize: 10
    })
    return result
})
```
**参数：**

| 参数 | 类型 | 说明 |
|------|------|------|
| query.customerId | `string` | 顾客 ID（可选） |
| query.paid | `boolean` | 是否已支付（可选） |
| query.delivered | `boolean` | 是否已发货（可选） |
| query.canceled | `boolean` | 是否已取消（可选） |
| query.startDate | `Date` | 起始时间（可选） |
| query.endDate | `Date` | 截止时间（可选） |
| query.pageIndex | `number` | 页码，默认 1 |
| query.pageSize | `number` | 每页条数，默认 10 |

**返回：** `{ list: OrderDetail[], count: number, pageIndex: number, pageSize: number }`

### Storefront 订单列表权限

公开 Account/Orders API 不得接受任意 `customerId`。身份必须来自当前 Commerce 会话：

```ts
k.api.get(() => {
    const customerId = k.commerce.customer.currentId || ''
    if (!customerId) return { authenticated: false, list: [], count: 0 }
    return k.commerce.order.list({ customerId, pageIndex: 1, pageSize: 10 })
})
```

## get()

获取指定订单详情。

```ts
k.api.get(() => {
    const order = k.commerce.order.get('order-id')
    if (!order) return { success: false, error: 'Order not found' }
    return { success: true, id: order.id, totalAmount: order.totalAmount }
})
```
**参数：** `orderId: string` | **返回：** `OrderDetail | null`

### 订单行展示字段

- 单价：`line.price`
- 行小计：`line.totalAmount`
- 变体/规格选项：`line.options`

Cart 行使用 `line.amount`，OrderLine 使用 `line.totalAmount`；两者不是同一份展示契约。订单页不要沿用 Cart 模板中的 `amount`，也不要用 `price * quantity` 替代运行时已经计算好的行小计。

### 浏览器时间序列化

订单实体的 `createdAt` 等时间以 UTC 创建并由订单 ViewModel 传出。浏览器 API 必须输出带明确时区的值，优先使用 ISO 8601 UTC（结尾为 `Z`）或 epoch，再交给 `new Date(...)`。不要输出 `2026/8/19 05:23:00` 这类无时区字符串；不同浏览器可能把它解释成本地时间或解析失败。

不要假定某一种 KScript 日期字符串格式在所有序列化边界都固定不变；API 层应显式完成 UTC 序列化。

公开订单详情、继续支付和取消 API 必须验证所有权：

```ts
k.api.get(() => {
    const customerId = k.commerce.customer.currentId || ''
    const orderId = k.request.queryString.get('orderId') || ''
    const order = orderId ? k.commerce.order.get(orderId) : null
    if (!customerId || !order?.customer?.id || order.customer.id !== customerId) {
        return { success: false, error: 'Order not found' }
    }
    return { success: true, order }
})
```

不要根据 `body.customerId`、`query.customerId` 或 URL 中的客户 ID 返回订单。对无权访问者统一返回 not found，避免泄露订单是否存在。

## create()

创建订单（需要先有购物车）。

```ts
k.api.post((body) => {
    const order = k.commerce.order.create(cartId, {
        address: {
            country: 'China',
            province: 'Fujian',
            city: 'Xiamen',
            address1: 'address1',
            address2: '',
            zip: '361000',
            firstName: 'firstName',
            lastName: 'lastName',
            phone: '13800138000',
            isDefault: false
        },
        note: 'test order',
        currency: body.currency
    })
    return { orderId: order.id }
})
```
**参数：**

| 参数 | 类型 | 说明 |
|------|------|------|
| cartId | `string` | 购物车 ID |
| options.address | `Address` | 收货地址（必填） |
| options.note | `string` | 备注（可选） |
| options.currency | `string` | 目标订单币种；未配置时会回退到站点默认货币（可选） |
| options.extensionFields | `KeyValue[]` | 扩展字段（可选） |
| options.scheduledDeliveryTime | `Date` | 计划交付时间（可选） |

**返回：** `OrderDetail`

`options.address` 遵循 [Commerce 顾客地址与收货地址契约](../../Scenario/commerce-customer-address-flow.md)。Checkout 联系电话写入 `address.phone`；复用顾客保存地址时复制完整地址对象，不按当前表单可见字段裁剪。

::: warning 前置约束
`k.commerce.order.create(cartId, ...)` 默认建立在 `k.commerce.customer.login(email, password)` 已成功的链路上。

:::

推荐示例：

```ts
k.api.post((body) => {
    const customerId = k.commerce.customer.currentId || ''
    if (!customerId) {
        return { success: false, error: 'Commerce customer login is required' }
    }

    const cartId = k.commerce.cart.currentId || ''
    if (!cartId) {
        return { success: false, error: 'Commerce cart is required' }
    }

    const cart = k.commerce.cart.get(cartId)
    if (!cart) {
        return { success: false, error: 'Cart not found' }
    }
    if (!cart.customerId || cart.customerId !== customerId) {
        return { success: false, error: 'Cart is not bound to the current Commerce customer' }
    }

    const order = k.commerce.order.create(cartId, {
        address: body.address,
        note: body.note,
        currency: body.currency
    })

    if (!order?.id || !order?.customer?.id || order.customer.id !== customerId) {
        return { success: false, error: 'Order ownership missing; do not start payment' }
    }

    return {
        success: true,
        orderId: order.id,
        customerId: order.customer.id,
        currency: order.currency
    }
})
```

::: danger 订单归属来自购物车
`order.CustomerId` 复制自 `cart.CustomerId`。正常情况下，`customer.login()` 会把当前 Commerce cart 绑定到登录顾客；Checkout 仍需显式比较 `cart.customerId === customer.currentId`，并在创建后比较 `order.customer.id === customer.currentId`。若不一致，排查会话/cart 漂移、旧 cartId 或缺失登录绑定，不要通过删除 ownership 校验来规避「订单不存在」。完整路径见 [Commerce 结算与支付黄金路径](../../Scenario/commerce-checkout-payment-flow.md)。
:::

::: tip 单一 Commerce 状态
登录、购物车和下单默认统一使用 `customer.currentId`、`cart.currentId` 与 `kb_commerce`。不要在下单 API 中再次要求邮箱密码，不要默认创建 `commerce_token`，也不要让 Checkout 与 Header 各自判断不同的登录态。
:::

::: tip 多货币下单
如果订单最终要走某个特定币种的第三方支付，优先在 `order.create(...)` 时就传 `currency`。这样订单金额、订单行金额和后续支付币种可以保持一致。
:::

## pay()

将订单直接标记为已支付。

**参数：**

| 参数 | 类型 | 说明 |
|------|------|------|
| orderId | `string` | 订单 ID |
| paymentMethod | `string` | 支付方式 |

::: danger Storefront 禁止直接调用
`k.commerce.order.pay(...)` 会立即把订单写成已支付、增加商品销量并触发已支付事件；它本身不验证支付提供商签名，也不代表真实资金到账。

不要把它放进公开 GET/POST API、支付 return URL、前端确认按钮、订单结果页或轮询接口。正常 storefront 应调用 `k.payment.xxx.charge({ order: orderId, ... })`，让 Kooboo 内建 provider callback / `checkStatus(...)` 在确认真实支付后更新订单。只有受信任的内部迁移、人工补单或已经完成独立验签的受控后台流程，才可显式使用此方法。
:::

## delivery()

订单发货。

```ts
k.api.post(() => {
    const shipping = k.commerce.shipping.list()[0]
    k.commerce.order.delivery('order-id', shipping.id, 'SF123456789')
})
```
**参数：**

| 参数 | 类型 | 说明 |
|------|------|------|
| orderId | `string` | 订单 ID |
| shippingCarrier | `string` | 物流公司 ID |
| trackingNumber | `string` | 物流单号 |

## cancel()

取消订单。

```ts
k.api.post(() => {
    k.commerce.order.cancel('order-id', 'cancel reason')
})
```
**参数：**

| 参数 | 类型 | 说明 |
|------|------|------|
| orderId | `string` | 订单 ID |
| reason | `string` | 取消原因 |

## reopen()

重新打开已取消的订单。

```ts
k.api.post(() => {
    k.commerce.order.reopen('order-id')
})
```

## updateAddress()

更新订单收货地址。

```ts
k.api.post(() => {
    k.commerce.order.updateAddress('order-id', {
        country: 'China',
        province: 'Fujian',
        city: 'Xiamen',
        address1: 'address1',
        address2: 'Room 1201',
        zip: '361000',
        firstName: 'firstName',
        lastName: 'lastName',
        phone: '13800138000',
        isDefault: true
    })
})
```

## updateShippingInfo()

更新订单配送信息（可更新物流单号、数字产品发货等）。

```ts
k.api.post(() => {
    const order = k.commerce.order.get('order-id')
    const lineId = order.lines[0].id
    const shipping = k.commerce.shipping.list()[0]
    k.commerce.order.updateShippingInfo('order-id', lineId, {
        shippingCarrier: shipping.id,
        trackingNumber: 'SF123456789'
    })
})
```
**参数：**

| 参数 | 类型 | 说明 |
|------|------|------|
| orderId | `string` | 订单 ID |
| orderLineId | `string` | 订单商品 ID |
| options.shippingCarrier | `string` | 物流公司（可选） |
| options.trackingNumber | `string` | 物流单号（可选） |
| options.digitalItems | `DigitalOrderItem[]` | 数字产品（可选） |

## delete()

删除订单。

```ts
k.api.post(() => {
    k.commerce.order.delete('order-id')
})
```

## 类型提示

如果你不确定 Order / OrderLine / Address 的具体字段，用 [Order Types](types/order.md) 获取完整类型定义。

## 边界说明

- 订单创建前需要先有购物车（[Cart Guide](cart.md)）
- `pay()`、`delivery()`、`cancel()`、`reopen()`、地址/配送信息更新和 `delete()` 本身不执行调用方权限校验；公开 API 调用前必须验证顾客所有权或适用的管理员权限。
- 顾客地址簿与订单收货地址字段 → [Commerce 顾客地址与收货地址契约](../../Scenario/commerce-customer-address-flow.md)
- commerce 顾客登录与下单主链路 → [Commerce 顾客登录与下单流程](../../Scenario/commerce-customer-login-flow.md)
- 本 guide 不展开购物车操作细节
