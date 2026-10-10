# k.utils.sms

> 短信发送（多提供商）

## 概述

`k.utils.sms` 按站点**控制台 SMS 设置**选择提供商。须在对应平台配置模板与签名。

| 属性 | 说明 |
|------|------|
| `aliSMS` | 阿里云短信 |
| `tencent` | 腾讯云短信 |
| `chinaMobile` | 中国移动 |

## aliSMS

| 方法 | 说明 |
|------|------|
| `send(templateCode, phone, key, value)` | 单变量模板 |
| `send(templateCode, phone, bindings)` | 多变量字典 |
| `sendInternational(...)` | 国际短信 |

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `templateCode` | `string` | 是 | 阿里云短信模板 Code |
| `phone` | `string` | 是 | 收件手机号；国内号码可不带 `+86` |
| `key` | `string` | 是 | 单变量模板参数名 |
| `value` | `string` | 是 | 单变量模板参数值 |
| `bindings` | `Record<string, any>` | 是 | 多变量模板参数对象 |

**返回：** `boolean`，发送成功返回 `true`；配置缺失或供应商返回错误时抛出异常。

阿里云短信会读取站点控制台中的 Ali SMS 设置；国内手机号可不带 `+86`，运行时会自动补齐。

```ts
const ok = k.utils.sms.aliSMS.send(
    "SMS_123456789",
    "13800138000",
    "code",
    "493821"
)

return { sent: ok }
```

多变量模板传对象：

```ts
const ok = k.utils.sms.aliSMS.send("SMS_ORDER_NOTICE", "13800138000", {
    orderNo: "SO20260702001",
    amount: "129.00"
})
```

国际短信使用已配置的国际签名：

```ts
k.utils.sms.aliSMS.sendInternational(
    "SMS_GLOBAL_CODE",
    "+14155550100",
    "code",
    "493821"
)
```

## tencent

`send(templateId, phone, parameters)` — `parameters` 为模板参数对象。

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `templateId` | `string` | 是 | 腾讯云短信模板 Id |
| `phone` | `string` | 是 | 收件手机号；国内号码可不带 `+86` |
| `parameters` | `Record<string, any>` | 否 | 模板参数对象，值会按对象枚举顺序传给腾讯云 |

**返回：** `boolean`，供应商返回 `Ok` 时为 `true`。

```ts
const ok = k.utils.sms.tencent.send("1850001", "13800138000", {
    code: "493821",
    minutes: "5"
})

return { sent: ok }
```

## chinaMobile

`send(phone, content)` 或 `send(phones[], content)`。

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `phone` | `string` | 是 | 单个手机号 |
| `phones` | `string[]` | 是 | 多个手机号 |
| `content` | `string` | 是 | 短信正文 |

**返回：** `boolean`，发送成功返回 `true`；供应商失败时抛出异常。

```ts
k.utils.sms.chinaMobile.send("13800138000", "Your verification code is 493821.")
```

批量发送：

```ts
k.utils.sms.chinaMobile.send(
    ["13800138000", "13900139000"],
    "Your order has been shipped."
)
```

## 相关文档

- [k.utils 概述](./index.md)
