# 代码（Code）

> 菜单：**开发 → 代码**  
> 深链：`/_Admin/development/code?SiteId={站点GUID}`

管理站点 **KScript / TypeScript 代码** 资源：API、CodeBlock、事件处理、页面脚本等。类型由后台 `Code/CodeType` 接口返回，页签与列表按类型筛选。脚本逻辑与 API 见 [KScript API](../../api/index.md)（如 `k.site.codes`）。

::: tip 权限
列表与编辑：`code`（查看 / `edit` / `delete`）。**API** 页签的 **设置**（Swagger 文档开关）需 `code·edit`。行末 **版本** 需 `site·log`。
:::

## 如何打开

1. 在 [编辑菜单](../navigation.md#权限与编辑菜单) 中勾选 **开发 → 代码**。
2. 左侧 **开发 → 代码**。

<DocImage src="/cms/development/code-overview.png" alt="代码列表总览" width="1120" />

## 页头

面包屑 **代码**；右侧 **搜索** 按名称或 URL 关键字过滤（防抖）。有文件夹层级时，搜索会自动 **展开全部** 以便命中子项。

<DocImage src="/cms/development/code-header.png" alt="代码页头：面包屑与搜索" width="1120" />

## 类型页签

| 页签 | query `name` | 说明 |
|------|----------------|------|
| **全部** | `all`（默认） | 所有类型；**创建** 为下拉，可选具体类型 |
| 各代码类型 | 如 `api`、`codeblock`、`event`… | 仅显示该类型；**创建** 为单按钮，文案为「创建{type}」 |

切换页签会更新 URL 的 `name` 参数并清空当前多选。

<DocImage src="/cms/development/code-tabs.png" alt="代码类型页签" width="1120" />

## 工具栏

| 控件 | 权限 | 说明 |
|------|------|------|
| **创建** | `code·edit` | **全部** 页签：下拉选择类型后进入 [编辑代码](#编辑代码)；指定类型页签：直接按该类型新建 |
| **搜索** | `code·edit` | 在线包搜索（**全部**、**CodeBlock** 等页签显示） |
| **Swagger** | — | 仅 **API** 页签，且已在 [API 设置](#api-设置弹窗) 中启用文档时显示，新标签打开 `/_Admin/swagger` |
| **设置** | `code·edit` | 仅 **API** 页签，打开 API 设置弹窗 |
| **展开全部 / 折叠全部** | — | 列表按 URL 路径生成文件夹层级时出现 |

<DocImage src="/cms/development/code-toolbar.png" alt="代码工具栏：创建与搜索" width="1120" />

### 创建类型下拉

在 **全部** 页签点击 **创建**，从下拉选择要新建的类型（选项与类型页签一致）。

<DocImage src="/cms/development/code-create-dropdown.png" alt="创建代码类型下拉" width="120" />

## 列表表格

树形表格：带 `url` 路径的项会按路径分段显示为 **文件夹行**（可点击展开/折叠）；叶子为具体代码。

| 列 | 说明 |
|----|------|
| **名称** | 点击进入编辑；行首复选框参与批量删除 |
| **代码类型** | 类型标签（如 API、CodeBlock） |
| **脚本类型** | `Classic` / `Module`（部分类型不显示，如内嵌 PageScript、CodeBlock） |
| **被引用** | 引用关系 |
| **预览** | 访问路径 `url`，点击预览 |
| **最后修改** | 保存时间 |
| **版本** | [版本历史](../operations/site-logs.md) |

支持多选 **删除**。

<DocImage src="/cms/development/code-list-table.png" alt="代码列表表格" width="1120" />

### API 设置弹窗

在 **API** 页签点工具栏 **设置**：

| 项 | 说明 |
|----|------|
| **启用 Swagger 文档** | 写入 `codeOpenApiSettings.enable`；开启后显示 **Swagger** 按钮 |
| **身份验证** | `None` / `Cookie` / `Header` |
| **Token 名称** | 非 None 时可填（默认 `jwt_token`） |

确认按钮文案为 **开始**（保存站点配置）。

<DocImage src="/cms/development/code-api-settings-dialog.png" alt="API Swagger 设置弹窗" width="1120" />

## 编辑代码

路径：`/_Admin/development/code/edit?SiteId=...`（新建带 `type={CodeType}`；编辑带 `id`）

<DocImage src="/cms/development/code-edit-overview.png" alt="代码编辑：TypeScript 编辑器" width="1120" />

### 页头表单

随 **代码类型** 不同，字段略有差异：

| 字段 | 说明 |
|------|------|
| **代码名称** | 新建必填、唯一（1～50 字符）；保存后不可改名；改名时可同步默认 `url` 为 `/{name}` |
| **URL** | **API** 类型必填，为接口路由 |
| **事件类型** | **Event** 类型：按分类下拉选择事件名（新建可选，保存后不可改） |
| **脚本类型** | **Classic** / **Module**（内嵌 PageScript、CodeBlock 等类型不显示此项） |
| **格式化** | 格式化编辑器内容 |

<DocImage src="/cms/development/code-edit-header.png" alt="代码编辑页头表单" width="1120" />

### 代码编辑器

- 语言：**TypeScript**，支持 **KScript**。
- **API**、**CodeBlock** 等类型在保存后支持代码引用。
- **保存** / **保存并返回**；离开前未保存会提示。

### 已加密代码

若代码已加密（`isDecrypted`），编辑区覆盖提示 **此代码已加密，无法编辑**，底栏仅保留 **返回**。

## 相关

| 文档 | 说明 |
|------|------|
| [开发概述](./index.md) | 开发菜单索引 |
| [代码日志](./code-log.md) | 代码运行日志 |
| [代码搜索](./code-search.md) | 全站代码搜索 |
| [KScript API](../../api/index.md) | 脚本 API |
