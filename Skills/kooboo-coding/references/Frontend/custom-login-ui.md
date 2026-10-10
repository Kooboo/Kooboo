# Custom Login UI Development Guide

> A site can call the Kooboo login APIs directly on the same domain. You can freely design login, registration, and password-recovery pages without rewriting the backend logic.

## 1. Login Entry and URL Parameters

When a user visits a site page that requires authentication, Kooboo middleware redirects the user to the login page with HTTP 302. The URL carries these parameters:

```
/_admin/login?permission=o&returnurl=%2F
```

| Parameter | Source | Description |
|------|------|------|
| `permission` | `WebSite.SiteType` | Site type: `p` = public site, `o` = organization user, `m` = site member, `u` = authenticated user |
| `returnurl` | URL-encoded original request path | Destination to return to after successful login |

**A custom login page must read these URL parameters and pass `returnurl` through every API request and redirect**; otherwise users may be sent to the system back office instead of the site after login.

> Publish the custom login page at `/_admin/login` to override the default `/Kooboo.Web/_Admin/login` page.
> Kooboo middleware will automatically redirect unauthenticated users to your custom login page.

Read the parameters:

```js
var qs = new URLSearchParams(location.search)
var returnurl = qs.get('returnurl')
var permission = qs.get('permission')
```

---

## 2. Token Mechanism (Required Reading)

Kooboo uses the **`jwt_token` cookie** as the authentication credential.

### Establishing the Login State

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

### Key Points

1. **The login request must include `withToken: true`**; otherwise the response headers will not contain `access_token`.
2. **The login request must include `returnurl`** so the post-login destination is known.
3. After a successful login, **`access_token` is in the response headers** (not the body). Extract it, set it manually as the `jwt_token` cookie (path=/, 30-day expiration), and then redirect to `returnurl`.

### Check the Login State

```js
function isLogin() {
  return document.cookie.indexOf('jwt_token=') !== -1
}
```

### Log Out

```
POST /_api/v2/user/logout
```

Delete the `jwt_token` and `_kooboo_api_user` cookies.

---

## 3. Form Validation Rules

> Every page that accepts user input (login, registration, account binding, and so on) should validate it on the frontend first to avoid invalid requests and improve the user experience.

### 3.1 Kooboo Account Rules

| Field | Frontend validation | Server rule | Description |
|------|-------------|-----------|------|
| Username | 5–30 characters, letters, digits, and hyphen `-` only; must start **and** end with a letter or digit | Only checks that it is non-empty; letters, digits, `_`, and `-` are allowed | The frontend rule is stricter; both checks must pass for registration |
| Password | 8–30 characters and at least two of letters, digits, and symbols | No server-side restriction | The server does not check password strength; the frontend is solely responsible |
| Email | Standard email format | Required during registration | — |
| Phone | Digits only, with no length limit (`/^\d*$/`) | No format validation | SMS sending requires a country code, such as `+8613800138000` |

> Note: `+` is required only by the SMS sending API (such as `SendTelCode`). The login-page input may allow `+`, but Kooboo's own frontend `phoneRule` validates digits only.

### 3.2 Common Validation Functions

The following validation rules are taken from the Kooboo source code:

- Username: frontend rule is 5–30 characters, only `a-zA-Z0-9-`, with a letter or digit at both ends; the backend additionally allows `_`
- Password: 8–30 characters and at least two of letters, digits, and symbols
- Phone: the frontend only requires digits `/^\d*$/`
- Reserved prefixes: starts with `_mx`, `_dmarc`, `_acme-challenge`, or `_domainkey` (case-insensitive)

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

### 3.3 Real-Time Validation (Check Username / Email Availability)

Registration and binding pages should call the availability APIs as the user types instead of waiting until submission to report an error:

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

> Note: `IsUniqueName` and `IsUniqueEmail` are built-in Kooboo V2 APIs; no custom backend logic is required.

### 3.4 Integrate Validation into Form Submission

Every form should run frontend validation before submission to avoid invalid requests:

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

## 4. Password Login

```
POST /_api/v2/user/login
Content-Type: application/json
```

### Request Parameters

| Parameter | Type | Required | Description |
|------|------|------|------|
| UserName | string | Yes | Username or email |
| Password | string | Yes | Password |
| withToken | boolean | **Yes** | Must be `true`; otherwise the response headers do not contain access_token |
| returnurl | string | **Strongly recommended** | Post-login destination (from the URL parameter); omitting it may redirect to the system back office |
| code | string | No | Two-factor code (send it only when retrying after it is requested) |

