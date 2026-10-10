# 服务集成 · OAuth2

> Group：**OAuth2**  
> 深链：`/_Admin/system/config?SiteId={站点GUID}&group=OAuth2`

**OAuth2** 分组配置第三方 **社交/平台登录**（AppId、Secret、回调等）。保存后，前台通过 Kooboo OAuth 回调路由完成授权；弹窗 **Alert** 常给出须在厂商控制台登记的 **回调 URL**（如 `{BaseUrl}_api/oauth2callback/GoogleLogin`）。

<DocImage src="/cms/settings/settings-integrations-oauth2.png" alt="OAuth2 分组" width="1120" />

## 集成项

| 名称 | 说明 |
|------|------|
| **GoogleLoginSetting** | Google 登录 |
| **FacebookLoginSetting** | Facebook 登录 |
| **WeChatLoginSetting** | 微信登录 |
| **WeiboLoginSetting** | 微博登录 |
| **AppleLoginSetting** | Apple 登录 |

部分项含 **CallbackCodeName** 等字段，用于与站点 Code 回调配合，以编辑弹窗为准。

## 与身份验证的区别

| | **OAuth2（本页）** | [开发 → 身份验证](../../development/authentication.md) |
|--|-------------------|--------------------------------------------------------|
| 用途 | 第三方 **登录授权** 配置 | 请求 **网关规则**（JWT / 自定义 Code） |
| 典型场景 | 「使用 Google 登录」 | API 必须带 JWT 才放行 |

二者可配合：OAuth 完成登录签发令牌，身份验证规则校验后续请求。

## 后台操作

1. 在 Google / 微信等开放平台创建应用，记下 AppId、Secret。  
2. 将 Alert 中的 **回调地址** 填入厂商控制台。  
3. 本页 **编辑** 对应 **LoginSetting**，保存。  
4. 前台链接触发登录流程测试。

## 相关

- [服务集成概述](./index.md)  
- [k.security.jwt](../../../api/security/index.md)（JWT 校验）
