# k.account.oAuth

> Third-party login authorization URL and callback processing

## Overview

`k.account.oAuth` generates authorized links in the site that jump to third-party platforms. You must first fill in the corresponding login settings (AppId, Secret, callback Code name, etc.) in the console **System → Configuration**, and create a **Code** (event script) processing `k.request` for receiving callbacks.

## Member list

| Member | Description |
|------|------|
| `weChat` | WeChat login (`getAuthUrl`, `getAuthJson`) |
| `weibo` | Weibo |
| `google` | Google |
| `facebook` | Facebook |
| `apple` | Apple |
| `activeDirectory` | Active Directory |
| `builtIn` | Built-in unified portal for multiple providers (`getAuthUrl`, `getUserInfo`, `providers`) |

## weChat

```html
<script engine="kscript">
    var url = k.account.oAuth.weChat.getAuthUrl({
        state: "custom_state"
    })
</script>
<a k-href="url">WeChat Login</a>
```

Scan the QR code to log in to output JSON configuration:

```html
<script engine="kscript">
    k.response.write(k.account.oAuth.weChat.getAuthJson({
        id: "qrcode_container",
        state: "custom_state"
    }))
</script>
```

## google/facebook/apple

Most providers support `getAuthUrl()` without parameters or passed in parameters, whichever is determined by smart prompts. Example (Google):

```html
<script engine="kscript">
    var url = k.account.oAuth.google.getAuthUrl()
</script>
<a k-href="url">Google Login</a>
```

The usage is the same for Facebook and Apple, replaced by `k.account.oAuth.facebook`, `k.account.oAuth.apple`.

## weibo

```html
<script engine="kscript">
    var url = k.account.oAuth.weibo.getAuthUrl()
</script>
<a k-href="url">Weibo Login</a>
```

## builtIn

Unified use of Kooboo’s built-in OAuth account configuration:

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

Get user information on the callback page:

```ts
k.api.get(() => {
    const user = k.account.oAuth.builtIn.Google.getUserInfo()
    return user
})
```

There are also lowercase entries such as `google`, `facebook`, `github`, `wechat` under `builtIn`, and uppercase entries such as `Google` according to the IDE prompts.

## Typical Flow

1. The console configuration corresponds to **LoginSetting** and the callback Code name.
2. Create a script named callback Code and process `k.request` (such as writing session and jumping) in the callback.
3. The login button page uses `getAuthUrl()` to generate links, and `k.response.redirect(url)` or `<a k-href>` to jump.
4. The user returns to the site after third-party authorization, and the callback code completes the login logic.

## Related Docs

- [k.account Overview](./index.md)
- [Login](./login.md)
