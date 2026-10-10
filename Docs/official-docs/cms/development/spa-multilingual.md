# SPA Multilingual

> Menu: **Development → SPA Multilingual**  
> Deep link: `/_Admin/development/spamultilingual?SiteId={siteGuid}`

Maintain key-value multilingual copy for **single-page applications (SPA)**: edit translations per language in admin; front-end fetches JSON dictionary via `/_spa/lang/{languageCode}`. For Vue/React etc. managing UI language without Kooboo server template rendering.

::: tip Permissions
View list: `spaMultilingual`. **Import**, **Settings** (language columns), row **Edit** require `spaMultilingual·edit`; multi-select **Delete** requires `spaMultilingual·delete`.
:::

::: info vs Content → Labels
| | **SPA Multilingual** (this page) | **[Content → Labels](../content/labels.md)** |
|--|------------------------|--------------------------------------|
| Consumption | Front-end requests `/_spa/lang/{lang}` for full JSON table | Templates / KScript: `k.label()`, `k.t()` |
| Typical scenario | SPA, static front-end bundled in site | Layout / Page / View server rendering |
| Script API | No `k.*` read/write; see [SPA Multilingual API](../../api/spa-multilingual/index.md) | [k.site.labels](../../api/label/index.md) |

Site **page-level multilingual** (routes, culture switch) is still in **Site settings → Basic**; see [Pages](../site/pages.md) and [k-data · culture](../../templateEngine/k-data/query.md#culture).
:::

## How to Open

1. In [Edit Menu](../navigation.md#edit-menu), enable **Development → SPA Multilingual**.
2. Open **Development → SPA Multilingual** in the left sidebar.

<DocImage src="/cms/development/spamultilingual-overview.png" alt="SPA multilingual list overview" width="1120" />

## Toolbar

| Button | Permission | Description |
|------|------|------|
| **Import** | `spaMultilingual·edit` | Open [import dialog](#import-dialog), batch write entries from JSON |
| **Export all** | No extra restriction | Download all entries as `translate-YYYYMMDD.json` |
| **Settings** | `spaMultilingual·edit` | Open [language settings dialog](#language-settings-dialog), enable language columns |

After list selection, toolbar shows **Export selected** (checked entries only).

<DocImage src="/cms/development/spamultilingual-toolbar.png" alt="SPA multilingual toolbar" width="1120" />

## List table

Columns from [language settings](#language-settings-dialog): first language column header marked **(default)** = entry `defaultLang`; others are language codes (e.g. `en`, `zh-cn`).

| Column | Description |
|----|------|
| **Name** | Entry key (unique), JSON key; max 50 chars (validated on import) |
| **Language columns** | Copy for that key in each language; empty = untranslated |
| **Actions** | **Edit** opens [edit entry dialog](#edit-entry-dialog) |

Supports multi-select **Delete** entries.

<DocImage src="/cms/development/spamultilingual-table.png" alt="SPA multilingual list" width="1120" />

::: tip New entries
No separate **Create** on list; use **Import** for first or batch keys. **Edit** rows to complete translations.
:::

## Language settings dialog

**Settings** opens. Tag-style checkboxes for enabled **language codes** (aligned with Element Plus locales, e.g. `en`, `zh-cn`, `zh-tw`).

- **First language in list** (column title "default") cannot be unchecked here — site default language column.
- Save calls `SetLang`: adds/removes language keys on **all** entries; new languages start empty, fill in list or import.

<DocImage src="/cms/development/spamultilingual-setting-dialog.png" alt="Language settings dialog" width="1120" />

## Import dialog

| Area | Description |
|------|------|
| **Submit mode** | **Replace**: overwrites existing translations for same key; **Merge**: only fills empty language fields |
| **Content type** | **File** (`.json`) or **Code** (Monaco paste JSON) |
| **Example** | Download `example.json` |

JSON structure: entry name → language code → copy:

```json
{
  "welcome": {
    "en": "Welcome",
    "zh-cn": "欢迎"
  },
  "nav.home": {
    "en": "Home",
    "zh-cn": "首页"
  }
}
```

- Each **key** (entry name) length ≤ 50.  
- Language codes normalized to lowercase (e.g. `zh-cn`).  
- **Replace** with existing data prompts confirmation to avoid accidental overwrite.

<DocImage src="/cms/development/spamultilingual-import-dialog.png" alt="Import dialog" width="1120" />

## Edit entry dialog

Row **Edit** (title may show **Settings**). Textareas per enabled language; save updates that key (`SpaMultilingual/post`).

## Front-end usage

After save, SPA requests at runtime (same origin):

```http
GET /_spa/lang/{languageCode}
```

Example `GET /_spa/lang/zh-cn` returns:

```json
{
  "welcome": "欢迎",
  "nav.home": "首页"
}
```

- Missing translation for a key uses that entry's **default language** copy.  
- Response `Content-Type: application/json`; CORS headers (local debugging).  
- Server caches full table ~**3 seconds** in memory (per site Id).

Integration examples (Vue i18n, react-i18next, etc.): **[SPA Multilingual API](../../api/spa-multilingual/index.md)**.

## Related

| Doc | Description |
|------|------|
| [SPA Multilingual API](../../api/spa-multilingual/index.md) | `/_spa/lang/{lang}` response and fallback |
| [Labels](../content/labels.md) | Template `k.label()` |
| [k.label API](../../api/label/index.md) | Server-side multilingual copy |
| [Development overview](./index.md) | Development menu index |
