---
name: kooboo-coding-k-script
description: Use when working with KScript modules in Kooboo site coding workflows.
---

# KScript Guide

Use this index to select the module required by the current task, then read only that module's reference. Do not preload unrelated modules.

## Read before writing KScript

Whenever a `k.*` return value will be used in native `href` interpolation, `k-for`, `k-content`, or `k-attribute`, or mapped / extended with new fields in a script, **you must first read** [KScript return objects and template binding](kscript-clr-objects.md). Do not assign to API return items with `forEach` or spread `{...item}`.

## Module navigation

### Core modules

| Module | Purpose |
|------|------|
| [Cookie](core/cookie.md) | Client-side Cookie read/write |
| [Request](core/request.md) | HTTP request information |
| [Response](core/response.md) | HTTP response handling |
| [API](core/api.md) | API route definition |
| [Security](core/security.md) | Security tools (hashing/encryption/JWT) |
| [Logger](core/logger.md) | Logging (debug/info/warning/error) |
| [Media](core/media.md) | In-site media library (images, video, audio) — prefer this module for in-site resources |

### Utility modules

| Module | Purpose |
| --- | --- |
| [Utils overview](utils/index.md) | Select the required utility module, including Office, XML, screenshots, SMS, and more |

### Business modules

- [Net module](net/index.md) (HTTP, WebSocket, DNS, IP, Ping, SSH, SFTP, and Telnet)
- [Google services](google.md) (PageSpeed/Lighthouse and Firebase)
- [Account module](account/index.md)
- [Commerce module](commerce/index.md)
- [Payment module](payment/index.md)
- [File operations](file/index.md)
- [DB module](DB/index.md)
- [Content module](content/index.md) (see [function_guide](content/function_guide.md) for modeling)
