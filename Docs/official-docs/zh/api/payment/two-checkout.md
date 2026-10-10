# two-checkout

> TwoCheckout 支付。

## 概述

`k.payment.twoCheckout` 用于创建 TwoCheckout 支付请求，支持卡信息和 3D 验证跳转。使用前需要在站点 CMS 中配置 TwoCheckout 商户信息。

## twoCheckout.charge()

创建 TwoCheckout 支付请求。

```ts
k.api.post(() => {
    const result = k.payment.twoCheckout.charge({
        name: 'TEST order',
        description: 'Order payment',
        totalAmount: 100,
        currency: 'USD',
        order: 'order-id',
        returnUrl: '/payment/success?orderId=order-id',
        cancelUrl: '/payment/cancel?orderId=order-id',
        card: {
            number: '378282246310005',
            expMonth: '03',
            expYear: '2030',
            cvc: '123',
            name: 'John Doe'
        },
        cardType: 'Amex',
        billingAddress: {
            firstName: 'Customer',
            lastName: '2Checkout',
            countryCode: 'US',
            state: 'California',
            city: 'LA',
            address1: 'Test Address',
            zip: '12345',
            email: 'testcustomer@2checkout.com',
            phone: '13800138000'
        }
    })

    if (result.nextAction?.redirectUrl) {
        return k.response.redirect(result.nextAction.redirectUrl)
    }

    return { paid: result.paid, requestId: result.requestId }
})
```

**参数：** `TwoCheckoutParams`

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| cancelUrl | `string` | 是 | 3D 验证取消后的跳转地址 |
| returnUrl | `string` | 是 | 3D 验证成功后的跳转地址 |
| card | `Card` | 是 | 卡信息 |
| cardType | `string` | 是 | 卡类型，如 `Visa`、`Mastercard`、`Amex` |
| billingAddress | `Address` | 是 | 账单地址 |
| totalAmount | `number` | 是 | 订单金额 |
| name | `string` | 是 | 订单名称 |
| description | `string` | 是 | 订单描述 |
| currency | `string` | 是 | 货币，如 `USD` |
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
    const status = k.payment.twoCheckout.checkStatus('request-id')
    return { paid: status.paid, failed: status.failed, status: status.status }
})
```

**返回：** `PaymentStatusResponse`

## updateOrder()

处理 TwoCheckout 回调并更新订单。

**参数：**

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `context` | `object` | 是 | 当前支付回调上下文，通常传入 `k.payment.twoCheckout.context`。 |

```ts
k.api.post(() => {
    const callback = k.payment.twoCheckout.updateOrder(k.payment.twoCheckout.context)
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

## Address

| 属性 | 类型 | 说明 |
|------|------|------|
| firstName | `string` | 名 |
| lastName | `string` | 姓 |
| countryCode | `string` | 国家代码 |
| state | `string` | 州或省 |
| city | `string` | 城市 |
| address1 | `string` | 地址 |
| zip | `string` | 邮编 |
| email | `string` | 邮箱 |
| phone | `string` | 电话 |

## TwoCheckoutSetting

| 属性 | 类型 | 说明 |
|------|------|------|
| baseUrl | `string` | TwoCheckout 接口地址 |
| merchantCode | `string` | 商户 Code |
| secretKey | `string` | Secret Key |
| testMode | `boolean` | 是否测试模式 |
