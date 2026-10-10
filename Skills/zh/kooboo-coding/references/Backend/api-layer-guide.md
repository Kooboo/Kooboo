# Api Layer Guide API 层规范

## 路由设计

相关操作归入同一个带 `{action}` 的 API 对象；基础路径用 `k.api.get("/", ...)`、`k.api.put("/", ...)`，独立操作用具名回调。只在最后给 GET 添加 `k.api.get(() => k.api.notFound())`，其他方法不加通配兜底。参照 [App Base 服务](app-base-services.md)。

### Url 与路由的关系

`create_api` 时的 `Url` 参数决定了文件内允许的路由模式：

| Url 定义 | 含义 | 示例 |
|---------|------|------|
| `/api/auth` | 单一路由，只能用通配 | `k.api.get(()=>{})` → GET /api/auth |
| `/api/auth/{action}` | 允许动态子路由 | `k.api.post("login",()=>{})` → POST /api/auth/login |

### 动态路由

如果需要在一个 API 文件中定义多个子路由（如 `/api/auth/login`、`/api/auth/logout`），创建时 `Url` 必须包含 `{action}`：

```ts
// create_api 时设置 Url: /api/auth/{action}

// POST /api/auth/login
k.api.post("login", () => {
    return { action: "login" }
})

// POST /api/auth/logout
k.api.post("logout", () => {
    return { action: "logout" }
})

// GET /api/auth
k.api.get("/", () => {
    return { message: "auth endpoint" }
})

k.api.get(() => k.api.notFound())
```

### 路由顺序

具名回调（包括基础路径的 `"/"`）放在前面，GET 的通配 404 兜底放在最后。不要从查询参数 `action` 判断实际路径；不要添加 POST、PUT、DELETE 的兜底回调。

```ts
k.api.get("/", () => { return { message: "auth endpoint" } })
k.api.get("status", () => { return { available: true } })
k.api.post("login", () => { return "login" })

k.api.get(() => k.api.notFound())
```

### {action} 检查

如果 `create_api` 时的 `Url` 不包含 `{action}`：
- 允许使用 `k.api.get(()=>{})` 等通配路由
- **不允许**使用 `k.api.get("sub", ...)` 动态子路由，否则不会生效

## 参数获取

### Query String（GET/DELETE 参数）

优先通过回调参数按名称绑定，字符串转换后再校验；需要限定取值来源时才显式读取 `k.request`。

```ts
// URL: /api/products/{action}
k.api.get("/", (id: string | null, page: string | null) => {
    const pageNumber = page == null ? 1 : Number(page)

    if (!Number.isInteger(pageNumber) || pageNumber < 1) {
        return k.api.httpCode(400, { message: "Invalid page" })
    }

    const result = { id, page: pageNumber }

    return result
})

k.api.get(() => k.api.notFound())
```

### Body（POST/PUT 参数）

直接在回调函数参数中接收：

```ts
// @k-url /api/user/{action}

k.api.post("create-user", (body) => {
    const { userName, password, email } = body
    return { success: true, data: { userName } }
})
```

## 响应格式

直接返回即可自动 JSON 序列化：(推荐封装 Utils 进行统一响应格式)

```ts
k.api.get(() => {
    return { success: true, data: { id: 1 } }
})
```

### 响应辅助方法

| 方法 | 用途 |
|------|------|
| `k.response.json(obj)` | 返回 JSON |
| `k.response.setHeader(key, value)` | 设置响应头 |
| `k.response.redirect(url)` | 重定向 |
| `k.response.notFound()` | 返回 404 |
| `k.response.unauthorized()` | 返回 401 |

### HTTP 状态码

| 方法 | 状态码 |
|------|--------|
| `k.api.ok()` | 200 |
| `k.api.badRequest()` | 400 |
| `k.api.unauthorized()` | 401 |
| `k.api.notFound()` | 404 |

::: warning `k.api.xxx()` 不支持传参
`k.api.ok()` / `k.api.badRequest()` / `k.api.unauthorized()` / `k.api.notFound()` **不接受任何参数**，只能单独调用。

**❌ 错误：**
```ts
return k.api.unauthorized(result.error);  // 不支持传参，会报错
```

**✅ 正确：**
```ts
k.response.json({ message: result.error, success: false })
return k.api.unauthorized();
```
:::

## App 首版 mock API

当已授权实施明确采用 mock API 时，在对应 API 代码对象中直接声明业务样本并返回 `data`。list/get 返回样本；create/update/delete 以及同类写入只返回成功信号，不必真正改数据。不需要为了搬运常量再创建 Service CodeBlock：

