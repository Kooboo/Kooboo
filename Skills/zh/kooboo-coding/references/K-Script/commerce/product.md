# Commerce Product CRUD

::: tip 流程提醒
`product` 可以单独用于商品数据读写；但如果当前需求属于站点侧 commerce 顾客流程，并且后续会进入 `cart` / `order`，登录链路必须先采用 `k.commerce.customer.login(...)`。完整链路见 [Commerce 顾客登录与下单流程](../../Scenario/commerce-customer-login-flow.md)。
:::

## list()

获取产品列表。

```ts
k.api.get(() => {
    const products = k.commerce.product.list()
    return { total: products.length, list: products }
})
```
**参数：**

| 参数 | 类型 | 说明 |
|------|------|------|
| query.categories | `string[]` | 按分类 ID 筛选 |
| query.includeOffline | `boolean` | 包含下架产品，默认 false |
| query.includeSubCategory | `boolean` | 包含子分类产品，默认 false |
| query.currency | `string` | 货币代码（可选） |
| query.where | `object` | 额外的 K-Data 筛选条件（可选） |

## get()

获取产品详情。**查询不到时抛出异常 `"Product not found"`**。

```ts
k.api.get(() => {
    try {
        return k.commerce.product.get('product-seo-name')
    } catch (e) {
        return { error: e.message }
    }
})
```
**参数：**

| 参数 | 类型 | 说明 |
|------|------|------|
| seoNameOrId | `string` | 产品 SEO 名称或 ID |
| options.currency | `string` | 货币代码（可选） |

## search()

关键词搜索产品。

```ts
k.api.get(() => {
    const result = k.commerce.product.search("关键词")
    return { count: result.list.length, facets: result.facets }
})
```
**参数：**

| 参数 | 类型 | 说明 |
|------|------|------|
| keyword | `string` | 搜索关键词 |
| options.categories | `string[]` | 按分类筛选 |
| options.includeOffline | `boolean` | 包含下架产品，默认 false |
| options.includeSubCategory | `boolean` | 包含子分类产品，默认 false |
| options.facetParams | `{ name: string; label: string }[]` | Facet 筛选条件（可选） |
| options.currency | `string` | 货币代码（可选） |

## create()

创建产品。

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
**Runtime 基础字段：** `title`, `price`, `active`。`seoName` 与 `featuredImage` 虽未由 runtime 强制，但 AI 创建 storefront 数据时必须同时提供，并在创建后读回确认。

## 列表价格与选中变体价格

- `product.price` 是全部变体的最低价，用于列表/起售价。
- 不要把 `product.variants[0].price` 当作列表价格别名。
- PDP、库存和加购在选定具体 variant 后使用该 variant 的 `price` / `inventory` / `id`。

## delete()

删除产品。

```ts
k.api.post(() => {
    k.commerce.product.delete(productId)
    return { success: true }
})
```

## updateField()

更新单个字段，支持多语言。

```ts
k.api.post(() => {
    k.commerce.product.updateField(productId, "title", "新标题")
    k.commerce.product.updateField(productId, "title", "New Title", "en") // 多语言
})
```
**参数：**

| 参数 | 类型 | 说明 |
|------|------|------|
| productId | `string` | 产品 ID |
| field | `string` | 字段名 |
| value | `any` | 字段值 |
| lang | `string` | 语言代码（可选） |

## updateFields()

批量更新字段。

```ts
k.api.post(() => {
    k.commerce.product.updateFields(productId, [
        { property: "title", value: "新标题", lang: "zh" },
        { property: "tags", value: ["tag1", "tag2"] }
    ])
})
```

## 类型提示

如果你不确定 Product 的具体字段（如有哪些属性、Variant 的结构），先检查上下文。如果不知道，用 [Product Types](types/product.md) 获取完整类型定义。

## 边界说明

