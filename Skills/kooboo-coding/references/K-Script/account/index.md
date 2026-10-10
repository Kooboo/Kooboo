# k.account

> Account-related operations, including login and account information

## isLogin

Check whether the current user is logged in.

```ts
k.api.get("isLogin", () => {
    return k.account.isLogin
})
```

**Returns:** `boolean`

## login()

Log in with an account and password.

```ts
k.api.get("login", () => {
    const account = k.account.login("username", "password")
    return { isLogin: k.account.isLogin, userName: account?.UserName ?? null }
})
```

**Parameters:**

| Parameter | Type | Required | Description |
|------|------|------|------|
| username | `string` | Yes | Username |
| password | `string` | Yes | Password |
| code | `string` | No | Optional verification code |

**Returns:** The underlying `Kooboo.Data.Models.User` on success, or `null` when validation fails. Do not return this object directly from a public API: project code should expose an allowlisted projection rather than raw account data.

For current-user data, prefer `k.account.user.current`, whose public fields are `userName`, `firstName`, `lastName`, `email`, `phone`, `language`, `currentOrgId`, `isAdmin`, and `id`. Never expose password, password-hash, OTP secret, or token fields from the underlying account model.

## logout()

Delete the current login cookie and optionally redirect to the specified page. When site SSO is enabled, the redirect goes through the SSO logout endpoint.

```ts
k.api.get("logout", () => {
    return k.account.logout("/K-account/isLogin")
})
```

**Parameter:** `redirectUrl: string` - URL to redirect to after logout

**Returns:** `void`. The response may be redirected or ended; do not expect a later response body from the same request.

## oAuth

To integrate third-party login, configure it first under **System → Configuration → OAuth2**.

```ts
k.api.get("oAuth", () => {
    return k.account.oAuth.google.getAuthUrl()
})
```

**Supported third parties:** `google` / `apple` / `facebook` / `wechat` / `weibo` / ...

**Returns:** The third-party login URL (`string`)

## organization

Get the current organization model for the site.

```ts
k.api.get("organization", () => {
    return k.account.organization
})
```

**Returns:** `OrganizationModel | null`

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

Get the users in the current organization model.

```ts
k.api.get("organizationCurrentUsers", () => {
    return k.account.organization.current.users
})
```

**Returns:** `UserModel[]`

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

## Login example

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

## To be completed

The following methods are not fully documented yet and are pending completion:

| Method | Status |
|------|------|
| `k.account.creditCard` | Pending |
| `k.account.generateToken()` | Pending |
| `k.account.loginOrganization()` | Pending |
| `k.account.membership` | Pending |
| `k.account.server` | Pending |
| `k.account.setIdentity()` | Pending |

## Related documents

- [k.cookie](../core/cookie.md) - Cookie management
- [k.security](../core/security.md) - Security tools (JWT and more)
- [k.request](../core/request.md) - HTTP request information
- [Commerce Customer](../commerce/customer.md) - Commerce customer login
