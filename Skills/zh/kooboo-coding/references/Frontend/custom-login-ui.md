# 自定义登录 UI 开发指南

> 站点在同域名下可直接调用 Kooboo 登录 API，自由设计登录/注册/找回密码页面，无需重写后端逻辑。

## 一、登录页入口与 URL 参数

当用户访问需要登录的站点页面时，Kooboo 中间件会将用户 302 跳转到登录页，URL 携带以下参数：

```
/_admin/login?permission=o&returnurl=%2F
```

| 参数 | 来源 | 说明 |
|------|------|------|
| `permission` | `WebSite.SiteType` | 站点类型：`p`=公开站点，`o`=组织内用户，`m`=站点会员，`u`=需登录用户 |
| `returnurl` | 原始请求路径 URL 编码 | 登录成功后跳回的目标地址 |

**自定义登录页必须读取这些 URL 参数，并在所有 API 请求和跳转中透传 `returnurl`**，否则用户登录后会跳转到系统后台而非站点页面。

> 将自定义登录页面发布为路由 `/_admin/login` 即可覆盖默认的 `/Kooboo.Web/_Admin/login` 页面，
> Kooboo 中间件会自动将未登录用户跳转到你的自定义登录页。

读取参数：

```js
var qs = new URLSearchParams(location.search)
var returnurl = qs.get('returnurl')
var permission = qs.get('permission')
```

---

## 二、Token 机制（必读）

Kooboo 使用 **`jwt_token` cookie** 作为登录凭证。

### 登录态建立流程

```
POST /_api/v2/user/login
  │ Body: { UserName, Password, withToken: true, returnurl }
  ▼
后端验证通过
  │ HTTP 200
  │ 响应 Header: access_token=xxx
  ▼
前端从响应头提取 access_token
  │ document.cookie = 'jwt_token=' + token + ';path=/;max-age=2592000'
  │ location.href = returnurl
  ▼
浏览器携带 jwt_token cookie 访问目标页面
  中间件验证 cookie 通过 → 用户已登录
```

### 关键点

1. **登录请求必须带 `withToken: true`**，否则响应头不会包含 `access_token`。
2. **登录请求必须带 `returnurl`**，否则登录后不知道跳回哪里。
3. 登录成功后 **`access_token` 在响应头中**（不在 body 里），提取后手动设置为 `jwt_token` cookie（path=/, 30 天过期），然后跳转到 `returnurl`。

### 检查登录态

```js
function isLogin() {
  return document.cookie.indexOf('jwt_token=') !== -1
}
```

### 登出

```
POST /_api/v2/user/logout
```

删除 `jwt_token` 和 `_kooboo_api_user` cookie。

---

## 三、表单验证规则

> 所有涉及用户输入的页面（登录、注册、绑定等）都应先做前端校验，避免无效请求、提升用户体验。

### 3.1 Kooboo 账号规则

| 字段 | 前端校验规则 | 服务端规则 | 说明 |
|------|-------------|-----------|------|
| 用户名 | 5~30 个字符，仅支持字母、数字、连字符 `-`，必须以字母或数字开头 **且** 以字母或数字结尾 | 仅校验非空，字符限字母数字 `_` `-` | 前端规则更严格，两者都通过才能注册 |
| 密码 | 8~30 个字符，须包含字母、数字、符号中的至少两种 | 无服务端限制 | 服务端不检查密码强度，完全由前端保证 |
| 邮箱 | 标准邮箱格式 | 注册时必填 | — |
| 手机号 | 纯数字，无长度限制（`/^\d*$/`） | 无格式校验 | SMS 发送时需要带区号，如 `+8613800138000` |

> 注：`+` 号仅在 SMS 发送 API 中需要（如 `SendTelCode`），前端登录页输入框可允许用户输入 `+`，但 Kooboo 前端本身的 `phoneRule` 仅校验纯数字。

### 3.2 通用校验函数

以下校验规则提取自 Kooboo 源码：

- 用户名：前端规则 5~30 位，仅 `a-zA-Z0-9-`，首尾须为字母或数字；后端额外允许 `_`
- 密码：8~30 位，须包含字母、数字、符号中的至少两种
- 手机号：前端校验仅要求纯数字 `/^\d*$/`
- 保留前缀：以 `_mx`、`_dmarc`、`_acme-challenge`、`_domainkey` 开头（不区分大小写）

