# Commerce Membership

::: tip 会员等级创建
会员等级需要手动到 Kooboo 后台创建：**电商 → 会员（默认在隐藏目录中）→ 创建会员**
:::

## list()

获取会员等级列表。

```ts
k.api.get(() => {
    const memberships = k.commerce.membership.list()
    return { count: memberships.length, items: memberships }
})
```
**返回：** `Membership[]`

## get()

根据 ID 获取会员等级详情。

```ts
k.api.get(() => {
    const memberships = k.commerce.membership.list()
    if (memberships && memberships.length > 0) {
        const membership = k.commerce.membership.get(memberships[0].id)
        return membership || { error: 'membership not found' }
    }
    return { error: 'no membership' }
})
```
**参数：** `id: string` | **返回：** `Membership | null`

## Membership 类型定义

```ts
interface Membership {
    id: string;
    name: string;
    description: string;
    price: number;
    duration: number;
    durationUnit: "Year" | "Month" | "Day" | "Week";
    priority: number;
    condition: Condition;
    allowAutoUpgrade: boolean;
    allowPurchase: boolean;
    customData: { key: string; value: string }[] | null;
    createdAt: string;
    updatedAt: string;
}

interface Condition {
    isAny: boolean;
    items: ConditionItem[];
}

interface ConditionItem {
    option: string;
    method: string;
    value: string;
}
```

## 边界说明

- 积分管理 → [Loyalty Guide](loyalty.md)
- 变更会员等级 → [Loyalty Guide](loyalty.md) 的 `changeMembership()`
