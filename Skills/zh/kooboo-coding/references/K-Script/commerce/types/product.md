# Commerce Product Types

## Product

```ts
interface Product {
    id: string;
    title: string;
    featuredImage: string;
    images: string[];
    imageMetas: string;
    seoName: string;
    tags: string[];
    /** 所有变体的最低价，适合列表或“起售价”展示。 */
    price: number;
    active: boolean;
    isDigital: boolean;
    attributes: { key: string; value: string }[];
    variants: Variant[];
    variantOptions: VariantOption[];
    currency: Currency;
    /** ProductDetail 才包含。 */
    description?: string;
    /** ProductDetail 才包含。 */
    categories?: Category[];
}

interface VariantOption {
    name: string;
    values: string[];
}

interface Currency {
    code: string;
    symbol: string;
    rate: number;
}

interface ProductPrice {
    currency: Currency;
    originalAmount: number;
    amount: number;
}
```

## Variant

```ts
interface Variant {
    id: string;
    productId: string;
    createdAt: Date;
    updatedAt: Date;
    sku: string;
    barcode: string;
    price: number;
    inventory: number;
    weight: number;
    order: number;
    sales: number;
    active: boolean;
    autoDelivery: boolean;
    image: string;
    selectedOptions: { name: string; value: string }[];
    digitals: Digital[];
}
```

## 价格选择

- `product.price` 是变体价格最小值，不保证等于 `product.variants[0].price`。
- 商品列表使用 `product.price`。
- PDP 选中具体变体后使用该 `variant.price`。
- 加入购物车/愿望清单使用选中 `variant.id`；只有单默认变体商品才直接使用 `variants[0].id`。

## Digital

```ts
interface Digital {
    id: string;
    type: 'file' | 'link' | 'text';
    name: string;
    value: string;          // file path / URL / text content
    contentType: string;    // only for type='file'
    size: number;           // only for type='file'
}
```

## 查询参数

```ts
interface ProductQueryParams {
    categories?: string[];
    includeOffline?: boolean;       // default: false
    includeSubCategory?: boolean;   // default: false
    currency?: string;
    where?: object;
}

interface SearchOptions {
    categories?: string[];
    includeOffline?: boolean;       // default: false
    includeSubCategory?: boolean;   // default: false
    facetParams?: FacetParam[];
    currency?: string;
}

interface FacetParam {
    name: string;
    label: string;
}

interface GetProductOptions {
    currency?: string;
}

interface DiscountOptions {
    customerId?: string;
    discountCode?: string;
}
```
