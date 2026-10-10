# k.openApi

> 按站点 OpenAPI 定义调用外部 HTTP API，并管理鉴权凭据

## 概述

`k.openApi` 是 Kooboo 对 **开发 → Open API** 中导入的 Swagger/OpenAPI 规范的脚本封装。每个定义在后台有唯一 **名称**（如 `petstore`），对应：

```text
k.openApi.{名称}.{操作名}(参数…)
k.openApi.{名称}.authorize.{方法}(…)
```

保存 Open API 定义后，Kooboo 会解析 `jsonData`，为当前站点生成 TypeScript 类型与补全（`OpenApi` 命名空间）。**操作名与参数因规范而异**，以下说明通用规则；具体名称以 IDE 提示或后台定义为准。

::: tip 先配后台
1. [创建 Open API 定义](../../cms/development/openapis.md)（URL / 代码 / 模板）  
2. 在 [授权配置](../../cms/development/openapis.md#授权配置) 中填写 `securitySchemes` 所需凭据  
3. 再在 Code / Layout / Page 的 `env="server"` 脚本中调用  
:::

::: info 与 k.net.httpClient 的区别
| API | 用途 |
|-----|------|
| `k.openApi` | 按 OpenAPI 操作调用、自动拼 URL/鉴权/缓存 |
| `k.net.httpClient` | 任意 URL 的手写 HTTP 请求 |

本站对外暴露的 API 文档（Code Swagger）见代码资源与 `/_api/v2/codeOpenApi/document`，**不是** `k.openApi`。
:::

## 访问结构

```text
k.openApi
  └── {openApiName}          // 后台「名称」
        ├── authorize        // 凭据管理（KAuthorize）
        ├── {operationId}    // 每个 path + method 一个可调用函数
        └── …                // 规范中的 model 类型（OpenApi.*）
```

## 操作名（operationId）

由规范中 **路径 + HTTP 方法** 生成，规则为：

```text
{标准化路径}_{Method}
```

- **路径**：非字母数字替换为 `_`，若以数字开头则加前缀 `_`（如 `/pet/{petId}` → `_pet__petId_`）  
- **Method**：`Get`、`Post`、`Put`、`Delete`、`Patch` 等（与 OpenAPI `OperationType` 一致）

示例：`GET /pets` → `pets_Get`；`POST /pets` → `pets_Post`（以实际生成为准，请以 IDE 补全为准）。

## 调用参数

每个操作的函数参数顺序与 OpenAPI 参数位置一致：

| 位置 | 参数名 | 类型 |
|------|--------|------|
| 请求体 | `body` | 由 schema 生成的对象，或 `any` |
| 查询 | `query` | `{operationId}_query` 形状的对象 |
| 路径 | `path` | `{operationId}_path` 形状的对象 |
| 请求头 | `header` | `{operationId}_header` |
| Cookie | `cookie` | `{operationId}_cookie` |

仅传需要的对象即可；未使用的可省略。

```ts
k.api.get(() => {
    // 操作名以 IDE 补全为准，例如 GET /items 为 items_Get
    const page = k.openApi.myService.items_Get({
        query: { limit: 10 },
    })
    return page
})
```

### 鉴权与默认 authorize

若操作声明了 `security`，调用前须存在对应 **授权** 记录（后台或脚本写入）。未指定时使用该 Open API 下 **第一个** 授权名称。

抛出 `Api not authorize` 表示缺少或错误的 `securitySchemes` 凭据。

### 自定义身份验证

后台开启 **自定义身份验证** 时，请求发出前会执行配置中的 KScript，可修改注入的 `request.headers`、`request.querys` 等（见 [CMS 文档](../../cms/development/openapis.md#通用字段)）。

### 响应缓存

后台为某 **方法 + 路径模式** 配置的缓存命中时，直接返回内存中的上次结果（键含 URL 与 authorize 名），无需再次请求远端。

### 数组查询参数

定义上勾选 **使用逗号数组** 时，query 中的数组会序列化为 `a=1,2,3`；否则为重复 key `a=1&a=2`。

## authorize

`k.openApi.{name}.authorize` 管理该定义下的多组凭据（对应后台 [授权配置](../../cms/development/openapis.md#授权配置)）。

### list()

返回所有 `authorizeName` 字符串数组。

**参数**：无。

**返回值**：`string[]`。当前 Open API 定义下的授权名称列表。

```ts
const names = k.openApi.myService.authorize.list()
```

### addOrUpdate(name, securityKey, data)

写入或更新一组凭据。`securityKey` 为规范里 `securitySchemes` 的键名（非法字符会规范为 `_` 小写）。`data` 字段依方案类型而定，例如：

| 场景 | 常用字段 |
|------|----------|
| HTTP Basic | `username`, `password` |
| Bearer / API Key | `accessToken`, `name` |
| OAuth2 | `clientId`, `clientSecret`, `accessToken`, `refreshToken`, `expiresIn` |

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `name` | `string` | 是 | 授权配置名称。 |
| `securityKey` | `string` | 是 | OpenAPI `securitySchemes` 中的键名。 |
| `data` | `object` | 是 | 凭据数据，字段取决于安全方案类型。 |

**返回值**：`void`。

```ts
k.openApi.myService.authorize.addOrUpdate("sandbox", "bearer_auth", {
    accessToken: "xxx",
    name: "Bearer",
})
```

### addOrUpdate_{securityKey}(name, data)

按安全方案拆分的便捷方法（与 `addOrUpdate` 等价，IDE 中可见具体后缀）。

```ts
k.openApi.myService.authorize.addOrUpdate_bearer_auth("sandbox", {
    accessToken: "xxx",
})
```

### delete(name)

删除名为 `name` 的授权配置。

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `name` | `string` | 是 | 要删除的授权配置名称。 |

**返回值**：`void`。

```ts
k.openApi.myService.authorize.delete("sandbox")
```

### getAuthorizationUrl_{securityKey}(name, clientId, clientSecret)

OAuth2 **authorizationCode** 流：生成授权页 URL（并暂存 clientId/secret）。模板示例：

```html
<script env="server">
    var url = k.openApi.myService.authorize.getAuthorizationUrl_oauth2(
        "admin",
        "client-id",
        "client-secret"
    )
</script>
<a k-attribute="href {url}" target="_blank">去授权</a>
```

回调 URL 形如：`{scheme}://{host}/_api/openapioauth2callback/{siteId}/{authorizeId}/{securityKey}`。

## 运行时行为摘要

1. 从站点缓存加载 OpenAPI 文档与授权。  
2. 合并 `Base URL` / `servers` 得到请求绝对地址。  
3. 应用 `security`（Basic、Bearer、API Key、OAuth2 等）。  
4. 可选执行自定义身份验证脚本。  
5. 填充 path / query / header / cookie，发送 HTTP 请求。  
6. 按 Content-Type 解析响应（多为 JSON），返回 JavaScript 对象。

## TypeScript 与 IDE

每个 Open API **名称** 会生成 `OpenApi` 命名空间下的接口与 `k.openApi.{name}` 上的方法签名。重新保存定义后刷新类型缓存即可更新补全。

安全相关数据模型会随 OpenAPI 定义生成，例如 `Token`、`OAuth2`、`HttpBasic` 等类型。

## 相关文档

- [Open API（CMS）](../../cms/development/openapis.md) — 导入规范、缓存、后台授权  
- [k.net.httpClient](../net/httpclient.md) — 通用 HTTP  
- [k.security](../security/index.md) — JWT、哈希等  
- [身份验证（CMS）](../../cms/development/authentication.md) — 进站请求鉴权（非外部 API）
