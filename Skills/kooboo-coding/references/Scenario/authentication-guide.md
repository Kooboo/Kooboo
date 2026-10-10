# Site Authentication Guide

> An authentication overview from the Scenario perspective: use it to decide which login system a site should adopt and how the options should be combined.

> Kooboo Sites support multiple authentication methods. This document covers the complete range from standard login to custom authentication.
>
> **Choosing an authentication system**:
> - **Kooboo account + extended information** (Chapter 3) [recommended]: users sign in with a Kooboo account, while the site stores extended data such as avatars, display names, and membership levels
> - **Custom account system** (Chapter 2): the site manages usernames and passwords independently and is isolated from the Kooboo account system
>
> The methods can be combined: standard login/SSO/OAuth2 establishes the user session, while site authentication rules protect specific pages/APIs.

---

## 1. Kooboo Account Login

The user-account system built into Kooboo stores user data in the Kooboo system database.

### 1.1 Password Login

**Default built-in login page**: `/_Admin/login` includes:

- Username/password login
- Third-party login (Google, GitHub, WeChat)
- Registration, password recovery, and phone verification-code login

(When a custom login UI is not needed, use Kooboo's built-in login page directly.)

**URL parameters**:

| Parameter | Description |
|------|------|
| `returnurl` | Target address to return to after successful login (required; otherwise login returns to the system back office instead of the original page) |
| `permission` | Site type from `WebSite.SiteType`: `p`=public site, `o`=organization users, `m`=site members, `u`=users who must log in |

**Entry point**: `POST /_api/v2/user/login`

**Core logic**: after successful login, extract `access_token` from the response headers, set it as the `jwt_token` cookie (`path=/`, expires in 30 days), and then redirect to `returnurl`.

::: tip Note
Kooboo's built-in `jwt_token` is issued and verified automatically by the system, so **manual JWT Secret configuration is not required**.
`k.security.jwt.encode/decode` (which requires a back-office JwtSetting) is only for the JWT scenario in a custom account system and is unrelated to built-in login.
:::

**Complete implementation**: see [custom-login-ui.md](../Frontend/custom-login-ui.md), including:

- The Token mechanism and `jwt_token` cookie
- A complete HTML/JS password-login example
- Registration, password recovery, and phone verification-code login

### 1.2 Kooboo OAuth2 Third-Party Login

Google, GitHub, WeChat, and other third-party logins are supported.

| Name | provider | Description |
|------|----------|------|
| Google | `google` | Supported by default |
| GitHub | `github` | Supported by default |
| WeChat | `wechat` | Supported by default |
| | `wechat-mp` | Use this provider inside a WeChat browser |

**Complete implementation**: see [custom-login-ui.md](../Frontend/custom-login-ui.md), including:

- A complete HTML/JS third-party login (OAuth) example

OAuth users are created as Kooboo accounts and can be read through `/_api/v2/user/GetUser`.

### 1.3 Getting Kooboo Account Information

```js
// 前端：需携带 jwt_token cookie
fetch('/_api/v2/user/GetUser')
```

Response structure:

```json
{
  "id": "xxx",
  "userName": "john",
  "emailAddress": "john@example.com",
  "fullName": "John Doe",
  "isAdmin": false,
  "tel": "+86xxxx",
  "isEmailVerified": true,
  "isTelVerified": false
}
```

Get the current signed-in user in `K-Script`:

```typescript
k.account.isLogin // 是否登录
k.account.user.current // 获取当前登录用户信息
```

For more syntax, see [K-Script/account/index.md](../K-Script/account/index.md).

::: tip Commerce integration
If the site will later enter the Commerce `product/category/cart/order` customer flow, do not treat Kooboo account login as the same Commerce login path. The Commerce side should use `k.commerce.customer.login(...)` separately; see [Commerce Customer Login and Ordering Flow](commerce-customer-login-flow.md) for the complete flow.
:::

### 1.4 SSO Single Sign-On

When SSO is enabled for a site, an unauthenticated user is automatically redirected to the central authentication server, which is essentially another form of Kooboo account login.

**How it works**:

```
用户访问站点页面（未登录）
  │
  ▼
KoobooMiddleware 检测到未登录
  │ 站点配置了 SsoLogin=true
  ▼
302 跳转到 SSO 认证服务器:
  {SsoLogin}/_Admin/login?permission={type}&returnurl={path}&sso=1&server={site}
  │
  ▼
用户在 SSO 服务器完成登录
  │ SSO 服务器设置 jwt_token cookie
  ▼
302 返回站点:
  {site}/_api/v2/user/ssoLogin?accessToken={token}&returnurl={path}
  │ SSO 服务器调用此端点设置 jwt_token cookie
  ▼
302 跳转到 returnurl
  │ 站点中间件验证 jwt_token cookie 通过
  ▼
用户已登录，正常访问页面
```

**Actual URL example**:
```
https://sso.kooboo.com/_Admin/login?permission=o&returnurl=%2f_app&sso=1&server=https://siteName.kooboo.com
```

**Configuration**:

1. Enable the site's SSO login in the system back office.
2. SSO is triggered automatically when a user visits a protected site page.


---

## 2. Custom Account Login

The site stores complete user information (username, password, and business fields) in its own database tables and is completely independent of the Kooboo account system.

> ⚠️ **Note**: if you only need extended user information (avatar, display name, membership level, and so on), use **Chapter 3: Kooboo account + extended user information** instead of maintaining an independent username/password system.
> This chapter applies to scenarios that need a completely independent account system, such as accounts that must not be shared across sites, special password policies, or integration with a third-party system.

### 2.1 Extended-Function Documentation

| Function | Detailed documentation |
|------|----------|
| Email verification-code delivery | [send-email-and-sms.md - Email verification code](./send-email-and-sms.md#L169-L211) |
| SMS verification-code delivery | [send-email-and-sms.md - SMS verification code](./send-email-and-sms.md#L146-L168) |
| Verification-code validation | [send-email-and-sms.md - verifyCode](./send-email-and-sms.md#L213-L232) |
| Phone number + verification-code login | To be added (combine the `sendEmailCode` / `sendSmsCode` references above with a custom login API) |
| Email + verification-code login | To be added (same as above) |
| Registration verification code | To be added |
| Password-recovery verification code | To be added |

### 2.2 Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                    自定义账号体系                            │
├─────────────────────────────────────────────────────────────┤
│  前端登录 → 自定义 API → 验证密码 → 查询自定义用户表          │
│                    ↓                                        │
│           生成 JWT Token → 设置自定义 Cookie/Header          │
│                    ↓                                        │
│           自定义认证规则 → 验证自定义 JWT                     │
└─────────────────────────────────────────────────────────────┘
```

### 2.2 Custom User-Table Design

```sql
CREATE TABLE SiteUsers (
    Id UUID PRIMARY KEY,
    UserName VARCHAR(100) NOT NULL,
    Password VARCHAR(255) NOT NULL,
    Email VARCHAR(255),
    Phone VARCHAR(50),
    NickName VARCHAR(100),
    Avatar VARCHAR(500),
    CreatedAt DATETIME,
    UpdatedAt DATETIME
)
```

### 2.3 Custom JWT Token

Use the `k.security` utility to issue a JWT:

```typescript
const token = k.security.jwt.encode({ userId: user.Id, userName: user.UserName }, secretKey)
k.cookie.set("site_token", token, { maxAge: 30 * 24 * 60 * 60, path: "/" })
```

### 2.4 Complete Login API Example

```typescript
k.api.post("site-login", () => {
    const body = JSON.parse(k.request.body)
    const { username, password } = body

    const user = k.DB.sqlite.SiteUsers.find({ UserName: username })
    if (!user) {
        return k.api.badRequest()
    }

    if (password !== user.Password) {
        return k.api.badRequest()
    }

    const token = k.security.jwt.encode({
        userId: user.Id,
        userName: user.UserName
    }, "your-secret-key")

    k.cookie.set("site_token", token, {
        maxAge: 30 * 24 * 60 * 60,
        path: "/"
    })

    return { success: true, token: token }
})
```

### 2.5 Commerce Ordering Integration

A custom account system can serve the site's own membership/permission model, but it must not become the login entry point for the Commerce customer flow.

If the site later uses `k.commerce.product`, `k.commerce.category`, `k.commerce.cart`, and `k.commerce.order` for a complete ordering flow, use `k.commerce.customer` as the customer system and establish the login state through `k.commerce.customer.login(...)`.

See [Commerce Customer Login and Ordering Flow](commerce-customer-login-flow.md) for the complete flow.

### 2.6 Custom Authentication Rule

Back office → Development → Authentication → Add an authentication rule:

```
Matcher: Any
Action: CustomCode
FailedAction: Redirect
URL: /login
```

**KScript authentication code example**:

```typescript
if (k.request.url.indexOf('/member') == 0 || k.request.url.indexOf('/api') == 0) {
    var token = k.request.cookies.get('site_token')
        || k.request.headers.get('X-Site-Token')

    if (!token) {
        k.response.redirect('/login')
        return
    }

    try {
        var payload = k.security.jwt.decode(token, secretKey)
        k.context.items["currentUserId"] = payload.userId
    } catch (e) {
        k.response.unauthorized()
    }
}
```

---

## 3. Kooboo Account + Site-Extended User Information

> ✅ **Recommended option**: most sites should use this approach. Users sign in with a Kooboo account, while the site stores additional business data (display name, avatar, membership level, and so on) in a custom table without maintaining an independent username/password system.

### 3.1 Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                    Kooboo 账号 + 扩展信息                      │
├─────────────────────────────────────────────────────────────┤
│  用户通过 Kooboo 账号登录（统一认证入口）                       │
│                    ↓                                        │
│  登录成功后自动查询/创建站点扩展用户表                          │
│  表中通过 KoobooUserName 关联 Kooboo 账号                      │
│                    ↓                                        │
│  返回完整用户信息（Kooboo 基础信息 + 站点扩展字段）             │
├─────────────────────────────────────────────────────────────┤
│  用户表设计（只关联 userName，无需密码）                       │
│  SiteUserProfiles: { KoobooUserName, NickName, Avatar... } │
└─────────────────────────────────────────────────────────────┘
```

### 3.2 Extended User-Table Design

```sql
CREATE TABLE SiteUserProfiles (
    Id UUID PRIMARY KEY,
    KoobooUserName VARCHAR(100) NOT NULL,
    NickName VARCHAR(100),
    Avatar VARCHAR(500),
    VIPLevel INT DEFAULT 0,
    Points INT DEFAULT 0,
    ExtraData TEXT,
    CreatedAt DATETIME,
    UpdatedAt DATETIME
)
```

### 3.3 Getting Complete User Information

```typescript
k.api.get("get-profile", () => {
    if (!k.account.isLogin) {
        return k.api.unauthorized()
    }

    const currentOrgId = k.account.organization.current.id
    const { id, userName, email, firstName, lastName, isAdmin } = k.account.user.current

    let fullName = userName
    if (firstName || lastName) {
        if (!lastName) {
            fullName = firstName
        } else if (!firstName) {
            fullName = lastName
        } else {
            const hasChinese = /[\u4e00-\u9fa5]/.test(firstName) || /[\u4e00-\u9fa5]/.test(lastName)
            fullName = hasChinese ? firstName + lastName : firstName + ' ' + lastName
        }
    }

    const koobooInfo = {
        id: String(id),
        userName,
        firstName,
        lastName,
        fullName,
        email,
        isOrganizationAdmin: isAdmin && currentOrgId === k.account.currentOrgId
    }

    const profile = k.DB.sqlite.SiteUserProfiles.find({ KoobooUserName: userName })

    if (!profile) {
        profile = {
            KoobooUserName: userName,
            NickName: fullName,
            Avatar: "",
            CreatedAt: new Date()
        }
        k.DB.sqlite.SiteUserProfiles.add(profile)
    }

    return {
        ...koobooInfo,
        profile: profile
    }
})
```

### 3.4 Updating Extended User Information

```typescript
k.api.post("update-profile", () => {
    if (!k.account.isLogin) {
        return k.api.unauthorized()
    }

    const { userName } = k.account.user.current
    const body = JSON.parse(k.request.body)
    const { nickName, avatar, extraData } = body

    const profile = k.DB.sqlite.SiteUserProfiles.find({ KoobooUserName: userName })
    if (!profile) {
        return k.api.notFound("用户档案不存在")
    }

    if (nickName !== undefined) profile.NickName = nickName
    if (avatar !== undefined) profile.Avatar = avatar
    if (extraData !== undefined) profile.ExtraData = extraData
    profile.UpdatedAt = new Date()

    k.DB.sqlite.SiteUserProfiles.update(profile)

    return { success: true }
})
```

### 3.5 Typical Use Cases

| Scenario | Description |
|------|------|
| Membership site | Display name, avatar, membership level, points |
| Commerce site | Shipping address, preferences, browsing history |
| Community site | Bio, following count, post count |
| Corporate site | Department, title, employee number |

::: tip Commerce integration
This approach is well suited to managing a commerce site's “site user system” and “business extension data”. However, when the site-side business flow enters Commerce `cart` / `order`, login must still return to `k.commerce.customer.login(...)`; extended user profiles must not be treated as Commerce customer login. See [Commerce Customer Login and Ordering Flow](commerce-customer-login-flow.md) for the complete flow.
:::

---

## 4. Authentication Comparison

### Account-System Comparison

| Feature | Kooboo account system | Custom account system |
|------|----------------|---------------|
| User storage | Kooboo system database | Custom tables in the site database |
| User fields | Fixed userName/email/tel and so on | Fully customizable |
| Password validation | Built in to the system | Must be implemented independently |
| JWT Token | jwt_token cookie (built in) | Custom JWT (k.security.jwt) |
| Third-party login | System OAuth2 (Google/GitHub/WeChat) | OAuth flow must be implemented independently |
| Applicable scenarios | General, unified authentication across sites | Site-specific business logic and complex membership systems |

### Authentication-Method Comparison

| Method | Applicable scenario | User interaction | Configuration complexity | Backend development required |
|------|---------|---------|-----------|----------------|
| Kooboo account login | Kooboo account only, with no extended data | Login page appears | Low | No |
| Kooboo account + extended information [recommended] | Membership site: avatar, display name, level, and so on | None | Low | Yes (KScript) |
| Custom account login | Independent site account system | Custom login page | Medium | Yes (KScript API) |
| Kooboo OAuth2 | Social login (Google/GitHub/WeChat) | Redirect to third-party authorization | Low | No (back-office configuration) |
| SSO single sign-on | Unified authentication across multiple sites | Redirect to the central login page | Medium | No (configure the SSO server) |
| Site authentication rule | Page/API protection | Automatic interception | Low | Optional KScript |
| HTTP Basic Auth | API calls | None (credentials in the header) | Low | No |

---

## 5. Combination Recommendations

```
公开站点（无登录）
  不需要任何认证配置

标准会员站点（使用 Kooboo 账号）
  Kooboo 账号登录 + 可选 Kooboo OAuth2 第三方登录
  保护会员页面：站点基础设置 > 访问控制 > 访问权限

会员站点（Kooboo 账号 + 扩展信息）【推荐】
  Kooboo 账号登录
  站点扩展表存储：昵称、头像、会员等级、积分等
  通过 KoobooUserName 关联，无需密码
  获取用户信息：k.account.user.current + k.DB.sqlite.SiteUserProfiles

电商/社区/企业站点
  Kooboo 账号登录 + 站点扩展信息
  收货地址、帖子数、部门职位等业务数据
  扩展表自动创建，按需更新

多站点统一认证
  SSO 单点登录 + Kooboo 账号登录
  各站点共享同一套用户体系
```
