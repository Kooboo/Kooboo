# 前端代码风格指南

## 专项规范入口

用户明确要求客户端渲染 Vue、Vue 3 CDN、无构建 Vue，或使用 Kooboo Page、Style、Script 构建浏览器直接运行的 Vue 应用时，必须先读取 [Kooboo 客户端渲染 Vue 开发规范](cdn-vue/index.md)。

客户端渲染 Vue 是无 Layout 的完整 HTML Page，其 Vue import map、Tailwind v3 CDN、ES Modules、Style 和 Script 对象规则优先于本文面向 Layout/View 建站的 Tailwind 引入方式及静态阶段限制。未被专项规范覆盖的 HTML 语义、命名和可访问性规则仍适用。

## 样式方案

- **推荐将 Tailwind utility class 直接写在 HTML 中**，减少独立 CSS 文件中的重复布局代码
- 使用 Tailwind 前，**确保在 Layout 的 `<head>` 中引入 CDN**：`<script src="https://www.kooboo.com/tailwindcss.js"></script>`
- 自定义配置必须通过独立 `<script>` 中的 `window.tailwind.config` 定义，并放在 Tailwind CDN `<script>` 之后；该顺序不可颠倒，写在 CDN 之前不会生效
- 需要手写 CSS 时（复杂动画、自定义渐变、特殊背景纹理等），直接在 Style 中按需添加即可

### Tailwind 兼容性

当前加载的 Tailwind CSS 版本为 3.4.17。Tailwind Preflight 会重置按钮、表单控件、标题、列表、表格、媒体等元素的浏览器默认外观。不得依赖浏览器原生样式，应根据设计显式提供所需样式。

## HTML 规范

- 缩进使用 4 空格
- 标签语义优先（`<header>` `<main>` `<section>` `<nav>` `<footer>`），避免纯 `<div>` 堆砌
- 属性顺序：`id` → `class` → `data-*` → 其余属性 → `aria-*`
- 自闭合标签不加 `/`（HTML5 风格）

## 命名规范

- HTML/CSS：kebab-case（`class="hero-section"`，`id="main-nav"`）
- KScript 变量：camelCase（`var siteTitle = '...'`）
- View/Page id：snake_case（`<view id="product_list">`）

## 展示编号与详情页标识

- 用户可见的记录编号和详情页 URL 使用简短、易读且稳定的业务编号、短 ID 或 slug，例如 `POST-1024`、`/post/1024`、`/post/spring-guide`；避免直接展示 GUID/UUID，或生成 `/post/bb2db063-12c2-9bf2-97f9-c9dfd4053a85` 这类地址。此规则同样适用于 hash 路由。
- 内部主键可以保留 GUID，另设持久化的 `publicId`、`code` 或 `slug`。后端在相应站点/业务范围保证唯一并处理碰撞；不要直接截断 GUID，也不要用列表序号或每次渲染重新生成的值充当标识。标题变更不自动改变已有 slug。
- 列表链接、路由参数和详情 API 使用同一公开标识契约，由服务端解析到真实记录；不能只改链接文本或显示时缩短 ID。可读编号不是访问权限，详情/修改/删除接口仍验证记录权限。
- 维护已有应用时保留已发布链接和外部 API 契约；需要迁移时兼容旧标识或提供重定向，不批量改写现有主键。

## View 结构

- 每个 View 最外层包裹一个带 `class` 的语义容器，不污染全局命名空间
- **静态 View**：HTML 结构 → `<script>`（前端交互）；仅当确实需要服务端常量/配置时，单独使用 `<script env="server">`。这里的静态允许移动菜单、FAQ、tabs、gallery、静态筛选、hover/focus/active 等不依赖后端的 JS/CSS 交互。
- **动态化 View**：**`<k-data>`（取数/查询/整形）** → 直接使用 `k-xx` 的 HTML 绑定 → `<script>`（前端交互）；**不要**给外层元素添加 `env="server"`，也不要用 `<script env="server">` 调用 `k.commerce.*` / `k.content.*` / `k.DB.*` 替代 k-data

## Script 放置

- **动态取数**：优先 `<k-data>`，放在 View 顶部、HTML 结构之前
- `env="server"` + `type="module"` 的 KScript：仅当须 import CodeBlock/Service 或 k-data 无法覆盖时，放在区块顶部
- 前端交互 JS：放在 View 底部或独立的 Script 资源中，且不能位于任何 `env="server"` 祖先元素内；静态建站阶段的 JS 只操作当前 DOM、class、ARIA 状态和可见性，禁止 `fetch`、`k.*`、真实提交、持久化或调用业务 API

## 图片

- 所有 `<img>` 和 CSS `background-image` 的占位图使用 ImagePlaceholder URL，不自找外链

网站与 App 的配图及可编辑图片/附件字段遵循 [图片与上传](images-and-files.md)：提供文件选择/预览/上传，不用图片链接输入框替代，并实现已落盘文件的引用和清理生命周期。静态站点阶段仍遵守本阶段禁止真实提交的边界。
