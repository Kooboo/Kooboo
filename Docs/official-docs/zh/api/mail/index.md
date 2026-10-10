# k.mail

> 发信、收信（IMAP）、邮件解析、Amazon SES 与反垃圾检测

## 概述

`k.mail` 提供站点脚本中的邮件能力，主要子 API：

| 成员 | 用途 |
|------|------|
| `smtp` | 发送邮件（站点默认通道或指定 SMTP 服务器） |
| `imap` | 从 IMAP 收件箱拉取邮件 |
| `utility` | 解析原始邮件、下载附件 |
| `amazonses` | 通过 AWS SES 发信 |
| `spamassassin` | SpamAssassin 垃圾邮件检测 |
| `module` | Kooboo Mail 模块上下文（邮箱应用） |
| `createMessage()` / `createSmtpServer()` | 构造邮件与 SMTP 配置对象 |

::: tip 与 k.emailMarketing 的区别
**`k.mail`** 面向通用发信/收信与 MIME 处理；**`k.emailMarketing`** 为营销邮件活动（另文）。
:::

## createMessage() / createSmtpServer()

| 方法 | 参数 | 返回值 | 说明 |
|------|------|--------|------|
| `createMessage()` | 无 | `MailMessage` | 创建邮件消息对象。 |
| `createSmtpServer()` | 无 | `SmtpServer` | 创建外部 SMTP 服务器配置对象。 |

```ts
const msg = k.mail.createMessage()
msg.from = "sender@example.com"
msg.to = "user@example.com"
msg.subject = "Hello"
msg.htmlBody = "<p>HTML body</p>"
msg.textBody = "Plain text"

const server = k.mail.createSmtpServer()
server.host = "smtp.example.com"
server.port = 465
server.ssl = true
server.username = "sender@example.com"
server.password = "your-password"
```

### MailMessage 常用字段

| 字段 | 说明 |
|------|------|
| `from` / `to` / `cc` / `bcc` / `replyTo` | 单个字符串；多个地址用英文逗号分隔（见下） |
| `subject` | 主题 |
| `htmlBody` / `textBody` / `body` | 正文 |
| `replyTo` | 回复地址 |
| `attachments` | 附件集合 |

| 方法 | 说明 |
|------|------|
| `addAttachment(urlOrPath)` | 按 URL 或路径添加附件 |
| `addAttachment(filename, bytes)` | 二进制附件 |
| `attachObject(filename, obj)` | 附加对象 |
| `toEml()` | 导出 EML 字符串 |

### 多个收件人

`to`、`cc`、`bcc`、`from`、`replyTo` 在服务端会解析为多个邮箱。同一字段可写在一个字符串里，用**英文逗号**分隔多个地址，也支持带显示名的 RFC 5322 写法：

```ts
msg.to = "alice@example.com,bob@example.com"
msg.to = "Alice <alice@example.com>, Bob <bob@example.com>"
msg.cc = "carol@example.com; dave@example.com" // 「; 」会被规范为逗号
```

使用站点默认通道 `k.mail.smtp.send(msg)` 时，组织邮件额度按解析出的**收件人个数**扣减（`to` 中每个有效地址计一次）。外部 SMTP 的 `k.mail.smtp.send(server, msg)` 由 MailKit 按 MIME 中的 To/Cc/Bcc 一并投递。

## smtp.send()

| 方法 | 参数 | 返回值 | 说明 |
|------|------|--------|------|
| `smtp.send(message)` | `message: MailMessage | object` | `void` | 使用站点默认发信通道发送邮件。 |
| `smtp.send(server, message)` | `server: SmtpServer`, `message: MailMessage | object` | `void` | 使用指定外部 SMTP 服务器发送邮件。 |

### 使用站点默认发信通道

传入 `MailMessage` 或等价 plain 对象（须含 `from`、`to`）：

```ts
k.api.post(() => {
    k.mail.smtp.send({
        from: "noreply@your-domain.com",
        to: k.request.form.email,
        subject: "Notification",
        htmlBody: "<p>Thanks for signing up.</p>"
    })
    return { sent: true }
})
```