- 变体操作（createVariant / updateVariantField 等）→ 本文件的 Variant 部分
- 分类关联（addCategory / removeCategory）→ [Category Guide](category.md)
- 本 guide 不展开变体和分类操作细节

---

# Commerce Product Variant

## createVariant()

为产品创建变体。

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
**必填字段：** `sku`, `price`, `inventory`, `active`

## updateVariantField()

更新单个变体字段。

```ts
k.api.post(() => {
    k.commerce.product.updateVariantField(variantId, "sku", "NEW-SKU")
})
```

## updateVariantFields()

批量更新变体字段。

```ts
k.api.post(() => {
    k.commerce.product.updateVariantFields(variantId, [
        { property: "sku", value: "NEW-SKU", lang: "zh" },
        { property: "price", value: 99 }
    ])
})
```

## getPriceDetail()

获取变体的计价结果，包含原始金额、计算后金额和货币信息。`customerId` 可能影响顾客专属价格；`currency` 用于选择返回的货币信息。

```ts
k.api.get(() => {
    const detail = k.commerce.product.getPriceDetail(variantId, {
        customerId: 'customer-id',
        currency: 'CNY'
    })
    return detail
})
```

**参数：**

| 参数 | 类型 | 说明 |
|------|------|------|
| variantId | `string` | 变体 ID |
| options.customerId | `string` | 顾客 ID（可选，影响会员价/已绑定优惠） |
| options.currency | `string` | 货币代码（可选） |

**返回：** `ProductPrice | null` — 含 `amount`、`originalAmount`、`currency` 的计价对象；变体无法计算价格时返回 `null`。

::: warning 勿用 getDiscountPrice
`getDiscountPrice` 在源码中已标记 `[Obsolete]` 且 `[KIgnore]`，**不要**在新代码中调用。统一使用 `getPriceDetail`。
:::

## addReview()

为指定**变体**写入一条商品评价（评分 + 文字），存入站点 Commerce 的 `ProductReview` 表。典型场景：顾客在「我的订单」或商品详情页提交评价后，由 API 调用。

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

**参数（`NewProductReview`）：**

| 参数 | 类型 | 说明 |
|------|------|------|
| variantId | `string` | 变体 ID（必填） |
| customerId | `string` | 顾客 ID |
| rating | `number` | 评分 |
| comment | `string` | 评价正文 |
| images | `string[]` | 可选；**当前实现未写入存储** |

## reviewList()

按变体 ID 查询已保存的评价列表。

```ts
k.api.get(() => {
    return k.commerce.product.reviewList(variantId)
})
```

## addCategory() / removeCategory()

关联或移除产品分类。

```ts
k.api.post(() => {
    k.commerce.product.addCategory(productId, categoryId)
})
k.api.post(() => {
    k.commerce.product.removeCategory(productId, categoryId)
})
```

## 数字产品操作

### addFileDigitalItem()

添加文件类型数字产品。

```ts
k.api.post(() => {
    k.commerce.product.addFileDigitalItem(variantId, '安装包', '/files/setup.exe')
})
```

### addLinkDigitalItem()

添加链接类型数字产品。

```ts
k.api.post(() => {
    k.commerce.product.addLinkDigitalItem(variantId, '下载链接', 'https://example.com/download')
})
```

### addTextDigitalItem()

添加文本类型数字产品。

```ts
k.api.post(() => {
    k.commerce.product.addTextDigitalItem(variantId, '序列号', 'XXXX-XXXX-XXXX')
})
```

### removeDigitalItem()

移除数字产品。

```ts
k.api.post(() => {
    k.commerce.product.removeDigitalItem(variantId, digitalId)
})
```

## 类型提示

如果你不确定 Variant 或 Digital 的具体字段，用 [Product Types](types/product.md) 获取完整类型定义。

## 边界说明

- 产品本身的新建/查询/删除 → 本文件的 Product CRUD 部分
- 本 guide 不展开产品级 CRUD 细节
