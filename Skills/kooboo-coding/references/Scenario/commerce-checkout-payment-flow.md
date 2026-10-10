# Commerce Checkout and Payment Golden Path

Use this guide for `commerceModel = online-checkout` flows from Cart → Checkout → Payment → Order Result → Account/Orders. The goal is to connect “logged in” to “can place an order and see the real payment status”, avoiding the missing orders, payment-page NREs, blank returns, and still-pending back-office orders seen in three site tests.

For Alipay details, see [Alipay Payment Guide](../K-Script/payment/alipay.md); for the order API, see [Commerce Order](../K-Script/commerce/order.md); for Checkout shipping-address fields, saved-address reuse, and hidden-field preservation, see [Commerce Customer and Shipping Address Contract](commerce-customer-address-flow.md).

## 1. End-to-End Steps (Required Order)

1. **Logged in**: `k.commerce.customer.currentId` is non-empty; Header/Account can restore the same session.
2. **Cart belongs to the current customer**: `cart.currentId` is non-empty and that cart's `customerId === currentId`.
3. **Create the Checkout order**: place the order using only `cart.currentId`; read back `order.customer.id` after creation and require it to equal `currentId` before entering payment.
4. **Navigate to the payment endpoint**: for example `/api/payment/alipay?orderId=...`; the browser navigates directly to this endpoint, where KScript calls `charge({ order })` and writes the payment form through `k.response.write(renderHtml)`.
5. **Keep `requestId`**: use it for pending diagnosis / `checkStatus`.
6. **ReturnUrl displays status only**: read the order or PaymentRequest again; do not treat the return as proof of payment and do not call `order.pay`.
7. **Account/Orders**: list orders with `order.list({ customerId: currentId })` and provide an order-detail entry.

## 2. Cart Ownership

The `order.create(cartId, ...)` `CustomerId` **comes from the cart**, not from a temporary value passed when placing the order. `customer.login(...)` binds the cart associated with the current `ECommerceState.CartId` to the logged-in customer, so the normal login path does not need to rebuild or migrate a guest cart by default.

Checkout must still verify ownership because an old client cartId, session/cookie drift, login in another browser context, or a cart switch after login can make the current cart and customer inconsistent.

Before creating a Checkout order, all of these conditions must hold:

```ts
const customerId = k.commerce.customer.currentId || ''
const cartId = k.commerce.cart.currentId || ''
if (!customerId) return { success: false, error: 'Login required' }
if (!cartId) return { success: false, error: 'Cart required' }

const cart = k.commerce.cart.get(cartId)
if (!cart || !cart.customerId || cart.customerId !== customerId) {
    return {
        success: false,
        error: 'Cart is not bound to the current Commerce customer. Refresh auth/cart state and retry checkout.'
    }
}
```

Troubleshoot in this order:

- Confirm that login, Cart, and Checkout use the same browser session and all use `kb_commerce`;
- After login, read `customer.currentId`, `cart.currentId`, and `cart.get(cartId)` again; do not keep using an old cartId cached by the client;
- Confirm that the login flow actually calls `customer.login(...)`; `updateContact` is not login and cannot replace it;
- Design an auditable migration strategy only when the product explicitly requires cross-session cart merging. Do not make rebuild/migration the default action on every login.

Immediately after the order is created, validate it:

```ts
const order = k.commerce.order.create(cartId, { address: body.address, note: body.note, currency: 'CNY' })
if (!order?.customer?.id || order.customer.id !== customerId) {
    return { success: false, error: 'Order was created without customer ownership; do not start payment' }
}
return { success: true, orderId: order.id, customerId: order.customer.id }
```

## 3. Checkout API (Client-Callable)

The Checkout page can use an API to create the order; it does not need `env="server"` for the form page.

`body.address` must follow [Commerce Customer and Shipping Address Contract](commerce-customer-address-flow.md): map Contact Phone to `address.phone`, copy the complete address when a saved address is selected, and do not infer or trim `AddressModel` from the fields currently visible in Checkout.

```ts
k.api.post((body) => {
    const customerId = k.commerce.customer.currentId || ''
    if (!customerId) return { success: false, error: 'Login required' }

    const cartId = k.commerce.cart.currentId || ''
    if (!cartId) return { success: false, error: 'Cart required' }

    const cart = k.commerce.cart.get(cartId)
    if (!cart) return { success: false, error: 'Cart not found' }
    if (!cart.customerId || cart.customerId !== customerId) {
        return { success: false, error: 'Cart customer mismatch' }
    }
    if (!cart.lines || cart.lines.length === 0) {
        return { success: false, error: 'Cart is empty' }
    }

    const order = k.commerce.order.create(cartId, {
        address: body.address,
        note: body.note,
        currency: 'CNY'
    })

    if (!order?.id || !order?.customer?.id || order.customer.id !== customerId) {
        return { success: false, error: 'Order ownership missing' }
    }

    return { success: true, orderId: order.id }
})
```

After frontend success:

```js
location.href = `/api/payment/alipay?orderId=${encodeURIComponent(orderId)}`
```

Forbidden:

- Put `alipayForm.charge` in the Checkout API and then inject `renderHtml` into the client with `innerHTML`;
- Use an expired `cartId` from page-local cache instead of reading `cart.currentId`;
- Navigate to the payment page when the user is not logged in or the cart has no owner.

