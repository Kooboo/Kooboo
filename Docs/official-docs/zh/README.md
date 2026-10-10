# Kooboo 产品文档

> Kooboo 产品文档的 markdown 版本：KScript API 参考、Kooboo 后台（CMS）指南、模板引擎参考、
> 快速上手教程。
>
> 英文版：[English](../README.md) · 全仓库的机器可读地图：[llms.txt](../../llms.txt)

共 208 篇，每篇都是独立的 markdown 文件，可单独阅读、引用或索引。下面是完整目录。

## Kooboo 文档中心 — `index.md` (1 篇)

从 AI 建站到后台 CMS、模板引擎与 KScript API，把 Kooboo 的产品能力串成一条可以落地的工作流。

| 页面 | 说明 |
| --- | --- |
| [Kooboo 文档中心](index.md) | 从 AI 建站到后台 CMS、模板引擎与 KScript API，把 Kooboo 的产品能力串成一条可以落地的工作流。 |

## API 详情 — `api/` (100 篇)

KScript API 完整参考（服务端 `k.*`）。文档路径与模块名一致，**按字母序**排列便于检索。

| 页面 | 说明 |
| --- | --- |
| [k.account](api/account/index.md) | Kooboo 平台账户：登录态、当前用户、组织与第三方登录 |
| [k.account — 登录与身份](api/account/login.md) | `k.account` 根对象上的登录态、登录/登出与令牌相关 API |
| [k.account.oAuth](api/account/oauth.md) | 第三方登录授权 URL 与回调处理 |
| [k.account.organization](api/account/organization.md) | 当前站点所属组织及组织内用户、部门 |
| [k.account.user](api/account/user.md) | 当前平台用户信息与用户查询 |
| [k.api](api/api/index.md) | 提供一系列方法用于更方便地创建 API |
| [k.cache](api/cache/index.md) | 站点级内存缓存，在同一站点内跨请求共享 |
| [address](api/commerce/address.md) | 地址管理 |
| [cart](api/commerce/cart.md) | 购物车 |
| [category](api/commerce/category.md) | 产品分类管理 - 分类 CRUD 和搜索 |
| [k.commerce](api/commerce/commerce.md) | 电商模块主模块 |
| [currency](api/commerce/currency.md) | 货币管理 |
| [customer](api/commerce/customer.md) | 客户管理 |
| [discount](api/commerce/discount.md) | 折扣优惠 |
| [loyalty](api/commerce/loyalty.md) | 会员积分 |
| [membership](api/commerce/membership.md) | 会员等级 |
| [order](api/commerce/order.md) | 订单 |
| [product](api/commerce/product.md) | 产品管理 - 产品 CRUD 和搜索 |
| [settings](api/commerce/settings.md) | 电商设置 |
| [shipping](api/commerce/shipping.md) | 配送方式 |
| [wishlist](api/commerce/wishlist.md) | 愿望清单 |
| [k.content](api/content/index.md) | 内容管理 - 内容 CRUD 和查询 |
| [k.cookie](api/cookie/index.md) | Cookie 管理 - 读写客户端 Cookie |
| [k.DB](api/database/index.md) | 数据库操作 - 支持多种数据库的统一 CRUD 接口 |
| [k.file/folder](api/file/file-folder.md) | 文件夹操作 |
| [k.file - 数据结构](api/file/file-info.md) | 数据结构定义 |
| [k.file/resumable](api/file/file-resumable.md) | 断点续传 |
| [k.file](api/file/file.md) | 文件操作 |
| [认证与授权](api/getting-started/auth.md) | 站点内用户登录、会话与访问控制相关 API |
| [API 快速开始](api/getting-started/index.md) | 了解如何在 Kooboo 中使用 KScript API |
| [k.google](api/google/index.md) | Google PageSpeed/Lighthouse 与 Firebase 服务 |
| [API 详情](api/index.md) | KScript API 完整参考（服务端 `k.*`）。文档路径与模块名一致，**按字母序**排列便于检索。 |
| [k.DB.indexedDb](api/indexed-db/index.md) | 站点 IndexedDB 动态表 — 与后台 IndexedDB 表 为同一套数据 |
| [k.DB.keyValue](api/key-value/index.md) | 站点键值对存储 — 与后台 键值存储 相同 |
| [k.site.labels](api/label/index.md) | 管理站点的多语言标签 |
| [k.logger](api/logger/index.md) | 日志记录与查询 |
| [k.mail](api/mail/index.md) | 发信、收信（IMAP）、邮件解析、Amazon SES 与反垃圾检测 |
| [k.media](api/media/index.md) | 站点媒体库（图片）的文件夹与文件管理 |
| [k.module](api/module/index.md) | 脚本模块（**开发 → 模块**）运行时的 KScript API |
| [k.net.DNS](api/net/dns.md) | DNS 查询、反向解析与 Whois |
| [k.net.httpClient](api/net/httpclient.md) | 出站 HTTP：统一 `send`、多种请求体与批量任务 |
| [k.net](api/net/index.md) | HTTP、WebSocket、DNS、IP 与远程连接 |
| [k.net.IP](api/net/ip.md) | 当前请求 IP 与 IP 地理信息 |
| [k.net.ping](api/net/ping.md) | 使用 ICMP 探测主机连通性与往返时间 |
| [k.net.sftpClient](api/net/sftpClient.md) | SFTP 文件上传与下载 |
| [k.net.sshClient](api/net/sshClient.md) | SSH 命令执行 |
| [k.net.telnetClient](api/net/telnetClient.md) | Telnet 会话 |
| [k.net.url](api/net/url.md) | 简便的同步 HTTP 请求（字符串 / 对象 / 二进制） |
| [k.net.webSocket](api/net/websocket.md) | 将当前请求升级为 WebSocket 并管理站点内连接 |
| [k.openApi](api/openapi/index.md) | 按站点 OpenAPI 定义调用外部 HTTP API，并管理鉴权凭据 |
| [k.page](api/page/index.md) | 当前 HTTP 请求所渲染页面的 `<title>` 与 `<meta>` 绑定 |
| [k.paramConfig](api/param-config/index.md) | 读取后台 **内容 → 参数配置** 中保存的结构化 JSON 数据 |
| [alipay](api/payment/alipay.md) | 支付宝支付。 |
| [k.payment](api/payment/index.md) | 第三方支付集成。 |
| [moneyboxs](api/payment/moneyboxs.md) | MoneyBoxs 支付。 |
| [paynl](api/payment/paynl.md) | Pay.NL 支付。 |
| [paypal](api/payment/paypal.md) | PayPal 支付。 |
| [square](api/payment/square.md) | Square 支付。 |
| [stripe](api/payment/stripe.md) | Stripe 支付。 |
| [two-checkout](api/payment/two-checkout.md) | TwoCheckout 支付。 |
| [微信 H5 支付流程](api/payment/wechat-h5-flow.md) | 面向 AI 与开发者的微信 **H5** 支付指南，说明从创建支付请求到回调验证的完整接入流程。 |
| [wechat](api/payment/wechat.md) | 微信支付。 |
| [k.request](api/request/index.md) | HTTP 请求信息 - 获取客户端请求的各种数据 |
| [k.response](api/response/index.md) | HTTP 响应处理 - 设置响应内容、状态码、重定向等 |
| [k.security](api/security/index.md) | 安全相关工具 - 哈希、加密、编码、GUID、JWT 等 |
| [k.session](api/session/index.md) | 会话级缓存 - 在服务器端存储会话数据 |
| [k.site.codes](api/site/code.md) | 站点代码仓库：API 端点脚本与可复用 CodeBlock 的增删改查 |
| [k.site.styles](api/site/css.md) | 站点 CSS 样式资源的增删改查、路由 URL 与版本日志 |
| [k.site](api/site/index.md) | 当前站点的版本化数据库访问入口（页面、布局、脚本、样式等资源） |
| [k.site.runJob](api/site/job.md) | 对应后台 开发 → 定时任务 |
| [k.site.scripts](api/site/js.md) | 站点 JavaScript 资源的增删改查、路由 URL 与版本日志 |
| [k.site.layouts](api/site/layout.md) | 站点布局（Layout）资源的增删改查与版本日志 |
| [k.site.menus](api/site/menu.md) | 站点前台导航菜单的读取与维护（与后台 **开发 → 菜单** 同一套数据） |
| [k.site.pages](api/site/page.md) | 站点页面的增删改查、路由 URL 与版本日志 |
| [k.site.views](api/site/view.md) | 可复用视图（View）片段的增删改查与版本日志 |
| [SPA 多语言（前台接口）](api/spa-multilingual/index.md) | 为单页应用提供 JSON 词典；词条在 CMS：SPA 多语言 维护 |
| [k.DB.sqlite](api/sqlite/index.md) | SQLite 数据库操作 - 无需配置的轻量级数据库 |
| [k.state](api/state/index.md) | 当前请求渲染上下文中的视图数据（ViewData） |
| [k.utils.CDN](api/utils/cdn.md) | 当前站点 CDN 开关与缓存时间 |
| [k.utils.clientJS](api/utils/clientJS.md) | 向页面输出客户端 JavaScript 变量 |
| [k.utils.community](api/utils/community.md) | Kooboo 社区资源分享与搜索 |
| [k.utils.compression](api/utils/compression.md) | Gzip 解压与 ZIP 打包 |
| [k.utils.converter](api/utils/converter.md) | 调用 Kooboo 转换服务（HTML/PDF、Office、GIF 等） |
| [k.utils.CRM](api/utils/crm.md) | Kooboo CRM 客户与订单拉取 |
| [k.utils.date](api/utils/date.md) | 日期时间与格式化 |
| [k.utils.dom](api/utils/dom.md) | HTML 解析与图片提取 |
| [k.utils.image](api/utils/image.md) | 图片格式、尺寸与水印 |
| [k.utils](api/utils/index.md) | KScript 工具集：按子模块划分的辅助 API |
| [k.utils.inlineHtml](api/utils/inlineHtml.md) | 将页面或 URL 资源内联为 data URI |
| [k.utils.mime](api/utils/mime.md) | 文件名与 MIME 类型互查 |
| [k.utils.ninjible](api/utils/ninjible.md) | 站点导入与 Lighthouse 对比任务（Ninjible） |
| [k.utils.office](api/utils/office.md) | Office 文档处理（Excel） |
| [k.utils.puppeteer](api/utils/puppeteer.md) | 无头浏览器截图（仅非线上环境） |
| [k.utils.screenShot](api/utils/screenShot.md) | 网页与页面截图 |
| [k.utils.sitemap](api/utils/sitemap.md) | 构建 XML Sitemap |
| [k.utils.sms](api/utils/sms.md) | 短信发送（多提供商） |
| [k.utils.string](api/utils/string.md) | 字符串与 HTML 文本处理（`KText`） |
| [k.utils.template](api/utils/template.md) | 视图与页面渲染 |
| [k.utils.uri](api/utils/uri.md) | URL 解析与查询字符串 |
| [k.utils.xml](api/utils/xml.md) | 解析与构建 XML 文档 |

