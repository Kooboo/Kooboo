# settings

> E-commerce settings

## Overview

`k.commerce.settings` provides operations related to global e-commerce settings and is used to obtain configuration information such as currency, payment, logistics, and notifications.

## currencyCode

Get currency code.

```ts
k.api.get(() => {
    return k.commerce.settings.currencyCode
})
```

**Return:** `string` (like `"CNY"`)

## currencySymbol

Get currency symbol.

```ts
k.api.get(() => {
    return k.commerce.settings.currencySymbol
})
```

**Return:** `string` (like `"CN¥"`)

## weightUnit

Get the weight unit.

```ts
k.api.get(() => {
    return k.commerce.settings.weightUnit
})
```

**Return:** `string` (like `"kg"`)

## payments

Get a list of payment methods.

```ts
k.api.get(() => {
    return k.commerce.settings.payments
})
```

**Return:** `string[]`

## earnPoint

Get points to get configuration.

```ts
k.api.get(() => {
    return k.commerce.settings.earnPoint
})
```

**Return:** `EarnPointSettings`

## redeemPoint

Get points deduction rules.

```ts
k.api.get(() => {
    return k.commerce.settings.redeemPoint
})
```

**Return:** `RedeemPointSettings`

## orderExtensionFields

Get the order extension field configuration.

```ts
k.api.get(() => {
    return k.commerce.settings.orderExtensionFields
})
```

**Return:** `ExtensionField[]`

## productCustomFields

Get product custom field configuration.

```ts
k.api.get(() => {
    return k.commerce.settings.productCustomFields
})
```

**Return:** `CustomField[]`

## hideAttributes

Get whether the attribute is hidden.

```ts
k.api.get(() => {
    return k.commerce.settings.hideAttributes
})
```

**Return:** `boolean`

## hideVariants

Get whether to hide specifications.

```ts
k.api.get(() => {
    return k.commerce.settings.hideVariants
})
```

**Return:** `boolean`

## customMailServer

Get custom mail server configuration.

```ts
k.api.get(() => {
    return k.commerce.settings.customMailServer
})
```

**Return:** `MailServer`

## mailServerType

Get the mail server type.

```ts
k.api.get(() => {
    return k.commerce.settings.mailServerType
})
```

**Return:** `"kooboo" | "custom"`

## koobooEmailAddress

Get a Kooboo email address.

```ts
k.api.get(() => {
    return k.commerce.settings.koobooEmailAddress
})
```

**Return:** `string`

## enableEmailNotification

Gets whether email notifications are enabled.

```ts
k.api.get(() => {
    return k.commerce.settings.enableEmailNotification
})
```

**Return:** `boolean`

## emailNotifications

Get the email notification configuration list.

```ts
k.api.get(() => {
    return k.commerce.settings.emailNotifications
})
```

**Return:** `EmailNotification[]`

## enableWebhook

Gets whether webhooks are enabled.

```ts
k.api.get(() => {
    return k.commerce.settings.enableWebhook
})
```

**Return:** `boolean`

## webhooks

Get a list of webhook configurations.

```ts
k.api.get(() => {
    return k.commerce.settings.webhooks
})
```

**Return:** `Webhook[]`

## webhookSecret

Get the webhook key.

```ts
k.api.get(() => {
    return k.commerce.settings.webhookSecret
})
```

**Return:** `string`

## structure

### Settings property

`k.commerce.settings` returns the complete e-commerce settings object, including the following attributes:

