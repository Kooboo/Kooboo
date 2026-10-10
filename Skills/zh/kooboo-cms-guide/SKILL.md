---
name: kooboo-cms-guide
description: Use when guiding users to Kooboo CMS admin URLs, show_action_button, get_site_info (siteId, baseUrl), or JWT/payment/SMS/OAuth setup links. Always use documented deep links with correct path and query params.
---

# Kooboo CMS 配置路径指引

当用户需要先完成 Kooboo CMS 后台配置、当前方案才能工作时，使用 `show_action_button` 显示跳转按钮，不要只用文字描述点击路径。

拼链接前**必须先**调用 `get_site_info`，从返回的 `siteSettings` 读取 **`siteId`** 与 **`baseUrl`**（禁止编造）。

不确定路径时，先 `read_skill_resource("kooboo-cms-guide", "references/admin-urls.md")` 查表，禁止凭记忆拼接。

## 页面知识 references

当页面 preset 已自动注入某个页面 reference 时，直接使用已注入内容，不要重复读取。只有用户追问的细节超出核心页面 reference 时，才读取其中明确链接的专题 reference。

- 基础设置核心知识：`references/system/settings/basic.md`
- 访问控制核心知识：`references/system/settings/access.md`
- 性能设置核心知识：`references/system/settings/performance.md`
- SEO 设置核心知识：`references/system/settings/seo.md`
- 编辑器设置核心知识：`references/system/settings/editor.md`
- 高级设置核心知识：`references/system/settings/advance.md`
- SPA 路由与 404 行为：`references/system/settings/spa-routing.md`
- 多语言配置：`references/system/settings/multilingual.md`
- 站点包导入导出：`references/system/settings/import-export.md`
- 内容入口与文件夹：`references/content/contents.md`
- 内容类型：`references/content/content-types.md`
- Label 多语言文案：`references/content/labels.md`
- HTML 块：`references/content/html-blocks.md`
- 通用文件：`references/content/files.md`
- 标签属性：`references/content/tag-attributes.md`
- 参数配置：`references/content/user-options.md`
- 媒体库与图片编辑：`references/content/media-library.md`
- 控制面板：`references/site-structure/dashboard.md`
- 页面：`references/site-structure/pages.md`
- 页面编辑器：`references/site-structure/page-editors.md`
- 布局：`references/site-structure/layouts.md`
- 视图：`references/site-structure/views.md`
- 菜单：`references/site-structure/menus.md`
- URL 与路由：`references/site-structure/urls.md`
- 前端脚本：`references/development/scripts.md`
- 前端样式：`references/development/styles.md`
- KScript 代码：`references/development/code.md`
- 表单：`references/development/forms.md`
- 身份验证规则：`references/development/authentication.md`
- 代码日志：`references/development/code-log.md`
- 代码搜索：`references/development/code-search.md`
- Open API：`references/development/openapis.md`
- SPA 多语言：`references/development/spa-multilingual.md`
- 定时任务：`references/development/jobs.md`
- 表单数据：`references/development/form-values.md`
- 模块与动态模块菜单：`references/development/modules.md`
- 开发模式多资源工作区：`references/development/dev-mode.md`
- 域名管理：`references/system/domains.md`
- 站点用户：`references/system/site-users.md`
- 角色权限：`references/system/roles.md`
- 请求 Hook：`references/system/front-events.md`
- 动作 Hook：`references/system/backend-events.md`
- 访问日志：`references/operations/visitor-logs.md`
- 用户行为分析：`references/operations/user-behavior-analysis.md`
- 资源防护：`references/operations/resource-guardian.md`
- 页面互动：`references/operations/page-interaction.md`
- 错误监控：`references/operations/error-monitoring.md`
- A/B 测试：`references/operations/ab-testing.md`
- Cookie 管理：`references/operations/cookie-management.md`
- 站点操作日志：`references/operations/site-logs.md`
- 协作同步：`references/operations/collaboration-sync.md`
- AI 服务：`references/operations/ai-service.md`
- 全文搜索索引：`references/operations/search-index.md`
- Automation：`references/operations/automation.md`
- 服务集成总览：`references/integrations/overview.md`
- 数据库集成：`references/integrations/database.md`
- 支付集成：`references/integrations/payment.md`
- 短信集成：`references/integrations/sms.md`
- OAuth2 集成：`references/integrations/oauth2.md`
- JWT 与 Firebase：`references/integrations/others.md`
- 站点 IndexedDB 表：`references/database/indexeddb-tables.md`
- IndexedDB 表关系：`references/database/table-relations.md`
- Key-Value Store：`references/database/key-value.md`
- SQLite、MySQL 与 SQL Server 表：`references/database/relational-tables.md`
- SQL 日志：`references/database/sql-logs.md`
- 商品管理：`references/commerce/products.md`
- 商品类型：`references/commerce/product-types.md`
- 商品分类：`references/commerce/categories.md`
- 购物车与后台结账：`references/commerce/carts.md`
- 电商客户：`references/commerce/customers.md`
- 订单：`references/commerce/orders.md`
- 销售统计：`references/commerce/sale-stats.md`
- 优惠：`references/commerce/discounts.md`
- 货币：`references/commerce/currencies.md`
- 会员等级：`references/commerce/memberships.md`
- 会员与积分明细：`references/commerce/members.md`
- 赚取积分配置：`references/commerce/earn-points.md`
- 积分抵扣配置：`references/commerce/redeem-points.md`
- 实体配送：`references/commerce/express-shipping.md`
- 数字配送：`references/commerce/digital-shipping.md`
- 税费：`references/commerce/taxes.md`
- 通知：`references/commerce/notifications.md`
- 电商设置：`references/commerce/settings.md`

