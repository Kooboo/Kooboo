# KScript 返回对象与模板绑定

`k.*` API 返回值可能是 CLR 支持的 Kooboo 运行时包装对象，也可能是普通字典/数组，具体取决于 API。这些差异发生在 KScript 和 SSR 模板求值阶段；浏览器最终拿到的是渲染后的 HTML，而不是这些服务端对象。

在 `<script env="server">` 里写起来像 JavaScript，但 **SSR 模板绑定**（原生 `href` 插值、`k-for`、`k-content`、`k-attribute`）会通过 Kooboo 模板读取路径解析对象成员。脚本侧与模板侧对「改属性、展开对象、大小写」的行为不一致，是常见坑的来源。

> 边界：本文只解释模板绑定中的字段命名与对象拷贝问题，不负责发现或修改站点对象；相关操作遵循对应对象或 Function reference。

## 现象（以 `currentURLs()` 为例）

`k.site.multilingual.currentURLs()` 返回语言链接列表。模块 reference 与生成的 `kooboo.d.ts` 是编写代码时的字段契约：`key`、`name`、`URL`（`[KeepCase]`）、`isActive`。生成定义中的 camelCase 不保证所有 CLR 包装对象的可枚举键名都相同。

### 坑 1：在 `forEach` 里给 item 挂新字段

```javascript
langLinks.forEach((item) => {
    item.linkClass = item.isActive ? 'active' : 'muted'
})
```

在普通 JavaScript 对象上合法，但不要把这种变异当成向 **CLR 支持的运行时对象**增加模板字段的可靠方式：

- 脚本侧赋值与模板侧成员读取使用不同的读取器；不能期待模板把新挂载的 `linkClass` 当作声明字段或源字段处理。

### 坑 2：用 `{ ...item, linkClass: '...' }` 再 `map`

```javascript
langLinks = langLinks.map((item) => ({
    ...item,
    linkClass: '...'
}))
```

会得到 **JavaScript 普通对象**，模板可以读取 `linkClass`，但对于 CLR 支持的源对象：

- 展开复制出来的键名可能保留 **CLR 名称**（`Key`、`Name`、`IsActive`），与生成定义的 camelCase 不一致。
- 模板对 **纯 JS 对象** 使用 **精确键名**，大小写必须与对象真实键名一致。
- 结果：`item.key` 失效，只剩 `item.Key`；与文档、其它页面写法冲突。

仅当来源可能仍是 CLR 包装对象或其他不透明运行时对象时，才应用这个警告。如果对应 API reference 已明确返回普通字典/数组，应遵循该 reference 的字段契约。

## 机制摘要

| 层级 | 行为 |
|------|------|
| **KScript 返回对象** | 运行时形态因 API 而异。字段应来自模块 reference、生成的 `kooboo.d.ts` 和已确认的源码契约；不要通过探测推断可枚举键名。 |
| **SSR 模板 `k-for`** | 循环变量可能仍引用原始运行时值；模板成员读取走 Kooboo 模板读取器，可能不同于脚本侧对象变异。 |
| **`{...item}` / `map` 返回新对象** | 变为普通 JS 对象；模板按**精确键名**读取。源对象是 CLR 包装对象时，展开可能复制 CLR 名称而非 d.ts 名称。 |
| **Commerce 部分 API** | `ToKscriptModel` 会递归把支持的值转成 camelCase 字典、数组和基础值。这是**服务端 DTO 契约**，不是全局规则。 |

`SiteMultilingualInfo` 上 `URL` 带 `[KeepCase]`，生成定义时保持 `URL`，不要用 `url`。

## AI 必须遵守的写法

### 1. 优先：在模板里算 UI 状态（推荐）

不要把 `linkClass` 预先挂到 item 上；用 `k-attribute` 的 class 对象语法（与 [multilingual-site-guide](../Scenario/multilingual-site-guide.md) 一致）：

```html
<script env="server">
    var links = k.site.multilingual.currentURLs()
</script>
<ul>
    <li k-for="item in links">
        <a href="{item.URL}" k-attribute="class {
            'text-white bg-cta/30 border border-cta/50 font-semibold': item.isActive,
            'text-white/70 hover:text-white hover:bg-white/10 border border-white/20': !item.isActive
        }" k-content="item.name"></a>
    </li>
</ul>
```

绑定字段名以 **`kooboo.d.ts` / 模块 reference 的 camelCase** 为准（`item.name`、`item.isActive`），`URL` 保持大写。

### 2. 必须在 KScript 里派生新字段时：显式构造 plain object

**禁止**依赖 `...item`  spread。根据文档字段**逐字段按需拷贝**，并统一 camelCase(**没用上的字段可以不必拷贝**)：

```javascript
var links = k.site.multilingual.currentURLs()
links = links.map(function (item) {
    return {
        key: item.key,
        name: item.name,
        URL: item.URL,
        isActive: item.isActive,
        linkClass: item.isActive
            ? 'text-white bg-cta/30 border border-cta/50 font-semibold'
            : 'text-white/70 hover:text-white hover:bg-white/10 border border-white/20'
    }
})
```

如果字段读取失败，先回到对应模块 reference、`kooboo.d.ts` 或已确认的公开源码契约。不要用运行时 introspection 或方法枚举来猜对象结构。对于 CLR 包装对象，若文档输出契约是 `key`/`name`，不要暴露展开得到的 `Key`/`Name`；对于明确返回普通字典的 API，遵循其实际文档键名。

### 3. 禁止

- 不要指望在 CLR 包装的 `k.*` 返回值上执行 `item.newField = ...` 后，模板就能读取该字段。
- 当来源是 CLR 包装对象或其他不透明对象时，不要假设 `{ ...item, ... }` 或 `Object.assign({}, item)` 会保留 d.ts 的字段大小写。
- 用 `JSON.parse(JSON.stringify(...))` 当通用「转纯对象」方案（可能丢类型、键名不可控、含不可序列化成员时失败）。
- 不要用运行时 introspection 或方法枚举发现未经 reference 确认的 API；站点写入遵循对应对象/Function reference。

### 4. 自检（写进 reasoning）

- 来源是 CLR 包装/不透明运行时对象，还是 reference 已明确承诺的普通字典/数组？→ 先判断类型，再应用对应规则，不要把所有 `k.*` 结果视为同一种对象。
- 是否只需 class / href 等展示逻辑？→ `href` 保持原生属性，动态 class 使用 `k-attribute`，不在脚本里变异 item。
- 是否必须 `map` 出新字段？→ 显式字段表，禁止 spread。
- 是否想通过探测确认字段/方法？→ 停止；改为读取模块 reference、`kooboo.d.ts` 或已确认的公开源码契约。

## 与其它文档的关系

- 模板绑定能力：[template-binding-syntax.md](../Frontend/template-binding-syntax.md)（`k-attribute` class 对象）
- 多语言切换示例：[multilingual-site-guide.md](../Scenario/multilingual-site-guide.md)
- 脚本变量命名：[code-style.md](../Frontend/code-style.md)。API 字段名以对应模块 reference、生成的 `kooboo.d.ts` 和已确认的源码契约为准。