```js
/* 用户名规则：5~30 位，仅字母、数字、连字符，首尾必须是字母或数字 */
function isValidUserName(name) {
  return /^[a-zA-Z0-9][a-zA-Z0-9-]{3,28}[a-zA-Z0-9]$/.test(name) && name.length >= 5 && name.length <= 30
}

/* 密码：8~30 位，需包含字母、数字、符号中的至少两种 */
function isValidPassword(pwd) {
  if (pwd.length < 8 || pwd.length > 30) return false
  var hasLetter = /[a-zA-Z]/.test(pwd)
  var hasDigit = /\d/.test(pwd)
  var hasSymbol = /[^a-zA-Z0-9]/.test(pwd)
  return (hasLetter + hasDigit + hasSymbol) >= 2
}

/* 邮箱 */
function isValidEmail(email) {
  return /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email)
}

/* 手机号（SMS 发送时需含区号） */
function isValidTel(tel) {
  return /^\+\d{7,15}$/.test(tel)
}

/* 保留前缀（`_mx`、`_dmarc`、`_acme-challenge`、`_domainkey`） */
function isReservedPrefix(name) {
  var lower = name.toLowerCase()
  return lower.startsWith('_mx') || lower.startsWith('_dmarc')
    || lower.startsWith('_acme-challenge') || lower.startsWith('_domainkey')
}

/* 显示错误 */
function showError(el, msg) {
  el.innerText = msg
  el.style.display = 'block'
}

function hideError(el) {
  el.style.display = 'none'
}
```

### 3.3 实时校验（检查用户名 / 邮箱是否可用）

注册和绑定页面应在用户输入后实时调用 API 检查可用性，而不是等提交时才报错：

```html
<input name="username" placeholder="用户名" oninput="checkName(this.value)" />
<span id="name-msg" style="font-size:12px"></span>

<input name="email" type="email" placeholder="邮箱" oninput="checkEmail(this.value)" />
<span id="email-msg" style="font-size:12px"></span>

<script>
var nameCheckTimer, emailCheckTimer

async function checkName(name) {
  clearTimeout(nameCheckTimer)
  var msgEl = document.getElementById('name-msg')
  if (!name) { msgEl.innerText = ''; return }
  if (!isValidUserName(name)) { msgEl.innerText = '用户名格式不正确（5~30位字母数字，首尾须为字母或数字）'; msgEl.style.color = 'red'; return }
  if (isReservedPrefix(name)) { msgEl.innerText = '该用户名前缀已被系统保留'; msgEl.style.color = 'red'; return }
  nameCheckTimer = setTimeout(async function() {
    var res = await fetch('/_api/v2/user/IsUniqueName?name=' + encodeURIComponent(name))
    var ok = await res.json()
    msgEl.innerText = ok ? '用户名可用' : '该用户名已存在'
    msgEl.style.color = ok ? 'green' : 'red'
  }, 300)
}

async function checkEmail(email) {
  clearTimeout(emailCheckTimer)
  var msgEl = document.getElementById('email-msg')
  if (!email) { msgEl.innerText = ''; return }
  if (!isValidEmail(email)) { msgEl.innerText = '邮箱格式不正确'; msgEl.style.color = 'red'; return }
  emailCheckTimer = setTimeout(async function() {
    var res = await fetch('/_api/v2/user/IsUniqueEmail?email=' + encodeURIComponent(email))
    var ok = await res.json()
    msgEl.innerText = ok ? '邮箱可用' : '该邮箱已注册'
    msgEl.style.color = ok ? 'green' : 'red'
  }, 300)
}
</script>
```

> 注：`IsUniqueName` 和 `IsUniqueEmail` 是 Kooboo 内置的 V2 API，无需自定义后端逻辑。

### 3.4 在表单提交中整合校验

所有表单提交前都应先做前端校验，避免无效请求：

```js
document.getElementById('register-form').onsubmit = async function(e) {
  e.preventDefault()
  var f = e.target
  var errorEl = document.getElementById('error')
  hideError(errorEl)

  /* 前端校验 */
  if (!isValidUserName(f.username.value)) {
    return showError(errorEl, '用户名格式不正确（5~30位，字母数字连字符，首尾须为字母或数字）')
  }
  if (isReservedPrefix(f.username.value)) {
    return showError(errorEl, '该用户名前缀已被系统保留')
  }
  if (!isValidPassword(f.password.value)) {
    return showError(errorEl, '密码需 8~30 位，包含字母、数字、符号中的至少两种')
  }
  if (!isValidEmail(f.email.value)) {
    return showError(errorEl, '邮箱格式不正确')
  }

  /* 提交请求... */
}
```

