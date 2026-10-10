# 站点认证方式指南

> Scenario 视角的认证总览：用于帮助判断站点应采用哪种登录体系，以及各方案如何组合落地。

> Kooboo 站点支持多种认证方式，本文档覆盖从标准登录到自定义认证的完整方案。
>
> **认证体系选择**：
> - **Kooboo 账号 + 扩展信息**（第三章）【推荐】：用户使用 Kooboo 账号登录，站点存储头像、昵称、会员等级等扩展数据
> - **自定义账号体系**（第二章）：站点完全独立管理用户账号密码，与 Kooboo 账号体系隔离
>
> 各方式可组合使用：标准登录/SSO/OAuth2 用于建立用户会话，站点认证规则用于保护特定页面/API。

---

## 一、Kooboo 账号登录

Kooboo 系统内置的用户账号体系，用户数据存储在 Kooboo 系统数据库中。

### 1.1 密码登录

**默认内置登录页**：`/_Admin/login` 包含：

- 用户名/密码登录
- 第三方登录(Google、GitHub、微信)
- 注册、忘记密码、手机验证码登录

（如不需要自定义登录 UI，可以直接使用 Kooboo 内置的登录页面）

**URL 参数**：

| 参数 | 说明 |
|------|------|
| `returnurl` | 登录成功后跳回的目标地址（必传，否则登录后会跳转到系统后台而非原页面） |
| `permission` | 站点类型，来自 `WebSite.SiteType`：`p`=公开站点，`o`=组织内用户，`m`=站点会员，`u`=需登录用户 |

**入口**：`POST /_api/v2/user/login`

**核心逻辑**：登录成功后从响应头提取 `access_token`，设置为 `jwt_token` cookie（`path=/`，30 天过期），然后跳转到 `returnurl`。

::: tip 注意
Kooboo 内置登录的 `jwt_token` 由系统自动签发和验证，**无需手动配置 JWT Secret**，开箱即用。
`k.security.jwt.encode/decode`（需后台配置 JwtSetting）仅用于自定义账号体系的 JWT 场景，与内置登录无关。
:::

**完整实现**见 [custom-login-ui.md](../Frontend/custom-login-ui.md)，包含：

- Token 机制与 `jwt_token` cookie
- 密码登录完整 HTML/JS 示例
- 注册、忘记密码、手机验证码登录

### 1.2 Kooboo OAuth2 第三方登录

支持 Google、GitHub、微信等第三方登录。

| 名称 | provider | 说明 |
|------|----------|------|
| Google | `google` | 默认支持 |
| GitHub | `github` | 默认支持 |
| 微信 | `wechat` | 默认支持 |
| | `wechat-mp` | 微信浏览器内使用此 provider |

**完整实现**见 [custom-login-ui.md](../Frontend/custom-login-ui.md)，包含：

- 第三方登录（OAuth）完整 HTML/JS 示例

OAuth 用户会创建为 Kooboo 账号，可通过 `/_api/v2/user/GetUser` 获取信息。

### 1.3 获取 Kooboo 账号信息

```js
// 前端：需携带 jwt_token cookie
fetch('/_api/v2/user/GetUser')
```

返回结构：

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

`K-Script`中获取当前登录用户信息

```typescript
k.account.isLogin // 是否登录
k.account.user.current // 获取当前登录用户信息
```

更多语法参考 [K-Script/account/index.md](../K-Script/account/index.md)

::: tip Commerce 对接
如果站点后续会进入 commerce 的 `product/category/cart/order` 顾客链路，不要把 Kooboo 账号登录视为同一条 commerce 登录。commerce 侧应单独使用 `k.commerce.customer.login(...)`，完整流程见 [Commerce 顾客登录与下单流程](references/Scenario/commerce-customer-login-flow.md)。
:::

### 1.4 SSO 单点登录

当站点启用 SSO 后，未登录用户会被自动跳转到统一认证服务器登录，本质上也是 Kooboo 账号登录。

**工作原理**：

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

**实际 URL 示例**：
```
https://sso.kooboo.com/_Admin/login?permission=o&returnurl=%2f_app&sso=1&server=https://siteName.kooboo.com
```

**配置方式**：

1. 在系统后台启用站点的 SSO 登录
2. 用户访问受保护的站点页面时自动触发 SSO 流程


---

## 二、自定义账号登录

站点使用自己的数据库表存储完整用户信息（用户名、密码、业务字段），完全脱离 Kooboo 账号体系。

> ⚠️ **注意**：如果只是需要扩展用户信息（头像、昵称、会员等级等），推荐使用**第三章：Kooboo 账号 + 扩展用户信息**，无需维护独立账号密码体系。
> 本章节适用于需要完全独立账号系统的场景（如：站点间账号不互通、特殊密码策略、与第三方系统对接等）。

### 2.1 扩展功能文档

