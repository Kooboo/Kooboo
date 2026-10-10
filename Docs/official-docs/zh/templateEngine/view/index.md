# View（视图）

> 可复用的 HTML 片段，通过 `<view id="...">` 嵌入 Page / Layout / 其他 View

## 是什么

**View**（后台有时称为组件 / Component）是站点内的一段 HTML 模板，**没有独立 URL**。在任意模板中写：

```html
<view id="hero"></view>
```

渲染时替换为名为 `hero` 的 View 资源内容，并继续解析其中的 `env="server"`、嵌套 `<view>` 等。

## 在 `<head>` 中引用 View

在 **`<head>`** 里不能再写 `<view id="...">`（例如把 Tailwind 片段 View 直接放进 head）。此类写法**已不再支持**。

**错误：**

```html
<head>
    <meta charset="UTF-8">
    <view id="tailwind"></view>
</head>
```

**兼容写法（当前）：** 在 `<head>` 内用服务端脚本输出 View：

```html
<head>
    <meta charset="UTF-8">
    <script env="server">
        k.response.renderView("<view id='tailwind'></view>")
    </script>
</head>
```

`renderView` 接收 View 标记字符串（与 body 中 `<view id="...">` 等价），在 head 上下文中渲染并写入输出。

**简略写法（推荐）：** 在支持的环境中可直接写 View 名：

```html
<script env="server" view="tailwind"></script>
```

`view` 为站点 **View 资源名称**（与 `<view id="tailwind">` 的 id 一致）。无脚本体时仅渲染该 View。

`<body>`、`<placeholder>` 内仍可直接使用 `<view id="...">`，无需 `renderView`。

服务端 API 见 [k.response.renderView](../../api/response/index.md#renderview)。

## 适用场景

| 适合放进 View | 留在 Page |
|---------------|-----------|
| 全站 Header / Footer | 路由级 `<layout>` 与 `<placeholder>` |
| 商品卡片、文章列表项 | 本页唯一、不复用的大段文案 |
| 带独立数据准备的区块 | 仅组合多个 View 的薄 Page |

## 结构建议

每个 View 最外层使用带 `class` 的语义容器，避免污染全局：

```html
<section class="hero">
    <script env="server">
        var title = 'Welcome';
    </script>
    <h1 k-content="title"></h1>
    <view id="hero_cta"></view>
</section>
```

推荐顺序：

1. `<script env="server">` — 准备本区块数据  
2. HTML 结构 + `k-content` / `k-for` 等  
3. 前端交互 — 底部 `<script>` 或引用 [Script](../js/)

## 命名

View 的 `id` 与资源 **name** 一致。建议 **snake_case**，例如 `product_list`、`site_header`。

## 嵌套

View 内可再引用其他 View：

```html
<view id="other_view"></view>
```

注意避免循环引用（A 引 B、B 又引 A）。

## 与 Layout / Page 的区别

| 资源 | 引用方式 | 路由 |
|------|----------|------|
| Layout | Page 用 `<layout id>` | 无 |
| Page | 站点 URL | 有 |
| View | `<view id>` | 无 |

## 用脚本管理 View

`k.site.views` 的增删改查见 [k.site.views](../../api/site/view.md)。

## 相关文档

- [Page](../page/)
- [模板绑定语法](../binding/)
- [Script](../js/)
