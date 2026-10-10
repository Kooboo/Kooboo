# Send Email And SMS Scenario 邮件与短信发送场景

适用于登录验证码、注册验证码、找回密码、订单通知、表单通知等需要从 Kooboo 服务端发送邮件或短信的场景。

## 适用 API

| 场景 | 推荐 API | 配置位置 |
|------|----------|----------|
| 短信 | `k.utils.sms.aliSMS.send(templateCode, phone, bindings)` | Kooboo 后台 -> 站点设置 -> 服务集成 -> SMS |
| 邮件 | `k.mail.createSmtpServer()` + `k.mail.createMessage()` + `k.mail.smtp.send(server, msg)` | 内容数据 / Setting / ENV CodeBlock |

## 短信配置

短信服务使用站点后台配置。进入：

```text
Kooboo 后台 -> 站点设置 -> 服务集成 -> SMS
```

也可以直接打开后台配置页：

```text
/_Admin/system/config?SiteId=xxx
```

其中 `SiteId=xxx` 需要替换成当前站点的 SiteId。发送前必须先配置好阿里云短信的 accessId、accessSecret、signName、regionId 等信息，否则会抛出 `Ali SMS service not configured`。

短信发送示例：

```ts
k.utils.sms.aliSMS.send("SMS_489735268", "+8613812345678", {
    code: "123456"
})
```

参数说明：

| 参数 | 说明 |
|------|------|
| `templateCode` | 阿里云短信模板 Code，如 `SMS_489735268` |
| `phone` | 手机号，建议显式带国家码，如 `+8613812345678` |
| `bindings` | 模板变量对象，字段名要和阿里云模板变量一致 |

::: tip 提示
`k.utils.sms.aliSMS.send` 也支持 `send(templateCode, phone, "code", "123456")` 这种单变量写法。多变量模板建议统一使用对象写法。
:::

## 邮箱配置

邮件 SMTP 配置不依赖 SMS 后台配置。可以放在任意可存储数据的位置，例如：

- 内容管理中的 `Setting` 内容项，方便内容管理员修改
- 单独的 `Utils.ENV` / `ENV` CodeBlock
- 某个业务 Service 内部直接声明常量

推荐做法是用 `ENV` CodeBlock 包装配置，业务代码只读取 `ENV`：

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

如果项目还没有内容配置，也可以先直接写在 CodeBlock 中，后续再迁移到内容管理：

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

::: warning 注意
`EMAIL_PASSWORD` 通常是 SMTP 授权码，不一定是邮箱登录密码。不要把真实密钥写进示例或公开仓库。
:::

## 推荐 Service 封装

将发送逻辑放在 Service CodeBlock，例如 `Services.notification`。API 只负责读取参数并调用 Service。

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

## API 调用示例

API 层通过 `import` 调用 Service，不要从 `k.request` 或 `k.req` 上反射调用 CodeBlock。

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

校验验证码示例：

```ts
// API: verify-code

import { verifyCode } from "./Services.notification"

k.api.post((body) => {
    return verifyCode(body.accountType, body.account, body.code)
})
```

## 常见业务组合

登录 / 注册 / 找回密码场景通常还需要先检查账号状态：

| 场景 | 发送前检查 |
|------|------------|
| 登录验证码 | 账号必须已存在 |
| 注册验证码 | 手机号 / 邮箱不能已存在 |
| 找回密码 | 账号必须已存在 |
| 修改手机 / 邮箱 | 新手机号 / 邮箱不能被其他账号占用 |

这些账号检查应放在业务 Service 中，检查通过后再调用 `sendSmsCode` 或 `sendEmailCode`。

## 常见错误

- 邮件 SMTP 配置未设置 `server.ssl`，导致 465 端口发送失败
- 将邮箱登录密码当成 SMTP 授权码使用
- 阿里云短信模板变量名和 `bindings` 字段名不一致
- 手机号没有国家码，国际号码场景下应使用完整号码；国内号码建议显式使用 `+86`
- 验证码发送后没有做缓存过期和发送频率限制
- API 层直接写发送细节，导致登录、注册、找回密码等场景重复代码过多
- 使用 `k.req.services.xxx()` 或 `k.request.services.xxx()` 调用 Service；Kooboo 应使用 `import { xxx } from "./Services.xxx"`
