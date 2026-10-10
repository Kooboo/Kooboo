# 认证与授权

> 站点内用户登录、会话与访问控制相关 API

## 概述

Kooboo 中与「当前用户是否已登录」相关的操作主要通过 `k.account` 完成；请求级会话见 [k.session](../session/)，Cookie 见 [k.cookie](../cookie/)。加密与令牌见 [k.security](../security/)。

## k.account.isLogin

判断当前请求是否已登录。

**参数**：无。

**返回值**：`boolean`。当前请求已登录时为 `true`。

```ts
k.api.get("isLogin", () => {
    return { isLogin: k.account.isLogin }
})
```

## k.account.login()

使用用户名和密码登录。

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `username` | `string` | 是 | 用户名。 |
| `password` | `string` | 是 | 密码。 |
| `code` | `string` | 否 | 验证码等。 |

**返回值**：`User | null`。登录成功时返回用户对象；失败时返回 `null`。

```ts
k.api.post("login", () => {
    const username = k.request.form.username
    const password = k.request.form.password
    k.account.login(username, password)
    return { isLogin: k.account.isLogin }
})
```

## k.account.logout()

退出登录，可选跳转到指定 URL。

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `returnUrl` | `string` | 否 | 登出后的跳转地址。 |

**返回值**：`void`。

```ts
k.api.get("logout", () => {
    k.account.logout("/")
    return { isLogin: k.account.isLogin }
})
```

## k.account.ensureLogin()

未登录时重定向到登录页（常用于需要登录的页面或 API）。

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `redirectUrl` | `string` | 是 | 未登录时跳转到的 URL。 |

**返回值**：`void`。

```ts
k.api.get("profile", () => {
    k.account.ensureLogin("/login?return=" + encodeURIComponent(k.request.url))
    return k.account.user.current
})
```

## 相关文档

- [k.session](../session/) — 服务端会话存储
- [k.security](../security/) — 加密、哈希等
- [快速开始](./index.md)
