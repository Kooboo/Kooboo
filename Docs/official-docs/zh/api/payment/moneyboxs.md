# moneyboxs

> MoneyBoxs 支付。

## 概述

`k.payment.moneyBoxs` 用于创建 MoneyBoxs 支付请求，支持卡信息、账单地址、收货地址和商品明细。使用前需要在站点 CMS 中配置 MoneyBoxs 商户信息。

## moneyBoxs.charge()

创建 MoneyBoxs 支付请求。

```ts
k.api.post(() => {
    const result = k.payment.moneyBoxs.charge({
        name: 'TEST order',
        description: 'Order payment',
        totalAmount: 100,
        currency: 'USD',
        order: 'order-id',
        returnUrl: '/payment/success?orderId=order-id',
        card: {
            number: '378282246310005',
            expMonth: '03',
            expYear: '2030',
            cvc: '123',
            name: 'John Doe'
        },
        billingAddress: {
            firstName: 'Customer',
            lastName: 'MoneyBoxs',
            countryCode: 'US',
            state: 'California',
            city: 'LA',
            address1: 'Test Address',
            zip: '12345',
            email: 'testcustomer@example.com',
            phone: '13800138000'
        },
        shipAddress: {
            firstName: 'Customer',
            lastName: 'MoneyBoxs',
            countryCode: 'US',
            state: 'California',
            city: 'LA',
            address1: 'Test Address',
            zip: '12345',
            email: 'testcustomer@example.com',
            phone: '13800138000'
        },
        goods: [
            { goodsName: 'T-shirt', quantity: '1', goodsPrice: '100' }
        ]
    })

    if (result.nextAction?.redirectUrl) {
        return k.response.redirect(result.nextAction.redirectUrl)
    }

    return { paid: result.paid, requestId: result.requestId }
})
```

**参数：** `MoneyBoxsParams`

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| returnUrl | `string` | 是 | 3D 验证成功后的跳转地址 |
| card | `Card` | 是 | 卡信息 |
| billingAddress | `Address` | 是 | 账单地址 |
| shipAddress | `Address` | 是 | 收货地址 |
| goods | `Good[]` | 是 | 商品明细 |
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
    const status = k.payment.moneyBoxs.checkStatus('request-id')
    return { paid: status.paid, failed: status.failed, status: status.status }
})
```

**返回：** `PaymentStatusResponse`

## updateOrder()

处理 MoneyBoxs 回调并更新订单。

**参数：**

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `context` | `object` | 是 | 当前支付回调上下文，通常传入 `k.payment.moneyBoxs.context`。 |

```ts
k.api.post(() => {
    const callback = k.payment.moneyBoxs.updateOrder(k.payment.moneyBoxs.context)
    return { paid: callback.paid, status: callback.status }
})
```

**返回：** `PaymentCallback`

## verify3DAuthCallback()

验证 3D Auth 回调。

```ts
k.api.post(() => {
    k.payment.moneyBoxs.verify3DAuthCallback(
        'orderNo',
        'merchantNo',
        'terminalNo',
        '100',
        'hashCode',
        '00'
    )
    return 'success'
})
```

**参数：**

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| orderNo | `string` | 是 | 订单号 |
| merchantNo | `string` | 是 | 商户号 |
| terminalNo | `string` | 是 | 终端号 |
| amount | `string` | 是 | 金额 |
| hashCode | `string` | 是 | 回调 Hash |
| respCode | `string` | 是 | 响应码 |

**返回：** `void`

## Good

| 属性 | 类型 | 说明 |
|------|------|------|
| goodsName | `string` | 商品名称 |
| quantity | `string` | 数量 |
| goodsPrice | `string` | 商品价格 |

## MoneyBoxsSetting

| 属性 | 类型 | 说明 |
|------|------|------|
| baseUrl | `string` | MoneyBoxs API 地址 |
| merchantNo | `string` | 商户号 |
| terminalNo | `string` | 终端号 |
| key | `string` | 密钥 |
| transactionURL | `string` | 交易地址 |
| type | `string` | 类型 |