发信受组织邮件额度限制；失败时可能抛出 `No enough email sending credits` 等异常。

### 使用外部 SMTP 服务器

```ts
k.api.post(() => {
    const msg = k.mail.createMessage()
    msg.from = "sender@qq.com"
    msg.to = k.request.form.to
    msg.subject = "Test"
    msg.htmlBody = "<div>Content</div>"

    const server = k.mail.createSmtpServer()
    server.host = "smtp.qq.com"
    server.port = 465
    server.ssl = true
    server.username = "sender@qq.com"
    server.password = k.request.form.smtpPassword

    k.mail.smtp.send(server, msg)
    return { sent: true }
})
```

## imap

从 IMAP **收件箱**拉取邮件（须配置 `ImapSetting`）。

```ts
const setting = {
    emailAddress: "user@example.com",
    host: "imap.example.com",
    forceSSL: true,
    port: 993,
    password: "your-password"
}
```

| 方法 | 说明 |
|------|------|
| `collect(setting, start, count)` | 按 UID 区间拉取 |
| `collectLatestMails(setting, count)` | 最近若干封 |
| `get(setting, uid)` | 按 UID 取单封 |
| `getRange(setting)` | 当前邮箱 UID 范围 |

```ts
k.api.post(() => {
    const setting = {
        emailAddress: k.request.form.email,
        host: k.request.form.host,
        forceSSL: true,
        port: 993,
        password: k.request.form.password
    }

    const list = k.mail.imap.collectLatestMails(setting, 5)
    const first = list[0]
    const detail = first ? k.mail.utility.parseDetail(first.rawBody) : null

    return {
        count: list.length,
        subject: detail?.subject
    }
})
```

返回项含 `uID`、`rawBody`（原始 RFC822 文本）。

## utility

| 方法 | 说明 |
|------|------|
| `parseSummary(rawBody)` | 解析为邮件摘要对象 |
| `parseDetail(rawBody)` | 解析为详情（含 HTML、附件列表等） |
| `downloadAttachment(rawBody, fileName)` | 从原始邮件取出附件二进制 |

```ts
k.api.post(() => {
    const raw = k.request.body.raw
    const detail = k.mail.utility.parseDetail(raw)
    return {
        subject: detail.subject,
        from: detail.from,
        attachmentCount: detail.attachments?.length ?? 0
    }
})
```

## amazonses

```ts
const client = k.mail.amazonses.createEmail({
    accessKeyId: "...",
    secretAccessKey: "...",
    region: "EUCentral1"
})

const res = client.send({
    from: "me@example.com",
    to: ["user@example.com"],
    subject: "Subject",
    htmlBody: "<p>HTML</p>",
    textBody: "Text"
})
```

也可 `createEmail` 后对 `MailMessage` 调用 `sendRaw(msg)` 发送完整 MIME（含复杂附件）。

## spamassassin

通过 SpamAssassin 服务检测垃圾邮件。

| 入口 | 说明 |
|------|------|
| `spamassassin.local` | 本地默认 SpamAssassin |
| `spamassassin.connect(host, user, port)` | 远程实例 |

| 方法 | 说明 |
|------|------|
| `check(mail)` | 是否垃圾邮件及分数 |
| `report(mail)` | 含详细报告正文 |
| `ping()` | 服务健康检查 |

```ts
k.api.post(() => {
    const raw = k.request.body.raw
    const rsp = k.mail.spamassassin.local.check(raw)
    return {
        spam: rsp.spam,
        score: rsp.score,
        currentScore: rsp.currentScore,
        criticalScore: rsp.criticalScore
    }
})
```

## module

`k.mail.module` 仅在 **Kooboo Mail 模块**执行上下文中可用（类似 `k.module`），提供 `list()`、`config`、`baseUrl`、`localSqlite` 等。在普通站点 API 中访问会抛错。

## 相关文档

- [k.api](../api/)
- [k.payment](../payment/) — 与邮件支付无关，业务收款用支付 API
