# order

> 订单

## 概述

`k.commerce.order` 提供订单相关操作，包括查询、创建、支付、发货、取消等功能。

## list()

获取订单列表。

```ts
k.api.get(() => {
    const result = k.commerce.order.list({
        startDate: new Date('2025-01-01'),
        endDate: new Date('2025-12-31'),
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

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| query.customerId | `string` | 否 | 顾客ID |
| query.paid | `boolean` | 否 | 是否已支付 |
| query.delivered | `boolean` | 否 | 是否已发货 |
| query.canceled | `boolean` | 否 | 是否已取消 |
| query.startDate | `Date` | 否 | 起始时间 |
| query.endDate | `Date` | 否 | 截止时间 |
| query.pageIndex | `number` | 否 | 页码，默认 1 |
| query.pageSize | `number` | 否 | 每页条数，默认 10 |

**返回：**

```ts
{
    list: OrderDetail[];  // 订单列表
    count: number;        // 总数量
    pageIndex: number;    // 当前页码
    pageSize: number;     // 每页条数
}
```

## get()

获取指定订单详情。

```ts
k.api.get(() => {
    const order = k.commerce.order.get('order-id')
    return { id: order.id, totalAmount: order.totalAmount }
})
```

**参数：**

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| orderId | `string` | 是 | 订单ID |

**返回：** `OrderDetail`

## create()

创建订单。

```ts
k.api.post(() => {
    // 创建购物车并添加商品
    const cartId = k.commerce.cart.create()
    const product = k.commerce.product.create({
        title: 'Order Test Product',
        description: 'Test',
        price: 99.9,
        active: true,
        seoName: 'order-test-' + Date.now(),
        featuredImage: ''
    } as any)
    k.commerce.product.createVariant(product.id, {
        sku: 'ORD-SKU-001',
        price: 99.9,
        inventory: 100,
        barcode: '',
        active: true,
        selectedOptions: []
    } as any)
    const refreshed = k.commerce.product.get(product.id)
    k.commerce.cart.addOrUpdateLine(cartId, refreshed.variants[0].id, 1)

    // 创建订单
    const order = k.commerce.order.create(cartId, {
        address: {
            country: '中国',
            province: '福建省',
            city: '厦门市',
            address1: 'address1',
            address2: '',
            zip: '361000',
            firstName: 'firstName',
            lastName: 'lastName',
            phone: '13800138000',
            isDefault: true
        },
        note: 'test order'
    })
    return { orderId: order.id }
})
```

**参数：**

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| cartId | `string` | 是 | 购物车ID |
| options | `object` | 是 | 订单选项 |
| options.address | `Address` | 是 | 收货地址 |
| options.extensionFields | `KeyValue[]` | 否 | 扩展字段 |
| options.note | `string` | 否 | 备注 |
| options.scheduledDeliveryTime | `Date` | 否 | 预计发货时间 |

**返回：** `OrderDetail`

## cancel()

取消订单。

```ts
k.api.post(() => {
    k.commerce.order.cancel('order-id', 'cancel reason')
    return 'success'
})
```

**参数：**

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| orderId | `string` | 是 | 订单ID |
| reason | `string` | 是 | 取消原因 |

**返回：** `void`

## delete()

删除订单。

```ts
k.api.post(() => {
    k.commerce.order.delete('order-id')
    return 'success'
})
```

**参数：**

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| orderId | `string` | 是 | 订单ID |

**返回：** `void`

## pay()

支付订单。

```ts
k.api.post(() => {
    k.commerce.order.pay('order-id', 'alipay')
    return 'success'
})
```

**参数：**

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| orderId | `string` | 是 | 订单ID |
| paymentMethod | `string` | 是 | 支付方式 |

**返回：** `void`

## delivery()

订单发货。

```ts
k.api.post(() => {
    const shipping = k.commerce.shipping.list()[0]
    k.commerce.order.delivery('order-id', shipping.id, 'SF123456789')
    return 'success'
})
```

**参数：**

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| orderId | `string` | 是 | 订单ID |
| shippingCarrier | `string` | 是 | 物流公司 |
| trackingNumber | `string` | 是 | 物流单号 |

**返回：** `void`

## reopen()

重新打开已取消的订单。

```ts
k.api.post(() => {
    k.commerce.order.reopen('order-id')
    return 'success'
})
```

**参数：**

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| orderId | `string` | 是 | 订单ID |

**返回：** `void`

## updateAddress()

更新订单收货地址。

```ts
k.api.post(() => {
    k.commerce.order.updateAddress('order-id', {
        country: '中国',
        province: '福建省',
        city: '厦门市',
        address1: 'address1',
        address2: '',
        zip: '361000',
        firstName: 'firstName',
        lastName: 'lastName',
        phone: '13800138000',
        isDefault: true
    })
    return 'success'
})
```

**参数：**

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| orderId | `string` | 是 | 订单ID |
| address | `Address` | 是 | 收货地址 |

**返回：** `void`

## updateShippingInfo()

更新订单配送信息。

```ts
k.api.post(() => {
    const order = k.commerce.order.get('order-id')
    const lineId = order.lines[0].id
    const shipping = k.commerce.shipping.list()[0]

    k.commerce.order.updateShippingInfo('order-id', lineId, {
        shippingCarrier: shipping.id,
        trackingNumber: 'SF123456789'
    })
    return 'success'
})
```

**参数：**

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| orderId | `string` | 是 | 订单ID |
| orderLineId | `string` | 是 | 订单商品ID |
| options | `object` | 是 | 配送选项 |
| options.shippingCarrier | `string` | 否 | 物流公司 |
| options.trackingNumber | `string` | 否 | 物流单号 |
| options.digitalItems | `DigitalOrderItem[]` | 否 | 数字产品 |

**返回：** `void`

## 结构

### OrderDetail 属性

| 属性 | 说明 | 类型 |
|------|------|------|
| id | 订单ID | `string` |
| customer | 顾客信息 | [`Customer`](#customer-属性) |
| pointsDeductionAmount | 积分扣除金额 | `number` |
| earnPoints | 赚取积分 | `number` |
| redeemPoints | 兑换积分 | `number` |
| totalAmount | 订单总金额 | `number` |
| taxAmount | 税额 | `number` |
| originalAmount | 原始金额 | `number` |
| shippingAmount | 配送金额 | `number` |
| insuranceAmount | 保险金额 | `number` |
| subtotalAmount | 小计金额 | `number` |
| originalSubtotalAmount | 原始小计金额 | `number` |
| totalQuantity | 总数量 | `number` |
| currency | 货币 | `string` |
| paid | 是否已支付 | `boolean` |
| paidAt | 支付时间 | `Date` |
| paymentMethod | 支付方式 | `string` |
| delivered | 是否已发货 | `boolean` |
| partialDelivered | 是否部分发货 | `boolean` |
| trackingNumber | 物流单号 | `string` |
| shippingCarrier | 物流公司 | `string` |
| shippingAt | 发货时间 | `Date` |
| scheduledDeliveryTime | 预计发货时间 | `Date` |
| canceled | 是否已取消 | `boolean` |
| cancelReason | 取消原因 | `string` |
| cancelAt | 取消时间 | `Date` |
| createdAt | 创建时间 | `Date` |
| updatedAt | 更新时间 | `Date` |
| shippingAddress | 发货地址 | [`Address`](#address-属性) |
| note | 备注 | `string` |
| lines | 订单商品 | [`OrderLine[]`](#orderline-属性) |
| ip | IP地址 | `string` |
| country | 国家 | `string` |
| source | 来源 | `string` |
| clientInfo | 客户端信息 | [`ClientInfo`](#clientinfo-属性) |
| extensionButton | 扩展按钮 | `{ text: string, url: string }` |
| discountAllocations | 自动折扣信息 | `DiscountAllocation[]` |
| shippingAllocations | 配送方式列表 | `ShippingAllocation[]` |
| extensionFields | 扩展字段 | `KeyValue[]` |

### Customer 属性

| 属性 | 说明 | 类型 |
|------|------|------|
| id | 客户ID | `string` |
| email | 客户邮箱 | `string` |
| firstName | 客户姓名 | `string` |
| lastName | 客户姓氏 | `string` |
| phone | 客户电话 | `string` |

### Address 属性

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

### OrderLine 属性

| 属性 | 说明 | 类型 |
|------|------|------|
| id | 订单商品ID | `string` |
| totalAmount | 订单商品总金额 | `number` |
| taxAmount | 订单商品税额 | `number` |
| originalAmount | 订单商品原始金额 | `number` |
| quantity | 订单商品数量 | `number` |
| totalQuantity | 订单商品总数量 | `number` |
| price | 订单商品价格 | `number` |
| originalPrice | 订单商品原价 | `number` |
| title | 订单商品名称 | `string` |
| image | 订单商品图片 | `string` |
| productId | 商品ID | `string` |
| variantId | 变体ID | `string` |
| sku | 商品SKU | `string` |
| orderId | 订单ID | `string` |
| options | 商品选项 | `{ name: string, value: string }[]` |
| discountAllocations | 自动折扣信息 | `DiscountAllocation[]` |
| groupName | 商品组名称 | `string` |
| isMain | 是否为主商品 | `boolean` |
| note | 订单商品备注 | `string` |
| extensionButton | 扩展按钮 | `{ text: string, url: string }` |
| trackingNumber | 物流单号 | `string` |
| shippingCarrier | 物流公司 | `string` |
| shippingAt | 发货时间 | `Date` |
| delivered | 是否已发货 | `boolean` |
| digitalItems | 数字产品列表 | `DigitalOrderItem[]` |
| autoDelivery | 是否自动发货 | `boolean` |
| isDigital | 是否为数字产品 | `boolean` |
| maxDownloadCount | 最大下载次数 | `number` |
| maxDownloadDay | 最大下载天数 | `number` |
| errorMessage | 错误信息 | `string` |

### ClientInfo 属性

| 属性 | 说明 | 类型 |
|------|------|------|
| platform | 平台 | `string` |
| oS | 操作系统 | `string` |
| device | 设备 | `string` |
| application | 应用信息 | [`ApplicationInfo`](#applicationinfo-属性) |

### ApplicationInfo 属性

| 属性 | 说明 | 类型 |
|------|------|------|
| isWebBrowser | 是否为网页浏览器 | `boolean` |
| name | 应用名称 | `string` |
| version | 应用版本 | `string` |

## 相关文档

- [k.commerce](./commerce.md) - 电商模块概述
- [cart](./cart.md) - 购物车
- [product](./product.md) - 产品管理
