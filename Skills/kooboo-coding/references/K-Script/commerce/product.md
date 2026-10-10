# Commerce Product CRUD

::: tip Flow reminder
`product` can be used independently for product data reads and writes. However, if the current requirement belongs to the site-side commerce customer flow and will later enter `cart` / `order`, the login flow must first use `k.commerce.customer.login(...)`. See [Commerce customer login and ordering flow](../../Scenario/commerce-customer-login-flow.md) for the complete flow.
:::

## list()

Get the product list.

```ts
k.api.get(() => {
    const products = k.commerce.product.list()
    return { total: products.length, list: products }
})
```
**Parameters:**

| Parameter | Type | Description |
|------|------|------|
| query.categories | `string[]` | Filter by category IDs |
| query.includeOffline | `boolean` | Include offline products; default false |
| query.includeSubCategory | `boolean` | Include products in child categories; default false |
| query.currency | `string` | Currency code (optional) |
| query.where | `object` | Additional K-Data filter (optional) |

## get()

Get product details. **Throws `"Product not found"` when not found**.

```ts
k.api.get(() => {
    try {
        return k.commerce.product.get('product-seo-name')
    } catch (e) {
        return { error: e.message }
    }
})
```
**Parameters:**

| Parameter | Type | Description |
|------|------|------|
| seoNameOrId | `string` | Product SEO name or ID |
| options.currency | `string` | Currency code (optional) |

## search()

Search products by keyword.

```ts
k.api.get(() => {
    const result = k.commerce.product.search("关键词")
    return { count: result.list.length, facets: result.facets }
})
```
**Parameters:**

| Parameter | Type | Description |
|------|------|------|
| keyword | `string` | Search keyword |
| options.categories | `string[]` | Filter by category |
| options.includeOffline | `boolean` | Include offline products; default false |
| options.includeSubCategory | `boolean` | Include products in child categories; default false |
| options.facetParams | `{ name: string; label: string }[]` | Facet filters (optional) |
| options.currency | `string` | Currency code (optional) |

## create()

Create a product.

```ts
k.api.post(() => {
    const product = k.commerce.product.create({
        title: '新产品',
        seoName: 'new-product',
        description: '描述',
        featuredImage: '/_api/v2/AiSiteBuilder/ImagePlaceholder?keyword=new%20product&orientation=squarish&size=regular&page=1',
        images: ['/_api/v2/AiSiteBuilder/ImagePlaceholder?keyword=new%20product&orientation=squarish&size=regular&page=1'],
        price: 99.9,
        inventory: 100,
        active: true
    })
    return { id: product.id, seoName: product.seoName, featuredImage: product.featuredImage }
})
```
**Runtime required fields:** `title`, `price`, `active`. Although `seoName` and `featuredImage` are not enforced by the runtime, AI must provide both when creating storefront data and read them back after creation.

## List price and selected-variant price

- `product.price` is the minimum price across all variants and is used for lists/starting prices.
- Do not treat `product.variants[0].price` as an alias for the list price.
- After a specific variant is selected, the PDP, inventory, and add-to-cart flow use that variant's `price` / `inventory` / `id`.

## delete()

Delete a product.

```ts
k.api.post(() => {
    k.commerce.product.delete(productId)
    return { success: true }
})
```

## updateField()

Update a single field with multilingual support.

```ts
k.api.post(() => {
    k.commerce.product.updateField(productId, "title", "新标题")
    k.commerce.product.updateField(productId, "title", "New Title", "en") // 多语言
})
```
**Parameters:**

| Parameter | Type | Description |
|------|------|------|
| productId | `string` | Product ID |
| field | `string` | Field name |
| value | `any` | Field value |
| lang | `string` | Language code (optional) |

## updateFields()

Update fields in bulk.

```ts
k.api.post(() => {
    k.commerce.product.updateFields(productId, [
        { property: "title", value: "新标题", lang: "zh" },
        { property: "tags", value: ["tag1", "tag2"] }
    ])
})
```

## Type hint

If you are unsure about Product fields, such as available attributes or the Variant structure, check the context first. If it remains unclear, use [Product Types](types/product.md) for the complete type definitions.

## Boundaries

- Variant operations (createVariant / updateVariantField, etc.) → the Variant section of this file
- Category associations (addCategory / removeCategory) → [Category Guide](category.md)
- This guide does not cover variant and category operation details

