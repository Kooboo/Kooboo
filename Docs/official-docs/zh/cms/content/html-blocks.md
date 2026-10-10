# HTML 片段

> 菜单：**内容 → HTML 片段**  
> 深链：`/_Admin/content/htmlblocks?SiteId={站点GUID}`  
> 编辑：`/_Admin/content/htmlBlock/edit?SiteId=...&id={片段Id}`（新建无 `id`）

**HTML 片段**（HtmlBlock）用于在后台集中维护一段 **HTML 内容**，在多个 Page、Layout、View 中复用。前台渲染时按当前站点语言输出对应版本，避免同一段页脚、广告条在多处复制粘贴。

::: tip 与 View、标签的区别
| | HTML 片段 | [View](../../templateEngine/view/index.md) | [标签](./labels.md) |
|--|-------------|-------------------------------|----------------------|
| 后台菜单 | 内容 → HTML 片段 | 开发 → View | 内容 → 标签 |
| 内容形态 | 富文本/HTML 块 | 模板片段（常含绑定） | 短文案字符串 |
| 模板引用 | `<htmlblock id="...">` | `<view id="...">` | `k.label()` |
:::

::: tip 前置：多语言（可选）
与 [标签](./labels.md) 相同：若站点启用多种语言，编辑页会为每种语言提供 **内容** 字段；须在 **站点设置 → 基础设置** 中先配置语言。单语言站点只显示一个 **内容** 输入区。
:::

## 如何打开

1. 进入站点后台（地址栏带 `SiteId`）。
2. 左侧 **内容 → HTML 片段**。

<DocImage src="/cms/content/html-blocks-list.png" alt="HTML 片段列表" width="1120" />

## 列表页

| 列 / 操作 | 说明 |
|-----------|------|
| 名称 | 片段标识，与模板中 `id` 一致；建议英文，如 `footer_links`、`promo_banner` |
| 引用 | 被哪些 Page / Layout / View 等资源引用（可点击查看） |
| 最后修改 | 更新时间 |
| **编辑** | 进入编辑页 |
| **版本** | 版本历史（站点日志权限允许时） |

工具栏 **新建** 进入编辑页（无 `id`）。勾选多行可 **批量删除**（权限 `htmlBlock` · `delete`）。

## 编辑页

全屏表单 + 底部 **保存** / **取消**。

| 字段 | 说明 |
|------|------|
| 名称 | **新建**时填写，须唯一、1～50 字符、符合资源命名规则；**保存后不可改名**（编辑时只读展示） |
| 内容 · `{语言}` | 每种已启用语言一行，使用 **富文本/HTML 编辑器**（KEditor）维护该语言下的 HTML |
| 多语言选择器 | 右上角切换当前编辑侧重（各语言字段仍按语言列出） |

离开页面前若有未保存修改，会提示确认。

<DocImage src="/cms/content/html-blocks-edit.png" alt="编辑 HTML 片段" width="1120" />

## 在模板中引用

片段 **名称** 与下列写法中的 `id` / 绑定值一致（大小写以站点内资源名为准）：

### 标签形式（常用）

在 Page、Layout 或 View 的 HTML 中：

```html
<htmlblock id="footer_links"></htmlblock>
```

### 绑定属性

也可使用渲染绑定（与引擎 `k-htmlblock` / `v-htmlblock` 对应）：

```html
<div k-htmlblock="footer_links"></div>
```

渲染时输出该片段在当前请求语言下的 HTML；找不到片段时行为以引擎为准（开发环境可对照资源名排查）。

::: tip 与 Layout 占位
片段常放在 Layout 的 `<placeholder>` 内，或与 `<view>` 并列使用，用于全站统一的页脚、侧栏公告等。Page 结构见 [模板引擎 · Page](../../templateEngine/page/index.md)。
:::

## 其它入口

| 场景 | 说明 |
|------|------|
| [在线编辑](../site/pages-inline-edit.md) | 若站点开启「编辑 HTML 片段」，可在预览页选中已绑定的片段块，右键/工具条进入编辑（保存后刷新预览） |
| 页面树 | **页面** 列表的 **页面树** 视图中，可将选中节点 **转换成 HTML 代码块**（新建或合并到已有片段），便于把重复 DOM 抽成片段 |
| 开发 · 代码搜索 | 可跳转到片段编辑页 |

## 相关

| 文档 | 说明 |
|------|------|
| [内容模块概述](./index.md) | 内容分组菜单 |
| [标签](./labels.md) | 多语言短文案 |
| [View](../../templateEngine/view/index.md) | 可复用视图片段 |
| [在线编辑](../site/pages-inline-edit.md) | 前台可视化改片段 |
| [k.utils.inlineHtml](../../api/utils/inlineHtml.md) | 将外部资源内联进 HTML 导出（与 HtmlBlock 管理无关） |
