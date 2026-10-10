# 站点多语言场景指南

适用于需要在 Kooboo 站点中同时完成前端页面、多语言标签、后台配置或 AI 直接更新站点设置、服务端跳转与语言切换的场景。

## 这套能力解决什么问题

Kooboo 的站点多语言不是单点功能，而是三端协作：

- 前端：页面中的静态文案、属性文案、语言切换器
- 后端 / KScript：按当前语言返回文本，生成带语言信息的 URL 和跳转
- Kooboo 后台 / AI 基础函数：开启多语言、配置语言、维护 Label 内容

如果只写了 `k-label`，但后台没开多语言，或者 redirect 仍然写死 `/cart`，最终体验依然会断掉。

## 先记结论

- 页面静态文案用 `k-label`
- 属性、脚本、API 文案用 `k.label(...)` 或 `k.t(...)`
- 开始前先调用 `get_site_info` 看当前站点是否已开启多语言，以及默认语言、语言列表、URL 模式
- 需要由 AI 直接改站点设置时，可调用 `update_site_settings` 更新 `isMultiLingual`、`defaultCulture`、`cultures`、`enableSitePath`
- `update_site_settings.cultures` 是整组覆盖，不是追加；想保留现有语言就要把完整列表一起传回
- 多语言总开关在 `/_Admin/system/settings?SiteId=xxx`
- Label 管理页在 `/_Admin/content/labels?SiteId=xxx`
- 语言切换优先用 `k.site.multilingual.currentURLs()`
- 不要手写 `/products?lang=en`、`/products?culture=en`、`/en/products` 或直接写死站内跳转；统一通过 `k.site.multilingual.*` 生成 URL

## 先读取当前站点配置

在决定“是保持参数模式还是改成路由模式”、“是否需要补语言列表”之前，先调用 `get_site_info`，不要凭猜测写多语言逻辑。

重点读取 `siteSettings`：

- `isMultiLingual`：当前是否已开启多语言
- `defaultCulture`：默认语言，例如 `en`
- `cultures`：当前站点支持的语言列表
- `enableSitePath`：`false` 表示参数模式（如 `/about?lang=en`），`true` 表示路由模式（如 `/en/about`）
- `baseUrl`：需要给用户前台预览地址时再使用

最小示例：

```json
{
  "siteSettings": {
    "name": "demo-site",
    "isMultiLingual": true,
    "defaultCulture": "en",
    "cultures": ["en", "zh"],
    "enableSitePath": false,
    "baseUrl": "https://example.com"
  }
}
```

## AI 直接更新站点设置

当用户明确希望 AI 直接帮他开启或调整站点多语言配置，而不是只给后台路径时，可以调用 `update_site_settings`。

适合直接改的字段：

- `isMultiLingual`：开启或关闭多语言
- `defaultCulture`：设置默认语言
- `cultures`：设置语言列表
- `enableSitePath`：`false` 为参数模式，`true` 为路由模式

使用规则：

- 先 `get_site_info`，确认当前状态后再更新
- `cultures` 一旦传入，就是整组替换；如果当前已有 `["en", "zh"]`，你想新增 `fr`，应传 `["en", "zh", "fr"]`，不能只传 `["fr"]`
- `defaultCulture` 应与 `Cultures` 保持一致，避免默认语言不在语言列表里
- `update_site_settings` 只改站点设置，不会自动创建或补齐 Label 内容
- 这是需要审批的操作，适合“用户明确要求直接修改配置”的场景

最小示例：

```json
{
  "isMultiLingual": true,
  "defaultCulture": "en",
  "cultures": ["en", "zh"],
  "enableSitePath": false
}
```

如果只是需要让用户自己进入后台确认或继续维护 Label，仍然按下面的后台入口指引执行。

## 后台入口

### 1. 开启站点多语言

打开：

```text
/_Admin/system/settings?SiteId=xxx
```

多语言相关设置位于 Basic 页中的 `Multilingual` 分组。

关键选项：

- `Multilingual`：总开关
- `Default Language`：默认语言
- `Parameter Mode` / `Router Mode`：参数模式或路由模式
- `Default Language Prefix`：默认语言是否也带前缀
- `Auto Detect Culture`：自动识别语言
- `HrefLang`：输出多语言 SEO 相关标记

### 2. 维护 Label 列表

打开：

```text
/_Admin/content/labels?SiteId=xxx
```

这里是 `k-label`、`k.label()`、`k.t()` 实际读取的多语言数据源。

一个 label 通常至少包含：

- `name`：label key，例如 `hero_title`
- `values.en`
- `values.zh`

## 前端如何定义多语言

### 文本节点

```html
<h1 k-label="hero_title">Build faster with Kooboo</h1>
<p k-label="hero_desc">One site, multiple languages.</p>
```

