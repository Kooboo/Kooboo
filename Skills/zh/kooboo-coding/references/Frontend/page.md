# Site Page 页面开发指南

## 概述

本指南提供站点页面开发的简洁参考，介绍常见结构、使用场景和示例代码，帮助快速构建功能页面。

## 使用场景

创建或修改站点页面时使用，包括标准页面和带布局的页面。

## Page URL 与动态路由

传给 `create_page` 的 `url` 就是浏览器访问路由，可以是静态路径，也可以包含 `{parameter}` 形式的动态路径模板。需要独立访问的详情页应优先用路径参数表达规范身份：文章使用 `/blog/{slug}`，Commerce 商品使用 `/product/{seoName}`；搜索、筛选、排序和分页等列表状态使用 query string。

Page URL、列表/详情链接和详情页 `k-data` 必须使用同一个参数名。`/blog/{slug}` 会把 `slug` 放入渲染上下文，`/product/{seoName}` 会把 `seoName` 放入渲染上下文。只有兼容既有公开 URL 时才保留原路由；路径可表达时，不要新建 query-string 详情身份。

## 示例

### 标准页面

```html
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <title>Document</title>
</head>
<body>
    <div>
        <h1>Site Page Demo</h1>
        <view id="hero"></view>
    </div>
</body>
</html>
```

### 带布局的页面

`<layout>` 是**外层容器**，不是声明标签。所有 `<placeholder>` 必须嵌套在 `<layout>` 内部；并列或空闭合会导致内容无法注入 Layout。

```html
<layout id="main">
    <placeholder id="Main">
        <div>main content</div>
    </placeholder>
    <placeholder id="Sider">
        <div>sider content</div>
    </placeholder>
</layout>
```

### 常见错误（禁止）

```html
<!-- 错误：layout 与 placeholder 并列，placeholder 内容不会进入 Layout -->
<layout id="main"></layout>
<placeholder id="Main">
    <div>main content</div>
</placeholder>
```

## 特性

- `<layout id="[LayoutName]">`: **容器标签**，必须包裹本页所有 `<placeholder>`；`id` 必须与 `create_page` 的 `layoutName` 及 Layout 对象名称一致
- `<view id="[Custom Name]"></view>`: 引用视图组件，参见 `view` 参考文档
- `<placeholder id="[Name]">`: 带 Layout 的 Page 必须用它把内容注入到 Layout 的 `k-placeholder="[Name]"` 区域；`id` 值大小写敏感，必须与 Layout 中的 `k-placeholder` 属性值逐字一致。例如 Layout 使用 `k-placeholder="Main"` 时，Page 必须使用 `<placeholder id="Main">`，不能写成 `main`。
- Page 普通容器不声明 `env="server"`；`k-data` / `k-xx` 绑定可直接使用，真正的服务端逻辑放在独立 `<script env="server">` 中

## 组件拆分原则

Page 不应承载过多代码。当 Page 内出现以下情况时，应将独立区块拆分为 View：

- 多个语义独立的区域（Header/Hero/Features/Footer 等）
- 可在其他页面复用的区块（商品卡片、文章列表、侧边栏等）
- 单个 Page 文件超过 3-4 个独立区块

拆分后 Page 保持简洁，具体逻辑下放到各 View 和对应的 Script 中。每个 View 只关注自身区块，便于维护和复用。

## Page 骨架与 View 装配（建站 S4）

建站 S4 步骤 ④ 的 **Page 内联骨架唯一出处**。其它 reference（含 SOP）只引用本节，不重复贴代码。

建站时先获取当前页 `page-item`，再 **`create_page`，随后逐个 `create_view`**，不要先把真实 View 全部做完。

1. **站点级**：`create_layout`（含 `site_header` / `site_footer` 等 chrome 的 `<view>` 引用）→ `create_style` → 轻量 `create_view`（仅 header/footer 等跨页块）。
2. **单页级**：`page-item` 获取 `blockRefs + compositionGroups` → `create_page` 内用普通 HTML/CSS 组织页面组合并占位各 **page-level** block → 预览打开该页 → 逐 block `create_view` 并替换骨架为 `<view id="..."></view>`。

**Chrome 不在 Page placeholder**：header/footer 由 Layout 引用；若在 Layout 与 Page 同时引用同一 view id，会重复渲染。

**Page 可以包含正常 HTML 元素**：`<placeholder>` 内可以使用 `main`、`section`、`div`、`aside`、grid/flex wrapper、页面标题和 breadcrumbs。View 负责独立内容块，Page 负责多个 View 之间的布局关系；不要把 Page 误写成只能连续排列 `<view>` 的清单。

内联骨架示例（仅 page-level blocks）：

```html
<layout id="main">
  <placeholder id="Main">
    <section data-block="hero" class="min-h-[40vh] bg-gray-100 animate-pulse rounded-lg"></section>
    <section data-block="features" class="h-32 animate-pulse bg-gray-50 rounded-lg mt-8"></section>
  </placeholder>
</layout>
```

包含 `compositionGroups` 时，先在 Page 骨架中建立组合 wrapper。下面只示意信息架构，具体列宽、间距、sticky 和视觉风格结合当前 Design System 决定：

```html
<layout id="main">
  <placeholder id="Main">
    <main class="mx-auto max-w-7xl px-4 py-10">
      <section data-composition-group="product-detail-product-decision"
               class="grid gap-8 lg:grid-cols-[minmax(0,1.25fr)_minmax(20rem,0.75fr)] lg:items-start">
        <div data-block="product-detail-product-gallery"
             class="min-h-[32rem] animate-pulse rounded-lg bg-gray-100"></div>
        <aside data-block="product-detail-product-summary"
               class="min-h-80 animate-pulse rounded-lg bg-gray-50"></aside>
      </section>
      <section data-block="product-detail-related-products"
               class="mt-16 h-48 animate-pulse rounded-lg bg-gray-50"></section>
    </main>
  </placeholder>
</layout>
```

真实 View 完成后，只替换对应 `data-block` 位置，保留 Page 的 composition wrapper：

```html
<section data-composition-group="product-detail-product-decision"
         class="grid gap-8 lg:grid-cols-[minmax(0,1.25fr)_minmax(20rem,0.75fr)] lg:items-start">
  <view id="product_detail_product_gallery"></view>
  <aside><view id="product_detail_product_summary"></view></aside>
</section>
```

Commerce 任务页默认关系：

- Product Detail：`product-gallery` 主区 + `product-summary` 侧区。
- Cart：`cart-items` 主区 + `checkout-summary` 侧区。
- Checkout：`checkout-form` 主区 + `checkout-summary` 侧区。
- Order Detail：`order-status` 主区 + `checkout-summary` 侧区。

桌面端默认相邻并排，移动端按主区在前、侧区在后的语义顺序堆叠。允许 AI 改变比例、留白、边界、sticky 与视觉处理，但不得仅因 View 独立就把这些任务区默认排成两个连续的全宽 section。
