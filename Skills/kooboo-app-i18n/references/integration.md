# Install and integrate

## Inspect and select

Read current site cultures/defaultCulture, index import map, main, router, layout, design runtime and existing Label values. Keep the App's auth, router, custom layout and startup behavior. A planned feature is not an installed feature. Record language scope in existing planning descriptions/acceptance criteria when applicable; no new App Plan fields are required.

Use the exact culture codes returned by the site. Kooboo's DefaultCulture setter currently normalizes region codes to two letters (for example zh-CN becomes zh); do not assume the default is zh-CN. Map App locales to component-library locales explicitly. The default must be among the App's supported site cultures. Configure missing site languages using the existing site-settings workflow before installation. Never change language domains/paths incidentally.

This template targets same-origin hash-router Apps. For a site already using language-specific domains/paths, inspect that policy first and adapt navigation/API URLs; the template does not implement cross-domain language switching or multilingual SEO. Business API culture remains server-owned: browser locale changes do not automatically localize business data or authenticated errors.

## Install independent assets

### Shared server configuration

App i18n settings belong to the module CodeBlock `ENV`, exported as `ENV.i18n`. Read the existing ENV source and merge only this section, preserving other settings and custom logic. If ENV uses a TypeScript interface, add the corresponding field to that interface too.

```ts
// Inside the existing exported ENV object:
i18n: {
  prefixes: ['app.orders.'],
  supportedLocales: ['zh', 'en']
}
```

`Services.i18n` imports `{ ENV }` from `./ENV`. Supported cultures and public Label prefixes come only from `ENV.i18n`; the default language remains the site's `defaultCulture`, not a second i18n default. The browser receives only the selected locale metadata and public messages, never the entire ENV object.

When ENV is absent, the installer creates a module exporting ENV with this section. When ENV already exists, the installer preserves its source byte-for-byte and checks its object type only; it does not evaluate or rewrite arbitrary JS/TS. Its `configuration` receipt identifies `object: "ENV"`, `field: "i18n"`, the requested value and status. `preserved-review-required` means compare the actual ENV.i18n with the intended settings and explicitly merge any needed changes before using the API. Installer arguments initialize a missing ENV and describe the requested section for an existing one; they are not a second runtime configuration source. The service rejects missing or invalid settings.

### Copy assets

Call `run_skill_script` with:

```json
{
  "skillName": "kooboo-app-i18n",
  "scriptName": "scripts/install-runtime.js",
  "arguments": {
    "prefixes": ["app.orders."],
    "supportedLocales": ["zh", "en"],
    "dryRun": true
  }
}
```

Choose prefixes for the actual App, then repeat with dryRun=false after checking the result. All prefixes must start with `app.` and end with `.`. Only public UI copy belongs in these prefixes; the read-only translation endpoint may be accessed anonymously before authentication bootstrap. Request parameters cannot broaden the prefix set.

The script copies packaged assets through existing create Functions without round-tripping the large vendor source through model output. It validates all names/routes before writing, skips identical objects and refuses different existing objects. It rechecks each write and reports completed objects on partial failure; multi-object installation is not a database transaction. Review customized objects rather than overwriting them or claiming rollback. It installs no Labels and does not change main/index/router/layout or site settings.

| Object | Name / browser URL | Purpose |
| --- | --- | --- |
| CodeBlock | ENV | Shared server configuration; only ENV.i18n owns supported cultures and Label prefixes |
| CodeBlock | Services.i18n | Strict culture reads, no implicit Label fallback |
| API | app.i18n / `/api/app-i18n/{action}` | GET base URL `/api/app-i18n?locale=en` |
| Script | vue-i18n.js / `/vue-i18n.js` | Pinned full browser ESM runtime with message compiler |
| Script | service.i18n.js / `/service.i18n.js` | Startup, page-session cache, switching and title helper |

