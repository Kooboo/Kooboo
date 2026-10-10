# product

> Product Management - Product CRUD and Search

## Overview

`k.commerce.product` provides product-related operations, including creation, query, update, delete, etc.

## TypeScript definition

```ts
interface KProduct {
    get(seoNameOrId: string): Product;
    list(query?: ProductQueryParams): Product[];
    create(product: NewProduct): Product;
    delete(productId: string): boolean;
    search(keyword: string, options?: SearchOptions): SearchResult;
    updateField(productId: string, field: string, value: any, lang?: string): void;
    updateFields(productId: string, updateContents: UpdateContent[]): void;
    getDiscountPrice(variantId: string, options?: DiscountOptions): number;
    createVariant(productId: string, variant: NewVariant): Product;
    updateVariantField(variantId: string, field: string, value: any): void;
    updateVariantFields(variantId: string, updateContents: UpdateContent[]): void;
    addCategory(productId: string, categoryId: string): void;
    removeCategory(productId: string, categoryId: string): void;
    addFileDigitalItem(variantId: string, name: string, filePath: string): void;
    addLinkDigitalItem(variantId: string, name: string, linkUrl: string): void;
    addTextDigitalItem(variantId: string, name: string, text: string): void;
    removeDigitalItem(variantId: string, digitalId: string): void;
}
```

## core method

### list()

Get product list.

```ts
k.api.get(() => {
    const products = k.commerce.product.list()
    return { count: products.length, products }
})
```
**Parameters:**

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `query` | `ProductQueryParams` | No | Product list filters. |
| `query.categories` | `string[]` | No | Filter by category ID. |
| `query.includeOffline` | `boolean` | No | Whether to include offline products. Default is `false`. |
| `query.includeSubCategory` | `boolean` | No | Whether to include products from child categories. Default is `false`. |

**Returns**: `Product[]`. Product list matching the filters.

### get()

Get product details (including variations).

::: tip exception handling
When the get method cannot query the product, it will throw an exception `"Product not found"`
:::

**Parameters:**

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `seoNameOrId` | `string` | Yes | Product SEO name or product ID. |

**Returns**: `Product`. Product detail, including variants.

```ts
k.api.get(() => {
    const product = k.commerce.product.get('product-seo-name')
    return { title: product.title, variants: product.variants }
})

// Error handling example
k.api.get(() => {
    try {
        return k.commerce.product.get('product-id')
    } catch (e) {
        return { error: e.message }
    }
})
```

### create()

Create products.

**Parameters:**

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `product` | `NewProduct` | Yes | New product data. |

**Returns**: `Product`. Created product object.

```ts
k.api.post(() => {
    const product = k.commerce.product.create({
        title: 'New Product',
        description: 'Product description',
        price: 99.9,
        active: true
    })
    return { id: product.id, title: product.title }
})
```

### createVariant()

Create variations for your product.

**Parameters:**

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `productId` | `string` | Yes | Product ID. |
| `variant` | `NewVariant` | Yes | New variant data. |

**Returns**: `Product`. Updated product object.

```ts
k.api.post(() => {
    const product = k.commerce.product.createVariant(productId, {
        sku: 'SKU-001',
        price: 88,
        inventory: 100
    })
    return { variantCount: product.variants.length }
})
```

### search()

Search for products.

**Parameters:**

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `keyword` | `string` | Yes | Search keyword. |
| `options` | `SearchOptions` | No | Search filters. |
| `options.categories` | `string[]` | No | Filter by category ID. |
| `options.includeOffline` | `boolean` | No | Whether to include offline products. Default is `false`. |
| `options.includeSubCategory` | `boolean` | No | Whether to include products from child categories. Default is `false`. |

**Returns**: `SearchResult`. Search result and facet data.

```ts
k.api.get(() => {
    const result = k.commerce.product.search("wireless earbuds")
    return { count: result.list.length, facets: result.facets }
})

// Filter with parameters
k.api.get(() => {
    const result = k.commerce.product.search("wireless earbuds", {
        includeOffline: true,
        includeSubCategory: false
    })
    return { count: result.list.length }
})
```

### updateField()

Update a single field.

**Parameters:**

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `productId` | `string` | Yes | Product ID. |
| `field` | `string` | Yes | Field name. |
| `value` | `any` | Yes | Field value. |
| `lang` | `string` | No | Language code for multilingual fields. |

**Returns**: `void`.

```ts
k.api.post(() => {
    // Standard update
    k.commerce.product.updateField(productId, "title", "Updated Title")
    return { success: true }
})

// Multilingual update (`lang` specifies the language)
k.api.post(() => {
    k.commerce.product.updateField(productId, "title", "New Title", "en")
    return { success: true }
})
```

