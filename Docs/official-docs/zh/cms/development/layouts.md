# 布局（Layouts）

> 菜单：**开发 → 布局**  
> 深链：`/_Admin/development/layouts?SiteId={站点GUID}`

**布局（Layout）** 定义站点的 **页面外壳**（如 HTML 骨架、`@RenderBody()` 占位、公共头尾）。页面在 [页面设置](../site/pages-setting.md) 或页面属性中绑定 Layout；模板语法见 [Layout 模板引擎](../../templateEngine/layout/index.md)。

::: tip 权限
菜单权限：`layout`（查看）。新建、编辑、删除、复制需 `layout` · `edit` / `delete`。列表行末 **版本** 图标需 `site` · `log`。
:::

## 如何打开

1. 在 [编辑菜单](../navigation.md#权限与编辑菜单) 中勾选 **开发 → 布局**。
2. 左侧 **开发 → 布局**。

<DocImage src="/cms/development/layouts-overview.png" alt="布局列表总览" width="1120" />

## 工具栏

| 按钮 | 权限 | 说明 |
|------|------|------|
| **新建布局** | `layout` · `edit` | 进入 [编辑页](#编辑布局)（新建，无 `id`） |

<DocImage src="/cms/development/layouts-toolbar.png" alt="布局列表工具栏：新建布局" width="1120" />

## 列表表格

| 列 | 说明 |
|----|------|
| **名称** | 点击进入编辑；保存后名称在编辑页不可再改 |
| **被引用** | 展示引用本 Layout 的页面等关系标签 |
| **最后修改** | 最近保存时间 |
| **版本** | 打开该对象的 [版本历史](../operations/site-logs.md)（`site` · `log`） |

与 [组件](./views.md) 列表不同，布局列表 **无预览列**（预览在绑定 Layout 的页面上完成）。

支持多选后批量 **删除**（`layout` · `delete`）。选中 **一行** 时，表格工具栏出现 **复制**（见 [复制布局弹窗](#复制布局弹窗)）。

<DocImage src="/cms/development/layouts-list-table.png" alt="布局列表表格" width="1120" />

### 复制布局弹窗

选中一行后点 **复制**，填写新 **名称**（默认带 `_Copy` 后缀），确认后生成副本并刷新列表。

<DocImage src="/cms/development/layouts-copy-dialog.png" alt="复制布局弹窗" width="1120" />

## 编辑布局

路径：`/_Admin/development/layout/edit?SiteId=...`（新建无 `id`；编辑带 `&id={LayoutId}`）

从列表点名称或 **新建布局** 进入。使用全屏 **开发编辑** 布局：页头为 **布局名称** 与 **格式化**，中部为 HTML 编辑器，底部为保存栏。布局编辑 **没有** 组件页那样的 **设置** 弹窗（无「作为页面元素」、缓存等项）。

<DocImage src="/cms/development/layouts-edit-overview.png" alt="布局编辑：Monaco 编辑器与底栏" width="1120" />

### 页头

| 控件 | 说明 |
|------|------|
| **布局名称** | 新建时必填、须唯一（1～50 字符）；保存后输入框禁用 |
| **格式化** | 格式化编辑器内 HTML |

<DocImage src="/cms/development/layouts-edit-header.png" alt="布局编辑页头：名称与格式化" width="1120" />

### 代码编辑器

- 语言：**HTML**，支持 **KScript** 与代码引用（`code-reference-type: layout`）。
- 典型正文包含页面主体占位（如 `@RenderBody()`），具体写法见 [Layout 模板引擎](../../templateEngine/layout/index.md)。
- 快捷键：**保存**；离开前未保存会提示。

### 底栏

| 按钮 | 说明 |
|------|------|
| **返回** | 回到布局列表 |
| **保存** | 保存；新建首次保存后 URL 带上 `id` |
| **保存并返回** | 保存后返回列表 |

## 相关

| 文档 | 说明 |
|------|------|
| [开发概述](./index.md) | 开发菜单与子项索引 |
| [组件（Views）](./views.md) | 可复用 View 片段 |
| [页面](../site/pages.md) | 页面树与绑定 Layout |
| [页面设置](../site/pages-setting.md) | 页面默认 Layout 等 |
| [Layout 模板引擎](../../templateEngine/layout/index.md) | Layout 语法与概念 |
