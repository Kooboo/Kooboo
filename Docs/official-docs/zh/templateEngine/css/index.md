# Style（站点样式）

> 带路由的 `.css` 资源：全局主题、补丁与 Layout `<head>` 引用

## 是什么

**Style** 是 Kooboo 站点中独立保存的 CSS 文件，注册后通过 **URL** 访问（例如 `/assets/site.css`）。用于全站主题、组件库补丁、不适合写在每个 Page 里的样式。

由 **`k.site.styles`** 管理，见 [k.site.styles](../../api/site/css.md)。

## 在模板中引用

在 Layout（推荐）或 Page 的 `<head>`：

```html
<link rel="stylesheet" href="/assets/site.css" />
```

`href` 为 Style 资源的访问路径。服务端可用 `k.site.styles.getUrl(id)` 获取相对 URL。

## 与 Tailwind、内联样式

| 方式 | 说明 |
|------|------|
| Tailwind CDN | 常在 Layout 引入 `https://www.kooboo.com/tailwindcss.js`，utility class 直接写在 HTML |
| Style 资源 | 复杂动画、全局变量、第三方组件库补丁（如 Element Plus + Tailwind 权重修正） |
| Page/View 内 `<style>` | 仅影响单页或单区块时使用 |

多数布局类样式优先 Tailwind；重复少、需集中维护的规则放进 Style 资源。

## 与 Script 的对称关系

| | Style | Script |
|---|--------|--------|
| 标签 | `<link rel="stylesheet">` | `<script src="...">` |
| 典型位置 | Layout `<head>` | Layout `<head>` 或 Page 底部 |
| API | `k.site.styles` | `k.site.scripts` |

二者均支持路由 URL 与版本日志；Layout / View **没有** 独立 URL。

## 相关文档

- [Layout](../layout/)
- [Script](../js/)
- [k.site.styles](../../api/site/css.md)