---

## 四、密码登录

```
POST /_api/v2/user/login
Content-Type: application/json
```

### 请求参数

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| UserName | string | 是 | 用户名或邮箱 |
| Password | string | 是 | 密码 |
| withToken | boolean | **是** | 必须为 `true`，否则响应头不含 access_token |
| returnurl | string | **强烈建议** | 登录后跳转地址（来自 URL 参数），不传可能跳到系统后台 |
| code | string | 否 | 双因素验证码（需要时重试才带） |

### 成功响应

V2 API 成功时返回 HTTP 200，**`access_token` 放在响应头中**：

```
HTTP 200
access_token: eyJhbGciOi...
Content-Type: application/json
```

响应 body 为跳转 URL（`/_Admin/start?accesstoken=xxx`），但不需要使用它。直接提取响应头的 `access_token`，设置 `jwt_token` cookie 后跳转。

### 失败响应

V2 API 失败时响应体为 `Messages` 数组：

```json
["User name or password not valid"]
```

### 双因素认证

失败时 Messages 数组中包含 `"NeedVerifyCode email|tel|otp"`：

```json
["NeedVerifyCode email"]
```

弹窗让用户输入验证码，带 `code` 参数重试。

### 完整 HTML 示例

```html
<form id="login-form">
  <input name="username" placeholder="用户名或邮箱" required />
  <input name="password" type="password" placeholder="密码" required />
  <button type="submit">登录</button>
</form>
<p id="error" style="color:red;display:none"></p>

<script>
var qs = new URLSearchParams(location.search)
var returnurl = qs.get('returnurl')

function setCookie(token) {
  document.cookie = 'jwt_token=' + token + ';path=/;max-age=' + (30*24*60*60)
}

document.getElementById('login-form').onsubmit = async function(e) {
  e.preventDefault()
  var f = e.target
  var errorEl = document.getElementById('error')
  errorEl.style.display = 'none'

  var res = await fetch('/_api/v2/user/login', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({
      UserName: f.username.value,
      Password: f.password.value,
      withToken: true,
      returnurl: returnurl || ''
    })
  })

  if (res.ok) {
    setCookie(res.headers.get('access_token'))
    location.href = returnurl || '/'
  } else {
    var msgs = await res.json()
    var msg = msgs[0] || '登录失败'
    if (msg.startsWith('NeedVerifyCode')) {
      var code = prompt('请输入验证码')
      if (code) {
        var res2 = await fetch('/_api/v2/user/login', {
          method: 'POST',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify({
            UserName: f.username.value,
            Password: f.password.value,
            withToken: true,
            returnurl: returnurl || '',
            code: code
          })
        })
        if (res2.ok) { setCookie(res2.headers.get('access_token')); location.href = returnurl || '/' }
        else { var msgs2 = await res2.json(); errorEl.innerText = msgs2[0] || '验证失败'; errorEl.style.display = 'block' }
      }
    } else {
      errorEl.innerText = msg
      errorEl.style.display = 'block'
    }
  }
}
</script>
```

---

## 五、注册

```
POST /_api/v2/user/register
Content-Type: application/json
```

### 请求参数

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| UserName | string | 是 | 用户名 |
| Password | string | 是 | 密码 |
| email | string | 是 | 邮箱 |
| code | string | 否 | 验证码（需要时） |
| withToken | boolean | **建议** | 设为 `true` 以获取 access_token |

### 成功响应（直接注册成功）

V2 API 成功时返回 HTTP 200，**`access_token` 放在响应头中**：

```
HTTP 200
access_token: eyJhbGciOi...
Content-Type: application/json
```

提取响应头的 `access_token`，设置 `jwt_token` cookie 后跳转。

### 需要验证码

```
HTTP 202
Content-Type: application/json

""
```

返回 HTTP 202 表示需要验证码，发送验证码后再带 `code` 重新提交。

### 失败响应

V2 API 失败时响应体为 `Messages` 数组：

```json
["Username exists"]
```

常见错误消息：`"Username or password or email not provided"`、`"Invalid User name formate"`、`"Reserved user name prefix"`、`"Username exists"`、`"Verify code Error"`。

### 完整 HTML 示例

