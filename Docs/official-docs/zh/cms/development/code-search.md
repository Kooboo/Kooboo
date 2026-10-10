# 代码搜索

> 菜单：**开发 → 代码搜索**  
> 深链：`/_Admin/development/code-search?SiteId={站点GUID}`

在站点内 **全文搜索代码与模板正文**：跨页面、View、Layout、Script、Style、Code、HtmlBlock、表单、内容等类型，列出命中行号与摘要，并可 **新标签页** 打开对应编辑页。

::: tip 权限
菜单权限：`code·view`。须在编辑菜单中勾选 **开发 → 代码搜索**（`advanced`）。结果行 **编辑** 图标另需对应对象类型的 `view` 权限（如页面为 `page·view`）。
:::

## 如何打开

1. 在 [编辑菜单](../navigation.md#权限与编辑菜单) 中勾选 **开发 → 代码搜索**。
2. 左侧 **开发 → 代码搜索**。

<DocImage src="/cms/development/code-search-overview.png" alt="代码搜索页总览" width="1120" />

## 搜索框

| 控件 | 说明 |
|------|------|
| 关键字输入 | `codeStore.keywords`，占位「搜索代码」；防抖约 1 秒后请求 `CodeSearch/Search` |
| **Aa** | 切换 **匹配大小写**（`ignoreCase` 取反；高亮表示区分大小写） |
| **.\*** | 切换 **使用正则表达式**（`isRegex`）；非法正则不会发起搜索 |

关键字为空时结果列表清空。离开本页且目标路由无 `id` 时会清空关键字（避免带回其它页面）。

快捷键（与输入框旁按钮一致）：**匹配大小写**、**使用正则表达式**。

<DocImage src="/cms/development/code-search-toolbar.png" alt="代码搜索框与 Aa、正则开关" width="1120" />

## 结果列表

无分页；有匹配时表格展示：

| 列 | 说明 |
|----|------|
| **名称** | 对象显示名 |
| **类型** | 如 `Page`、`View`、`Code`、`Layout`、`Script`、`Style` 等 |
| **行** | 每个命中一行：`行 N:` + 摘要（`matched[].lineNumber` 从 0 起，界面显示为 N+1） |
| **编辑** | 铅笔图标，**新标签** 打开该对象编辑路由（页面按页面类型跳转设计器/富文本/布局页等） |

<DocImage src="/cms/development/code-search-results.png" alt="代码搜索结果列表" width="1120" />

### 编辑跳转（按类型）

| 类型 | 典型路由 |
|------|----------|
| `Page` | 依 `params.type`：布局设计器、富文本、布局页编辑等 |
| `View` / `Layout` / `Script` / `Style` / `Code` | 对应 `view-edit`、`layout-edit` 等 |
| `HtmlBlock` | `htmlBlock-edit` |
| `Form` | `form-edit` |
| `TextContent` | 内容编辑 |

## 相关

| 文档 | 说明 |
|------|------|
| [开发概述](./index.md) | 开发菜单索引 |
| [代码](./code.md) | 代码文件管理 |
| [代码日志](./code-log.md) | 运行日志 |
