# Basic · Editor Tab

> Tab: **Editor** · `name=editor`  
> Deep link: `/_Admin/system/settings?SiteId={siteGuid}&name=editor`

Configures admin and **inline editing** experience: rich text (TinyMCE) toolbar, **inline design** defaults, Monaco **code suggestion** libraries, and **operation log screen recording**.

<DocImage src="/cms/settings/settings-basic-editor.png" alt="Editor tab" width="1120" />

## Rich Text Editor

**RichTextEditorConfig** (`tinymce.vue`): site defaults for TinyMCE plugins, toolbar buttons, etc., affecting content fields, multiline text, and other rich-text controls.

## Inline Design Settings

| Item | Description |
|------|-------------|
| **Inline design settings** | Click **Edit** to open `InlineDesignSettingsDialog` for default inline edit behavior and panel options (`inlineDesignSettings`) |

Use with [Page inline edit](../../site/pages-inline-edit.md).

## Additional Code Suggestions

**Additional code suggestions** (`codeSuggestions`) multi-select: inject extra type/snippet libraries into Monaco, e.g.:

- `bootstrap-v3`
- `tailwind-v3.1`
- `element-plus-v2.2`
- `amp-v0`

Affects IntelliSense in [Development -> Code](../../development/code.md) and similar editors.

## Operation Log Screen Recording

**Record site log video** (`recordSiteLogVideo`): when on, operation logs may include screen recording (related to [Operations · Site logs](../../operations/site-logs.md); product behavior may vary).

## Related

- [Basic Settings overview](./index.md)  
- [Content · Entries](../../content/contents-entries.md)