```ts
const requests = [
  { id: "req-001", title: "办公设备采购", status: "pending" }
]

k.api.get(() => {
  return { success: true, data: requests }
})

k.api.post(() => {
  return { success: true }
})

k.api.put(() => {
  return { success: true }
})

k.api.delete(() => {
  return { success: true }
})
```

- 样本必须符合 Blueprint 的 Business Object、Attribute、State 与 App Plan Data Model 语义；
- list/get 可以做最小 query 参数读取和确定性筛选；写入类接口返回 `{ success: true }` 即可；
- Kooboo API 每次请求都会重新执行脚本，模块级变量不会跨请求保留。这是预期行为，不得因此改用 SQLite，也不得把该项改成 `new-store`；
- 不在 mock API 中实现复杂业务规则、权限或持久化，不得调用 `k.DB.sqlite`，也不得为该项创建表；
- 前端通过 HTTP service 调用该 API，按成功信号处理写入结果；禁止再生成前端 `mock.js`、复制同一份样本，或用 localStorage 冒充写入；
- 同一 Milestone 中 `strategy=new-store` 的项可以接入真实存储；该项后续改为真实数据时保持 URL 和响应契约，复杂逻辑再移入 Service/Data 层。

## create_api 时的对象 Name

使用 `create_api` 创建 **API 代码对象**时，其 `name` 参数应为**简洁小写标识**（如 `product`、`auth`、`init-sqlite`），与 [Backend 对象创建](../Functions/backend-create.md) 中 create_api 说明一致。

**`Services.xxx` / `Utils.xxx` 形式的名称**仅用于 **Service / 工具 CodeBlock**（`create_codeblock`），勿把 API 对象的 `name` 写成 `Services.xxx`。

## 调用 Service（唯一合法方式）

API 层**只通过文件顶部的静态 `import`** 引用其它 CodeBlock 导出的函数；业务逻辑在 Service 内实现，API 只做参数与返回。

**✅ 正确：**

```ts
import { getCurrentAppUser } from "./Services.user"

k.api.get(() => {
    return getCurrentAppUser()
})
```

**❌ 错误（Kooboo 无此语法，禁止写入）：**

```ts
k.api.get(() => {
    return k.req.services.getCurrentAppUser()
})
```

- **禁止** `k.req.services`、`k.request.services`，以及任何未在 KScript reference 中出现的「请求对象上挂 Service 再按名调用」写法。
- Service 文件内必须使用 `export function ...`；未 `export` 的顶层函数无法被 API `import`。

## CodeBlock 组织

API 层负责 import Service 和 Utils：

```ts
import { getProductByList } from "./Services.product"
import { successResponse } from "./Utils.response"

k.api.get("list", () => {
    const { list, pager } = getProductByList()
    return successResponse({
        list,
        pager: {
            total,
            page,
            page_size
        }
    })
})
```

## HTTP 方法

| 方法 | 用途 | 路由后缀 |
|------|------|----------|
| `k.api.get()` | 查询、获取数据 | ?querystring |
| `k.api.post()` | 创建新资源 | + body |
| `k.api.put()` | 更新资源 | + body |
| `k.api.delete()` | 删除资源 | + id 参数 |

## 重定向

```ts
// 相对路径
k.api.get(() => {
    k.response.redirect("/login")
})

// 绝对路径
k.api.get(() => {
    k.response.redirect("https://example.com")
})
```

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| url | string | 是 | 目标 URL |
| absolute | boolean | 否 | 是否为绝对路径（包含协议） |

## Form 数据

获取表单提交的数据（用于文件上传场景）：

```ts
k.api.post(() => {
    const data = k.request.form
    return { received: data }
})
```

::: tip 说明
Form 数据通常配合文件上传使用。纯文件上传场景可使用 [KFile 模块](../K-Script/file/index.md)。
:::

## 注意事项

1. **不要在 API 层写业务逻辑**，只负责参数获取、调用 Service、返回结果；`mock-api` 交付可按上文返回代码内样本，写入只返回成功信号，但不扩展为 SQLite 或其它持久化实现
2. **验证参数**：检查必填参数是否存在
3. **错误处理**：返回适当的 HTTP 状态码
4. **Service 层逻辑仅通过 `import` 调用**（见上文「调用 Service」），不要在 API 层直接操作真实数据源，也不得使用臆造的 `k.req.services` 等；`mock-api` 的代码内样本与写入成功信号按上文例外处理
5. **调试输出使用 `k.logger`**，不要使用 `console.log` / `console.error`（Kooboo 服务端不支持 console 方法）
6. **`k.api.xxx()` 不传参**：状态码方法（`ok` / `badRequest` / `unauthorized` / `notFound`）不接受参数，响应体用 `k.response.json()` 单独设置
