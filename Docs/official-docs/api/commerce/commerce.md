# k.commerce

> E-commerce module main module

## Overview

`k.commerce` provides access to main modules related to e-commerce.

## Submodules

| module | Description |
|------|------|
| [Overview](./commerce.md) | Module overview |
| [product](./product.md) | product management |
| [category](./category.md) | Product classification |
| [cart](./cart.md) | shopping cart |
| [order](./order.md) | Order |
| [customer](./customer.md) | Customer management |
| [discount](./discount.md) | Discounts |
| [address](./address.md) | Address management |
| [loyalty](./loyalty.md) | Member points |
| [membership](./membership.md) | Membership level |
| [shipping](./shipping.md) | Delivery method |
| [wishlist](./wishlist.md) | Wishlist |
| [currency](./currency.md) | currency management |
| [settings](./settings.md) | E-commerce settings |

## TypeScript definition

```ts
interface KCommerce {
    product: KProduct;
    cart: KCart;
    order: KOrder;
}
```
