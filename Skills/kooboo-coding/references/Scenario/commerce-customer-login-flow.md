# Commerce Customer Login and Ordering Flow

Use this guide for Commerce sites that need login, registration, accounts, carts, or orders. The goal is for every consumer to share the `kb_commerce` state maintained by Kooboo, preventing the login API, Account, Header, and Cart from each maintaining a different state.

## 1. One Default Login State

The Commerce customer flow uses only these defaults:

- `k.commerce.customer.login(email, password)`: validates identity and writes the current `ECommerceState.CustomerId`; the response maintains `kb_commerce`.
- `k.commerce.customer.currentId`: reads the current Commerce customer ID.
- `k.commerce.customer.get(currentId)`: reads the current customer profile.
- `k.commerce.customer.logout()`: clears the current Commerce customer/cart/order context.

Do not create `commerce_token` by default, and do not put a token in `localStorage` as a second login source of truth for Account/Header. Issue an additional token only when the user explicitly requests independent application authentication; that token cannot replace `customer.login/currentId/logout` or `kb_commerce`.

`customer.login(...)` also binds the cart associated with the current `ECommerceState.CartId` to the logged-in customer. If `cart.customerId !== customer.currentId` still occurs after login, first investigate session/cart-state drift, a stale cartId saved by the client, a login response that did not reach the current browser, or a cart switch after login; do not assume that the cart should be rebuilt and do not invent a binding API.

Forbidden:

- `k.request.cookies.get(...)`: the Cookie API is `k.cookie.get(...)`, but the default Commerce login state does not even require manual cookie reads.
- `k.cookie.delete(...)`: use `k.cookie.remove(...)` to remove a cookie.
- `k.request.query...`: use `k.request.queryString.get(...)` for URL parameters.
- Determining Commerce login only through `getByEmail(...)`, a custom JWT, or `localStorage`.

## 2. Four Minimum Endpoints

### Login

```ts
k.api.post((body) => {
    const email = (body.email || '').trim().toLowerCase()
    const password = body.password || ''
    if (!email || !password) {
        return { success: false, authenticated: false, error: 'email and password are required' }
    }

    const customer = k.commerce.customer.login(email, password)
    if (!customer) {
        return { success: false, authenticated: false, error: 'email or password is incorrect' }
    }

    return {
        success: true,
        authenticated: true,
        customerId: customer.id,
        email: customer.email,
        firstName: customer.firstName
    }
})
```

### Registration and Establishing the Login State

Successful registration does not mean that the user is logged in. After creating the customer, call `login(...)` immediately so Account/Header/Cart read the same Commerce state on the next request.

```ts
k.api.post((body) => {
    const email = (body.email || '').trim().toLowerCase()
    const password = body.password || ''
    if (!email || !password) {
        return { success: false, authenticated: false, error: 'email and password are required' }
    }

    k.commerce.customer.create({
        email,
        password,
        firstName: body.firstName || '',
        lastName: body.lastName || '',
        phone: body.phone || ''
    })

    const customer = k.commerce.customer.login(email, password)
    if (!customer) {
        return { success: false, authenticated: false, error: 'customer created but login state was not established' }
    }

    return { success: true, authenticated: true, customerId: customer.id, email: customer.email }
})
```

### Current Customer

```ts
k.api.get(() => {
    const customerId = k.commerce.customer.currentId || ''
    if (!customerId) {
        return { success: true, authenticated: false, customer: null }
    }

    const customer = k.commerce.customer.get(customerId)
    if (!customer) {
        return { success: true, authenticated: false, customer: null }
    }

    return {
        success: true,
        authenticated: true,
        customer: {
            id: customer.id,
            email: customer.email,
            firstName: customer.firstName,
            lastName: customer.lastName,
            phone: customer.phone
        }
    }
})
```

### Logout

```ts
k.api.post(() => {
    k.commerce.customer.logout()
    return { success: true, authenticated: false }
})
```

## 3. Frontend Consumer Contract

After login or registration succeeds, navigate to Account. Account and the shared Header restore state through the same “current customer” API (`/api/auth/me`). After successful login, the cookie / `kb_commerce` already has `CustomerId`; during the initial render, both guest and authenticated areas may briefly be visible and then be corrected by the asynchronous fetch.

At the current stage, use **API + client-side visibility** by default; do not introduce `env="server"` for the Header's first render.

### Header / Account (Only Recommended Implementation)

