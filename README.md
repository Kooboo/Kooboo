
## The AI-native Web OS for building, running, and owning websites and business applications.

Kooboo is one integrated platform for creating websites, online stores, and business applications.

AI website creation, visual editing, content management, databases, ecommerce, email, analytics, hosting, development, and deployment all work together in one portable system.

**Build a real website in minutes. Edit every part. Run it anywhere. Keep control of the result.**

[Download the latest release](https://github.com/Kooboo/Kooboo/releases/latest) ·
[Start with Kooboo Cloud](https://www.kooboo.com/) ·
[Documentation](https://docs.kooboo.com) ·
[Report an issue](https://github.com/Kooboo/Kooboo/issues)

![Kooboo overview](Docs/assets/kooboo-overview.png)

> **Free to use. Not open source.**  
> Anyone can download and use Kooboo without a time limit. Public releases contain compiled packages. Controlled source access is available separately to approved partners.

# Getting started

You can run Kooboo on your own computer or server, or use the hosted Kooboo Cloud service.

## Run Kooboo locally

1. Open the [latest Kooboo release](https://github.com/Kooboo/Kooboo/releases/latest).
2. Find the package for Windows, macOS, or Linux.
3. Download and extract the package if required.
4. Start Kooboo:
   - **Windows:** double-click the Kooboo application.
   - **macOS:** open the Kooboo application from the downloaded package.
   - **Linux:** run the Kooboo executable from the terminal or as a service
5. Open the local address displayed by Kooboo in your browser.

For a production Linux server, Kooboo can be configured as a system service so that it starts automatically and restarts after a server reboot.
 
## Use Kooboo Cloud for free

If you do not want to install anything, you can use the hosted Kooboo Cloud service.

[Start with Kooboo Cloud](https://www.kooboo.com/)

Create an account, create a website, and begin building directly in your browser.

![Kooboo cloud IDE](Docs/assets/kooboo-instant-start.png)

# Development

Kooboo supports different development styles—from AI-assisted creation to declarative HTML and full server-side scripting.

You can choose the simplest method for each task and combine all three inside the same website.

## Hello, world

After starting Kooboo:

1. Create a new website.
2. Open **Pages**.
3. Create a new page.
4. Add the following HTML:

```html
<h1>Hello, world!</h1>
<p>This page is running in Kooboo.</p>
```

5. Save the page and open its URL.

You now have a working Kooboo website.

## 2. Build with Native AI

Kooboo Native AI works directly with the website’s Pages, Layouts, Views, content, databases, APIs, scripts, media, configuration, and history.

It can build a website, run it, inspect the result, diagnose problems, make changes, and validate the finished site.

**Build → Run → Observe → Diagnose → Modify → Validate → Publish → Improve**

![Building a website with Kooboo Native AI](Docs/assets/kooboo-ai.png)

[Learn more about Kooboo Native AI](https://www.kooboo.com/articles/native-ai)

**Built by AI. Tested by AI. Run by AI. Owned by you.**


## 2. Build fully dynamic websites using HTML

Kooboo can query structured content and render dynamic pages directly from an HTML template.

The following example loads every item from `BlogFolder` and displays its title and summary:

```html
<k-data>
  <query
    as="blogs"
    source="content"
    resource="BlogFolder"
    action="list"
    export>
  </query>
</k-data>

<div k-for="blog in blogs">
  <h2 k-content="blog.title"></h2>
  <p k-content="blog.summary"></p>
</div>
```

There is no separate controller or external template engine to configure. The data query, loop, and content binding are declared directly in the page.
 

## 3. Use JavaScript with KScript

KScript is standard JavaScript with the additional `k.*` namespace for accessing Kooboo functions.

You can use normal JavaScript syntax, libraries, objects, arrays, and programming patterns. Through the `k.*` namespace, your code can work directly with Kooboo content, databases, APIs, users, requests, configuration, email, and other platform services.

This example loads blog content using server-side JavaScript:

```html
<script env="server">
var blogs = k.content.BlogFolder.all();
</script>

<div k-for="blog in blogs">
  <h2 k-content="blog.title"></h2>
  <p k-content="blog.summary"></p>
</div>
 
```
JavaScript development is included in the free version of Kooboo cloud. TypeScript development is available in paid Plans.
 

# Key features

## Native AI

Kooboo Native AI works inside the website platform rather than operating as a separate chat box.

It can work with Pages, Layouts, Views, content, databases, APIs, ecommerce, scripts, media, configuration, analytics, and website history.

The AI can build the website, run it, inspect the result, diagnose problems, make changes, validate those changes, and continue improving the site.

**Build → Run → Observe → Diagnose → Modify → Validate → Publish → Improve**
 

[Learn more about Kooboo Native AI](https://www.kooboo.com/articles/native-ai)

**Built by AI. Tested by AI. Run by AI. Owned by you.**
 

## High performance

Kooboo uses one integrated runtime for domain resolution, routing, data access, server-side rendering, caching, and network delivery.

There is no need to connect and operate a large collection of separate services just to run a dynamic website.

In a published public-Internet pressure test, one Kooboo process hosted:

- **5,000** independently addressable dynamic websites
- **90,000** complete HTTPS requests
- **89,969** correct first-attempt responses
- **99.97%** first-attempt success
- A server with only **2 vCPUs and 4 GB of memory**

[Read the complete 5,000-site benchmark](Docs/5000-sites-benchmark.md)


The benchmark report includes the hardware, workload, latency distribution, failures, CPU and memory records, request-level data, test client, and reproduction packages.

> Do not benchmark the brochure. Download Kooboo and benchmark the product.

## Portal and Marketplace

The Kooboo portal gives you one place to create, import, organize, edit, and operate websites and applications.

The Kooboo Marketplace provides complete website and application packages—not only visual themes.

A package can include:

- Pages, Layouts, and Views
- Content and data
- HTML, CSS, and JavaScript
- Images and other media
- Routes and configuration
- APIs and application logic
- Ecommerce and business features

Install a package, inspect how it works, customize every part, and continue running it in your own Kooboo environment.

![Kooboo portal and Marketplace](Docs/assets/kooboo-marketplace.png)
 
[Learn more about the Marketplace](https://www.kooboo.com/articles/marketplace)

## Everything included

Kooboo combines the tools required to build and operate a complete website or application.

| | | |
| --- | --- | --- |
| **[Native AI](https://www.kooboo.com/articles/native-ai)**<br>Build, inspect, test, repair, and operate real websites with AI. | **[Performance](https://www.kooboo.com/articles/performance)**<br>Run dynamic websites through one optimized, integrated runtime. | **[Development](https://www.kooboo.com/articles/development)**<br>Build frontend, server-side logic, APIs, and data-driven applications. |
| **[Inline editor](https://www.kooboo.com/articles/inline-editor)**<br>Click an element on the website and edit its content, layout, view, or style. | **[Marketplace](https://www.kooboo.com/articles/marketplace)**<br>Install complete, editable websites, applications, and tools. | **[Ecommerce](https://www.kooboo.com/articles/ecommerce)**<br>Manage products, customers, carts, orders, discounts, email, and analytics. |
| **[Multilingual](https://www.kooboo.com/articles/multilingual)**<br>Manage translated content, routes, links, and SEO in one website. | **[Relation Intelligence](https://www.kooboo.com/articles/relation-intelligence)**<br>See how pages, code, content, routes, and resources are connected. | **[Email Marketing](https://www.kooboo.com/articles/email-marketing)**<br>Create personalized campaigns using your website’s own data and logic. |
| **[History and Deployment](https://www.kooboo.com/articles/history-deployment)**<br>Compare, undo, restore, package, transfer, and publish changes. | **[Domain and Email](https://www.kooboo.com/articles/domain-email)**<br>Connect domains, HTTPS, DNS, hosting, and email in one environment. | **[Server-Side Analytics](https://www.kooboo.com/articles/server-side-analytics)**<br>Understand visitors, journeys, bots, resources, performance, and errors. |

## Additional capabilities

Kooboo also includes:

- Visual website building
- Inline content and style editing
- Direct HTML, CSS, and JavaScript development
- Server-side scripting
- Content management
- Dynamic database queries
- Forms and APIs
- Scheduled jobs and automation
- Media and image management
- Website importing and cloning
- Website packaging and transfer
- Users, roles, and permissions
- Version comparison and rollback
- Error and request diagnostics
- Website traffic analytics
- Domain routing and hosting
- Integrated email services
- Online stores and payment integrations
- Custom business applications

## Portable by design

A Kooboo website is not locked inside a remote visual editor.

Pages, code, content, data, media, configuration, and operational history remain part of a portable website project.

You can:

- Develop locally
- Run Kooboo on your own server
- Use Kooboo Cloud
- Move a website between Kooboo installations
- Package and share complete websites
- Inspect and edit installed Marketplace packages
- Back up and restore your website
- Keep control of the finished result

## Download and support

- [Latest GitHub release](https://github.com/Kooboo/Kooboo/releases/latest)
- [Kooboo website](https://www.kooboo.com/) 
- [Documentation](https://docs.kooboo.com)
- [GitHub issues](https://github.com/Kooboo/Kooboo/issues)
- [5,000-site benchmark](Docs/5000-sites-benchmark.md)

When reporting a problem, include:

- Your Kooboo version
- Operating system
- Installation type
- Relevant logs
- Steps to reproduce the problem

Never publish passwords, API keys, private website data, or other confidential information.

## Licensing and source access

Kooboo is free to download and use without a time limit.

Downloading a public release grants use under the applicable Kooboo end-user license. It does not grant access to the Kooboo implementation source or an open-source license.

Third-party and Marketplace items remain subject to their respective licenses and notices.

Approved technology, hosting, infrastructure, integration, and distribution partners may receive controlled source access under separate written agreements and access controls.

This repository is maintained for official compiled releases, checksums, release notes, issue reporting, and release feedback. It is not a public source-code repository.

---

Copyright © Kooboo. All rights reserved.
