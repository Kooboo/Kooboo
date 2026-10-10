# Service Integrations - OAuth2

## Page Purpose

- The OAuth2 group stores client information for Weibo, Facebook, WeChat, Google, and Apple login applications, together with the callback-code name to execute after authorization.
- This page does not automatically create login buttons, login pages, site users, sessions, or account-binding logic. The frontend must still generate the provider authorization URL, and callback code decides how to handle user information and login state.

## Callback Addresses and Domains

- Each provider uses a fixed callback entry under the site's public base address in the form `/_api/oauth2callback/{Provider}`. Register the exact address shown on the page in the provider console.
- After changing the primary domain, protocol, or site base address, update the provider's callback allowlist as well. Most providers require a publicly accessible HTTPS address; saving AppId and Secret alone cannot solve DNS, certificate, or callback-allowlist problems.
- WeChat web login also involves an authorization domain in the provider console; domain registration and the complete callback URL are not the same configuration.

## Authorization and Callback Code

- After frontend authorization begins, the provider returns code or error to the fixed callback entry. The system uses configured credentials to exchange for a token and user information, then passes the result to the specified site callback code.
- Callback code must actually exist; a wrong name or deleted code causes authorization to fail at callback time. Saving configuration does not verify or generate callback code.
- A token and user profile reaching callback code does not mean the user is logged in. Creating or finding a site user, establishing a Cookie or JWT, handling state, preventing mistaken account binding, and deciding the final redirect are callback-code responsibilities.
- `state` prevents request mix-up only when both frontend authorization and callback handling validate it correctly; its presence in a URL alone does not prove security validation is complete.

## Risks and Verification

- App Secret, Apple private keys, and similar values are sensitive credentials. Changing them affects later token exchange but does not revoke provider-issued tokens or existing site sessions.
- Verification must cover authorization denial, missing code, token-exchange failure, missing user profile, callback-code errors, duplicate callbacks, and final login state. Being able to open the provider authorization page does not mean OAuth login succeeded.