```html
<form id="register-form">
  <input name="username" placeholder="用户名" required />
  <input name="email" type="email" placeholder="邮箱" required />
  <input name="password" type="password" placeholder="密码" required />
  <button type="submit">注册</button>
</form>
<p id="error" style="color:red;display:none"></p>

<script>
var qs = new URLSearchParams(location.search)
var returnurl = qs.get('returnurl')

function setCookie(token) {
  document.cookie = 'jwt_token=' + token + ';path=/;max-age=' + (30*24*60*60)
}

document.getElementById('register-form').onsubmit = async function(e) {
  e.preventDefault()
  var f = e.target
  var errorEl = document.getElementById('error')
  errorEl.style.display = 'none'

  var res = await fetch('/_api/v2/user/register', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({
      UserName: f.username.value,
      Password: f.password.value,
      email: f.email.value,
      withToken: true,
      returnurl: returnurl || ''
    })
  })

  if (res.status === 202) {
    var code = prompt('请输入邮箱收到的验证码')
    var res2 = await fetch('/_api/v2/user/register', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({
        UserName: f.username.value,
        Password: f.password.value,
        email: f.email.value,
        withToken: true,
        returnurl: returnurl || '',
        code: code
      })
    })
    if (res2.ok) { setCookie(res2.headers.get('access_token')); location.href = returnurl || '/' }
    else { var msgs2 = await res2.json(); errorEl.innerText = msgs2[0] || '注册失败'; errorEl.style.display = 'block' }
  } else if (res.ok) {
    setCookie(res.headers.get('access_token'))
    location.href = returnurl || '/'
  } else {
    var msgs = await res.json()
    errorEl.innerText = msgs[0] || '注册失败'
    errorEl.style.display = 'block'
  }
}
</script>
```

---

## 六、忘记密码

### 6.1 发送重置邮件

```
POST /_api/v2/user/ForgotPassword
Content-Type: application/json
```

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| Email | string | 是 | 注册邮箱 |

响应为服务端返回的文字消息。邮件中会包含重置密码链接（含 reset token）。

---

### 6.2 通过邮箱验证码重置密码

```
POST /_api/v2/password/ResetByEmail
Content-Type: application/json
```

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| email | string | 是 | 邮箱 |
| code | string | 是 | 邮箱收到的验证码 |
| newPassword | string | 是 | 新密码 |

### 6.3 发送邮箱验证码

```
POST /_api/v2/password/EmailCode
Content-Type: application/json
```

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| email | string | 是 | 邮箱地址 |

---

### 6.4 通过手机验证码重置密码

```
POST /_api/v2/password/ResetByTel
Content-Type: application/json
```

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| Tel | string | 是 | 手机号 |
| code | string | 是 | 短信验证码 |
| newPassword | string | 是 | 新密码 |

### 6.5 发送手机验证码

```
POST /_api/v2/password/SmsCode
Content-Type: application/json
```

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| Tel | string | 是 | 手机号 |

---

### 完整 HTML 示例

```html
<div id="step1">
  <h3>忘记密码</h3>
  <input id="email-input" type="email" placeholder="请输入注册邮箱" />
  <button onclick="sendEmailCode()">发送验证码</button>
</div>

<div id="step2" style="display:none">
  <h3>重置密码</h3>
  <input id="code-input" placeholder="邮箱验证码" />
  <input id="new-pwd" type="password" placeholder="新密码" />
  <button onclick="resetPassword()">重置密码</button>
</div>

<script>
var email = ''

async function sendEmailCode() {
  email = document.getElementById('email-input').value
  await fetch('/_api/v2/password/EmailCode', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ email: email })
  })
  document.getElementById('step1').style.display = 'none'
  document.getElementById('step2').style.display = 'block'
}

async function resetPassword() {
  var code = document.getElementById('code-input').value
  var pwd = document.getElementById('new-pwd').value
  var res = await fetch('/_api/v2/password/ResetByEmail', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ email: email, code: code, newPassword: pwd })
  })
  if (!res.ok) {
    var msgs = await res.json()
    alert(msgs[0] || '重置失败')
    return
  }
  alert('密码已重置')
  location.href = '/'
}
</script>
```

---

## 七、手机验证码登录

### 7.1 发送短信验证码

```
POST /_api/v2/user/SendTelCode
Content-Type: application/json
```

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| Tel | string | 是 | 手机号（含区号，如 +8613800138000） |

---

