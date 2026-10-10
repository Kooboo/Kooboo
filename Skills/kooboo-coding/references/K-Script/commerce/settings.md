# Commerce Settings

All properties are accessed directly as `k.commerce.settings.XXX` and return the corresponding value.

## Currency and weight

| Property | Return value | Description |
|------|--------|------|
| `currencyCode` | `string` | Currency code, such as `"CNY"` |
| `currencySymbol` | `string` | Currency symbol, such as `"CN¥"` |
| `weightUnit` | `string` | Weight unit, such as `"kg"` |

## Payments and points

| Property | Return value | Description |
|------|--------|------|
| `payments` | `string[]` | List of available payment methods, such as `["alipay", "wechat"]` |
| `earnPoint` | `EarnPointSettings` | Point-earning rules |
| `redeemPoint` | `RedeemPointSettings` | Point-redemption rules |

## Orders and products

| Property | Return value | Description |
|------|--------|------|
| `orderExtensionFields` | `ExtensionField[]` | Order extension-field configuration |
| `productCustomFields` | `CustomField[]` | Product custom-field configuration |
| `categoryCustomFields` | `CustomField[]` | Category custom-field configuration |
| `hideAttributes` | `boolean` | Whether to hide attributes |
| `hideVariants` | `boolean` | Whether to hide variants |
| `productDigitalItemRequired` | `boolean` | Whether a product digital item is required |

## Email notifications

| Property | Return value | Description |
|------|--------|------|
| `mailServerType` | `"kooboo" \| "custom"` | Mail-server type |
| `customMailServer` | `MailServer` | Custom mail-server configuration |
| `koobooEmailAddress` | `string` | Kooboo email address |
| `enableEmailNotification` | `boolean` | Whether email notifications are enabled |
| `emailNotifications` | `EmailNotification[]` | Email-notification configuration list |

## Webhook

| Property | Return value | Description |
|------|--------|------|
| `enableWebhook` | `boolean` | Whether Webhook is enabled |
| `webhooks` | `Webhook[]` | Webhook configuration list |
| `webhookSecret` | `string` | Webhook secret |

## Type definitions

```ts
interface EarnPointSettings {
    activeDurationUnit: "Year" | "Month" | "Week" | "Day";
    activeDuration: number;
    orderEarnRules: OrderEarnPointsRule[];
    loginEarnRules: LoginEarnPointsRule[];
}

interface RedeemPointSettings {
    exchangeRate: number;
    orderRedeemRules: OrderRedeemPointsRule[];
}

interface ExtensionField {
    name: string;
    displayName: string;
    type: string;
    isSummaryField: boolean;
    width: number;
    editable: boolean;
    exportable: boolean;
    exportByCode: boolean;
    exportCode: string;
    filterable: boolean;
    isSelection: boolean;
}

interface OrderEarnPointsRule {
    value: number;
    isPercent: boolean;
    description: string;
    condition: Condition;
}

interface LoginEarnPointsRule {
    value: number;
    durationUnit: "Year" | "Month" | "Week" | "Day";
    description: string;
    condition: Condition;
}

interface OrderRedeemPointsRule {
    value: number;
    isPercent: boolean;
    description: string;
    condition: Condition;
}

interface Webhook {
    event: string;
    url: string;
}

interface EmailNotification {
    id: string;
    event: string;
    subjectTemplate: string;
    bodyTemplate: string;
    sendToCustomer: boolean;
    sendToAddresses: string[];
}

interface MailServer {
    server: string;
    port: number;
    ssl: boolean;
    from: string;
    userName: string;
    password: string;
}

interface Condition {
    isAny: boolean;
    items: ConditionItem[];
}

interface ConditionItem {
    option: string;
    method: string;
    value: string;
}
```
