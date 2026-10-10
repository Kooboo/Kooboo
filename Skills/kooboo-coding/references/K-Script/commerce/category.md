# Commerce Category

::: tip Flow reminder
`category` can be used independently for category data management. However, if the current requirement belongs to the site-side commerce customer flow and will later enter `cart` / `order`, the login flow must first use `k.commerce.customer.login(...)`. See [Commerce customer login and ordering flow](../../Scenario/commerce-customer-login-flow.md) for the complete flow.
:::

## Category type definition

```ts
interface CategorySimple {
    id: string;
    title: string;
    description: string;
    image: string;
    imageMeta: string;
    seoName: string;
    parentId: string;
    tags: string[];
    active: boolean;
}

interface CategoryDetail extends CategorySimple {
    path: CategorySimple[];
    children: CategorySimple[];
}
```

## list()

Get the category list.

```ts
k.api.get(() => {
    const categories = k.commerce.category.list()
    return { count: categories.length, categories }
})
```
**Parameters:**

| Parameter | Type | Description |
|------|------|------|
| query.includeOffline | `boolean` | Include offline categories; defaults to false |
| query.where | `object` | Additional K-Data filter (optional) |

## get()

Get category details. **Returns `null` when the category cannot be found**.

```ts
k.api.get(() => {
    const category = k.commerce.category.get('category-seo-name')
    if (!category) {
        return { error: 'Category not found' }
    }
    return { title: category.title, id: category.id }
})
```
**Parameter:** `seoNameOrId: string` | **Returns:** `CategoryDetail | null`

## create()

Create a category.

```ts
k.api.post(() => {
    const category = k.commerce.category.create({
        title: 'Test Category',
        seoName: 'test-category-' + Date.now(),
        image: '/_api/v2/AiSiteBuilder/ImagePlaceholder?keyword=test%20category&orientation=squarish&size=regular&page=1',
        active: true,
        // 可选字段，创建子分类时填入父级分类 id
        parentId: 'parent-category-id'
    })
    return { id: category.id }
})
```
**Required fields:** `title`, `seoName`, `active`

When AI creates a category for a storefront, it must also provide `image` and read back `id`, `seoName`, `image`, and `active` after creation. This requirement does not change the optional fields for generic category writes in the Commerce runtime.

## updateField()

Update a single field, with multilingual support.

```ts
k.api.post(() => {
    k.commerce.category.updateField(categoryId, "title", "新标题")
    k.commerce.category.updateField(categoryId, "title", "New Title", "en")
})
```
**Parameters:**

| Parameter | Type | Description |
|------|------|------|
| categoryId | `string` | Category ID |
| field | `string` | Field name |
| value | `any` | Field value |
| lang | `string` | Language code (optional) |

## updateFields()

Update fields in bulk.

```ts
k.api.post(() => {
    k.commerce.category.updateFields(categoryId, [
        { property: "title", value: "新标题", lang: "zh" },
        { property: "tags", value: ["tag1", "tag2"] }
    ])
})
```

## delete()

Delete a category.

```ts
k.api.post(() => {
    k.commerce.category.delete(categoryId)
    return { success: true }
})
```

## getParents()

Get the parent category chain.

```ts
k.api.get(() => {
    const parents = k.commerce.category.getParents('category-seo-name')
    return { count: parents.length, parents }
})
```
The result contains ancestors from the immediate parent upward and does not include the current category. Empty input returns `null`.

## addProduct() / removeProduct()

Associate or remove a product.

```ts
k.api.post(() => {
    k.commerce.category.addProduct(categoryId, productId)
})
k.api.post(() => {
    k.commerce.category.removeProduct(categoryId, productId)
})
```

## Boundaries

- Filter the product list by category → `list()` in [Product Guide](product.md) supports `query.categories`
- This guide does not cover product-level operation details
