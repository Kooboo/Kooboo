# Integrations · Others

> Group: **Others**  
> Deep link: `/_Admin/system/config?SiteId={siteGuid}&group=Others`

The **Others** group holds **site-level extension integrations** that do not fit Database / Payment / etc.

<DocImage src="/cms/settings/settings-integrations-others.png" alt="Others group" width="1120" />

## Integration Items

| Name | Description |
|------|-------------|
| **JwtSetting** | JWT issuance: `Secret`, `Exp` (seconds), `EnableExp`; Alert includes examples. Used for token validation in [Authentication](../../development/authentication.md) ([k.security.jwt](../../../api/security/index.md)) |
| **FirebaseSetting** | Firebase-related config (fields per dialog) |

## Admin Steps

1. **Edit** the target item.  
2. Generate or fill **Secret** etc. per Alert.  
3. **Save**, then verify in dependent Code / authentication rules.

## Related

- [Integrations overview](./index.md)  
- [Basic Settings · Advanced](../basic/advanced.md)
