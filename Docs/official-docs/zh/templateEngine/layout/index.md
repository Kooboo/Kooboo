# Layout（布局）

> 站点级 HTML 骨架：`<head>`、全局资源与 `k-placeholder` 占位区

## 是什么

**Layout** 是 Kooboo 站点中的一种资源，保存一整页 HTML 结构。多个 **Page** 可以共用同一 Layout，只替换占位区里的内容，从而统一导航、页脚、CDN 与全局样式。

Layout **没有** 自己的访问 URL；用户始终通过 Page 的 URL 访问，渲染时把 Page 内容合并进 Layout。

## 必须理解：`k-placeholder`

Layout 正文中至少要有一个占位元素，用于接收 Page 注入的内容：

```html
<div k-placeholder="Main"></div>
```

| 要点 | 说明 |
|------|------|
| 属性名 | `k-placeholder`（历史别名 `placeholder` / `position` 标签仍可能被解析，新站建议只用 `k-placeholder`） |
| 值 | 自定义名称，如 `Main`、`Sider`；**须与 Page 里 `<placeholder id="...">` 的 id 一致**（区分大小写） |
| 数量 | 可多个，实现主栏 + 侧栏等布局 |

### 双栏示例

```html
<!DOCTYPE html>
<html lang="zh-CN">
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <title>站点标题</title>
    <link rel="stylesheet" href="/assets/site.css" />
</head>
<body>
    <header>全站导航</header>
    <div class="flex">
        <main k-placeholder="Main"></main>
        <aside k-placeholder="Sider"></aside>
    </div>
    <footer>页脚</footer>
</body>
</html>
```

## 常见放什么

- **`<head>`**：meta、全局 [Style](../css/)、Tailwind CDN、`<script type="importmap">`（见 [Script](../js/)）
- **`<body>` 固定结构**：顶栏、侧栏壳、页脚
- **`k-placeholder`**：留给各 Page 的差异内容

::: warning head 中引用 View
`<head>` 内**不要**写 `<view id="...">`（例如在 head 里挂 Tailwind View）。请用 `k.response.renderView("<view id='...'></view>")` 或 `<script env="server" view="视图名"></script>`，详见 [View — 在 head 中引用](../view/#在-head-中引用-view)。
:::

Layout 里也可以写 **`<script env="server">`** 准备全站共享的服务端变量（例如站点名），再通过 `k-content` 等模板指令使用，详见 [模板绑定语法](../binding/)。

## 与 Page 的配合

Page 通过 `<layout id="布局名称">` 引用 Layout，并用 `<placeholder id="Main">` 把片段填进对应的 `k-placeholder`：

```html
<layout id="main">
    <placeholder id="Main">
        <h1>本页主内容</h1>
    </placeholder>
    <placeholder id="Sider">
        <nav>侧栏</nav>
    </placeholder>
</layout>
```

`layout` 的 `id` 对应后台 Layout 资源的 **名称**。占位 id 与 Layout 中 `k-placeholder` 的值一一对应。详见 [Page](../page/)。

## 何时新建 Layout

- 新站第一套壳（含 head、占位）
- 需要另一套完全不同的 head / 占位结构（例如营销落地页 vs 后台式双栏）

小改动（多一个占位、改页脚）通常 **改现有 Layout** 即可，不必为每页单独建 Layout。

## 用脚本管理 Layout

在 KScript 中通过 `k.site.layouts` 创建与更新，见 [k.site.layouts](../../api/site/layout.md)。

## 相关文档

- [Page](../page/)
- [View](../view/)
- [Script](../js/) · [Style](../css/)
- [模板绑定语法](../binding/)
