# membership

> 会员等级

## 概述

`k.commerce.membership` 提供会员等级相关操作，用于获取会员等级信息。

::: tip 会员等级创建
会员等级需要手动到 Kooboo 后台创建：**电商 → 会员（默认在隐藏目录中）→ 创建会员**
:::

## list()

获取会员等级列表。

**参数**：无。

```ts
k.api.get(() => {
    const memberships = k.commerce.membership.list()
    return { count: memberships.length, items: memberships }
})
```

**返回：** `Membership[]`

## get()

根据会员 ID 获取会员等级信息。

```ts
k.api.get(() => {
    const memberships = k.commerce.membership.list()
    if (memberships && memberships.length > 0) {
        return k.commerce.membership.get(memberships[0].id)
    }
    return { error: 'no membership' }
})
```

**参数：**

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| id | `string` | 是 | 会员ID |

**返回：** `Membership`

## 结构

### Membership 属性

| 属性 | 说明 | 类型 |
|------|------|------|
| id | 会员ID | `string` |
| name | 会员名称 | `string` |
| description | 会员描述 | `string` |
| price | 会员价格 | `number` |
| duration | 会员时长 | `number` |
| durationUnit | 会员时长单位 | `"Year"` \| `"Month"` \| `"Day"` \| `"Week"` |
| priority | 会员优先级 | `number` |
| condition | 升级条件 | [`Condition[]`](#condition-属性) |
| allowAutoUpgrade | 是否允许自动升级 | `boolean` |
| allowPurchase | 是否允许购买 | `boolean` |
| customData | 自定义数据 | `{key: value}[] \| null` |
| createdAt | 创建时间 | `string` |
| updatedAt | 更新时间 | `string` |

### Condition 属性

| 属性 | 说明 | 类型 |
|------|------|------|
| isAny | 任一条件满足即可 | `boolean` |
| items | 条件项列表 | [`ConditionItem[]`](#conditionitem-属性) |

### ConditionItem 属性

| 属性 | 说明 | 类型 |
|------|------|------|
| option | 条件选项 | `string` |
| method | 条件方法 | `string` |
| value | 条件值 | `string` |

## 相关文档

- [k.commerce](./commerce.md) - 电商模块概述
- [loyalty](./loyalty.md) - 会员积分
- [customer](./customer.md) - 客户管理
