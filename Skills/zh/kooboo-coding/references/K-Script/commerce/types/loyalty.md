# Commerce Loyalty Types

## LoyaltyInfo

```ts
interface LoyaltyInfo {
    points: number;
    membershipStatus: MembershipStatus | null;
}
```

## MembershipStatus

```ts
interface MembershipStatus {
    membership: Membership;
    startedAt: string;
    endAt: string;
    active: boolean;
}
```

## Membership

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
```

## CustomerPointModel

```ts
interface CustomerPointModel {
    customerId: string;
    points: number;
    createdAt: string;
    type: string;
    description: string;
    key: string;
    disabled: boolean;
}
```

## Condition

```ts
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
