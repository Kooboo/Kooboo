# k.account

> Kooboo 平台账户：登录态、当前用户、组织与第三方登录

## 概述

`k.account` 用于站点代码里处理 **Kooboo 平台用户**（控制台账号）的登录、身份与组织信息。它与下列 API 不同：

| API | 用途 |
|-----|------|
| **`k.account`** | 平台用户登录、`k.account.user` / `organization`、OAuth 登录 URL |
| **`k.payment`** | 站点业务收款（订单、支付宝/Stripe 等 **站点支付配置**） |
| **`k.commerce.membership`** | 电商会员等级（店铺顾客），不是 `k.account.membership` |
| **`k.cookie` / `k.session`** | 当前请求的 Cookie 与临时会话数据，不是平台账号体系 |


## 子模块

| 成员 | 文档 | 说明 |
|------|------|------|
| （根） | [login.md](./login.md) | `isLogin`、`login`、`logout`、令牌等 |
| `user` | [user.md](./user.md) | 当前用户与用户名/邮箱查询 |
| `organization` | [organization.md](./organization.md) | 当前站点所属组织、`current.users` 等 |
| `oAuth` | [oauth.md](./oauth.md) | 微信/Google 等第三方登录 |

## 快速示例

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

## 相关文档

- [登录](./login.md)
- [k.payment](../payment/) — 站点支付
- [k.cookie](../cookie/) — 请求 Cookie
