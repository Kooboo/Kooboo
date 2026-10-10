# Payment Guide

## Module navigation

| User goal | Payment method | `nextAction` shape | Guide |
| --- | --- | --- | --- |
| WeChat QR payment, PC web QR payment | `k.payment.wechat` | `qrcode` + `renderHtml` / `responseData` | [WeChat Payment Guide](wechat.md) |
| WeChat in-browser payment | `k.payment.wechatJsApi` | `responseData` | [WeChat Payment Guide](wechat.md) |
| WeChat H5 redirect payment | `k.payment.weChatH5` | `redirectUrl` | [WeChat Payment Guide](wechat.md) |
| WeChat App payment | `k.payment.wechatApp` | `responseData` | [WeChat Payment Guide](wechat.md) |
| Alipay web checkout / form redirect | `k.payment.alipayForm` | `renderHtml` | [Alipay Payment Guide](alipay.md) |
| Alipay H5 payment | `k.payment.alipayH5` | `renderHtml` | [Alipay Payment Guide](alipay.md) |
| Alipay App payment | `k.payment.alipayApp` | `responseData` | [Alipay Payment Guide](alipay.md) |
| PayPal Checkout redirect | `k.payment.paypalCheckout` | `redirect` + `renderHtml` | [PayPal Payment Guide](paypal.md) |
| PayPal form submission | `k.payment.paypalForm` | `form` + `renderHtml` | [PayPal Payment Guide](paypal.md) |
| Stripe Hosted Checkout | `k.payment.stripeCheckout` | `redirect` + `renderHtml` | [Stripe Payment Guide](stripe.md) |
| Stripe direct card payment / 3DS | `k.payment.stripe` | `paid` or `3dSecure` | [Stripe Payment Guide](stripe.md) |
| Square Checkout redirect | `k.payment.square` | `redirect` + `renderHtml` | [Square Payment Guide](square.md) |

## Multi-currency and provider currency limits

Kooboo commerce supports multiple currencies, but whether a third-party payment provider can accept a currency must be filtered through the provider layer.

1. Configure the default and additional currencies in the Commerce admin before designing a multi-currency payment flow.
2. `k.commerce.currency.list()` returns only the site default currency and additional currencies configured in the admin.
3. `k.commerce.order.create(...)` can explicitly pass `currency` to create an order in that currency; if the currency is not configured in the admin, the runtime falls back to the site default currency.
4. Each payment method has a `supportedCurrency` contract. The final available set is the intersection of:
   - currencies configured for the site;
   - provider `supportedCurrency`;
   - currencies currently allowed by the business.

It is recommended to perform this intersection check in the payment page or payment API first:

```ts
k.api.get(() => {
    const configured = k.commerce.currency.list().map(item => item.code.toUpperCase())
    const provider = k.payment.paypalCheckout.supportedCurrency.map(code => code.toUpperCase())
    const available = provider.filter(code => configured.includes(code))
    return {
        defaultCurrency: k.commerce.currency.default,
        available
    }
})
```

Additional rules:

- A single-currency provider (such as WeChat / Alipay / Square) automatically fills in its only supported currency when `currency` is omitted; explicitly passing another currency returns `currency not supported by payment method`.
- A multi-currency provider (such as PayPal / Stripe) should receive an explicit `currency`; do not guess from a default.
- `supportedCurrency` is a provider constraint, not a global Kooboo commerce limitation.

## Standard flow

1. Choose the payment method from the user scenario and target currency; do not default to a provider merely because the request mentions “payment.”
2. When calling `k.payment.xxx.charge(...)`, pass `order` whenever this is a commerce order payment.
3. If the order currency must be changed to one supported by the provider, create the order in the target currency during `k.commerce.order.create(...)`; `k.commerce.currency.convert(...)` is better suited to display prices, quotes, or conversion before choosing a provider.
4. Let Kooboo's built-in `PaymentRequest` / callback / `checkStatus(...)` mechanisms manage payment request state; do not create a second payment-request table by default.
5. Handle the frontend according to the actual `nextAction` shape:
   - `renderHtml`: write it directly from a server endpoint; navigate the browser to that endpoint instead of returning JSON and injecting it with client-side `innerHTML`.
   - `redirectUrl`: redirect from the server or frontend.
   - `responseData`: pass it to the provider SDK or your own frontend logic.
6. Use `callbackCodeName` only when the business genuinely needs additional actions such as notifications, points, invoicing, or fulfillment.

::: danger Do not confirm payment on the return page
`returnUrl`, order-result pages, frontend polling, and public APIs must not call `k.commerce.order.pay(...)`. This method directly changes the order state without validating the provider signature. The payment result must come from Kooboo's built-in callback, or be confirmed by provider `checkStatus(...)` and then applied by the built-in callback worker.
:::

## Kooboo built-in contract

`k.payment.xxx.charge(...)` does more than “send a request to a third party.” The Kooboo runtime also:

- writes the request to the site's built-in `PaymentRequestRepository` first;
- writes back `Paid` / `Failed` after a provider callback or `checkStatus(...)` confirms the result;
- calls commerce `OrderService.Pay(...)` from the built-in callback worker to update the order payment state when `order` is provided;
- executes the corresponding CodeBlock after payment completes when `callbackCodeName` is provided.

Therefore, references should default to reusing the built-in payment flow; add a separate storage layer only when the business genuinely needs extra fields or audit data.

The browser returning to `returnUrl` only means that one navigation completed; it is not trusted payment evidence. The return page should only read and display the order/PaymentRequest state. If the state is still pending, wait for the callback or call the provider status API; do not proactively change the order to paid.

If the provider shows success but the Commerce Order is still pending, inspect `PaymentRequest`, the provider callback/notify, and `checkStatus(...)` using the same `requestId`. Do not continue guessing “the key is not configured” when provider success evidence already exists.

## Configuration-entry boundary

- Payment configuration is under the Payment service integration in the admin, but AI must not guess an admin URL or write a SiteId by hand.
- Generic `show_action_button` is not a Payment configuration entry point and must not be used to fake a shortcut.
- When no dedicated, verified configuration entry point exists, state clearly that the user must configure it manually in the admin and retain the item as incomplete evidence.

## When to continue to a provider guide

- To choose a provider or mode, read the provider guide's “Mode selection” section.
- To confirm provider configuration, read the provider guide's “Pre-run configuration” section.
- To generate executable AI code, follow the provider guide's minimal end-to-end example; do not mix providers.
- To connect payment back to a commerce page, also read [Commerce Order](../commerce/order.md), [Commerce checkout and payment golden path](../../Scenario/commerce-checkout-payment-flow.md), and [Commerce Settings](../commerce/settings.md).

## Scope boundaries

- This entry point covers provider routing and shared contracts only; it does not enumerate every provider parameter.
- Providers support different currencies, so one example cannot be mechanically substituted for another provider.
- A currency not configured in the admin must not be used directly to create an order or start a payment request.
- `renderHtml`, `redirectUrl`, and `responseData` are not synonyms; consume them according to the provider guide and runtime return shape.