### 7.2 验证手机验证码登录

```
POST /_api/v2/verifycode/VerifyTelCode
Content-Type: application/json
```

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| Tel | string | 是 | 手机号 |
| code | int | 是 | 验证码 |
| returnurl | string | 否 | 登录后跳转地址 |

V2 API 成功时响应体为 `access_token` 字符串，需手动设置 cookie：

### 完整 HTML 示例

```html
<div id="phone-login">
  <div><input id="phone-input" placeholder="手机号" /></div>
  <div><input id="sms-code" placeholder="验证码" /></div>
  <button onclick="sendSms()">发送验证码</button>
  <button onclick="phoneLogin()">登录</button>
</div>

<script>
var qs = new URLSearchParams(location.search)
var returnurl = qs.get('returnurl')

async function sendSms() {
  var phone = document.getElementById('phone-input').value
  await fetch('/_api/v2/user/SendTelCode', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ Tel: phone })
  })
  alert('验证码已发送')
}

async function phoneLogin() {
  var phone = document.getElementById('phone-input').value
  var code = document.getElementById('sms-code').value
  var res = await fetch('/_api/v2/verifycode/VerifyTelCode', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ Tel: phone, code: parseInt(code), returnurl: returnurl || '' })
  })
  if (!res.ok) {
    var msgs = await res.json()
    alert(msgs[0] || '验证失败')
    return
  }
  var token = await res.json()
  document.cookie = 'jwt_token=' + token + ';path=/;max-age=2592000'
  location.href = returnurl || '/'
}
</script>
```

---

## 八、第三方登录（OAuth）

### 8.1 获取 OAuth 授权 URL

```
POST /_api/v2/oauth/Url/{provider}
Content-Type: application/json
```

| 路径参数 | 说明 |
|---------|------|
| provider | `google`、`github`、`wechat`、`facebook`、`apple`、`weibo` 等 |

| 请求参数 | 类型 | 必填 | 说明 |
|---------|------|------|------|
| redirectUri | string | 是 | OAuth 完成后的落地地址 |

返回：string — 第三方授权页面 URL，直接跳转即可。

### 8.2 OAuth 流程详解

```
1. 前端调用 POST /_api/v2/oauth/Url/google
     Body: { redirectUri: location.href }
     → 返回 Google 授权 URL

2. 浏览器跳转到 Google 授权
     → 用户授权后 Google 回调到 /_api/oauth2callback/GoogleLogin?code=xxx

3. Kooboo 后台处理 OAuth 回调
     → 获取用户信息，设置 jwt_token cookie
     → 302 跳转到 步骤1 传入的 redirectUri（即登录页 URL）
     → 登录页 URL 此时带上 ?oauthid=xxx&requireinfo=true

4. 登录页重新加载，检测到 oauthid 参数
     → 如果 requireinfo=true：跳转绑定/注册页
     → 否则：已登录，跳转到 returnurl
```

### 8.3 `redirectUri` 的正确值

**`redirectUri` 必须是当前登录页的完整 URL（`location.href`）**，因为 OAuth 回调完成后后台会把用户跳回这个地址。这样 `permission`、`returnurl` 等参数会跟随 OAuth 流程完整传递。

❌ 错误：
```js
var redirectUri = location.origin + '/_api/oauth2callback/GoogleLogin'
// 这是 OAuth 回调处理器地址，不是落地页地址！
```

✅ 正确：
```js
var redirectUri = location.href
// 例如：https://example.com/_admin/login?permission=o&returnurl=%2F
```

### 8.4 OAuth 回调处理

OAuth 成功后用户回到登录页，URL 会携带 `oauthid` 或 `accesstoken` 参数：

```js
var qs = new URLSearchParams(location.search)
var oauthId = qs.get('oauthid')
var requireInfo = qs.get('requireinfo')
var accessToken = qs.get('access_token') || qs.get('accesstoken') || qs.get('accessToken')

/* 方案一：携带 accesstoken，直接设置 cookie 后跳转 */
if (accessToken) {
  document.cookie = 'jwt_token=' + accessToken + ';path=/;max-age=' + (30*24*60*60)
  location.href = qs.get('returnurl') || '/'
}

/* 方案二：携带 oauthid，需要进一步处理 */
if (oauthId && requireInfo === 'true') {
  // 新 OAuth 用户，需要绑定已有账号或注册新账号
  location.href = '/_admin/login/bind/name?oauthid=' + oauthId
} else if (oauthId) {
  // 已绑定的 OAuth 用户，直接登录成功，跳到 returnurl
  location.href = qs.get('returnurl') || '/'
}
```

