# Commerce Customer

## create()

Create a customer.

```ts
k.api.post(() => {
    const customer = k.commerce.customer.create({
        email: 'test@example.com',
        firstName: 'Test',
        lastName: 'User',
        password: '123456',
        phone: '15291101010'
    })
    return { id: customer.id, email: customer.email }
})
```
**Required:** `email`

::: tip Note
`email` is the unique identifier for `k.commerce.customer`.
If the current requirement continues into the site-side commerce customer flow through `product` / `category` / `cart` / `order`, use `k.commerce.customer.login(...)` directly after registration instead of going through another login system and connecting to commerce afterward.
:::

## get()

Get a customer by ID.

```ts
k.api.get(() => {
    const customer = k.commerce.customer.get('customer-id')
    if (!customer) return { error: 'customer not found' }
    return { id: customer.id, email: customer.email }
})
```
**Parameter:** `id: string` | **Returns:** `Customer | null`

## getByEmail()

Get a customer by email.

```ts
k.api.get(() => {
    const customer = k.commerce.customer.getByEmail('test@example.com')
    if (!customer) return { error: 'customer not found' }
    return { id: customer.id }
})
```
**Parameter:** `email: string` | **Returns:** `Customer | null`

::: tip Usage recommendation
This is a customer lookup entry point, not a commerce login entry point. It is better suited to admin queries, profile synchronization, order tracking, or reading a known customer than to replacing `k.commerce.customer.login(...)` to establish a site-side customer session.
:::

## getByPhone()

Get a customer by phone number.

```ts
k.api.get(() => {
    const customer = k.commerce.customer.getByPhone('15291101010')
    if (!customer) return { error: 'customer not found' }
    return { id: customer.id }
})
```
**Parameter:** `phone: string` | **Returns:** `Customer | null`

## login()

Log in a customer. On success, this writes the CustomerId to the current Commerce state; later requests restore it through `kb_commerce`.

```ts
k.api.post((body) => {
    const email = (body.email || '').trim().toLowerCase()
    const password = body.password || ''

    const result = k.commerce.customer.login(email, password)
    if (!result) {
        return { success: false, error: 'email or password is incorrect' }
    }

    return {
        success: true,
        authenticated: true,
        id: result.id,
        email: result.email
    }
})
```

::: tip Default entry point
Whenever a requirement belongs to the site-side commerce customer flow, `k.commerce.customer.login(...)` is the default login entry point. Do not treat Kooboo account login, SSO, OAuth, or custom sqlite login as the same commerce customer flow.
:::

::: tip Do not create a second login state by default
Account, Header, Cart, and Order share state through `customer.currentId` / `kb_commerce` by default; no additional JWT or `localStorage` is needed. Issue an additional token only when there is a clearly defined independent application authorization requirement.
:::

## currentId

Read the customer ID in the current `kb_commerce` state.

```ts
k.api.get(() => {
    const customerId = k.commerce.customer.currentId || ''
    if (!customerId) return { authenticated: false, customer: null }
    const customer = k.commerce.customer.get(customerId)
    return { authenticated: !!customer, customer }
})
```

## logout()

Clear the current Commerce customer/cart/order state.

```ts
k.api.post(() => {
    k.commerce.customer.logout()
    return { success: true, authenticated: false }
})
```

## setPassword()

Set a customer password.

```ts
k.api.post(() => {
    k.commerce.customer.setPassword('customer-id', 'new-password')
    return 'password set success'
})
```

## setOAuth()

Bind a third-party OAuth identity.

```ts
k.api.post(() => {
    k.commerce.customer.setOAuth('customer-id', 'open-id-123', 'Apple')
    return 'set OAuth success'
})
```

## getByOAuth()

Get a customer by OAuth information.

```ts
k.api.get(() => {
    const customer = k.commerce.customer.getByOAuth('open-id-123', 'Apple')
    if (!customer) return { error: 'customer not found' }
    return { id: customer.id }
})
```
**Returns:** `Customer | null`

## oAuthList()

Get the list of OAuth identities bound to a customer.

```ts
k.api.get(() => {
    const list = k.commerce.customer.oAuthList('customer-id')
    return { count: list.length }
})
```

## orders()

Get a customer's order list.

```ts
k.api.get(() => {
    const customerId = k.commerce.customer.currentId || ''
    if (!customerId) return { authenticated: false, count: 0, orders: [] }

    const orders = k.commerce.customer.orders(customerId, {
        pageIndex: 1,
        pageSize: 10,
        paid: true
    })
    return { authenticated: true, count: orders?.count || 0, orders: orders?.list || [] }
})
```
Use the current Commerce customer ID in public Account/Orders APIs; do not accept an arbitrary `customerId` from the request.

::: warning Authorization boundary
These low-level methods do not verify caller authorization. Public endpoints must validate the current customer's ownership before exposing or changing customer data; administrative endpoints must enforce the appropriate admin authorization.
:::

## updateInfo()

Update customer information.

```ts
k.api.post(() => {
    k.commerce.customer.updateInfo('customer-id', {
        firstName: 'Updated',
        lastName: 'Name',
        phone: '13900139000',
        tags: ['vip', 'active']
    })
})
```

## updateAddresses()

Update the customer address list. **The entire collection is replaced**: the supplied array replaces the current `customer.addresses`; it is not a partial patch. Before adding, editing, deleting, or setting a default address, read the complete current list and preserve unchanged fields. See [Commerce customer and shipping-address contract](../../Scenario/commerce-customer-address-flow.md) for the complete rules.

```ts
k.api.post(() => {
    k.commerce.customer.updateAddresses('customer-id', [{
        country: 'China',
        province: 'Fujian',
        city: 'Xiamen',
        address1: 'address1',
        address2: 'Room 1201',
        zip: '361000',
        firstName: 'firstName',
        lastName: 'lastName',
        phone: '13800138000',
        isDefault: true
    }])
})
```

## remove()

Delete a customer.

```ts
k.api.post(() => {
    k.commerce.customer.remove('customer-id')
})
```

## Type hint

If you are unsure about the fields of Customer / AddressModel, use [Customer Types](types/customer.md) for the complete type definitions.

## Boundaries

- Address lookup/administrative divisions → [Address Guide](address.md)
- Customer address book and Checkout shipping addresses → [Commerce customer and shipping-address contract](../../Scenario/commerce-customer-address-flow.md)
- The customer order list depends on [Order Types](types/order.md)
- Main commerce customer login and ordering flow → [Commerce customer login and ordering flow](../../Scenario/commerce-customer-login-flow.md)
