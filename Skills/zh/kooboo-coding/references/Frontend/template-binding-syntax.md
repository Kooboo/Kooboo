# 模板绑定语法

Kooboo 模板由服务端执行的 `k-xxx` 指令与原生 `href` URL 插值共同组成。

::: tip 与 k-data 的分工
- **动态取数**（查询 commerce/content/sqlite、读请求参数、分页筛选）：优先 [`k-data`](k-data/basic.md)，不要用本节 `<script env="server">` 直接调 `k.*` 查库。
- **本节职责**：原生 `href` URL 插值、`k-if` / `k-for` / `k-content` / `k-attribute` 等模板绑定语法，以及在 k-data 无法覆盖时的补充服务端脚本。
:::

## env="server"

`env="server"` 只用于确实需要执行 KScript 的 `<script>`，例如读取没有 k-data 映射的 `k.*` API，或 `type="module"` import CodeBlock。

::: danger 不要写在普通容器上
`k-if`、`k-for`、`k-content`、`k-attribute` 和 `<k-data>` 不需要父级元素声明 `env="server"`。不要在 `<div>`、`<section>`、Page/Layout/View 外层等普通 UI 容器上添加它。

`env="server"` 会向后代继承；如果容器内还有普通客户端 `<script>`，该脚本可能被当成服务端脚本处理，导致点击、登录、购物车等前端交互失效。
:::

### 示例

```html
<script env="server">
    var msg = "hello";
    var imageUrl = "/images/logo.png";
    var list = [{ title: "apple" }, { title: "orange" }];
</script>
<div>
    <div k-content="msg"></div>
    <img k-attribute="src imageUrl">
    <div k-for="item in list" k-content="item.title"></div>
</div>
```

### 独立脚本示例

如果脚本需要在页面级别定义变量（供多个子元素使用），可以单独声明 `env="server"`：

```html
<script env="server">
    var msg = "hello";
</script>
<div k-content="msg"></div>
```

## k-if

判断是否渲染元素。

### 示例

```html
<script env="server">
    var isShow = true;
</script>
<div k-if="isShow">hello</div>
```

## k-for

循环渲染元素。

::: warning 数据源来自 `k.*` API 时
数组元素多为运行时对象代理，不要在 KScript 里 `item.foo = ...` 或 `{...item}` 后再绑定。见 [KScript 返回对象与模板绑定](../K-Script/kscript-clr-objects.md)；派生 class 等优先用下文 `k-attribute` 的 class 对象语法。
:::

### 示例

```html
<script env="server">
    var list = [{ label: "apple" }, { label: "orange" }];
</script>
<div k-for="item in list" k-content="item.label"></div>
```

## k-content

绑定文本内容。

### 示例

```html
<script env="server">
    var blogTitle = "hello";
</script>
<h1 k-content="blogTitle"></h1>
```

## 链接 URL 绑定（`href`）

链接 URL 直接使用原生 `href`。Kooboo 会通过 URL 渲染路径解析 `href` 中的 `{...}` 占位符，并处理路由与多语言地址。
占位符只能引用已有变量或字段路径；数学运算或条件结果必须先预计算，再插入 `href`。

```html
<a href="{linkHref}">About Us</a>
<a href="/detail/{itemId}">Detail</a>
```

链接还需要其它动态属性时，保留原生 `href`，只把剩余属性交给 `k-attribute`：

```html
<a href="{linkHref}" k-attribute="class {active: isActive}">About Us</a>
```

## 动态属性绑定（`k-attribute`）

除 `href` 外的动态属性使用 `k-attribute`；单属性和多属性都通过同一语法表达，多属性用 `;` 分隔。

### 示例

```html
<script env="server">
    var imageUrl = "/images/logo.png";
    var linkHref = "/about";
    var isActive = true;
    var isChecked = false;
    var itemId = 123;
    var classObj = { active: true, 'text-danger': false };
    var attrsObj = { id: 'container', class: 'wrapper' };
</script>
<div>

    <!-- 链接 URL 使用原生 href -->
    <a href="{linkHref}">About Us</a>
    <a href="/detail/{itemId}">Detail</a>

    <!-- 其它属性使用 k-attribute -->
    <img k-attribute="src imageUrl">

    <!-- 字符串字面量（加引号） -->
    <img k-attribute="src imageUrl; alt 'Logo'">

    <!-- class 对象语法：条件为真时添加类名 -->
    <div k-attribute="class {active: isActive; checked: isChecked}">Content</div>

    <!-- class 数组语法 -->
    <div k-attribute="class ['static', 'active']">Content</div>

    <!-- class 来自对象变量（自动解包） -->
    <div k-attribute="class classObj">Content</div>

    <!-- 静态 class + 动态 class 共存，渲染时自动合并 -->
    <a class="text-xs font-medium px-2 py-1"
       k-attribute="class {'text-white bg-blue-600': isActive, 'text-gray-500': !isActive}">
      Button
    </a>

    <!-- 属性展开（无属性名，对象所有键展开为属性） -->
    <div k-attribute="attrsObj"></div>
</div>
```

### 动态属性名

用 `[expr]` 作属性名，在运行时动态解析。

```html
<script env="server">
    var dynamicKey = 'data-target';
    var targetValue = 'details';
</script>
<button k-attribute="[dynamicKey] targetValue">Open</button>
```

::: tip 适用场景

- 图片 src、背景图 URL
- 动态 class、style
- 自定义 data 属性
- 属性批量展开
- 动态属性名
:::

::: warning 不支持 JS 运算符拼接
`k-attribute` 解析器**不执行 JS 表达式**，不能使用 `+` 拼接：
```html
<!-- ❌ 错误 -->
<div k-attribute="onclick 'editCategory(\'' + cat.id + '\')'"></div>
```
使用 `{var}` 花括号插值即可：
```html
<!-- ✅ 正确 -->
<button k-attribute="data-url /detail/{itemId}">Detail</button>
```
花括号插值 `{var}` 同样**不执行表达式运算**（如 `{pageIndex - 1}`、`{price * qty}`），只接受变量名或字段路径。需要预计算时，再绑定。
:::

## k-label

绑定多语言标签，自动根据当前语言渲染文本。

### 示例

```html
<h1 k-label="title">默认标题</h1>
```

更多用法（`k-content`、`k-attribute` 结合 `k.label()`、参数替换等）参见 `k-label` 参考文档。

## type="module"

与服务端 codeblock 联动。

### 示例

```html
<script type="module" env="server">
    import { getBlogDetail } from "./Services.Blog";
    import { formatDate } from './Utils';
    const blogDetail = getBlogDetail();
    export {
        blogDetail,
        formatDate
    }
</script>
<div>
    <div k-content="blogDetail.title"></div>
    <div k-content="formatDate(blogDetail.created)"></div>
</div>
```
