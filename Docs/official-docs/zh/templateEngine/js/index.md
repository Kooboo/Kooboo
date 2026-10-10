# Script（站点脚本）

> 带路由的 `.js` 资源：全局工具、ES Module、与 Page 中的 `<script>` 配合

## 是什么

**Script** 是 Kooboo 站点中独立保存的 JavaScript 文件，注册后可通过 **URL** 访问（例如 `/assets/app.js`）。适合：

- 多页共用的工具函数、Vue/React 入口
- `importmap` + `type="module"` 架构中的 `/scripts/layout.js`
- 不宜塞进每个 Page 的大段前端逻辑

在 KScript 中由 **`k.site.scripts`** 管理，见 [k.site.scripts](../../api/site/js.md)。

::: tip 与 Code / API 的区别
- **Script**：静态 JS 文件，浏览器按 URL 加载  
- **Code（Api）**：服务端路由 + KScript，见 [k.site.codes](../../api/site/code.md)  
- Page 内 **`<script env="server">`**：服务端 KScript，不是 Script 资源
:::

## 在模板中引用

在 Layout 或 Page 的 HTML 中：

```html
<script src="/scripts/layout.js" type="module"></script>
```

路径为创建 Script 时配置的 **url**（未配置时默认为 `/{name}.js`）。可用 `k.site.scripts.getUrl(id)` 在服务端拼出相对路径。

### async / defer

创建资源时可设置 `async`、`defer`，对应生成标签属性，见 API 文档。

## 两种脚本分工

| 类型 | 写法 | 执行环境 |
|------|------|----------|
| 服务端 KScript | `<script env="server">` | 服务器渲染 |
| 前端 JS | 独立 Script 资源，或 Page 内 `<script>` / `<script type="module">` | 浏览器 |

不要依赖父级容器继承，应把 `env="server"` 直接写在 KScript 标签上。**推荐**：数据准备使用 View/Page 顶部的 `<script env="server">`；交互逻辑放在 View 底部或 Script 资源。

## ES Module + importmap（常见架构）

多页应用常在 **Layout** 的 `<head>` 声明 importmap，在 **Page** 末尾 `type="module"` 启动应用：

```html
<!-- Layout head -->
<script type="importmap">
{
  "imports": {
    "vue": "https://cdn.jsdelivr.net/npm/vue@3/dist/vue.esm-browser.prod.js"
  }
}
</script>

<!-- Page 底部 -->
<script type="module">
import { createApp } from 'vue'
import { store } from '/scripts/layout.js'
createApp({ /* ... */ }).mount('#app')
</script>
```

`/scripts/layout.js` 为站点 **Script** 资源，内部可 `export` 供各 Page `import`。

::: tip 无构建工具
浏览器原生 ES Module + importmap 即可，无需 Webpack/Vite 也能组织中大型前台（详见团队 CDN-Vue 参考实现）。
:::

## `type="module"` 与服务端 CodeBlock

服务端可使用带 `env="server"` 的 module 脚本 `import` 其他 CodeBlock，并向模板 `export` 变量，见 [模板绑定语法](../binding/#type-module)。

## 相关文档

- [Layout](../layout/) — 常放 importmap
- [Page](../page/)
- [Style](../css/)
- [k.site.scripts](../../api/site/js.md)
