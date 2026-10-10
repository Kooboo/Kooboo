# Product Management (List)

> Menu: **Commerce -> Product Management**  
> Deep link: `/_Admin/commerce/product-management?SiteId={siteGuid}`

Maintain site sellable products **list, filters, and entry points**; per-product form and variants in [Product detail (create and edit)](./product-management-detail.md).

::: tip Prerequisite
Requires [product types](./product-types.md); categories optional, see [product categories](./product-categories.md). Global toggles and custom fields in [settings](./settings.md).
:::

## How to Open

1. In the [Edit menu](../navigation.md#edit-menu), enable **Commerce -> Product Management** (and the parent **Commerce** group).
2. Open **Commerce -> Product Management** in the left sidebar.

## List Page

Path: `/_Admin/commerce/product-management?SiteId=...`

| Area | Description |
|------|-------------|
| Statistics | Out of stock count, total products, total inventory, etc. |
| Filters | Keyword, active status, stock status, etc.; click **Search** to refresh |
| Table columns | From [product types](./product-types.md) and **product custom fields** in [settings](./settings.md); common: title, featured image, variant count, stock, sales, tags, categories |
| **Create product** | Dropdown: blank product, or select **product type** then [create page](./product-management-detail.md#how-to-enter) |
| **Edit** | Row pencil → [edit page](./product-management-detail.md#how-to-enter) |
| **Duplicate** | After selecting **one** row, toolbar **Duplicate** appears; confirm to create copy |
| **Batch delete** | Select rows → toolbar **Delete**; confirm to remove selected products |

<DocImage src="/cms/commerce/product-management-list.png" alt="Product management list: stats, filters, table and create product dropdown" width="1120" />

::: tip Variants
When [settings](./settings.md) has not enabled "Hide variants", list shows variant count; variant details maintained on [product detail](./product-management-detail.md) page.
:::

## Digital Products

Digital products (ebooks, software licenses, downloadable assets, etc.) require a **digital product** [product type](./product-types.md#digital-products) first, then create or edit specific products on this list.

| Step | Where |
|------|-------|
| 1. Define type | [Product types](./product-types.md) — check **Digital product**, optional download count/day limits |
| 2. Create product | This page **Create product** → select that type (or edit existing digital product) |
| 3. Configure delivery | [Product detail](./product-management-detail.md#digital-products) — in variant table **Digital products** column, add file / link / text for **each SKU** |

List summary or table often shows **Digital product** tag (`isDigital`) to distinguish from physical products. Delivery items are not maintained on list; click **Edit** for detail page.

If [settings](./settings.md) enables **Digital product delivery required**, products without delivery on any variant **cannot be saved**.

## Related

- [Product detail (create and edit)](./product-management-detail.md) — [digital product configuration](./product-management-detail.md#digital-products)  
- [Product types](./product-types.md) — [digital product type](./product-types.md#digital-products)  
- [Product categories](./product-categories.md)  
- [Commerce overview](./index.md)