| Property | Description | Type |
|------|------|------|
| currencyCode | currency code | `string` |
| currencySymbol | currency symbol | `string` |
| weightUnit | weight unit | `string` |
| earnPoint | Points acquisition configuration | `EarnPointSettings` |
| redeemPoint | Points deduction configuration | `RedeemPointSettings` |
| shippingCost | freight | `number` |
| payments | Payment method list | `string[]` |
| productCustomFields | Product custom fields | `CustomField[]` |
| categoryCustomFields | Classification custom fields | `CustomField[]` |
| orderExtensionFields | Order extension fields | `ExtensionField[]` |
| enableEmailNotification | Whether to enable email notifications | `boolean` |
| customMailServer | Custom mail server | `MailServer` |
| koobooEmailAddress | Kooboo email address | `string` |
| mailServerType | Mail server type | `string` |
| emailNotifications | Email notification list | `EmailNotification[]` |
| enableWebhook | Whether to enable webhooks | `boolean` |
| webhooks | Webhook configuration list | `Webhook[]` |
| webhookSecret | Webhook key | `string` |
| hideAttributes | Whether to hide attributes | `boolean` |
| hideVariants | Whether to hide specifications | `boolean` |
| productDigitalItemRequired | Is digital product required? | `boolean` |

### EarnPointSettings Property

| Property | Description | Type |
|------|------|------|
| activeDurationUnit | Points valid time unit | `string` |
| activeDuration | Points valid time value | `number` |
| orderEarnRules | Order earning rules | [OrderEarnPointsRule](#orderearnpointsrule-property) |
| loginEarnRules | Login to earn rules | [LoginEarnPointsRule](#loginearnpointsrule-property) |

### RedeemPointSettings Property

| Property | Description | Type |
|------|------|------|
| exchangeRate | Exchange ratio | `number` |
| orderRedeemRules | Order deduction rules | [OrderRedeemPointsRule](#orderredeempointsrule-property) |

### OrderEarnPointsRule Property

| Property | Description | Type |
|------|------|------|
| value | Earn points value | `number` |
| isPercent | Whether it is a percentage of the order amount | `boolean` |
| description | Description | `string` |
| condition | condition | [Condition](#condition-property) |

### LoginEarnPointsRule Property

| Property | Description | Type |
|------|------|------|
| value | Earn points value | `number` |
| durationUnit | Points valid time unit | `string` |
| description | Description | `string` |
| condition | condition | [Condition](#condition-property) |

### OrderRedeemPointsRule Property

| Property | Description | Type |
|------|------|------|
| value | Deduction point value | `number` |
| isPercent | Whether it is a percentage of the order amount | `boolean` |
| description | Description | `string` |
| condition | condition | [Condition](#condition-property) |

### Condition property

| Property | Description | Type |
|------|------|------|
| isAny | Any condition can be met | `boolean` |
| items | List of conditional items | [ConditionItem](#conditionitem-property) |

### ConditionItem Property

| Property | Description | Type |
|------|------|------|
| option | Condition options | `string` |
| Method | conditional method | `string` |
| value | condition value | `string` |

### ExtensionField Property

| Property | Description | Type |
|------|------|------|
| name | Field name | `string` |
| displayName | display name | `string` |
| Type | type | `string` |
| editable | Is it editable? | `boolean` |
| exportable | Is it exportable? | `boolean` |
| filterable | Is it filterable? | `boolean` |
| isSelection | Whether it is a selection type | `boolean` |

### Webhook properties

| Property | Description | Type |
|------|------|------|
| event | event type | `string` |
| url | Webhook URL | `string` |

### EmailNotification Property

| Property | Description | Type |
|------|------|------|
| ID | Notification ID | `string` |
| event | event type | `string` |
| subjectTemplate | Email subject template | `string` |
| bodyTemplate | Email content template | `string` |
| sendToCustomer | Whether to send to customers | `boolean` |
| sendToAddresses | Send address list | `string[]` |

### MailServer Properties

| Property | Description | Type |
|------|------|------|
| server | Server address | `string` |
| port | port | `number` |
| ssl | Whether to enable SSL | `boolean` |
| from | Sender address | `string` |
| userName | username | `string` |
| password | password | `string` |

## Related Docs

- [k.commerce](./commerce.md) - E-commerce module overview
- [order](./order.md) - Order Management
- [cart](./cart.md) - Shopping Cart
