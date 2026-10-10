# k.account — Login and Identity

> `k.account` Login status, login/logout and token-related APIs on the root object

## TypeScript Definition

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

Whether the current HTTP request carries a valid platform user identity (`context.User` is not empty).

```ts
k.api.get(() => {
    return { isLogin: k.account.isLogin }
})
```

## login()

Log in with username and password, and on success writes a JWT cookie and sets the current requesting user.

| Parameter | Type | Required | Description |
|------|------|------|------|
| username | `string` | yes | Platform username |
| password | `string` | yes | password |
| code | `string` | no | Verification code, etc. (if required by account policy) |

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

On failure, `null` is returned, `isLogin` remains `false`.

## loginOrganization()

Use the **Organization User** form to log in: internally process the user name as `{username}@{current site Organization Id}` and then call `login`.

Applicable to scenarios where the site is bound to an organization and uses organization sub-accounts.

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `username` | `string` | Yes | Organization sub-account username, without the organization suffix. |
| `password` | `string` | Yes | Password. |

**Returns**: `User | null`. User object on success; `null` on failure.

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

Clear login cookies. If SSO is enabled on the site, it will jump to the SSO logout address; otherwise, it will redirect to this URL when `returnUrl` is provided.

| Parameter | Type | Required | Description |
|------|------|------|------|
| returnUrl | `string` | no | Jump path after logout (relative site) |

```ts
k.api.get(() => {
    k.account.logout("/")
    return { isLogin: k.account.isLogin }
})
```

**Returns**: `void`.

## ensureLogin()

If not logged in, **302 redirect** to `redirectUrl` and end the response; if logged in, no operation. Commonly used for pages or APIs that must be logged in to access.

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `redirectUrl` | `string` | Yes | URL to redirect to when the user is not logged in. |

**Returns**: `void`.

```ts
k.api.get(() => {
    k.account.ensureLogin("/login?return=" + encodeURIComponent(k.request.url))
    return k.account.user.current
})
```

## generateToken()

Generate a JWT login token for the specified platform user (usually required to have corresponding administrative rights and request the Account service internally).

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `userName` | `string` | Yes | Target username. |
| `expireIn` | `number` | Yes | Valid seconds, for example `86400` is 1 day. |

**Returns**: `string`. Generated JWT login token.

```ts
k.api.post(() => {
    const userName = k.request.form.userName
    const expireIn = parseInt(k.request.form.expireIn || "86400", 10)
    const token = k.account.generateToken(userName, expireIn)
    return { tokenLength: token?.length }
})
```

## getAccessToken()

Read the JWT string in the current request context (if logged in).

**Parameters**: none.

**Returns**: `string | null`. JWT for the current request, or an empty value when not logged in or no token is available.

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

Set the incoming JWT to the `Authorization: Bearer …` of the current request, which is used in subsequent logic to access login-dependent APIs as that identity.

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `token` | `string` | Yes | JWT login token. |

**Returns**: `void`.

```ts
k.api.post(() => {
    const token = k.request.form.token
    k.account.setIdentity(token)
    return { isLogin: k.account.isLogin }
})
```

## thirdPartyLogin()

Obtain the third-party login information from the Account service based on the third-party name and user name (the process used to interface with the external IdP).

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `thirdPartyName` | `string` | Yes | Third-party identifier. |
| `userName` | `string` | Yes | Associated username. |

**Returns**: `ThirdPartyLoginInfo`. Third-party login information.

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

Obtain current account/site renewal related information (to call the Account service, you need to be logged in and have permission).

**Parameters**: none.

**Returns**: `RenewInfo`. Current account or site renewal information.

```ts
k.api.get(() => {
    if (!k.account.isLogin) {
        return { error: "not logged in" }
    }
    return k.account.renewInfo()
})
```

## Related Docs

- [k.account Overview](./index.md)
- [k.account.user](./user.md)
- [k.account.oAuth](./oauth.md)