### 8.5 已支持的第三方登录

| 名称 | provider | 备注 |
|------|----------|------|
| Google | `google` | — |
| GitHub | `github` | — |
| 微信 | `wechat` | — |
| | `wechat-mp` | 微信浏览器内使用此 provider，需前端检测 `inWeChat()` 后切换 |

> 仅内置 Google、GitHub、微信三种。其他提供商（如 Facebook、Apple、微博）需要额外配置。

### 8.6 第三方登录按钮示例

```html
<button onclick="oauthLogin('google')">Google 登录</button>
<button onclick="oauthLogin('github')">GitHub 登录</button>
<button onclick="oauthLogin('wechat')">微信登录</button>

<script>
function inWeChat() {
  return navigator.userAgent.toLowerCase().indexOf('micromessenger') !== -1
}

async function oauthLogin(provider) {
  // 微信浏览器内需使用 wechat-mp
  if (provider === 'wechat' && inWeChat()) {
    provider = 'wechat-mp'
  }
  var redirectUri = location.href
  var res = await fetch('/_api/v2/oauth/Url/' + provider, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ redirectUri: redirectUri })
  })
  var url = await res.json()
  location.href = url
}
</script>
```

---

## 九、其他用户 API

### 获取当前用户信息（需登录）

```
GET /_api/v2/user/GetUser
```

需要携带 `jwt_token` cookie。

```json
{
  "id": "...",
  "userName": "...",
  "emailAddress": "...",
  "fullName": "...",
  "firstName": "...",
  "lastName": "...",
  "language": "...",
  "tel": "...",
  "isAdmin": true
}
```

### 检查用户名是否存在

```
GET /_api/v2/user/IsUniqueName?name=xxx
```

返回 `true` 表示可用。

### 检查邮箱是否已注册

```
GET /_api/v2/user/IsUniqueEmail?email=xxx@example.com
```

返回 `true` 表示可用。

### 登出

```
POST /_api/v2/user/logout
Content-Type: application/json
```

| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| returnUrl | string | 否 | 登出后跳转地址 |

```js
await fetch('/_api/v2/user/logout', {
  method: 'POST',
  headers: { 'Content-Type': 'application/json' },
  body: JSON.stringify({ returnUrl: '/' })
})
location.href = '/'
```

---


---

## 十一、完整登录页组合示例

