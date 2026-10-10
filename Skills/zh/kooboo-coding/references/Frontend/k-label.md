# Site 多语言标签开发指南

适用于 Kooboo 站点前端中的 Label 多语言文案。`k-label`、`k.label()`、`k.t()` 都基于同一套 Label 存储，核心目标都是“按当前语言取值”。

## 先记结论

- 纯文本节点优先用 `k-label`
- 属性值优先用 `k.label(...)`
- 服务端脚本里读取文案用 `k.label(...)` 或 `k.t(...)`
- Label 支持 `{param}` 占位符传参
- `k.t(...)` 比 `k.label(...)` 多一个行为：当 label 不存在时，会自动补一条同 key 的 label

## 三种写法的关系

### `k-label`

用于把元素内部文本交给 Label 系统管理。

```html
<h1 k-label="hero_title">Build faster with Kooboo</h1>
```

渲染时会读取当前语言下 `hero_title` 的值。

适合：

- 标题
- 按钮文案
- 段落文案
- 链接文本

不适合：

- `<input>`、`<textarea>` 这类需要设置属性值的元素
- 需要在脚本中先拼装再输出的复杂内容

### `k.label(...)`

用于在 KScript / 模板绑定中主动读取 label。

```html
<div k-content="k.label('hero_title')"></div>
```

适合：

- `k-content`
- `k-attribute`
- `env="server"` 的 `<script>`
- API / Service / redirect 逻辑里返回当前语言文本

### `k.t(...)`

`k.t(...)` 也是读当前语言 label，但更适合把现成静态文案快速转成多语言。

```html
<p k-content="k.t('No data available')"></p>
```

当前实现里，如果 `"No data available"` 这条 label 还不存在，`k.t(...)` 会先自动补一条 label，再返回当前语言值。

推荐理解：

- `k.label("agreed_key")`：读取一个已经约定好的 label key
- `k.t("literal text")`：把现成文案直接接入多语言系统

## 常见写法

### 文本节点

```html
<h2 k-label="pricing_title">Simple pricing</h2>
<p k-label="pricing_desc">Choose a plan that fits your team.</p>
```

### 属性值

`k-label` 不能直接用在 `<input>` 上，因为它只负责渲染元素内部内容。属性值请改用 `k.label(...)`。

```html
<input k-attribute="placeholder k.label('search_placeholder')">
```

### 文本和属性混用

```html
<a href="/contact" k-label="contact_us">Contact us</a>
<input k-attribute="aria-label k.label('search_aria_label')">
```

## Label 传参

Label 值支持 `{param}` 占位符。

例如后台保存的 label 值可以是：

```text
Hello {name}
```

前端或服务端读取时传入参数：

```html
<div k-content="k.label('welcome_user', { name: 'Kooboo' })"></div>
```

也可以在 KScript 中使用：

```js
var message = k.label("order_created", {
    orderNo: "KB20260514001"
})
```

如果 label 内容是：

```text
Order created successfully: {orderNo}
```

则会输出：

```text
Order created successfully: KB20260514001
```

## 选型建议

### 优先用 `k-label` 的场景

- 页面上是固定文本
- 希望 AI 直接在 HTML 结构里标记哪些文案要多语言化
- 目标是让后台内容团队在 Label 列表里统一维护这些文案

### 优先用 `k.label(...)` 的场景

- 文案要写到 `placeholder`、`title`、`aria-label`、`alt` 等属性里
- 文案要先在服务端脚本中参与拼装
- API / Service 里要返回当前语言消息

### 优先用 `k.t(...)` 的场景

- 你手上先有一份现成静态文案，想快速转成多语言
- 允许第一次运行时自动补 label

## 与后台的协作关系

- 多语言总开关在 `/_Admin/system/settings?SiteId=xxx` 的 Basic 页
- Label 列表在 `/_Admin/content/labels?SiteId=xxx`
- `k-label`、`k.label()`、`k.t()` 最终都依赖 Label 列表中的多语言值

## 相关文档:

- [站点多语言场景指南](../Scenario/multilingual-site-guide.md)