## 链接类型

| 类型 | 何时使用 | 拼法 |
|------|----------|------|
| **后台配置页** | 站点设置、开发、内容、电商、数据库等 CMS 功能 | `/_Admin{路由}?SiteId={siteId}`，必要时加 `&group=`、`&name=` 等 |
| **前台站点页** | 预览页面、访客可见路由、需与域名一致的绝对地址 | `{baseUrl}{路径}`（完整 `http`/`https` URL） |

### `siteId` 与 `baseUrl`

- **`siteId`**：站点 GUID。后台深链查询参数名固定为 **`SiteId`**（首字母大写），例如 `?SiteId={siteId}`；已有查询串时用 `&SiteId={siteId}`。
- **`baseUrl`**：站点对外根地址（协议 + 主机，可能含路径前缀）。相对路径 `/` 在 AI 控制台域名下解析，**不等于**访客站点域名；前台页必须用 `baseUrl` 拼绝对 URL。

## 路径红线（常见错误）

调用 `show_action_button` 前对照以下规则，避免跳错页：

1. **大小写**：后台前缀固定为 `/_Admin/`（`A` 大写），不是 `/_admin/`、`/admin/`、`/Admin/`。
2. **站点级必须带 `SiteId`**：除账户级页面（`/_Admin/login`、`/_Admin/create`）外，站点后台路径都要带 `SiteId={siteId}`。
3. **不要混淆两个 `/system` 入口**：
   - `/_Admin/system/settings` → **基础设置**（多语言、SEO、性能等 Tab）
   - `/_Admin/system/config` → **服务集成**（Database / Payment / SMS / OAuth2 / Others 等）
4. **服务集成要带 `group`**：需要落到具体分组时，必须加 `&group={GroupName}`，不要只写 `/_Admin/system/config?SiteId=...` 让用户自己找分组：
   - JWT → `group=Others`（项名 `JwtSetting`）
   - 支付 → `group=Payment`（Alipay、WeChatV3 等在此，**不在**电商设置里配密钥）
   - 短信 → `group=SMS`
   - OAuth2 社交登录 → `group=OAuth2`
   - 外部数据库 → `group=Database`
   - 对象存储 → `group=Storage`
5. **不要混淆电商「设置」与支付集成**：
   - 支付渠道密钥 → `/_Admin/system/config?SiteId=...&group=Payment`
   - 电商全局（计量单位、结账支付方式开关、字段模型）→ `/_Admin/commerce/settings?SiteId=...`
   - 货币 → `/_Admin/commerce/currencies?SiteId=...`
6. **身份验证规则 ≠ JWT 配置 ≠ 登录页**：
   - JWT Secret → `/_Admin/system/config?SiteId=...&group=Others`
   - 访问控制规则（JWT 校验 / KScript）→ `/_Admin/development/authentication?SiteId=...`
   - Kooboo 内置登录页 → `/_Admin/login`（账户级，通常不带 `SiteId`）
7. **多语言开关在基础设置 Tab**，不是独立菜单：`/_Admin/system/settings?SiteId=...&name=basic`（多语言在 Basic Tab 的 Multilingual 区域）。
8. **Label 文案** → `/_Admin/content/labels?SiteId=...`（须先开启多语言）。
9. **代码日志** → `/_Admin/development/code-log?SiteId=...`（不是运维操作日志 `site-logs`）。
10. **禁止臆造路径**：如 `/settings/...`、`/cms/...`、`/kooboo/...`、把菜单中文直译成英文 slug 等。

## show_action_button

参数：

- `message`：可选，简短说明为何需要打开该页。
- `label`：必填，按钮文字，动作明确。
- `url`：必填。`http`/`https` 完整链接，或以 `/` 开头的 Kooboo 相对路径。

## 高频场景（已校验深链）

将 `{siteId}` 替换为 `get_site_info` 返回的 `siteSettings.siteId`。

### 服务集成

