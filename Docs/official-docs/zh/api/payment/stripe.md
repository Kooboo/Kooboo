# stripe

> Stripe 支付。

## 概述

Stripe 支付包含 Checkout 和 PaymentIntent 两种方式。使用前需要在站点 CMS 中配置 Stripe 密钥和 Webhook 签名密钥。

| API | 说明 |
|-----|------|
| `k.payment.stripeCheckout` | Stripe Checkout 托管支付页 |
| `k.payment.stripe` | Stripe PaymentIntent 和卡管理 |

## stripeCheckout.charge()

创建 Stripe Checkout 支付请求。

```ts
k.api.get(() => {
    const result = k.payment.stripeCheckout.charge({
        name: 'T-shirt',
        description: 'Order payment',
        totalAmount: 1.5,
        currency: 'USD',
        order: 'order-id',
        returnUrl: '/payment/success?orderId=order-id',
        cancelUrl: '/payment/cancel?orderId=order-id',
        customer: 'customer@example.com',
        callbackCodeName: 'PaymentCallback'
    })

    return k.response.renderView(result.nextAction.renderHtml)
})
```

**参数：** `StripeCheckoutParams`

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| cancelUrl | `string` | 是 | Checkout 返回按钮或取消支付地址 |
| returnUrl | `string` | 是 | 支付成功后的返回地址 |
| customer | `string` | 否 | 客户标识。提供后可自动创建 Stripe Customer |
| totalAmount | `number` | 是 | 订单金额 |
| name | `string` | 是 | 订单名称 |
| description | `string` | 是 | 订单描述 |
| currency | `string` | 是 | 货币，如 `USD`、`EUR` |
| order | `string` | 否 | Commerce 订单 ID。提供后支付成功会更新订单支付状态 |
| callbackCodeName | `string` | 否 | 支付完成后执行的回调代码名称 |

**返回：** `ChargeResponse`

## stripe.charge()

创建 Stripe PaymentIntent 支付请求。

```ts
k.api.post(() => {
    const result = k.payment.stripe.charge({
        name: 'T-shirt',
        description: 'Order payment',
        totalAmount: 1.5,
        currency: 'USD',
        order: 'order-id',
        returnUrl: '/payment/success?orderId=order-id',
        customer: 'customer-id',
        paymentMethodId: 'payment-method-id',
        card: {
            number: '4242424242424242',
            expMonth: '12',
            expYear: '2030',
            cvc: '123',
            name: 'John Doe'
        }
    })

    return { requestId: result.requestId, nextAction: result.nextAction }
})
```

**参数：** `StripePaymentIntentParams`

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| returnUrl | `string` | 是 | 支付完成后的返回地址 |
| card | `Card` | 否 | 卡信息 |
| paymentMethodId | `string` | 否 | 已保存的 Stripe PaymentMethod ID |
| customer | `string` | 否 | Stripe Customer ID 或客户标识 |
| totalAmount | `number` | 是 | 订单金额 |
| name | `string` | 是 | 订单名称 |
| description | `string` | 是 | 订单描述 |
| currency | `string` | 是 | 货币，如 `USD`、`EUR` |
| order | `string` | 否 | Commerce 订单 ID。提供后支付成功会更新订单支付状态 |
| callbackCodeName | `string` | 否 | 支付完成后执行的回调代码名称 |

**返回：** `ChargeResponse`

## checkStatus()

检查支付状态。

**参数：**

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `requestId` | `string` | 是 | `charge()` 返回的 `requestId`。 |

```ts
k.api.get(() => {
    const status = k.payment.stripeCheckout.checkStatus('request-id')
    return { paid: status.paid, failed: status.failed, status: status.status }
})
```

**返回：** `PaymentStatusResponse`

## getCardList()

获取指定客户保存的卡列表。

```ts
k.api.get(() => {
    return k.payment.stripe.getCardList('customer-id')
})
```

**参数：**

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| customer | `string` | 是 | Stripe Customer ID |

**返回：** `any`

## createCard()

为客户创建卡。

**参数：**

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `customer` | `string` | 是 | Stripe Customer ID。 |
| `card` | `Card` | 是 | 要保存的卡信息。 |

```ts
k.api.post(() => {
    return k.payment.stripe.createCard('customer-id', {
        number: '4242424242424242',
        expMonth: '12',
        expYear: '2030',
        cvc: '123',
        name: 'John Doe'
    })
})
```

**返回：** `string`

## removeCard()

从客户上移除卡。

**参数：**

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `cardId` | `string` | 是 | 要移除的 Stripe 卡 ID。 |

```ts
k.api.post(() => {
    k.payment.stripe.removeCard('card-id')
    return 'success'
})
```

**返回：** `void`

## updateOrder()

处理 Stripe Checkout 回调并更新订单。

**参数：**

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `context` | `object` | 是 | 当前支付回调上下文，通常传入 `k.payment.stripeCheckout.context`。 |

```ts
k.api.post(() => {
    const callback = k.payment.stripeCheckout.updateOrder(k.payment.stripeCheckout.context)
    return { paid: callback.paid, status: callback.status }
})
```

**返回：** `PaymentCallback`

## Card

| 属性 | 类型 | 说明 |
|------|------|------|
| number | `string` | 卡号 |
| cvc | `string` | CVC |
| expMonth | `string` | 过期月份 |
| expYear | `string` | 过期年份 |
| name | `string` | 持卡人姓名 |

## StripeSetting

| 属性 | 类型 | 说明 |
|------|------|------|
| secretKey | `string` | Stripe Secret Key |
| webhookSigningSecret | `string` | Webhook 签名密钥 |
