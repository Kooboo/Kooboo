# Site Preflight

Site Building，以及尚未确立 App profile 的普通对象级编码会话，使用 Site profile。会话确立 App 身份后，Plan 绑定失效时必须重新执行 App 校验。

AI Chat adapter 会权威注入 `profile: "site"`。不要提交 adapter 管理的绑定字段。`taskType`、`layers`、`deliveryStage`、`mode` 和 `artifacts` 继续遵循 [Preflight Core](index.md) 的共享契约。

Site Preflight 负责：

- Site Brief 与 Page Plan 交付阶段；
- Site 交付阶段，以及当前仅适用于 Site 的 k-data/Commerce-template 检查；
- 按实际能力选择与 App 共享的装配、导航、多语言、资源、Commerce 和 Payment 检查，参见[能力分类](capabilities.md)；
- 收尾检查与自然的下一步推荐。

Site 回执包含 `structuralReceipt.profile: "site"`。结构 PASS 仍不能替代运行证据和业务证据。
