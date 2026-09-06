# Kooboo

### High performance. Native AI. A marketplace for complete websites and applications.

Kooboo is an integrated platform for building, running, and owning dynamic
websites, online stores, and web applications. Pages, content, databases,
commerce, APIs, analytics, hosting, development tools, AI, and deployment work
together in one portable system.

**Build a real website in minutes—and keep control of the result.**

[Download Kooboo](https://www.kooboo.com/downloads) ·
[Read the benchmark](Docs/5000-sites-benchmark.md) ·
[Visit Kooboo](https://www.kooboo.com/) ·
[Report an issue](https://github.com/Kooboo/Kooboo/issues)

> ** 5,000 dynamic websites on a 2-vCPU, 4-GB server.** We published
> the complete ten-minute pressure test: 90,000 HTTPS requests, 89,969 verified
> responses, 99.97% first-attempt success, server monitoring, request-level
> records, source code, and reproduction packages.
> **[Read the benchmark and inspect the evidence](Docs/5000-sites-benchmark.md)**

> **Free to use. Not open source.** Anyone can download Kooboo and use it
> without a time limit. Public releases contain compiled packages. Controlled
> source access is available separately to approved partners.

## Why Kooboo

| High performance | Native AI | Marketplace |
| --- | --- | --- |
| One optimized runtime from domain routing to dynamic rendering and network output | AI works with the website's real objects, runtime, data, and history | Install, modify, publish, and operate portable website and application packages |
| Reproducible 5,000-site benchmark with raw records | Build, run, inspect, repair, validate, and publish in one working loop | Templates, applications, and services can be free or separately licensed by their publishers |
| Designed for efficient multi-site operation on modest hardware | The finished website remains editable, portable, and controlled by its owner | Marketplace packages run inside the same integrated Kooboo environment |

## 1. High performance by architecture—and by measurement

Kooboo is designed as one integrated performance path. Domain resolution,
site selection, routing, data access, server-side rendering, caching, and
response delivery are handled by one coordinated runtime rather than a chain
of separately deployed services.

Websites share the optimized Kooboo platform without becoming one shared
website. Each site retains its own identity, domain, pages, layouts, views,
content, files, configuration, permissions, analytics, and history.

### Published 5,000-site pressure test

One Kooboo process hosted **5,000 independently addressable and independently
editable dynamic websites** on a Tencent Cloud server with **2 vCPUs and 4 GB
of memory**.

| Measured result | Value |
| --- | ---: |
| Independent dynamic websites | 5,000 |
| Complete HTML requests attempted | 90,000 |
| Correct first-attempt responses | 89,969 — 99.97% |
| Measured request start rate | 146.59 requests/second |
| Measured completion rate | 146.52 requests/second |
| Complete-content p50 | 296.73 ms |
| Complete-content p95 | 1,431.27 ms |
| Complete-content p99 | 3,393.82 ms |
| Average Kooboo CPU | 45.6% of total two-vCPU capacity |
| Sampled peak Kooboo CPU | 81.5% of total two-vCPU capacity |
| Sampled peak Kooboo RSS | Approximately 2.50 GiB |

This was an end-to-end HTTPS pressure test across the public Internet. The
client downloaded every complete HTML response and verified the expected site
number in its body. The report publishes the hardware, workload, latency
distribution, failures, CPU and memory records, request-level CSV, limitations,
test client, and reproduction packages.

**[Read the complete benchmark and download its raw records](Docs/5000-sites-benchmark.md)**

![Kooboo dashboard containing a folder of 5,000 independently editable websites](Docs/5000-sites-benchmark/kooboo-5000-sites-folder.png)

> **Do not benchmark the brochure. Download Kooboo and benchmark the product.**

## 2. Native AI that works inside the platform

Kooboo Native AI is not a chat box attached to an unrelated website builder.
It works with the architecture and operating environment of the website:
Pages, Layouts, Views, content, databases, commerce, APIs, server-side code,
media, configuration, analytics, and history.

This gives AI a complete working loop:

**Build → Run → Observe → Diagnose → Modify → Validate → Publish → Measure → Improve**

Kooboo Native AI can:

- Build a real dynamic website rather than only generating a static frontend.
- Work with structured site objects, content models, database data, APIs, and
  application logic.
- Open and inspect the running result instead of stopping after code generation.
- Detect runtime errors and abnormal requests, then continue repairing the site.
- Connect a selected element on the rendered page to the Page, View, or Layout
  that produced it.
- Work in an isolated Kooboo sandbox before approved changes reach production.
- Use website history to inspect, compare, undo, and recover supported changes.
- Run scheduled checks and longer-running operational tasks.
- Work with supported AI providers and models selected by the website owner.

![Kooboo Native AI workspace](https://www.kooboo.com/whykooboo/AI-kooboo-agent.png)



**Built by AI. Tested by AI. Run by AI. Owned by you.**

## 3. Marketplace: install working systems, not screenshots

Kooboo websites and applications are portable packages. A package can contain
the working structure of a site—including Pages, Layouts, Views, content,
scripts, styles, routes, images, and configuration—not merely a visual theme.

The Kooboo Marketplace builds on that portability:

- Discover website templates, applications, and connected services.
- Install a package directly into a Kooboo environment.
- Inspect and edit the installed result using the same Kooboo development tools.
- Continue operating the website on your own Kooboo instance.
- Package and transfer the completed website when deployment requirements change.
- Share and update packages with names, screenshots, metadata, and publisher
  information.
- Support free offerings and separately licensed or paid publisher offerings.

The Marketplace is intended to shorten the distance between finding a useful
starting point and owning a working, editable system. Native AI can then help
adapt the installed website or application to the owner's actual content,
design, data, and operational requirements.

Marketplace items remain subject to the terms supplied by their respective
publishers. Availability in the Marketplace does not change the license of the
Kooboo platform or grant access to Kooboo's non-public implementation source.

## What else is built in

- Visual website building and inline content editing
- Direct HTML, CSS, JavaScript, and server-side development
- Content management and multilingual content
- Dynamic database-driven pages and queries
- Products, customers, carts, orders, discounts, and online stores
- Forms, APIs, scheduled jobs, and custom business applications
- Website importing, cloning, packaging, transfer, and deployment
- Domain routing, hosting, and email services
- Server-side traffic analytics, diagnostics, and error inspection
- Media management and image processing
- Users, roles, permissions, configuration, and operational history
- Version comparison, rollback, restoration, and website checkout

## Download and start

Download the newest official build from the
[latest Kooboo release](https://www.kooboo.com/downloads).

1. Download the package for your operating system.
2. Verify its SHA-256 checksum when one is supplied.
3. Extract the portable package or run the Windows installer.
4. Start Kooboo and open the local address shown by the application.
5. Create a website, import an existing site, install a Marketplace package, or
   begin with Native AI.

Before upgrading a production installation, back up the Kooboo data and read
the release notes.

## Documentation and support

- [Kooboo website](https://www.kooboo.com/)
- [Downloads](https://www.kooboo.com/downloads)
- [Documentation](https://docs.kooboo.com)
- [GitHub issues](https://github.com/Kooboo/Kooboo/issues)

When reporting a problem, include the Kooboo version, operating system,
installation type, relevant logs, and reproduction steps. Never publish
passwords, API keys, private website data, or other confidential information.

## Licensing and source access

Kooboo is free to download and use without a time limit. Downloading a public
release grants use under the applicable Kooboo end-user license, but it does
not grant access to the Kooboo implementation source or an open-source license.

Third-party and Marketplace items remain subject to their respective licenses
and notices. Approved technology, hosting, infrastructure, integration, and
distribution partners may receive controlled source access under separate
written agreements and access controls.

This repository is maintained for official compiled releases, checksums,
release notes, issue reporting, and release feedback. It is not a public
source-code repository.

---

Copyright © Kooboo. All rights reserved.
