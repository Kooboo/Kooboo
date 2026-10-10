# Local UI

Use `prompt-recommendations` to replace a local section:

```json
{
  "mode":"prompt-recommendations",
  "query":"editorial portfolio modern",
  "sectionType":"hero",
  "pageName":"home",
  "limit":5,
  "excludePromptIds":[]
}
```

In a site-building context, `brief` can be used instead of `query`. For general material search, use `search` + `query` + optional `domain` / `limit`; for stack rules, use `stack` + `query` + `stack`. The `limit` range is 1–20.

Local UI results must still follow the current site's Design System, real content, accessibility, responsive behavior, and interaction capabilities. Do not fabricate data, external images, or submission behavior.