| 功能 | 详细文档 |
|------|----------|
| 邮件验证码发送 | [send-email-and-sms.md - 邮箱验证码](send-email-and-sms.md#L169-L211) |
| 短信验证码发送 | [send-email-and-sms.md - 短信验证码](send-email-and-sms.md#L146-L168) |
| 验证码校验 | [send-email-and-sms.md - verifyCode](send-email-and-sms.md#L213-L232) |
| 手机号 + 验证码登录 | 待补充（参考上方 `sendEmailCode` / `sendSmsCode` + 自定义登录 API 组合实现） |
| 邮箱 + 验证码登录 | 待补充（同上） |
| 注册验证码 | 待补充 |
| 找回密码验证码 | 待补充 |

### 2.2 架构说明

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

### 2.2 自定义用户表设计

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

### 2.3 自定义 JWT Token

使用 `k.security` 工具签发 JWT：

```typescript
const token = k.security.jwt.encode({ userId: user.Id, userName: user.UserName }, secretKey)
k.cookie.set("site_token", token, { maxAge: 30 * 24 * 60 * 60, path: "/" })
```

### 2.4 完整登录 API 示例

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

### 2.5 Commerce 下单对接

自定义账号体系可以用于站点自己的会员/权限模型，但不应再作为 commerce 顾客链路的登录入口。

如果后续要使用 `k.commerce.product`、`k.commerce.category`、`k.commerce.cart`、`k.commerce.order` 形成完整下单流程，应改为以 `k.commerce.customer` 作为顾客体系，并通过 `k.commerce.customer.login(...)` 建立登录态。

完整流程见 [Commerce 顾客登录与下单流程](references/Scenario/commerce-customer-login-flow.md)。

### 2.6 自定义认证规则

后台 → Development → Authentication → 新增认证规则：

```
Matcher: Any
Action: CustomCode
FailedAction: Redirect
URL: /login
```

**KScript 认证代码示例**：

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

## 三、Kooboo 账号 + 站点扩展用户信息

> ✅ **推荐方案**：大多数站点应采用此方式。用户通过 Kooboo 账号登录，站点在自定义表中存储额外的业务数据（昵称、头像、会员等级等），无需维护独立账号密码体系。

### 3.1 架构说明

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

### 3.2 扩展用户表设计

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

### 3.3 获取完整用户信息

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

### 3.4 更新用户扩展信息

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

### 3.5 典型使用场景

| 场景 | 说明 |
|------|------|
| 会员站点 | 昵称、头像、会员等级、积分 |
| 电商站点 | 收货地址、偏好设置、浏览历史 |
| 社区站点 | 签名、关注数、帖子数 |
| 企业站点 | 部门、职位、工号 |

::: tip Commerce 对接
这一方案非常适合电商站点的“站点用户体系”和“业务扩展信息”管理；但如果站点侧业务流程要继续进入 commerce 的 `cart` / `order`，登录仍应回到 `k.commerce.customer.login(...)`，而不是把扩展用户资料直接当成 commerce 顾客登录。完整流程见 [Commerce 顾客登录与下单流程](references/Scenario/commerce-customer-login-flow.md)。
:::

---

## 四、认证方式对比

### 账号体系对比

| 特性 | Kooboo 账号体系 | 自定义账号体系 |
|------|----------------|---------------|
| 用户存储 | Kooboo 系统数据库 | 站点数据库自定义表 |
| 用户字段 | 固定的 userName/email/tel 等 | 完全自定义 |
| 密码验证 | 系统内置 | 需要自行实现 |
| JWT Token | jwt_token cookie（系统内置） | 自定义 JWT（k.security.jwt） |
| 第三方登录 | 系统 OAuth2（Google/GitHub/微信） | 需要自行实现 OAuth 流程 |
| 适用场景 | 通用、跨站点统一认证 | 站点特定业务逻辑、复杂会员体系 |

### 认证方式对比

| 方式 | 适用场景 | 用户交互 | 配置复杂度 | 是否需要后端开发 |
|------|---------|---------|-----------|----------------|
| Kooboo 账号登录 | 仅使用 Kooboo 账号，无需扩展数据 | 弹出登录页 | 低 | 否 |
| Kooboo 账号 + 扩展信息【推荐】 | 会员站点：头像、昵称、等级等 | 无 | 低 | 是（KScript） |
| 自定义账号登录 | 站点独立账号体系 | 自定义登录页 | 中 | 是（KScript API） |
| Kooboo OAuth2 | 社交登录（Google/GitHub/微信） | 跳转第三方授权 | 低 | 否（后台配置） |
| SSO 单点登录 | 多站点统一认证 | 跳转统一登录页 | 中 | 否（配置 SSO 服务器） |
| 站点认证规则 | 页面/API 保护 | 自动拦截 | 低 | 可选 KScript |
| HTTP Basic Auth | API 调用 | 无（header 携带） | 低 | 否 |

---

## 五、组合使用建议

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
