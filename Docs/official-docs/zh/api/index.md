# API 详情

KScript API 完整参考（服务端 `k.*`）。文档路径与模块名一致，**按字母序**排列便于检索。

## 入门

| 文档 | 说明 |
|------|------|
| [快速开始](./getting-started/) | API 基础与快速上手 |
| [认证与授权](./getting-started/auth.md) | 登录与会话 |

## KScript API（A–Z）

| 模块 | 文档 | 说明 |
|------|------|------|
| `k.account` | [account/](./account/) | 登录、用户、组织、OAuth |
| `k.api` | [api/](./api/) | API 路由 |
| `k.cache` | [cache/](./cache/) | 站点内存缓存 |
| `k.commerce` | [commerce/](./commerce/commerce.md) | 电商（含 product、cart、order 等子页） |
| `k.content` | [content/](./content/) | 内容类型与内容项 |
| `k.cookie` | [cookie/](./cookie/) | Cookie |
| `k.DB` | [database/](./database/) · [indexed-db/](./indexed-db/) · [key-value/](./key-value/) · [sqlite/](./sqlite/) | 数据库、IndexedDB、键值、SQLite |
| `k.file` | [file/](./file/file.md) | 站点目录文件 |
| `k.google` | [google/](./google/) | PageSpeed/Lighthouse 与 Firebase |
| `k.label` | [label/](./label/) | 多语言 `k.t` / `k.label` |
| `k.logger` | [logger/](./logger/) | 代码日志 |
| `k.mail` | [mail/](./mail/) | 发信、IMAP、SES |
| `k.media` | [media/](./media/) | 媒体库 |
| `k.module` | [module/](./module/) | 模块包：`config`、`task`、`localFile`、导入导出 |
| `k.openApi` | [openapi/](./openapi/) | 外部 OpenAPI 定义调用与授权 |
| `k.net` | [net/](./net/) | HTTP、WebSocket、DNS、IP、Ping、SSH、SFTP、Telnet |
| `k.page` | [page/](./page/) | 当前页 title / meta |
| `k.paramConfig` | [param-config/](./param-config/) | 参数配置（结构化 JSON） |
| `k.payment` | [payment/](./payment/) | 支付集成 |
| `k.request` | [request/](./request/) | HTTP 请求 |
| `k.response` | [response/](./response/) | HTTP 响应 |
| `k.security` | [security/](./security/) | 加密与哈希 |
| `k.session` | [session/](./session/) | 会话 |
| `k.site` | [site/](./site/) | 页面、布局、视图、脚本、样式、codes |
| `k.state` | [state/](./state/) | 当前请求视图数据 |
| `k.utils` | [utils/](./utils/) | 工具集（子模块见 utils 索引） |
| SPA 多语言 | [spa-multilingual/](./spa-multilingual/) | `/_spa/lang/{lang}` 词典（无 `k.*` CRUD） |

## `k.site` 子资源

| 成员 | 文档 |
|------|------|
| `k.site.pages` | [page.md](./site/page.md) |
| `k.site.layouts` | [layout.md](./site/layout.md) |
| `k.site.views` | [view.md](./site/view.md) |
| `k.site.scripts` | [js.md](./site/js.md) |
| `k.site.styles` | [css.md](./site/css.md) |
| `k.site.codes` | [code.md](./site/code.md) |
| `k.site.menus` | [menu.md](./site/menu.md) |
| `k.site.runJob` | [job.md](./site/job.md) |

## `k.file` 子页

[folder](./file/file-folder.md) · [resumable](./file/file-resumable.md) · [数据结构](./file/file-info.md)

## 相关区域

| 场景 | 后续文档 |
|------|----------|
| 需要先准备内容模型或站点设置 | [CMS 后台](../cms/index.md) |
| 需要先了解 Page / Layout / View 渲染规则 | [模板引擎](../templateEngine/index.md) |
