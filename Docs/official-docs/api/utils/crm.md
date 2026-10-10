# k.utils.CRM

> Kooboo CRM customer and order pulling

## Overview

`k.utils.CRM` pulls data from the Kooboo CRM interface by start date (attribute name **`CRM`**, case sensitive). A valid `crmKey` must be configured.

## getCustomers()

| Parameter | Type | Required | Description |
|------|------|------|------|
| `startDate` | `Date` | Yes | Start date; submitted as local time |
| `crmKey` | `string` | Yes | CRM key |

**Returns:** `CRMCustomer[]`.

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

| Parameter | Type | Required | Description |
|------|------|------|------|
| `startDate` | `Date` | Yes | Start date; submitted as local time |
| `crmKey` | `string` | Yes | CRM key |

**Returns:** `CRMOrder[]`.

```ts
const startDate = new Date("2024-01-01")
const crmKey = k.request.queryString.get("crmKey")
const orders = k.utils.CRM.getOrders(startDate, crmKey)

return {
    count: orders.length,
    orders: orders.slice(0, 10)
}
```

## Related Docs

- [k.utils Overview](./index.md)
