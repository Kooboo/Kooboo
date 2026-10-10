# Commerce Settings

所有属性均为 `k.commerce.settings.XXX` 直接访问，返回对应值。

## 货币与重量

| 属性 | 返回值 | 说明 |
|------|--------|------|
| `currencyCode` | `string` | 货币代码，如 `"CNY"` |
| `currencySymbol` | `string` | 货币符号，如 `"CN¥"` |
| `weightUnit` | `string` | 重量单位，如 `"kg"` |

## 支付与积分

| 属性 | 返回值 | 说明 |
|------|--------|------|
| `payments` | `string[]` | 可用支付方式列表，如 `["alipay", "wechat"]` |
| `earnPoint` | `EarnPointSettings` | 积分获取规则 |
| `redeemPoint` | `RedeemPointSettings` | 积分抵扣规则 |

## 订单与产品

| 属性 | 返回值 | 说明 |
|------|--------|------|
| `orderExtensionFields` | `ExtensionField[]` | 订单扩展字段配置 |
| `productCustomFields` | `CustomField[]` | 产品自定义字段配置 |
| `categoryCustomFields` | `CustomField[]` | 分类自定义字段配置 |
| `hideAttributes` | `boolean` | 是否隐藏属性 |
| `hideVariants` | `boolean` | 是否隐藏规格 |
| `productDigitalItemRequired` | `boolean` | 是否要求商品必须有数字商品项 |

## 邮件通知

| 属性 | 返回值 | 说明 |
|------|--------|------|
| `mailServerType` | `"kooboo" \| "custom"` | 邮件服务器类型 |
| `customMailServer` | `MailServer` | 自定义邮件服务器配置 |
| `koobooEmailAddress` | `string` | Kooboo 邮箱地址 |
| `enableEmailNotification` | `boolean` | 是否启用邮件通知 |
| `emailNotifications` | `EmailNotification[]` | 邮件通知配置列表 |

## Webhook

| 属性 | 返回值 | 说明 |
|------|--------|------|
| `enableWebhook` | `boolean` | 是否启用 Webhook |
| `webhooks` | `Webhook[]` | Webhook 配置列表 |
| `webhookSecret` | `string` | Webhook 密钥 |

## 类型定义

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
