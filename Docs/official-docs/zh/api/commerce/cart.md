# cart

> 购物车

## 概述

`k.commerce.cart` 提供购物车相关操作，支持创建购物车、添加商品、更新配送方式、使用优惠券等功能。

## Cart 数据结构

```ts
interface Cart {
    id: string;              // 购物车ID
    customerId: string;      // 顾客ID
    contact: string;         // 联系邮箱
    country: string;         // 国家
    discountCodes: string[]; // 优惠券代码
    note: string;            // 备注
    lines: CartItem[];       // 购物车商品
    shippingId: string;      // 配送方式ID
    digitalShippingId: string; // 数字配送方式ID
    insuranceAmount: number; // 保险金额
    extensionButton: { text: string; url: string } | null; // 扩展按钮
    redeemPoints: boolean;   // 是否使用积分
    createdAt: Date;         // 创建时间
    updatedAt: Date;         // 更新时间
}
```

## CartDetail 数据结构

```ts
interface CartDetail {
    id: string;
    customerId: string;
    contact: string;
    country: string;
    currency: { code: string; symbol: string; rate: number }; // 货币信息
    discountCodes: string[];
    activeDiscountCodes: string[]; // 激活的优惠券代码
    note: string;
    discountAllocations: DiscountAllocation[]; // 自动折扣信息
    shippingAllocations: ShippingAllocation[]; // 配送方式
    shippingAmount: number;       // 配送金额
    insuranceAmount: number;      // 保险金额
    subtotalAmount: number;       // 小计金额
    totalAmount: number;          // 总金额
    taxAmount: number;            // 税额
    totalQuantity: number;        // 总数量
    redeemPoints: number;         // 积分
    pointsDeductionAmount: number; // 积分抵扣金额
    earnPoints: number;          // 获取积分数额
    shipping: Shipping | null;   // 配送方式
    lines: CartItem[];           // 购物车商品
}
```

## CartItem 数据结构

```ts
interface CartItem {
    variantId: string;            // 商品规格ID
    productId: string;             // 商品ID
    title: string;                // 商品名称
    options: Option[];            // 商品选项
    sku: string;                  // 商品SKU
    image: string;                // 商品图片
    originalPrice: number;        // 商品原价
    price: number;                // 商品价格
    discountAllocations: DiscountAllocation[]; // 自动折扣信息
    quantity: number;             // 商品数量
    totalQuantity: number;        // 商品总数量
    amount: number;               // 商品小计金额
    taxAmount: number;            // 商品税额
    inventory: number;             // 商品库存
    groupName: string;            // 商品组名
    isMain: boolean;              // 是否为主商品
    note: string;                 // 商品备注
}
```

## ShippingAllocation 数据结构

```ts
interface ShippingAllocation {
    cost: number;     // 配送费用
    title: string;    // 配送方式名称
    isAddtional: boolean; // 是否为附加运费
}
```

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

**参数：**

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| options | object | 否 | 可选参数 |
| options.extensionButton | `{ text: string, url: string }` | 否 | 扩展按钮 |
| options.insuranceAmount | `number` | 否 | 保险金额 |

**返回：** `string` - 购物车ID

## get()

获取购物车信息。

```ts
k.api.get(() => {
    return k.commerce.cart.get(cartId)
})
```

**参数：**

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| cartId | `string` | 是 | 购物车ID |

**返回：** `Cart`

## getDetail()

获取购物车详情（包含金额计算）。

```ts
k.api.get(() => {
    return k.commerce.cart.getDetail(cartId, {
        country: '中国',
        province: '广东省',
        city: '广州市'
    })
})
```

**参数：**

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| cartId | `string` | 是 | 购物车ID |
| options | object | 否 | 可选参数 |
| options.country | `string` | 否 | 国家 |
| options.province | `string` | 否 | 省份 |
| options.city | `string` | 否 | 城市 |

**返回：** `CartDetail`

## addOrUpdateLine()

添加或更新购物车商品。

```ts
k.api.post(() => {
    const product = k.commerce.product.get('test-product')
    const variantId = product.variants[0].id

    k.commerce.cart.addOrUpdateLine(cartId, variantId, 2, {
        note: 'test line',
        groupName: '捆绑组1',
        isMain: true
    })
    return 'success'
})
```

**参数：**

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| cartId | `string` | 是 | 购物车ID |
| variantId | `string` | 是 | 商品变体ID |
| quantity | `number` | 是 | 数量 |
| options | object | 否 | 可选参数 |
| options.note | `string` | 否 | 备注 |
| options.groupName | `string` | 否 | 捆绑产品组名（具有相同 groupName 的 line 将变成捆绑产品） |
| options.isMain | `boolean` | 否 | 是否作为捆绑产品中的主产品 |
| options.extensionButton | `{ text: string, url: string }` | 否 | 扩展按钮 |

**返回：** `void`

## removeLine()

删除购物车商品。

```ts
k.api.post(() => {
    const product = k.commerce.product.get('test-product')
    const variantId = product.variants[0].id

    k.commerce.cart.removeLine(cartId, variantId)
    return 'success'
})
```

**参数：**

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| cartId | `string` | 是 | 购物车ID |
| variantId | `string` | 是 | 商品变体ID |
| groupName | `string` | 否 | 捆绑产品组名 |

**返回：** `void`

## updateAdditional()

更新购物车附加信息。

