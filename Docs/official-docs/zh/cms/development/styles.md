# 样式（Styles）

> 菜单：**开发 → 样式**  
> 深链：`/_Admin/development/styles?SiteId={站点GUID}`

管理站点 **CSS** 资源：独立 `.css` 文件（外部）、页面/布局等 **内嵌样式**、设计器产生的 **行内样式规则**，以及合并多条外部样式的 **样式组**。引用方式见 [Style 模板引擎](../../templateEngine/css/index.md)。

::: tip 权限
列表与编辑：`style`（查看 / `edit` / `delete`）。页签栏 **设置**（站点级压缩、UnoCSS、按媒体拆分等）需 `site · edit`。行末 **版本** 需 `site· log`。
:::

## 如何打开

1. 在 [编辑菜单](../navigation.md#权限与编辑菜单) 中勾选 **开发 → 样式**。
2. 左侧 **开发 → 样式**。

<DocImage src="/cms/development/styles-overview.png" alt="样式页：外部页签总览" width="1120" />

## 页签栏与站点设置

| 页签 | query `name` | 说明 |
|------|----------------|------|
| **外部** | `external`（默认） | 独立 CSS 文件，可新建、上传、在线搜索 |
| **内嵌** | `embedded` | 随页面、Layout 等保存的 CSS，仅编辑与删除 |
| **行内样式** | `inline` | 页面设计器写入的 CSS 规则块，在弹窗中编辑声明 |
| **群组** | `group` | 多个外部样式合并为一条引用 |

页签右侧 **设置** 打开 [样式站点设置弹窗](#样式站点设置弹窗)。

<DocImage src="/cms/development/styles-tabs.png" alt="样式页签与设置" width="1120" />

## 外部页签

### 工具栏

| 按钮 | 权限 | 说明 |
|------|------|------|
| **新建样式** | `style` · `edit` | 进入 [编辑样式](#编辑样式) |
| **上传样式** | `style` · `edit` | 上传 `text/css` 文件；重名时确认覆盖 |
| **搜索** | `style` · `edit` | 在线包搜索，安装后刷新列表 |

<DocImage src="/cms/development/styles-external-toolbar.png" alt="外部样式工具栏" width="1120" />

### 列表表格

| 列 | 说明 |
|----|------|
| **名称** | 进入编辑 |
| **预览** | 路由路径，新标签打开样式 URL |
| **被引用** | 引用关系 |
| **最后修改** | 保存时间 |
| **版本** | [版本历史](../operations/site-logs.md) |

支持多选 **删除**。

<DocImage src="/cms/development/styles-external-table.png" alt="外部样式列表" width="1120" />

### 样式站点设置弹窗

**设置** 保存站点级样式投递与工具链选项：

| 区域 | 字段 / 说明 |
|------|-------------|
| **JS/CSS 压缩** | `enableJsCssCompress` |
| **JS/CSS 浏览器缓存** | `enableJsCssBrowerCache` |
| **原子化 CSS (unocss)** | `unocssSettings.enable`；开启后可配置 **禁用 SSR**、**重置默认样式**、JSON **配置**（须合法 JSON） |
| **按媒体查询拆分 CSS** | `enableCssSplitByMedia`；开启后设置移动端 **最大宽度**、桌面端 **最小宽度**（用于拆分/裁剪媒体规则） |

<DocImage src="/cms/development/styles-settings-dialog.png" alt="样式站点设置弹窗" width="1120" />

## 内嵌页签

列出 **内嵌样式**。无新建工具栏；点名称进入编辑。编辑页显示 **内嵌样式** 标题，不可改名称。

| 列 | 说明 |
|----|------|
| **名称** | 进入编辑 |
| **被引用** | 所属对象 |
| **最后修改** | 保存时间 |

<DocImage src="/cms/development/styles-embedded-table.png" alt="内嵌样式列表" width="1120" />

## 行内样式页签

管理 **CSS 规则**（选择器 + 声明列表），多来自可视化编辑。点 **样式** 列名称打开 [行内样式编辑弹窗](#行内样式编辑弹窗)。

| 列 | 说明 |
|----|------|
| **样式** | 规则名称/选择器摘要，点击编辑 |
| **所有者类型** | 如 Page、View 等（`ownerType`） |
| **站点对象** | 所属对象名称（`ownerName`） |
| **最后修改** | 保存时间 |

<DocImage src="/cms/development/styles-inline-table.png" alt="行内样式列表" width="1120" />

### 行内样式编辑弹窗

以 `{ ... }` 形式编辑多条 **属性 : 值**：

| 操作 | 说明 |
|------|------|
| 属性下拉 | 常用 CSS 属性，可筛选或自定义输入 |
| 值输入 | 属性值；颜色类属性旁有 **取色器**；背景图等可 **选媒体库图片** |
| **+** | 新增一条声明 |
| 删除 | 移除当前行 |

确认后写回该规则并刷新列表。

<DocImage src="/cms/development/styles-inline-dialog.png" alt="行内样式编辑弹窗" width="1120" />

## 样式群组页签

将多个 **外部** 样式合并为一条资源组（`typeName: Style`）。

### 工具栏

| 按钮 | 权限 | 说明 |
|------|------|------|
| **新建群组** | `style` · `edit` | 打开 [样式组弹窗](#样式组弹窗) |

<DocImage src="/cms/development/styles-group-toolbar.png" alt="样式群组工具栏" width="1120" />

### 列表表格

| 列 | 说明 |
|----|------|
| **名称** | 打开群组弹窗 |
| **子项** | 组内样式数量 |
| **预览** | 群组预览 URL |
| **被引用** | 引用关系 |
| **最后修改** | 保存时间 |

<DocImage src="/cms/development/styles-group-table.png" alt="样式群组列表" width="1120" />

### 样式组弹窗

| 区域 | 说明 |
|------|------|
| **名称** | 新建必填、唯一；保存后不可改名 |
| **添加样式** | 从未入组的外部样式中选择 |
| **列表** | 已加入样式，可排序、删除 |

<DocImage src="/cms/development/styles-group-dialog.png" alt="样式组弹窗" width="1120" />

## 编辑样式

路径：`/_Admin/development/style/edit?SiteId=...`

从 **外部** 页签 **新建样式** 或点名称进入。

<DocImage src="/cms/development/styles-edit-overview.png" alt="样式编辑：CSS 编辑器" width="1120" />

### 页头（外部样式）

| 控件 | 说明 |
|------|------|
| **样式名称** | 新建必填、唯一；保存后禁用 |
| **格式化** | 格式化 CSS（亦可用快捷键） |

内嵌样式仅 **内嵌样式** 标题 + **格式化**。

<DocImage src="/cms/development/styles-edit-header.png" alt="样式编辑页头" width="1120" />

### 代码编辑器

- 语言：**CSS**，支持 **KScript**；外部样式支持代码引用（`code-reference-type: style`）。
- **保存** / **保存并返回** / 离开未保存提示。

## 相关

| 文档 | 说明 |
|------|------|
| [开发概述](./index.md) | 开发菜单索引 |
| [脚本](./scripts.md) | 站点 JS，页签结构类似 |
| [布局](./layouts.md) | 可产生内嵌样式 |
| [Style 模板引擎](../../templateEngine/css/index.md) | 引用与语法 |
