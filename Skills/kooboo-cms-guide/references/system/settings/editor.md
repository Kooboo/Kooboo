# Site Settings - Editor

## Page Purpose

- Admin page: `/system/settings?name=editor`
- Configures rich-text editor defaults, inline-design capabilities, code-editor hints, and code-edit recording.
- These settings mainly affect Kooboo admin editing experience and do not install a frontend framework or change visitor-page permissions directly.

## Rich-Text Editor Configuration

- When enabled, toolbar, font list, and font-size list can be set uniformly for rich-text fields in content types.
- A field without its own configuration inherits site defaults; a field-specific setting takes priority.
- Resetting an item restores default behavior rather than clearing existing rich-text content.
- This affects editor options and does not automatically rewrite saved article HTML.

## Inline Design Settings

- Controls which editing actions are visible or available in inline design, such as text, image, link, style, content, menu, component, and global color/image/link/text entries.
- Turning an item off usually hides or disables the tool in inline design and does not delete existing page content.
- These switches do not replace user permissions; even visible tools remain constrained by admin permission and object state on save.
- Batch updates to similar pages, element deletion, and global-resource changes have broad impact; editors should understand the scope after enabling them.

## Code-Editor Hints

- Hints can be added for tags, attributes, or style classes for Bootstrap 3, Tailwind CSS 3.1, Element Plus 2.2, and AMP.
- These options only extend intelligent suggestions in Kooboo's code editor.
- Selecting a hint package does not add framework runtime files to a page or guarantee the site uses the same version.
- If the site does not actually load the framework, editor-suggested class names or components do not take effect on the frontend automatically.

## Record Site-Log Video

- When enabled, Kooboo records supported code-editor operation events and associates the editing process with site logs when pages, layouts, views, styles, scripts, forms, or code are saved.
- It records the editor operation sequence, not a screen recording or visitor-session recording.
- No actual change or an operation outside supported editors may produce no replayable record.
- Enabling increases log data and save requests; decide whether to keep it on based on collaboration-trace needs.

## Common Misconceptions

- Rich-text defaults do not override field-specific configuration.
- Inline-design switches are not an admin permission system.
- Code hints do not install a frontend framework automatically.
- Log video is not screen recording or visitor behavior analysis.
