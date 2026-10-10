# k.module

> KScript API for script module (**Develop → Module**) runtime

## Overview

**A module** is an extension package with its own directories and routes (`view/`, `api/`, `code/`, `module.config`, and so on). It is mounted under `/_moduleName/{part}` inside the site. `k.module` exposes the current module context, configuration, scheduled tasks, local files and databases, plus module package management such as import, export, and repository install.

::: tip Configure the module first
Create or import the module in [Modules (CMS)](../../cms/development/modules.md), edit its files in development mode, and save the runtime parameters defined by `module.config` in **Settings**.
:::

::: warning Execution context
Many members such as `baseUrl`, `config`, `name`, `localFile`, and `task` are available only when a **module script** is executing (module view/api/code, or an entry loaded with `module:` / `ModuleApi:`). Accessing them from normal site Code or API routes throws `part of k.module only available under Module execution context`.

Site-level listing, creation, deletion, etc. can be called from any script with permissions (such as `k.module.list()`).
:::

::: info Difference from `k.site`
| API | Scope |
|-----|--------|
| `k.site.*` | Current site pages, layout, Code and other resources |
| `k.module.*` | Current **module package** directory, configuration and module routing |

Reference other scripts within the module:

```ts
import { helper } from "module:myModule/code/utils"
// module API entry
import api from "ModuleApi:myModule/api/hello"
```

Module paths are resolved from directories such as `code/` and `api/` inside the module.
:::

## Common Members Inside a Module

### baseUrl

The external root URL of the current module (including module routing prefix), used for linking or redirection.

```ts
// in module view/api scripts
const home = k.module.baseUrl + "index"
```

### config

Reads the backend **Settings** saved key (`settingDefines` + site `Settings` JSON from `module.config`). May be an empty object when not configured.

```ts
const title = k.module.config?.text
```

### name

The name of the currently executing module.

### task

Module scheduled-job chain API, typically written in `event.js` or a task script:

```ts
k.module.task
  .day(1)
  .hour(0)
  .run(function () {
    // scheduled logic
  })
```

`minute(n)` can also be used. The scheduler executes the supplied function according to the chained plan.

### localFile/openFileStream

Access files in the module disk directory (divided into folders by type):

| Member | Table of contents |
|------|------|
| `k.module.localFile.view` | `view/` |
| `k.module.localFile.api` | `api/` |
| `k.module.localFile.js` | `js/` |
| `k.module.localFile.css` | `css/` |
| `k.module.localFile.img` | `img/` |
| `k.module.localFile.file` | `file/` |

Or use `k.module.openFileStream("api")` etc. (`fileType` is one of `css`, `js`, `view`, `api`, `img`, `file`).

`KModuleFiles` provides file operations such as reading, writing, and enumeration. Use site IDE completion for the exact method list.

### localDatabase/localSqlite

| Member | Description |
|------|------|
| `k.module.localDatabase` / `localIndexedDb` | IndexedDB style native library (`KModuleDatabase`) in the module directory |
| `k.module.localSqlite` | SQLite access to module root `_sqlite.db` |

Suitable for module private data, separate from site `k.DB`.

## Site-Level Management APIs

The following methods can be called in **Site Code/API** (requires corresponding site permissions) for automated installation and operation and maintenance.

### list()

Returns an array of all `ScriptModule` objects in the site.

**Parameters**: none.

**Returns**: `ScriptModule[]`. All modules in the current site.

```ts
k.api.get(() => {
  return k.module.list().map((m) => ({ id: m.id, name: m.name, online: m.online }))
})
```

### createModule(name)

Create an empty module and register the route, returning `ScriptModule`.

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `name` | `string` | Yes | New module name. |

**Returns**: `ScriptModule`. Created module object.

```ts
k.api.post(() => {
  const module = k.module.createModule(k.request.form.name)
  return { id: module.id, name: module.name }
})
```

### remove(idOrName) / remove(idOrName, destinationSiteUrl)

Delete module; two-parameter overload can specify the target site domain name to delete the module with the same name on the remote site.

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `idOrName` | `string` | Yes | Module ID or module name. |
| `destinationSiteUrl` | `string` | No | Target site domain. Omit it to remove the module from the current site. |

**Returns**: `void`.

```ts
k.api.post(() => {
  k.module.remove(k.request.form.name)
  return { ok: true }
})
```

### isNameExists(name)

Check whether the module name is already occupied.

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `name` | `string` | Yes | Module name to check. |

**Returns**: `boolean`. Returns `true` when the name already exists.

```ts
k.api.get(() => {
  return { exists: k.module.isNameExists(k.request.queryString.name) }
})
```

### importZip(name, binary)

