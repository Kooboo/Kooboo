# Commerce Category

::: tip 流程提醒
`category` 可以单独用于分类数据管理；但如果当前需求属于站点侧 commerce 顾客流程，并且后续会进入 `cart` / `order`，登录链路必须先采用 `k.commerce.customer.login(...)`。完整链路见 [Commerce 顾客登录与下单流程](../../Scenario/commerce-customer-login-flow.md)。
:::

## Category 类型定义

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

获取分类列表。

```ts
k.api.get(() => {
    const categories = k.commerce.category.list()
    return { count: categories.length, categories }
})
```
**参数：**

| 参数 | 类型 | 说明 |
|------|------|------|
| query.includeOffline | `boolean` | 包含下架分类，默认 false |
| query.where | `object` | 额外的 K-Data 筛选条件（可选） |

## get()

获取分类详情。**查询不到时返回 `null`**。

```ts
k.api.get(() => {
    const category = k.commerce.category.get('category-seo-name')
    if (!category) {
        return { error: 'Category not found' }
    }
    return { title: category.title, id: category.id }
})
```
**参数：** `seoNameOrId: string` | **返回：** `CategoryDetail | null`

## create()

创建分类。

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
**必填字段：** `title`, `seoName`, `active`

AI 为 storefront 创建分类时还必须提供 `image`，并在创建后读回 `id`、`seoName`、`image`、`active`。该要求不改变 Commerce runtime 对通用分类写入的可选字段。

## updateField()

更新单个字段，支持多语言。

```ts
k.api.post(() => {
    k.commerce.category.updateField(categoryId, "title", "新标题")
    k.commerce.category.updateField(categoryId, "title", "New Title", "en")
})
```
**参数：**

| 参数 | 类型 | 说明 |
|------|------|------|
| categoryId | `string` | 分类 ID |
| field | `string` | 字段名 |
| value | `any` | 字段值 |
| lang | `string` | 语言代码（可选） |

## updateFields()

批量更新字段。

```ts
k.api.post(() => {
    k.commerce.category.updateFields(categoryId, [
        { property: "title", value: "新标题", lang: "zh" },
        { property: "tags", value: ["tag1", "tag2"] }
    ])
})
```

## delete()

删除分类。

```ts
k.api.post(() => {
    k.commerce.category.delete(categoryId)
    return { success: true }
})
```

## getParents()

获取父级分类链。

```ts
k.api.get(() => {
    const parents = k.commerce.category.getParents('category-seo-name')
    return { count: parents.length, parents }
})
```
结果包含从直接父级开始向上的祖先分类，不包含当前分类；空输入返回 `null`。

## addProduct() / removeProduct()

关联或移除产品。

```ts
k.api.post(() => {
    k.commerce.category.addProduct(categoryId, productId)
})
k.api.post(() => {
    k.commerce.category.removeProduct(categoryId, productId)
})
```

## 边界说明

- 产品列表筛选分类 → [Product Guide](product.md) 的 `list()` 支持 `query.categories`
- 本 guide 不展开产品级操作细节
