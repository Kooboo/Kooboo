# Data Layer Guide 数据层自检指引

## 自检问题

在进行数据操作前，先问自己两个问题：

1. **这个数据属于哪个业务域？**
2. **需要做什么操作（增/删/改/查）？**

## 数据源选择

| 数据域 | 数据源 | 说明 |
|--------|--------|------|
| 电商相关（产品、订单、购物车、会员） | `k.commerce.*` | 使用 commerce 模块 API |
| CMS 内容管理 | `k.content.*` | 使用内容管理 API |
| 关系型数据存储 | `k.DB.sqlite.*` | 使用 SQLite API（见 [SQLite Guide](../K-Script/DB/sqlite.md)） |

## Commerce 模块数据源

Commerce 模块是 Kooboo 的电商核心，数据通过 `k.commerce` 访问：

| 对象 | 数据源 | 说明 |
|------|--------|------|
| 产品 | `k.commerce.product.*` | 产品 CRUD、搜索、变体管理 |
| 分类 | `k.commerce.category.*` | 分类 CRUD、产品关联 |
| 购物车 | `k.commerce.cart.*` | 购物车管理、优惠券 |
| 订单 | `k.commerce.order.*` | 订单创建、状态管理、支付 |
| 顾客 | `k.commerce.customer.*` | 顾客注册、登录、OAuth |
| 会员积分 | `k.commerce.loyalty.*` | 积分查询、赚取、消费 |
| 会员等级 | `k.commerce.membership.*` | 会员等级查询 |
| 折扣 | `k.commerce.discount.*` | 优惠券查询 |
| 愿望清单 | `k.commerce.wishlist.*` | 愿望清单管理 |
| 配送 | `k.commerce.shipping.*` | 配送方式查询 |
| 地址 | `k.commerce.address.*` | 行政区划查询 |
| 货币 | `k.commerce.currency.*` | 货币查询、汇率转换 |
| 电商设置 | `k.commerce.settings.*` | 全局配置查询 |

**Commerce 模块详情** → [Commerce 模块](../K-Script/commerce/index.md)

**SQLite 模块详情** → [DB / SQLite](../K-Script/DB/index.md)

## 数据层自检流程

1. 判断数据属于哪个域
2. 对应域的 skill 是否已加载？（如不确定，先 [Commerce 模块](../K-Script/commerce/index.md)）
3. 确定使用哪个数据源的哪个方法
4. 在 Service 层实现数据操作，不在 API 层直接操作数据

## SSR 页面如何消费数据

| 场景 | 推荐方式 |
|------|----------|
| 首屏列表/详情/分页（commerce、content、sqlite） | 前端 **`<k-data>` + `<query>`**（见 [k-data 基础](../Frontend/k-data/basic.md)） |
| 须 import CodeBlock、或无 k-data source 的 `k.*` API | `<script type="module" env="server">` |
| 客户端交互、表单提交、实时刷新 | 轨道 B：API + 前端 fetch |

**不要**在动态化 SSR 中用 `<script env="server">` 直接 `k.commerce.*` / `k.content.*` / `k.DB.*` 查询替代 k-data。

## Service 层中的数据操作

数据操作应在 Service/Utils 中完成：

```ts
// Services.product (CodeBlock)

export function listProducts(query) {
    const products = k.commerce.product.list(query)
    return products
}

export function getProduct(id) {
    try {
        return k.commerce.product.get(id)
    } catch (e) {
        return null
    }
}
```

然后在 API 层 import 调用：

```ts
import { listProducts } from "./Services.product"

k.api.get("list", (page) => {
    return listProducts({ page })
})
```
