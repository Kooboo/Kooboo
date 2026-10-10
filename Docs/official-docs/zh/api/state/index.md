# k.state

> 当前请求渲染上下文中的视图数据（ViewData）

## 概述

`k.state` 在**当前 HTTP 请求**的视图数据栈中读写键值，供页面、布局、视图模板绑定使用（例如 `k-state-key` 或模板中读取同名数据）。数据随请求结束而释放，**不**跨请求持久化。

::: tip 与 k.cache、k.session 的区别
| API | 作用域 | 典型用途 |
|-----|--------|----------|
| **`k.state`** | 单次请求内的视图渲染 | 控制器/CodeBlock 向模板传 `product`、`breadcrumb` |
| **`k.cache`** | 站点级内存，可过期 | 跨请求共享的计算结果 |
| **`k.session`** | 访客会话 | 登录态、购物车临时数据 |
:::

须在参与 **HTML 页面渲染** 的流程中使用（页面 CodeBlock、布局脚本等）。纯 `k.api` 返回 JSON、不渲染模板的请求中，`set`/`get` 通常只对当次脚本可见，不会出现在响应 HTML 里。

## TypeScript 定义（节选）

```ts
interface KState {
  set(key: string, value: any): void;
  setCurrent(key: string, value: any): void;
  get(key: string): any;
}
```

## set()

向视图数据栈**压入**一条命名数据（与后续 `set` 可形成层级，由渲染引擎解析）。

```ts
const product = { id: "1", name: "Sample" }
k.state.set("product", product)
```

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `key` | `string` | 是 | 模板中使用的键名 |
| `value` | `any` | 是 | 任意可序列化对象或值 |

**返回：** `void`。

## setCurrent()

写入或覆盖**当前栈顶**帧上的键；若栈为空，行为与 `set` 相同。

```ts
k.state.set("items", list)
k.state.setCurrent("items", updatedList)
```

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `key` | `string` | 是 | 键名 |
| `value` | `any` | 是 | 新值 |

**返回：** `void`。

## get()

按键读取视图数据（从当前数据上下文解析，含外层栈）。

```ts
const product = k.state.get("product")
```

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `key` | `string` | 是 | 键名 |

**返回：** 先前 `set` / `setCurrent` 写入的值；不存在时为 `undefined` 或空（以运行时为准）。

## 示例：列表页向模板传数据

```ts
// 页面 CodeBlock
const items = k.DB.sqlite.table("Product").all()
k.state.set("products", items)
k.page.setTitle("产品列表")
```

```ts
// 另一段脚本读取
const products = k.state.get("products")
```

## 相关文档

- [k.page](../page/) — 当前页 `<title>` / `<meta>`
- [k.cache](../cache/) — 站点缓存
- [k.session](../session/) — 会话
