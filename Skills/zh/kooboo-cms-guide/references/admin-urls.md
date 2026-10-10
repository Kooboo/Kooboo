# Kooboo 后台深链索引

> 站点级深链格式：`/_Admin{路由}?SiteId={站点GUID}`  
> `{站点GUID}` 来自 `get_site_info` → `siteSettings.siteId`，查询参数名固定为 **`SiteId`**（首字母大写）。

## 账户级（无 SiteId）

| 页面 | 路径 |
|------|------|
| 我的站点 | `/_Admin/` |
| 登录 | `/_Admin/login` |
| 新建站点 | `/_Admin/create` |
| 导入站点 | `/_Admin/create/import` |
| 克隆站点 | `/_Admin/create/clone` |
| 文件夹内列表 | `/_Admin/?currentFolder={文件夹名}` |

## 顶栏与常用入口

| 入口 | 深链 |
|------|------|
| 控制面板 | `/_Admin/site?SiteId={站点GUID}` |
| 媒体库 | `/_Admin/site/media?SiteId={站点GUID}` |
| 页面列表 | `/_Admin/site/pages?SiteId={站点GUID}` |
| 页面设置（HTML） | `/_Admin/page/setting?SiteId={站点GUID}&id={页面Id}` |
| 页面代码编辑 | `/_Admin/page/edit?SiteId={站点GUID}&id={页面Id}` |
| 内联编辑 | `/_Admin/inline-design?SiteId={站点GUID}&path={页面路径}&id={页面Id}&type={页面类型}` |

## 内容

| 菜单 | 深链 |
|------|------|
| 内容夹与条目 | `/_Admin/content/contents?SiteId={站点GUID}` |
| 内容条目（按夹） | `/_Admin/content/textContentsByFolder?SiteId={站点GUID}&folder={内容文件夹Id}` |
| 编辑内容条目 | `/_Admin/content/content?SiteId={站点GUID}&folder={内容文件夹Id}&id={条目Id}` |
| 数据类型 | `/_Admin/content/contentTypes?SiteId={站点GUID}` |
| 编辑数据类型 | `/_Admin/content/contentType?SiteId={站点GUID}&id={类型Id}` |
| 标签（Label） | `/_Admin/content/labels?SiteId={站点GUID}` |
| HTML 片段 | `/_Admin/content/htmlblocks?SiteId={站点GUID}` |
| 编辑 HTML 片段 | `/_Admin/content/htmlBlock/edit?SiteId={站点GUID}&id={片段Id}` |
| 文件 | `/_Admin/content/files?SiteId={站点GUID}` |
| 元素属性 | `/_Admin/content/text?SiteId={站点GUID}` |
| 参数配置 | `/_Admin/content/useroptions?SiteId={站点GUID}` |

## 电商

| 菜单 | 深链 |
|------|------|
| 商品管理 | `/_Admin/commerce/product-management?SiteId={站点GUID}` |
| 商品类型 | `/_Admin/commerce/product-types?SiteId={站点GUID}` |
| 商品分类 | `/_Admin/commerce/product-categories?SiteId={站点GUID}` |
| 购物车 | `/_Admin/commerce/carts?SiteId={站点GUID}` |
| 顾客管理 | `/_Admin/commerce/customers?SiteId={站点GUID}` |
| 订单 | `/_Admin/commerce/orders?SiteId={站点GUID}` |
| 销售统计 | `/_Admin/commerce/sale-stats?SiteId={站点GUID}` |
| 优惠折扣 | `/_Admin/commerce/discounts?SiteId={站点GUID}` |
| 货币 | `/_Admin/commerce/currencies?SiteId={站点GUID}` |
| 配送 | `/_Admin/commerce/shippings?SiteId={站点GUID}` |
| 会员 | `/_Admin/commerce/loyalty?SiteId={站点GUID}` |
| 税 | `/_Admin/commerce/taxes?SiteId={站点GUID}` |
| 通知 | `/_Admin/commerce/notification?SiteId={站点GUID}` |
| 电商设置 | `/_Admin/commerce/settings?SiteId={站点GUID}` |

::: warning 支付密钥不在电商设置
支付渠道密钥只在 **站点设置 → 服务集成 → Payment**：`/_Admin/system/config?SiteId={站点GUID}&group=Payment`
:::

## 运维中心

路由前缀：`/_Admin/system/...?SiteId={站点GUID}`（与「站点设置」共用 `/system` 前缀，左侧菜单分组不同）

| 菜单 | 深链 |
|------|------|
| 访问统计 | `/_Admin/system/visitor-logs?SiteId={站点GUID}` |
| 资源守护 | `/_Admin/system/resource-guardian?SiteId={站点GUID}` |
| 页面交互 | `/_Admin/system/page-interaction?SiteId={站点GUID}` |
| AB 测试 | `/_Admin/system/ab-testing?SiteId={站点GUID}` |
| 操作日志 | `/_Admin/system/site-logs?SiteId={站点GUID}` |
| 协作同步 | `/_Admin/system/sync?SiteId={站点GUID}` |
| AI 服务 | `/_Admin/system/ai-service?SiteId={站点GUID}` |

## 开发

路由前缀：`/_Admin/development/...?SiteId={站点GUID}`

