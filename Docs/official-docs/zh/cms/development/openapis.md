# Open API

> 菜单：**开发 → Open API**  
> 深链：`/_Admin/development/openapis?SiteId={站点GUID}`

将外部服务的 **OpenAPI（Swagger）文档** 导入站点，配置鉴权凭据与响应缓存，并在 KScript 中通过 **`k.openApi.{名称}`** 调用远端 HTTP 接口。与 [代码](./code.md) 里为本站生成的 Swagger（`codeOpenApi`）不同：本功能面向 **第三方 API** 聚合。

::: tip 权限
列表：`openApi`（查看）。**创建**、编辑定义与授权、**更新文档**（URL 类型）需 `openApi·edit`；多选 **删除** 需 `openApi·delete`。
:::

::: info 与 k.net 的区别
| 方式 | 适用 |
|------|------|
| **Open API**（本页） | 已有 OpenAPI 规范、需 OAuth/API Key、按操作名调用、可缓存 |
| **[k.net.httpClient](../../api/net/httpclient.md)** | 手写 URL、headers，无规范文档 |
:::

## 如何打开

1. 在 [编辑菜单](../navigation.md#权限与编辑菜单) 中勾选 **开发 → Open API**。
2. 左侧 **开发 → Open API**。

<DocImage src="/cms/development/openapis-overview.png" alt="Open API 列表总览" width="1120" />

## 定义列表

### 工具栏

| 按钮 | 权限 | 说明 |
|------|------|------|
| **创建** | `openApi·edit` | 进入 [编辑 Open API](#编辑-open-api)（新建） |

<DocImage src="/cms/development/openapis-toolbar.png" alt="Open API 工具栏" width="1120" />

### 列表表格

| 列 | 说明 |
|----|------|
| **名称** | 唯一标识；保存后不可改名；对应脚本中的 `k.openApi.{名称}` |
| **类型** | `url` / `code` / `template`（见 [编辑页类型](#类型新建时可选)） |
| **URL** | 规范来源地址或模板详情链接 |
| **最后修改** | 保存时间 |
| **操作** | 见下表 |

| 图标 | 条件 | 说明 |
|------|------|------|
| **更新文档** | 类型为 `url` | 从 **URL** 重新下载 OpenAPI JSON 并写回站点 |
| **授权** | 始终 | 进入 [授权配置](#授权配置) |
| **编辑** | 始终 | 进入 [编辑 Open API](#编辑-open-api) |

支持多选 **删除**（`openApi·delete`）；会同时删除该定义下所有 **授权** 记录。

<DocImage src="/cms/development/openapis-list-table.png" alt="Open API 列表" width="1120" />

## 编辑 Open API

路径：`/_Admin/development/openapi/edit?SiteId=...`（新建无 `id`；编辑带 `id={OpenApiId}`）

底部栏 **保存** / **返回**；离开未保存时提示。新建保存后返回列表。

<DocImage src="/cms/development/openapis-edit.png" alt="编辑 Open API" width="1120" />

### 通用字段

| 字段 | 说明 |
|------|------|
| **名称** | 必填、唯一，1～50 字符，字母或数字开头；**编辑时只读** |
| **Base URL** | 可选；覆盖规范内 `servers` 的基址（绝对 HTTP(S) URL） |
| **缓存** | 按 **HTTP 方法** + **路径模式** 缓存响应，见 [响应缓存](#响应缓存) |
| **使用逗号数组** | 查询参数中数组序列化为逗号分隔单值（否则重复 key） |
| **自定义身份验证** | 开关后显示 KScript 编辑器；在发请求前注入 header/query/cookie 等 |

**自定义身份验证** 脚本中可使用全局 `request` 对象（只读结构由运行时注入），字段包括 `body`、`querys`、`paths`、`headers`、`cookies`。示例（编辑器默认注释）：

```typescript
// request.headers.Authorization = 'bearer xxxx'
// request.querys.token = 'xxxx'
```

### 类型（新建时可选）

创建后 **类型不可再改**。

| 类型 | 说明 |
|------|------|
| **URL** | 填写 OpenAPI JSON 的 **URL**（如 `https://petstore3.swagger.io/api/v3/openapi.json`），保存时下载并规范化 JSON |
| **代码** | 在 Monaco 中直接粘贴 **OpenAPI JSON** 正文（`jsonData`） |
| **模板** | 从 Kooboo **模板库**（`openapi_template.kooboo.net`）选择；自动填充 `baseUrl`、`authUrl`、文档与可选示例代码 |

<DocImage src="/cms/development/openapis-edit-url.png" alt="类型 URL：规范地址" width="1120" />

<DocImage src="/cms/development/openapis-edit-code.png" alt="类型代码：JSON 正文" width="1120" />

### 响应缓存

每条缓存规则：

| 字段 | 说明 |
|------|------|
| **方法** | `GET` / `POST` / … / `ALL` |
| **路径模式** | 与规范中 path 匹配的片段（`Contains` 匹配） |
| **过期时间** | 分钟数 |

命中缓存时在内存中返回上次响应，直至过期。

<DocImage src="/cms/development/openapis-edit-cache.png" alt="响应缓存规则" width="1120" />

## 授权配置

路径：`/_Admin/development/openapi/authorizes?SiteId=...&id={OpenApiId}`

从列表 **授权**（锁图标）进入。面包屑：**OpenApis → Authorizes**。

为当前 Open API 定义维护多组 **授权名称**（如 `production`、`sandbox`），每组保存规范 `securitySchemes` 所需的凭据。脚本调用远端接口前须先在此配置（或通过 `k.openApi.{名}.authorize` 写入），否则会报 **Api not authorize**。

### 工具栏

| 按钮 | 说明 |
|------|------|
| **创建** | 新建授权记录 |

### 列表

| 列 | 说明 |
|----|------|
| **名称** | `authorizeName`，脚本中选择使用哪一组凭据 |
| **最后修改** | 保存时间 |
| **操作** | **挑战**（OAuth 等一键跳转）、**编辑** |

<DocImage src="/cms/development/openapis-authorizes.png" alt="授权列表" width="1120" />

### 新建/编辑授权弹窗

| 字段 | 说明 |
|------|------|
| **名称** | 授权配置名（新建必填、唯一） |
| **安全方案** | 根据 OpenAPI `components.securitySchemes` 动态生成表单项 |

常见凭据字段（按方案类型显示）：

| 方案 | 字段 |
|------|------|
| **HTTP Basic** | `username`、`password` |
| **Bearer / API Key** | `accessToken`、`name`（如 `Bearer` 前缀） |
| **OAuth2** | `clientId`、`clientSecret`、`accessToken`、`refreshToken`、`expiresIn` 等 |

**挑战** 按钮：对配置了 `authUrl` 或 OAuth2 `authorizationCode` 流的定义，打开浏览器完成授权；回调由 Kooboo `/_api/openapioauth2callback/...` 或 `/_api/OpenApi/SaveToken` 处理并写回令牌。

<DocImage src="/cms/development/openapis-authorize-dialog.png" alt="编辑授权弹窗" width="1120" />

## 在站点中使用

1. 在本页 **创建** Open API 并保存（确保 `jsonData` 有效）。
2. 在 [授权配置](#授权配置) 中至少添加一组凭据。
3. 在 Layout / Page / Code 的服务端脚本中调用，例如：

```javascript
// 名称与后台「名称」列一致；操作名见 k.openApi API 文档
const result = k.openApi.myService.somePath_get({ query: { page: 1 } })
```

操作名、参数结构与 IDE 中针对该站生成的 `OpenApi` 类型定义一致，详见 **[k.openApi](../../api/openapi/index.md)**。

## 相关

| 文档 | 说明 |
|------|------|
| [k.openApi](../../api/openapi/index.md) | KScript：调用操作、`authorize` 管理凭据 |
| [k.net.httpClient](../../api/net/httpclient.md) | 通用 HTTP 客户端 |
| [k.security.jwt](../../api/security/index.md) | JWT（与规范中的 Bearer 配合） |
| [代码](./code.md) | 本站 API / CodeBlock |
| [身份验证](./authentication.md) | 进站请求鉴权（非调用外部 API） |
| [开发概述](./index.md) | 开发分组索引 |
