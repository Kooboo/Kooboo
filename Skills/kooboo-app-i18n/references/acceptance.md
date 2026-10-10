# Acceptance and diagnosis

Read back the affected objects. Installer success only proves independent assets were copied. Before claiming integration complete, check all of these within the requested language/page scope:

- Read back ENV.i18n and Services.i18n's import; preserved ENV settings have been reviewed even if asset installation succeeded. Unrelated ENV fields remain intact and are not exposed to the browser. The site default and configured supported cultures agree; configured prefixes expose only the App's public UI text. Only exact Label values are returned, no regional/first-language substitutions or accidental creation.
- Import map uses one Vue, vue-i18n is registered before mount, and anonymous startup can load interface copy. First-load failure displays a retryable error.
- All referenced keys have default-language values. Existing translated values survived migration. Named parameters, literal `@`, `|`, braces, and any plural forms render as intended.
- Startup with a saved non-default language makes one translation request; switching to the default pack already included in that response makes none.
- Switching does not reload or change the route/auth state. Menus, title, placeholders, validation UI, component-library text, document.lang, dates and numbers update. Current business data/timezone remain governed by their own contracts.
- Failed switches preserve the old language; retry works. Fast repeated requests cannot revert to an older selection. Storage restrictions do not break the App. URL lang and the selected preference agree after switching.
- Refresh observes edited/deleted Labels. Requests/cache/preferences do not mix sites or unrelated prefixes. Unsupported explicit locale returns 400.
- Repeat installation skips identical objects; differing objects or routes are preserved. Report any partial installation and unverified write instead of claiming atomic rollback.
- Existing custom main/layout and design-system adapters remain intact. Follow kooboo-coding's browser opt-in rule; report visual/interaction behavior as unverified if no browser verification was requested.

Useful failure distinctions:

| Symptom | Inspect |
| --- | --- |
| Key is displayed | Exact default Label value, configured prefix, installed import map and t key |
| Unexpected language fallback | Ensure API uses Services.i18n exact-key check, not k.label/get_label or /_spa/lang |
| Menu/title stays in old language | Translation inside computed/watch; title helper registered after legacy router title hook |
| Controls still Chinese | Element locale mapping, shell label props or hardcoded internals of the used Volcengine component |
| Refresh undoes selection | URL lang before hash, site-scoped preference and any external domain/path policy |
| Label edit not visible | Reload the App's page-session cache; inspect response headers and external caching |
| Installer conflicts | Read the named existing object and route; do not rerun create blindly |

Repository development tests (not commands to run inside a hosted Site):

```sh
node --test Kooboo.Sites/AI/Skills/packages/kooboo-app-i18n/scripts/runtime.spec.mjs
dotnet test tests/Kooboo.SitesTests/Kooboo.SitesTests.csproj --no-restore --filter FullyQualifiedName~AppI18nTest -m:1 -nodeReuse:false
```

The Node suite executes the vendored Vue/vue-i18n with the service; the C# suite executes installation and routed APIs in the real KScript engine. Neither replaces browser interaction testing on a delivered App.
