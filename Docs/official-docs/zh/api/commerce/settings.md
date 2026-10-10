# settings

> 电商设置

## 概述

`k.commerce.settings` 提供电商全局设置相关操作，用于获取货币、支付、物流、通知等配置信息。

## currencyCode

获取货币代码。

```ts
k.api.get(() => {
    return k.commerce.settings.currencyCode
})
```

**返回：** `string`（如 `"CNY"`）

## currencySymbol

获取货币符号。

```ts
k.api.get(() => {
    return k.commerce.settings.currencySymbol
})
```

**返回：** `string`（如 `"CN¥"`）

## weightUnit

获取重量单位。

```ts
k.api.get(() => {
    return k.commerce.settings.weightUnit
})
```

**返回：** `string`（如 `"kg"`）

## payments

获取支付方式列表。

```ts
k.api.get(() => {
    return k.commerce.settings.payments
})
```

**返回：** `string[]`

## earnPoint

获取积分获取配置。

```ts
k.api.get(() => {
    return k.commerce.settings.earnPoint
})
```

**返回：** `EarnPointSettings`

## redeemPoint

获取积分抵扣规则。

```ts
k.api.get(() => {
    return k.commerce.settings.redeemPoint
})
```

**返回：** `RedeemPointSettings`

## orderExtensionFields

获取订单扩展字段配置。

```ts
k.api.get(() => {
    return k.commerce.settings.orderExtensionFields
})
```

**返回：** `ExtensionField[]`

## productCustomFields

获取产品自定义字段配置。

```ts
k.api.get(() => {
    return k.commerce.settings.productCustomFields
})
```

**返回：** `CustomField[]`

## hideAttributes

获取是否隐藏属性。

```ts
k.api.get(() => {
    return k.commerce.settings.hideAttributes
})
```

**返回：** `boolean`

## hideVariants

获取是否隐藏规格。

```ts
k.api.get(() => {
    return k.commerce.settings.hideVariants
})
```

**返回：** `boolean`

## customMailServer

获取自定义邮件服务器配置。

```ts
k.api.get(() => {
    return k.commerce.settings.customMailServer
})
```

**返回：** `MailServer`

## mailServerType

获取邮件服务器类型。

```ts
k.api.get(() => {
    return k.commerce.settings.mailServerType
})
```

**返回：** `"kooboo" | "custom"`

## koobooEmailAddress

获取 Kooboo 邮箱地址。

```ts
k.api.get(() => {
    return k.commerce.settings.koobooEmailAddress
})
```

**返回：** `string`

## enableEmailNotification

获取是否启用邮件通知。

```ts
k.api.get(() => {
    return k.commerce.settings.enableEmailNotification
})
```

**返回：** `boolean`

## emailNotifications

获取邮件通知配置列表。

```ts
k.api.get(() => {
    return k.commerce.settings.emailNotifications
})
```

**返回：** `EmailNotification[]`

## enableWebhook

获取是否启用 Webhook。

```ts
k.api.get(() => {
    return k.commerce.settings.enableWebhook
})
```

**返回：** `boolean`

## webhooks

获取 Webhook 配置列表。

```ts
k.api.get(() => {
    return k.commerce.settings.webhooks
})
```

**返回：** `Webhook[]`

## webhookSecret

获取 Webhook 密钥。

```ts
k.api.get(() => {
    return k.commerce.settings.webhookSecret
})
```

**返回：** `string`

## 结构

### Settings 属性

`k.commerce.settings` 返回完整的电商设置对象，包含以下属性：

| 属性 | 说明 | 类型 |
|------|------|------|
| currencyCode | 货币代码 | `string` |
| currencySymbol | 货币符号 | `string` |
| weightUnit | 重量单位 | `string` |
| earnPoint | 积分获取配置 | `EarnPointSettings` |
| redeemPoint | 积分抵扣配置 | `RedeemPointSettings` |
| shippingCost | 运费 | `number` |
| payments | 支付方式列表 | `string[]` |
| productCustomFields | 产品自定义字段 | `CustomField[]` |
| categoryCustomFields | 分类自定义字段 | `CustomField[]` |
| orderExtensionFields | 订单扩展字段 | `ExtensionField[]` |
| enableEmailNotification | 是否启用邮件通知 | `boolean` |
| customMailServer | 自定义邮件服务器 | `MailServer` |
| koobooEmailAddress | Kooboo 邮箱地址 | `string` |
| mailServerType | 邮件服务器类型 | `string` |
| emailNotifications | 邮件通知列表 | `EmailNotification[]` |
| enableWebhook | 是否启用 Webhook | `boolean` |
| webhooks | Webhook 配置列表 | `Webhook[]` |
| webhookSecret | Webhook 密钥 | `string` |
| hideAttributes | 是否隐藏属性 | `boolean` |
| hideVariants | 是否隐藏规格 | `boolean` |
| productDigitalItemRequired | 数字产品是否必填 | `boolean` |