`GET /api/app-i18n` returns siteId, locale, currentLocale, defaultLocale, supportedLocales and messages for the selected/default languages. When locale is omitted, startup selects the first supported value from preferredLocale (URL choice), savedLocale (stored choice), server culture and default. Unsupported preferences are ignored; an invalid explicit locale still returns 400. Response is not wrapped in a `data` field. Only exact, non-empty Label culture values are included. No k.t calls or persistent server cache are used. Changes/deletions become visible on page reload.

## Add Label copy

Use stable semantic keys such as `app.orders.common.save`. Keep the complete key within 64 ASCII letters/digits/underscores/hyphens/dots. Inspect raw existing culture keys before inserting missing translations: ordinary get_label/k.label/GetCultureValue can fall back to another language. To inspect one Label in KScript, first inspect `label.keys`; call `label.getCultureValue(exactKey)` only if that culture key exists. `Services.i18n` shows this pattern.

Use existing `set_label` batches with an explicit culture, preserving other languages and existing translations. Example arguments after inspection:

```json
{"culture":"en","labels":[
  {"name":"app.orders.common.save","value":"Save"},
  {"name":"app.orders.language.label","value":"Language"},
  {"name":"app.orders.language.error","value":"Could not switch language. Try again."}
]}
```

Supply the actual default-language equivalents too. Include externalized startup/shell/account/notification UI, empty/loading/error states, aria-labels, placeholders, validation and actions. Existing notification title/body records and other business content remain data; do not rewrite them as interface keys. For new backend errors, return stable code/params and translate the mapped UI key; keep legacy message as a fallback.

Label messages use vue-i18n syntax: `{name}` for named interpolation, `no items | one item | {count} items` for plural forms. Literal examples: `user{'@'}example.com`, `left{'|'}right`, `{'{'}name{'}'}`. Do not interpolate on the server first, split sentences around variables, or feed Label content into v-html. This richer syntax is not equivalent to k.label's simple replacement.

## Patch startup locally

Add `"vue-i18n": "/vue-i18n.js"` to the existing import map, keeping `vue` mapped to the same `/vue.js`. The bundle imports Vue externally; do not load a second Vue or use a bundler-only npm entry.

The bundled version, checksum and license are recorded in [vendor provenance](../assets/vendor.md).

Merge these operations into the existing main.js try block, preserving its bootstrap, router, error handler, preview bridge and icon initialization:

```js
import { createAppI18n, bindTranslatedTitle } from '/service.i18n.js'

// Alongside the existing loadAppBootstrap call, before mounting:
const languages = await createAppI18n()
// After the existing createApp and createAppRouter:
app.use(languages.i18n)
app.provide('app.i18n', languages)
// Register after createAppRouter has registered its legacy afterEach hook.
const stopTitle = bindTranslatedTitle(router, languages.i18n, {
  appTitleKey: 'app.orders.name', appTitle: document.title
})
// Keep existing app.use(router), await router.isReady(), and app.mount(...).
// If this App has an unmount lifecycle, call stopTitle there.
```

Initial failure must reach the existing startup error UI with a retry/reload action. A short static emergency message is acceptable while translations are unavailable; do not keep a duplicate full translation dictionary. `createAppI18n` rejects failures instead of silently showing keys. Initial priority: supported URL `?lang=` before the hash, then site-scoped stored choice, then server-selected locale, then default. Hash route queries are not server language selection. The installer embeds the site ID in service.i18n.js so startup can read the existing site-scoped preference before its first request. Configuration and the selected/default messages arrive together in one request. Both returned packs are cached, so switching back to the default language requires no additional fetch.

Use `languages.setLocale(value)` for switching; do not assign its locale ref directly. It loads both requested/default messages, prepares component resources, and only the latest successful request commits. Failed requests can retry. It updates document.lang and optional localStorage; existing URL lang is updated without changing hash/history state. No persistent message cache is installed. Refresh to pick up edited/deleted Labels; runtime configuration changes require reload.

## Vue and switch control

