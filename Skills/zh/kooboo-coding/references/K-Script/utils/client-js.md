# k.utils.clientJS

> 向页面 HTML 注入浏览器端 JavaScript 变量

## 概述

在 **SSR / 页面渲染** 流程中，将服务端数据写入当前响应的 HTML（`<script>`），浏览器端可通过注入的全局变量名读取。

仅对**当前页面 HTML 输出**有效；纯 JSON API 响应不需要此模块。

多语言文案、语言切换 URL 请使用 `k.label` / `k.t` 与 `k.site.multilingual`，见 [站点多语言场景指南](../../Scenario/multilingual-site-guide.md)。本文档不涵盖 `clientJS.i18n`。

## 相关 TypeScript 签名

```ts
interface ClientJs {
  setVariable(key: string, obj: any): void;
  monacoResource(callback: string): string;
}
```

## setVariable()

向当前响应 HTML 写入变量声明。`key` 会直接拼入 `var {key} = ...`，因此必须使用安全的 JavaScript 标识符，例如 `pageContext`；不要传入不可信文本、带连字符的名称、引号、分号或代码片段。

优先传入普通对象、数组和 Kooboo JSON 序列化器能够处理的服务端数据。该包装层不是直接调用 JavaScript 的 `JSON.stringify`：基础类型走单独的字符串转换路径，不应假设所有 JavaScript 类型都能保持原样。应避免传入序列化器无法处理的数据，包括：

- 循环引用
- 函数、DOM、undefined（序列化后会丢失或报错）
- Kooboo 运行时无法序列化的复杂类型

```ts
const payload = {
  siteName: k.site.info.name,
  count: 3,
  tags: ["a", "b"],
};

k.utils.clientJS.setVariable("pageContext", payload);
```

浏览器端使用注入的变量名（上例为 `pageContext`）。

## monacoResource()

从 Kooboo 服务端 `_Admin/monaco.html` 读取 **Monaco Editor** 所需的 `<script>` / `<link>` 标签片段，拼成一段 HTML 字符串，供你插入自定义页面的 `<head>`。

### 做什么

1. 解析后台模板 `monaco.html` 的 `<head>` 依赖（Monaco 的 JS/CSS）。
2. 对路径以 `/_Admin/assets/monaco` 开头的 `<script>`，自动加上 `onload="yourCallback()"`，便于在 Monaco 脚本加载完成后执行初始化。
3. 返回多行 HTML 字符串（不是 URL），需在页面模板或 Layout 中**原样输出**到 `<head>`。

### 典型用途

- 在**站点自定义页面**里嵌入代码编辑器（类似后台 Code 编辑器体验），而不是在普通业务 API 里调用。
- Monaco 脚本加载完成后，在 `onload` 回调里用 **`_monaco.instance`** 创建编辑器（见下节）。

### 加载后的全局对象：`_monaco`

`monacoResource` 注入的 script（形如 `/_Admin/assets/monaco.{hash}.js`）执行后，当前 Kooboo Monaco 前端资源会在浏览器全局暴露包装对象 **`_monaco`**，**不是** CDN 文档里常见的 `monaco` 全局变量。`monacoResource()` 只返回依赖标签，`_monaco` 对象来自这些前端资源本身。

| 成员 | 说明 |
| --- | --- |
| `_monaco.instance` | 完整的 Monaco AMD 模块，含 `editor`、`languages`、`Range`、`Uri`、`KeyCode` 等 |
| `_monaco.init` | Kooboo 自定义初始化（异步），可配置 `kview` / `kscript` / `classCompletion` 等补全 |

创建编辑器应使用：

```js
_monaco.instance.editor.create(container, options);
```

**不要**写 `monaco.editor.create(...)`（除非你自己另行引入了标准 Monaco 全局）。

可选：在回调开头 `var monaco = _monaco.instance;`，再按官方 Monaco 文档写法调用。

### 完整示例

```html
<div id="editor" style="height: 400px;"></div>

<script env="server">
  k.response.write(k.utils.clientJS.monacoResource("initMonacoEditor"));
</script>
<script>
  function initMonacoEditor() {
    var monaco = _monaco.instance;
    monaco.editor.create(document.getElementById("editor"), {
      value: "// code",
      language: "javascript",
      theme: "vs-dark",
      automaticLayout: true,
    });
  }
</script>
```

### 限制

- 依赖服务器磁盘上的 `_Admin/monaco.html`；不存在时返回空字符串。
- 面向**带 Monaco 的编辑页**场景；一般内容页、列表页无需调用。
- **全局名** — 脚本加载后为 `_monaco`；通过 `_monaco.instance` 访问 Monaco API。

## 注意事项

1. **调用时机** — `setVariable` 会立即写入当前响应。应在浏览器代码读取变量前调用，并根据生成 `<script>` 所需的位置安排调用。
2. **Monaco 回调** — 传入已经定义的函数名，例如 `"initMonacoEditor"`；不要传入 `"initMonacoEditor()"` 或 HTML/JavaScript 片段，因为该值会直接拼入 `onload="...()"`。