Creates a module from a zip binary and extracts it to the modules directory, returning the new module Id string.

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `name` | `string` | Yes | Imported module name. |
| `binary` | `number[]` | Yes | Zip file bytes. |

**Returns**: `string`. New module ID.

```ts
k.api.post(() => {
  const file = k.request.files[0]
  const id = k.module.importZip(k.request.form.name, file.bytes)
  return { id }
})
```

### exportAsZip(nameOrId)

Export the module as a zip byte array.

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `nameOrId` | `string` | Yes | Module name or module ID. |

**Returns**: `number[]`. Module zip package bytes.

```ts
k.api.get(() => {
  const bytes = k.module.exportAsZip(k.request.queryString.name)
  return { size: bytes.length }
})
```

### importFromUrl(siteUrl, moduleId, newModuleName[, destinationSiteUrl])

Pull the zip from the shared URL (`/_api/PublicModuleFiles/PrivateShare?ModuleId=...`) of other Kooboo sites and install it.

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `siteUrl` | `string` | Yes | Source site URL. |
| `moduleId` | `string` | Yes | Source module ID. |
| `newModuleName` | `string` | Yes | Module name after installation. |
| `destinationSiteUrl` | `string` | No | Target site URL. Omit it to install into the current site. |

**Returns**: `string`. Installed module ID.

```ts
k.api.post(() => {
  const id = k.module.importFromUrl(
    k.request.form.siteUrl,
    k.request.form.moduleId,
    k.request.form.name
  )
  return { id }
})
```

### searchRepository(keyword) / installFromRepository(packageId, name)

Search the Kooboo app store module package, download and install it (same origin as the **Search** button in the background).

| Method | Parameters | Returns | Description |
|--------|------------|---------|-------------|
| `searchRepository(keyword)` | `keyword: string` | `ModuleSearchResult[]` | Search module packages by keyword. |
| `installFromRepository(packageId, name)` | `packageId: string`, `name: string` | `string` | Install the package and return the module ID. |

```ts
k.api.post(() => {
  const results = k.module.searchRepository(k.request.form.keyword)
  if (!results.length) return { installed: false }
  const id = k.module.installFromRepository(results[0].id, k.request.form.name)
  return { installed: true, id }
})
```

## TypeScript Definition

```ts
interface KModule {
  baseUrl: string
  config: any
  name: string
  task: KTask
  localFile: LocalFile
  localDatabase: IDatabase
  localIndexedDb: IDatabase
  localSqlite: SqliteDatabase
  list(): ScriptModule[]
  createModule(name: string): ScriptModule
  remove(idOrName: string): void
  isNameExists(name: string): boolean
  importZip(name: string, binary: number[]): string
  exportAsZip(nameOrId: string): number[]
  openFileStream(fileType: string): KModuleFiles
  searchRepository(keyword: string): ModuleSearchResult[]
  installFromRepository(packageId: string, name: string): string
}
```

The complete signature is subject to site `kooboo.d.ts` / IDE completion.

## `module.config` Conventions

Module root directory **`module.config`** (JSON) common fields:

| Field | Description |
|------|------|
| `name` / `version` / `description` | Package metadata; **`name` is the module ID when mounting the menu** |
| `settingDefines` | Backend **Settings** form definition (`name`, `type`, `defaultValue`, `display`, etc.) |
| `menu` | Mounted to [the left sidebar in the site admin](../../cms/development/modules.md); only **online** modules take effect |

### Menu Configuration

When the site is loaded, the `Site` API reads the `module.config` of each online module. If `menu` exists, it is merged into `moduleMenus` and rendered to the left menu (see CMS documentation for details).

| Field | Description |
|------|------|
| `name` | menu title |
| `nameTranslation` | Optional, displayed according to the background user language |
| `parent` | Optional, `menu.name` for top-level menus (e.g. `commerce`, `content`) |
| `url` | Optional, relative path under module `view/`; if empty, use module default starting view |
| `icon` | Optional, file name under `img/` |
| `children` | Optional submenu, each item contains `name`, `url`, etc. |

The resolved access URL has the form `/_moduleName/{viewPath}?SiteId={siteGuid}`. Use `k.module.config` inside module views to read runtime settings.

The saved key value of `settingDefines` is exposed to the script through **`k.module.config`** (not related to `menu`). The documentation is written in **`Readme.md`**.

## Related Docs

- [Modules (CMS)](../../cms/development/modules.md) — list, import, development mode, online, and sharing
- [k.site](../site/) — Site Resources
- [k.DB](../database/) — site database
- [k.file](../file/file.md) — site disk file (not module directory)
- [Development -> Scheduled Jobs](../../cms/development/jobs.md) — site-level scheduled jobs (`k.site.runJob`)
