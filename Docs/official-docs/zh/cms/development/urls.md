# URL

> 菜单：**开发 → URL**  
> 深链：`/_Admin/development/urls?SiteId={站点GUID}`

管理站点的 **访问路径与路由映射**：**内部** URL（页面、视图、路由别名等）、**外部** URL（指向外部或远程资源的路径），以及访客访问过但尚未配置的 **未找到** 地址。与 [页面](../site/pages.md) 树中的路径配合使用。

::: tip 权限
菜单权限：`link`（查看）。**创建别名**、编辑内部/外部 URL 需 `link·edit`；删除内部 URL、删除外部 URL 分别受表格权限控制（内部删除为 `link·edit`，外部删除为 `link·delete`）。
:::

## 如何打开

1. 在 [编辑菜单](../navigation.md#权限与编辑菜单) 中勾选 **开发 → URL**。
2. 左侧 **开发 → URL**。

默认打开 **内部** 页签。

<DocImage src="/cms/development/urls-overview.png" alt="URL 管理页总览（内部页签）" width="1120" />

## 页签

| 页签 | 说明 |
|------|------|
| **内部** | 站点内资源对应的 URL（页面、Route 别名、视图等） |
| **外部** | 映射到外部 Script / Style / 图片等资源的 URL |
| **未找到** | 访客请求过、站点尚未配置的路由记录，可据此 **创建别名** |

<DocImage src="/cms/development/urls-tabs.png" alt="URL 页签：内部、外部、未找到" width="1120" />

## 内部页签

### 工具栏

| 控件 | 权限 | 说明 |
|------|------|------|
| **创建别名** | `link·edit` | 打开 [创建别名弹窗](#创建别名弹窗)，为任意 URL 指定重定向目标 |
| **资源类型** | — | 筛选 `resourceType` |
| **存在对象** | — | 是否已绑定站点对象（是/否） |
| **URL** 搜索 | — | 按路径关键字搜索，回车或搜索按钮刷新 |

<DocImage src="/cms/development/urls-internal-toolbar.png" alt="内部 URL 工具栏与筛选" width="1120" />

### 列表表格

分页（每页 30 条）。

| 列 | 说明 |
|----|------|
| **URL** | 访问路径 |
| **资源类型** | 如 Page、View、Route 等 |
| **语言** | `culture` |
| **存在对象** | 是否已关联对象 |
| **被引用** | 引用关系 |
| **最后修改** | 保存时间 |
| **操作** | **编辑**、**预览** |

已绑定对象且类型 **不是** `Route` 的行 **不可勾选** 批量删除（避免误删仍在使用的映射）。

支持多选删除（`link·edit`）。

<DocImage src="/cms/development/urls-internal-table.png" alt="内部 URL 列表" width="1120" />

### 编辑内部 URL 弹窗

点行内 **编辑** 打开（标题 **编辑 URL**）：

| 字段 | 说明 |
|------|------|
| **URL** | 路径，自动去除空格 |
| **重定向到** | 仅当资源类型为 **Route** 时：选择目标路由，下方展示路由 **参数** 标签 |

<DocImage src="/cms/development/urls-internal-edit-dialog.png" alt="编辑内部 URL 弹窗" width="1120" />

### 创建别名弹窗

**创建别名** 或 **未找到** 页签中 **创建别名** 均使用同一弹窗：

| 字段 | 说明 |
|------|------|
| **URL** | 别名路径（从未找到页进入时会预填访客请求的 URL） |
| **重定向到** | 选择已有路由；选中后展示该路由所需 **参数** 标签 |

<DocImage src="/cms/development/urls-make-alias-dialog.png" alt="创建别名弹窗" width="1120" />

## 外部页签

### 筛选

**资源类型**、**URL** 关键字（无「创建别名」按钮）。

<DocImage src="/cms/development/urls-external-toolbar.png" alt="外部 URL 筛选栏" width="1120" />

### 列表表格

| 列 | 说明 |
|----|------|
| **URL** | 外部路径 |
| **资源类型** | 如 Script、Style、Image 等 |
| **被引用** | 引用关系 |
| **最后修改** | 保存时间 |
| **操作** | **编辑**、**转为内部资源**（仅 Script / Style / Image）、**预览** |

**转为内部资源** 将外部映射收归站点内部管理（成功后提示「已转为内部资源」）。多选 **删除** 需 `link·delete`。

<DocImage src="/cms/development/urls-external-table.png" alt="外部 URL 列表" width="1120" />

### 编辑外部 URL 弹窗

点行内 **编辑** 打开（标题 **编辑 URL**）：仅可修改 **URL** 路径，无「重定向到」字段（与 [编辑内部 URL 弹窗](#编辑内部-url-弹窗) 不同）。

## 未找到页签

展示访客访问失败的路径记录（按时间）。

| 列 | 说明 |
|----|------|
| **URL** | 请求的地址 |
| **日期时间** | `startTime` |
| **操作** | **创建别名**（`link·edit`），打开 [创建别名弹窗](#创建别名弹窗) 并预填该 URL |

<DocImage src="/cms/development/urls-not-found-table.png" alt="未找到 URL 列表" width="1120" />

## 相关

| 文档 | 说明 |
|------|------|
| [开发概述](./index.md) | 开发菜单索引 |
| [页面](../site/pages.md) | 页面路径与发布 |
| [组件](./views.md) | View 路由 |
| [代码](./code.md) | API 路径（Code 类型 URL） |
