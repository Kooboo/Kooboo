# Create a Site

> Create a site from **My Sites**: blank site, import package, or clone an existing site.

::: tip Prerequisite
Complete [Login and site list](./login-and-site-list.md) first. If you start site creation while you are inside a folder, the new site is added to that folder automatically.
:::

## Open the Creation Flow

1. From **My Sites**, click **Create Site**.
2. The URL changes to `/_Admin/create`.
3. The left side shows the form for the current creation mode, and the right side shows the list of alternative creation modes.

<DocImage src="/cms/getting-started/04-create-site-nav.png" alt="Create site screen" width="1120" />

## Creation Modes

| Mode | Route | Typical use |
|------|-------|-------------|
| **Blank Site** | `/_Admin/create` | Start from scratch with your own site name and default domain |
| **Clone Site** | `/_Admin/create/clone` | Copy an existing site or clone from a remote source |
| **Import Site** | `/_Admin/create/import` | Upload a Kooboo site package or a standard HTML package |
| **Pick Template** | Redirects to the template marketplace | Create from an existing template |

## Blank Site

Blank site creation is the default mode.

1. Enter the **Site Name**.
2. Enter the **Domain**: subdomain plus the platform-provided root domain.
3. Click **Start Creating**.
4. After creation finishes, return to **My Sites** and click **Manage** on the new site.

::: tip
Creating a blank site does not automatically open the site admin. Enter the site from the list after creation.
:::

## Import Site

<DocImage src="/cms/getting-started/import-site.png" alt="Import site flow" width="1120" />

1. Switch to **Import Site** to open `/_Admin/create/import`.
2. Upload a `.zip` package.
3. Depending on the package type, Kooboo imports the exported site or converts a standard HTML package into a Kooboo site.

## Clone Site

<DocImage src="/cms/getting-started/clone-site.png" alt="Clone site flow" width="1120" />

1. Switch to **Clone Site** to open `/_Admin/create/clone`.
2. Select the source site or enter the clone source required by your environment.
3. Confirm the form to create a new copy in **My Sites**.

## Recommended Next Steps

After you enter the site admin (`/_Admin/site?SiteId=...`), a practical order is:

1. [Site admin menu overview](../navigation.md)
2. [Content overview](../content/index.md)
3. [Template Engine](../../templateEngine/index.md)
4. [API Details](../../api/index.md)

## Quick Links

| Page | Route |
|------|-------|
| My Sites | `/_Admin/` |
| Create site | `/_Admin/create` |
| Import | `/_Admin/create/import` |
| Clone | `/_Admin/create/clone` |
| Site dashboard | `/_Admin/site?SiteId={siteGuid}` |
