# 服务集成 · SMS

> Group：**SMS**  
> 深链：`/_Admin/system/config?SiteId={站点GUID}&group=SMS`

**SMS** 分组配置短信服务商参数，供站点发送验证码、通知等（具体调用以脚本/模块为准）。

<DocImage src="/cms/settings/settings-integrations-sms.png" alt="SMS 分组" width="1120" />

## 集成项

| 名称 | 说明 |
|------|------|
| **AliSMS** | 阿里云短信：`accessId`、`accessSecret`、`regionId`、`signName` 等 |
| **TencentSMSSetting** | 腾讯云短信 |
| **ChinaMobileSMSSetting** | 中国移动短信 |

## 后台操作

1. 在厂商控制台开通短信服务并创建 **签名**、**模板**。  
2. 本页 **编辑** 对应项，填入密钥与签名名。  
3. **保存** 后在业务 Code 或表单验证流程中测试发送。

字段名以 **编辑弹窗** 为准（动态 `GetFields`）；密钥勿泄露至公开仓库。

## 相关

- [服务集成概述](./index.md)
