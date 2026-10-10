# Development Resources - Modules

## Page Purpose

- Admin module list: `/development/modules`.
- Module admin-menu container: `/development/module-menu/:module`; `:module` represents the current module, and the actual content is determined by the admin pages configured by that module.
- A module is a separately packaged set of site-extension resources that can contain page fragments, scripts, styles, code, events, admin pages, and its own data. It is not an ordinary Page, View, or Layout resource.
- Installing a module puts executable extensions into the current site. A trusted source, successful import, or readable documentation does not mean the module code is safe, compatible with the site, or correct in business results.

## Creation, Files, and Runtime Status

- When an empty module is created, its name is normalized and module configuration, event scripts, admin scripts, and documentation files are initialized. Module resources are stored in an independent directory; clicking the list name opens them for editing in Development Mode.
- A module has its own access entry, usually under `/_模块名/...`. Creating or saving a module does not automatically assemble it into existing pages or prove that a module page is usable.
- `Online` controls whether the module participates in runtime. Only an online module executes module routes and may contribute entries to the admin's left menu.
- Offline is not deletion: the module record, files, and settings remain, but features depending on module routes or dynamic menus may stop working or displaying.
- Changing module files affects entries using the module. A successful save proves only that files were written; it does not prove scripts, pages, events, permissions, and external services cooperate correctly.

## Import and Online Installation

- Import creates a module from a module name and ZIP package. If the name already exists, same-name import overwrites the existing module directory rather than safely merging file by file; files in the old directory may be removed.
- Online-search installation downloads a ZIP from the module service and installs it into a new module directory. Successful download or extraction does not mean the module is online or that its source, dependencies, permissions, or runtime behavior have been verified by the current site.
- Before import or installation, confirm source, version, configuration, and site compatibility. Before going online, inspect module files and verify key reads/writes, permissions, and external calls in an isolated environment.
- A module can include executable code and events and provide frontend and admin pages, so installing an unknown module is riskier than importing an ordinary image or text resource.

## Settings and Documentation

- The Settings form comes from the module's own configuration definition. When settings exist, it reads saved JSON; without saved settings, the module may initialize and save settings from configuration defaults during runtime.
- Changing settings saves only module-setting data and does not guarantee that module business is active. Whether a page reload, dependency, permission, or external connection is required is determined by the module.
- Documentation displays Markdown bundled with the module. It is usage documentation from the module author, not Kooboo's endorsement of the module's safety, accuracy, or compatibility.
- Do not infer actual module capabilities from documentation alone; verify them with module files, configuration, and actual runtime results.

## Backend Manage and Dynamic Module Menus

- When a module contains admin HTML pages, the list may show Backend Manage. The system prefers a common default entry name and otherwise uses the first admin HTML page found.
- An online module can add entries, subitems, icons, display names, and menu groups to the admin menu through its own configuration. Relative addresses resolve to the module's access path; an empty main address may fall back to the module's start page.
- `/development/module-menu/:module` itself only opens the menu's target URL in an iframe. An iframe finishing load means only that the browser finished loading; it does not prove page permissions, data requests, saves, or external business succeeded.
- The functions, fields, permissions, data writes, and risks of a dynamic-menu page are determined entirely by the installed module. Generic page knowledge cannot replace the module's own documentation, and module documentation cannot automatically be trusted or treated as system-level facts.
- The same generic page may host different modules. During troubleshooting, first confirm module name, actual target address, online status, and module configuration, then inspect that module's runtime logs and business results.

## Export, Sharing, and Deletion

- Export packages the current module directory as a ZIP, suitable for backing up or migrating module files; it does not prove external dependencies, site data, keys, and runtime environment are included.
- Sharing uploads the module package to an external module service and depends on the current account, network, and server processing. A successful button request must not be interpreted directly as proof that the module was reviewed, published, or can be installed normally by other sites.
- Deletion removes the module record and attempts to delete the module directory. It does not automatically repair static or dynamic references in pages, scripts, jobs, or external systems.
- Offlining or deleting also does not undo business data written by the module, external requests already sent, or other irreversible side effects. Back up and inspect consumers first, then reverify related pages and business flows afterward.
