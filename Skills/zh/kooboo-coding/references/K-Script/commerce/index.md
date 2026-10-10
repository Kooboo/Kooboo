# Commerce Guide 电商模块

::: warning 登录前置
只要当前需求属于站点侧 commerce 顾客流程，并且会串起 `product`、`category`、`cart`、`order` 中的任意环节与登录/下单，就必须先读 [Customer Guide](customer.md) 和 [Commerce 顾客登录与下单流程](../../Scenario/commerce-customer-login-flow.md)，并默认使用 `k.commerce.customer.login(...)`。
:::

## 模块导航

根据用户目标，一次选择一个或多个叶子 skill：

| 用户目标 | 使用的 Skill |
|----------|--------------|
| 产品检索、列表、搜索、详情 | [Product Guide](product.md) |
| 变体创建、变体字段更新、数字产品 | [Variant Guide](product.md) |
| 分类检索、创建、关联产品 | [Category Guide](category.md) |
| 购物车管理、优惠券、配送 | [Cart Guide](cart.md) |
| 订单创建、状态更新、支付、取消 | [Order Guide](order.md) |
| 顾客创建、查询、登录、OAuth | [Customer Guide](customer.md) |
| 顾客地址簿、Checkout 收货地址、后台地址互通 | [顾客地址与收货地址契约](../../Scenario/commerce-customer-address-flow.md) + [Address Guide](address.md) + [Customer Types](types/customer.md) |
| 会员积分查询、赚取、消费、等级变更 | [Loyalty Guide](loyalty.md) |
| 折扣优惠券查询 | [Discount Guide](discount.md) |
| 愿望清单查询、添加、移除 | [Wishlist Guide](wishlist.md) |
| 会员等级查询 | [Membership Guide](membership.md) |
| 配送方式查询 | [Shipping Guide](shipping.md) |
| 国家、省份、城市行政区划查询 | [Address Guide](address.md) |
| 货币查询、汇率转换 | [Currency Guide](currency.md) |
| 电商全局设置（货币、支付、积分、Webhook） | [Settings Guide](settings.md) |
| 第三方支付接入（微信 / 支付宝 / PayPal / Stripe / Square） | [Payment Guide](../payment/index.md) |

## 类型定义

当需要确认类型字段时，按需加载：

- [Product Types](types/product.md) — Product / Variant / Digital 结构
- [Cart Types](types/cart.md) — Cart / CartLine / CartDetailLine 结构
- [Order Types](types/order.md) — Order / OrderLine / Address 结构
- [Customer Types](types/customer.md) — Customer / AddressModel 结构
- [Loyalty Types](types/loyalty.md) — LoyaltyInfo / Membership 结构

## 使用流程

1. 如果需求涉及站点侧商品浏览、加购、结账或订单，先加载 [Customer Guide](customer.md) 和 [Commerce 顾客登录与下单流程](../../Scenario/commerce-customer-login-flow.md)
2. 加载对应的 behavior guide
3. 撰写实现 plan
4. 涉及顾客地址或订单收货地址时，必须加载 [顾客地址与收货地址契约](../../Scenario/commerce-customer-address-flow.md) 和 [Customer Types](types/customer.md)，不能从现有页面字段反推模型
5. 其它字段不确定时，加载对应类型定义
6. 将 plan 提交用户确认
7. 用户确认后再开始写代码

## 边界说明

- 本入口只做路由，不展开任何具体 API 细节
- 每个子模块的行为 guide 相互独立，不要跨模块展开
- 如果需求同时跨 `product`、`category`、`cart`、`order`，并且最终会形成站点侧顾客业务链路，默认先走 `k.commerce.customer.login(...)`，完整流程参考 [Commerce 顾客登录与下单流程](../../Scenario/commerce-customer-login-flow.md)
