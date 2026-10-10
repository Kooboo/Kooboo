# Carts

> Menu: **Commerce -> Carts**  
> Deep link: `/_Admin/commerce/carts?SiteId={siteGuid}`  
> **Create**: `/_Admin/commerce/cart/create?SiteId=...`  
> **Edit**: `/_Admin/commerce/cart/edit?SiteId=...&id={cartId}`  
> **Checkout**: `/_Admin/commerce/cart/checkout?SiteId=...&id={cartId}`

View and manage site **cart** instances: storefront add-to-cart, marketing automation, and other flows all create records; the admin can also **create carts manually**, edit line items, and **check out** to generate [orders](./orders.md). Line amounts, shipping, tax, and discounts are trial-calculated server-side via `calculate` (same as storefront checkout), and depend on [shipping](./shippings.md), [customers](./customers.md), [discounts](./discounts.md), and related configuration.

## How to Open

1. In the [Edit menu](../navigation.md#edit-menu), enable **Commerce -> Carts** (and the parent **Commerce** group).
2. Open **Commerce -> Carts** in the left sidebar.

## List Page

Path: `/_Admin/commerce/carts?SiteId=...`

| Column / Action | Description |
|-----------------|-------------|
| **Products** | Inline thumbnail strip; hover shows title and variant text; badge `x {qty}` is the line total quantity |
| **Contact** | When a customer is bound: email, phone, name; otherwise shows `contact` text |
| **Country** | Cart country (relevant to trial calculation / shipping) |
| **Last modified** | `updateAt` |
| **Condition** (narrow column) | When type is **Automated**, shows automation condition summary (`option` / `method` / `value`) |
| **Create** | Opens [Create cart](#create-cart) |
| **Edit** | Row pencil → [Edit cart](#edit-cart) |
| **Checkout** | Row checkout icon; shown only when the cart **has at least one product line** → [Checkout](#checkout) |
| **Pagination** | 30 rows per page by default |
| **Batch delete** | Select rows → **Delete** → confirm |

<DocImage src="/cms/commerce/carts-list.png" alt="Carts list: product thumbnails, contact, country, edit and checkout" width="1120" />

## Create Cart

Path: `/_Admin/commerce/cart/create?SiteId=...`

Breadcrumb at top: **Carts -> Create**. When the site has multiple [currencies](./currencies.md), you can choose a **trial currency** in the top-right (default currency plus enabled additional currencies from settings).

### Product Lines and Amount Trial Calculation

See [Product line editor](#product-line-editor-productseditor).

<DocImage src="/cms/commerce/carts-create.png" alt="Create cart: product table, discount codes, subtotal shipping total and contact delivery" width="1120" />

### Contact and Delivery

| Field | Description |
|-------|-------------|
| **Contact** | `ContactCard`: select an existing [customer](./customers.md) or clear; selected customer participates in points deduction trial calculation |
| **Express shipping** | Shown when the cart contains **physical** products; choose a [physical shipping](./shippings.md) method |
| **Digital delivery** | Shown when the cart contains **digital** products; choose [digital shipping](./shippings.md) |
| **Note** | Cart note (`note`); read-only on checkout page |

Bottom **Save** creates the cart and returns to the list; **Cancel** discards.

### Add Product Dialog

Click **Add product** to open the variant picker, search and select a product **variant**, then add it to the table; adding the same variant again **accumulates quantity**.

<DocImage src="/cms/commerce/carts-select-variant-dialog.png" alt="Add product: search and select product variant" width="960" />

## Edit Cart

Path: `/_Admin/commerce/cart/edit?SiteId=...&id=...`

Same layout as create: product area can change line items, [discount codes](#discount-codes), and points deduction; **Contact** is read-only (bound customers cannot be changed on the edit page—create a new cart or handle via storefront flow). **Express shipping / digital delivery / note** can still be edited.

<DocImage src="/cms/commerce/carts-edit.png" alt="Edit cart: product table and read-only contact card" width="1120" />

## Checkout

Path: `/_Admin/commerce/cart/checkout?SiteId=...&id=...`

Complete checkout on behalf of a customer: product table is **read-only**; contact and delivery methods are **read-only**; cart **note** is shown (or `-` if empty).

| Section | Description |
|---------|-------------|
| **Product lines** | Read-only; table includes an extra **Tax** column; footer totals include a **Tax** row |
| **Shipping address** | Shown only for physical products; **select** one address from the customer's address book (affects country/region trial calculation) |
| **Order note** | Written to the generated order's `note`; different from the cart note |

Bottom primary button label is **Checkout**; on confirm, `createOrder` generates an order and returns to the list.

<DocImage src="/cms/commerce/carts-checkout.png" alt="Cart checkout: read-only products, shipping address selection and order note" width="1120" />

## Product Line Editor (ProductsEditor)

Shared by create, edit, and checkout pages (checkout uses `readonly` + `mode=checkout`).

| Capability | Description |
|------------|-------------|
| **Add product** | Non-read-only only; opens [Add product dialog](#add-product-dialog) |
| **Table columns** | Featured image, title (with variants), stock, unit price, quantity, line total qty, amount, discount tags; checkout mode also includes **Tax** |
| **Quantity** | Editable; changing triggers `calculate` |
| **Delete line** | Delete icon; for **bundle** products, if the row is main (`isMain` + `groupName`), deleting removes all child lines in the group |
| **Child lines** | Indented when sold as a bundle |
| **Discount codes** | Add/edit/remove below the product table; options come from [discounts](./discounts.md) rules with **method = discount code**. Create and enable the code under discounts first for trial calculation to apply to lines and totals |
| **Points deduction** | Shown when a customer is selected and trial `canPointsDeductionAmount > 0` (create/edit); deduction amount and available points shown in copy |
| **Estimated points earned** | When trial `earnPoints > 0`, shows points this order will earn (see [loyalty](./loyalty.md)) |
| **Totals area** | Order-level discounts, **Subtotal**, (checkout) **Tax**, insurance (if any), points deduction, **Shipping** (physical lines or shipping > 0; itemized when multiple delivery methods apply), **Total** |
| **Extension button** | If the server returns `extensionButton`, opens an in-site link (new tab) |

For shipping and default delivery rules, see [Shipping](./shippings.md) — "Default shipping for new sites".

### Discount Codes

Add discount codes via tag input **below the product line table** (UI copy often says "Discount code" / `discountCodes`). `calculate` applies only when the code is configured as **discount code** under [discounts](./discounts.md), within validity, and meeting **conditions**; line **Discount** column and footer **Discount**, **Subtotal**, **Total** update accordingly. Read-only on checkout.

## Related

- [Orders](./orders.md) — checkout output  
- [Customers](./customers.md) — contact and shipping addresses  
- [Product management](./product-management.md) — variants and inventory  
- [Shipping](./shippings.md), [Discounts](./discounts.md), [Loyalty](./loyalty.md)  
- [k.commerce.cart](../../api/commerce/cart.md)
