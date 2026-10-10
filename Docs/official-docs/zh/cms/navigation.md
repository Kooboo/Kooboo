# 后台菜单总览

> 按 Kooboo 站点后台左侧菜单归纳。界面文案以你登录后的中文界面为准。

::: tip 先完成外层步骤
若你仍在 **我的站点** 列表（`/_Admin/`），请先阅读 [登录与站点列表](./getting-started/login-and-site-list.md)，点击某一站点的 **管理** 后再使用下文菜单。
:::

## 账户级（无 SiteId）

| 页面 | 路径 |
|------|------|
| 我的站点 | `/` |
| 登录 | `/login` |
| 新建站点 | `/create` |
| 导入站点 | `/create/import` |
| 克隆站点 | `/create/clone` |
| 文件夹内列表 | `/?currentFolder={文件夹名}` |

说明见 [登录与站点列表](./getting-started/login-and-site-list.md)、[新建站点](./getting-started/create-site.md)。

## 站点级深链格式

```text
/_Admin{路由路径}?SiteId={站点GUID}
```

将 `{站点GUID}` 换成地址栏中的 `SiteId`。下表「路径」均相对于 `/_Admin`（且需带 `SiteId`）。

## 顶栏与常用入口

| 入口 | 路径 | 说明 |
|------|------|------|
| 控制面板（首页） | `/site` | 站点概览，见 [控制面板](./site/dashboard.md) |
| 媒体库 | `/site/media` | 图片与媒体资源，见 [媒体库](./site/media.md) |
| 页面 | `/site/pages` | 页面列表与编辑，见 [页面](./site/pages.md) |

## 内容

左侧分组：**内容**（部分子项需在 **编辑菜单** 中勾选后才显示）

| 菜单 | 路径 | 说明 |
|------|------|------|
| 内容 | `/content/contents` | 内容夹与条目，对应 `k.content.{文件夹名}` |
| 数据类型 | `/content/contentTypes` | 字段模型，须先于内容夹创建 |
| 标签 | `/content/labels` | 多语言文案，对应 `k.label` / `k.t` |
| HTML 片段 | `/content/htmlblocks` | 可复用 HTML 块，见 [HTML 片段](./content/html-blocks.md) |
| 文件 | `/content/files` | CMS 文件库，见 [文件](./content/files.md) |
| 元素属性 | `/content/text` | 模板 `k-config`，见 [元素属性](./content/tag-attributes.md) |
| 参数配置 | `/content/useroptions` | 结构化站点参数，见 [参数配置](./content/user-options.md) |

相关文档：[内容概述](./content/index.md) · [数据类型](./content/content-types.md) · [内容列表](./content/contents-folders-list.md) · [标签](./content/labels.md) · [HTML 片段](./content/html-blocks.md) · [文件](./content/files.md) · [元素属性](./content/tag-attributes.md) · [参数配置](./content/user-options.md)

## 电商

左侧分组：**电商**（`advanced`）

| 菜单 | 路径 | 文档 |
|------|------|------|
| 商品管理 | `/commerce/product-management` | [列表](./commerce/product-management.md) · [详情](./commerce/product-management-detail.md) |
| 商品类型 | `/commerce/product-types` | [商品类型](./commerce/product-types.md) |
| 商品分类 | `/commerce/product-categories` | [商品分类](./commerce/product-categories.md) |
| 购物车 | `/commerce/carts` | [购物车](./commerce/carts.md) |
| 顾客管理 | `/commerce/customers` | [顾客管理](./commerce/customers.md) |
| 订单 | `/commerce/orders` | [订单](./commerce/orders.md) |
| 销售统计 | `/commerce/sale-stats` | [销售统计](./commerce/sale-stats.md) |
| 优惠折扣 | `/commerce/discounts` | [优惠折扣](./commerce/discounts.md) |
| 货币 | `/commerce/currencies` | [货币](./commerce/currencies.md) |
| 配送 | `/commerce/shippings` | [配送](./commerce/shippings.md) |
| 会员 | `/commerce/loyalty` | [会员](./commerce/loyalty.md) |
| 税 | `/commerce/taxes` | [税](./commerce/taxes.md) |
| 通知 | `/commerce/notification` | [通知](./commerce/notification.md) |
| 设置 | `/commerce/settings` | [设置](./commerce/settings.md) |

概述：[电商](./commerce/index.md) · 开发参考：[k.commerce](../api/commerce/commerce.md)

## 运维中心

左侧分组：**运维中心**（路由前缀 `/system`，与「站点设置」不同分组）

| 菜单 | 路径 | 文档 |
|------|------|------|
| 访问统计 | `/system/visitor-logs` | [访问统计](./operations/visitor-logs.md) |
| 行为分析 | `/system/user-behavior-analysis` | 暂未单独成篇 |
| 资源守护 | `/system/resource-guardian` | [资源守护](./operations/resource-guardian.md) |
| 页面交互 | `/system/page-interaction` | [页面交互](./operations/page-interaction.md) |
| AB 测试 | `/system/ab-testing` | [AB 测试](./operations/ab-testing.md) |
| 错误监控 | `/system/error-monitoring` | 暂未单独成篇 |
| 操作日志 | `/system/site-logs` | [操作日志](./operations/site-logs.md) |
| 协作同步 | `/system/sync` | [协作同步](./operations/sync.md) |
| 搜索 | `/system/search` | 暂未单独成篇 |
| 自动化 | `/system/automation` | 暂未单独成篇 |
| AI 服务 | `/system/ai-service` | [AI 服务](./settings/ai-service.md) |