### Successful Response

The V2 API returns HTTP 200 on success, with **`access_token` in the response headers**:

```
HTTP 200
access_token: eyJhbGciOi...
Content-Type: application/json
```

The response body is a redirect URL (`/_Admin/start?accesstoken=xxx`), but it is not needed. Extract `access_token` from the response headers, set the `jwt_token` cookie, and redirect.

### Failed Response

When the V2 API fails, the response body is a `Messages` array:

```json
["User name or password not valid"]
```

### Two-Factor Authentication

On failure, the Messages array may contain `"NeedVerifyCode email|tel|otp"`:

```json
["NeedVerifyCode email"]
```

Prompt the user for the code and retry with the `code` parameter.

### Complete HTML Example

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

## 5. Registration

```
POST /_api/v2/user/register
Content-Type: application/json
```

### Request Parameters

| Parameter | Type | Required | Description |
|------|------|------|------|
| UserName | string | Yes | Username |
| Password | string | Yes | Password |
| email | string | Yes | Email |
| code | string | No | Verification code (when required) |
| withToken | boolean | **Recommended** | Set to `true` to obtain access_token |

### Successful Response (Immediate Registration)

The V2 API returns HTTP 200 on success, with **`access_token` in the response headers**:

```
HTTP 200
access_token: eyJhbGciOi...
Content-Type: application/json
```

Extract `access_token` from the response headers, set the `jwt_token` cookie, and redirect.

### Verification Code Required

```
HTTP 202
Content-Type: application/json

""
```

HTTP 202 means that a verification code is required. Send the code, then submit again with `code`.

### Failed Response

When the V2 API fails, the response body is a `Messages` array:

```json
["Username exists"]
```

Common error messages include `"Username or password or email not provided"`, `"Invalid User name formate"`, `"Reserved user name prefix"`, `"Username exists"`, and `"Verify code Error"`.

### Complete HTML Example

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

## 6. Forgot Password

### 6.1 Send the Reset Email

```
POST /_api/v2/user/ForgotPassword
Content-Type: application/json
```

| Parameter | Type | Required | Description |
|------|------|------|------|
| Email | string | Yes | Registered email |

The response is a text message returned by the server. The email contains a password-reset link with a reset token.

---

### 6.2 Reset the Password with an Email Code

```
POST /_api/v2/password/ResetByEmail
Content-Type: application/json
```

| Parameter | Type | Required | Description |
|------|------|------|------|
| email | string | Yes | Email |
| code | string | Yes | Code received by email |
| newPassword | string | Yes | New password |

### 6.3 Send an Email Code

```
POST /_api/v2/password/EmailCode
Content-Type: application/json
```

| Parameter | Type | Required | Description |
|------|------|------|------|
| email | string | Yes | Email address |

---

### 6.4 Reset the Password with a Phone Code

```
POST /_api/v2/password/ResetByTel
Content-Type: application/json
```

| Parameter | Type | Required | Description |
|------|------|------|------|
| Tel | string | Yes | Phone number |
| code | string | Yes | SMS code |
| newPassword | string | Yes | New password |

### 6.5 Send a Phone Code

```
POST /_api/v2/password/SmsCode
Content-Type: application/json
```

| Parameter | Type | Required | Description |
|------|------|------|------|
| Tel | string | Yes | Phone number |

---

### Complete HTML Example

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

## 7. Phone-Code Login

### 7.1 Send an SMS Code

```
POST /_api/v2/user/SendTelCode
Content-Type: application/json
```

| Parameter | Type | Required | Description |
|------|------|------|------|
| Tel | string | Yes | Phone number (including country code, such as +8613800138000) |

---

### 7.2 Verify the Phone Code and Log In

```
POST /_api/v2/verifycode/VerifyTelCode
Content-Type: application/json
```

| Parameter | Type | Required | Description |
|------|------|------|------|
| Tel | string | Yes | Phone number |
| code | int | Yes | Verification code |
| returnurl | string | No | Post-login destination |

On success, the V2 API returns `access_token` as a string in the response body; set the cookie manually:

### Complete HTML Example

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

## 8. Third-Party Login (OAuth)

### 8.1 Get the OAuth Authorization URL

