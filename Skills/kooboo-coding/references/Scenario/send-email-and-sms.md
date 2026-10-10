# Send Email and SMS Scenario

Use this scenario when the Kooboo server must send email or SMS for login codes, registration codes, password recovery, order notifications, form notifications, and similar flows.

## Applicable APIs

| Scenario | Recommended API | Configuration |
|------|----------|----------|
| SMS | `k.utils.sms.aliSMS.send(templateCode, phone, bindings)` | Kooboo back office -> Site Settings -> Service Integrations -> SMS |
| Email | `k.mail.createSmtpServer()` + `k.mail.createMessage()` + `k.mail.smtp.send(server, msg)` | Content data / Setting / ENV CodeBlock |

## SMS Configuration

The SMS service uses the site back-office configuration. Open:

```text
Kooboo 后台 -> 站点设置 -> 服务集成 -> SMS
```

You can also open the configuration page directly:

```text
/_Admin/system/config?SiteId=xxx
```

Replace `SiteId=xxx` with the current site's SiteId. Before sending, configure the Alibaba Cloud SMS accessId, accessSecret, signName, regionId, and other required values; otherwise the call throws `Ali SMS service not configured`.

SMS sending example:

```ts
k.utils.sms.aliSMS.send("SMS_489735268", "+8613812345678", {
    code: "123456"
})
```

Parameter reference:

| Parameter | Description |
|------|------|
| `templateCode` | Alibaba Cloud SMS template code, such as `SMS_489735268` |
| `phone` | Phone number; include the country code explicitly, such as `+8613812345678` |
| `bindings` | Template-variable object whose field names must match the Alibaba Cloud template variables |

::: tip
`k.utils.sms.aliSMS.send` also supports the single-variable form `send(templateCode, phone, "code", "123456")`. For templates with multiple variables, use the object form consistently.
:::

## Email Configuration

Email SMTP configuration is independent of the SMS back-office configuration. Store it in any suitable persistent location, for example:

- A `Setting` content item for content administrators to edit
- A dedicated `Utils.ENV` / `ENV` CodeBlock
- Constants declared directly inside a business Service

The recommended approach is to wrap the configuration in an `ENV` CodeBlock and have business code read only `ENV`:

```ts
// Utils.ENV (CodeBlock)

const setting = k.content.Setting

const ENV = {
    MOCK_SEND: false,
    VERIFY_CODE_ONE_TIME: true,
    EMAIL_HOST: setting.host,
    EMAIL_PORT: +(setting.port || 465),
    EMAIL_SSL: true,
    SENDER_EMAIL: setting.sendEmail,
    EMAIL_USERNAME: setting.senderName || setting.sendEmail,
    EMAIL_PASSWORD: setting.password
}

export default ENV
```

If the project has no content configuration yet, define the values directly in the CodeBlock first and migrate them to content management later:

```ts
// Utils.ENV (CodeBlock)

const ENV = {
    MOCK_SEND: false,
    VERIFY_CODE_ONE_TIME: true,
    EMAIL_HOST: "smtp.qq.com",
    EMAIL_PORT: 465,
    EMAIL_SSL: true,
    SENDER_EMAIL: "noreply@example.com",
    EMAIL_USERNAME: "noreply@example.com",
    EMAIL_PASSWORD: "smtp_authorization_code"
}

export default ENV
```

::: warning
`EMAIL_PASSWORD` is usually an SMTP authorization code, not necessarily the mailbox login password. Never put real secrets in examples or public repositories.
:::

## Recommended Service Wrapper

Put sending logic in a Service CodeBlock such as `Services.notification`. The API should only read parameters and call the Service.