概述：[运维中心](./operations/index.md)

## 开发

左侧分组：**开发**（`advanced`）

| 菜单 | 路径 | 文档 |
|------|------|------|
| 组件（Views） | `/development/views` | [组件](./development/views.md) |
| 布局（Layouts） | `/development/layouts` | [布局](./development/layouts.md) |
| 脚本 | `/development/scripts` | [脚本](./development/scripts.md) |
| 样式 | `/development/styles` | [样式](./development/styles.md) |
| 代码 | `/development/code` | [代码](./development/code.md) |
| 代码日志 | `/development/code-log` | [代码日志](./development/code-log.md) |
| 代码搜索 | `/development/code-search` | [代码搜索](./development/code-search.md) |
| URL | `/development/urls` | [URL](./development/urls.md) |
| 表单 | `/development/forms` | [表单](./development/forms.md) |
| 菜单 | `/development/menus` | [菜单](./development/menus.md) |
| 身份验证 | `/development/authentication` | [身份验证](./development/authentication.md) |
| Open API | `/development/openapis` | [Open API](./development/openapis.md) |
| SPA 多语言 | `/development/spamultilingual` | [SPA 多语言](./development/spa-multilingual.md) |
| 模块 | `/development/modules` | [模块](./development/modules.md) |
| 定时任务 | `/development/jobs` | [定时任务](./development/jobs.md) |

概述：[开发](./development/index.md)（含模板引擎 / API 对照表）

::: info 模块注入的侧栏菜单
已上线模块可在 **`module.config`** 中配置 **`menu`**，将自定义入口挂到左侧某一 **顶级分组** 下。`parent` 填该分组在路由中的 **`menu.name`**（如 `content`、`commerce`、`development`、`system`、`database`）。详见 [模块 · 挂载站点后台菜单](./development/modules.md#挂载站点后台菜单-menu)。
:::

页面设计器路由（无左侧菜单项）：`/page/edit`、`/page/design` 等，从 [页面](./site/pages.md) 列表进入。

## 数据库

左侧分组：**数据库**（`advanced`）

| 菜单 | 路径 | 文档 |
|------|------|------|
| IndexedDB 表 | `/database/table` | [IndexedDB 表](./database/table.md) |
| IndexedDB 表关系 | `/database/table-relation` | [表关系](./database/table-relation.md) |
| 键值存储 | `/database/key-value` | [键值存储](./database/key-value.md) |
| SQLite 表 | `/database/sqlite-table` | [SQLite 表](./database/sqlite-table.md) |
| MySQL 表 | `/database/mysql-table` | [MySQL 表](./database/mysql-table.md) |
| SQL Server 表 | `/database/sqlserver-table` | [SQL Server 表](./database/sqlserver-table.md) |
| SQL 日志 | `/database/sql-logs` | [SQL 日志](./database/sql-logs.md) |

概述：[数据库](./database/index.md) · 开发参考：[k.DB](../api/database/index.md) · [k.DB.sqlite](../api/sqlite/index.md)

## 站点设置

左侧分组：**站点设置**（同为 `/system` 前缀，菜单名与「运维中心」不同）

| 菜单 | 路径 | 文档 |
|------|------|------|
| 基础设置 | `/system/settings` | [基础设置](./settings/basic/) |
| 域名管理 | `/system/domains` | [域名管理](./settings/domains.md) |
| 服务集成 | `/system/config` | [服务集成](./settings/integrations/) |
| 用户管理 | `/system/siteuser` | [站点用户](./settings/site-users.md) |
| 角色权限 | `/system/roles` | [角色权限](./settings/roles.md) |
| 请求 Hook | `/system/front-events` | [请求 Hook](./settings/request-hooks.md) |
| 操作 Hook | `/system/backend-events` | [操作 Hook](./settings/action-hooks.md) |
| Cookie 管理 | `/system/cookie` | [Cookie](./settings/cookie.md) |
| AI 服务 | `/system/ai-service` | [AI 服务](./settings/ai-service.md) |

概述：[站点设置](./settings/index.md) · 集成深链示例：`/_Admin/system/config?SiteId=...&group=Payment`

## 独立功能（非左侧主菜单）

| 功能 | 路径 |
|------|------|
| AI App Builder | `/ai-app-builder` |
| AI Chat | `/ai-chat/overview` |

## 权限与编辑菜单

- 子菜单是否可见，还取决于站点角色与功能权限（`meta.menu.permission`）。
- 路由上标有 `advanced: true` 的项（如 **文件**、**HTML 片段**、**标签属性** 等）默认**不显示**在左侧栏，需要在 **编辑菜单** 里勾选后才会出现。

### 如何打开编辑菜单

1. 进入站点后台任意页（如 [控制面板](./site/dashboard.md)）。
2. 点击左侧栏底部的 **编辑菜单**（铅笔图标，`editMenu`）。
3. 在弹出面板中勾选或取消各分组下的菜单项；提示文案为「将不常用的菜单隐藏」。
4. 关闭面板后，左侧菜单立即按你的选择显示或隐藏。

<DocImage src="/cms/navigation-edit-menu.png" alt="编辑菜单：勾选要在侧栏显示的内容、站点设置等子项" width="1120" />

勾选 **内容 → 文件**、**HTML 片段** 等后，即可在左侧 **内容** 分组下看到对应入口，详见各子文档。
