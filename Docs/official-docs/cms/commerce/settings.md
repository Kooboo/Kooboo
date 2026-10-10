# Settings

> Menu: **Commerce -> Settings**  
> Deep link: `/_Admin/commerce/settings?SiteId={siteGuid}`

Site-level **commerce global configuration** (Commerce Store `settings`): units of measure, checkout payment methods, product/category/order field models, and variant/digital product display rules. After save, affects [product types](./product-types.md), [product management](./product-management.md), [product categories](./product-categories.md), [orders](./orders.md), etc.; scripts read via `k.commerce.settings`, see [k.commerce settings](../../api/commerce/settings.md).

::: tip vs "Currencies" menu
Storefront default **currency code / symbol** is usually maintained under **Commerce -> Currencies**; this page **Basic** section currently has **weight unit** and **payment methods** (if currency dropdown appears later, follow actual admin UI).
:::

## How to Open

1. In the [Edit menu](../navigation.md#edit-menu), enable **Commerce -> Settings** (and the parent **Commerce** group).
2. Open **Commerce -> Settings** in the left sidebar.

## Page Structure

Single-page form, three white cards top to bottom + bottom fixed **Save** (`KBottomBar`, no **Cancel**). Keyboard shortcut save supported (same as other site edit pages).

<DocImage src="/cms/commerce/commerce-settings-overview.png" alt="Commerce settings: basic, custom fields, display toggles" width="1120" />

| Section | Content |
|---------|---------|
| **Basic** | Weight unit, enabled payment methods |
| **Custom fields** | Product fields, category fields, order extension fields |
| **Display toggles** | Hide attributes / hide variants / digital product delivery required (conditional) |

## Basic

| Item | Description |
|------|-------------|
| **Weight unit** | `kg` or `g`; used for variant weight and [shipping](./shippings.md) freight calculation |
| **Enabled payment methods** | Multi-select dropdown; options from installed payment modules (configure payment in [service integrations](../settings/integrations/index.md) `/_Admin/system/config`, see [k.payment](../../api/payment/index.md)). Saved as `payments` array; checkout shows enabled only. Tooltip: configures `k.commerce.settings.payments` |

## Custom Fields

Three **SortableList** lists, structure similar to [Content -> Content Types](../content/content-types.md) field table, but data writes to commerce `settings` not content types.

<DocImage src="/cms/commerce/commerce-settings-custom-fields.png" alt="Commerce settings: product, category, order extension field lists" width="1120" />

### Product Fields

Controls [product detail](./product-management-detail.md) form and [product management](./product-management.md) list **summary columns**.

| Action | Description |
|--------|-------------|
| **Add** | Opens **custom field** dialog for new field |
| **Edit** | Change type, display name, validation, summary column, etc. |
| **Delete** | Non-**system fields** deletable; system fields (Title, Images, etc.) cannot be deleted |
| **Sort** | Drag to reorder fields |

Tooltip: **Summary fields appear on the list page**.

### Category Fields

Controls [product categories](./product-categories.md) edit page and list dynamic columns; same rules as product fields.

### Order Extension Fields

Adds extension data definitions for [orders](./orders.md); field types geared to orders (e.g. `filterable`, `exportable`, column width, etc.); dialog differs slightly from product/category custom field dialog.

### Custom Field Dialog (product / category)

Click **Add** or row **Edit** on product or category field list opens `CustomFieldDialog`.

<DocImage src="/cms/commerce/commerce-settings-custom-field-dialog.png" alt="Custom field dialog: basic, advanced, validation tabs" width="960" />

Common configuration (same control types as content field editor):

| Tab / Item | Description |
|------------|-------------|
| **Basic** | Name, display name, control type (text, rich text, image, select, etc.), multivalue, **summary field**, multilingual, etc. |
| **Advanced** | Dynamic options, content folder relation, etc. (per control type) |
| **Validation** | Required, length rules, etc. |
| System fields **Images / Image** | Allow editing image **meta** template (product featured/category image metadata) |

After saving dialog, field enters corresponding list; must click page bottom **Save** to write to site.

## Display Toggles

<DocImage src="/cms/commerce/commerce-settings-display.png" alt="Commerce settings: hide attributes, hide variants, digital delivery required" width="1120" />

| Toggle | Description |
|--------|-------------|
| **Hide attributes** | When on, [product types](./product-types.md) create dialog and list **Attributes** column hidden; [product detail](./product-management-detail.md) basic info hides **Attributes** key-value pairs |
| **Hide variants** | When on, products treated as single variant; no **variant options / variants** on type and detail; list hides variant count |
| **Digital product content cannot be empty** | Shown only when at least one [product type](./product-types.md) has **Digital product** checked; when on, saving digital product requires each variant to configure [digital product](./product-management-detail.md#digital-products) delivery |

## Related

- [Commerce overview](./index.md) — recommended setup order  
- [Product types](./product-types.md) · [Product management](./product-management.md) · [Product categories](./product-categories.md)  
- [Currencies](./currencies.md) · [Shipping](./shippings.md) · [Discounts](./discounts.md)  
- [k.commerce settings](../../api/commerce/settings.md)
