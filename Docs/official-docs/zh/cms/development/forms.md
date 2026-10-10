# 表单（Forms）

> 菜单：**开发 → 表单**  
> 深链：`/_Admin/development/forms?SiteId={站点GUID}`

管理站点 **表单** 定义与提交数据：**外部** 表单可在此新建并编辑 HTML 模板；**内嵌** 表单随页面等对象创建，在此查看与配置。提交记录在 [表单数据](#表单数据子页) 中查看。

::: tip 权限
菜单权限：`form`（查看）。**创建表单**、编辑、设置、删除提交数据需 `form·edit` / `form·delete`（以表格/按钮为准）。外部列表 **版本** 需 `site·log`。
:::

## 如何打开

1. 在 [编辑菜单](../navigation.md#权限与编辑菜单) 中勾选 **开发 → 表单**。
2. 左侧 **开发 → 表单**。

默认 **外部** 页签；切换页签会更新 URL 的 `name=external|embedded`。

<DocImage src="/cms/development/forms-overview.png" alt="表单页总览（外部页签）" width="1120" />

## 页签

| 页签 | query `name` | 说明 |
|------|----------------|------|
| **外部** | `external`（默认） | 独立表单，可 **创建表单** |
| **内嵌** | `embedded` | 页面/模块内嵌表单，不可在此新建 |

<DocImage src="/cms/development/forms-tabs.png" alt="表单页签：外部与内嵌" width="1120" />

## 外部页签

### 工具栏

| 按钮 | 权限 | 说明 |
|------|------|------|
| **创建表单** | `form·edit` | 进入 [编辑表单](#编辑表单)（新建） |

<DocImage src="/cms/development/forms-external-toolbar.png" alt="外部表单工具栏" width="1120" />

### 列表表格

| 列 | 说明 |
|----|------|
| **名称** | 点击进入 [编辑表单](#编辑表单) |
| **数据** | 数字标签为提交条数，点击进入 [表单数据子页](#表单数据子页) |
| **被引用** | 引用关系 |
| **最后修改** | 保存时间 |
| **操作** | **设置**（[表单设置弹窗](#表单设置弹窗)）、**版本**（`site·log`） |

支持多选 **删除**（`form·delete`）。

<DocImage src="/cms/development/forms-external-table.png" alt="外部表单列表" width="1120" />

## 内嵌页签

无 **创建** 按钮。列表列与外部相同（**名称** 进入编辑时 URL 带 `type=embedded`；编辑页显示 **内嵌表单** 且不可改名称）。行末仅有 **设置**，无 **版本** 图标。

<DocImage src="/cms/development/forms-embedded-table.png" alt="内嵌表单列表" width="1120" />

### 表单设置弹窗

外部/内嵌列表点 **设置** 打开（**表单设置**）：

| 项 | 说明 |
|----|------|
| **启用** | 是否启用该表单的提交处理 |
| **方法** | `get` / `post` |
| **重定向到** | 提交后跳转：可选 **刷新页面**（`RefreshSelf()`）或站点 [页面](../site/pages.md) 路径，亦可手输 |
| **提交方式** | 从 `availableSubmitters` 选择（如提交到邮件、外部 URL 等） |
| 提交方式相关字段 | 例如 **SubmitToExternal** 时需填接收 URL；其它方式展示对应下拉配置项 |

## 编辑表单

路径：`/_Admin/development/form/edit?SiteId=...`（新建无 `id`；编辑带 `id`；内嵌带 `type=embedded`）

<DocImage src="/cms/development/forms-edit-overview.png" alt="表单编辑：HTML 编辑器" width="1120" />

### 页头

| 控件 | 说明 |
|------|------|
| **表单名称** | 仅 **新建外部** 表单可填且须唯一；保存后或内嵌表单不可改名 |
| **格式化** | 格式化 HTML 模板 |

<DocImage src="/cms/development/forms-edit-header.png" alt="表单编辑页头" width="1120" />

### 代码编辑器

- 语言：**HTML**，支持 **KScript**。
- **保存** / **保存并返回**；离开未保存会提示。

## 表单数据子页

路径：`/_Admin/development/form-values?SiteId=...&id={FormId}`

面包屑：**表单 → 数据**。表格列为各次提交中的 **字段名**（动态列），支持分页与多选 **删除**。

## 相关

| 文档 | 说明 |
|------|------|
| [开发概述](./index.md) | 开发菜单索引 |
| [页面](../site/pages.md) | 页面内嵌表单来源 |
| [URL](./urls.md) | 表单提交路径 |
