# k.mail

> Sending and receiving emails (IMAP), email parsing, Amazon SES and anti-spam detection

## Overview

`k.mail` provides email capabilities in site scripts, main sub-API:

| Member | Purpose |
|------|------|
| `smtp` | Send mail (site default channel or specified SMTP server) |
| `imap` | Pull messages from your IMAP inbox |
| `utility` | Parse original email and download attachments |
| `amazonses` | Send via AWS SES |
| `spamassassin` | SpamAssassin Spam Detection |
| `module` | Kooboo Mail module context (mailbox application) |
| `createMessage()` / `createSmtpServer()` | Construct mail and SMTP configuration objects |

::: tip Difference from `k.emailMarketing`
**`k.mail`** is for general sending/receiving and MIME processing; **`k.emailMarketing`** is for marketing email campaigns (separate article).
:::

## createMessage() / createSmtpServer()

| Method | Parameters | Returns | Description |
|--------|------------|---------|-------------|
| `createMessage()` | none | `MailMessage` | Create a mail message object. |
| `createSmtpServer()` | none | `SmtpServer` | Create an external SMTP server configuration object. |

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

### Common `MailMessage` Fields

| Field | Description |
|------|------|
| `from` / `to` / `cc` / `bcc` / `replyTo` | A single string; multiple addresses separated by commas (see below) |
| `subject` | Subject |
| `htmlBody` / `textBody` / `body` | Message body |
| `replyTo` | Reply-to address |
| `attachments` | Attachment collection |

| Method | Description |
|------|------|
| `addAttachment(urlOrPath)` | Add attachments by URL or path |
| `addAttachment(filename, bytes)` | Binary attachment |
| `attachObject(filename, obj)` | Attach an object |
| `toEml()` | Export EML string |

### Multiple Recipients

`to`, `cc`, `bcc`, `from`, `replyTo` will be resolved into multiple mailboxes on the server side. The same field can be written in a string, and multiple addresses are separated by commas. The RFC 5322 writing method with display name is also supported:

```ts
msg.to = "alice@example.com,bob@example.com"
msg.to = "Alice <alice@example.com>, Bob <bob@example.com>"
msg.cc = "carol@example.com; dave@example.com" // semicolons are normalized to commas
```

When using the site's default channel `k.mail.smtp.send(msg)`, the organization's email quota is deducted based on the parsed **number of recipients** (counted once for each valid address in `to`). External SMTP's `k.mail.smtp.send(server, msg)` is delivered by MailKit as To/Cc/Bcc in MIME.

## smtp.send()

| Method | Parameters | Returns | Description |
|--------|------------|---------|-------------|
| `smtp.send(message)` | `message: MailMessage | object` | `void` | Send mail through the site default mail channel. |
| `smtp.send(server, message)` | `server: SmtpServer`, `message: MailMessage | object` | `void` | Send mail through the specified external SMTP server. |

### Use the Site Default Mail Channel

Pass in `MailMessage` or equivalent plain object (must contain `from`, `to`):

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

Sending is limited by the organization's email quota; failures may throw errors such as `No enough email sending credits`.

### Use an external SMTP server

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

Pull mail from IMAP **Inbox** (require `ImapSetting` configured).

```ts
const setting = {
    emailAddress: "user@example.com",
    host: "imap.example.com",
    forceSSL: true,
    port: 993,
    password: "your-password"
}
```

| Method | Description |
|------|------|
| `collect(setting, start, count)` | Pull by UID range |
| `collectLatestMails(setting, count)` | Several recent letters |
| `get(setting, uid)` | Get order envelope by UID |
| `getRange(setting)` | Current mailbox UID range |

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

The returned items contain `uID`, `rawBody` (original RFC822 text).

## utility

| Method | Description |
|------|------|
| `parseSummary(rawBody)` | Parsed into a message digest object |
| `parseDetail(rawBody)` | Parse into details (including HTML, attachment list, etc.) |
| `downloadAttachment(rawBody, fileName)` | Extract attachment binary from original email |

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

## Amazons

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

You can also call `sendRaw(msg)` on `MailMessage` after `createEmail` to send the complete MIME (including complex attachments).

## spamassassin

Detect spam via the SpamAssassin service.

| Entrance | Description |
|------|------|
| `spamassassin.local` | Local default SpamAssassin |
| `spamassassin.connect(host, user, port)` | remote instance |

| Method | Description |
|------|------|
| `check(mail)` | Whether spam and score |
| `report(mail)` | Contains detailed report text |
| `ping()` | Service health check |

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

`k.mail.module` is only available in the **Kooboo Mail module** execution context (similar to `k.module`), providing `list()`, `config`, `baseUrl`, `localSqlite`, etc. Accessing it in the normal site API will throw an error.

## Related Docs

- [k.api](../api/)
- [k.payment](../payment/) — has nothing to do with email payment, business collection uses payment API
