# customer

> Customer management

## Overview

`k.commerce.customer` provides customer-related operations, including creation, query, login, order management and other functions.

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

**parameter:**

| Parameter | Type | Required | Description |
|------|------|------|------|
| value | `object` | yes | customer information |
| value.email | `string` | yes | Mail |
| value.firstName | `string` | no | name |
| value.lastName | `string` | no | surname |
| value.password | `string` | no | password |
| value.phone | `string` | no | Telephone |

**Return:** `Customer`

## get()

Get the customer based on customer ID.

```ts
k.api.get(() => {
    const customer = k.commerce.customer.get('customer-id')
    return { id: customer.id, email: customer.email }
})
```

**parameter:**

| Parameter | Type | Required | Description |
|------|------|------|------|
| ID | `string` | yes | Customer ID |

**Return:** `Customer`

## getByEmail()

Get customers based on their email addresses.

```ts
k.api.get(() => {
    const customer = k.commerce.customer.getByEmail('test@example.com')
    return { id: customer.id, email: customer.email }
})
```

**parameter:**

| Parameter | Type | Required | Description |
|------|------|------|------|
| email | `string` | yes | Customer email |

**Return:** `Customer`

## getByPhone()

Obtain customers based on their mobile phone numbers.

```ts
k.api.get(() => {
    const customer = k.commerce.customer.getByPhone('15291101010')
    return { id: customer.id, phone: customer.phone }
})
```

**parameter:**

| Parameter | Type | Required | Description |
|------|------|------|------|
| phone | `string` | yes | Phone number |

**Return:** `Customer`

## login()

Customer login.

```ts
k.api.post(() => {
    const result = k.commerce.customer.login('test@example.com', '123456')
    if (!result) {
        return { error: 'email or password is incorrect' }
    }
    return { id: result.id, email: result.email }
})
```

**parameter:**

| Parameter | Type | Required | Description |
|------|------|------|------|
| email | `string` | yes | Mail |
| password | `string` | yes | password |

**Return:** `Customer | string` - Returns Customer if login is successful, error message is returned if failed

## setPassword()

Set customer password.

```ts
k.api.post(() => {
    k.commerce.customer.setPassword('customer-id', 'new-password')
    return 'password set success'
})
```

**parameter:**

| Parameter | Type | Required | Description |
|------|------|------|------|
| customerId | `string` | yes | Customer ID |
| password | `string` | yes | New Password |

**Return:** `Customer`

## setOAuth()

Set up the customer's OAuth information.

```ts
k.api.post(() => {
    k.commerce.customer.setOAuth('customer-id', 'open-id-123', 'Apple')
    return 'set OAuth success'
})
```

**parameter:**

| Parameter | Type | Required | Description |
|------|------|------|------|
| customerId | `string` | yes | Customer ID |
| openId | `string` | yes | openId of third-party platform |
| Type | `string` | yes | Third-party platform type (such as `Apple`) |

**Return:** `void`

## getByOAuth()

Get customers based on OAuth information.

```ts
k.api.get(() => {
    const customer = k.commerce.customer.getByOAuth('open-id-123', 'Apple')
    return { id: customer.id, email: customer.email }
})
```

**parameter:**

| Parameter | Type | Required | Description |
|------|------|------|------|
| openId | `string` | yes | Third-party platform openId |
| Type | `string` | yes | Third-party platform type |

**Return:** `Customer`

## oAuthList()

Get the OAuth list bound to the customer.

```ts
k.api.get(() => {
    const list = k.commerce.customer.oAuthList('customer-id')
    return { count: list.length, items: list }
})
```

**parameter:**

| Parameter | Type | Required | Description |
|------|------|------|------|
| customerId | `string` | yes | Customer ID |

**Return:** `CustomerOAuthModel[]`

## orders()

Get the customer's order list.

```ts
k.api.get(() => {
    const orders = k.commerce.customer.orders('customer-id', {
        pageIndex: 1,
        pageSize: 10,
        paid: true,
        delivered: true,
        canceled: false
    })
    return { count: orders.count, orders: orders.list }
})
```

**parameter:**

| Parameter | Type | Required | Description |
|------|------|------|------|
| customerId | `string` | yes | Customer ID |
| query | `object` | no | query parameters |
| query.pageIndex | `number` | no | Page number, default 1 |
| query.pageSize | `number` | no | Number of items per page, default 10 |
| query.paid | `boolean` | no | Has it been paid? |
| query.delivered | `boolean` | no | Has it been shipped? |
| query.canceled | `boolean` | no | Has it been cancelled? |
| query.startDate | `Date` | no | start time |
| query.endDate | `Date` | no | Deadline |

**return:**

```ts
{
    list: OrderDetail[];
    count: number;
    pageIndex: number;
    pageSize: number;
}
```

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
    return 'update success'
})
```

**parameter:**

| Parameter | Type | Required | Description |
|------|------|------|------|
| customerId | `string` | yes | Customer ID |
| info | `object` | yes | Update content |
| info.email | `string` | no | Mail |
| info.firstName | `string` | no | name |
| info.lastName | `string` | no | surname |
| info.phone | `string` | no | Telephone |
| info.tags | `string[]` | no | Label |

**Return:** `void`

## updateAddresses()

Update the customer's address list.

::: tip note
This operation is overwriting, that is, the passed array is the final address list.
:::

```ts
k.api.post(() => {
    k.commerce.customer.updateAddresses('customer-id', [{
        country: 'China',
        province: 'Fujian',
        city: 'Xiamen',
        address1: 'address1',
        address2: 'address2',
        zip: '361000',
        firstName: 'firstName',
        lastName: 'lastName',
        phone: '13800138000',
        isDefault: true
    }])
    return 'update success'
})
```

**parameter:**

| Parameter | Type | Required | Description |
|------|------|------|------|
| customerId | `string` | yes | Customer ID |
| addresses | `AddressModel[]` | yes | address list |

**Return:** `void`

## remove()

Delete customer.

```ts
k.api.post(() => {
    k.commerce.customer.remove('customer-id')
    return 'remove success'
})
```

**parameter:**

| Parameter | Type | Required | Description |
|------|------|------|------|
| customerId | `string` | yes | Customer ID |

**Return:** `void`

## structure

### Customer attribute

| Property | Description | Type |
|------|------|------|
| ID | Customer ID | `string` |
| email | Customer email | `string` |
| firstName | Customer name | `string` |
| lastName | Customer last name | `string` |
| phone | Customer phone number | `string` |
| password | Customer password | `string` |
| lastPlaceOrderDate | Last order time | `string` |
| addresses | address list | [AddressModel](#addressmodel-property)[] |
| tags | tag list | `string[]` |
| createdAt | creation time | `string` |
| updatedAt | Update time | `string` |

### AddressModel Property

| Property | Description | Type |
|------|------|------|
| ID | Address ID | `string` |
| isDefault | Whether the default address | `boolean` |
| country | nation | `string` |
| province | province | `string` |
| city | City | `string` |
| address1 | Address 1 | `string` |
| address2 | Address 2 | `string` |
| firstName | Consignee name | `string` |
| lastName | Consignee's last name | `string` |
| phone | Telephone | `string` |
| zip | post code | `string` |

### CustomerOAuthModel Properties

| Property | Description | Type |
|------|------|------|
| CustomerId | Customer ID | `string` |
| OpenId | Third-party platform openId | `string` |
| Type | Third-party platform type | `string` |
| CreatedAt | creation time | `string` |

## Related Docs

- [k.commerce](./commerce.md) - E-commerce module overview
- [order](./order.md) - Order Management
- [cart](./cart.md) - Shopping Cart