```html
<!DOCTYPE html>
<html>
<head>
<meta charset="utf-8" />
<meta name="viewport" content="width=device-width,initial-scale=1" />
<title>登录</title>
<style>
* { box-sizing: border-box; margin: 0; padding: 0; }
body { font-family: sans-serif; background: #f5f5f5; display: flex; justify-content: center; align-items: center; min-height: 100vh; }
.card { background: #fff; border-radius: 12px; padding: 40px; width: 380px; box-shadow: 0 2px 12px rgba(0,0,0,.1); }
h2 { text-align: center; margin-bottom: 24px; }
input { width: 100%; padding: 10px 12px; border: 1px solid #ddd; border-radius: 6px; font-size: 14px; margin-bottom: 12px; }
.btn { width: 100%; padding: 10px; border: none; border-radius: 6px; font-size: 15px; cursor: pointer; }
.btn-primary { background: #4f46e5; color: #fff; margin-bottom: 16px; }
.btn-outline { background: transparent; border: 1px solid #ddd; margin-bottom: 8px; display: flex; align-items: center; justify-content: center; gap: 8px; }
.btn-outline:hover { background: #f0f0f0; }
.links { text-align: center; margin-top: 16px; font-size: 13px; color: #666; }
.links a { color: #4f46e5; cursor: pointer; margin: 0 4px; }
.error { color: #ef4444; font-size: 13px; margin-bottom: 12px; display: none; }
</style>
</head>
<body>

<div class="card">
  <h2>登录</h2>
  <p class="error" id="error"></p>
  <form id="login-form">
    <input name="username" placeholder="用户名或邮箱" required />
    <input name="password" type="password" placeholder="密码" required />
    <button type="submit" class="btn btn-primary">登录</button>
  </form>

  <button class="btn btn-outline" onclick="oauthLogin('google')">Google 登录</button>
  <button class="btn btn-outline" onclick="oauthLogin('github')">GitHub 登录</button>

  <div class="links">
    <a href="/register">注册</a>
    <a href="/forgot-password">忘记密码</a>
  </div>
</div>

<script>
var qs = new URLSearchParams(location.search)
var returnurl = qs.get('returnurl')
var oauthId = qs.get('oauthid')
var requireInfo = qs.get('requireinfo')
var errorEl = document.getElementById('error')

/* === OAuth 回调处理 === */
if (oauthId && requireInfo === 'true') {
  location.href = '/_admin/login/bind/name?oauthid=' + oauthId
}

/* === 密码登录 === */
function setCookie(token) {
  document.cookie = 'jwt_token=' + token + ';path=/;max-age=' + (30*24*60*60)
}

document.getElementById('login-form').onsubmit = async function(e) {
  e.preventDefault()
  var f = e.target
  errorEl.style.display = 'none'

  var res = await fetch('/_api/v2/user/login', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({
      UserName: f.username.value,
      Password: f.password.value,
      withToken: true,
      returnurl: returnurl || ''
    })
  })

  if (res.ok) {
    setCookie(res.headers.get('access_token'))
    location.href = returnurl || '/'
  } else {
    var msgs = await res.json()
    var msg = msgs[0] || '登录失败'
    if (msg.startsWith('NeedVerifyCode')) {
      var code = prompt('请输入验证码')
      if (code) {
        var res2 = await fetch('/_api/v2/user/login', {
          method: 'POST',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify({
            UserName: f.username.value,
            Password: f.password.value,
            withToken: true,
            returnurl: returnurl || '',
            code: code
          })
        })
        if (res2.ok) { setCookie(res2.headers.get('access_token')); location.href = returnurl || '/' }
        else { var msgs2 = await res2.json(); errorEl.innerText = msgs2[0] || '验证失败'; errorEl.style.display = 'block' }
      }
    } else {
      errorEl.innerText = msg
      errorEl.style.display = 'block'
    }
  }
}

/* === 第三方登录 === */
async function oauthLogin(provider) {
  var redirectUri = location.href
  var res = await fetch('/_api/v2/oauth/Url/' + provider, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ redirectUri: redirectUri })
  })
  var url = await res.json()
  location.href = url
}
</script>

</body>
</html>
```

---

## 十二、API 汇总

### 无需登录

| 方法 | 路径 | 说明 |
|------|------|------|
| POST | `/_api/v2/user/login` | 密码登录（需 `withToken: true`） |
| POST | `/_api/v2/user/register` | 注册 |
| POST | `/_api/v2/user/ForgotPassword` | 发送重置密码邮件 |
| POST | `/_api/v2/user/ResetPassword` | 重置密码（需邮件中的 token） |
| POST | `/_api/v2/user/Logout` | 登出 |
| GET | `/_api/v2/user/IsUniqueName?name=xxx` | 检查用户名可用 |
| GET | `/_api/v2/user/IsUniqueEmail?email=xxx` | 检查邮箱可用 |
| POST | `/_api/v2/user/SendTelCode` | 发送短信验证码 |
| POST | `/_api/v2/verifycode/SmsCode` | 发送短信验证码 |
| POST | `/_api/v2/verifycode/VerifyTelCode` | 验证手机验证码并返回 access_token |
| POST | `/_api/v2/password/EmailCode` | 发送邮箱验证码 |
| POST | `/_api/v2/password/ResetByEmail` | 通过邮箱验证码重置密码 |
| POST | `/_api/v2/password/SmsCode` | 发送手机验证码 |
| POST | `/_api/v2/password/ResetByTel` | 通过手机验证码重置密码 |
| POST | `/_api/v2/oauth/Url/{provider}` | 获取 OAuth 授权 URL |

### 需登录（携带 jwt_token cookie）

| 方法 | 路径 | 说明 |
|------|------|------|
| GET | `/_api/v2/user/GetUser` | 获取当前用户信息 |
| POST | `/_api/v2/user/ChangePassword` | 修改密码 |
| POST | `/_api/v2/user/UpdateProfile` | 更新个人信息 |
| POST | `/_api/v2/user/UpdateName` | 更新姓名 |

---

## 相关文档

- [站点认证方式指南](../Scenario/authentication-guide.md) — 登录方案选型、SSO、自定义账号与扩展用户信息总览
- [k.account](../K-Script/account/index.md) — 后端 k.script 中的账户操作