```js
import { computed, inject, ref } from 'vue'
import { useI18n } from 'vue-i18n'
// Inside the existing setup():
const { t, locale } = useI18n({ useScope: 'global' })
const languages = inject('app.i18n')
const switching = ref(false)
const languageError = ref(false)
async function changeLanguage(event) {
  switching.value = true
  languageError.value = false
  try { await languages.setLocale(event.target.value) }
  catch { languageError.value = true }
  finally {
    event.target.value = locale.value // restore the select when a request fails
    switching.value = false
  }
}
const rules = computed(() => ({
  name: [{ required: true, message: t('app.orders.nameRequired'), trigger: 'blur' }]
}))
// Expose t, locale, languages, switching, languageError, changeLanguage, rules.
```

Example accessible control, adapted to the current design system:

```html
<label>
  {{ t('app.orders.language.label') }}
  <select :value="locale" :disabled="switching" @change="changeLanguage">
    <option v-for="code in languages.supportedLocales" :key="code" :value="code">{{ code }}</option>
  </select>
</label>
<p v-if="languageError" role="alert">{{ t('app.orders.language.error') }}</p>
```

Use localized or native language display names when appropriate. Add all referenced keys to Label. Buttons use `{{ t(key) }}`, attributes use `:placeholder="t(key)"` / `:aria-label="t(key)"`. Compute menus from stable `meta.navigationLabelKey` / `navigationGroupKey` with old text as compatibility fallback. Keep translations inside computed getters, not once at route creation. Add `meta.titleKey`; the supplied title helper handles both navigation and switching. Locale-aware dates/numbers use Intl or vue-i18n formatting; timezone is independent. For currently displayed form validation errors, revalidate or clear them on switching according to the form's behavior.

## Design system adapters

Element Plus already exports epZhCn and epEn from `/element-plus.js`. Create a shared shallowRef for the component locale and wire prepare/commit during startup:

```js
import { shallowRef } from 'vue'
import { epZhCn, epEn } from '/element-plus.js'
const elementLocale = shallowRef(epZhCn)
const elementLocales = { zh: epZhCn, 'zh-CN': epZhCn, en: epEn }
const languages = await createAppI18n({
  prepareLocale: async code => {
    if (!elementLocales[code]) throw new Error('Component language unavailable')
    return elementLocales[code]
  },
  commitLocale: (_code, prepared) => { elementLocale.value = prepared }
})
app.provide('app.elementLocale', elementLocale)
```

In the layout inject `app.elementLocale` and replace fixed `:locale="epZhCn"` with `:locale="elementLocale"`. Extend the explicit map and provide real component locale objects for additional languages. prepareLocale may be asynchronous; commitLocale must be synchronous, small and non-throwing. Translate KbElementShell's navigation/menu labels and surrounding popovers as well; ElConfigProvider does not translate App copy.

Volcengine has no universal locale provider. Patch application shell text, and pass translated label props where supported (for example YlPagination label/previousLabel/nextLabel). Some options such as pagination totals/jumper include internal hardcoded copy: inspect the components actually used, then adapt the installed source locally or compose translated App controls. Do not claim those internals are multilingual merely because the shell changes language. Avoid replacing the complete design runtime with a second bundled copy. Before later design-runtime synchronization, inspect and preserve these App-owned adaptations.

## Maintenance and removal

After verification, record the service/main registration, supported cultures and key prefixes in site memory. On later edits load this skill if those actual assets are in use. Keep installation customized code intact; a template version marker is provenance, not permission to overwrite it. To remove i18n, first remove translated consumers and registration, then unused assets. Preserve Labels unless deletion is explicitly requested; existing Label reference scans do not prove a Vue Script key is unused.

When maintaining an installed App, review the App-owned service.i18n.js, Services.i18n and app.i18n together. Preserve custom code, ENV and Labels. Verify Services.i18n reads ENV.i18n and bind installedSiteId in the frontend service to the target site ID (or pass siteId to createAppI18n). Do not blindly rerun the installer over customized objects. Copying a bound service to another site requires rebinding its site ID. When removing i18n, remove only the unused ENV.i18n section; preserve the shared ENV object and other modules' settings.
