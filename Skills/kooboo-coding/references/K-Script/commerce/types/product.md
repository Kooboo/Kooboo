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

## Price selection

- `product.price` is the minimum variant price and is not guaranteed to equal `product.variants[0].price`.
- Use `product.price` in product lists.
- After a specific variant is selected on the PDP, use that `variant.price`.
- Use the selected `variant.id` for adding to the cart or wishlist; use `variants[0].id` directly only for products with a single default variant.

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

## Query parameters

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
