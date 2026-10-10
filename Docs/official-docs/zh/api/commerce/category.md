# category

> 产品分类管理 - 分类 CRUD 和搜索

## 概述

`k.commerce.category` 提供产品分类相关操作，包括创建、查询、更新、删除、获取父级分类等。

## TypeScript 定义

```ts
interface KCategory {
    list(query?: CategoryQueryParams): Category[];
    get(seoNameOrId: string): Category;
    create(category: NewCategory): Category;
    delete(categoryId: string): void;
    updateField(categoryId: string, field: string, value: any, lang?: string): void;
    updateFields(categoryId: string, updateContents: UpdateContent[]): void;
    addProduct(categoryId: string, productId: string): void;
    removeProduct(categoryId: string, productId: string): void;
    getParents(seoNameOrId: string): Category[];
}
```

## 核心方法

### list()

获取分类列表。

**参数：**

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `query` | `CategoryQueryParams` | 否 | 分类列表筛选条件。 |
| `query.includeOffline` | `boolean` | 否 | 是否包含下架分类，默认 `false`。 |

**返回值**：`Category[]`。匹配条件的分类列表。

```ts
k.api.get(() => {
    const categories = k.commerce.category.list()
    return { count: categories.length, categories: categories.slice(0, 3) }
})

// 带参数筛选
k.api.get(() => {
    const categories = k.commerce.category.list({ includeOffline: true })
    return { count: categories.length }
})
```

### get()

获取分类详情。

:::tip 未找到分类
查询不到分类时返回 `null`
:::

**参数：**

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `seoNameOrId` | `string` | 是 | 分类 SEO 名称或分类 ID。 |

**返回值**：`Category | null`。匹配到的分类；不存在时返回 `null`。

```ts
k.api.get(() => {
    const category = k.commerce.category.get('category-seo-name')
    if (!category) {
        return { error: 'Category not found' }
    }
    return { title: category.title, id: category.id }
})
```

### create()

创建分类。

**参数：**

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `category` | `NewCategory` | 是 | 新分类数据。 |

**返回值**：`Category`。创建后的分类对象。

```ts
k.api.post(() => {
    const category = k.commerce.category.create({
        title: 'Test Category',
        description: 'Test Description',
        seoName: 'test-category-' + Date.now(),
        active: true
    })
    return { id: category.id, title: category.title }
})
```

### updateField()

更新单个字段。

**参数：**

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `categoryId` | `string` | 是 | 分类 ID。 |
| `field` | `string` | 是 | 字段名。 |
| `value` | `any` | 是 | 字段值。 |
| `lang` | `string` | 否 | 语言代码，用于指定多语言字段。 |

**返回值**：`void`。

```ts
k.api.post(() => {
    k.commerce.category.updateField(categoryId, "title", "新标题")
    return { success: true }
})

// 多语言更新
k.api.post(() => {
    k.commerce.category.updateField(categoryId, "title", "New Title", "en")
    return { success: true }
})
```

### updateFields()

批量更新字段。

**参数：**

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `categoryId` | `string` | 是 | 分类 ID。 |
| `updateContents` | `UpdateContent[]` | 是 | 要批量更新的字段列表。 |
| `updateContents[].property` | `string` | 是 | 字段名。 |
| `updateContents[].value` | `any` | 是 | 字段值。 |
| `updateContents[].lang` | `string` | 否 | 语言代码，用于指定多语言字段。 |

**返回值**：`void`。

```ts
k.api.post(() => {
    k.commerce.category.updateFields(categoryId, [
        { property: "title", value: "新标题", lang: "zh" },
        { property: "tags", value: ["tag1", "tag2"] }
    ])
    return { success: true }
})
```

### delete()

删除分类。

**参数：**

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `categoryId` | `string` | 是 | 要删除的分类 ID。 |

**返回值**：`void`。

```ts
k.api.post(() => {
    k.commerce.category.delete(categoryId)
    return { success: true }
})
```

### getParents()

获取父级分类链。

**参数：**

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `seoNameOrId` | `string` | 是 | 分类 SEO 名称或分类 ID。 |

**返回值**：`Category[]`。从当前分类向上的父级分类链。

```ts
k.api.get(() => {
    const parents = k.commerce.category.getParents('category-seo-name')
    return { count: parents.length, parents: parents }
})
```

### addProduct()

给分类添加产品。

**参数：**

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `categoryId` | `string` | 是 | 分类 ID。 |
| `productId` | `string` | 是 | 产品 ID。 |

**返回值**：`void`。

```ts
k.api.post(() => {
    k.commerce.category.addProduct(categoryId, productId)
    return { success: true }
})
```

### removeProduct()

从分类中移除产品。

**参数：**

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `categoryId` | `string` | 是 | 分类 ID。 |
| `productId` | `string` | 是 | 产品 ID。 |

**返回值**：`void`。

```ts
k.api.post(() => {
    k.commerce.category.removeProduct(categoryId, productId)
    return { success: true }
})
```

## 分类结构

### Category 属性

| 属性 | 说明 | 类型 |
|------|------|------|
| id | 分类 ID | `string` |
| title | 分类名称 | `string` |
| description | 分类描述 | `string` |
| image | 分类图片 | `string` |
| seoName | SEO 名称 | `string` |
| parentId | 父级分类 ID | `string` |
| tags | 标签 | `string[]` |
| active | 是否启用 | `boolean` |
| order | 排序 | `number` |
| createdAt | 创建时间 | `Date` |
| updatedAt | 更新时间 | `Date` |
| children | 子分类列表 | [`Category`](#category-属性)[] |

## 相关文档

- [k.commerce](./commerce.md) - 电商模块概述
- [product](./product.md) - 产品管理
- [cart](./cart.md) - 购物车
