# Commerce Membership

::: tip Creating membership tiers
Membership tiers must be created manually in the Kooboo admin: **Commerce → Membership (hidden by default) → Create membership**
:::

## list()

Get the membership-tier list.

```ts
k.api.get(() => {
    const memberships = k.commerce.membership.list()
    return { count: memberships.length, items: memberships }
})
```
**Returns:** `Membership[]`

## get()

Get membership-tier details by ID.

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
**Parameter:** `id: string` | **Returns:** `Membership | null`

## Membership type definition

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

## Boundaries

- Point management → [Loyalty Guide](loyalty.md)
- Change membership tier → `changeMembership()` in [Loyalty Guide](loyalty.md)
