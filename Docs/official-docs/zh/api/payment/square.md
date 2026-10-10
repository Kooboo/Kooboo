# square

> Square 支付。

## 概述

`k.payment.square` 用于创建 Square Checkout 支付请求。使用前需要在站点 CMS 中配置 Square 应用和 Location 信息。

## square.charge()

创建 Square Checkout 支付请求。

```ts
k.api.get(() => {
    const result = k.payment.square.charge({
        name: 'Order summary',
        description: 'Order payment',
        totalAmount: 1.5,
        currency: 'USD',
        order: 'order-id',
        returnUrl: '/payment/success?orderId=order-id',
        callbackCodeName: 'PaymentCallback'
    })

    return k.response.redirect(result.nextAction.redirectUrl)
})
```

**参数：** `SquareCheckoutParams`

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
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

**参数：**

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `requestId` | `string` | 是 | `charge()` 返回的 `requestId`。 |

```ts
k.api.get(() => {
    const status = k.payment.square.checkStatus('request-id')
    return { paid: status.paid, failed: status.failed, status: status.status }
})
```

**返回：** `PaymentStatusResponse`

## notify()

处理 Square 通知回调。

**参数：**

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `context` | `object` | 是 | 当前支付通知上下文，通常传入 `k.payment.square.context`。 |

```ts
k.api.post(() => {
    const callback = k.payment.square.notify(k.payment.square.context)
    return { paid: callback.paid, status: callback.status }
})
```

**返回：** `PaymentCallback`

## updateOrder()

处理 Square 回调并更新订单。

**参数：**

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `context` | `object` | 是 | 当前支付回调上下文，通常传入 `k.payment.square.context`。 |

```ts
k.api.post(() => {
    const callback = k.payment.square.updateOrder(k.payment.square.context)
    return { paid: callback.paid, status: callback.status }
})
```

**返回：** `PaymentCallback`

## SquareSetting

| 属性 | 类型 | 说明 |
|------|------|------|
| useSandBox | `boolean` | 是否使用沙箱 |
| applicationId | `string` | Square Application ID |
| accessToken | `string` | Square Access Token |
| locationId | `string` | Square Location ID |
| baseURL | `string` | Square API 地址 |
