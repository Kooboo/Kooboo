# Commerce Built-in Function Guide

> Guide AI in using **Kooboo built-in functions** (`BuiltIn/Commerce`) to create, query, update, and delete products and product categories.
> For reading commerce objects such as `k.product` and `k.category` with runtime KScript, also use [index.md](index.md) in this directory.

## When to read this guide

- You need to maintain store product or category data directly through function calls rather than only display it read-only.
- You need to call functions such as `create_product`, `update_product_category`, or `get_product_detail` but are unsure about their parameter contracts or call order.
- You need to distinguish operations that write data (approval required) from query-only operations (no approval required).

## Division of responsibility with `index.md` (KScript Commerce)

| Scenario | Use |
| --- | --- |
| CRUD for products/categories through AI tool functions | This guide + the corresponding built-in function |
| Read and render commerce data in page/API/SSR KScript | [index.md](index.md) |

## Recommended call order

1. `list_product_category` / `list_product` — inspect existing data first to avoid duplicate names or accidental deletion.
2. Creation: `create_product_category` (optional) -> `create_product`.
3. Updates: first confirm the target with `get_product_detail` / `get_product_category_detail`, then run `update_*`.
4. Deletion: confirm once with a detail function before deletion, then call `delete_product` or `delete_product_category`.
5. After deletion, run `list_*` again to verify the result.

When dynamicizing the storefront, Product / Category / Variant belong to Commerce by default. Do not create same-name ContentType/ContentFolder objects merely for “admin maintainability”; see `../../Scenario/site-building/commerce-dynamicization.md` for the complete boundary.

## Approval

The following functions have `ApprovalRequired == true` and generally require user confirmation before execution:

- `create_product`
- `update_product`
- `delete_product`
- `create_product_category`
- `update_product_category`
- `delete_product_category`

The following functions have `ApprovalRequired == false`:

- `list_product`
- `get_product_detail`
- `list_product_category`
- `get_product_category_detail`

## Product functions

## `create_product`

Create a product and automatically create one default variant.

| Parameter | Type | Required | Description |
| --- | --- | --- | --- |
| Title | string | Yes | Product title |
| SeoName | string | Yes | SEO-friendly name (URL identifier) |
| Description | string | No | Product description |
| FeaturedImage | string | No | Featured-image URL; required when AI creates storefront data |
| Price | double | Yes | Default variant price |
| Inventory | int | Yes | Default variant inventory |
| CategoryIds | string[] | Yes | Existing manual Commerce category IDs, obtained from a query or `create_product_category`; pass `[]` when an explicitly uncategorized product is required |
| Tags | string[] | No | Tag array |

Returns a structured save receipt containing `message`, product/default-variant identifiers, `categoryIds`, and key fields. After writing, still read back with `get_product_detail` and confirm `categories`.

## `update_product`

Update basic information by product ID; omitted fields remain unchanged.

| Parameter | Type | Required | Description |
| --- | --- | --- | --- |
| Id | string | Yes | Product ID |
| Title | string | No | Product title |
| SeoName | string | No | SEO name |
| Description | string | No | Product description |
| FeaturedImage | string | No | Featured-image URL |
| Tags | string[] | No | Tag array |
| Active | bool | No | Whether enabled |
| CategoryIds | string[] | No | Replacement set of manual Commerce category IDs; omit to keep current relations, pass `[]` to remove all relations |

Returns the string `"Update success"`.

## `delete_product`

Delete a product by product ID; the implementation also removes its related variants and category associations.

| Parameter | Type | Required | Description |
| --- | --- | --- | --- |
| Id | string | Yes | Product ID |

Returns the string `"Success deleted"`.

## `list_product`

List basic information for all products (a flat list).

| Parameter | Type | Required | Description |
| --- | --- | --- | --- |
| (none) | - | - | No input parameters |

Each list item contains `Id`, `SeoName`, and `Title`.

## `get_product_detail`

Get product details by `Id` or `SeoName`, including extensions such as variants and categories.

| Parameter | Type | Required | Description |
| --- | --- | --- | --- |
| SeoNameOrId | string | Yes | Product ID or SeoName |

Returns a product-detail object and throws when not found.

## Category functions

## `create_product_category`

Create a product category.

| Parameter | Type | Required | Description |
| --- | --- | --- | --- |
| Title | string | Yes | Category title |
| SeoName | string | Yes | SEO-friendly name |
| Description | string | No | Category description |
| Image | string | No | Category image URL; required when AI creates storefront data |
| Tags | string[] | No | Tag array |
| ParentId | string | No | Parent category ID; empty means a root category |

Returns a structured save receipt containing `message`, category identifiers, and key fields. After writing, still read back with `get_product_category_detail` to confirm.

## `update_product_category`

Update category information by category ID; omitted fields remain unchanged.

| Parameter | Type | Required | Description |
| --- | --- | --- | --- |
| Id | string | Yes | Category ID |
| Title | string | No | Category title |
| SeoName | string | No | SEO name |
| Description | string | No | Category description |
| Image | string | No | Category image URL |
| Tags | string[] | No | Tag array |
| ParentId | string | No | Parent category ID; an empty string moves it to the root |
| Active | bool | No | Whether enabled |

Returns the string `"Update success"`.

## `delete_product_category`

Delete a category by category ID; the implementation removes the product-category associations for that category.

| Parameter | Type | Required | Description |
| --- | --- | --- | --- |
| Id | string | Yes | Category ID |

Returns the string `"Success deleted"`.

## `list_product_category`

List basic information for all product categories (a flat list).

| Parameter | Type | Required | Description |
| --- | --- | --- | --- |
| (none) | - | - | No input parameters |

Each list item contains `Id`, `SeoName`, and `Title`.

## `get_product_category_detail`

Get category details by `Id` or `SeoName`, returning the current category, ancestor path, and direct children.

| Parameter | Type | Required | Description |
| --- | --- | --- | --- |
| SeoNameOrId | string | Yes | Category ID or SeoName |

Returns a `CategoryDetail` object and throws when not found.

## Common mistakes

1. **Using a title as the unique identifier for updates/deletion**: `update_*` and `delete_*` generally require `Id`; use `list_*` or `get_*_detail` first to obtain the exact ID.
2. **Assuming query functions support paging/filtering**: the current `list_product` and `list_product_category` functions return complete basic lists and do not accept filter parameters.
3. **Missing ParentId semantics when updating hierarchical categories**: an empty `update_product_category.ParentId` means “move to root,” which differs from omitting the field.
4. **Misreading `update_product.CategoryIds`**: omitting it preserves current relations, `[]` removes all relations, and a nonempty array replaces them with the specified manual Commerce category IDs.
5. **Not confirming references before deletion**: delete functions cascade through related records (product variants/product-category relations); query details first to confirm the impact.
6. **Treating `product.price` as an alias for the first variant**: the top-level price is the minimum variant price; use the selected variant for the exact purchase price.
7. **Missing featured image or seoName in storefront data**: AI site-building writes must provide both a stable URL identifier and a loadable featured image, not only a title and price.
8. **Creating categories and products without creating the relationship**: use the relevant manual category ID in `create_product.categoryIds`, then read back `product.categories` after creation; do not merely compare whether two lists exist.

## Implementation source

The source of truth is the implementation in `Kooboo.Sites/AI/Functions/BuiltIn/Commerce/Product/*.cs` and `Kooboo.Sites/AI/Functions/BuiltIn/Commerce/Category/*.cs`.
