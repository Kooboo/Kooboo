# k.utils.CRM

> Kooboo CRM 客户与订单拉取

## 概述

`k.utils.CRM` 从 Kooboo CRM 接口按起始日期拉取数据（属性名 **`CRM`**，大小写敏感）。须配置有效的 `crmKey`。

## getCustomers()

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `startDate` | `Date` | 是 | 起始日期，运行时会按本地时间提交 |
| `crmKey` | `string` | 是 | CRM 密钥 |

**返回：** `CRMCustomer[]`。

```ts
const startDate = new Date("2024-01-01")
const crmKey = k.request.queryString.get("crmKey")
const list = k.utils.CRM.getCustomers(startDate, crmKey)

return {
    count: list.length,
    customers: list.slice(0, 10)
}
```

## getOrders()

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `startDate` | `Date` | 是 | 起始日期，运行时会按本地时间提交 |
| `crmKey` | `string` | 是 | CRM 密钥 |

**返回：** `CRMOrder[]`。

```ts
const startDate = new Date("2024-01-01")
const crmKey = k.request.queryString.get("crmKey")
const orders = k.utils.CRM.getOrders(startDate, crmKey)

return {
    count: orders.length,
    orders: orders.slice(0, 10)
}
```

## 相关文档

- [k.utils 概述](./index.md)
