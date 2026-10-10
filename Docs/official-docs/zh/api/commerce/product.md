# product

> 产品管理 - 产品 CRUD 和搜索

## 概述

`k.commerce.product` 提供产品相关操作，包括创建、查询、更新、删除等。

## TypeScript 定义

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

## 核心方法

### list()

获取产品列表。

```ts
k.api.get(() => {
    const products = k.commerce.product.list()
    return { count: products.length, products }
})
```
**参数：**

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `query` | `ProductQueryParams` | 否 | 产品列表筛选条件。 |
| `query.categories` | `string[]` | 否 | 按分类 ID 筛选。 |
| `query.includeOffline` | `boolean` | 否 | 是否包含下架产品，默认 `false`。 |
| `query.includeSubCategory` | `boolean` | 否 | 是否包含子分类产品，默认 `false`。 |

**返回值**：`Product[]`。匹配条件的产品列表。

### get()

获取产品详情（包括变体）。

::: tip 异常处理
get 方法查询不到产品时会抛出异常 `"Product not found"`
:::

**参数：**

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `seoNameOrId` | `string` | 是 | 产品 SEO 名称或产品 ID。 |

**返回值**：`Product`。产品详情，包含变体信息。

```ts
k.api.get(() => {
    const product = k.commerce.product.get('product-seo-name')
    return { title: product.title, variants: product.variants }
})

// 异常处理示例
k.api.get(() => {
    try {
        return k.commerce.product.get('product-id')
    } catch (e) {
        return { error: e.message }
    }
})
```

### create()

创建产品。

**参数：**

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `product` | `NewProduct` | 是 | 新产品数据。 |

**返回值**：`Product`。创建后的产品对象。

```ts
k.api.post(() => {
    const product = k.commerce.product.create({
        title: '新产品',
        description: '产品描述',
        price: 99.9,
        active: true
    })
    return { id: product.id, title: product.title }
})
```

### createVariant()

为产品创建变体。

**参数：**

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `productId` | `string` | 是 | 产品 ID。 |
| `variant` | `NewVariant` | 是 | 新变体数据。 |

**返回值**：`Product`。更新后的产品对象。

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

搜索产品。

**参数：**

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `keyword` | `string` | 是 | 搜索关键词。 |
| `options` | `SearchOptions` | 否 | 搜索筛选条件。 |
| `options.categories` | `string[]` | 否 | 按分类 ID 筛选。 |
| `options.includeOffline` | `boolean` | 否 | 是否包含下架产品，默认 `false`。 |
| `options.includeSubCategory` | `boolean` | 否 | 是否包含子分类产品，默认 `false`。 |

**返回值**：`SearchResult`。搜索结果和聚合信息。

```ts
k.api.get(() => {
    const result = k.commerce.product.search("关键词")
    return { count: result.list.length, facets: result.facets }
})

// 带参数筛选
k.api.get(() => {
    const result = k.commerce.product.search("关键词", {
        includeOffline: true,
        includeSubCategory: false
    })
    return { count: result.list.length }
})
```

### updateField()

更新单个字段。

**参数：**

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `productId` | `string` | 是 | 产品 ID。 |
| `field` | `string` | 是 | 字段名。 |
| `value` | `any` | 是 | 字段值。 |
| `lang` | `string` | 否 | 语言代码，用于指定多语言字段。 |

**返回值**：`void`。

```ts
k.api.post(() => {
    // 普通更新
    k.commerce.product.updateField(productId, "title", "新标题")
    return { success: true }
})

// 多语言更新（lang 用于指定语言）
k.api.post(() => {
    k.commerce.product.updateField(productId, "title", "New Title", "en")
    return { success: true }
})
```

### updateFields()

批量更新字段。

**参数：**

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `productId` | `string` | 是 | 产品 ID。 |
| `updateContents` | `UpdateContent[]` | 是 | 要批量更新的字段列表。 |
| `updateContents[].property` | `string` | 是 | 字段名。 |
| `updateContents[].value` | `any` | 是 | 字段值。 |
| `updateContents[].lang` | `string` | 否 | 语言代码，用于指定多语言字段。 |

**返回值**：`void`。

```ts
k.api.post(() => {
    k.commerce.product.updateFields(productId, [
        { property: "title", value: "新标题", lang: "zh" },
        { property: "tags", value: ["tag1", "tag2"] }
    ])
    return { success: true }
})
```

