# Integrations · SMS

> Group: **SMS**  
> Deep link: `/_Admin/system/config?SiteId={siteGuid}&group=SMS`

The **SMS** group configures SMS provider parameters for verification codes, notifications, etc. (exact calls depend on scripts/modules).

<DocImage src="/cms/settings/settings-integrations-sms.png" alt="SMS group" width="1120" />

## Integration Items

| Name | Description |
|------|-------------|
| **AliSMS** | Alibaba Cloud SMS: `accessId`, `accessSecret`, `regionId`, `signName`, etc. |
| **TencentSMSSetting** | Tencent Cloud SMS |
| **ChinaMobileSMSSetting** | China Mobile SMS |

## Admin Steps

1. Enable SMS at provider; create **signature** and **template**.  
2. **Edit** item here; enter keys and signature name.  
3. **Save**, test send in business Code or form verification.

Field names come from **edit dialog** (`GetFields`); do not leak secrets to public repos.

## Related

- [Integrations overview](./index.md)