```html
<header>
  <div class="hidden md:block">
    <div data-auth-guest class="hidden">
      <div class="flex items-center gap-4">
        <a href="/login">Login</a>
        <a href="/register">Register</a>
      </div>
    </div>
    <div data-auth-user class="hidden">
      <div class="flex items-center gap-4">
        <a href="/account" data-auth-name></a>
        <a href="/orders">Orders</a>
        <button type="button" data-logout>Logout</button>
      </div>
    </div>
  </div>

  <div class="md:hidden">
    <div data-auth-guest class="hidden">
      <a href="/login">Login</a>
      <a href="/register">Register</a>
    </div>
    <div data-auth-user class="hidden">
      <a href="/account" data-auth-name></a>
      <a href="/orders">Orders</a>
      <button type="button" data-logout>Logout</button>
    </div>
  </div>
</header>

<script>
function applyAuthState(state) {
  const authenticated = !!state.authenticated
  document.querySelectorAll('[data-auth-guest]').forEach((el) => {
    el.classList.toggle('hidden', authenticated)
  })
  document.querySelectorAll('[data-auth-user]').forEach((el) => {
    el.classList.toggle('hidden', !authenticated)
  })

  if (!authenticated || !state.customer) return
  document.querySelectorAll('[data-auth-name]').forEach((el) => {
    el.textContent = state.customer.firstName || state.customer.email || ''
  })
}

async function refreshAuth() {
  try {
    const response = await fetch('/api/auth/me', { credentials: 'same-origin' })
    if (!response.ok) throw new Error('Unable to load auth state')
    applyAuthState(await response.json())
  } catch (error) {
    applyAuthState({ authenticated: false, customer: null })
  }
}

document.addEventListener('click', async (event) => {
  const button = event.target.closest('[data-logout]')
  if (!button) return
  await fetch('/api/auth/logout', { method: 'POST', credentials: 'same-origin' })
  await refreshAuth()
})

refreshAuth()
</script>
```

Requirements:

- Both guest and authenticated **sides must initially be `hidden`**. Open only one side after `/api/auth/me` returns; both sides must not be visible during the initial render.
- Dynamic visibility nodes containing `data-auth-guest` / `data-auth-user` may use only the base `hidden` class and must not also carry responsive display classes such as `md:flex`, `lg:flex`, or `sm:block`. Put responsive layout on an outer breakpoint container or an inner `flex/grid` container; otherwise breakpoint styles can override runtime `hidden`.
- Desktop and Mobile may use different DOM, but must reuse the same `applyAuthState`; all `data-auth-guest` and `data-auth-user` nodes follow the same mutually exclusive state.
- Toggle mutually according to `authenticated`; do not only `remove('hidden')` from one side and forget to hide the other.
- Account and Header share the same auth-status endpoint; do not replace it with `localStorage` / `commerce_token`.
- Do not parse the `kb_commerce` cookie manually.
- Refresh the Header after login, registration, and logout (through full-page navigation or another call to `refreshAuth`).
- Cart/Order use the same `kb_commerce` context; do not accept email/password again in the ordering API.
- Do not replace the entire Header with `innerHTML` when refreshing it (especially do not replace the cart node along with it).
- When Account contains an address book or Checkout reuses saved addresses, read the complete `AddressModel` through [the Commerce Customer and Shipping Address Contract](commerce-customer-address-flow.md); do not overwrite hidden fields with only the fields visible on the page.

## 4. Required Pre-Delivery Paths

The login feature may be marked `runtime verified` only after the following same-path checks are complete:

1. Register a new customer → automatically enter the authenticated Account.
2. Log out → Account becomes guest and Header restores login/register.
3. Log in as an existing customer → Account shows customer data, Header shows Account/Orders/Logout, and login/register are **no longer shown**.
4. Refresh Account and any page containing the Header → login state remains consistent; login and account entries **must not** both be visible while loading.
5. Create/read a Cart after login → the Cart's `customerId` matches the current customer.

A Preflight PASS proves only that no known structural error was hit and cannot replace browser runtime evidence above. When Preview/Browser cannot run, mark the result `structurally verified` and do not say that login is complete.

## Related References

- Customer API: [Commerce Customer](../K-Script/commerce/customer.md)
- Customer and shipping address: [Commerce Customer and Shipping Address Contract](commerce-customer-address-flow.md)
- Cart: [Commerce Cart](../K-Script/commerce/cart.md)
- Order: [Commerce Order](../K-Script/commerce/order.md)
- Checkout and payment: [Commerce Checkout and Payment Golden Path](commerce-checkout-payment-flow.md)
- Whole-site Commerce dynamicization: [Commerce Site Dynamicization Contract](site-building/commerce-dynamicization.md)