### updateVariantField()

更新变体字段。

**参数：**

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `variantId` | `string` | 是 | 变体 ID。 |
| `field` | `string` | 是 | 字段名。 |
| `value` | `any` | 是 | 字段值。 |

**返回值**：`void`。

```ts
k.api.post(() => {
    k.commerce.product.updateVariantField(variantId, "sku", "NEW-SKU")
    return { success: true }
})
```

### updateVariantFields()

批量更新变体字段。

**参数：**

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `variantId` | `string` | 是 | 变体 ID。 |
| `updateContents` | `UpdateContent[]` | 是 | 要批量更新的字段列表。 |
| `updateContents[].property` | `string` | 是 | 字段名。 |
| `updateContents[].value` | `any` | 是 | 字段值。 |
| `updateContents[].lang` | `string` | 否 | 语言代码，用于指定多语言字段。 |

**返回值**：`void`。

```ts
k.api.post(() => {
    k.commerce.product.updateVariantFields(variantId, [
        { property: "sku", value: "NEW-SKU", lang: "zh" }
    ])
    return { success: true }
})
```

### getDiscountPrice()

获取折扣价格。

**参数：**

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `variantId` | `string` | 是 | 变体 ID。 |
| `options` | `DiscountOptions` | 否 | 折扣计算选项。 |

**返回值**：`number`。应用折扣后的价格。

```ts
k.api.get(() => {
    const price = k.commerce.product.getDiscountPrice(variantId)
    return { price }
})
```

### delete()

删除产品。

**参数：**

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `productId` | `string` | 是 | 要删除的产品 ID。 |

**返回值**：`boolean`。删除成功时返回 `true`。

```ts
k.api.post(() => {
    const success = k.commerce.product.delete(productId)
    return { success }
})
```

## 产品结构

### Product 属性

| 属性 | 说明 | 类型 |
|------|------|------|
| id | 产品 ID | `string` |
| title | 产品名称 | `string` |
| description | 产品描述 | `string` |
| featuredImage | 主图 | `string` |
| images | 图片列表 | `string[]` |
| seoName | SEO 名称 | `string` |
| tags | 标签 | `string[]` |
| price | 价格（默认应用到第一个变体） | `number` |
| inventory | 库存 | `number` |
| active | 是否启用 | `boolean` |
| isDigital | 是否数字产品 | `boolean` |
| autoDelivery | 是否自动发货 | `boolean` |
| variantImage | 变体图片 | `string` |
| maxDownloadCount | 最大下载次数（数字产品） | `number` |
| maxDownloadDay | 最大下载天数（数字产品） | `number` |
| attributes | 属性键值对 | `{key: string, value: string}[]` |
| variants | 变体列表 | [`Variant`](#variant-属性)[] |
| categories | 分类列表 | [`Category`](./category.md)[] |

### Variant 属性

| 属性 | 说明 | 类型 |
|------|------|------|
| id | 变体 ID | `string` |
| productId | 所属产品 ID | `string` |
| createdAt | 创建时间 | `Date` |
| updatedAt | 更新时间 | `Date` |
| sku | SKU 编码 | `string` |
| barcode | 条码 | `string` |
| price | 价格 | `number` |
| inventory | 库存 | `number` |
| weight | 重量 | `number` |
| order | 排序 | `number` |
| sales | 销量 | `number` |
| active | 是否启用 | `boolean` |
| autoDelivery | 是否自动发货 | `boolean` |
| image | 变体图片 | `string` |
| selectedOptions | 选项（如颜色/尺寸） | `{name: string, value: string}[]` |
| digitals | 数字产品列表 | [`Digital`](#digital-属性)[] |

### Digital 属性

| 属性 | 说明 | 类型 |
|------|------|------|
| id | 数字产品 ID | `string` |
| type | 类型 | `'file' \| 'image' \| 'video'` |
| name | 名称 | `string` |
| value | 内容（文件路径/链接/文本） | `string` |
| contentType | 内容类型（仅 file 类型） | `string` |
| size | 大小（仅 file 类型） | `number` |

## 相关文档

- [k.commerce](./commerce.md) - 电商模块概述
- [category](./category.md) - 产品分类
- [cart](./cart.md) - 购物车
