# k.account

> Kooboo platform account: login status, current user, organization and third-party login

## Overview

`k.account` is used in site code to handle the login, identity and organization information of **Kooboo platform users** (console accounts). It is different from the following APIs:

| API | Purpose |
|-----|------|
| **`k.account`** | Platform user login, `k.account.user` / `organization`, OAuth login URL |
| **`k.payment`** | Site business payment collection (orders, Alipay/Stripe, etc. **Site payment configuration**) |
| **`k.commerce.membership`** | E-commerce membership level (store customer), not `k.account.membership` |
| **`k.cookie` / `k.session`** | The currently requested cookie and temporary session data are not the platform account system. |


## Submodules

| Member | Doc | Description |
|------|------|------|
| (root) | [login.md](./login.md) | `isLogin`, `login`, `logout`, tokens, etc. |
| `user` | [user.md](./user.md) | Current user and username/email query |
| `organization` | [organization.md](./organization.md) | The organization to which the current site belongs, `current.users`, etc. |
| `oAuth` | [oauth.md](./oauth.md) | Third-party login such as WeChat/Google |

## Quick Example

```ts
k.api.get(() => {
    return { isLogin: k.account.isLogin }
})
```

```ts
k.api.post(() => {
    const username = k.request.form.username
    const password = k.request.form.password
    k.account.login(username, password)
    return { isLogin: k.account.isLogin }
})
```

## Related Docs

- [Login](./login.md)
- [k.payment](../payment/) — site payment integrations
- [k.cookie](../cookie/) — request cookies