| 场景 | 后台菜单路径 | 深链 |
|------|--------------|------|
| JWT Secret | 站点设置 → 服务集成 → Others → JwtSetting | `/_Admin/system/config?SiteId={siteId}&group=Others` |
| 微信支付 / 支付宝等 | 站点设置 → 服务集成 → Payment | `/_Admin/system/config?SiteId={siteId}&group=Payment` |
| 阿里云短信 | 站点设置 → 服务集成 → SMS | `/_Admin/system/config?SiteId={siteId}&group=SMS` |
| OAuth2 社交登录 | 站点设置 → 服务集成 → OAuth2 | `/_Admin/system/config?SiteId={siteId}&group=OAuth2` |
| MySQL / SQL Server 连接 | 站点设置 → 服务集成 → Database | `/_Admin/system/config?SiteId={siteId}&group=Database` |
| 对象存储 | 站点设置 → 服务集成 → Storage | `/_Admin/system/config?SiteId={siteId}&group=Storage` |

### 站点设置

| 场景 | 深链 |
|------|------|
| 基础设置（含多语言） | `/_Admin/system/settings?SiteId={siteId}&name=basic` |
| SEO | `/_Admin/system/settings?SiteId={siteId}&name=seo` |
| 访问控制 | `/_Admin/system/settings?SiteId={siteId}&name=access` |
| 性能 | `/_Admin/system/settings?SiteId={siteId}&name=performance` |
| 高级（SQL 日志开关等） | `/_Admin/system/settings?SiteId={siteId}&name=advance` |
| 域名管理 | `/_Admin/system/domains?SiteId={siteId}` |
| Cookie 管理 | `/_Admin/system/cookie?SiteId={siteId}` |
| AI 服务 | `/_Admin/system/ai-service?SiteId={siteId}` |

### 内容与多语言

| 场景 | 深链 |
|------|------|
| Label 多语言文案 | `/_Admin/content/labels?SiteId={siteId}` |
| 数据类型 | `/_Admin/content/contentTypes?SiteId={siteId}` |
| 内容夹与条目 | `/_Admin/content/contents?SiteId={siteId}` |
| HTML 片段 | `/_Admin/content/htmlblocks?SiteId={siteId}` |

### 开发

| 场景 | 深链 |
|------|------|
| 身份验证规则 | `/_Admin/development/authentication?SiteId={siteId}` |
| 代码（API / CodeBlock） | `/_Admin/development/code?SiteId={siteId}` |
| 代码日志 | `/_Admin/development/code-log?SiteId={siteId}` |
| 代码搜索 | `/_Admin/development/code-search?SiteId={siteId}` |
| 表单 | `/_Admin/development/forms?SiteId={siteId}` |
| URL 路由 | `/_Admin/development/urls?SiteId={siteId}` |
| Open API | `/_Admin/development/openapis?SiteId={siteId}` |
| SPA 多语言 | `/_Admin/development/spamultilingual?SiteId={siteId}` |
| 定时任务 | `/_Admin/development/jobs?SiteId={siteId}` |

### 电商

| 场景 | 深链 |
|------|------|
| 货币 | `/_Admin/commerce/currencies?SiteId={siteId}` |
| 电商全局设置 | `/_Admin/commerce/settings?SiteId={siteId}` |
| 商品管理 | `/_Admin/commerce/product-management?SiteId={siteId}` |
| 订单 | `/_Admin/commerce/orders?SiteId={siteId}` |
| 会员等级 | `/_Admin/commerce/loyalty?SiteId={siteId}` |

### 站点页面

| 场景 | 深链 |
|------|------|
| 控制面板 | `/_Admin/site?SiteId={siteId}` |
| 页面列表 | `/_Admin/site/pages?SiteId={siteId}` |
| 媒体库 | `/_Admin/site/media?SiteId={siteId}` |

## 示例

JWT 配置（注意 `group=Others`）：

```json
{
  "message": "JWT 签发与校验依赖站点 JwtSetting，请先在后台配置 Secret。",
  "label": "配置 JWT",
  "url": "/_Admin/system/config?SiteId={siteId}&group=Others"
}
```

微信支付配置（注意 `group=Payment`，不是电商设置）：

```json
{
  "message": "微信支付商户密钥在「服务集成 → Payment」中配置。",
  "label": "配置微信支付",
  "url": "/_Admin/system/config?SiteId={siteId}&group=Payment"
}
```

多语言与 Label：

```json
{
  "message": "请先在基础设置开启多语言，再在 Label 页维护各语言文案。",
  "label": "开启多语言",
  "url": "/_Admin/system/settings?SiteId={siteId}&name=basic"
}
```

## 完整路径索引

其余菜单与子路由见 [references/admin-urls.md](references/admin-urls.md)（来源：Kooboo CMS 官方文档 `navigation.md` 与各模块深链）。
