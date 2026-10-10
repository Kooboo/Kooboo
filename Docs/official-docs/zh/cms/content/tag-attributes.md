# 元素属性

> 菜单：**内容 → 元素属性**（路由 `text`；须在 **编辑菜单** 中勾选显示）  
> 深链：`/_Admin/content/text?SiteId={站点GUID}`

**元素属性**（引擎中的 **KConfig**）用于在后台**集中修改**模板里某段 HTML 标签的可编辑属性（如 `href`、`src`、标签内 HTML 等），而不用每次改 Page/View 源码。条目来自模板中的 **`k-config`** 标记；本页**不能凭空新建**一条配置，也没有「新建」按钮。

::: tip 冷门但有用
适合「页脚链接地址」「某张图的 `src`」「某段可替换的块级 HTML」等需要在多处页面复用、又希望运营在后台改属性的场景。若只是改几个字的多语言文案，用 [标签](./labels.md)；若是一整块复用 HTML，用 [HTML 片段](./html-blocks.md)。
:::

::: warning 与「标签」、内容条目无关
| | **元素属性**（本页） | [标签](./labels.md) | [内容条目](./contents-entries.md) |
|--|---------------------|---------------------|-----------------------------------|
| 路由 | `/content/text` | `/content/labels` | `/content/content` |
| 数据 | KConfig：HTML 属性键值 | Label：多语言字符串 | TextContent：内容夹字段 |
| 模板标记 | `k-config="键名"` | `k.label()` / `k.t()` | `k.content` / k-data |
:::

## 原理简述

在 Page、Layout 或 View 的 HTML 上为元素加上 **`k-config="唯一键名"`**（也可用 `v-config`，与引擎绑定一致）。站点**渲染或保存**相关页面时，Kooboo 会登记或更新对应的 KConfig 记录，并抽出可编辑属性到 **binding** 字典（如 `href`、`src`、`innerHtml` 等）。

渲染输出时，`k-config` 属性本身不会出现在最终 HTML 中；各 binding 的值会写回对应属性。

```html
<!-- 示例：页脚链接，键名 footer-home -->
<a k-config="footer-home" href="/" class="footer-link">首页</a>

<!-- 示例：图片 src 可在本页或媒体库中改 -->
<img k-config="logo-main" src="/images/logo.png" alt="Logo" />
```

::: tip 哪些属性会进 binding？
一般 **普通 HTML 属性**（如 `href`、`src`、`class`、`id`）会纳入；**`style`**、**`on*` 事件**、名称中含 **`-` / `_`** 的属性通常保持静态，不在此页编辑（以引擎 `IsIgnoreAttribute` 为准）。非自闭合标签还会包含 **`innerHtml`**（标签内部 HTML）。
:::

## 如何打开

1. 在 [编辑菜单](../navigation.md#权限与编辑菜单) 中勾选 **元素属性**。
2. 左侧 **内容 → 元素属性**。
3. 若列表为空，请先在模板中加入 `k-config` 并保存/访问相关页面，让系统生成条目。

<DocImage src="/cms/content/tag-attributes-list.png" alt="元素属性列表" width="1120" />

## 列表页

| 列 / 操作 | 说明 |
|-----------|------|
| 名称 | KConfig 的 **键名**（即 `k-config` 的值）或显示名；副标题为 binding 中第一项的摘要（如 `href: /`） |
| 预览 | 若标签为 **`img`** 且 binding 含 **`src`**，显示缩略图 |
| 标签名 | 原始 HTML 标签，如 `<a>`、`<img>`；悬停可看 `tagHtml` 片段 |
| 引用 | 被哪些 Page / Layout / View 等引用 |
| 最后修改 | 更新时间 |
| **编辑** | 修改 binding；`img` 的 `src` 改为打开 [媒体库](../site/media.md) 选图 |
| **版本** | 版本历史（权限允许时） |

支持勾选后 **批量删除**（权限 `text` · `delete`）。

## 编辑弹窗

标题为 **元素属性**。每个 **binding 键** 一行（多行文本框），例如：

| 键 | 含义（常见） |
|----|----------------|
| `href` | 链接地址 |
| `src` | 图片/资源 URL |
| `innerHtml` | 标签内部 HTML（非自闭合标签） |
| `class` / `id` 等 | 其它可编辑属性 |

保存后写入站点 KConfig，后续请求按新值渲染。

::: tip `img` 与 `src`
标签名为 **`img`** 时，编辑 **`src`** 将直接弹出 **媒体库** 选择器（与列表行内逻辑一致），选图后写回 binding。
:::

<DocImage src="/cms/content/tag-attributes-edit.png" alt="编辑元素属性：修改 href、innerHtml 等 binding" width="960" />

## 与在线编辑

[在线编辑](../site/pages-inline-edit.md) 中若选中带 `k-config` 的元素，可快捷打开同一套编辑弹窗（提示一般为立即生效类文案）。适合在预览页上改链接或图片，无需进本列表。

## 典型流程

1. 开发在 View 或 Page 中为元素添加 `k-config="promo-banner"` 及初始 `href`、`innerHtml` 等。
2. 保存并预览/发布页面，使 KConfig 出现在 **元素属性** 列表。
3. 运营在本页或 **在线编辑** 中修改 `href` / `innerHtml` 等。
4. 用 **引用** 列确认仍被哪些页面使用；无用条目可删除。

## 相关

| 文档 | 说明 |
|------|------|
| [内容模块概述](./index.md) | 内容分组菜单 |
| [编辑菜单](../navigation.md#权限与编辑菜单) | 显示本菜单 |
| [在线编辑](../site/pages-inline-edit.md) | 预览页改 KConfig |
| [模板引擎 · Page](../../templateEngine/page/index.md) | 编写带 `k-config` 的 HTML |
| [标签](./labels.md) | 多语言短文案（非本功能） |
