# Commerce Guide

::: warning Login prerequisite
Whenever the current requirement belongs to a site-side commerce customer flow and connects any part of `product`, `category`, `cart`, or `order` with login/order placement, first read [Customer Guide](customer.md) and [Commerce customer login and ordering flow](../../Scenario/commerce-customer-login-flow.md), and default to `k.commerce.customer.login(...)`.
:::

## Module navigation

According to the user's goal, select one or more leaf skills:

| User goal | Skill to use |
|----------|--------------|
| Product retrieval, lists, search, and details | [Product Guide](product.md) |
| Variant creation, variant-field updates, and digital products | [Variant Guide](product.md) |
| Category retrieval, creation, and product association | [Category Guide](category.md) |
| Cart management, coupons, and shipping | [Cart Guide](cart.md) |
| Order creation, status updates, payment, and cancellation | [Order Guide](order.md) |
| Customer creation, lookup, login, and OAuth | [Customer Guide](customer.md) |
| Customer address book, Checkout shipping address, and backend address interoperability | [Customer and shipping-address contract](../../Scenario/commerce-customer-address-flow.md) + [Address Guide](address.md) + [Customer Types](types/customer.md) |
| Loyalty-point lookup, earning, spending, and tier changes | [Loyalty Guide](loyalty.md) |
| Discount and coupon lookup | [Discount Guide](discount.md) |
| Wishlist lookup, add, and remove | [Wishlist Guide](wishlist.md) |
| Membership-tier lookup | [Membership Guide](membership.md) |
| Shipping-method lookup | [Shipping Guide](shipping.md) |
| Country, province, and city administrative-division lookup | [Address Guide](address.md) |
| Currency lookup and exchange-rate conversion | [Currency Guide](currency.md) |
| Global commerce settings (currency, payment, loyalty points, Webhook) | [Settings Guide](settings.md) |
| Third-party payment integration (WeChat / Alipay / PayPal / Stripe / Square) | [Payment Guide](../payment/index.md) |

## Type definitions

When type fields need confirmation, load as needed:

- [Product Types](types/product.md) — Product / Variant / Digital structures
- [Cart Types](types/cart.md) — Cart / CartLine / CartDetailLine structures
- [Order Types](types/order.md) — Order / OrderLine / Address structures
- [Customer Types](types/customer.md) — Customer / AddressModel structures
- [Loyalty Types](types/loyalty.md) — LoyaltyInfo / Membership structures

## Usage flow

1. If the requirement involves site-side product browsing, adding to cart, checkout, or orders, first load [Customer Guide](customer.md) and [Commerce customer login and ordering flow](../../Scenario/commerce-customer-login-flow.md).
2. Load the corresponding behavior guide.
3. Write the implementation plan.
4. When customer or order shipping addresses are involved, you must load the [customer and shipping-address contract](../../Scenario/commerce-customer-address-flow.md) and [Customer Types](types/customer.md); do not infer the model from fields on existing pages.
5. When other fields are uncertain, load the corresponding type definition.
6. Submit the plan for user confirmation.
7. Start writing code only after user confirmation.

## Boundaries

- This entry only routes; it does not cover concrete API details.
- Each submodule's behavior guide is independent; do not expand across modules.
- If a requirement spans `product`, `category`, `cart`, and `order` and will ultimately form a site-side customer business flow, default to `k.commerce.customer.login(...)` first; see [Commerce customer login and ordering flow](../../Scenario/commerce-customer-login-flow.md) for the complete process.
