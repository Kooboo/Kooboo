# Integrations · Payment

> Group: **Payment**  
> Deep link: `/_Admin/system/config?SiteId={siteGuid}&group=Payment`

The **Payment** group enables and configures **payment channels**: merchant keys, callbacks, and certificates. After saving, Code and Commerce call properties exposed by completion, such as **`k.payment.alipayForm`** and **`k.payment.wechat`**; item **names** must match API property names.

<DocImage src="/cms/settings/settings-integrations-payment.png" alt="Payment group (sensitive fields redacted in doc image)" width="1120" />

List **Value** column may show JSON summaries; doc image redacts secrets—your admin shows your site config.

## Items and API Docs

| Name (list) | Script entry (example) | API doc |
|-------------|------------------------|---------|
| **Alipay** | `k.payment.alipayForm` / `alipayH5` / `alipayApp` | [Alipay](../../../api/payment/alipay.md) |
| **WeChatV3** | `k.payment.wechat` / `weChatH5` / `wechatJsApi` | [WeChat Pay](../../../api/payment/wechat.md) |
| **Paypal** | `k.payment.paypalCheckout` | [PayPal](../../../api/payment/paypal.md) |
| **PaypalForm** | `k.payment.paypalForm` | [PayPal](../../../api/payment/paypal.md) |
| **StripePayment** | `k.payment.stripeCheckout` / `stripe` | [Stripe](../../../api/payment/stripe.md) |
| **SquarePay** | `k.payment.square` | [Square](../../../api/payment/square.md) |
| **Paynl** | `k.payment.paynlCheckout` | [Pay.NL](../../../api/payment/paynl.md) |
| **TwoCheckout** | `k.payment.twoCheckout` | [TwoCheckout](../../../api/payment/two-checkout.md) |
| **MoneyBoxs** | `k.payment.moneyBoxs` | [MoneyBoxs](../../../api/payment/moneyboxs.md) |

Full capabilities and `charge()` flow: [k.payment overview](../../../api/payment/index.md).

## Admin Steps

1. Expand **Payment**, find channel **name**.  
2. **Edit**, fill AppId, keys, certificate **file** fields per Alert.  
3. **Save**, then run a small test payment in a test environment.  

Some fields are **files** (e.g. merchant cert): choose local file (≤10KB), saved as `filename|base64`.

## Relationship to Commerce

- Currency, shipping, orders live under **Commerce**; **payment channel secrets are configured only here**.  
- Multi-currency must satisfy [k.commerce](../../../api/commerce/commerce.md) and channel `supportedCurrency` ([k.payment · multi-currency](../../../api/payment/index.md#multi-currency-note)).

## Related

- [Integrations overview](./index.md)  
- [Commerce overview](../../commerce/index.md)
