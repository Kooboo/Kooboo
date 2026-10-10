# 站点设置

> 菜单：左侧 **站点设置** 分组（须在 **编辑菜单** 中勾选显示）
> 路由前缀：`/_Admin/system/...?SiteId={站点GUID}`

**站点设置**用于配置站点运行所需的基础信息、域名、第三方 **服务集成**、后台用户与权限、Hook 与 Cookie 等。与 [运维中心](../operations/index.md) 共用 `/system` 路径前缀，但 **左侧菜单分组不同**（本分组 `menu.name` 为 `system`，文案为「站点设置」）。

::: info 高级菜单
整个 **站点设置** 分组在路由上标记为 `advanced: true`，默认不在左侧显示，须在 [编辑菜单](../navigation.md#权限与编辑菜单) 中勾选。其中 **角色权限**、**请求 Hook**、**操作 Hook**、**AI 服务** 子项亦为 `advanced`。
:::

::: tip 与运维中心的重叠
| 菜单 | 路径 | 说明 |
|------|------|------|
| **AI 服务** | `/system/ai-service` | 与运维中心同名入口为 **同一页面**；文档只维护 [AI 服务](./ai-service.md) 一份，两处互相链接 |

运维侧其余菜单（访问日志、协作同步等）见 [运维中心](../operations/index.md)。
:::

## 如何打开

1. 在 [编辑菜单](../navigation.md#权限与编辑菜单) 中勾选 **站点设置** 及其需要的子项。
2. 左侧展开 **站点设置**。

## 子菜单（与后台顺序一致）

| 菜单 | 路径 | 权限（feature） | 文档 | 状态 |
|------|------|-----------------|------|------|
| 基础设置 | `/system/settings` | `site·edit` | [基础设置](./basic/) | ✅ |
| 域名管理 | `/system/domains` | `domain` | [域名管理](./domains.md) | ✅ |
| 服务集成 | `/system/config` | `config` | [服务集成](./integrations/) | ✅ |
| 用户管理 | `/system/siteuser` | `siteUser` | [站点用户](./site-users.md) | ✅ |
| 角色权限 | `/system/roles` | `role` | [角色权限](./roles.md) | ✅ |
| 请求 Hook | `/system/front-events` | `frontEvents` | [请求 Hook](./request-hooks.md) | ✅ |
| 操作 Hook | `/system/backend-events` | `backendEvents` | [操作 Hook](./action-hooks.md) | ✅ |
| Cookie 管理 | `/system/cookie` | `site·edit` | [Cookie](./cookie.md) | ✅ |
| AI 服务 | `/system/ai-service` | `ai` | [AI 服务](./ai-service.md) | ✅ |

深链示例：

```text
/_Admin/system/settings?SiteId={站点GUID}&name=basic
/_Admin/system/config?SiteId={站点GUID}&group=Database
/_Admin/system/front-events/edit?SiteId={站点GUID}&name=RouteFinding&display=RouteFinding
/_Admin/system/backend-events/edit?SiteId={站点GUID}&name=ContentUpdated&display=ContentUpdated
```

## 无侧栏菜单的子路由

| 路由 | 从哪进入 | 写入文档 |
|------|----------|----------|
| `/system/settings?name={tab}` | 基础设置内 Tab 切换 | [基础设置](./basic/) 各 Tab 子篇 |
| `/system/front-events/edit` | 请求 Hook 列表 | [请求 Hook](./request-hooks.md) |
| `/system/backend-events/edit` | 操作 Hook 列表 | [操作 Hook](./action-hooks.md) |

### 基础设置 Tab（`name` 查询参数）

| `name` | Tab 文案 | 主要内容 |
|--------|----------|----------|
| `basic` | 基础 | 显示名、Base URL、强制 SSL、SPA、**多语言**、导入/导出站点包 |
| `access` | 访问控制 | IP / 地区限制等 |
| `performance` | 性能 | 缓存、压缩等站点性能项 |
| `seo` | SEO | 站点级 SEO 相关开关 |
| `editor` | 编辑器 | 后台编辑器偏好 |
| `advance` | 高级 | 默认数据库、SQL 日志、约束修复等（与 [SQL 日志](../database/sql-logs.md) 联动） |

多语言配置嵌在 **基础** Tab 的 `Multilingual` 区域，**不单独成菜单**；文档写在 [基础 · 基础 Tab](./basic/general.md#多语言)，勿与 [开发 → SPA 多语言](../development/spa-multilingual.md) 混淆。

## 服务集成（`config`）分组预览

**服务集成**页按 **Group** 折叠展示 `CoreSetting` 项；详见 [服务集成](./integrations/)。

| Group | 文档 |
|-------|------|
| **Database** | [Database](./integrations/database.md) |
| **Payment** | [Payment](./integrations/payment.md) |
| **SMS** | [SMS](./integrations/sms.md) |
| **OAuth2** | [OAuth2](./integrations/oauth2.md) |
| **Storage** | [Storage](./integrations/storage.md) |
| **Others** | [Others](./integrations/others.md) |

支付类集成 **不在** 电商设置里重复配置，以 **Payment** 分组为准。

## 建议配置顺序

与实施常见路径一致，建议按下列顺序完成站点设置：

1. **[基础设置](./basic/)** — 六 Tab 子篇（含多语言、SQL 日志开关等）
2. **[域名管理](./domains.md)** — 绑定、主域名、SSL
3. **[服务集成](./integrations/)** — 各 Group 子篇；Database / Payment 与 API 交叉链接
4. **[站点用户](./site-users.md)** · **[角色权限](./roles.md)** — 后台账号与 feature 权限
5. **[请求 Hook](./request-hooks.md)** · **[操作 Hook](./action-hooks.md)** — 含 edit 子页
6. **[Cookie](./cookie.md)**
7. **[AI 服务](./ai-service.md)** — 与 [运维中心](../operations/index.md) 互链

## 开发参考（KScript / API）

| 后台能力 | API 文档 |
|----------|----------|
| 服务集成 CoreSetting | `k.config` / 站点设置相关 API（按项补充） |
| 支付 | [k.payment](../../api/payment/index.md) |
| 数据库连接 | [k.DB](../../api/database/index.md) |
| 多语言 / Label | [k.label](../../api/label/index.md) |
| JWT 等 | 集成项对应 API |

CMS 正文写 **后台点击路径** 与表单字段；脚本签名与示例留在 API 文档。

## 相关

| 文档 | 说明 |
|------|------|
| [站点后台菜单总览](../navigation.md) | 全站菜单树 |
| [数据库](../database/index.md) | 使用 Database 集成后的表管理 |
| [运维中心](../operations/index.md) | 同前缀 `/system` 的观测与协作 |