---

# Commerce Product Variant

## createVariant()

Create a variant for a product.

```ts
k.api.post(() => {
    k.commerce.product.createVariant(productId, {
        sku: 'SKU-001',
        price: 88,
        inventory: 100,
        active: true,
        selectedOptions: [
            { name: '颜色', value: '红色' },
            { name: '尺寸', value: 'M' }
        ]
    })
    return { success: true }
})
```
**Required fields:** `sku`, `price`, `inventory`, `active`

## updateVariantField()

Update a single variant field.

```ts
k.api.post(() => {
    k.commerce.product.updateVariantField(variantId, "sku", "NEW-SKU")
})
```

## updateVariantFields()

Update variant fields in bulk.

```ts
k.api.post(() => {
    k.commerce.product.updateVariantFields(variantId, [
        { property: "sku", value: "NEW-SKU", lang: "zh" },
        { property: "price", value: 99 }
    ])
})
```

## getPriceDetail()

Get the calculated price result for a variant, including the original amount, calculated amount, and currency metadata. `customerId` can affect customer-specific pricing; `currency` selects the returned currency metadata.

```ts
k.api.get(() => {
    const detail = k.commerce.product.getPriceDetail(variantId, {
        customerId: 'customer-id',
        currency: 'CNY'
    })
    return detail
})
```

**Parameters:**

| Parameter | Type | Description |
|------|------|------|
| variantId | `string` | Variant ID |
| options.customerId | `string` | Customer ID (optional; affects membership pricing/bound discounts) |
| options.currency | `string` | Currency code (optional) |

**Returns:** `ProductPrice | null` — a pricing object containing `amount`, `originalAmount`, and `currency`; `null` when the variant has no calculated price.

::: warning Do not use getDiscountPrice
`getDiscountPrice` is marked `[Obsolete]` and `[KIgnore]` in source; **do not** call it in new code. Use `getPriceDetail` consistently.
:::

## addReview()

Write a product review (rating + text) for a specified **variant** to the Site Commerce `ProductReview` table. A typical scenario is an API call after a customer submits a review from “My Orders” or the product-detail page.

```ts
k.api.post(() => {
    k.commerce.product.addReview({
        variantId: 'variant-id',
        customerId: 'customer-id',
        rating: 5,
        comment: '质量很好',
        images: [] // 模型字段存在，当前 runtime 写入逻辑未持久化图片
    })
    return { ok: true }
})
```

**Parameters (`NewProductReview`):**

| Parameter | Type | Description |
|------|------|------|
| variantId | `string` | Variant ID (required) |
| customerId | `string` | Customer ID |
| rating | `number` | Rating |
| comment | `string` | Review text |
| images | `string[]` | Optional; **the current implementation does not persist this** |

## reviewList()

Query saved reviews by variant ID.

```ts
k.api.get(() => {
    return k.commerce.product.reviewList(variantId)
})
```

## addCategory() / removeCategory()

Associate or remove product categories.

```ts
k.api.post(() => {
    k.commerce.product.addCategory(productId, categoryId)
})
k.api.post(() => {
    k.commerce.product.removeCategory(productId, categoryId)
})
```

## Digital-product operations

### addFileDigitalItem()

Add a file-type digital product.

```ts
k.api.post(() => {
    k.commerce.product.addFileDigitalItem(variantId, '安装包', '/files/setup.exe')
})
```

### addLinkDigitalItem()

Add a link-type digital product.

```ts
k.api.post(() => {
    k.commerce.product.addLinkDigitalItem(variantId, '下载链接', 'https://example.com/download')
})
```

### addTextDigitalItem()

Add a text-type digital product.

```ts
k.api.post(() => {
    k.commerce.product.addTextDigitalItem(variantId, '序列号', 'XXXX-XXXX-XXXX')
})
```

### removeDigitalItem()

Remove a digital product.

```ts
k.api.post(() => {
    k.commerce.product.removeDigitalItem(variantId, digitalId)
})
```

## Type hint

If you are unsure about Variant or Digital fields, use [Product Types](types/product.md) for the complete type definitions.

## Boundaries

- Create/query/delete the product itself → the Product CRUD section of this file
- This guide does not cover product-level CRUD details
