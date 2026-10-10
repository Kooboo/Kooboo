# Commerce Customer Types

## Customer

```ts
interface Customer {
    id: string;
    email: string;
    firstName: string;
    lastName: string;
    phone: string;
    password: string;
    lastPlaceOrderDate: string | null;
    addresses: AddressModel[];
    tags: string[];
    createdAt: string;
    updatedAt: string;
}
```

## AddressModel

```ts
interface AddressModel {
    id: string;
    isDefault: boolean;
    country: string;
    province: string;
    city: string;
    address1: string;
    address2: string;
    firstName: string;
    lastName: string;
    phone: string;
    zip: string;
}
```

Field roles:

- `password` is a stored password hash, not a value to expose to clients. Project a safe customer shape instead of returning the full `Customer` object from a public API.
- `id` and `isDefault` are address-book state; preserve `id` when editing an existing address, and allow at most one default address in a non-empty list.
- `country`, `province`, and `city` store the English `name` of Commerce administrative-division objects by default; localized names are for display only.
- `address2` is an optional supplemental address, and `phone` is the recipient's contact phone. When a page does not display these fields, editing must still preserve their existing values.
- `email` is not part of `AddressModel`; the Checkout contact email remains at the customer/contact-information layer.
- `customer.updateAddresses()` replaces the entire address array. See [Commerce customer and shipping-address contract](../../../Scenario/commerce-customer-address-flow.md) for assembling and validating Account, Checkout, and order addresses.

## CustomerOAuthModel

```ts
interface CustomerOAuthModel {
    CustomerId: string;
    OpenId: string;
    Type: string;
    CreatedAt: string;
}
```
