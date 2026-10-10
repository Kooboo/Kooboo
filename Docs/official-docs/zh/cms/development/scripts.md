# 脚本（Scripts）

> 菜单：**开发 → 脚本**  
> 深链：`/_Admin/development/scripts?SiteId={站点GUID}`

管理站点 **JavaScript** 资源：独立 `.js` 文件（外部）、由页面/布局等生成的 **内嵌脚本**，以及将多个外部脚本合并引用的 **脚本群组**。语法与引用方式见 [Script 模板引擎](../../templateEngine/js/index.md)。

::: tip 权限
列表与编辑：`script`（查看 / `edit` / `delete`）。页签栏右侧 **设置**（站点级 JS/CSS 压缩与浏览器缓存）需 `site` · `edit`。行末 **版本** 需 `site` · `log`。
:::

## 如何打开

1. 在 [编辑菜单](../navigation.md#权限与编辑菜单) 中勾选 **开发 → 脚本**。
2. 左侧 **开发 → 脚本**。

<DocImage src="/cms/development/scripts-overview.png" alt="脚本页：外部页签总览" width="1120" />

## 页签栏与站点设置

| 页签 | 说明 |
|------|------|
| **外部** | 站点根目录下的独立脚本文件，可新建、上传、在线搜索安装 |
| **内嵌** | 随页面、Layout 等保存的内嵌脚本，仅可编辑正文与删除，不能在此新建 |
| **群组** | 将多个外部脚本组合为一条引用（见 [脚本群组](#脚本群组页签)） |

页签右侧 **设置** 图标打开 [脚本站点设置弹窗](#脚本站点设置弹窗)（写入站点配置，非单个脚本属性）。

<DocImage src="/cms/development/scripts-tabs.png" alt="脚本页签：外部、内嵌、群组与设置" width="1120" />

## 外部页签

默认进入 **外部** 页签。

### 工具栏

| 按钮 | 权限 | 说明 |
|------|------|------|
| **新建脚本** | `script` · `edit` | 进入 [编辑脚本](#编辑脚本)（新建） |
| **上传脚本** | `script` · `edit` | 选择本地 `.js` 等文件上传；重名时确认覆盖 |
| **搜索** | `script` · `edit` | 打开在线包搜索，安装社区/模板脚本后刷新列表 |

<DocImage src="/cms/development/scripts-external-toolbar.png" alt="外部脚本工具栏" width="1120" />

### 列表表格

| 列 | 说明 |
|----|------|
| **名称** | 点击进入编辑 |
| **预览** | 显示路由路径，点击在新标签打开脚本 URL |
| **被引用** | 引用关系标签 |
| **最后修改** | 保存时间 |
| **版本** | [版本历史](../operations/site-logs.md) |

支持多选 **删除**。

<DocImage src="/cms/development/scripts-external-table.png" alt="外部脚本列表" width="1120" />

### 脚本站点设置弹窗

**设置** → 两项站点开关（保存后影响全站脚本/样式投递）：

| 开关 | 字段 | 说明 |
|------|------|------|
| **JS/CSS 压缩** | `enableJsCssCompress` | 发布时压缩 JS/CSS |
| **JS/CSS 浏览器缓存** | `enableJsCssBrowerCache` | 启用浏览器缓存策略 |

<DocImage src="/cms/development/scripts-settings-dialog.png" alt="脚本站点设置：压缩与浏览器缓存" width="1120" />

## 内嵌页签

路径示例：`/_Admin/development/scripts?SiteId=...&name=embedded`

列出 **内嵌脚本**。无新建/上传工具栏；点 **名称** 进入编辑。编辑页页头显示 **内嵌脚本** 文案，**不显示** 可改名称的表单项（名称由所属页面/资源决定）。

| 列 | 说明 |
|----|------|
| **名称** | 进入编辑 |
| **被引用** | 所属页面等 |
| **最后修改** | 保存时间 |

<DocImage src="/cms/development/scripts-embedded-table.png" alt="内嵌脚本列表" width="1120" />

## 脚本群组页签

路径示例：`/_Admin/development/scripts?SiteId=...&name=group`

将多个 **外部** 脚本合并为一条资源组，页面中引用群组即可按顺序加载组内脚本。

### 工具栏

| 按钮 | 权限 | 说明 |
|------|------|------|
| **新建群组** | `script` · `edit` | 打开 [脚本群组弹窗](#脚本群组弹窗) |

<DocImage src="/cms/development/scripts-group-toolbar.png" alt="脚本群组工具栏" width="1120" />

### 列表表格

| 列 | 说明 |
|----|------|
| **名称** | 点击打开群组弹窗编辑 |
| **子项** | 组内脚本数量 |
| **预览** | 群组预览 URL |
| **被引用** | 引用关系 |
| **最后修改** | 保存时间 |

<DocImage src="/cms/development/scripts-group-table.png" alt="脚本群组列表" width="1120" />

### 脚本群组弹窗

新建或编辑群组：

| 区域 | 说明 |
|------|------|
| **名称** | 新建必填、须唯一；保存后不可改名 |
| **添加脚本** | 下拉选择尚未加入组的外部脚本（显示名称与路径） |
| **列表** | 已加入的脚本，可拖拽排序、删除项 |

确认后保存群组并刷新列表。

<DocImage src="/cms/development/scripts-group-dialog.png" alt="脚本群组弹窗：名称与脚本列表" width="1120" />

## 编辑脚本

路径：`/_Admin/development/script/edit?SiteId=...`

从 **外部** 页签 **新建脚本** 或点名称进入。全屏开发编辑布局。

<DocImage src="/cms/development/scripts-edit-overview.png" alt="脚本编辑：JavaScript 编辑器" width="1120" />

### 页头（外部脚本）

| 控件 | 说明 |
|------|------|
| **脚本名称** | 新建必填、唯一（1～64 字符）；保存后禁用 |
| **格式化** | 格式化 JavaScript |

内嵌脚本仅显示 **内嵌脚本** 标题与 **格式化**（见 [内嵌页签](#内嵌页签)）。

<DocImage src="/cms/development/scripts-edit-header.png" alt="脚本编辑页头" width="1120" />

### 代码编辑器

- 语言：**JavaScript**，支持 **KScript**；外部脚本支持代码引用。
- **保存** / **保存并返回** / 离开未保存提示，与 [布局](./layouts.md) 编辑一致。

## 相关

| 文档 | 说明 |
|------|------|
| [开发概述](./index.md) | 开发菜单索引 |
| [样式](./styles.md) | 站点 CSS（含行内样式页签） |
| [布局](./layouts.md) | 可产生内嵌脚本 |
| [Script 模板引擎](../../templateEngine/js/index.md) | 引用与语法 |
