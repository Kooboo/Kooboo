---
name: kooboo-coding-k-script
description: Use when working with KScript modules in Kooboo site coding workflows.
---

# KScript Guide

本文用于按当前任务选择所需模块；选定后只读取对应模块 reference，不预加载无关模块。

## 写 KScript 前必读

凡 `k.*` 返回值要进入原生 `href` 插值、`k-for`、`k-content` 或 `k-attribute`，或者要在脚本里 `map` / 挂新字段，**必须先读** [KScript 返回对象与模板绑定](kscript-clr-objects.md)。禁止对 API 返回项使用 `forEach` 赋值或 `{...item}` spread。

## 模块导航

### 核心模块

| 模块 | 用途 |
|------|------|
| [Cookie](core/cookie.md) | 客户端 Cookie 读写 |
| [Request](core/request.md) | HTTP 请求信息 |
| [Response](core/response.md) | HTTP 响应处理 |
| [API](core/api.md) | API 路由定义 |
| [Security](core/security.md) | 安全工具（哈希/加密/JWT） |
| [Logger](core/logger.md) | 日志记录（调试/信息/警告/错误） |
| [Media](core/media.md) | 站内媒体库（图片、视频、音频）— 站内资源优先用此模块 |

### 工具模块

| 模块 | 用途 |
| --- | --- |
| [Utils 总览](utils/index.md) | 选择所需的工具模块，包括 Office、XML、截图、短信等 |

### 业务模块

- [Net 网络模块](net/index.md)（HTTP、WebSocket、DNS、IP、Ping、SSH、SFTP、Telnet）
- [Google 服务](google.md)（PageSpeed/Lighthouse、Firebase）
- [Account 账户模块](account/index.md)
- [Commerce 电商模块](commerce/index.md)
- [Payment 支付模块](payment/index.md)
- [File 文件操作](file/index.md)
- [DB 数据库模块](DB/index.md)
- [Content 内容模块](content/index.md)（建模见 [function_guide](content/function_guide.md)）
