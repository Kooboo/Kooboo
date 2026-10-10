# k.account — 登录与身份

> `k.account` 根对象上的登录态、登录/登出与令牌相关 API

## TypeScript 定义（节选）

```ts
interface KAccount {
  isLogin: boolean;
  login(username: string, password: string, code?: string): User;
  loginOrganization(username: string, password: string): User;
  logout(returnUrl?: string): void;
  ensureLogin(redirectUrl: string): void;
  generateToken(userName: string, expireIn: number): string;
  getAccessToken(): string;
  setIdentity(token: string): void;
  thirdPartyLogin(thirdPartyName: string, userName: string): ThirdPartyLoginInfo;
  renewInfo(): RenewInfo;
  user: KUser;
  organization: KOrganization;
  oAuth: kOAuth;
}
```

## isLogin

当前 HTTP 请求是否已携带有效的平台用户身份（`context.User` 非空）。

```ts
k.api.get(() => {
    return { isLogin: k.account.isLogin }
})
```

## login()

使用用户名和密码登录，成功时写入 JWT Cookie 并设置当前请求用户。

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| username | `string` | 是 | 平台用户名 |
| password | `string` | 是 | 密码 |
| code | `string` | 否 | 验证码等（若账号策略要求） |

```ts
k.api.post(() => {
    const username = k.request.form.username
    const password = k.request.form.password
    const user = k.account.login(username, password)
    return {
        isLogin: k.account.isLogin,
        userName: user?.userName
    }
})
```

失败时返回 `null`，`isLogin` 仍为 `false`。

## loginOrganization()

使用 **组织用户** 形式登录：内部将用户名处理为 `{username}@{当前站点组织 Id}` 再调用 `login`。

适用于站点绑定组织、使用组织子账号的场景。

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `username` | `string` | 是 | 组织子账号用户名，不包含组织后缀。 |
| `password` | `string` | 是 | 密码。 |

**返回值**：`User | null`。登录成功时返回用户对象；失败时返回 `null`。

```ts
k.api.post(() => {
    const username = k.request.form.username
    const password = k.request.form.password
    const user = k.account.loginOrganization(username, password)
    return {
        isLogin: k.account.isLogin,
        userName: user?.userName
    }
})
```

## logout()

清除登录 Cookie。若站点开启 SSO，会跳转到 SSO 登出地址；否则在提供 `returnUrl` 时重定向到该 URL。

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| returnUrl | `string` | 否 | 登出后跳转路径（相对站点） |

```ts
k.api.get(() => {
    k.account.logout("/")
    return { isLogin: k.account.isLogin }
})
```

**返回值**：`void`。

## ensureLogin()

若未登录，则 **302 重定向** 到 `redirectUrl` 并结束响应；已登录则无操作。常用于必须登录才能访问的页面或 API。

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `redirectUrl` | `string` | 是 | 未登录时跳转到的 URL。 |

**返回值**：`void`。

```ts
k.api.get(() => {
    k.account.ensureLogin("/login?return=" + encodeURIComponent(k.request.url))
    return k.account.user.current
})
```

## generateToken()

为指定平台用户生成 JWT 登录令牌（通常需具备相应管理权限，内部请求 Account 服务）。

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `userName` | `string` | 是 | 目标用户名。 |
| `expireIn` | `number` | 是 | 有效秒数，如 `86400` 为 1 天。 |

**返回值**：`string`。生成的 JWT 登录令牌。

```ts
k.api.post(() => {
    const userName = k.request.form.userName
    const expireIn = parseInt(k.request.form.expireIn || "86400", 10)
    const token = k.account.generateToken(userName, expireIn)
    return { tokenLength: token?.length }
})
```

## getAccessToken()

读取当前请求上下文中的 JWT 字符串（若已登录）。

**参数**：无。

**返回值**：`string | null`。当前请求的 JWT；未登录或没有令牌时为空值。

```ts
k.api.get(() => {
    const token = k.account.getAccessToken()
    return {
        hasToken: !!token,
        prefix: token ? token.substring(0, 20) + "..." : null
    }
})
```

## setIdentity()

将传入的 JWT 设为当前请求的 `Authorization: Bearer …`，用于在后续逻辑中以该身份访问依赖登录的 API。

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `token` | `string` | 是 | JWT 登录令牌。 |

**返回值**：`void`。

```ts
k.api.post(() => {
    const token = k.request.form.token
    k.account.setIdentity(token)
    return { isLogin: k.account.isLogin }
})
```

## thirdPartyLogin()

根据第三方名称与用户名，从 Account 服务获取第三方登录信息（用于与外部 IdP 对接的流程）。

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `thirdPartyName` | `string` | 是 | 第三方标识。 |
| `userName` | `string` | 是 | 关联用户名。 |

**返回值**：`ThirdPartyLoginInfo`。第三方登录信息。

```ts
k.api.post(() => {
    const info = k.account.thirdPartyLogin(
        k.request.form.thirdPartyName,
        k.request.form.userName
    )
    return { verified: !!info }
})
```

## renewInfo()

获取当前账户/站点续费相关信息（调用 Account 服务，需已登录且具备权限）。

**参数**：无。

**返回值**：`RenewInfo`。当前账户或站点续费信息。

```ts
k.api.get(() => {
    if (!k.account.isLogin) {
        return { error: "not logged in" }
    }
    return k.account.renewInfo()
})
```

## 相关文档

- [k.account 概述](./index.md)
- [k.account.user](./user.md)
- [k.account.oAuth](./oauth.md)
