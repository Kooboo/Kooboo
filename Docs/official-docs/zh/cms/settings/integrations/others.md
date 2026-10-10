# 服务集成 · Others

> Group：**Others**  
> 深链：`/_Admin/system/config?SiteId={站点GUID}&group=Others`

**Others** 分组存放不便归入 Database / Payment 等的 **站点级扩展集成**。

<DocImage src="/cms/settings/settings-integrations-others.png" alt="Others 分组" width="1120" />

## 集成项

| 名称 | 说明 |
|------|------|
| **JwtSetting** | JWT 签发：`Secret`、`Exp`（秒）、`EnableExp`；Alert 含示例。用于 [身份验证](../../development/authentication.md) 等场景的令牌校验（见 [k.security.jwt](../../../api/security/index.md)） |
| **FirebaseSetting** | Firebase 相关配置（字段以弹窗为准） |

## 后台操作

1. **编辑** 目标项。  
2. 按 Alert 生成或填写 **Secret** 等参数。  
3. **保存** 后在依赖该配置的 Code / 身份验证规则中验证。

## 相关

- [服务集成概述](./index.md)  
- [基础设置 · 高级](../basic/advanced.md)