```ts
k.api.post(() => {
    k.commerce.cart.updateAdditional(cartId, {
        redeemPoints: true,
        extensionButton: { text: '测试', url: '/test' },
        insuranceAmount: 100
    })
    return 'success'
})
```

**参数：**

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| cartId | `string` | 是 | 购物车ID |
| options | `object` | 是 | 更新内容 |
| options.redeemPoints | `boolean` | 否 | 是否使用积分 |
| options.extensionButton | `{ text: string, url: string }` | 否 | 扩展按钮 |
| options.insuranceAmount | `number` | 否 | 保险金额 |

**返回：** `void`

## updateContact()

更新购物车顾客联系信息。

```ts
k.api.post(() => {
    k.commerce.cart.updateContact(cartId, 'test@test.com')
    return 'success'
})
```

::: tip 注意
如果 email 在 customer 中存在，会自动绑定 customerId；否则仅填入 email
:::

**参数：**

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| cartId | `string` | 是 | 购物车ID |
| email | `string` | 是 | 联系邮箱 |

**返回：** `void`

## updateDigitalShipping()

更新数字配送方式。

```ts
k.api.post(() => {
    k.commerce.cart.updateDigitalShipping(cartId, shippingId)
    return 'success'
})
```

**参数：**

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| cartId | `string` | 是 | 购物车ID |
| shippingId | `string` | 是 | 数字配送方式ID |

**返回：** `void`

## updateDiscountCodes()

更新购物车优惠券代码。

```ts
k.api.post(() => {
    const discount = k.commerce.discount.list()[0]
    k.commerce.cart.updateDiscountCodes(cartId, [discount.code])
    return 'success'
})
```

**参数：**

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| cartId | `string` | 是 | 购物车ID |
| discountCodes | `string[]` | 是 | 优惠券代码数组 |

**返回：** `void`

## updateShipping()

更新购物车配送方式。

```ts
k.api.post(() => {
    k.commerce.cart.updateShipping(cartId, shippingId)
    return 'success'
})
```

**参数：**

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| cartId | `string` | 是 | 购物车ID |
| shippingId | `string` | 是 | 配送方式ID |

**返回：** `void`

## 结构

### Cart 属性

| 属性 | 说明 | 类型 |
|------|------|------|
| id | 购物车ID | `string` |
| customerId | 顾客ID | `string` |
| contact | 联系邮箱 | `string` |
| country | 国家 | `string` |
| discountCodes | 优惠券代码 | `string[]` |
| note | 备注 | `string` |
| lines | 购物车商品 | [`CartItem`](#cartitem-属性)[] |
| shippingId | 配送方式ID | `string` |
| digitalShippingId | 数字配送方式ID | `string` |
| insuranceAmount | 保险金额 | `number` |
| extensionButton | 扩展按钮 | `{ text: string, url: string } \| null` |
| redeemPoints | 是否使用积分 | `boolean` |
| createdAt | 创建时间 | `Date` |
| updatedAt | 更新时间 | `Date` |

### CartDetail 属性

| 属性 | 说明 | 类型 |
|------|------|------|
| id | 购物车ID | `string` |
| customerId | 顾客ID | `string` |
| contact | 联系邮箱 | `string` |
| country | 国家 | `string` |
| currency | 货币信息 | `{ code: string, symbol: string, rate: number }` |
| discountCodes | 优惠券代码 | `string[]` |
| activeDiscountCodes | 激活的优惠券代码 | `string[]` |
| note | 备注 | `string` |
| discountAllocations | 自动折扣信息 | `DiscountAllocation[]` |
| shippingAllocations | 配送方式列表 | [`ShippingAllocation`](#shippingallocation-属性)[] |
| shippingAmount | 配送金额 | `number` |
| insuranceAmount | 保险金额 | `number` |
| subtotalAmount | 小计金额 | `number` |
| totalAmount | 总金额 | `number` |
| taxAmount | 税额 | `number` |
| totalQuantity | 总数量 | `number` |
| redeemPoints | 积分 | `number` |
| pointsDeductionAmount | 积分抵扣金额 | `number` |
| earnPoints | 获取积分数额 | `number` |
| shipping | 配送方式 | `Shipping \| null` |
| lines | 购物车商品 | [`CartItem`](#cartitem-属性)[] |

### CartItem 属性

| 属性 | 说明 | 类型 |
|------|------|------|
| variantId | 商品规格ID | `string` |
| productId | 商品ID | `string` |
| title | 商品名称 | `string` |
| options | 商品选项 | `{ name: string, value: string }[]` |
| sku | 商品SKU | `string` |
| image | 商品图片 | `string` |
| originalPrice | 商品原价 | `number` |
| price | 商品价格 | `number` |
| discountAllocations | 自动折扣信息 | `DiscountAllocation[]` |
| quantity | 商品数量 | `number` |
| totalQuantity | 商品总数量 | `number` |
| amount | 商品小计金额 | `number` |
| taxAmount | 商品税额 | `number` |
| inventory | 商品库存 | `number` |
| groupName | 商品组名 | `string` |
| isMain | 是否为主商品 | `boolean` |
| note | 商品备注 | `string` |

### ShippingAllocation 属性

| 属性 | 说明 | 类型 |
|------|------|------|
| cost | 配送费用 | `number` |
| title | 配送方式名称 | `string` |
| isAdditional | 是否为附加运费 | `boolean` |

## 相关文档

- [k.commerce](./commerce.md) - 电商模块概述
- [product](./product.md) - 产品管理
- [category](./category.md) - 分类管理
