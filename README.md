# Kooboo

### Build a real website in minutes

Kooboo is an integrated platform for creating, running, and managing websites,
online stores, and web applications. It brings the tools needed to build and
operate a website into one portable system, so you can move from an idea to a
working site without assembling a large collection of separate services.

> **This is the official Kooboo public release repository.**  
> Kooboo is proprietary software. Public releases contain compiled packages;
> source access is available only to approved Kooboo partners under a separate
> written agreement.

## 1. Performance: 5,000 dynamic websites on one small server

Kooboo is designed as one integrated performance path—from domain routing and
data access to rendering and network output. Websites share the optimized
platform runtime without becoming one shared website: each site retains its
own domain, pages, layouts, views, content, files, configuration, analytics,
permissions, and history.

In our published end-to-end benchmark, one Kooboo process hosted **5,000
independently addressable and independently editable dynamic websites** on a
server with **2 vCPUs and 4 GB of memory**.

| Measured result | Value |
| --- | ---: |
| Independent dynamic websites | 5,000 |
| Complete HTML requests attempted | 90,000 |
| Verified first-attempt responses | 89,969 — 99.97% |
| Measured request start rate | 146.59 requests/second |
| Measured completion rate | 146.52 requests/second |
| Complete-content p50 | 296.73 ms |
| Complete-content p95 | 1,431.27 ms |
| Average Kooboo CPU usage | 45.6% of total two-vCPU capacity |

This was an end-to-end public-internet HTTP/HTTPS pressure test, not an empty
response or isolated render-loop benchmark. The full report documents the
hardware, regions, workload, latency distribution, CPU and memory observations,
failures, raw records, limitations, and reproduction procedure.

**[Read the complete 5,000-site benchmark](Docs/BENCHMARK.md)**

![Kooboo dashboard containing a folder of 5,000 independent websites](images/kooboo-5000-sites.png)

> **Do not benchmark the brochure. Download Kooboo and benchmark the product.**

## 2. Native AI: AI that can finish the website

Kooboo Native AI is not a chat box added beside an unrelated website builder.
AI works with the real architecture and operating environment of the website.
It can understand and work with Pages, Layouts, Views, content, databases,
commerce, APIs, server-side JavaScript, media, configuration, analytics, and
history through native platform capabilities.

This gives AI a complete working loop:

**Build → Run → Observe → Diagnose → Modify → Validate → Publish → Measure → Improve**

Kooboo Native AI can:

- Build a real dynamic website rather than only generating a static frontend.
- Work with structured website objects, content models, data, APIs, and
  application logic.
- Open the running website, interact with it, inspect runtime errors and
  abnormal network requests, then continue repairing the result.
- Connect an element selected on the rendered page to the Page, View, or Layout
  that produced it.
- Work inside an isolated Kooboo sandbox before approved changes reach the
  production website.
- Use website history to inspect, compare, undo, and recover supported changes.
- Run scheduled website checks and longer-running operational tasks.
- Work with supported AI providers and models selected by the website owner.

![Kooboo Native AI workspace](images/kooboo-native-ai.png)

**Built by AI. Tested by AI. Run by AI. Owned by you.**

## More Kooboo capabilities

- Visual website building and inline content editing
- Direct HTML, CSS, and JavaScript website development
- Website importing, cloning, packaging, transfer, and deployment
- Content management and multilingual content
- Dynamic database-driven pages and server-side queries
- Online stores, products, customers, carts, orders, and discounts
- Forms, APIs, backend JavaScript, scheduled jobs, and custom applications
- Built-in web hosting, domain routing, and email services
- Server-side traffic analytics, diagnostics, and error inspection
- Reusable Pages, Layouts, Views, HTML blocks, scripts, and styles
- Users, roles, permissions, configuration, and operational history
- Media management and image processing
- Version comparison, rollback, restoration, and website checkout

## Download

Download the newest official build from the
[latest Kooboo release](https://github.com/Kooboo/Kooboo/releases/latest).

Depending on the release, packages may include:

| Package | Intended use |
| --- | --- |
| Windows x64 | Windows servers and desktop installations |
| Linux x64 | 64-bit Linux servers |
| Portable package | Extract-and-run installation or manual deployment |
| Windows installer | Guided installation on Windows |

Public release assets are compiled binaries and do not include the Kooboo
source repository.

## Quick start

1. Open the [latest release](https://github.com/Kooboo/Kooboo/releases/latest).
2. Download the package for your operating system.
3. Verify the package using the published SHA-256 checksum when available.
4. Extract the portable package or run the installer.
5. Start Kooboo and open the local address shown by the application.
6. Create a website, import an existing site, or begin with AI-assisted
   website creation.

Before upgrading a production installation, back up your Kooboo data and read
the notes accompanying that release.

## What you can build

Kooboo supports projects ranging from a single website to integrated business
applications, including:

- Marketing and company websites
- Content-rich and multilingual websites
- Online stores and product catalogs
- Customer portals and internal applications
- Custom database-driven web applications
- Self-hosted web and email services

You can begin visually, work directly with HTML, CSS, and JavaScript when
needed, and package the completed website for deployment or transfer.

## Documentation and support

- [Kooboo website](https://www.kooboo.com/)
- [Downloads](https://www.kooboo.com/downloads)
- [GitHub releases](https://github.com/Kooboo/Kooboo/releases) 

When reporting a problem, include the Kooboo version, operating system,
installation type, relevant logs, and clear reproduction steps. Do not post
passwords, API keys, private website data, or other confidential information.

## Package verification

When a release contains `SHA256SUMS.txt`, verify a downloaded file before
installing it.

PowerShell:

```powershell
Get-FileHash -Algorithm SHA256 -LiteralPath '.\Kooboo-package.zip'
```

Linux:

```bash
sha256sum ./Kooboo-package.zip
```

Compare the result with the corresponding entry in `SHA256SUMS.txt`.

## Licensing

Kooboo is proprietary software. Downloading or accessing a public release does
not grant access to its source code or a source-code license.

Use of Kooboo is governed by the license distributed with the product and the
`LICENSE` file in this repository. Third-party components remain subject to
their respective licenses and notices.

You may not assume permission to copy, modify, redistribute, decompile, or
reverse engineer Kooboo except where the applicable license or mandatory law
expressly permits it.

## Partner source access

Kooboo works with selected technology, hosting, infrastructure, integration,
and distribution partners. Approved partners may receive controlled source
access when it is required for an agreed project or commercial relationship.

Partner access is provided separately and may require a partnership agreement,
confidentiality obligations, access controls, and additional licensing terms.
It does not make Kooboo open source and does not grant a general right to
publish or redistribute the source code.

Organizations interested in becoming a Kooboo partner can contact Kooboo
through the [official website](https://www.kooboo.com/).

## Repository policy

This repository is maintained for:

- Official Kooboo release announcements
- Compiled installation packages
- Checksums and release notes
- Public issue reporting and release feedback

This is not a public source-code repository, and public source contributions
are not accepted here. Source collaboration with approved partners takes place
through separately authorized private channels. Please do not submit pull
requests containing reconstructed, copied, or proprietary Kooboo
implementation code.

---

Copyright © Kooboo. All rights reserved.