### EarnPointSettings 属性

| 属性 | 说明 | 类型 |
|------|------|------|
| activeDurationUnit | 积分有效时长单位 | `string` |
| activeDuration | 积分有效时长数值 | `number` |
| orderEarnRules | 订单赚取规则 | [`OrderEarnPointsRule[]`](#ordarnearnpointsrule-属性) |
| loginEarnRules | 登录赚取规则 | [`LoginEarnPointsRule[]`](#loginearnpointsrule-属性) |

### RedeemPointSettings 属性

| 属性 | 说明 | 类型 |
|------|------|------|
| exchangeRate | 兑换比例 | `number` |
| orderRedeemRules | 订单抵扣规则 | [`OrderRedeemPointsRule[]`](#orderredeempointsrule-属性) |

### OrderEarnPointsRule 属性

| 属性 | 说明 | 类型 |
|------|------|------|
| value | 赚取积分值 | `number` |
| isPercent | 是否为订单金额百分比 | `boolean` |
| description | 描述 | `string` |
| condition | 条件 | [`Condition`](#condition-属性) |

### LoginEarnPointsRule 属性

| 属性 | 说明 | 类型 |
|------|------|------|
| value | 赚取积分值 | `number` |
| durationUnit | 积分有效时长单位 | `string` |
| description | 描述 | `string` |
| condition | 条件 | [`Condition`](#condition-属性) |

### OrderRedeemPointsRule 属性

| 属性 | 说明 | 类型 |
|------|------|------|
| value | 抵扣积分值 | `number` |
| isPercent | 是否为订单金额百分比 | `boolean` |
| description | 描述 | `string` |
| condition | 条件 | [`Condition`](#condition-属性) |

### Condition 属性

| 属性 | 说明 | 类型 |
|------|------|------|
| isAny | 任一条件满足即可 | `boolean` |
| items | 条件项列表 | [`ConditionItem[]`](#conditionitem-属性) |

### ConditionItem 属性

| 属性 | 说明 | 类型 |
|------|------|------|
| option | 条件选项 | `string` |
| method | 条件方法 | `string` |
| value | 条件值 | `string` |

### ExtensionField 属性

| 属性 | 说明 | 类型 |
|------|------|------|
| name | 字段名 | `string` |
| displayName | 显示名称 | `string` |
| type | 类型 | `string` |
| editable | 是否可编辑 | `boolean` |
| exportable | 是否可导出 | `boolean` |
| filterable | 是否可筛选 | `boolean` |
| isSelection | 是否为选择类型 | `boolean` |

### Webhook 属性

| 属性 | 说明 | 类型 |
|------|------|------|
| event | 事件类型 | `string` |
| url | Webhook URL | `string` |

### EmailNotification 属性

| 属性 | 说明 | 类型 |
|------|------|------|
| id | 通知ID | `string` |
| event | 事件类型 | `string` |
| subjectTemplate | 邮件主题模板 | `string` |
| bodyTemplate | 邮件内容模板 | `string` |
| sendToCustomer | 是否发送给客户 | `boolean` |
| sendToAddresses | 发送地址列表 | `string[]` |

### MailServer 属性

| 属性 | 说明 | 类型 |
|------|------|------|
| server | 服务器地址 | `string` |
| port | 端口 | `number` |
| ssl | 是否启用 SSL | `boolean` |
| from | 发件人地址 | `string` |
| userName | 用户名 | `string` |
| password | 密码 | `string` |

## 相关文档

- [k.commerce](./commerce.md) - 电商模块概述
- [order](./order.md) - 订单管理
- [cart](./cart.md) - 购物车
