# Create Your First Site

> Go from signing in to opening the site administration page.

## Sign In and Confirm the Entry Point

| Item | Details |
|------|---------|
| Admin entry | `/_Admin/` |
| Legacy path compatibility | `/_Admin/Sites` returns to the site list |
| Main pages | Site list, folders, and site creation |

After signing in, you enter the site list. This is an account-level page and does not contain a `SiteId`. If you already have a site, select **Manage** on its card to enter site Admin.

## Create a Site

Admin provides three common methods:

| Method | Deep link | Recommended use |
|--------|-----------|-----------------|
| Blank site | `/_Admin/create` | Start with the smallest site structure |
| Import site | `/_Admin/create/import` | Import an existing site package, ZIP file, or migration source |
| Clone site | `/_Admin/create/clone` | Copy an existing site as the starting point for a new project |

If template-based site creation is available in your environment, select a template from the site creation flow and review its name, domain, and source before creating the site.

## Enter Site Admin for the First Time

After entering site Admin, the URL normally contains the site ID:

```text
/_Admin/site/dashboard?SiteId={site-guid}
```

Site-level deep links in the CMS documentation use the same format. Replace `{site-guid}` with the `SiteId` from the address bar of your current site.

## Recommended Initial Checks

| Check | Location |
|-------|----------|
| Site name, default language, and Base URL | [Basic Settings: General](../cms/settings/basic/general.md) |
| Whether the site needs multiple languages | [Basic Settings: General](../cms/settings/basic/general.md) and [SPA Multilingual](../cms/development/spa-multilingual.md) |
| Whether the site needs team collaboration | [Site Users](../cms/settings/site-users.md) and [Roles](../cms/settings/roles.md) |
| Whether the site needs AI capabilities | [AI Service](../cms/settings/ai-service.md) |

## Next Step

Continue to [Build Your First Page](./build-first-page.md).
