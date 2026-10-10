# Commerce Customer

## create()

创建顾客。

```ts
k.api.post(() => {
    const customer = k.commerce.customer.create({
        email: 'test@example.com',
        firstName: 'Test',
        lastName: 'User',
        password: '123456',
        phone: '15291101010'
    })
    return { id: customer.id, email: customer.email }
})
```
**必填：** `email`

::: tip 注意
`email` 是 `k.commerce.customer` 的唯一标识。  
如果当前需求会继续进入 `product` / `category` / `cart` / `order` 这条站点侧 commerce 顾客链路，注册后应直接配合 `k.commerce.customer.login(...)` 使用，而不是走其他登录体系后再补接到 commerce。
:::

## get()

根据 ID 获取顾客。

```ts
k.api.get(() => {
    const customer = k.commerce.customer.get('customer-id')
    if (!customer) return { error: 'customer not found' }
    return { id: customer.id, email: customer.email }
})
```
**参数：** `id: string` | **返回：** `Customer | null`

## getByEmail()

根据邮箱获取顾客。

```ts
k.api.get(() => {
    const customer = k.commerce.customer.getByEmail('test@example.com')
    if (!customer) return { error: 'customer not found' }
    return { id: customer.id }
})
```
**参数：** `email: string` | **返回：** `Customer | null`

::: tip 使用建议
这是顾客查询入口，不是 commerce 登录入口。更适合后台查询、资料同步、订单追踪或读取已知顾客，而不是替代 `k.commerce.customer.login(...)` 去建立站点侧顾客会话。
:::

## getByPhone()

根据手机号获取顾客。

```ts
k.api.get(() => {
    const customer = k.commerce.customer.getByPhone('15291101010')
    if (!customer) return { error: 'customer not found' }
    return { id: customer.id }
})
```
**参数：** `phone: string` | **返回：** `Customer | null`

## login()

顾客登录。成功后会把 CustomerId 写入当前 Commerce 状态；后续请求通过 `kb_commerce` 恢复。

```ts
k.api.post((body) => {
    const email = (body.email || '').trim().toLowerCase()
    const password = body.password || ''

    const result = k.commerce.customer.login(email, password)
    if (!result) {
        return { success: false, error: 'email or password is incorrect' }
    }

    return {
        success: true,
        authenticated: true,
        id: result.id,
        email: result.email
    }
})
```

::: tip 默认入口
只要需求属于 commerce 的站点侧顾客流程，`k.commerce.customer.login(...)` 就是默认登录入口。不要再把 Kooboo 账号登录、SSO、OAuth 或 sqlite 自定义登录当成同一条 commerce 顾客链路。
:::

::: tip 默认不要再造第二套登录态
Account、Header、Cart 和 Order 默认通过 `customer.currentId` / `kb_commerce` 共享状态，不需要额外 JWT 或 `localStorage`。只有明确存在独立应用鉴权需求时才额外签发 token。
:::

## currentId

读取当前 `kb_commerce` 中的顾客 ID。

```ts
k.api.get(() => {
    const customerId = k.commerce.customer.currentId || ''
    if (!customerId) return { authenticated: false, customer: null }
    const customer = k.commerce.customer.get(customerId)
    return { authenticated: !!customer, customer }
})
```

## logout()

清除当前 Commerce customer/cart/order 状态。

```ts
k.api.post(() => {
    k.commerce.customer.logout()
    return { success: true, authenticated: false }
})
```

## setPassword()

设置顾客密码。

```ts
k.api.post(() => {
    k.commerce.customer.setPassword('customer-id', 'new-password')
    return 'password set success'
})
```

## setOAuth()

绑定第三方 OAuth。

```ts
k.api.post(() => {
    k.commerce.customer.setOAuth('customer-id', 'open-id-123', 'Apple')
    return 'set OAuth success'
})
```

## getByOAuth()

根据 OAuth 信息获取顾客。

```ts
k.api.get(() => {
    const customer = k.commerce.customer.getByOAuth('open-id-123', 'Apple')
    if (!customer) return { error: 'customer not found' }
    return { id: customer.id }
})
```
**返回：** `Customer | null`

## oAuthList()

获取顾客绑定的 OAuth 列表。

```ts
k.api.get(() => {
    const list = k.commerce.customer.oAuthList('customer-id')
    return { count: list.length }
})
```

## orders()

获取顾客的订单列表。

```ts
k.api.get(() => {
    const customerId = k.commerce.customer.currentId || ''
    if (!customerId) return { authenticated: false, count: 0, orders: [] }

    const orders = k.commerce.customer.orders(customerId, {
        pageIndex: 1,
        pageSize: 10,
        paid: true
    })
    return { authenticated: true, count: orders?.count || 0, orders: orders?.list || [] }
})
```
公开 Account/Orders API 应使用当前 Commerce 顾客 ID，不要接受请求中的任意 `customerId`。

::: warning 授权边界
这些底层方法不会替调用方执行授权校验。公开接口在暴露或修改顾客数据前，必须验证当前顾客归属；后台接口必须执行相应的管理员授权。
:::

## updateInfo()

更新顾客信息。

```ts
k.api.post(() => {
    k.commerce.customer.updateInfo('customer-id', {
        firstName: 'Updated',
        lastName: 'Name',
        phone: '13900139000',
        tags: ['vip', 'active']
    })
})
```

## updateAddresses()

更新顾客地址列表。**整组覆盖写入**：传入数组会替换当前 `customer.addresses`，不是部分 patch。新增、编辑、删除或设置默认地址前，先读取当前完整列表并保留未修改字段；完整规则见 [Commerce 顾客地址与收货地址契约](../../Scenario/commerce-customer-address-flow.md)。

```ts
k.api.post(() => {
    k.commerce.customer.updateAddresses('customer-id', [{
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
    }])
})
```

## remove()

删除顾客。

```ts
k.api.post(() => {
    k.commerce.customer.remove('customer-id')
})
```

## 类型提示

如果你不确定 Customer / AddressModel 的具体字段，用 [Customer Types](types/customer.md) 获取完整类型定义。

## 边界说明

- 地址查询/行政区划 → [Address Guide](address.md)
- 顾客地址簿与 Checkout 收货地址 → [Commerce 顾客地址与收货地址契约](../../Scenario/commerce-customer-address-flow.md)
- 顾客订单列表依赖 [Order Types](types/order.md)
- commerce 顾客登录与下单主链路 → [Commerce 顾客登录与下单流程](../../Scenario/commerce-customer-login-flow.md)
