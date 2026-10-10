# customer

> 客户管理

## 概述

`k.commerce.customer` 提供客户相关操作，包括创建、查询、登录、订单管理等功能。

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

**参数：**

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| value | `object` | 是 | 顾客信息 |
| value.email | `string` | 是 | 邮箱 |
| value.firstName | `string` | 否 | 名 |
| value.lastName | `string` | 否 | 姓 |
| value.password | `string` | 否 | 密码 |
| value.phone | `string` | 否 | 电话 |

**返回：** `Customer`

## get()

根据客户 ID 获取顾客。

```ts
k.api.get(() => {
    const customer = k.commerce.customer.get('customer-id')
    return { id: customer.id, email: customer.email }
})
```

**参数：**

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| id | `string` | 是 | 顾客ID |

**返回：** `Customer`

## getByEmail()

根据邮箱获取顾客。

```ts
k.api.get(() => {
    const customer = k.commerce.customer.getByEmail('test@example.com')
    return { id: customer.id, email: customer.email }
})
```

**参数：**

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| email | `string` | 是 | 顾客邮箱 |

**返回：** `Customer`

## getByPhone()

根据手机号获取顾客。

```ts
k.api.get(() => {
    const customer = k.commerce.customer.getByPhone('15291101010')
    return { id: customer.id, phone: customer.phone }
})
```

**参数：**

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| phone | `string` | 是 | 手机号 |

**返回：** `Customer`

## login()

顾客登录。

```ts
k.api.post(() => {
    const result = k.commerce.customer.login('test@example.com', '123456')
    if (!result) {
        return { error: 'email or password is incorrect' }
    }
    return { id: result.id, email: result.email }
})
```

**参数：**

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| email | `string` | 是 | 邮箱 |
| password | `string` | 是 | 密码 |

**返回：** `Customer | string` - 登录成功返回 Customer，失败返回错误信息

## setPassword()

设置顾客密码。

```ts
k.api.post(() => {
    k.commerce.customer.setPassword('customer-id', 'new-password')
    return 'password set success'
})
```

**参数：**

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| customerId | `string` | 是 | 顾客ID |
| password | `string` | 是 | 新密码 |

**返回：** `Customer`

## setOAuth()

设置顾客的 OAuth 信息。

```ts
k.api.post(() => {
    k.commerce.customer.setOAuth('customer-id', 'open-id-123', 'Apple')
    return 'set OAuth success'
})
```

**参数：**

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| customerId | `string` | 是 | 顾客ID |
| openId | `string` | 是 | 第三方平台的 openId |
| type | `string` | 是 | 第三方平台类型（如 `Apple`） |

**返回：** `void`

## getByOAuth()

根据 OAuth 信息获取顾客。

```ts
k.api.get(() => {
    const customer = k.commerce.customer.getByOAuth('open-id-123', 'Apple')
    return { id: customer.id, email: customer.email }
})
```

**参数：**

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| openId | `string` | 是 | 第三方平台 openId |
| type | `string` | 是 | 第三方平台类型 |

**返回：** `Customer`

## oAuthList()

获取顾客绑定的 OAuth 列表。

```ts
k.api.get(() => {
    const list = k.commerce.customer.oAuthList('customer-id')
    return { count: list.length, items: list }
})
```

**参数：**

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| customerId | `string` | 是 | 顾客ID |

**返回：** `CustomerOAuthModel[]`

## orders()

获取顾客的订单列表。

```ts
k.api.get(() => {
    const orders = k.commerce.customer.orders('customer-id', {
        pageIndex: 1,
        pageSize: 10,
        paid: true,
        delivered: true,
        canceled: false
    })
    return { count: orders.count, orders: orders.list }
})
```

**参数：**

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| customerId | `string` | 是 | 顾客ID |
| query | `object` | 否 | 查询参数 |
| query.pageIndex | `number` | 否 | 页码，默认 1 |
| query.pageSize | `number` | 否 | 每页条数，默认 10 |
| query.paid | `boolean` | 否 | 是否已支付 |
| query.delivered | `boolean` | 否 | 是否已发货 |
| query.canceled | `boolean` | 否 | 是否已取消 |
| query.startDate | `Date` | 否 | 起始时间 |
| query.endDate | `Date` | 否 | 截止时间 |

**返回：**

```ts
{
    list: OrderDetail[];
    count: number;
    pageIndex: number;
    pageSize: number;
}
```

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
    return 'update success'
})
```

**参数：**

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| customerId | `string` | 是 | 顾客ID |
| info | `object` | 是 | 更新内容 |
| info.email | `string` | 否 | 邮箱 |
| info.firstName | `string` | 否 | 名 |
| info.lastName | `string` | 否 | 姓 |
| info.phone | `string` | 否 | 电话 |
| info.tags | `string[]` | 否 | 标签 |

**返回：** `void`

## updateAddresses()

更新顾客的地址列表。

::: tip 注意
本操作为覆盖写入，即传入的数组就是最终的地址列表
:::

```ts
k.api.post(() => {
    k.commerce.customer.updateAddresses('customer-id', [{
        country: '中国',
        province: '福建省',
        city: '厦门市',
        address1: 'address1',
        address2: 'address2',
        zip: '361000',
        firstName: 'firstName',
        lastName: 'lastName',
        phone: '13800138000',
        isDefault: true
    }])
    return 'update success'
})
```

**参数：**

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| customerId | `string` | 是 | 顾客ID |
| addresses | `AddressModel[]` | 是 | 地址列表 |

**返回：** `void`

## remove()

删除顾客。

```ts
k.api.post(() => {
    k.commerce.customer.remove('customer-id')
    return 'remove success'
})
```

**参数：**

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| customerId | `string` | 是 | 顾客ID |

**返回：** `void`

## 结构

### Customer 属性

| 属性 | 说明 | 类型 |
|------|------|------|
| id | 客户ID | `string` |
| email | 客户邮箱 | `string` |
| firstName | 客户姓名 | `string` |
| lastName | 客户姓氏 | `string` |
| phone | 客户电话 | `string` |
| password | 客户密码 | `string` |
| lastPlaceOrderDate | 最后下单时间 | `string` |
| addresses | 地址列表 | [`AddressModel[]`](#addressmodel-属性) |
| tags | 标签列表 | `string[]` |
| createdAt | 创建时间 | `string` |
| updatedAt | 更新时间 | `string` |

### AddressModel 属性

| 属性 | 说明 | 类型 |
|------|------|------|
| id | 地址ID | `string` |
| isDefault | 是否默认地址 | `boolean` |
| country | 国家 | `string` |
| province | 省份 | `string` |
| city | 城市 | `string` |
| address1 | 地址1 | `string` |
| address2 | 地址2 | `string` |
| firstName | 收货人姓名 | `string` |
| lastName | 收货人姓氏 | `string` |
| phone | 电话 | `string` |
| zip | 邮编 | `string` |

### CustomerOAuthModel 属性

| 属性 | 说明 | 类型 |
|------|------|------|
| CustomerId | 顾客ID | `string` |
| OpenId | 第三方平台 openId | `string` |
| Type | 第三方平台类型 | `string` |
| CreatedAt | 创建时间 | `string` |

## 相关文档

- [k.commerce](./commerce.md) - 电商模块概述
- [order](./order.md) - 订单管理
- [cart](./cart.md) - 购物车
