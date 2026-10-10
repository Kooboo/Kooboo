# k.account.oAuth

> 第三方登录授权 URL 与回调处理

## 概述

`k.account.oAuth` 在站点中生成跳转到第三方平台的授权链接。须先在控制台 **系统 → 配置** 中填写对应登录设置（AppId、Secret、回调 Code 名称等），并创建用于接收回调的 **Code**（事件脚本）处理 `k.request`。

## 成员一览

| 成员 | 说明 |
|------|------|
| `weChat` | 微信登录（`getAuthUrl`、`getAuthJson`） |
| `weibo` | 微博 |
| `google` | Google |
| `facebook` | Facebook |
| `apple` | Apple |
| `activeDirectory` | Active Directory |
| `builtIn` | 内置多提供商统一入口（`getAuthUrl`、`getUserInfo`、`providers`） |

## weChat

```html
<script engine="kscript">
    var url = k.account.oAuth.weChat.getAuthUrl({
        state: "custom_state"
    })
</script>
<a k-href="url">微信登录</a>
```

扫码登录可输出 JSON 配置：

```html
<script engine="kscript">
    k.response.write(k.account.oAuth.weChat.getAuthJson({
        id: "qrcode_container",
        state: "custom_state"
    }))
</script>
```

## google / facebook / apple

多数提供商支持无参或传入参数的 `getAuthUrl()`，具体以智能提示为准。示例（Google）：

```html
<script engine="kscript">
    var url = k.account.oAuth.google.getAuthUrl()
</script>
<a k-href="url">Google 登录</a>
```

Facebook、Apple 用法相同，替换为 `k.account.oAuth.facebook`、`k.account.oAuth.apple`。

## weibo

```html
<script engine="kscript">
    var url = k.account.oAuth.weibo.getAuthUrl()
</script>
<a k-href="url">微博登录</a>
```

## builtIn

统一使用 Kooboo 内置 OAuth 账号配置：

```ts
k.api.get(() => {
    const providers = k.account.oAuth.builtIn.providers()
    return { providers }
})
```

```html
<script engine="kscript">
    var url = k.account.oAuth.builtIn.Google.getAuthUrl("/oauth/callback")
</script>
```

在回调页获取用户信息：

```ts
k.api.get(() => {
    const user = k.account.oAuth.builtIn.Google.getUserInfo()
    return user
})
```

`builtIn` 下另有 `google`、`facebook`、`github`、`wechat` 等小写入口，与 `Google` 等大写类型按 IDE 提示选用。

## 典型流程

1. 控制台配置对应 **LoginSetting** 与回调 Code 名称。  
2. 创建名为回调 Code 的脚本，在回调中处理 `k.request`（如写 session、跳转）。  
3. 登录按钮页用 `getAuthUrl()` 生成链接，`k.response.redirect(url)` 或 `<a k-href>` 跳转。  
4. 用户在第三方授权后回到站点，由回调 Code 完成登录逻辑。

## 相关文档

- [k.account 概述](./index.md)
- [登录](./login.md)
