# k.utils.clientJS

> 向页面输出客户端 JavaScript 变量

## 概述

`k.utils.clientJS` 在**当前 HTML 响应**中注入 `<script>`，将服务端对象以 `JSON.parse` 形式暴露给浏览器（须处于页面渲染流程）。

## setVariable()

```ts
const data = { firstname: "Kooboo", count: 3 }
k.utils.clientJS.setVariable("pageData", data)
// 浏览器: var pageData = JSON.parse("...")
```

| 参数 | 类型 | 说明 |
|------|------|------|
| `key` | `string` | 全局变量名 |
| `obj` | `any` | 可 JSON 序列化的对象 |

**返回：** `void`，直接写入当前 HTML 响应。

## monacoResource()

返回 Monaco 编辑器所需资源标签 HTML 片段，供回调 `onload` 使用。

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `callback` | `string` | 是 | Monaco 主脚本加载后调用的全局函数名 |

**返回：** `string`，包含 `<script>` / `<link>` 的 HTML 片段。

```ts
const resources = k.utils.clientJS.monacoResource("initEditor")
k.response.write(resources)
k.response.write(`
<div id="editor"></div>
<script>
function initEditor() {
    monaco.editor.create(document.getElementById("editor"), {
        value: "console.log('Kooboo')",
        language: "javascript"
    })
}
</script>
`)
```

## i18n()

返回当前站点多语言配置对象。

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `all` | `boolean` | 否 | 是否返回全部语言配置 |

**返回：** `any`，当前站点多语言配置对象。

```ts
const cfg = k.utils.clientJS.i18n()
const all = k.utils.clientJS.i18n(true)
```

## 相关文档

- [k.label](../label/) — 多语言标签
