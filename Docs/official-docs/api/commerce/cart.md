# cart

> shopping cart

## Overview

`k.commerce.cart` provides shopping cart related operations, supporting functions such as creating a shopping cart, adding products, updating delivery methods, and using coupons.

## Cart data structure

```ts
interface Cart {
    id: string;              // Cart ID
    customerId: string;      // Customer ID
    contact: string;         // Contact email
    country: string;         // Country
    discountCodes: string[]; // Discount codes
    note: string;            // Note
    lines: CartItem[];       // Cart items
    shippingId: string;      // Shipping method ID
    digitalShippingId: string; // Digital shipping method ID
    insuranceAmount: number; // Insurance amount
    extensionButton: { text: string; url: string } | null; // Extension button
    redeemPoints: boolean;   // Whether points are used
    createdAt: Date;         // Created time
    updatedAt: Date;         // Updated time
}
```

## CartDetail data structure

```ts
interface CartDetail {
    id: string;
    customerId: string;
    contact: string;
    country: string;
    currency: { code: string; symbol: string; rate: number }; // Currency information
    discountCodes: string[];
    activeDiscountCodes: string[]; // Active discount codes
    note: string;
    discountAllocations: DiscountAllocation[]; // Automatic discount information
    shippingAllocations: ShippingAllocation[]; // Shipping methods
    shippingAmount: number;       // Shipping amount
    insuranceAmount: number;      // Insurance amount
    subtotalAmount: number;       // Subtotal amount
    totalAmount: number;          // Total amount
    taxAmount: number;            // Tax amount
    totalQuantity: number;        // Total quantity
    redeemPoints: number;         // Redeemed points
    pointsDeductionAmount: number; // Points deduction amount
    earnPoints: number;          // Earned points
    shipping: Shipping | null;   // Shipping method
    lines: CartItem[];           // Cart items
}
```

## CartItem data structure

```ts
interface CartItem {
    variantId: string;            // Variant ID
    productId: string;             // Product ID
    title: string;                // Product title
    options: Option[];            // Product options
    sku: string;                  // Product SKU
    image: string;                // Product image
    originalPrice: number;        // Original product price
    price: number;                // Product price
    discountAllocations: DiscountAllocation[]; // Automatic discount information
    quantity: number;             // Product quantity
    totalQuantity: number;        // Total product quantity
    amount: number;               // Product subtotal amount
    taxAmount: number;            // Product tax amount
    inventory: number;             // Product inventory
    groupName: string;            // Product group name
    isMain: boolean;              // Whether it is the main product
    note: string;                 // Product note
}
```

## ShippingAllocation data structure

```ts
interface ShippingAllocation {
    cost: number;     // Shipping cost
    title: string;    // Shipping method title
    isAddtional: boolean; // Whether it is additional shipping
}
```

## create()

Create a shopping cart.

```ts
k.api.post(() => {
    const cartId = k.commerce.cart.create({
        extensionButton: { text: 'Test', url: '/test' },
        insuranceAmount: 100
    })
    return { cartId }
})
```

**parameter:**

| Parameter | Type | Required | Description |
|------|------|------|------|
| options | object | no | Optional parameters |
| options.extensionButton | `{ text: string, url: string }` | no | Extension button |
| options.insuranceAmount | `number` | no | Insurance amount |

**Return:** `string` - Shopping Cart ID

## get()

Get shopping cart information.

```ts
k.api.get(() => {
    return k.commerce.cart.get(cartId)
})
```

**parameter:**

| Parameter | Type | Required | Description |
|------|------|------|------|
| cartId | `string` | yes | Cart ID |

**Return:** `Cart`

## getDetail()

Get shopping cart details (including amount calculation).

```ts
k.api.get(() => {
    return k.commerce.cart.getDetail(cartId, {
        country: 'China',
        province: 'Guangdong',
        city: 'Guangzhou'
    })
})
```

**parameter:**

| Parameter | Type | Required | Description |
|------|------|------|------|
| cartId | `string` | yes | Cart ID |
| options | object | no | Optional parameters |
| options.country | `string` | no | nation |
| options.province | `string` | no | province |
| options.city | `string` | no | City |

**Return:** `CartDetail`

## addOrUpdateLine()

Add or update shopping cart items.

```ts
k.api.post(() => {
    const product = k.commerce.product.get('test-product')
    const variantId = product.variants[0].id

    k.commerce.cart.addOrUpdateLine(cartId, variantId, 2, {
        note: 'test line',
        groupName: 'Bundle Group 1',
        isMain: true
    })
    return 'success'
})
```

**parameter:**

| Parameter | Type | Required | Description |
|------|------|------|------|
| cartId | `string` | yes | Cart ID |
| variantId | `string` | yes | Product variant ID |
| quantity | `number` | yes | quantity |
| options | object | no | Optional parameters |
| options.note | `string` | no | Remark |
| options.groupName | `string` | no | Bundle product group name (lines with the same groupName will become bundled products) |
| options.isMain | `boolean` | no | Whether to serve as the main product in a bundled product |
| options.extensionButton | `{ text: string, url: string }` | no | Extension button |

**Return:** `void`

## removeLine()

Delete shopping cart items.

```ts
k.api.post(() => {
    const product = k.commerce.product.get('test-product')
    const variantId = product.variants[0].id

    k.commerce.cart.removeLine(cartId, variantId)
    return 'success'
})
```

**parameter:**

| Parameter | Type | Required | Description |
|------|------|------|------|
| cartId | `string` | yes | Cart ID |
| variantId | `string` | yes | Product variant ID |
| groupName | `string` | no | Bundled product group name |

