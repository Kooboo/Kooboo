# alipay

> 支付宝支付。

## 概述

支付宝支付包含 App、网页表单和 H5 三种方式。使用前需要在站点 CMS 中配置支付宝应用信息。

| API | 说明 |
|-----|------|
| `k.payment.alipayApp` | 支付宝 App 支付 |
| `k.payment.alipayForm` | 支付宝网页支付，通常渲染 `nextAction.renderHtml` |
| `k.payment.alipayH5` | 支付宝 H5 支付 |

## alipayApp.charge()

创建支付宝 App 支付请求。

```ts
k.api.post(() => {
    const result = k.payment.alipayApp.charge({
        name: 'Test Alipay App',
        description: 'Order payment',
        totalAmount: 1.5,
        currency: 'CNY',
        order: 'order-id',
        callbackCodeName: 'PaymentCallback'
    })

    return { requestId: result.requestId, paid: result.paid }
})
```

**参数：** `ChargeParams`

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| totalAmount | `number` | 是 | 订单金额 |
| name | `string` | 是 | 订单名称 |
| description | `string` | 是 | 订单描述 |
| currency | `string` | 是 | 货币，如 `CNY` |
| order | `string` | 否 | Commerce 订单 ID。提供后支付成功会更新订单支付状态 |
| callbackCodeName | `string` | 否 | 支付完成后执行的回调代码名称 |

**返回：** `ChargeResponse`

## alipayForm.charge()

创建支付宝网页支付请求。

```ts
k.api.get(() => {
    const result = k.payment.alipayForm.charge({
        name: 'Test Alipay Form',
        description: 'Order payment',
        totalAmount: 1.5,
        currency: 'CNY',
        order: 'order-id',
        returnUrl: '/payment/success?orderId=order-id',
        callbackCodeName: 'PaymentCallback'
    })

    return k.response.renderView(result.nextAction.renderHtml)
})
```

**参数：** `AlipayFormParams`

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| returnUrl | `string` | 是 | 支付完成后的跳转地址 |
| totalAmount | `number` | 是 | 订单金额 |
| name | `string` | 是 | 订单名称 |
| description | `string` | 是 | 订单描述 |
| currency | `string` | 是 | 货币，如 `CNY` |
| order | `string` | 否 | Commerce 订单 ID。提供后支付成功会更新订单支付状态 |
| callbackCodeName | `string` | 否 | 支付完成后执行的回调代码名称 |

**返回：** `ChargeResponse`

## alipayH5.charge()

创建支付宝 H5 支付请求。

```ts
k.api.get(() => {
    const result = k.payment.alipayH5.charge({
        name: 'Test Alipay H5',
        description: 'Order payment',
        totalAmount: 1.5,
        currency: 'CNY',
        order: 'order-id',
        returnUrl: '/payment/success?orderId=order-id'
    })

    return k.response.renderView(result.nextAction.renderHtml)
})
```

**参数：** `AlipayFormParams`

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| returnUrl | `string` | 是 | 支付完成后的跳转地址 |
| totalAmount | `number` | 是 | 订单金额 |
| name | `string` | 是 | 订单名称 |
| description | `string` | 是 | 订单描述 |
| currency | `string` | 是 | 货币，如 `CNY` |
| order | `string` | 否 | Commerce 订单 ID。提供后支付成功会更新订单支付状态 |
| callbackCodeName | `string` | 否 | 支付完成后执行的回调代码名称 |

**返回：** `ChargeResponse`

## checkStatus()

检查支付状态。

```ts
k.api.get(() => {
    const status = k.payment.alipayForm.checkStatus('request-id')
    return { paid: status.paid, failed: status.failed, status: status.status }
})
```

**参数：**

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| requestId | `string` | 是 | `charge()` 返回的 `requestId` |

**返回：** `PaymentStatusResponse`

## updateOrder()

处理支付回调并更新订单。该方法仅 `alipayForm` 提供。

```ts
k.api.post(() => {
    const callback = k.payment.alipayForm.updateOrder(k.payment.alipayForm.context)
    return { paid: callback.paid, status: callback.status }
})
```

**参数：**

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| context | `object` | 是 | 当前支付回调上下文 |

**返回：** `PaymentCallback`

## AlipaySetting

| 属性 | 类型 | 说明 |
|------|------|------|
| useSandBox | `boolean` | 是否使用沙箱环境 |
| appId | `string` | 支付宝应用 ID |
| appPrivateKey | `string` | 应用私钥 |
| alipayPublicKey | `string` | 支付宝公钥 |
| signType | `string` | 签名类型 |
| charset | `string` | 编码 |
| serverUrl | `string` | 支付宝网关地址 |
