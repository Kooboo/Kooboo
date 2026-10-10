# Content Management - Labels

## Page Purpose

- Admin page: `/content/labels`
- A Label saves interface text that can vary by language under a stable name, and is suitable for site copy such as buttons, titles, and prompts.
- After a page, layout, or view references a Label, the frontend reads the corresponding value for the current access language.

## Names and Language Values

- A Label name is the stable identifier used by references; language values are the text visitors ultimately see.
- After site multilingual support is enabled, a different value can be maintained for each configured language. An empty language is not automatically translated or reliably given a high-quality fallback.
- Changing one language value affects every location that references the Label, but does not change the Label name.
- The default list, page view, and category view are different organizations of the same Labels; they do not create copies.

## Usage Relationships and Versions

- Used by information helps determine whether resources such as pages, layouts, or views reference the Label.
- The version entry shows historical changes for the Label; availability depends on site-log permissions and history.
- Dynamically concatenated names and custom-script references may not be fully reflected in usage relationships, so validate the site before deletion.

## Scanning Labels

- Ordinary scanning finds text or attributes in HTML resources such as pages, layouts, and views that are suitable for extraction as Labels.
- Scan results are only candidates. Kooboo creates the corresponding Label and rewrites the source resource as a Label reference only after items are selected and confirmed.
- Therefore, Scan and Add is not purely read-only. Before confirming, verify names, original text, attributes, and affected objects; retain a version or backup for important sites first.

## Scanning Multilingual Calls

- Multilingual scanning finds candidate multilingual text calls in script resources.
- After an update is confirmed, the corresponding Label is created and the selected script content is rewritten.
- The scanner recognizes only supported forms; dynamically generated text, indirect concatenation, or special wrappers are not guaranteed to be found.

## Import and Export

- The export file is JSON. A multilingual site organizes it as “language → Label name → value”; a single-language site exports names and values in the default language.
- Import adds or updates the language values supplied in the file by Label name.
- Import is not a full synchronization deletion: existing Labels absent from the file are not automatically removed.
- Before importing, confirm that the file uses a supported JSON structure. Structural or language-key errors can leave some values unimported rather than being inferred into the correct format.

## Common Misconceptions

- Labels are suitable for short interface copy, not content articles or rich-text fragments.
- Scanning does not automatically translate the whole site.
- Deleting a referenced Label does not mean all references are safely deleted at the same time.
