# k.google

> Google PageSpeed/Lighthouse 与 Firebase Admin 服务

使用 `k.google` 调用 PageSpeed/Lighthouse，使用 `k.google.firebase` 调用 Firebase Admin。访问 `k.google` 时会立即初始化 Firebase；因此即使只调用 PageSpeed 方法，Firebase 服务账号配置缺失或无效也可能导致初始化失败。

## PageSpeed / Lighthouse

```ts
const json = k.google.lHRJson("https://www.kooboo.com", false);
const summary = k.google.parseJson(json).getSummary();

return {
  performance: summary.performance,
  accessibility: summary.accessibility,
  seo: summary.seo,
};
```

| 方法 | 契约 |
| --- | --- |
| `lHRJson(absoluteUrl, mobileDevice)` | 请求远程 PageSpeed JSON 并返回响应字符串。`absoluteUrl` 会直接拼入请求，runtime 不会显式编码 URL；远程失败会向外传播。 |
| `lHRUrl(absoluteUrl, mobileDevice)` | 调用 `lHRJson()`，再把响应嵌入 Kooboo Lighthouse 报告 URL。 |
| `lHRUrlFromJson(json)` | 将输入文本包装为 Lighthouse payload 并压缩到报告 URL；不会先验证 JSON。 |
| `parseJson(json)` | 将 JSON 反序列化为 Lighthouse 结果对象；非法 JSON 可能抛异常。 |
| `saveJson(json)` | 将报告文本写入临时存储并返回 `Guid`。 |
| `getJson(id)` | 根据 `Guid` 读取报告；文件不存在时返回 `null`。 |
| `shortLhrReportUrl(json)` | 保存报告并返回站点内的 `/_api/LightHouse/ReadReport?id=...` URL。 |

`mobileDevice` 仍是必填兼容参数；当前请求不会根据该值切换策略。

## Firebase

当前公开方法包括 `verifyIdToken`、`createCustomToken`、`createUser`、`deleteUser`、`updateUser`、`getUserByEmail`、`getUserByPhoneNumber`、`generateEmailVerificationLink`、`generateSignInWithEmailLink` 和 `generatePasswordResetLink`。

创建 `k.google` 时就要求有效的 Firebase 服务账号配置。Firebase 失败、Token 无效或用户参数无效时可能抛异常，不要假设这些方法会返回统一的空值失败结果。

`generatePasswordResetLink(email, settings)` 重载虽然名称是密码重置，但当前实际调用的是 Firebase 邮件链接登录 API。不要把它当作密码重置链接；密码重置使用单参数重载，邮件链接登录使用 `generateSignInWithEmailLink`。
