# Commerce Customer Address and Shipping Address Contract

Use this contract for Account address storage, Checkout shipping addresses, order-address display/update, and site implementations that must interoperate with Commerce customer addresses in the back office. Address fields must come from the Commerce `Address` contract; do not reverse-engineer the data model from a generated View, one Checkout form, or the fields currently visible on a page.

Before implementation, read all of the following together:

- `updateAddresses()` in [Commerce Customer](../K-Script/commerce/customer.md);
- country, province, and city queries in [Commerce Address](../K-Script/commerce/address.md);
- `AddressModel` in [Customer Types](../K-Script/commerce/types/customer.md);
- [Commerce Order](../K-Script/commerce/order.md) again when creating an order.

## 1. Authoritative Field Contract

Saved Commerce customer addresses and order shipping addresses share these field semantics:

| Field | Role |
|---|---|
| `id` | Stable identifier for a saved address; preserve it when editing an existing address |
| `isDefault` | Customer address-book state; a temporary Checkout shipping address does not require the user to fill it |
| `country` | Commerce country name in English; options come from `k.commerce.address.countries()` |
| `province` | Commerce province/state name in English; options come from `provinces(country)` |
| `city` | Commerce city name in English; options come from `cities(country, province)` |
| `address1` | Primary street address |
| `address2` | Optional additional address, such as building, unit, or room |
| `firstName` | Recipient's given name |
| `lastName` | Recipient's family name |
| `phone` | Recipient contact phone; the Commerce customer address consumes this field |
| `zip` | Postal code |

`email` belongs to customer/Checkout contact information and is not part of `AddressModel`. Checkout may map the phone number in the Contact area to `address.phone`, but must not remove it from the shipping-address payload just because the UI places the phone in Contact.

A page may hide optional fields for business reasons, but “not displayed” does not mean “removed from the model”. When editing an existing address, preserve fields that are not displayed or not submitted in the current round.

## 2. Countries, Provinces, and Cities

Use Commerce administrative divisions as the default option source:

```ts
const countries = k.commerce.address.countries()
const provinces = k.commerce.address.provinces(country)
const cities = k.commerce.address.cities(country, province)
```

- Display text may use `nameTranslations`; save the object's `name` as the value.
- Reload Province after the country changes and clear a Province/City that no longer matches.
- Reload City after the Province changes and clear a City that no longer matches.
- Do not copy a hard-coded country list from another site's View and claim that it is consistent with the back office.
- When Commerce has no Province/City list for a country and the product genuinely needs free input, a text fallback may be offered. State clearly that this is a product fallback, preserve the complete string, and do not describe free input as a back-office administrative division that has been verified to match.

## 3. Updating the Customer Address Book

`k.commerce.customer.updateAddresses(customerId, addresses)` is a **full replacement**: the runtime directly replaces `customer.addresses` with the passed array. Therefore, adding, editing, deleting, and setting a default address must first read the complete current list and then submit the complete next version.

When editing an address, merge through a field allowlist and do not turn missing fields into empty strings:

```ts
const addressFields = [
    'country', 'province', 'city', 'address1', 'address2',
    'firstName', 'lastName', 'phone', 'zip'
]

function copyAddress(source) {
    const result = {
        id: source.id || '',
        isDefault: !!source.isDefault
    }
    addressFields.forEach((name) => {
        result[name] = source[name] || ''
    })
    return result
}

function patchAddress(current, patch) {
    const result = copyAddress(current)
    addressFields.forEach((name) => {
        if (Object.prototype.hasOwnProperty.call(patch, name)) {
            result[name] = patch[name] || ''
        }
    })
    if (Object.prototype.hasOwnProperty.call(patch, 'isDefault')) {
        result.isDefault = !!patch.isDefault
    }
    return result
}
```

Do not process a partial form with this pattern:

```ts
current.phone = body.address.phone || ''
current.address2 = body.address.address2 || ''
```

If the current form has no `phone` or `address2`, this clears existing data when the user changes only a postal code, city, or another field.

The address book must also maintain:

- Preserve `id` when editing an existing address;
- At most one address is the default in a non-empty list;
- After deleting the default address, explicitly choose a new default among the remaining addresses;
- After calling `updateAddresses()`, read back with `customer.get(customerId)`; do not treat a successful void call as persistence verification.

## 4. Checkout Shipping Address

The visible Checkout layout may differ from the Account address book, but the field semantics submitted to `order.create(..., { address })` must remain the same:

- When the address is entered manually, assemble the visible fields into an `Address`; map Contact Phone to `address.phone`.
- When a saved address is selected, copy the complete address object rather than reconstructing a subset containing only fields currently visible on the page.
- Checkout does not update the customer address book by default. Call `updateAddresses()` only when the product explicitly provides “save to address book” or “update this address”.
- Hiding an optional field such as `address2` does not affect order creation, but existing values must still be carried into the order address when a saved address enters Checkout.

After creating an order, read the order back and check key fields in `shippingAddress`; the absence of JavaScript errors or a passing UI audit cannot prove that the address payload is correct.

## 5. Completion and Acceptance

Claim address capability as verified only after the corresponding paths are complete:

1. Add an address in Account and read back the complete `AddressModel`.
2. Use an existing address containing `phone/address2`, change only one other field, and confirm that the unsubmitted fields remain.
3. Use the same Commerce `name` for Country → Province → City options and saved values.
4. Create one order with a manually entered address and one with a saved address, then read back `shippingAddress` and check its fields.
5. Confirm that the back-office customer address, Account address book, and order details consume the same address; do not replace round-trip verification with static source review or generic UI diagnostics.

When real reads/writes cannot be executed, state only contract/structurally verified; do not claim that the fields fully match the back office or that the business loop has passed.
