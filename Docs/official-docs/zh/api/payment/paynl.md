# paynl

> Pay.NL 支付。

## 概述

`k.payment.paynlCheckout` 用于创建 Pay.NL Checkout 支付请求。使用前需要在站点 CMS 中配置 Pay.NL 销售点和 Token 信息。

## paynlCheckout.charge()

创建 Pay.NL Checkout 支付请求。

```ts
k.api.get(() => {
    const result = k.payment.paynlCheckout.charge({
        name: 'T-shirt',
        description: 'Order payment',
        totalAmount: 1.5,
        currency: 'EUR',
        order: 'order-id',
        returnUrl: '/payment/success?orderId=order-id',
        exchangeUrl: '/payment/paynl/exchange',
        statsData: {
            extra1: 'order-id',
            extra2: '',
            extra3: ''
        },
        callbackCodeName: 'PaymentCallback'
    })

    return k.response.renderView(result.nextAction.renderHtml)
})
```

**参数：** `PaynlCheckoutParams`

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| returnUrl | `string` | 是 | 支付成功后的返回地址 |
| exchangeUrl | `string` | 是 | Pay.NL 用于交换交易状态的地址 |
| statsData | `StatsData` | 否 | 统计扩展数据 |
| totalAmount | `number` | 是 | 订单金额 |
| name | `string` | 是 | 订单名称 |
| description | `string` | 是 | 订单描述 |
| currency | `string` | 是 | 货币，如 `EUR` |
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
    const status = k.payment.paynlCheckout.checkStatus('request-id')
    return { paid: status.paid, failed: status.failed, status: status.status }
})
```

**返回：** `PaymentStatusResponse`

## updateOrder()

处理 Pay.NL 回调并更新订单。

**参数：**

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `context` | `object` | 是 | 当前支付回调上下文，通常传入 `k.payment.paynlCheckout.context`。 |

```ts
k.api.post(() => {
    const callback = k.payment.paynlCheckout.updateOrder(k.payment.paynlCheckout.context)
    return { paid: callback.paid, status: callback.status }
})
```

**返回：** `PaymentCallback`

## StatsData

| 属性 | 类型 | 说明 |
|------|------|------|
| extra1 | `string` | 扩展字段 1 |
| extra2 | `string` | 扩展字段 2 |
| extra3 | `string` | 扩展字段 3 |

## PaynlSetting

| 属性 | 类型 | 说明 |
|------|------|------|
| paynlUrl | `string` | Pay.NL 接口地址 |
| testMode | `boolean` | 是否测试模式 |
| salesLocationCode | `string` | 销售点 Code |
| salesLocationSecret | `string` | 销售点 Secret |
| code | `string` | 配置 Code |
| token | `string` | Token |