## Kooboo 后台（CMS） — `cms/` (89 篇)

在 Kooboo 控制台中配置站点、内容与集成——面向管理员与实施人员

| 页面 | 说明 |
| --- | --- |
| [购物车](cms/commerce/carts.md) | 菜单：**电商 → 购物车** |
| [货币](cms/commerce/currencies.md) | 菜单：**电商 → 货币** |
| [顾客管理](cms/commerce/customers.md) | 菜单：**电商 → 顾客管理** |
| [优惠折扣](cms/commerce/discounts.md) | 菜单：**电商 → 优惠折扣** |
| [电商](cms/commerce/index.md) | 菜单：左侧 **电商** 分组（须在 **编辑菜单** 中勾选显示） |
| [会员](cms/commerce/loyalty.md) | 菜单：**电商 → 会员** |
| [通知](cms/commerce/notification.md) | 菜单：**电商 → 通知** |
| [订单](cms/commerce/orders.md) | 菜单：**电商 → 订单** |
| [商品分类](cms/commerce/product-categories.md) | 菜单：**电商 → 商品分类** |
| [商品详情（新建与编辑）](cms/commerce/product-management-detail.md) | **新建**：`/_Admin/commerce/product-management/create?SiteId=...&typeId={类型Id或空白}` |
| [商品管理（列表）](cms/commerce/product-management.md) | 菜单：**电商 → 商品管理** |
| [商品类型](cms/commerce/product-types.md) | 菜单：**电商 → 商品类型** |
| [销售统计](cms/commerce/sale-stats.md) | 菜单：**电商 → 销售统计** |
| [设置](cms/commerce/settings.md) | 菜单：**电商 → 设置** |
| [配送](cms/commerce/shippings.md) | 菜单：**电商 → 配送** |
| [税（税费）](cms/commerce/taxes.md) | 菜单：**电商 → 税** |
| [数据类型](cms/content/content-types.md) | 菜单：**内容 → 数据类型** |
| [内容条目](cms/content/contents-entries.md) | **文件夹**：`/_Admin/content/textContentsByFolder?SiteId=...&folder={内容文件夹Id}` |
| [内容设置](cms/content/contents-folder-settings.md) | 从 内容列表 行内 **设置**（齿轮）或 **新建** 向导打开 |
| [内容列表](cms/content/contents-folders-list.md) | 菜单：**内容 → 内容** |
| [文件](cms/content/files.md) | 菜单：**内容 → 文件**（须在后台 **"编辑菜单"** 中勾选显示；路由 `advanced: true`） |
| [HTML 片段](cms/content/html-blocks.md) | 菜单：**内容 → HTML 片段** |
| [内容](cms/content/index.md) | 菜单：左侧 **内容** 分组（部分子项需在 **高级菜单** 中开启） |
| [标签](cms/content/labels.md) | 菜单：**内容 → 标签**（界面英文菜单名可能为 Labels） |
| [元素属性](cms/content/tag-attributes.md) | 菜单：**内容 → 元素属性**（路由 `text`；须在 **编辑菜单** 中勾选显示） |
| [参数配置](cms/content/user-options.md) | 菜单：**内容 → 参数配置**（路由 `useroptions`；须在 **编辑菜单** 中勾选显示） |
| [数据库](cms/database/index.md) | 菜单：左侧 **数据库** 分组（须在 **编辑菜单** 中勾选显示） |
| [键值存储](cms/database/key-value.md) | 菜单：**数据库 → 键值存储** |
| [MySQL 表](cms/database/mysql-table.md) | 菜单：**数据库 → MySQL 表** |
| [SQL 日志](cms/database/sql-logs.md) | 菜单：**数据库 → SQL 日志** |
| [SQLite 表](cms/database/sqlite-table.md) | 菜单：**数据库 → SQLite 表** |
| [SQL Server 表](cms/database/sqlserver-table.md) | 菜单：**数据库 → SQL Server 表** |
| [IndexedDB 表关系](cms/database/table-relation.md) | 菜单：**数据库 → IndexedDB 表关系** |
| [IndexedDB 表](cms/database/table.md) | 菜单：**数据库 → IndexedDB 表** |
| [身份验证（Authentication）](cms/development/authentication.md) | 菜单：**开发 → 身份验证** |
| [代码日志](cms/development/code-log.md) | 菜单：**开发 → 代码日志** |
| [代码搜索](cms/development/code-search.md) | 菜单：**开发 → 代码搜索** |
| [代码（Code）](cms/development/code.md) | 菜单：**开发 → 代码** |
| [表单（Forms）](cms/development/forms.md) | 菜单：**开发 → 表单** |
| [开发](cms/development/index.md) | 菜单：左侧 **开发** 分组（须在 **编辑菜单** 中勾选显示） |
| [定时任务（Jobs）](cms/development/jobs.md) | 菜单：**开发 → 定时任务** |
| [布局（Layouts）](cms/development/layouts.md) | 菜单：**开发 → 布局** |
| [菜单（Menus）](cms/development/menus.md) | 菜单：**开发 → 菜单** |
| [模块（Modules）](cms/development/modules.md) | 菜单：**开发 → 模块** |
| [Open API](cms/development/openapis.md) | 菜单：**开发 → Open API** |
| [脚本（Scripts）](cms/development/scripts.md) | 菜单：**开发 → 脚本** |
| [SPA 多语言](cms/development/spa-multilingual.md) | 菜单：**开发 → SPA 多语言** |
| [样式（Styles）](cms/development/styles.md) | 菜单：**开发 → 样式** |
| [URL](cms/development/urls.md) | 菜单：**开发 → URL** |
| [组件（Views）](cms/development/views.md) | 菜单：**开发 → 组件** |
| [新建站点](cms/getting-started/create-site.md) | 从 **我的站点** 创建站点：空白站点、导入包、克隆现有站点 |
| [登录与站点列表](cms/getting-started/login-and-site-list.md) | 从控制台首页进入 Kooboo：**我的站点**、文件夹整理、进入某一站点的后台 |
| [Kooboo 后台（CMS）](cms/index.md) | 在 Kooboo 控制台中配置站点、内容与集成——面向管理员与实施人员 |
| [后台菜单总览](cms/navigation.md) | 按 Kooboo 站点后台左侧菜单归纳。界面文案以你登录后的中文界面为准。 |
| [AB 测试](cms/operations/ab-testing.md) | 菜单：**运维中心 → AB 测试** |
| [运维中心](cms/operations/index.md) | 菜单：左侧 **运维中心** 分组（须在 **编辑菜单** 中勾选显示） |
| [页面交互](cms/operations/page-interaction.md) | 菜单：**运维中心 → 页面交互** |
| [资源守护](cms/operations/resource-guardian.md) | 菜单：**运维中心 → 资源守护** |
| [操作日志](cms/operations/site-logs.md) | 菜单：**运维中心 → 操作日志** |
| [协作同步](cms/operations/sync.md) | 菜单：**运维中心 → 协作同步** |
| [访问统计](cms/operations/visitor-logs.md) | 菜单：**运维中心 → 访问统计** |
| [操作 Hook](cms/settings/action-hooks.md) | 菜单：**站点设置 → 操作 Hook**（高级菜单） |
| [AI 服务](cms/settings/ai-service.md) | 菜单：**站点设置 → AI 服务**（高级菜单） |
| [基础 · 访问控制 Tab](cms/settings/basic/access.md) | Tab：**访问控制** · `name=access` |
| [基础 · 高级 Tab](cms/settings/basic/advanced.md) | Tab：**高级** · `name=advance` |
| [基础 · 编辑器 Tab](cms/settings/basic/editor.md) | Tab：**编辑器** · `name=editor` |
| [基础 · 基础 Tab](cms/settings/basic/general.md) | Tab：**基础** · `name=basic` |
| [基础设置](cms/settings/basic/index.md) | 菜单：**站点设置 → 基础设置** |
| [基础 · 性能 Tab](cms/settings/basic/performance.md) | Tab：**性能** · `name=performance` |
| [基础 · SEO Tab](cms/settings/basic/seo.md) | Tab：**SEO** · `name=seo` |
| [Cookie](cms/settings/cookie.md) | 菜单：**站点设置 → Cookie 管理** |
| [域名管理](cms/settings/domains.md) | 菜单：**站点设置 → 域名管理** |
| [站点设置](cms/settings/index.md) | 菜单：左侧 **站点设置** 分组（须在 **编辑菜单** 中勾选显示） |
| [服务集成 · Database](cms/settings/integrations/database.md) | Group：**Database** |
| [服务集成](cms/settings/integrations/index.md) | 菜单：**站点设置 → 服务集成** |
| [服务集成 · OAuth2](cms/settings/integrations/oauth2.md) | Group：**OAuth2** |
| [服务集成 · Others](cms/settings/integrations/others.md) | Group：**Others** |
| [服务集成 · Payment](cms/settings/integrations/payment.md) | Group：**Payment** |
| [服务集成 · SMS](cms/settings/integrations/sms.md) | Group：**SMS** |
| [服务集成 · Storage](cms/settings/integrations/storage.md) | Group：**Storage** |
| [请求 Hook](cms/settings/request-hooks.md) | 菜单：**站点设置 → 请求 Hook**（高级菜单） |
| [角色权限](cms/settings/roles.md) | 菜单：**站点设置 → 角色权限**（高级菜单） |
| [站点用户](cms/settings/site-users.md) | 菜单：**站点设置 → 用户管理** |
| [控制面板](cms/site/dashboard.md) | 菜单：左侧顶栏 **主页**（进入站点后默认页） |
| [媒体库](cms/site/media.md) | 菜单：**媒体库**（左侧顶栏，紧挨主页图标下方） |
| [编辑代码](cms/site/pages-edit-code.md) | 从 页面列表 行内 **编辑代码** 进入（设计页为 **设计**） |
| [在线编辑](cms/site/pages-inline-edit.md) | 从 页面列表 行内 **在线编辑** 进入 |
| [页面设置](cms/site/pages-setting.md) | 从 页面列表 行内 **设置** 进入 |
| [页面](cms/site/pages.md) | 菜单：**页面**（左侧顶栏） |