```
POST /_api/v2/oauth/Url/{provider}
Content-Type: application/json
```

| Path parameter | Description |
|---------|------|
| provider | `google`, `github`, `wechat`, `facebook`, `apple`, `weibo`, and so on |

| Request parameter | Type | Required | Description |
|---------|------|------|------|
| redirectUri | string | Yes | Landing address after OAuth completes |

Returns a string containing the third-party authorization page URL; redirect to it directly.

### 8.2 OAuth Flow in Detail

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

### 8.3 Correct Value for `redirectUri`

**`redirectUri` must be the complete URL of the current login page (`location.href`)** because the back office redirects the user to this address after the OAuth callback completes. This carries parameters such as `permission` and `returnurl` through the entire OAuth flow.

❌ Incorrect:
```js
var redirectUri = location.origin + '/_api/oauth2callback/GoogleLogin'
// 这是 OAuth 回调处理器地址，不是落地页地址！
```

✅ Correct:
```js
var redirectUri = location.href
// 例如：https://example.com/_admin/login?permission=o&returnurl=%2F
```

### 8.4 Handle the OAuth Callback

After a successful OAuth flow, the user returns to the login page with an `oauthid` or `accesstoken` parameter in the URL:

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

### 8.5 Supported Third-Party Providers

| Name | provider | Notes |
|------|----------|------|
| Google | `google` | — |
| GitHub | `github` | — |
| WeChat | `wechat` | — |
| | `wechat-mp` | Use this provider in the WeChat browser; detect `inWeChat()` on the frontend and switch to it |

> Only Google, GitHub, and WeChat are built in. Other providers such as Facebook, Apple, and Weibo require additional configuration.

### 8.6 Third-Party Login Button Example

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

## 9. Other User APIs

### Get the Current User (Login Required)

```
GET /_api/v2/user/GetUser
```

The request must include the `jwt_token` cookie.

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

### Check Whether a Username Exists

```
GET /_api/v2/user/IsUniqueName?name=xxx
```

`true` means that the username is available.

### Check Whether an Email Is Registered

```
GET /_api/v2/user/IsUniqueEmail?email=xxx@example.com
```

`true` means that the email is available.

### Log Out

```
POST /_api/v2/user/logout
Content-Type: application/json
```

| Parameter | Type | Required | Description |
|------|------|------|------|
| returnUrl | string | No | Post-logout destination |

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

## 11. Complete Login-Page Composition Example

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

## 12. API Summary

### No Login Required

| Method | Path | Description |
|------|------|------|
| POST | `/_api/v2/user/login` | Password login (requires `withToken: true`) |
| POST | `/_api/v2/user/register` | Registration |
| POST | `/_api/v2/user/ForgotPassword` | Send a password-reset email |
| POST | `/_api/v2/user/ResetPassword` | Reset the password (requires the email token) |
| POST | `/_api/v2/user/Logout` | Log out |
| GET | `/_api/v2/user/IsUniqueName?name=xxx` | Check username availability |
| GET | `/_api/v2/user/IsUniqueEmail?email=xxx` | Check email availability |
| POST | `/_api/v2/user/SendTelCode` | Send an SMS code |
| POST | `/_api/v2/verifycode/SmsCode` | Send an SMS code |
| POST | `/_api/v2/verifycode/VerifyTelCode` | Verify a phone code and return access_token |
| POST | `/_api/v2/password/EmailCode` | Send an email code |
| POST | `/_api/v2/password/ResetByEmail` | Reset the password with an email code |
| POST | `/_api/v2/password/SmsCode` | Send a phone code |
| POST | `/_api/v2/password/ResetByTel` | Reset the password with a phone code |
| POST | `/_api/v2/oauth/Url/{provider}` | Get the OAuth authorization URL |

### Login Required (Include the jwt_token Cookie)

| Method | Path | Description |
|------|------|------|
| GET | `/_api/v2/user/GetUser` | Get the current user |
| POST | `/_api/v2/user/ChangePassword` | Change the password |
| POST | `/_api/v2/user/UpdateProfile` | Update the profile |
| POST | `/_api/v2/user/UpdateName` | Update the name |

---

## Related Documentation

- [Site Authentication Guide](../Scenario/authentication-guide.md) — Overview of login selection, SSO, custom accounts, and extended user information
- [k.account](../K-Script/account/index.md) — Account operations in backend k.script
