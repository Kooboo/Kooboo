# SPA multi-language (front-end interface)

> Provides a JSON dictionary for single-page applications; entries are maintained at [CMS: SPA Multilingual](../../cms/development/spa-multilingual.md)

## Overview

Kooboo **SPA Multilingual** saves translations as site resources (key name + multilingual `value` dictionary), and outputs the key→copywriting mapping in a certain language through a fixed URL at one time for loading by front-end frameworks such as Vue and React.

::: warning No KScript management API
The current version **does not** have a `k.spaMultilingual` or similar `k.*` interface to add, delete or modify entries in the script. Please import/edit in the background **Develop → SPA Multi-Language**; if you need similar copywriting on the script side, please use [k.label() / k.t()](../label/index.md).
:::

::: Division of labor between tip and k.label
| ability | SPA Multilingual | k.label/k.t |
|------|----------|----------------|
| Data maintenance | Backstage SPA multilingual page | Backend **Content → Tag** |
| Consumption | Browser `fetch('/_spa/lang/en')` | Template `k-label`, Code `k.t('key')` |
| language origin | URL path segment `{languageCode}` | Site multilingual / current request culture |

---

## Get dictionary

### ask

```http
GET /_spa/lang/{lang}
```

| part | illustrate |
|------|------|
| `{lang}` | Language code, consistent with the background column (lowercase), such as `en`, `zh-cn`, `zh-tw` |

No `SiteId` query parameter is required: the site context is resolved by the currently visited site domain name.

### response

- **Status code**: 200
- **Content-Type**:`application/json`
- **Body**: object, key is the entry name, value is a string

```json
{
  "welcome": "Welcome",
  "nav.home": "Home"
}
```

Only entries with a value in this language (or after fallback) are included; keys that have no translation and no value in the default language will not appear in the results.

### fallback

For each entry:

1. Get `value[lang]` (the language in the request path);
2. If it is empty, take the copy corresponding to the **default language** (`defaultLang`) configured in the background for this item;
3. Write the response if there is still a value, otherwise skip the key.

The default language corresponds to the column in the list marked **(Default)**.

### cache

`SpaMultilingualMiddleware` caches the serialized JSON by **site Id** for about **3 seconds** to reduce the pressure of high-frequency refresh. After the entry is saved in the background, in the worst case, the front end may temporarily read the old data. You can try again later or cache it on the front end.

### CORS

This path will set the CORS related response header (`CorsHelper.HandleHeaders`) to facilitate cross-domain debugging of the SPA development server; it is still recommended to deploy from the same origin as the site in the production environment.

---

## Front-end access example

### native fetch

```javascript
const lang = "zh-cn" // Keep this consistent with the vue-i18n locale
const dict = await fetch(`/_spa/lang/${lang}`).then((r) => r.json())
document.querySelector("h1").textContent = dict.welcome ?? "welcome"
```

### Vue 3 Example

```javascript
import { createApp, ref } from "vue"

const locale = ref("zh-cn")
const messages = ref({})

async function loadLocale(code) {
  messages.value = await fetch(`/_spa/lang/${code}`).then((r) => r.json())
  locale.value = code
}

createApp({
  setup() {
    loadLocale("zh-cn")
    return { locale, messages, loadLocale }
  },
  template: `<p>{{ messages.welcome }}</p>`,
}).mount("#app")
```

The actual project can be encapsulated as a plug-in and re-installed when switching languages.

### Multi-language linkage with the site

If the site also enables Kooboo **routing-level multilingual** (URL prefix or `?lang=`), it is recommended that the `{lang}` used by the SPA be **unified** with the current UI language code (both use `zh-cn` or both use `zh`) to avoid inconsistency between the dictionary language and routing culture.

## Import JSON Format

During batch maintenance, files can be prepared according to this structure and imported into CMS:

```json
{
  "keyName": {
    "en": "English text",
    "zh-cn": "Simplified Chinese"
  }
}
```

- `keyName`: Maximum 50 characters.
- Inner key: language code (stored in lowercase after import).

---

## Related Docs

- [SPA Multilingual (CMS)](../../cms/development/spa-multilingual.md) — import, export, language list, editing
- [k.label / k.site.labels](../label/index.md) — server-side template multi-language
- [k-data culture](../../templateEngine/k-data/query.md#culture) - Multilingual URL within template
