# 模板绑定语法

> 服务端 `k-*` 指令：在 Layout / Page / View 中绑定数据与属性

Kooboo 的 `k-if`、`k-for`、`k-content`、`k-attribute` 等模板绑定会自动在**服务器端**求值。只有需要执行 KScript 的 `<script>` 才使用 **`env="server"`**。这些绑定用于 [Page](../page/)、[View](../view/) 等 HTML 资源，与浏览器中的 Vue/React 无关。

## env="server" {#env-server}

让 `<script>` 作为服务端 KScript 执行。应把该属性直接写在需要执行的脚本上。

::: warning 不要写在普通容器上
`k-if`、`k-for`、`k-content`、`k-attribute` 不需要父元素声明 `env="server"`。虽然服务端环境会向后代继承，但把它写在 `<div>`、`<section>` 等 UI 容器上，可能导致内部的浏览器脚本被当成 KScript 处理。
:::

```html
<script env="server">
    var msg = "hello";
    var imageUrl = "/images/logo.png";
    var list = [{ title: "apple" }, { title: "orange" }];
</script>
<div k-content="msg"></div>
<img k-attribute="src imageUrl">
<div k-for="item in list" k-content="item.title"></div>
```

页面级变量可单独写在脚本标签上：

```html
<script env="server">
    var msg = "hello";
</script>
<div k-content="msg"></div>
```

## k-define

在模板中声明一个值，供当前元素及后续绑定使用。语法为 `变量名 值`；字符串使用引号，JSON 对象使用反引号包裹。

```html
<div k-define='profile `{"name":"Kooboo","role":"admin"}`'>
    <strong k-content="profile.name"></strong>
    <span k-content="profile.role"></span>
</div>
```

## k-if

按条件输出元素。

```html
<script env="server">
    var isShow = true;
</script>
<div k-if="isShow">hello</div>
```

## k-else-if

在前面的同级 `k-if` 条件不成立时继续判断。条件分支应放在同一 DOM 层级，并按顺序组成一条分支链。

```html
<script env="server">
    var isAdmin = false;
    var isMember = true;
</script>

<p k-if="isAdmin">Administrator</p>
<p k-else-if="isMember">Member</p>
<p k-else>Guest</p>
```

## k-else

当前面同级的 `k-if` 和 `k-else-if` 均不成立时渲染，不需要填写属性值。

```html
<p k-if="isLoggedIn">Welcome back</p>
<p k-else>Please sign in</p>
```

## k-for

循环渲染。

```html
<script env="server">
    var list = [{ label: "apple" }, { label: "orange" }];
</script>
<div k-for="item in list" k-content="item.label"></div>
```

## k-content

绑定文本内容。

```html
<script env="server">
    var blogTitle = "hello";
</script>
<h1 k-content="blogTitle"></h1>
```

## k-attribute

统一用于属性绑定；多个属性用 `;` 分隔。

```html
<script env="server">
    var imageUrl = "/images/logo.png";
    var linkHref = "/about";
    var isActive = true;
    var itemId = 123;
    var classObj = { active: true, 'text-danger': false };
    var attrsObj = { id: 'container', class: 'wrapper' };
</script>

<div>
    <a k-attribute="href linkHref">About Us</a>
    <img k-attribute="src imageUrl">
    <img k-attribute="src imageUrl; alt 'Logo'">
    <a k-attribute="href /detail?id={itemId}">Detail</a>
    <div k-attribute="class {active: isActive}">Content</div>
    <div k-attribute="class classObj">Content</div>
    <div k-attribute="attrsObj"></div>
</div>
```

动态属性名：

```html
<a k-attribute="[dynamicKey] linkUrl">About</a>
```

::: warning 不支持 JS 表达式拼接
`k-attribute` **不执行** `+` 等运算。路径拼接用花括号插值：

```html
<a k-attribute="href /detail?id={itemId}">Detail</a>
```

`{var}` 只接受变量名或字段路径，不支持 `{pageIndex - 1}` 这类运算；请先算好再绑定。
:::

## k-label

按当前语言渲染标签文案。

```html
<h1 k-label="title">默认标题</h1>
```

更多见 [k.label / k.t](../../api/label/index.md)。

## type="module" {#type-module}

服务端 module 脚本可 `import` 其他 CodeBlock 并向模板 `export` 符号：

```html
<script type="module" env="server">
    import { getBlogDetail } from "./Services.Blog";
    const blogDetail = getBlogDetail();
    export { blogDetail }
</script>
<div k-content="blogDetail.title"></div>
```

## 相关文档

- [Page](../page/) · [View](../view/)
- [k-data](../k-data/) — 声明式数据（替代 k-query）
