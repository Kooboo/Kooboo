# Page（页面）

> 带路由的 HTML 页面：独立完整页，或通过 Layout 占位注入

## 是什么

**Page** 是 Kooboo 站点中**有 URL** 的 HTML 资源。用户访问 `/about` 时，渲染引擎加载对应 Page，再按需套上 Layout、解析其中的 `<view>` 与服务端绑定。

与 [k.page](../../api/page/index.md) 不同：`k.page` 只改**当前这次请求**的 title/meta；**Page 资源**才是站点页面仓库里的实体，由 [k.site.pages](../../api/site/page.md) 管理。

## 两种写法

### 1. 独立完整页面

不引用 Layout，Page 自身包含完整 `<!DOCTYPE html>` 结构，适合落地页或极简页。

若在 **`<head>`** 中需要输出 View（如全局 Tailwind 片段），不能写 `<view id="...">`，应使用 [View — 在 head 中引用](../view/#在-head-中引用-view)。

```html
<!DOCTYPE html>
<html lang="zh-CN">
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <title>关于我们</title>
</head>
<body>
    <script env="server">
        var title = '关于我们';
    </script>
    <h1 k-content="title"></h1>
    <view id="hero"></view>
</body>
</html>
```

### 2. 使用 Layout（推荐多页站点）

Page **body** 只写布局引用与占位内容，公共 head/导航在 Layout 中维护。

```html
<layout id="main">
    <placeholder id="Main">
        <div class="prose">
            <h1>主内容</h1>
            <p>仅本页差异部分</p>
        </div>
    </placeholder>
    <placeholder id="Sider">
        <aside>侧栏</aside>
    </placeholder>
</layout>
```

| 元素 | 说明 |
|------|------|
| `<layout id="...">` | `id` = Layout 资源名称，与后台 Layout 名一致 |
| `<placeholder id="...">` | `id` = Layout 里某个 `k-placeholder` 的名称 |
| 占位内部 | 普通 HTML，可含 `<view>`、内联 script，以及 `<script env="server">` |

::: tip 名称对齐
Layout 中 `k-placeholder="Main"` 与 Page 中 `<placeholder id="Main">` 必须一致。拼写或大小写不一致会导致占位区空白。
:::

## 引用 View

在 Page（或 Layout、View）任意位置插入可复用片段：

```html
<view id="product_card"></view>
```

`id` 对应站点 **View** 资源名称。详见 [View](../view/)。

## 脚本与样式

- **全局**：在 Layout 的 `<head>` 用 `<link href="...">`、`<script src="...">` 引用 [Style](../css/) / [Script](../js/) 的路由 URL
- **本页**：可在 Page 内写 `<script env="server">`（KScript）或 `<script type="module">`（前端 ES Module）

推荐结构：`<script env="server">` 数据脚本放在区块顶部，前端交互 script 放在 View 底部或独立 [Script](../js/) 资源。

## 组件拆分原则

Page 不宜堆过多 markup。出现以下情况时，把区块拆成 **View**：

- 多个语义区（Header / Hero / Features / Footer）
- 多页复用的块（商品卡片、文章列表）
- 单页超过 3–4 个明显独立区块

拆分后 Page 只保留骨架与占位，细节在 View 及其 Script 中维护。

## 用脚本管理 Page

创建页面、注册 URL、绑定 Layout 名称等，见 [k.site.pages](../../api/site/page.md)。

## 相关文档

- [Layout](../layout/)
- [View](../view/)
- [模板绑定语法](../binding/)
- [k.page](../../api/page/index.md) — 当前页 Meta
