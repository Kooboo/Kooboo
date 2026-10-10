# k.commerce

> 电商模块主模块

## 概述

`k.commerce` 提供电商相关的主模块访问。

## 子模块

| 模块 | 说明 |
|------|------|
| [概述](./commerce.md) | 模块概述 |
| [product](./product.md) | 产品管理 |
| [category](./category.md) | 产品分类 |
| [cart](./cart.md) | 购物车 |
| [order](./order.md) | 订单 |
| [customer](./customer.md) | 客户管理 |
| [discount](./discount.md) | 折扣优惠 |
| [address](./address.md) | 地址管理 |
| [loyalty](./loyalty.md) | 会员积分 |
| [membership](./membership.md) | 会员等级 |
| [shipping](./shipping.md) | 配送方式 |
| [wishlist](./wishlist.md) | 心愿单 |
| [currency](./currency.md) | 货币管理 |
| [settings](./settings.md) | 电商设置 |

## TypeScript 定义

```ts
interface KCommerce {
    product: KProduct;
    cart: KCart;
    order: KOrder;
}
```
