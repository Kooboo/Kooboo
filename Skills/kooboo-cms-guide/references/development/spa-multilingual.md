# Development Resources - SPA Multilingual

## Page Purpose

- Admin page: `/development/spamultilingual`
- This page maintains the “translation key → text in each language” data used by an SPA. It is a different data set from ordinary CMS content localization and Label copy.
- The system returns key-value data by language through the site's SPA language-data interface. When the requested language has no value, it attempts to use the translation item's own default language. This feature does not translate copy automatically.

## Translation Items and Language Settings

- Each translation item has a name as its key and stores values in multiple languages. The page shows the current default language in the first column, but historical or imported data may have inconsistent language sets.
- Editing a translation item updates the language values shown on the current page. An empty value may fall back to the item's default language at runtime; when the default language also has no value, the key is omitted from the response.
- Settings uniformly adjusts language keys on existing translation items. Adding a language adds an empty value to every existing item.
- Removing a language deletes that language value from every item rather than temporarily hiding a column. The current interface does not allow the first default language to be removed, but export a backup before changing languages.

## Import Rules

- Import supports a JSON file or JSON entered directly. Top-level keys are translation-item names, each at most 50 characters; the nested object contains languages and values.
- `combine` fills only missing or empty language values on existing items and does not overwrite existing non-empty text.
- `replace` overwrites values for the same item and language with non-empty values from the import file; empty imported values are skipped.
- `replace` does not delete translation items absent from the import file or clear languages not supplied in the file. It overwrites by imported values rather than mirroring the entire translation library.
- A newly imported translation item uses its first language as the default and fills empty values for languages already on the site. Before importing, ensure language codes and default-language order are as expected.

## Export, Runtime, and Verification

- Export All and Export Selected generate a JSON file in the browser from the currently loaded list. Export is only a data snapshot; it does not publish the SPA, switch site language, or modify frontend code.
- Deleting a translation item deletes data for that key and does not automatically scan or repair references in SPA code. The frontend may receive a missing value when it continues to read the deleted key.
- After changes, visit the real SPA and request the target language, a language missing the translation, and the default language separately. Verify key names, fallback results, and frontend loading logic. A correct admin table does not mean the SPA uses the correct language address or has refreshed its own cache.
