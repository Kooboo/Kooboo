# Kooboo product documentation

> The Kooboo product documentation in plain markdown: the KScript API reference, the Kooboo
> Admin (CMS) guide, the template engine reference, and the quick-start tutorials.
>
> Chinese edition: [zh/](zh) · Machine-readable map of the whole repository: [llms.txt](../../llms.txt)

208 pages, each a standalone markdown file, so any single topic can be read, quoted or
indexed on its own. The list below is the complete table of contents.

## Kooboo Documentation Center — `index.md` (1 page)

Learn to build and manage Kooboo websites with Admin CMS tutorials, template engine guides, and the KScript API reference.

| Page | Summary |
| --- | --- |
| [Kooboo Documentation Center](index.md) | Learn to build and manage Kooboo websites with Admin CMS tutorials, template engine guides, and the KScript API reference. |

## API Details — `api/` (100 pages)

Complete reference for Kooboo server-side `k.*` APIs, including authentication, content, databases, commerce, and site resources.

| Page | Summary |
| --- | --- |
| [k.account](api/account/index.md) | Kooboo platform account: login status, current user, organization and third-party login |
| [k.account — Login and Identity](api/account/login.md) | `k.account` Login status, login/logout and token-related APIs on the root object |
| [k.account.oAuth](api/account/oauth.md) | Third-party login authorization URL and callback processing |
| [k.account.organization](api/account/organization.md) | The organization to which the current site belongs and the users and departments within the organization |
| [k.account.user](api/account/user.md) | Current platform user information and user query |
| [k.api](api/api/index.md) | Provides a series of methods to create APIs more conveniently |
| [k.cache](api/cache/index.md) | Site-wide in-memory cache, shared across requests within the same site |
| [address](api/commerce/address.md) | Address management |
| [cart](api/commerce/cart.md) | shopping cart |
| [category](api/commerce/category.md) | Product Category Management - Category CRUD and Search |
| [k.commerce](api/commerce/commerce.md) | E-commerce module main module |
| [currency](api/commerce/currency.md) | currency management |
| [customer](api/commerce/customer.md) | Customer management |
| [discount](api/commerce/discount.md) | Discounts |
| [loyalty](api/commerce/loyalty.md) | Member points |
| [membership](api/commerce/membership.md) | Membership level |
| [order](api/commerce/order.md) | Order |
| [product](api/commerce/product.md) | Product Management - Product CRUD and Search |
| [settings](api/commerce/settings.md) | E-commerce settings |
| [shipping](api/commerce/shipping.md) | Delivery method |
| [wishlist](api/commerce/wishlist.md) | wish list |
| [k.content](api/content/index.md) | Content management - CRUD and query APIs for site content |
| [k.cookie](api/cookie/index.md) | Cookie Management - Reading and Writing Client Cookies |
| [k.DB](api/database/index.md) | Database operations - unified CRUD interface supporting multiple databases |
| [k.file/folder](api/file/file-folder.md) | Folder operations |
| [k.file - data structure](api/file/file-info.md) | Data structure definition |
| [k.file/resumable](api/file/file-resumable.md) | Resume upload from breakpoint |
| [k.file](api/file/file.md) | File operations |
| [Authentication and Authorization](api/getting-started/auth.md) | APIs related to user login, session and access control within the site |
| [API Getting Started](api/getting-started/index.md) | Learn how to use KScript APIs inside Kooboo. |
| [k.google](api/google/index.md) | Google PageSpeed/Lighthouse and Firebase services |
| [API Details](api/index.md) | Complete reference for Kooboo server-side `k.*` APIs, including authentication, content, databases, commerce, and site resources. |
| [k.DB.indexedDb](api/indexed-db/index.md) | Site IndexedDB dynamic table - the same set of data as the background IndexedDB Tables |
| [k.DB.keyValue](api/key-value/index.md) | Site key-value store — same as backend Key-Value Storage |
| [k.site.labels](api/label/index.md) | Manage multilingual tags for your site |
| [k.logger](api/logger/index.md) | Logging and querying |
| [k.mail](api/mail/index.md) | Sending and receiving emails (IMAP), email parsing, Amazon SES and anti-spam detection |
| [k.media](api/media/index.md) | Folder and file management of site media library (pictures) |
| [k.module](api/module/index.md) | KScript API for script module (**Develop → Module**) runtime |
| [k.net.DNS](api/net/dns.md) | DNS lookups, reverse resolution, and Whois |
| [k.net.httpClient](api/net/httpclient.md) | Outbound HTTP: unified `send`, multiple request bodies, and batch tasks |
| [k.net](api/net/index.md) | HTTP, WebSocket, DNS, IP, and remote connections |
| [k.net.IP](api/net/ip.md) | Current requesting IP and IP geographic information |
| [k.net.ping](api/net/ping.md) | Probe host connectivity and round-trip time with ICMP |
| [k.net.sftpClient](api/net/sftpClient.md) | SFTP file upload and download |
| [k.net.sshClient](api/net/sshClient.md) | SSH command execution |
| [k.net.telnetClient](api/net/telnetClient.md) | Telnet sessions |
| [k.net.url](api/net/url.md) | Easy synchronous HTTP requests (string/object/binary) |
| [k.net.webSocket](api/net/websocket.md) | Upgrade current requests to WebSocket and manage intrasite connections |
| [k.openApi](api/openapi/index.md) | Call external HTTP API according to site OpenAPI definition and manage authentication credentials |
| [k.page](api/page/index.md) | The `<title>` and `<meta>` bindings of the page rendered by the current HTTP request |
| [k.paramConfig](api/param-config/index.md) | Read the structured JSON data saved in the background **Content → Parameter Configuration** |
| [alipay](api/payment/alipay.md) | Alipay payment. |
| [k.payment](api/payment/index.md) | Third-party payment integration. |
| [moneyboxes](api/payment/moneyboxs.md) | MoneyBoxes payments. |
| [paynl](api/payment/paynl.md) | Pay.NL Pay. |
| [paypal](api/payment/paypal.md) | PayPal payment. |
| [square](api/payment/square.md) | Square Payments. |
| [stripe](api/payment/stripe.md) | Stripe payments. |
| [two-checkout](api/payment/two-checkout.md) | TwoCheckout payment. |
| [WeChat H5 payment process](api/payment/wechat-h5-flow.md) | WeChat **H5** payment guide for AI and developers, covering the full flow from creating a payment request to callback verification. |
| [wechat](api/payment/wechat.md) | WeChat Pay. |
| [k.request](api/request/index.md) | HTTP request information - obtain various data requested by the client |
| [k.response](api/response/index.md) | HTTP response processing - set response content, status code, redirection, etc. |
| [k.security](api/security/index.md) | Security related tools - hashing, encryption, encoding, GUID, JWT, etc. |
| [k.session](api/session/index.md) | Session-level caching - stores session data on the server side |
| [k.site.codes](api/site/code.md) | Site code repository: API endpoint scripts and reusable CodeBlock additions, deletions, modifications and queries |
| [k.site.styles](api/site/css.md) | Addition, deletion, modification and query of site CSS style resources, routing URL and version log |
| [k.site](api/site/index.md) | Versioned site resources for the current site: pages, layouts, views, scripts, styles, codes, and menus. |
| [k.site.runJob](api/site/job.md) | Corresponding backend Develop -> Scheduled Jobs |
| [k.site.scripts](api/site/js.md) | Addition, deletion, modification, routing URL and version log of site JavaScript resources |
| [k.site.layouts](api/site/layout.md) | Create, read, update, delete, and inspect version logs for Layout resources |
| [k.site.menus](api/site/menu.md) | Reading and maintenance of the site's front-end navigation menu (the same set of data as the back-end **Development → Menu**) |
| [k.site.pages](api/site/page.md) | Site page addition, deletion, modification, routing URL and version log |
| [k.site.views](api/site/view.md) | Addition, deletion, modification and version log of reusable view (View) fragments |
| [SPA multi-language (front-end interface)](api/spa-multilingual/index.md) | Provides a JSON dictionary for single-page applications; entries are maintained at CMS: SPA Multilingual |
| [k.DB.sqlite](api/sqlite/index.md) | SQLite database operations - lightweight database that requires no configuration |
| [k.state](api/state/index.md) | View data (ViewData) in the current request rendering context |
| [k.utils.CDN](api/utils/cdn.md) | Current site CDN switch and cache time |
| [k.utils.clientJS](api/utils/clientJS.md) | Output client-side JavaScript variables to the page |
| [k.utils.community](api/utils/community.md) | Kooboo community resource sharing and search |
| [k.utils.compression](api/utils/compression.md) | Gzip decompression and ZIP packaging |
| [k.utils.converter](api/utils/converter.md) | Call Kooboo conversion services (HTML/PDF, Office, GIF, etc.) |
| [k.utils.CRM](api/utils/crm.md) | Kooboo CRM customer and order pulling |
| [k.utils.date](api/utils/date.md) | Date time and formatting |
| [k.utils.dom](api/utils/dom.md) | HTML parsing and image extraction |
| [k.utils.image](api/utils/image.md) | Image format, size and watermark |
| [k.utils](api/utils/index.md) | KScript Toolset: Helper API by submodule |
| [k.utils.inlineHtml](api/utils/inlineHtml.md) | Inline a page or URL resource as a data URI |
| [k.utils.mime](api/utils/mime.md) | Check file name and MIME type |
| [k.utils.ninjible](api/utils/ninjible.md) | Site import and Lighthouse comparison task (Ninjible) |
| [k.utils.office](api/utils/office.md) | Office document processing (Excel) |
| [k.utils.puppeteer](api/utils/puppeteer.md) | Headless browser screenshot (non-online environment only) |
| [k.utils.screenShot](api/utils/screenShot.md) | Web pages and page screenshots |
| [k.utils.sitemap](api/utils/sitemap.md) | Build XML Sitemap |
| [k.utils.sms](api/utils/sms.md) | SMS sending (multi-provider) |
| [k.utils.string](api/utils/string.md) | String and HTML text processing (`KText`) |
| [k.utils.template](api/utils/template.md) | View and page rendering |
| [k.utils.uri](api/utils/uri.md) | URL parsing and query strings |
| [k.utils.xml](api/utils/xml.md) | Parse and build XML documents |