## 4. Payment Endpoint (Server-Side Form Output)

Alipay web payment must output `renderHtml` in the **server response**. Create a dedicated Payment API/CodeBlock and let the browser navigate to it directly; do not add `<script env="server">` to a View, and do not fetch JSON first and inject it with client-side `innerHTML`.

```ts
k.api.get((orderId) => {
    const customerId = k.commerce.customer.currentId || ''
    if (!customerId || !orderId) return k.api.notFound()

    const order = k.commerce.order.get(orderId)
    if (!order?.customer?.id || order.customer.id !== customerId) {
        return k.api.notFound()
    }
    if (String(order.currency || '').toUpperCase() !== 'CNY') {
        return k.api.httpCode(400, { error: 'Alipay requires a CNY order' })
    }

    const response = k.payment.alipayForm.charge({
        order: order.id,
        name: `Kooboo order:${order.id}`,
        totalAmount: order.totalAmount,
        currency: 'CNY',
        returnUrl: `/order-result?orderId=${encodeURIComponent(order.id)}`
    })

    if (!response?.requestId || !response?.nextAction?.renderHtml) {
        return k.api.httpCode(502, { error: 'Payment provider did not return a form' })
    }

    // requestId is a recovery hint only. Result/status APIs must still verify order ownership.
    k.cookie.set(`payment_request_${order.id}`, response.requestId, 1)
    k.response.write(response.nextAction.renderHtml)
    return null
})
```

For the complete structure, see [Alipay Payment Guide](../K-Script/payment/alipay.md). Hard requirements:

- Validate `order.customer.id === customer.currentId`;
- `charge({ order: orderId, ... })` must include `order`;
- Preserve `response.requestId`;
- Set `returnUrl` to an order-result page such as `/order-result?orderId=...`;
- When Alipay keys are not configured, tell the user that they must configure the Payment integration in the back office and mark the item incomplete; do not fabricate success.

Common runtime symptoms:

| Symptom | First checks |
| --- | --- |
| Order not found | Not logged in, incorrect orderId, or `order.customer.id` is empty/mismatched |
| Cart not found | An expired cartId was used or `cart.currentId` was not read |
| Object reference / non-JSON response | The payment page did not null-check an empty order/null charge, or the API error was parsed by the frontend as JSON |
| Provider reports success but Commerce remains pending | Use `requestId` to check `PaymentRequest`, provider callback/notify, and `checkStatus`; when provider-success evidence already exists, do not continue guessing that a key is missing |
| Blank return page | The result page did not read the order again by orderId, or forced login caused the return-session check to fail |

## 5. Order Result Page

`returnUrl` only means that the browser returned. The result page must read the real status again:

- Use `orderId` from the query to call the order detail/status API;
- Display `paid` / `pending` / `failed` / `canceled`;
- In pending, `requestId` may be retained for `checkStatus`, but `order.pay` must still not be called;
- Provide legitimate entries such as “View order details” and “Continue payment” (only when unpaid and ownership has been checked).

Reading an order on the result page must also validate `order.customer.id === customer.currentId`. Do not return an order summary to an unauthenticated user based only on a public `orderId`. If the business genuinely needs an unauthenticated return display, design a short-lived, signed, order/request-bound, expiring return credential separately; this is not the default implementation and must not weaken ownership checks for continue payment, cancellation, or the account order list.

## 6. Account / Orders (Required in the First Delivery)

The first online-checkout delivery must include:

- An Account recent-order list: `order.list({ customerId: currentId })`;
- Payment/fulfillment status for each order;
- An entry to order details (`/order-detail?orderId=`);
- Product lines, amounts, address, status, and legitimate recovery actions on the detail page.

Do not use a generic `order.list()` without customer filtering, and do not accept an arbitrary `customerId` from the request body.

## 7. Configuration Entry and Completion Wording

- Payment keys are in the Payment service integration in the back office. Without a dedicated and verified navigation capability, state only where manual configuration is required and retain incomplete evidence; do not guess a back-office URL or present generic `show_action_button` as a Payment configuration shortcut.
- Preflight PASS / `structurally verified` ≠ payment completed.
- Only after the same path succeeds—“place order → payment form output → (sandbox/real) callback or checkStatus → paid orders in both frontend and back office, with the order visible in Account”—may the result be called `runtime verified` / `business verified`.
- When the provider reports success but the Commerce Order is still pending, first inspect `PaymentRequest`, callback/notify, and `checkStatus` for the same `requestId`; do not fall back to an evidence-free “missing key” guess.

## Related References

- Login state: [Commerce Customer Login and Ordering Flow](commerce-customer-login-flow.md)
- Customer and shipping address: [Commerce Customer and Shipping Address Contract](commerce-customer-address-flow.md)
- Dynamicization contract: [Commerce Site Dynamicization Contract](site-building/commerce-dynamicization.md)
- Payment overview: [Payment Guide](../K-Script/payment/index.md)
- Alipay: [Alipay Payment Guide](../K-Script/payment/alipay.md)
- Order: [Commerce Order](../K-Script/commerce/order.md)
- Cart: [Commerce Cart](../K-Script/commerce/cart.md)