**Return:** `void`

## updateAdditional()

Update cart extensions.

```ts
k.api.post(() => {
    k.commerce.cart.updateAdditional(cartId, {
        redeemPoints: true,
        extensionButton: { text: 'Test', url: '/test' },
        insuranceAmount: 100
    })
    return 'success'
})
```

**parameter:**

| Parameter | Type | Required | Description |
|------|------|------|------|
| cartId | `string` | yes | Cart ID |
| options | `object` | yes | Update content |
| options.redeemPoints | `boolean` | no | Whether to use points |
| options.extensionButton | `{ text: string, url: string }` | no | Extension button |
| options.insuranceAmount | `number` | no | Insurance amount |

**Return:** `void`

## updateContact()

Update shopping cart customer contact information.

```ts
k.api.post(() => {
    k.commerce.cart.updateContact(cartId, 'test@test.com')
    return 'success'
})
```

::: tip note
If email exists in customer, customerId will be automatically bound; otherwise, only email will be filled in
:::

**parameter:**

| Parameter | Type | Required | Description |
|------|------|------|------|
| cartId | `string` | yes | Cart ID |
| email | `string` | yes | Contact email |

**Return:** `void`

## updateDigitalShipping()

Updated digital delivery methods.

```ts
k.api.post(() => {
    k.commerce.cart.updateDigitalShipping(cartId, shippingId)
    return 'success'
})
```

**parameter:**

| Parameter | Type | Required | Description |
|------|------|------|------|
| cartId | `string` | yes | Cart ID |
| shippingId | `string` | yes | Numeric shipping method ID |

**Return:** `void`

## updateDiscountCodes()

Update shopping cart coupon code.

```ts
k.api.post(() => {
    const discount = k.commerce.discount.list()[0]
    k.commerce.cart.updateDiscountCodes(cartId, [discount.code])
    return 'success'
})
```

**parameter:**

| Parameter | Type | Required | Description |
|------|------|------|------|
| cartId | `string` | yes | Cart ID |
| discountCodes | `string[]` | yes | array of coupon codes |

**Return:** `void`

## updateShipping()

Update shopping cart shipping method.

```ts
k.api.post(() => {
    k.commerce.cart.updateShipping(cartId, shippingId)
    return 'success'
})
```

**parameter:**

| Parameter | Type | Required | Description |
|------|------|------|------|
| cartId | `string` | yes | Cart ID |
| shippingId | `string` | yes | Shipping method ID |

**Return:** `void`

## structure

### Cart properties

| Property | Description | Type |
|------|------|------|
| ID | Cart ID | `string` |
| customerId | Customer ID | `string` |
| contact | Contact email | `string` |
| country | nation | `string` |
| discountCodes | coupon code | `string[]` |
| note | Remark | `string` |
| lines | Shopping cart items | [CartItem](#cartitem-properties)[] |
| shippingId | Shipping method ID | `string` |
| digitalShippingId | Numeric shipping method ID | `string` |
| insuranceAmount | Insurance amount | `number` |
| extensionButton | Extension button | `{ text: string, url: string } \| null` |
| redeemPoints | Whether to use points | `boolean` |
| createdAt | creation time | `Date` |
| updatedAt | Update time | `Date` |

### CartDetail Property

| Property | Description | Type |
|------|------|------|
| ID | Cart ID | `string` |
| customerId | Customer ID | `string` |
| contact | Contact email | `string` |
| country | nation | `string` |
| currency | currency information | `{ code: string, symbol: string, rate: number }` |
| discountCodes | coupon code | `string[]` |
| activeDiscountCodes | Activated coupon code | `string[]` |
| note | Remark | `string` |
| discountAllocations | Automatic discount information | `DiscountAllocation[]` |
| shippingAllocations | Shipping method list | [ShippingAllocation](#shippingallocation-property)[] |
| shippingAmount | Delivery amount | `number` |
| insuranceAmount | Insurance amount | `number` |
| subtotalAmount | Subtotal amount | `number` |
| totalAmount | lump sum | `number` |
| taxAmount | tax | `number` |
| totalQuantity | total quantity | `number` |
| redeemPoints | integral | `number` |
| pointsDeductionAmount | Points deduction amount | `number` |
| earnPoints | Get the amount of points | `number` |
| shipping | Delivery method | `Shipping\| null` |
| lines | Shopping cart items | [CartItem](#cartitem-properties)[] |

### CartItem Properties

| Property | Description | Type |
|------|------|------|
| variantId | Product specification ID | `string` |
| productId | Product ID | `string` |
| title | Product name | `string` |
| options | Product options | `{ name: string, value: string }[]` |
| sku | Product SKU | `string` |
| image | Product pictures | `string` |
| originalPrice | Product original price | `number` |
| price | Product price | `number` |
| discountAllocations | Automatic discount information | `DiscountAllocation[]` |
| quantity | Product quantity | `number` |
| totalQuantity | Total quantity of goods | `number` |
| amount | Product subtotal amount | `number` |
| taxAmount | Commodity tax | `number` |
| inventory | Product inventory | `number` |
| groupName | Product group name | `string` |
| isMain | Is it the main product? | `boolean` |
| note | Product remarks | `string` |

### ShippingAllocation Property

| Property | Description | Type |
|------|------|------|
| cost | Delivery fee | `number` |
| title | Shipping method name | `string` |
| isAdditional | Is it an additional shipping fee? | `boolean` |

## Related Docs

- [k.commerce](./commerce.md) - E-commerce module overview
- [product](./product.md) - Product Management
- [category](./category.md) - Classification management