## Kooboo Admin (CMS) — `cms/` (89 pages)

Configure sites, content, and integrations in the Kooboo console — for administrators and implementers

| Page | Summary |
| --- | --- |
| [Carts](cms/commerce/carts.md) | Menu: **Commerce -> Carts** |
| [Currencies](cms/commerce/currencies.md) | Menu: **Commerce -> Currencies** |
| [Customers](cms/commerce/customers.md) | Menu: **Commerce -> Customers** |
| [Discounts](cms/commerce/discounts.md) | Menu: **Commerce -> Discounts** |
| [Commerce](cms/commerce/index.md) | Menu: left sidebar **Commerce** group (must be enabled in **Edit menu**) |
| [Loyalty](cms/commerce/loyalty.md) | Menu: **Commerce -> Loyalty** |
| [Notification](cms/commerce/notification.md) | Menu: **Commerce -> Notification** |
| [Orders](cms/commerce/orders.md) | Menu: **Commerce -> Orders** |
| [Product Categories](cms/commerce/product-categories.md) | Menu: **Commerce -> Product Categories** |
| [Product Detail (Create and Edit)](cms/commerce/product-management-detail.md) | **Create**: `/_Admin/commerce/product-management/create?SiteId=...&typeId={typeId or blank}` |
| [Product Management (List)](cms/commerce/product-management.md) | Menu: **Commerce -> Product Management** |
| [Product Types](cms/commerce/product-types.md) | Menu: **Commerce -> Product Types** |
| [Sale Stats](cms/commerce/sale-stats.md) | Menu: **Commerce -> Sale Stats** |
| [Settings](cms/commerce/settings.md) | Menu: **Commerce -> Settings** |
| [Shipping](cms/commerce/shippings.md) | Menu: **Commerce -> Shipping** |
| [Tax](cms/commerce/taxes.md) | Menu: **Commerce -> Tax** |
| [Content Types](cms/content/content-types.md) | Menu: **Content → Content Types** |
| [Content Entries](cms/content/contents-entries.md) | **Folder**: `/_Admin/content/textContentsByFolder?SiteId=...&folder={contentFolderId}` |
| [Content Folder Settings](cms/content/contents-folder-settings.md) | Open from row **Settings** (gear) or the **New** wizard on the content folder list |
| [Content Folders](cms/content/contents-folders-list.md) | Menu: **Content -> Content** |
| [Files](cms/content/files.md) | Menu: **Content → Files** (must be enabled in **Edit Menu**; route `advanced: true`) |
| [HTML Blocks](cms/content/html-blocks.md) | Menu: **Content → HTML Blocks** |
| [Content](cms/content/index.md) | Core routes: `/_Admin/content/contentTypes?SiteId=...` and `/_Admin/content/contents?SiteId=...` |
| [Labels](cms/content/labels.md) | Menu: **Content → Labels** (admin UI may show “Labels”) |
| [Element Attributes](cms/content/tag-attributes.md) | Menu: **Content → Element attributes** (route `text`; must be enabled in **Edit Menu**) |
| [Parameter Config](cms/content/user-options.md) | Menu: **Content → Parameter config** (route `useroptions`; must be enabled in **Edit Menu**) |
| [Database](cms/database/index.md) | Menu: **Database** group in the left sidebar (must be enabled in **Edit Menu**) |
| [Key-Value Storage](cms/database/key-value.md) | Menu: **Database → Key-Value Storage** |
| [MySQL Tables](cms/database/mysql-table.md) | Menu: **Database → MySQL Tables** |
| [SQL Logs](cms/database/sql-logs.md) | Menu: **Database → SQL Logs** |
| [SQLite Tables](cms/database/sqlite-table.md) | Menu: **Database → SQLite Tables** |
| [SQL Server Tables](cms/database/sqlserver-table.md) | Menu: **Database → SQL Server Tables** |
| [IndexedDB Table Relations](cms/database/table-relation.md) | Menu: **Database → IndexedDB Table Relations** |
| [IndexedDB Tables](cms/database/table.md) | Menu: **Database → IndexedDB Tables** |
| [Authentication](cms/development/authentication.md) | Menu: **Development → Authentication** |
| [Code Log](cms/development/code-log.md) | Menu: **Development → Code Log** |
| [Code Search](cms/development/code-search.md) | Menu: **Development → Code Search** |
| [Code](cms/development/code.md) | Menu: **Development → Code** |
| [Forms](cms/development/forms.md) | Menu: **Development → Forms** |
| [Development](cms/development/index.md) | Menu: **Development** group in the left sidebar (must be enabled in **Edit Menu**) |
| [Jobs](cms/development/jobs.md) | Menu: **Development → Jobs** |
| [Layouts](cms/development/layouts.md) | Menu: **Development → Layouts** |
| [Menus](cms/development/menus.md) | Menu: **Development → Menus** |
| [Modules](cms/development/modules.md) | Menu: **Development → Modules** |
| [Open API](cms/development/openapis.md) | Menu: **Development → Open API** |
| [Scripts](cms/development/scripts.md) | Menu: **Development → Scripts** |
| [SPA Multilingual](cms/development/spa-multilingual.md) | Menu: **Development → SPA Multilingual** |
| [Styles](cms/development/styles.md) | Menu: **Development → Styles** |
| [URL](cms/development/urls.md) | Menu: **Development → URL** |
| [Views](cms/development/views.md) | Menu: **Development → Views** |
| [Create a Site](cms/getting-started/create-site.md) | Create a site from **My Sites**: blank site, import package, or clone an existing site. |
| [Login and Site List](cms/getting-started/login-and-site-list.md) | Enter Kooboo from the account-level console, organize sites with folders, and open a site's admin workspace. |
| [Kooboo Admin (CMS)](cms/index.md) | Configure sites, content, and integrations in the Kooboo console — for administrators and implementers |
| [Site Admin Menu Overview](cms/navigation.md) | Organized by the Kooboo site-admin left sidebar. UI labels follow the language of your logged-in admin. |
| [AB Testing](cms/operations/ab-testing.md) | Menu: **Operations Center → AB Testing** |
| [Operations Center](cms/operations/index.md) | Menu: **Operations Center** group in the left sidebar |
| [Page Interaction](cms/operations/page-interaction.md) | Menu: **Operations Center → Page Interaction** |
| [Resource Guardian](cms/operations/resource-guardian.md) | Menu: **Operations Center → Resource Guardian** |
| [Site Logs](cms/operations/site-logs.md) | Menu: **Operations Center -> Site Logs** |
| [Sync](cms/operations/sync.md) | Menu: **Operations Center → Sync** |
| [Visitor Logs](cms/operations/visitor-logs.md) | Menu: **Operations Center → Visitor Logs** |
| [Action Hooks](cms/settings/action-hooks.md) | Menu: **Settings -> Action Hooks** (advanced menu) |
| [AI Service](cms/settings/ai-service.md) | Menu: **Settings -> AI Service** (advanced menu) |
| [Basic · Access Control Tab](cms/settings/basic/access.md) | Tab: **Access Control** · `name=access` |
| [Basic · Advanced Tab](cms/settings/basic/advanced.md) | Tab: **Advanced** · `name=advance` |
| [Basic · Editor Tab](cms/settings/basic/editor.md) | Tab: **Editor** · `name=editor` |
| [Basic · General Tab](cms/settings/basic/general.md) | Tab: **General** · `name=basic` |
| [Basic Settings](cms/settings/basic/index.md) | Menu: **Settings -> Basic Settings** |
| [Basic · Performance Tab](cms/settings/basic/performance.md) | Tab: **Performance** · `name=performance` |
| [Basic · SEO Tab](cms/settings/basic/seo.md) | Tab: **SEO** · `name=seo` |
| [Cookie](cms/settings/cookie.md) | Menu: **Settings -> Cookie Management** |
| [Domains](cms/settings/domains.md) | Menu: **Settings -> Domains** |
| [Settings](cms/settings/index.md) | Menu: left sidebar **Settings** group (enable in **Edit Menu**) |
| [Integrations · Database](cms/settings/integrations/database.md) | Group: **Database** |
| [Integrations](cms/settings/integrations/index.md) | Menu: **Settings -> Integrations** |
| [Integrations · OAuth2](cms/settings/integrations/oauth2.md) | Group: **OAuth2** |
| [Integrations · Others](cms/settings/integrations/others.md) | Group: **Others** |
| [Integrations · Payment](cms/settings/integrations/payment.md) | Group: **Payment** |
| [Integrations · SMS](cms/settings/integrations/sms.md) | Group: **SMS** |
| [Integrations · Storage](cms/settings/integrations/storage.md) | Group: **Storage** |
| [Request Hooks](cms/settings/request-hooks.md) | Menu: **Settings -> Request Hooks** (advanced menu) |
| [Roles](cms/settings/roles.md) | Menu: **Settings -> Roles** (advanced menu) |
| [Site Users](cms/settings/site-users.md) | Menu: **Settings -> Site Users** |
| [Dashboard](cms/site/dashboard.md) | Menu: top-left **Home** after entering a site |
| [Media Library](cms/site/media.md) | Menu: **Media Library** (top of left sidebar, below the home icon) |
| [Edit Code](cms/site/pages-edit-code.md) | Open from Pages row **Edit Code** (design pages use **Design**) |
| [Inline Edit](cms/site/pages-inline-edit.md) | Open from Pages row **Inline Edit** |
| [Page Settings](cms/site/pages-setting.md) | Open from a row in Pages with **Settings** |
| [Pages](cms/site/pages.md) | Menu: top-left **Pages** |

