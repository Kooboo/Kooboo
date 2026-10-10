# k.utils.sms

> SMS sending (multi-provider)

## Overview

`k.utils.sms` Select provider by site **Console SMS Settings**. Templates and signatures must be configured on the corresponding platform.

| Property | Description |
|------|------|
| `aliSMS` | Alibaba Cloud SMS |
| `tencent` | Tencent Cloud SMS |
| `chinaMobile` | China Mobile |

## aliSMS

| Method | Description |
|------|------|
| `send(templateCode, phone, key, value)` | Single variable template |
| `send(templateCode, phone, bindings)` | multivariable dictionary |
| `sendInternational(...)` | International SMS |

| Parameter | Type | Required | Description |
|------|------|------|------|
| `templateCode` | `string` | Yes | Alibaba Cloud SMS template code |
| `phone` | `string` | Yes | Recipient phone number; China domestic numbers can omit `+86` |
| `key` | `string` | Yes | Single template variable name |
| `value` | `string` | Yes | Single template variable value |
| `bindings` | `Record<string, any>` | Yes | Multi-variable template parameter object |

**Returns:** `boolean`; returns `true` on success, and throws when settings are missing or the provider reports an error.

Alibaba Cloud SMS reads the Ali SMS settings from the site console. Domestic China numbers can omit `+86`; the runtime adds it automatically.

```ts
const ok = k.utils.sms.aliSMS.send(
    "SMS_123456789",
    "13800138000",
    "code",
    "493821"
)

return { sent: ok }
```

Pass an object for templates with multiple variables:

```ts
const ok = k.utils.sms.aliSMS.send("SMS_ORDER_NOTICE", "13800138000", {
    orderNo: "SO20260702001",
    amount: "129.00"
})
```

Use the configured international signature for international SMS:

```ts
k.utils.sms.aliSMS.sendInternational(
    "SMS_GLOBAL_CODE",
    "+14155550100",
    "code",
    "493821"
)
```

## tencent

`send(templateId, phone, parameters)` — `parameters` is the template parameter object.

| Parameter | Type | Required | Description |
|------|------|------|------|
| `templateId` | `string` | Yes | Tencent Cloud SMS template Id |
| `phone` | `string` | Yes | Recipient phone number; China domestic numbers can omit `+86` |
| `parameters` | `Record<string, any>` | No | Template parameter object; values are passed in object enumeration order |

**Returns:** `boolean`; returns `true` when the provider status is `Ok`.

```ts
const ok = k.utils.sms.tencent.send("1850001", "13800138000", {
    code: "493821",
    minutes: "5"
})

return { sent: ok }
```

## chinaMobile

`send(phone, content)` or `send(phones[], content)`.

| Parameter | Type | Required | Description |
|------|------|------|------|
| `phone` | `string` | Yes | Single phone number |
| `phones` | `string[]` | Yes | Multiple phone numbers |
| `content` | `string` | Yes | SMS message body |

**Returns:** `boolean`; returns `true` on success, and throws when the provider fails.

```ts
k.utils.sms.chinaMobile.send("13800138000", "Your verification code is 493821.")
```

Send to multiple numbers:

```ts
k.utils.sms.chinaMobile.send(
    ["13800138000", "13900139000"],
    "Your order has been shipped."
)
```

## Related Docs

- [k.utils Overview](./index.md)
