# k.utils.template / k.utils.inlineHtml

> 视图渲染与页面内联 HTML

## k.utils.template

渲染模板字符串。简单的空 `<view id="..."></view>` 输入会走直接渲染 View 的路径；其他模板字符串会交给正常模板执行器。

### render()

```ts
const html = k.utils.template.render("<view id='product-card'></view>");
```

快速路径要求输入是只包含 `id` 属性的简单空 View 标签；这不是 `render()` 对所有输入的要求。

适用于在 API 或 CodeBlock 中拼装片段 HTML。

其他公开方法：

| 方法 | 契约 |
| --- | --- |
| `renderView(viewNameOrId)` | 渲染 View 并返回 HTML；找不到 View 时返回 `null`。 |
| `executeView(viewNameOrId)` | 将 View 执行并写入当前响应，返回 `void`；找不到 View 时不产生输出。 |
| `renderPage(url)` | 使用克隆的请求上下文渲染匹配的 Page。URL 为空或无法匹配时返回空字符串；Page 数据缺失时返回 `null`。 |

---

## k.utils.inlineHtml

将站点 **Page** 转为可独立使用的内联 HTML，常用于预览、截图前处理。实现会尝试内联可解析的站点内部图片、文件、脚本、样式及 CSS 图片 URL；外部 HTTP(S)、协议相对 URL 和 data URI 保持外部引用。

### convertPageInline()

按 Page 名称或 ID 查找：

```ts
const html = k.utils.inlineHtml.convertPageInline("home");
```

### convertPageUrlInline()

按站点 **相对 URL** 查找。该 URL 只用于选择 Page，内联时不会保留原始请求的完整上下文：

```ts
const html = k.utils.inlineHtml.convertPageUrlInline("/about");
```

找不到页面时返回 `null`。

## 相关模块

- 截图：[screenshot.md](screenshot.md) 中 `takePage` 内部会使用内联 HTML