## Quick Start — `quick-start/` (6 pages)

Take the shortest path through Kooboo: create a site, build a page, add content, write a small amount of server-side logic, and publish it.

| Page | Summary |
| --- | --- |
| [Add Content](quick-start/add-content.md) | Create a content model and content items so pages can retrieve managed data from Admin. |
| [Write Your First KScript API](quick-start/add-script-api.md) | Add a server-side endpoint to Kooboo so the frontend can retrieve JSON or execute business logic. |
| [Build Your First Page](quick-start/build-first-page.md) | Use the Pages feature in Admin to create an accessible URL. |
| [Create Your First Site](quick-start/create-first-site.md) | Go from signing in to opening the site administration page. |
| [Quick Start](quick-start/index.md) | Take the shortest path through Kooboo: create a site, build a page, add content, write a small amount of server-side logic, and publish it. |
| [Publish and Domains](quick-start/publish-domain.md) | Confirm that the site can be reached externally and complete the basic go-live configuration. |

## Template Engine — `templateEngine/` (12 pages)

How Kooboo organizes and renders pages, layouts, views, and routed static assets.

| Page | Summary |
| --- | --- |
| [Template Binding Syntax](templateEngine/binding/index.md) | Server-side `k-*` directives for binding data and attributes inside Layout, Page, and View templates. |
| [Style](templateEngine/css/index.md) | Routed `.css` resources for global themes, patches, and Layout `<head>` references. |
| [Template Engine](templateEngine/index.md) | How Kooboo organizes and renders pages, layouts, views, and routed static assets. |
| [Script](templateEngine/js/index.md) | Routed `.js` resources for shared utilities, ES Modules, and cooperation with `<script>` tags inside Pages. |
| [JSON5 Condition Expressions](templateEngine/k-data/condition.md) | Used by `<query where="...">` and `<when test="...">`. |
| [k-data Built-in Functions](templateEngine/k-data/functions.md) | Functions available through `<let use="functionName" params="...">`. |
| [k-data](templateEngine/k-data/index.md) | Declare data flow with HTML tags inside a template. This is the recommended approach and replaces deprecated `k-query`. |
| [Query Sources](templateEngine/k-data/query.md) | How `<query>` combines `source`, `resource`, and `action`. |
| [k-data Tag Syntax](templateEngine/k-data/tags.md) | Attributes and composition patterns for the tags used inside `<k-data>`. |
| [Layout](templateEngine/layout/index.md) | Site-level HTML skeleton: shared `<head>`, global assets, and `k-placeholder` regions. |
| [Page](templateEngine/page/index.md) | An HTML page with a route: either a full standalone document or a Layout-based page with injected placeholders. |
| [View](templateEngine/view/index.md) | Reusable HTML fragments embedded into a Page, Layout, or another View with `<view id="...">` |
