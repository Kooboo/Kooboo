# Service Integrations - Others

## Page Purpose

- Others currently contains JwtSetting and FirebaseSetting. Both are runtime credentials for site code and do not automatically create login pages, access-control rules, or user-management interfaces.

## JWT

- JwtSetting stores the HMAC-signing Secret, default expiration seconds, and whether expiration is added automatically when issuing.
- The encoding entry adds the default expiration to the current UTC time only when automatic expiration is enabled and the claims to be issued have no `exp`; an existing `exp` is not overwritten.
- The decoding entry reads the token from Authorization Bearer and can fall back to the `jwt_access_token` Cookie when absent, then validates the signature and the expiration already in the token.
- Without an explicitly configured Secret, the KScript JWT entry may fall back to the site identifier as the signing key. This is not a production-safe configuration; save a strong random Secret and handle invalidation of old tokens after changing it.
- An entry that only parses the payload does not validate the signature and cannot be used for authentication. JwtSetting also does not automatically create authentication rules, login issuance flows, or permission checks.

## Firebase

- FirebaseSetting stores Firebase Admin service-account Credential JSON for site code to validate ID tokens, create custom tokens, and manage Firebase users.
- Credential should come from the correct Firebase project's service account; a successful save does not validate JSON, private keys, project permissions, or external network access.
- Firebase Admin configuration is not frontend Firebase Client configuration and does not automatically initialize the browser SDK, create login UI, synchronize Kooboo site users, or establish site sessions.
- Managing users, creating tokens, and generating email action links make real Firebase calls and may create external side effects; calling code must handle permissions, errors, and business consistency.

## Security and Verification

- JWT Secret and Firebase service accounts are highly sensitive credentials. Do not use example values, expose them through screenshots or logs, or grant broad configuration-view and edit permissions.
- For JWT, actually verify issuance, Bearer and Cookie reads, expiration, invalid signatures, and key rotation. For Firebase, use a controlled project to verify tokens, permissions, and target-user operations. A saved value shown in the list is not evidence of an authentication or Firebase business loop.
