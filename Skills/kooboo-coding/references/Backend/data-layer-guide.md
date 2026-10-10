# Data Layer Guide

## Self-check questions

Before performing data operations, ask two questions:

1. **Which business domain does this data belong to?**
2. **What operation is needed (create/delete/update/read)?**

## Data-source selection

| Data domain | Data source | Description |
|--------|--------|------|
| Commerce-related (products, orders, cart, membership) | `k.commerce.*` | Use the commerce module API |
| CMS content management | `k.content.*` | Use the content-management API |
| Relational data storage | `k.DB.sqlite.*` | Use the SQLite API (see [SQLite Guide](../K-Script/DB/sqlite.md)) |

## Commerce module data sources

The Commerce module is Kooboo's commerce core; access its data through `k.commerce`:

| Object | Data source | Description |
|------|--------|------|
| Product | `k.commerce.product.*` | Product CRUD, search, and variant management |
| Category | `k.commerce.category.*` | Category CRUD and product association |
| Cart | `k.commerce.cart.*` | Cart management and coupons |
| Order | `k.commerce.order.*` | Order creation, status management, and payment |
| Customer | `k.commerce.customer.*` | Customer registration, login, and OAuth |
| Loyalty points | `k.commerce.loyalty.*` | Point lookup, earning, and spending |
| Membership tier | `k.commerce.membership.*` | Membership-tier lookup |
| Discount | `k.commerce.discount.*` | Coupon lookup |
| Wishlist | `k.commerce.wishlist.*` | Wishlist management |
| Shipping | `k.commerce.shipping.*` | Shipping-method lookup |
| Address | `k.commerce.address.*` | Administrative-division lookup |
| Currency | `k.commerce.currency.*` | Currency lookup and exchange-rate conversion |
| Commerce settings | `k.commerce.settings.*` | Global configuration lookup |

**Commerce module details** → [Commerce module](../K-Script/commerce/index.md)

**SQLite module details** → [DB / SQLite](../K-Script/DB/index.md)

## Data-layer self-check flow

1. Determine the data domain.
2. Is the skill for the corresponding domain loaded? (If uncertain, start with the [Commerce module](../K-Script/commerce/index.md).)
3. Determine which method from which data source to use.
4. Implement data operations in the Service layer; do not operate on data directly in the API layer.

## How SSR pages consume data

| Scenario | Recommended approach |
|------|----------|
| First-screen lists/details/pagination (commerce, content, sqlite) | Frontend **`<k-data>` + `<query>`** (see [k-data basics](../Frontend/k-data/basic.md)) |
| A CodeBlock must be imported, or a `k.*` API has no k-data source | `<script type="module" env="server">` |
| Client interaction, form submission, and live refresh | Track B: API + frontend fetch |

**Do not** replace k-data with direct `k.commerce.*` / `k.content.*` / `k.DB.*` queries through `<script env="server">` in dynamic SSR.

## Data operations in the Service layer

Data operations should be completed in Service/Utils:

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

Then import and call it in the API layer:

```ts
import { listProducts } from "./Services.product"

k.api.get("list", (page) => {
    return listProducts({ page })
})
```
