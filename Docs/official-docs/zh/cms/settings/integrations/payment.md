# 服务集成 · Payment

> Group：**Payment**  
> 深链：`/_Admin/system/config?SiteId={站点GUID}&group=Payment`

**Payment** 分组启用并配置各 **支付渠道** 的商户密钥、回调与证书。保存后，Code 与电商通过 **`k.payment.alipayForm`**、**`k.payment.wechat`** 等智能提示中提供的属性调用；配置项 **名称** 须与 API 属性名一致。

<DocImage src="/cms/settings/settings-integrations-payment.png" alt="Payment 分组（文档配图已打码）" width="1120" />

列表 **值** 列可能显示 JSON 摘要；文档配图已对密钥等敏感字段打码，实际后台以你站点配置为准。

## 集成项与 API 文档

| 名称（列表） | 脚本入口（示例） | API 文档 |
|--------------|------------------|----------|
| **Alipay** | `k.payment.alipayForm` / `alipayH5` / `alipayApp` | [支付宝](../../../api/payment/alipay.md) |
| **WeChatV3** | `k.payment.wechat` / `weChatH5` / `wechatJsApi` | [微信支付](../../../api/payment/wechat.md) |
| **Paypal** | `k.payment.paypalCheckout` | [PayPal](../../../api/payment/paypal.md) |
| **PaypalForm** | `k.payment.paypalForm` | [PayPal](../../../api/payment/paypal.md) |
| **StripePayment** | `k.payment.stripeCheckout` / `stripe` | [Stripe](../../../api/payment/stripe.md) |
| **SquarePay** | `k.payment.square` | [Square](../../../api/payment/square.md) |
| **Paynl** | `k.payment.paynlCheckout` | [Pay.NL](../../../api/payment/paynl.md) |
| **TwoCheckout** | `k.payment.twoCheckout` | [TwoCheckout](../../../api/payment/two-checkout.md) |
| **MoneyBoxs** | `k.payment.moneyBoxs` | [MoneyBoxs](../../../api/payment/moneyboxs.md) |

完整能力与 `charge()` 流程见 [k.payment 概述](../../../api/payment/index.md)。

## 后台操作

1. 展开 **Payment**，找到渠道 **名称**。  
2. **编辑**，按 Alert 填写 AppId、密钥、证书文件（`file` 类型字段）等。  
3. **保存** 后在测试环境发起一笔小额支付验证回调。  

部分字段为 **文件**（如商户证书）：选择本地文件（≤10KB），以 `文件名|base64` 形式保存。

## 与电商的关系

- 货币、运费、订单在 **电商** 菜单维护；**支付通道密钥只在本页 Payment 分组配置**。  
- 多货币时须同时满足 [k.commerce](../../../api/commerce/commerce.md) 与渠道的 `supportedCurrency`（见 [k.payment · 多货币](../../../api/payment/index.md#多货币注意)）。

## 相关

- [服务集成概述](./index.md)  
- [电商概述](../../commerce/index.md)
