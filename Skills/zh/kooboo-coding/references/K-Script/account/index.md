# k.account

> Account 包含登录及账户信息的一系列相关操作

## isLogin

判断当前是否已登录。

```ts
k.api.get("isLogin", () => {
    return k.account.isLogin
})
```

**返回：** `boolean`

## login()

账号密码登录。

```ts
k.api.get("login", () => {
    const account = k.account.login("username", "password")
    return { isLogin: k.account.isLogin, userName: account?.UserName ?? null }
})
```

**参数：**

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| username | `string` | 是 | 用户名 |
| password | `string` | 是 | 密码 |
| code | `string` | 否 | 可选验证码 |

**返回：** 成功时返回底层 `Kooboo.Data.Models.User`，校验失败时返回 `null`。不要在公开 API 中直接返回该对象；应显式选择允许暴露的字段，而不是返回原始账户数据。

读取当前用户资料时优先使用 `k.account.user.current`，其公开字段为 `userName`、`firstName`、`lastName`、`email`、`phone`、`language`、`currentOrgId`、`isAdmin` 和 `id`。绝不要从底层账户模型暴露密码、密码哈希、OTP secret 或 token 字段。

## logout()

删除当前登录 Cookie，并可跳转到指定页面。站点启用 SSO 时，会经由 SSO 退出端点跳转。

```ts
k.api.get("logout", () => {
    return k.account.logout("/K-account/isLogin")
})
```

**参数：** `redirectUrl: string` - 退出后跳转地址

**返回：** `void`。响应可能已经重定向或结束，不要期待同一请求后续仍能写入响应正文。

## oAuth

接入第三方登录，需要先到 **系统 → 配置 → OAuth2** 中进行相关配置。

```ts
k.api.get("oAuth", () => {
    return k.account.oAuth.google.getAuthUrl()
})
```

**支持的第三方：** `google` / `apple` / `facebook` / `wechat` / `weibo` / ...

**返回：** 第三方登录的 URL（`string`）

## organization

获取当前站点的组织模型。

```ts
k.api.get("organization", () => {
    return k.account.organization
})
```

**返回：** `OrganizationModel | null`

```ts
interface OrganizationModel {
    id: string;
    name: string;
    displayName: string;
    serverLevel: number;
    currency: string;
    isPartner: boolean;
    departments: DepartmentModel[];
    users: UserModel[];
    organizationUsers: UserModel[];
}
```

## organization.current.users

获取当前组织模型中的用户。

```ts
k.api.get("organizationCurrentUsers", () => {
    return k.account.organization.current.users
})
```

**返回：** `UserModel[]`

```ts
interface UserModel {
    departments: DepartmentModel[];
    userName: string;
    firstName: string;
    lastName: string;
    email: string;
    phone: string;
    language: string;
    currentOrgId: string;
    isAdmin: boolean;
    id: string;
}
```

## 登录示例

```ts
k.api.post("login", () => {
    try {
        const body = JSON.parse(k.request.body)
        const { username, password } = body

        if (!username || !password) {
            return k.api.badRequest()
        }

        const account = k.account.login(username, password)
        if (!k.account.isLogin) {
            return { error: "用户名或密码错误" }
        }

        return { success: true, user: account.UserName }
    } catch (e) {
        return { error: e.message }
    }
})

k.api.get("checkLogin", () => {
    if (!k.account.isLogin) {
        return k.api.unauthorized()
    }
    return { message: "已登录" }
})

k.api.get("logout", () => {
    k.account.logout("/")
    return { message: "已退出" }
})
```

## 待补充

以下方法尚未完整文档化，等待补充：

| 方法 | 状态 |
|------|------|
| `k.account.creditCard` | 待补充 |
| `k.account.generateToken()` | 待补充 |
| `k.account.loginOrganization()` | 待补充 |
| `k.account.membership` | 待补充 |
| `k.account.server` | 待补充 |
| `k.account.setIdentity()` | 待补充 |

## 相关文档

- [k.cookie](../core/cookie.md) - Cookie 管理
- [k.security](../core/security.md) - 安全工具（JWT 等）
- [k.request](../core/request.md) - HTTP 请求信息
- [Commerce Customer](../commerce/customer.md) - 电商顾客登录
