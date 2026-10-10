# Development

> Menu: **Development** group in the left sidebar (must be enabled in **Edit Menu**)  
> Route prefix: `/_Admin/development/...?SiteId={siteGuid}`

The **Development** group centralizes **templates and site-level resources** for the site: View, Layout, Script, Style, Code, plus URL routing, forms, admin menus, authentication, Open API, SPA multilingual, modules, scheduled jobs, and more. Together with [Pages](../site/pages.md) (page tree and visual editing), it forms the frontend implementation layer of a Kooboo site.

::: tip Template engine docs
This series explains **where to click and what to fill in** in the admin; syntax and APIs are in the [Template Engine](../../templateEngine/index.md) (Layout, View, Script, Style, etc.) and [KScript API](../../api/index.md). The two map one-to-one via object types in the menu.
:::

::: info Advanced menu
The Development group and most of its items are marked **advanced** in Edit Menu. If unchecked, this group does not appear in the left sidebar.
:::

## How to Open

1. In [Edit Menu](../navigation.md#edit-menu), enable **Development** and the sub-items you need.
2. Expand **Development** in the left sidebar.

## Sub-menus (same order as the admin)

| Menu | Path | Permission (feature) | Doc |
|------|------|----------------------|-----|
| Views | `/development/views` | `view` | [Views](./views.md) |
| Layouts | `/development/layouts` | `layout` | [Layouts](./layouts.md) |
| Scripts | `/development/scripts` | `script` | [Scripts](./scripts.md) |
| Styles | `/development/styles` | `style` | [Styles](./styles.md) |
| Code | `/development/code` | `code` | [Code](./code.md) |
| Code Log | `/development/code-log` | `code·log` | [Code Log](./code-log.md) |
| Code Search | `/development/code-search` | `code·view` | [Code Search](./code-search.md) |
| URL | `/development/urls` | `link` | [URL](./urls.md) |
| Forms | `/development/forms` | `form` | [Forms](./forms.md) |
| Menus | `/development/menus` | `menu` | [Menus](./menus.md) |
| Authentication | `/development/authentication` | `authentication` | [Authentication](./authentication.md) |
| Open API | `/development/openapis` | `openApi` | [Open API](./openapis.md) |
| SPA Multilingual | `/development/spamultilingual` | `spaMultilingual` | [SPA Multilingual](./spa-multilingual.md) |
| Modules | `/development/modules` | `module` | [Modules](./modules.md) |
| Jobs | `/development/jobs` | `job` | [Jobs](./jobs.md) |

### Development reference (template engine / API)

| Menu | Development docs |
|------|------------------|
| Views | [View](../../templateEngine/view/index.md) · `k.site.views` |
| Layouts | [Layout](../../templateEngine/layout/index.md) |
| Scripts | [Script](../../templateEngine/js/index.md) |
| Styles | [Style](../../templateEngine/css/index.md) |
| Code | `k.site.codes` (API) |
| Menus | [k.site.menus](../../api/site/menu.md) |
| Authentication | [k.security.jwt](../../api/security/index.md) · [k.response](../../api/response/index.md) |
| Open API | [k.openApi](../../api/openapi/index.md) |
| SPA Multilingual | [SPA Multilingual API](../../api/spa-multilingual/index.md) |
| Modules | [k.module](../../api/module/index.md) |
| Jobs | [k.site.runJob](../../api/site/job.md) |

## Edit routes without sidebar menu entries

The following pages are opened from list **Edit** actions; breadcrumbs still belong to the corresponding menu item:

| Route | Description |
|------|------|
| `/development/layout/edit` | Edit Layout |
| `/development/view/edit` | Edit View |
| `/development/script/edit` | Edit Script |
| `/development/style/edit` | Edit Style |
| `/development/code/edit` | Edit Code |
| `/development/form/edit` | Edit form |
| `/development/menu/edit` | Edit admin menu item |
| `/development/openapi/edit` | Edit Open API definition |
| `/development/form-values` | Form submission data (`activeMenu: forms`) |
| `/development/openapi/authorizes` | Open API authorization records |
| `/development/module-menu/:module` | Enter installed module sub-menu |

## Relationship to the Pages module

| Capability | Entry | Doc |
|------|------|------|
| Page tree, publish, rich text / designer | **Site → Pages** | [Pages](../site/pages.md) |
| Edit HTML online, bind View/Layout | Page list **Inline Edit** | [Inline Edit](../site/pages-inline-edit.md) |
| Edit page code files directly | Page **Edit Code** | [Edit Code](../site/pages-edit-code.md) |
| Site-level View/Layout library | **Development → Views / Layouts** | [Layouts](./layouts.md) · [Views](./views.md) |

Page designer routes (such as `/_Admin/page/edit`, `/_Admin/page/design`) are entered from the **Pages** list, not under the Development group.

## Suggested reading order

When using development capabilities from a site-building perspective, a common order is:

1. **Layouts** — page shell and shared structure  
2. **Views** — reusable fragments referenced by pages or Layouts  
3. **Scripts / Styles** — site-level JS/CSS resources  
4. **URLs** — custom routing and bindings  
5. **Forms / Menus / Authentication** — forms, admin menus, and login methods (as needed)  
6. **Open API / SPA Multilingual / Modules / Jobs** — integration and extensions  

See each sub-page for specific steps.

## Related

| Doc | Description |
|------|------|
| [Site admin menu overview](../navigation.md) | Full menu tree |
| [Pages](../site/pages.md) | Pages and visual editing |
| [Operations](../operations/index.md) | Runtime observability (separate from development implementation) |
| [Template Engine](../../templateEngine/index.md) | Template syntax and concepts |
| [KScript API](../../api/index.md) | Script APIs |
