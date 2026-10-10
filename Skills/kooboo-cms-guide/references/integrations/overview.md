# Service Integrations - Overview

## Page Purpose

- This page centrally stores external services and runtime credentials used by the current site. Built-in groups include Database, Payment, SMS, OAuth2, and Others.
- Groups are configuration categories, not enabled status or runtime results. An item appearing in the list does not mean it is filled in; a successful save proves only that configuration was written to the site, not that the external service is reachable, credentials valid, or the business flow succeeds.
- These settings are usually read by name by KScript, payment, login callbacks, or other runtime entries. They do not automatically create pages, APIs, login flows, checkout flows, or admin jobs.

## Editing and Permissions

- The page shows text, switches, or small-file fields according to the configuration type. Editing one item overwrites its saved fields and does not automatically migrate other configurations of the same kind.
- Values may contain database passwords, payment keys, SMS keys, OAuth Secret, JWT Secret, or service-account credentials. Users with configuration-view permission may see sensitive values in the list or editor; do not share them through screenshots, chats, or logs.
- After deleting text or uploading a new file, clicking save immediately affects later requests that read the configuration; established connections, in-flight requests, and external side effects are not automatically undone.

## Verification Principles

- The configuration page has no unified Test All Connections loop. Verify through the corresponding real entry: run a minimal database query, perform a controlled SMS test, complete payment-provider and callback verification, complete OAuth authorization and callback-script verification, and complete JWT issuance and validation.
- When verification fails, distinguish save errors, network or DNS, provider credentials, permissions or quotas, callback addresses, site publication state, and business-code errors; repeatedly saving the same key is not enough.