| 菜单 | 深链 |
|------|------|
| 组件（Views） | `/_Admin/development/views?SiteId={站点GUID}` |
| 布局（Layouts） | `/_Admin/development/layouts?SiteId={站点GUID}` |
| 脚本 | `/_Admin/development/scripts?SiteId={站点GUID}` |
| 样式 | `/_Admin/development/styles?SiteId={站点GUID}` |
| 代码 | `/_Admin/development/code?SiteId={站点GUID}` |
| 编辑代码 | `/_Admin/development/code/edit?SiteId={站点GUID}` |
| 代码日志 | `/_Admin/development/code-log?SiteId={站点GUID}` |
| 代码搜索 | `/_Admin/development/code-search?SiteId={站点GUID}` |
| URL | `/_Admin/development/urls?SiteId={站点GUID}` |
| 表单 | `/_Admin/development/forms?SiteId={站点GUID}` |
| 菜单 | `/_Admin/development/menus?SiteId={站点GUID}` |
| 身份验证 | `/_Admin/development/authentication?SiteId={站点GUID}` |
| Open API | `/_Admin/development/openapis?SiteId={站点GUID}` |
| SPA 多语言 | `/_Admin/development/spamultilingual?SiteId={站点GUID}` |
| 模块 | `/_Admin/development/modules?SiteId={站点GUID}` |
| 定时任务 | `/_Admin/development/jobs?SiteId={站点GUID}` |

## 数据库

路由前缀：`/_Admin/database/...?SiteId={站点GUID}`

| 菜单 | 深链 |
|------|------|
| IndexedDB 表 | `/_Admin/database/table?SiteId={站点GUID}` |
| 表关系 | `/_Admin/database/table-relation?SiteId={站点GUID}` |
| 键值存储 | `/_Admin/database/key-value?SiteId={站点GUID}` |
| SQLite 表 | `/_Admin/database/sqlite-table?SiteId={站点GUID}` |
| MySQL 表 | `/_Admin/database/mysql-table?SiteId={站点GUID}` |
| SQL Server 表 | `/_Admin/database/sqlserver-table?SiteId={站点GUID}` |
| SQL 日志 | `/_Admin/database/sql-logs?SiteId={站点GUID}` |

未配置外部库时，MySQL / SQL Server 引导页会跳转到 `/_Admin/system/config?SiteId={站点GUID}&group=Database`。

## 站点设置

路由前缀：`/_Admin/system/...?SiteId={站点GUID}`

| 菜单 | 深链 |
|------|------|
| 基础设置 | `/_Admin/system/settings?SiteId={站点GUID}` |
| 域名管理 | `/_Admin/system/domains?SiteId={站点GUID}` |
| 服务集成 | `/_Admin/system/config?SiteId={站点GUID}` |
| 用户管理 | `/_Admin/system/siteuser?SiteId={站点GUID}` |
| 角色权限 | `/_Admin/system/roles?SiteId={站点GUID}` |
| 请求 Hook | `/_Admin/system/front-events?SiteId={站点GUID}` |
| 操作 Hook | `/_Admin/system/backend-events?SiteId={站点GUID}` |
| Cookie 管理 | `/_Admin/system/cookie?SiteId={站点GUID}` |
| AI 服务 | `/_Admin/system/ai-service?SiteId={站点GUID}` |

### 基础设置 Tab（`name` 查询参数）

| `name` | 内容 |
|--------|------|
| `basic` | 显示名、Base URL、SSL、SPA、**多语言** |
| `access` | IP / 地区访问控制 |
| `performance` | 缓存、压缩 |
| `seo` | 站点级 SEO |
| `editor` | 后台编辑器偏好 |
| `advance` | 默认数据库、SQL 日志开关等 |

示例：`/_Admin/system/settings?SiteId={站点GUID}&name=basic`

### 服务集成分组（`group` 查询参数）

| `group` | 典型集成项 |
|---------|------------|
| `Database` | Mysql、SqlServer、Mongo、Redis、Sqlite |
| `Payment` | Alipay、WeChatV3、Paypal、StripePayment 等 |
| `SMS` | AliSMS、TencentSMSSetting |
| `OAuth2` | GoogleLoginSetting、FacebookLoginSetting 等 |
| `Storage` | 对象存储 |
| `Others` | **JwtSetting**、FirebaseSetting |

示例：`/_Admin/system/config?SiteId={站点GUID}&group=Payment`

### Hook 编辑子页

| 类型 | 深链 |
|------|------|
| 请求 Hook 编辑 | `/_Admin/system/front-events/edit?SiteId={站点GUID}&name={事件名}&display={显示名}` |
| 操作 Hook 编辑 | `/_Admin/system/backend-events/edit?SiteId={站点GUID}&name={事件名}&display={显示名}` |

## 独立功能（非左侧主菜单）

| 功能 | 深链 |
|------|------|
| AI App Builder | `/_Admin/ai-app-builder?SiteId={站点GUID}` |
| AI Chat | `/_Admin/ai-chat/overview?SiteId={站点GUID}` |

## 易混淆对照

| 你想去的功能 | 正确深链 | 常见错误 |
|--------------|----------|----------|
| JWT Secret | `.../system/config?...&group=Others` | 只写 `config?SiteId=` 无 group；或写成 `commerce/settings` |
| 支付渠道密钥 | `.../system/config?...&group=Payment` | 写成 `commerce/settings` 或 `commerce/payment` |
| 多语言开关 | `.../system/settings?...&name=basic` | 写成 `development/spamultilingual`（那是 SPA 多语言） |
| 访问控制规则 | `.../development/authentication?...` | 写成 `system/config` 或 `system/settings` |
| 代码日志（KScript） | `.../development/code-log?...` | 写成 `system/site-logs`（那是后台操作日志） |
| Label 文案 | `.../content/labels?...` | 写成 `content/text`（那是元素属性） |
