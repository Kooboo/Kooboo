# paypal

> PayPal 支付。

## 概述

PayPal 支付包含 Checkout 和 Form 两种方式。使用前需要在站点 CMS 中配置 PayPal 账号或应用信息。

| API | 说明 |
|-----|------|
| `k.payment.paypalCheckout` | PayPal Checkout |
| `k.payment.paypalForm` | PayPal 表单支付 |

## paypalCheckout.charge()

创建 PayPal Checkout 支付请求。

```ts
k.api.get(() => {
    const result = k.payment.paypalCheckout.charge({
        name: 'T-shirt',
        description: 'Order payment',
        totalAmount: 1.5,
        currency: 'USD',
        order: 'order-id',
        returnUrl: '/payment/success?orderId=order-id',
        cancelUrl: '/payment/cancel?orderId=order-id',
        brandName: 'Kooboo Store',
        customer: 'customer@example.com',
        callbackCodeName: 'PaymentCallback'
    })

    return k.response.renderView(result.nextAction.renderHtml)
})
```

**参数：** `PaypalCheckoutParams`

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| cancelUrl | `string` | 是 | 取消支付时的返回地址 |
| returnUrl | `string` | 是 | 支付成功后的返回地址 |
| brandName | `string` | 否 | PayPal 支付页展示的品牌名 |
| customer | `string` | 否 | 客户标识 |
| totalAmount | `number` | 是 | 订单金额 |
| name | `string` | 是 | 订单名称 |
| description | `string` | 是 | 订单描述 |
| currency | `string` | 是 | 货币，如 `USD` |
| order | `string` | 否 | Commerce 订单 ID。提供后支付成功会更新订单支付状态 |
| callbackCodeName | `string` | 否 | 支付完成后执行的回调代码名称 |

**返回：** `ChargeResponse`

## paypalForm.charge()

创建 PayPal 表单支付请求。

```ts
k.api.get(() => {
    const result = k.payment.paypalForm.charge({
        name: 'T-shirt',
        description: 'Order payment',
        totalAmount: 1.5,
        currency: 'USD',
        order: 'order-id',
        returnUrl: '/payment/success?orderId=order-id',
        cancelUrl: '/payment/cancel?orderId=order-id'
    })

    return k.response.renderView(result.nextAction.renderHtml)
})
```

**参数：** `PaypalFormParams`

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| cancelUrl | `string` | 是 | 取消支付时的返回地址 |
| returnUrl | `string` | 是 | 支付成功后的返回地址 |
| totalAmount | `number` | 是 | 订单金额 |
| name | `string` | 是 | 订单名称 |
| description | `string` | 是 | 订单描述 |
| currency | `string` | 是 | 货币，如 `USD` |
| order | `string` | 否 | Commerce 订单 ID。提供后支付成功会更新订单支付状态 |
| callbackCodeName | `string` | 否 | 支付完成后执行的回调代码名称 |

**返回：** `ChargeResponse`

## checkStatus()

检查支付状态。

```ts
k.api.get(() => {
    const status = k.payment.paypalCheckout.checkStatus('request-id')
    return { paid: status.paid, failed: status.failed, status: status.status }
})
```

**参数：**

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| requestId | `string` | 是 | `charge()` 返回的 `requestId` |

**返回：** `PaymentStatusResponse`

## updateOrder()

处理支付回调并更新订单。`paypalCheckout` 提供该方法。

**参数：**

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `context` | `object` | 是 | 当前支付回调上下文，通常传入 `k.payment.paypalCheckout.context`。 |

```ts
k.api.post(() => {
    const callback = k.payment.paypalCheckout.updateOrder(k.payment.paypalCheckout.context)
    return { paid: callback.paid, status: callback.status }
})
```

**返回：** `PaymentCallback`

## nofityUrl()

处理 PayPal Form 通知回调。方法名在当前类型定义中为 `nofityUrl`。

**参数：**

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `context` | `object` | 是 | 当前 PayPal Form 通知上下文，通常传入 `k.payment.paypalForm.context`。 |

```ts
k.api.post(() => {
    const callback = k.payment.paypalForm.nofityUrl(k.payment.paypalForm.context)
    return { paid: callback.paid, status: callback.status }
})
```

**返回：** `PaymentCallback`

## 配置结构

### PaypalSetting

| 属性 | 类型 | 说明 |
|------|------|------|
| paypalUrl | `string` | PayPal 接口地址 |
| useSandBox | `boolean` | 是否使用沙箱 |
| clientId | `string` | PayPal Client ID |
| secret | `string` | PayPal Secret |

### PaypalFormSetting

| 属性 | 类型 | 说明 |
|------|------|------|
| paypalUrl | `string` | PayPal 接口地址 |
| iPNUrl | `string` | IPN 通知地址 |
| emailAddress | `string` | 收款账号邮箱 |
| logoImage | `string` | 支付页 Logo |
| useSandBox | `boolean` | 是否使用沙箱 |
