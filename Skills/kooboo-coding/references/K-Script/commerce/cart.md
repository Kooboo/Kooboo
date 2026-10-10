# Commerce Cart

::: warning Login prerequisite
`cart` is part of the commerce customer flow. If the current requirement will eventually enter `order` or another site-side customer business flow, login must first use `k.commerce.customer.login(...)`. See [Commerce customer login and ordering flow](../../Scenario/commerce-customer-login-flow.md) for the complete flow.
:::

::: tip State source
In commerce scenarios, use the server-maintained `kb_commerce` as the default source of the current cart/customer/order context. Account, Header, Cart, and Checkout must not maintain a second `commerce_token` or `localStorage` login state. An additional token may be used as an extra authorization layer only when the user explicitly requires independent application authorization, and it cannot replace `kb_commerce`.
:::

## currentId

Get the current cart ID from `kb_commerce`.

```ts
k.api.get(() => {
    return { cartId: k.commerce.cart.currentId || '' }
})
```

The Header count, Cart page, and Checkout should read from the same current cart to avoid maintaining inconsistent cart IDs independently.

::: danger Customer ownership
`cart.create()` writes the current `ECommerceState.CustomerId` to the cart; `customer.login()` also binds the cart identified by the current `ECommerceState.CartId` to the logged-in customer. Before Checkout, still verify `cart.customerId === customer.currentId`. If they differ, first investigate session/cart drift or a stale client cartId; do not rebuild the cart by default or invent a binding API. See [Checkout and payment golden path](../../Scenario/commerce-checkout-payment-flow.md).
:::

## create()

Create a cart.

```ts
k.api.post(() => {
    const cartId = k.commerce.cart.create({
        extensionButton: { text: '测试', url: '/test' },
        insuranceAmount: 100
    })
    return { cartId }
})
```
**Returns:** `string` — Cart ID

## get()

Get basic cart information (without amount calculation).

```ts
k.api.get(() => {
    return k.commerce.cart.get(cartId)
})
```
**Parameter:** `cartId: string` | **Returns:** `Cart | null`

## getDetail()

Get cart details (including amount calculation).

```ts
k.api.get(() => {
    return k.commerce.cart.getDetail(cartId, {
        country: 'China',
        province: 'Guangdong',
        city: 'Guangzhou'
    })
})
```
**Parameters:**

| Parameter | Type | Description |
|------|------|------|
| cartId | `string` | Cart ID |
| options.currency | `string` | Currency code (optional) |
| options.country | `string` | Country (optional) |
| options.province | `string` | Province (optional) |
| options.city | `string` | City (optional) |

**Returns:** `CartDetail | null`

Use the English `name` from Commerce administrative-division objects for country, province, and city; see [Address Guide](address.md) for selection and display rules.
When both `province` and `city` are provided, `province` is used for the shipping region; `city` is used only when `province` is absent.

### CartDetail display fields

- Header count: `detail.totalQuantity`; do not use `detail.lines.length` as the total product quantity.
- Line unit price: `line.price`; original unit price: `line.originalPrice`.
- Line amount: `line.amount`.
- Cart subtotal/total: `detail.subtotalAmount` / `detail.totalAmount`.
- `unitPrice` and `totalPrice` are not fields of `CartDetail.lines`; using them produces the situation where “the total is correct but the product detail prices are wrong.”

## addOrUpdateLine()

Add or update a cart item. `quantity` is the **final absolute quantity** written to the line, not an increment to the existing quantity; the runtime assigns it directly to the cart line.

```ts
k.api.post(() => {
    const product = k.commerce.product.get('test-product')
    // 仅适用于确认只有一个默认变体的商品；多变体商品须先匹配用户选项。
    const variantId = product.variants[0].id
    k.commerce.cart.addOrUpdateLine(cartId, variantId, 2, {
        note: 'test line',
        groupName: '捆绑组1',
        isMain: true
    })
    return { success: true }
})
```
**Parameters:**

| Parameter | Type | Description |
|------|------|------|
| cartId | `string` | Cart ID |
| variantId | `string` | Product variant ID |
| quantity | `number` | Final absolute quantity to write |
| options.note | `string` | Note (optional) |
| options.groupName | `string` | Bundled product group name (optional) |
| options.isMain | `boolean` | Whether this is the main product (optional) |
| options.extensionButton | `object` | Extension button (optional) |

### Quantity write semantics

- Cart quantity editor: pass the final quantity requested by the user directly; for example, pass `3` when changing 2 to 3.
- PDP “add N more”: the server first reads the current line quantity, then passes `existingQuantity + requestedQuantity`; do not pass `N` directly to `addOrUpdateLine(...)` and expect it to increment automatically.
- For a final quantity `<= 0`, call `removeLine(...)`; do not continue writing a non-positive quantity.
- Shared API quantity parsing must distinguish “missing/invalid” from a valid `0`. Do not use `body.quantity || 1`, because it changes the deletion semantics of `0` to `1`.

The PDP frontend may continue to send only the `quantity` for the current request, but the `/api/cart/add` server endpoint must explicitly own the semantics of “read the existing line and increment it”:

```ts
k.api.post((body) => {
    const cartId = k.commerce.cart.currentId || k.commerce.cart.create()
    const requestedQuantity = Number(body.quantity)
    if (!Number.isFinite(requestedQuantity) || requestedQuantity <= 0) {
        return { success: false, error: 'quantity must be a positive number' }
    }

    const detail = k.commerce.cart.getDetail(cartId)
    const existingLine = detail.lines.find((line) => line.variantId === body.variantId)
    const finalQuantity = (existingLine?.quantity || 0) + requestedQuantity
    k.commerce.cart.addOrUpdateLine(cartId, body.variantId, finalQuantity)
    return { success: true, quantity: finalQuantity }
})
```

### Standard PDP add-to-cart frontend path

The server first renders the default variant into the DOM:

```html
<button id="addToCart"
  k-attribute="data-product-id product_detail.id; data-variant-id product_detail.variants[0].id">
  Add to cart
</button>
```

The client only reads the DOM and notifies the shared Header after success:

```js
const button = document.querySelector('#addToCart')

button.addEventListener('click', async () => {
    const selected = document.querySelector('[name="variantId"]:checked')
    const variantId = selected?.value || button.dataset.variantId
    if (!variantId) return

    const originalText = button.textContent
    button.disabled = true
    button.textContent = 'Adding...'
    try {
        const response = await fetch('/api/cart/add', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ variantId, quantity: 1 })
        })
        if (!response.ok) throw new Error('Add to cart failed')
        window.dispatchEvent(new CustomEvent('kooboo:cart-changed'))
    } finally {
        button.disabled = false
        button.textContent = originalText
    }
})
```

Prohibited:

- Reading k-data server-template variables such as `product_detail.variants[0].id` directly from the client;
- Forcing a read of `input[name="variant"]:checked` when the page has no radio buttons;
- Leaving the button in a permanent loading state after an API failure;
- Updating only the local PDP number after a successful add-to-cart without refreshing the shared Header's authoritative cart state.

The Header listens for the same event:

```js
async function refreshCartCount() {
    const response = await fetch('/api/cart/get')
    const detail = await response.json()
    document.querySelector('#cartCount').textContent = String(detail.totalQuantity || 0)
}

window.addEventListener('kooboo:cart-changed', refreshCartCount)
refreshCartCount()
```

## removeLine()

Remove a cart item.

```ts
k.api.post(() => {
    k.commerce.cart.removeLine(cartId, variantId)
})
```
**Parameters:**

| Parameter | Type | Description |
|------|------|------|
| cartId | `string` | Cart ID |
| variantId | `string` | Product variant ID |
| groupName | `string` | Bundled product group name (optional) |

If the removed line is the main line of a bundle, the runtime removes all lines in that bundle; otherwise, it removes only the matched line.

## updateContact()

Update the cart customer's contact information.

```ts
k.api.post(() => {
    k.commerce.cart.updateContact(cartId, 'test@test.com')
})
```
::: tip Note
If the email matches a customer record, the matching customerId is bound; otherwise, the cart customerId is cleared and only the email remains.
:::

::: warning Usage boundary
`k.commerce.cart.updateContact(...)` only supplements cart contact information; it does not establish a customer login state. Do not use it as a replacement for `k.commerce.customer.login(...)`.
:::

## updateDiscountCodes()

Update cart coupon codes.

```ts
k.api.post(() => {
    const discount = k.commerce.discount.list()[0]
    k.commerce.cart.updateDiscountCodes(cartId, [discount.code])
})
```
**Parameters:**

| Parameter | Type | Description |
|------|------|------|
| cartId | `string` | Cart ID |
| discountCodes | `string[]` | Array of coupon codes |

## updateShipping()

Update the cart shipping method.

```ts
k.api.post(() => {
    k.commerce.cart.updateShipping(cartId, shippingId)
})
```
**Parameters:**

| Parameter | Type | Description |
|------|------|------|
| cartId | `string` | Cart ID |
| shippingId | `string` | Shipping-method ID |

## updateDigitalShipping()

Update the digital shipping method.

```ts
k.api.post(() => {
    k.commerce.cart.updateDigitalShipping(cartId, shippingId)
})
```
**Parameters:**

| Parameter | Type | Description |
|------|------|------|
| cartId | `string` | Cart ID |
| shippingId | `string` | Digital shipping-method ID |

## updateAdditional()

Update additional cart information (points, insurance, and extension button).

```ts
k.api.post(() => {
    k.commerce.cart.updateAdditional(cartId, {
        redeemPoints: true,
        extensionButton: { text: '测试', url: '/test' },
        insuranceAmount: 100
    })
})
```
**Parameters:**

| Parameter | Type | Description |
|------|------|------|
| cartId | `string` | Cart ID |
| options.redeemPoints | `boolean` | Whether to use points (optional) |
| options.extensionButton | `object` | Extension button (optional) |
| options.insuranceAmount | `number` | Insurance amount (optional) |

## Type hint

If you are unsure about the fields of Cart / CartItem, use [Cart Types](types/cart.md) for the complete type definitions.

## Boundaries

- Create an order (a cart is required first) → [Order Guide](order.md)
- Shipping-area values → [Address Guide](address.md)
- Main commerce customer login and ordering flow → [Commerce customer login and ordering flow](../../Scenario/commerce-customer-login-flow.md)
- This guide does not cover order-flow details