### 属性值

```html
<input k-attribute="placeholder k.label('search_placeholder')">
```

### Label 传参

后台 label 内容可以是：

```text
Welcome back, {name}
```

读取：

```html
<div k-content="k.label('welcome_user', { name: userName })"></div>
```

## `k-label`、`k.label()`、`k.t()` 怎么选

- `k-label`：最适合固定文本节点
- `k.label()`：最适合属性、脚本、API、返回消息
- `k.t()`：和 `k.label()` 读取的是同一套 Label 系统，但更适合把现成文案快速接入多语言；当前实现里缺 label 时会自动补一条

推荐约定：

- 页面结构里的固定文案默认优先 `k-label`
- 逻辑层和属性层默认优先 `k.label(...)`
- 只有在“先把现成文案快速接入多语言”时再用 `k.t(...)`

## 参数模式和路由模式

### 参数模式

参数模式的 URL 形态：

```text
/about?lang=en
/about?lang=zh
```

当前请求语言可以从 `lang` 或 `language` 查询参数进入，但系统生成链接时默认会输出 `lang=...`。

这里的 `/about?lang=en` 只是运行时输出结果，不是推荐手写的跳转字符串。AI 在写切换语言或 redirect 时，仍然应该优先调用 `k.site.multilingual.currentURLs()`、`getLangUrl(...)`、`getUrl(...)`。

适合：

- 现有站点 URL 已经上线，不想改路径结构
- 语言切换只需要保留在 query 上

### 路由模式

路由模式的 URL 形态：

```text
/en/about
/zh/about
```

开启后，Kooboo 会在渲染站内链接时自动把当前语言信息带到 URL 上。

例如页面里原本写：

```html
<a href="/about">About</a>
```

在路由模式下，渲染结果会按当前语言变成：

```html
<a href="/en/about">About</a>
```

或：

```html
<a href="/zh/about">About</a>
```

同样要注意：这些 `/en/about`、`/zh/about` 也是最终渲染结果，不是推荐 AI 自己手写的路径模板。

默认语言是否带前缀，取决于 `Default Language Prefix`：

- 关闭：默认语言通常保持 `/about`
- 开启：默认语言也会变成 `/en/about`

### 当前实现的一个重要约束

路由模式的匹配和链接拼接核心逻辑使用的是 `WebSite.Culture` 的 key，也就是语言代码本身。

因此生成页面时，请默认：

- `en` 对应 `/en/...`
- `zh` 对应 `/zh/...`

不要假设当前项目已经支持把 `en` 映射成 `/english/...` 这类自定义路径前缀。

另外，当前设置页和后端实现都更偏向两位语言码，默认按 `en`、`zh` 这类值来写。


## 切换语言的推荐写法

### 推荐方案：`currentURLs()`

这是最稳妥的方式，因为它会根据当前站点配置直接给出每种语言对应的 URL。

::: warning 不要 mutate / spread API 返回项
`currentURLs()` 的元素是 **运行时对象代理**，不是普通 JS 对象。禁止 `forEach` 里 `item.linkClass = ...`，禁止 `map` 里 `{ ...item, linkClass: ... }`（会导致模板里 `item.key` 等 camelCase 失效）。样式请用下方 `k-attribute` class 对象，或见 [KScript 返回对象与模板绑定](../K-Script/kscript-clr-objects.md) 显式构造 plain object。
:::

```html
<div>
    <script env="server">
        var links = k.site.multilingual.currentURLs()
    </script>

    <ul class="language-switcher">
        <li k-for="item in links">
            <a href="{item.URL}" k-attribute="class {'is-active': item.isActive}" k-content="item.name"></a>
        </li>
    </ul>
</div>
```

可用字段通常包括：

- `item.key`：语言代码，如 `en`、`zh`
- `item.name`：语言显示名
- `item.URL`：切换到该语言后的地址
- `item.isActive`：是否当前语言

## 后端 / KScript / API 如何配合

### API 返回当前语言消息

```ts
k.api.post((body) => {
    return {
        success: true,
        message: k.label("order_created", {
            orderNo: body.orderNo
        })
    }
})
```

### redirect 时保留当前语言

不要直接：

```ts
k.response.redirect("/cart")
```

应改成：

```ts
var redirectUrl = k.site.multilingual.getUrl("/cart")
k.response.redirect(redirectUrl)
```

这样无论站点当前是参数模式还是路由模式，跳转都能跟随当前语言。

### 登录 / 注册 / 下单后跳转

凡是这些动作：

- 登录成功回跳
- 注册成功跳首页
- 提交表单后跳感谢页
- 加入购物车后跳 `/cart`
- 下单后跳 `/checkout` 或 `/order-success`

