# 服务集成 - Others

## 页面用途

- Others 当前包含 JwtSetting 和 FirebaseSetting。两者都是供站点代码使用的运行时凭证，不会自动创建登录页面、访问控制规则或用户管理界面。

## JWT

- JwtSetting 保存 HMAC 签名 Secret、默认过期秒数以及是否在签发时自动加入过期时间。
- 只有开启自动过期且待签发 claims 中没有 `exp` 时，编码入口才会按当前 UTC 时间加上默认过期值；已有 `exp` 不会被覆盖。
- 解码入口从 Authorization Bearer 读取 token，缺失时可回退读取 `jwt_access_token` Cookie，并验证签名和 token 中已有的过期时间。
- 未显式配置 Secret 时，KScript JWT 入口可能退回使用站点标识作为签名密钥。这不应当作生产安全配置；应保存高强度随机 Secret，并在变更后处理旧 token 失效问题。
- 仅解析 payload 的入口不会验证签名，不能用于鉴权。JwtSetting 也不会自动创建认证规则、登录签发流程或权限检查。

## Firebase

- FirebaseSetting 保存 Firebase Admin 服务账户 Credential JSON，供站点代码验证 ID token、创建自定义 token 及管理 Firebase 用户。
- Credential 应来自正确 Firebase 项目的服务账户；保存成功不会验证 JSON、私钥、项目权限或外部网络连接。
- Firebase Admin 配置不等于前端 Firebase Client 配置，也不会自动初始化浏览器 SDK、创建登录 UI、同步 Kooboo 站点用户或建立站点会话。
- 管理用户、创建 token 和生成邮件动作链接都会真实调用 Firebase，可能产生外部副作用；调用代码必须处理权限、错误和业务一致性。

## 安全与验证

- JWT Secret 和 Firebase 服务账户都属于高敏感凭证。不要使用示例值，不要通过截图或日志传播，并限制配置查看和编辑权限。
- JWT 应实际验证签发、Bearer 与 Cookie 读取、过期、错误签名和密钥轮换；Firebase 应用受控项目验证 token、权限和目标用户操作。列表显示已保存不能作为鉴权或 Firebase 业务闭环证据。
