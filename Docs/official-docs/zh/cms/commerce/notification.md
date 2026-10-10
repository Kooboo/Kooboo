# 通知

> 菜单：**电商 → 通知**  
> 深链：`/_Admin/commerce/notification?SiteId={站点GUID}`

配置电商 **邮件通知** 与 **Webhook**：在订单创建、支付成功等事件触发时发信或回调外部 URL。设置保存在 Commerce `settings`。页底 **保存** 写入站点（`saveSettings`），保存后会刷新 Monaco 脚本缓存（邮件模板编辑器 IntelliSense）。

## 如何打开

1. 在 [编辑菜单](../navigation.md#权限与编辑菜单) 中勾选 **电商 → 通知**（及父级 **电商**）。
2. 左侧 **电商 → 通知**。

<DocImage src="/cms/commerce/notification-overview.png" alt="电商通知：邮件与 Webhook 两块配置区" width="1120" />

## 邮件通知

### 开关与发信方式

| 项 | 说明 |
|----|------|
| **启用邮件通知** | `enableEmailNotification` 总开关 |
| **邮件服务器** | **Kooboo**：使用平台邮件地址；**自定义**：自建 SMTP |
| **Kooboo 发件地址** | 下拉选择站点邮件地址（排除通配符地址） |
| **自定义 SMTP** | 点 **设置服务器信息** 打开 [SMTP 设置弹窗](#smtp-设置弹窗)，保存后显示服务器/端口/账号摘要 |
| **事件** | 已配置的邮件规则列表，见下 |
| **日志** | 卡片右上角 **日志** → [邮件日志弹窗](#邮件日志弹窗) |

### 邮件事件规则

列表每项显示：事件说明、事件标签（`display`）、**设置**（编辑）、**删除**。

点击圆形 **+** 或行内 **设置** 打开 [邮件通知弹窗](#邮件通知弹窗)：

<DocImage src="/cms/commerce/notification-email-dialog.png" alt="邮件通知弹窗：事件、收件人与模板预览/代码" width="460" />

| 字段 | 说明 |
|------|------|
| **事件** | 下拉，选项来自 `getEmailEvents()`（如订单相关事件） |
| **发送至** | **顾客**（`sendToCustomer`）；**合作伙伴** 可填多个邮箱（`SelectInput`） |
| **预览 / 代码** | 选事件后出现：**预览** 调用 `emailPreview` 渲染主题与正文；**代码** 编辑 `subjectTemplate`、`bodyTemplate`（Monaco HTML + KScript） |
| 切换事件 | 自动带入该事件默认主题/正文模板 |

确认后写入 `settings.emailNotifications` 列表，须再点页面底部 **保存** 才生效。

### SMTP 设置弹窗

仅 **自定义** 邮件服务器时出现 **设置服务器信息**。

<DocImage src="/cms/commerce/notification-smtp-dialog.png" alt="SMTP 设置：服务器、端口、SSL、账号与发件人" width="460" />

| 字段 | 说明 |
|------|------|
| **服务器** | SMTP 主机 |
| **端口** | 数字 |
| **SSL** | 开关 |
| **账号 / 密码** | 登录凭据 |
| **发件地址** | `from` |

保存后更新 `settings.customMailServer`（仍须页面底部 **保存**）。

### 邮件日志弹窗

| 列 | 说明 |
|----|------|
| **邮箱** | 收件地址 |
| **事件** | 触发事件名 |
| **内容** | 主题，可点开展示详情 |
| **时间** | 发送时间 |
| **状态** | 成功/失败；失败可悬停看错误 |

<DocImage src="/cms/commerce/notification-email-log-dialog.png" alt="邮件日志弹窗：发送记录与分页" width="1000" />

## Webhook

### 开关与配置

| 项 | 说明 |
|----|------|
| **启用 Webhook** | `enableWebhook` |
| **Webhooks** | 规则列表；标签旁 **?** 展示 `webhookSecret` 与签名校验说明：`SHA256Hash(request.body+secret) == request.header["X-Kooboo-Hmac-SHA256"]` |
| **日志** | 卡片右上角 **日志** → [Webhook 日志弹窗](#webhook-日志弹窗) |

点击 **+** 或行内 **设置** 打开 [Webhook 弹窗](#webhook-弹窗)：

<DocImage src="/cms/commerce/notification-webhook-dialog.png" alt="Webhook 弹窗：事件与 URL" width="460" />

| 字段 | 说明 |
|------|------|
| **事件** | `getWebhookEvents()` 中的事件 |
| **URL** | 回调地址，须通过 URL 校验 |

### Webhook 日志弹窗

| 列 | 说明 |
|----|------|
| **URL** | 回调 URL（`row.address`） |
| **事件** | 触发事件名（`row.event`，与 Webhook 规则中配置的事件一致） |
| **内容** | 请求体摘要，可点击查看完整内容（`JsonTree`） |
| **时间** | 调用时间 |
| **状态** | 成功/失败；失败可悬停查看错误信息 |

<DocImage src="/cms/commerce/notification-webhook-log-dialog.png" alt="Webhook 日志弹窗" width="1000" />

## 相关

- [订单](./orders.md) — 常见触发源（下单、付款等）  
- [设置](./settings.md) — 其它 Commerce 全局项  
- [k.mail](../../api/mail/index.md) — 站点邮件能力；Kooboo 发件依赖邮件地址配置