### updateFields()

Update fields in batches.

**Parameters:**

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `productId` | `string` | Yes | Product ID. |
| `updateContents` | `UpdateContent[]` | Yes | List of fields to update. |
| `updateContents[].property` | `string` | Yes | Field name. |
| `updateContents[].value` | `any` | Yes | Field value. |
| `updateContents[].lang` | `string` | No | Language code for multilingual fields. |

**Returns**: `void`.

```ts
k.api.post(() => {
    k.commerce.product.updateFields(productId, [
        { property: "title", value: "Updated Title", lang: "en" },
        { property: "tags", value: ["tag1", "tag2"] }
    ])
    return { success: true }
})
```

### updateVariantField()

Update variant fields.

**Parameters:**

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `variantId` | `string` | Yes | Variant ID. |
| `field` | `string` | Yes | Field name. |
| `value` | `any` | Yes | Field value. |

**Returns**: `void`.

```ts
k.api.post(() => {
    k.commerce.product.updateVariantField(variantId, "sku", "NEW-SKU")
    return { success: true }
})
```

### updateVariantFields()

Batch update variant fields.

**Parameters:**

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `variantId` | `string` | Yes | Variant ID. |
| `updateContents` | `UpdateContent[]` | Yes | List of fields to update. |
| `updateContents[].property` | `string` | Yes | Field name. |
| `updateContents[].value` | `any` | Yes | Field value. |
| `updateContents[].lang` | `string` | No | Language code for multilingual fields. |

**Returns**: `void`.

```ts
k.api.post(() => {
    k.commerce.product.updateVariantFields(variantId, [
        { property: "sku", value: "NEW-SKU", lang: "zh" }
    ])
    return { success: true }
})
```

### getDiscountPrice()

Get discounted prices.

**Parameters:**

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `variantId` | `string` | Yes | Variant ID. |
| `options` | `DiscountOptions` | No | Discount calculation options. |

**Returns**: `number`. Discounted price.

```ts
k.api.get(() => {
    const price = k.commerce.product.getDiscountPrice(variantId)
    return { price }
})
```

### delete()

Delete product.

**Parameters:**

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `productId` | `string` | Yes | Product ID to delete. |

**Returns**: `boolean`. Returns `true` when deletion succeeds.

```ts
k.api.post(() => {
    const success = k.commerce.product.delete(productId)
    return { success }
})
```

## Product structure

### Product attributes

| Property | Description | Type |
|------|------|------|
| ID | Product ID | `string` |
| title | Product name | `string` |
| description | Product Description | `string` |
| featuredImage | Main picture | `string` |
| images | Picture list | `string[]` |
| seoName | SEO name | `string` |
| tags | Label | `string[]` |
| price | Price (default applied to first variant) | `number` |
| inventory | in stock | `number` |
| active | Whether to enable | `boolean` |
| isDigital | Is it a digital product? | `boolean` |
| autoDelivery | Whether to ship automatically | `boolean` |
| variantImage | Variant pictures | `string` |
| maxDownloadCount | Maximum number of downloads (digital products) | `number` |
| maxDownloadDay | Maximum download days (digital products) | `number` |
| attributes | attribute key-value pair | `{key: string, value: string}[]` |
| variants | Variation list | [Variant](#variant-properties)[] |
| categories | Category list | [Category](./category.md#category-property)[] |

### Variant properties

| Property | Description | Type |
|------|------|------|
| ID | Variant ID | `string` |
| productId | Product ID | `string` |
| createdAt | creation time | `Date` |
| updatedAt | Update time | `Date` |
| sku | SKU code | `string` |
| barcode | barcode | `string` |
| price | price | `number` |
| inventory | in stock | `number` |
| weight | weight | `number` |
| order | sort | `number` |
| sales | Sales volume | `number` |
| active | Whether to enable | `boolean` |
| autoDelivery | Whether to ship automatically | `boolean` |
| image | Variant pictures | `string` |
| selectedOptions | Options (e.g. color/size) | `{name: string, value: string}[]` |
| digitals | Digital product list | [Digital](#digital-property)[] |

### Digital property

| Property | Description | Type |
|------|------|------|
| ID | Numeric Product ID | `string` |
| Type | type | `'file' \| 'image' \| 'video'` |
| name | name | `string` |
| value | Content (file path/link/text) | `string` |
| contentType | Content type (file type only) | `string` |
| size | size (file type only) | `number` |

## Related Docs

- [k.commerce](./commerce.md) - E-commerce module overview
- [category](./category.md) - Product Category
- [cart](./cart.md) - Shopping Cart