都应优先通过 `k.site.multilingual.getUrl(...)` 生成目标 URL，而不是手工拼字符串。

## 推荐工作流

### AI 写页面时

1. 先把所有固定文案用 `k-label` 标出来
2. 把 `placeholder`、`title`、`aria-label`、`alt` 改成 `k.label(...)`
3. 如果有语言切换器，优先使用 `k.site.multilingual.currentURLs()`
4. 如果页面里有跳转逻辑，统一改成 `k.site.multilingual.getUrl(...)`

### AI 配置多语言时

1. 先调用 `get_site_info`，确认当前 `isMultiLingual`、`defaultCulture`、`cultures`、`enableSitePath`
2. 如果用户明确要求 AI 直接改配置，调用 `update_site_settings`
3. 如果需要用户自己确认后台配置，打开 `/_Admin/system/settings?SiteId=xxx`
4. 在 Basic 页开启 `Multilingual`，并选择默认语言与 `Parameter Mode` / `Router Mode`
5. 再打开 `/_Admin/content/labels?SiteId=xxx` 补齐各语言 label

### AI 跑 Preflight 时

多语言对象建议显式传 `multilingual: true`，这样 Preflight 会额外检查“不要手写多语言 URL / 跳转”的规则。

```json
{
  "taskType": "dynamic-page",
  "layers": ["frontend", "kooboo-template", "kscript", "integration"],
  "artifacts": [
    {
      "name": "checkout",
      "objectType": "Page",
      "multilingual": true
    }
  ]
}
```

这条检查会重点拦截：

- `href="/products?lang=en"`
- `href="/products?culture=en"`
- `href="/en/products"`
- `location.href = "/cart"`
- `k.response.redirect("/checkout")`

## 实战示例

下面是一个最小可用组合：

```html
<div>
    <script env="server">
        var links = k.site.multilingual.currentURLs()
        var searchPlaceholder = k.label("search_placeholder")
    </script>

    <header class="site-header">
        <a href="/" class="logo" k-label="site_name">Kooboo Store</a>

        <nav>
            <a href="/about" k-label="nav_about">About</a>
            <a href="/pricing" k-label="nav_pricing">Pricing</a>
            <a href="/cart" k-label="nav_cart">Cart</a>
        </nav>

        <input k-attribute="placeholder searchPlaceholder">

        <ul class="language-switcher">
            <li k-for="item in links">
                <a href="{item.URL}" k-attribute="class {'is-active': item.isActive}" k-content="item.name"></a>
            </li>
        </ul>
    </header>
</div>
```

## 常见误区

### 误区 1：把 `k-label` 用在 `<input>`

错误：

```html
<input k-label="search_placeholder">
```

正确：

```html
<input k-attribute="placeholder k.label('search_placeholder')">
```

### 误区 2：路由模式下自己手工拼 `/en/...`

错误：

```html
<a href="/{culture}/about">About</a>
```

正确做法：

- 普通站内链接直接写 `href="/about"`，交给系统渲染
- 逻辑跳转使用 `k.site.multilingual.getUrl("/about")`
- 切换语言使用 `k.site.multilingual.currentURLs()` 或 `getLangUrl(...)`

### 误区 3：参数模式下自己手工拼 `?lang=` 或 `?culture=`

错误：

```html
<a href="/products?lang=en">English</a>
<a href="/products?culture=en">English</a>
```

```js
location.href = "/cart?lang=en"
k.response.redirect("/checkout?culture=zh")
```

正确做法：

- 切换当前页语言时，优先使用 `k.site.multilingual.currentURLs()`
- 切换到指定页面的指定语言时，使用 `k.site.multilingual.getLangUrl("/products", "en")`
- 保持当前语言做逻辑跳转时，使用 `k.site.multilingual.getUrl("/cart")`
- 即使你知道参数模式最终会输出 `?lang=...`，也不要自己手工拼接

### 误区 4：只开多语言开关，不补 Label

开启多语言只决定站点支持多语言，不会自动帮你生成所有业务文案。最终显示什么，仍然取决于 Label 列表里有没有对应 key 和各语言值。

### 误区 5：把 `cultures` 当成“增量追加”

错误理解：

- 当前已有 `["en", "zh"]`
- 想新增 `fr` 时，只传 `["fr"]`

这样会把站点语言列表直接覆盖成只剩 `fr`。

正确做法：

- 先 `get_site_info`
- 基于当前值拼出完整目标列表
- 再调用 `update_site_settings`，例如传 `["en", "zh", "fr"]`

## 相关参考

- [Site 多语言标签开发指南](../Frontend/k-label.md)
- [Template binding](../Frontend/template-binding-syntax.md)
