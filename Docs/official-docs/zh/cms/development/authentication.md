# 身份验证（Authentication）

> 菜单：**开发 → 身份验证**  
> 深链：`/_Admin/development/authentication?SiteId={站点GUID}`

为站点前台请求配置 **访问控制规则**：按 URL / HTTP 方法匹配请求，再执行 **JWT 校验** 或 **自定义 KScript**，未通过时可返回 HTTP 状态码或重定向。规则在站点页面/API 等已绑定路由的请求进入渲染前执行。

::: tip 权限
菜单权限：`authentication`（查看）。**创建**、编辑规则需 `authentication·edit`；列表多选 **删除** 需 `authentication·delete`。
:::

::: info 与站点账号登录的区别
本页控制的是 **访客请求是否允许继续渲染**（网关式规则），不是 Kooboo 后台账号登录，也不是 [k.account](../../api/getting-started/auth.md) 的站点账号登录流程。JWT 校验使用 [k.security.jwt](../../api/security/index.md) 解码请求中的令牌。
:::

## 如何打开

1. 在 [编辑菜单](../navigation.md#权限与编辑菜单) 中勾选 **开发 → 身份验证**。
2. 左侧 **开发 → 身份验证**。

<DocImage src="/cms/development/authentication-overview.png" alt="身份验证列表总览" width="1120" />

## 规则列表

### 工具栏

| 按钮 | 权限 | 说明 |
|------|------|------|
| **创建** | `authentication·edit` | 打开 [新建/编辑规则弹窗](#新建与编辑规则弹窗) |

<DocImage src="/cms/development/authentication-toolbar.png" alt="身份验证工具栏" width="1120" />

### 列表表格

| 列 | 说明 |
|----|------|
| **名称** | 规则唯一标识（新建后不可在此列表改名称，仅编辑弹窗内配置） |
| **匹配器** | `None` / `Any` / `Condition`；为 `Condition` 时显示条件摘要（如 `url = '/api'`） |
| **操作** | `None` / `JwtAuth` / `CustomCode` |
| **失败操作** | `None` / `ResultCode` / `Redirect`；操作为 `CustomCode` 时列显示 `CustomCode` |
| **参数** | 失败时相关参数：自定义代码名称、`httpCode`、重定向 `url` 等 |
| **最后修改** | 保存时间 |
| **操作** | **设置** 图标进入 [编辑](#新建与编辑规则弹窗) |

支持多选 **删除**（`authentication·delete`）。

<DocImage src="/cms/development/authentication-list-table.png" alt="身份验证列表" width="1120" />

## 新建与编辑规则弹窗

点击 **创建** 或行末 **设置** 打开。新建需填写 **名称**（1～50 字符，字母或数字开头、唯一）；编辑已有规则时名称不可改。

<DocImage src="/cms/development/authentication-edit-dialog.png" alt="身份验证规则编辑弹窗" width="1120" />

### 匹配器（Matcher）

| 值 | 说明 |
|----|------|
| **None** | 不参与匹配（通常用于占位，实际不会命中请求） |
| **Any** | 匹配所有进入规则链的请求 |
| **Condition** | 按下方条件匹配；多条条件为 **且（&&）** 关系，须全部满足 |

**Condition** 时每行包含：

| 字段 | 可选值 | 说明 |
|------|--------|------|
| 左侧 | `url`、`method` | 分别为请求相对路径、HTTP 方法 |
| 运算符 | `=`、`!=`、`contains`、`notcontains`、`startwith`、`notstartwith` | 比较时不区分大小写 |
| 右侧 | 文本 | 与左侧字段比较的值 |

可 **添加** / **删除** 条件行；至少保留一条有效条件，否则无法保存。

<DocImage src="/cms/development/authentication-condition.png" alt="Condition 匹配器与条件行" width="1120" />

### 操作（Action）

| 值 | 说明 |
|----|------|
| **None** | 匹配后放行，不做校验 |
| **JwtAuth** | 使用 JWT 解码校验；失败时执行下方 **失败操作** |
| **CustomCode** | 执行内联 **TypeScript/KScript**（Monaco 编辑器）；失败时由脚本设置响应状态 |

选择 **CustomCode** 时显示代码编辑器，默认含示例：检查 `Authorization` 头并调用 `k.response.unauthorized()`。保存的是内联 `customCode` 正文（亦可与站点 **代码** 资源关联，列表加载时可能回填 `customCodeName` 对应正文）。

选择 **JwtAuth** 时显示 **失败操作** 区块（见下）。

### 失败操作（FailedAction）

仅在 **操作** 为 **JwtAuth** 时出现（`CustomCode` 由脚本自行处理响应）。

| 值 | 说明 |
|----|------|
| **None** | JWT 校验失败时，将解码结果中的 `value` 写入响应正文 |
| **ResultCode** | 返回指定 **HTTP 状态码**（如 `401`） |
| **Redirect** | **302** 重定向到填写的 **URL**；自动附带 `returnurl` 查询参数（当前请求路径） |

| 附加字段 | 何时显示 | 说明 |
|----------|----------|------|
| **HTTP 状态码** | `ResultCode` | 必填，合法 HTTP 状态码 |
| **URL** | `Redirect` | 必填，重定向目标（输入时自动去除空格） |

## 规则如何生效

- 仅对 **已绑定站点路由** 的请求生效（有 `Route.objectId` 的页面、API 等）；纯静态或未匹配路由的请求不经过此列表。
- 系统按站点内规则 **列表顺序** 依次尝试匹配；**第一条** 匹配成功的规则被执行，后续规则不再检查。多条规则并存时请把更具体的 `Condition` 排在前面，或避免重叠的 `Any`。
- `JwtAuth`：解码成功且结果 `code !== 1` 时放行，并将 `value` 放入请求上下文 `jwt_payload`；`code === 1` 视为失败，按 **失败操作** 处理。
- `CustomCode`：执行脚本后若响应状态码 **小于 300** 则放行，否则请求终止（中间件可能返回 **403**）。
- 校验未通过且未写入完整响应时，可能返回 **403**。

自定义代码中常用 API：[k.request](../../api/request/index.md)、[k.response](../../api/response/index.md)（如 `unauthorized()`）、[k.security.jwt](../../api/security/index.md)。

## 相关

| 文档 | 说明 |
|------|------|
| [开发概述](./index.md) | 开发分组索引 |
| [代码](./code.md) | CodeBlock，可与 CustomCode 配合 |
| [k.security.jwt](../../api/security/index.md) | JWT 编解码 |
| [k.response](../../api/response/index.md) | `unauthorized()`、重定向等 |
| [认证与授权（API）](../../api/getting-started/auth.md) | `k.account` 会员登录（非本页规则） |