```ts
// Services.notification (CodeBlock)

import ENV from "./Utils.ENV"

const VERIFY_CODE_TTL = 300
const VERIFY_CODE_INTERVAL = 60

function isPhone(phone: string): boolean {
    return /^1[3-9]\d{9}$/.test(phone)
}

function isEmail(email: string): boolean {
    return /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email)
}

function verifyCodeKey(type: "phone" | "email", account: string): string {
    return type === "email"
        ? `verify_code_email_${account}`
        : `verify_code_phone_${account}`
}

function throttleKey(type: "phone" | "email", account: string): string {
    return `${type}_verify_time_${account}`
}

function createVerifyCode(): string {
    return Math.floor(100000 + Math.random() * 900000).toString()
}

function assertSendInterval(type: "phone" | "email", account: string): void {
    const lastSent = k.cache.get(throttleKey(type, account))
    if (lastSent && Date.now() - lastSent < VERIFY_CODE_INTERVAL * 1000) {
        throw new Error("验证码发送过于频繁，请稍后再试")
    }
}

function saveVerifyCode(type: "phone" | "email", account: string, code: string): void {
    k.cache.set(verifyCodeKey(type, account), code, VERIFY_CODE_TTL)
    k.cache.set(throttleKey(type, account), Date.now(), VERIFY_CODE_INTERVAL)
}

export function sendSmsCode(phone: string, templateCode: string) {
    const normalizedPhone = (phone ?? "").trim()
    if (!normalizedPhone) throw new Error("请输入手机号")
    if (!isPhone(normalizedPhone)) throw new Error("请输入正确的手机号")

    assertSendInterval("phone", normalizedPhone)

    const code = createVerifyCode()
    saveVerifyCode("phone", normalizedPhone, code)

    if (!ENV.MOCK_SEND) {
        k.utils.sms.aliSMS.send(templateCode, `+86${normalizedPhone}`, {
            code
        })
    }

    return {
        message: ENV.MOCK_SEND
            ? `验证码已生成：${code}`
            : "验证码已发送到您的手机号",
        phone: normalizedPhone
    }
}

export function sendEmailCode(email: string) {
    const normalizedEmail = (email ?? "").trim().toLowerCase()
    if (!normalizedEmail) throw new Error("请输入邮箱地址")
    if (!isEmail(normalizedEmail)) throw new Error("请输入正确的邮箱地址")

    assertSendInterval("email", normalizedEmail)

    const code = createVerifyCode()
    saveVerifyCode("email", normalizedEmail, code)

    if (!ENV.MOCK_SEND) {
        const server = k.mail.createSmtpServer()
        server.host = ENV.EMAIL_HOST
        server.port = ENV.EMAIL_PORT
        server.ssl = ENV.EMAIL_SSL
        server.username = ENV.EMAIL_USERNAME
        server.password = ENV.EMAIL_PASSWORD

        const msg = k.mail.createMessage()
        msg.from = ENV.SENDER_EMAIL
        msg.to = normalizedEmail
        msg.subject = "邮箱验证码"
        msg.htmlBody = `
            <div style="font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;">
                <h2>邮箱验证码</h2>
                <p>您好，您的验证码为：</p>
                <p style="font-size: 28px; font-weight: bold; letter-spacing: 6px;">${code}</p>
                <p>验证码有效期为 5 分钟，请及时使用。</p>
                <p>如果这不是您的操作，请忽略此邮件。</p>
            </div>
        `

        k.mail.smtp.send(server, msg)
    }

    return {
        message: ENV.MOCK_SEND
            ? `验证码已生成：${code}`
            : "验证码已发送到您的邮箱",
        email: normalizedEmail
    }
}

export function verifyCode(type: "phone" | "email", account: string, code: string) {
    const normalizedAccount =
        type === "email" ? (account ?? "").trim().toLowerCase() : (account ?? "").trim()
    const normalizedCode = (code ?? "").trim()

    if (!normalizedAccount) throw new Error("账号不能为空")
    if (!normalizedCode) throw new Error("请输入验证码")

    const key = verifyCodeKey(type, normalizedAccount)
    const stored = k.cache.get(key)
    if (!stored || stored !== normalizedCode) {
        throw new Error("验证码错误或已过期")
    }

    if (ENV.VERIFY_CODE_ONE_TIME) {
        k.cache.remove(key)
    }

    return true
}
```

## API Invocation Example

The API layer calls the Service through `import`; do not reflectively invoke a CodeBlock through `k.request` or `k.req`.

```ts
// API: send-verification-code

import { sendEmailCode, sendSmsCode } from "./Services.notification"

k.api.post((body) => {
    const accountType = body.accountType
    const account = body.account

    if (accountType === "phone") {
        return sendSmsCode(account, "SMS_489735268")
    }

    if (accountType === "email") {
        return sendEmailCode(account)
    }

    throw new Error("发送类型错误")
})
```

Verification-code example:

```ts
// API: verify-code

import { verifyCode } from "./Services.notification"

k.api.post((body) => {
    return verifyCode(body.accountType, body.account, body.code)
})
```

## Common Business Combinations

Login, registration, and password-recovery flows usually need an account-state check before sending:

| Scenario | Check before sending |
|------|------------|
| Login code | The account must already exist |
| Registration code | The phone number / email must not already exist |
| Password recovery | The account must already exist |
| Change phone / email | The new phone number / email must not belong to another account |

Put these account checks in the business Service and call `sendSmsCode` or `sendEmailCode` only after the checks pass.

## Common Errors

- Email SMTP configuration omits `server.ssl`, causing sends on port 465 to fail
- The mailbox login password is used as the SMTP authorization code
- Alibaba Cloud template variable names do not match the `bindings` fields
- The phone number has no country code; international numbers need the full number, and domestic numbers should explicitly use `+86`
- No cache expiration or send-frequency limit is applied after issuing a code
- Sending details are written directly in the API layer, duplicating code across login, registration, and password-recovery flows
- The Service is called through `k.req.services.xxx()` or `k.request.services.xxx()`; Kooboo requires `import { xxx } from "./Services.xxx"`
