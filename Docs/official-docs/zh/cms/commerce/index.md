# 电商

> 菜单：左侧 **电商** 分组（须在 **编辑菜单** 中勾选显示）  
> 路由前缀：`/_Admin/commerce/...?SiteId={站点GUID}`

**电商**模块用于在 Kooboo 站点后台管理商品、订单、购物车、配送与优惠等。前台下单与商品数据通过 [k.commerce](../../api/commerce/commerce.md) 读写；本系列说明**后台菜单与页面操作**。

::: tip 与「内容」模块的区别
| | **内容** | **电商** |
|--|----------|----------|
| 典型场景 | 文章、Banner、非交易结构化内容 | 商品 SKU、订单、库存、结算 |
| 后台 | 内容 → 数据类型 / 内容夹 | 电商 → 商品类型 / 商品管理 |
| API | `k.content` | `k.commerce` |
:::

## 如何打开

1. 在 [编辑菜单](../navigation.md#权限与编辑菜单) 中勾选 **电商** 及其子项（部分子菜单项本身也标有 `advanced`）。
2. 左侧展开 **电商**。

## 子菜单（与后台顺序一致）

| 菜单 | 路径 | 文档 |
|------|------|------|
| 商品管理 | `/commerce/product-management` | [列表](./product-management.md) · [详情](./product-management-detail.md) |
| 商品类型 | `/commerce/product-types` | [商品类型](./product-types.md) |
| 商品分类 | `/commerce/product-categories` | [商品分类](./product-categories.md) |
| 购物车 | `/commerce/carts` | [购物车](./carts.md) |
| 顾客管理 | `/commerce/customers` | [顾客管理](./customers.md) |
| 订单 | `/commerce/orders` | [订单](./orders.md) |
| 销售统计 | `/commerce/sale-stats` | [销售统计](./sale-stats.md) |
| 优惠折扣 | `/commerce/discounts` | [优惠折扣](./discounts.md) |
| 货币 | `/commerce/currencies` | [货币](./currencies.md) |
| 配送 | `/commerce/shippings` | [配送](./shippings.md) |
| 会员 | `/commerce/loyalty` | [会员](./loyalty.md) |
| 税 | `/commerce/taxes` | [税](./taxes.md) |
| 通知 | `/commerce/notification` | [通知](./notification.md) |
| 设置 | `/commerce/settings` | [设置](./settings.md) |

## 建议实施顺序

菜单最后一项是 **设置**，但首次开通电商时建议**先读完设置并保存**，再配置类型、分类与商品（与菜单排序不同，仅为实施顺序）：

```mermaid
flowchart LR
  S[设置] --> T[商品类型]
  T --> C[商品分类]
  C --> P[商品管理]
  P --> O[订单 / 顾客 / 购物车]
```

1. [设置](./settings.md) — 重量单位、支付方式、商品/分类自定义字段等。  
2. [商品类型](./product-types.md) — 定义商品字段与是否数字商品。  
3. [商品分类](./product-categories.md) — 分类树与展示字段。  
4. [商品管理](./product-management.md) — 列表上架；规格等在 [商品详情](./product-management-detail.md) 维护。  
5. 运营期再配置 [优惠折扣](./discounts.md)、[配送](./shippings.md)、[货币](./currencies.md)、[税](./taxes.md) 等。

## 相关

| 文档 | 说明 |
|------|------|
| [k.commerce](../../api/commerce/commerce.md) | 商品、购物车、订单等 API |
| [k.payment](../../api/payment/index.md) | 支付渠道（与设置中的支付方式配合） |
| [站点后台菜单总览](../navigation.md) | 完整菜单树 |