## 快速开始 — `quick-start/` (6 篇)

用最短路径体验 Kooboo：创建站点、做页面、放内容、写一点服务端逻辑并发布。

| 页面 | 说明 |
| --- | --- |
| [添加内容](quick-start/add-content.md) | 建立内容模型与内容条目，让页面从后台读取数据。 |
| [写第一个 KScript API](quick-start/add-script-api.md) | 在 Kooboo 中加入一个服务端接口，让前台页面可以读取 JSON 或执行业务逻辑。 |
| [做第一个页面](quick-start/build-first-page.md) | 用后台页面功能创建一个可以访问的 URL。 |
| [创建第一个站点](quick-start/create-first-site.md) | 从登录后台到进入站点管理页。 |
| [快速开始](quick-start/index.md) | 用最短路径体验 Kooboo：创建站点、做页面、放内容、写一点服务端逻辑并发布。 |
| [发布与域名](quick-start/publish-domain.md) | 确认站点可以被外部访问，并完成基础上线配置。 |

## 模板引擎 — `templateEngine/` (12 篇)

Kooboo 站点前端：页面、布局、视图与静态资源如何组织与渲染

| 页面 | 说明 |
| --- | --- |
| [模板绑定语法](templateEngine/binding/index.md) | 服务端 `k-*` 指令：在 Layout / Page / View 中绑定数据与属性 |
| [Style（站点样式）](templateEngine/css/index.md) | 带路由的 `.css` 资源：全局主题、补丁与 Layout `<head>` 引用 |
| [模板引擎](templateEngine/index.md) | Kooboo 站点前端：页面、布局、视图与静态资源如何组织与渲染 |
| [Script（站点脚本）](templateEngine/js/index.md) | 带路由的 `.js` 资源：全局工具、ES Module、与 Page 中的 `<script>` 配合 |
| [JSON5 条件表达式](templateEngine/k-data/condition.md) | 用于 `<query where="...">` 与 `<when test="...">` |
| [k-data 内置函数](templateEngine/k-data/functions.md) | `<let use="函数名" params="...">` 可用函数 |
| [k-data](templateEngine/k-data/index.md) | 在 HTML 模板中用标签声明数据流程（**推荐**，替代已弃用的 `k-query`） |
| [query 数据源](templateEngine/k-data/query.md) | `<query>` 的 `source`、`resource`、`action` 组合说明 |
| [k-data 标签语法](templateEngine/k-data/tags.md) | `<k-data>` 内各标签的属性与组合方式 |
| [Layout（布局）](templateEngine/layout/index.md) | 站点级 HTML 骨架：`<head>`、全局资源与 `k-placeholder` 占位区 |
| [Page（页面）](templateEngine/page/index.md) | 带路由的 HTML 页面：独立完整页，或通过 Layout 占位注入 |
| [View（视图）](templateEngine/view/index.md) | 可复用的 HTML 片段，通过 `<view id="...">` 嵌入 Page / Layout / 其他 View |
