# Kooboo AI skills

The skill packages that ship with the Kooboo AI chat: the instructions and reference
material the agent loads at runtime.

Both locales use the same shape as [`Docs/official-docs/`](../Docs/official-docs): English
at the top level, Chinese in a `zh/` subfolder.

| Locale | Location | Files |
| --- | --- | --- |
| English (primary) | `<skill-name>/` | 747 |
| Chinese | [`zh/<skill-name>/`](zh) | 256 |

Each locale tree is self-contained — the locale is expressed by the folder, never by a
filename suffix (there is no `SKILL-zh.md` or `references-zh/` anywhere):

```
Skills/<skill-name>/SKILL.md              English
Skills/<skill-name>/references/           English reference material
Skills/<skill-name>/assets|scripts|data/  shared runtime resources (English tree only)
Skills/zh/<skill-name>/SKILL.md           Chinese
Skills/zh/<skill-name>/references/        Chinese reference material
```

## Skills

| Skill | English | Chinese | Purpose |
| --- | --- | --- | --- |
| `kooboo-coding` | [dir](kooboo-coding) | [zh](zh/kooboo-coding) | Site / App / scoped coding router (largest skill) |
| `kooboo-cms-guide` | [dir](kooboo-cms-guide) | [zh](zh/kooboo-cms-guide) | Admin console, CMS, commerce, operations |
| `ui-ux-pro-max-pp` | [dir](ui-ux-pro-max-pp) | [zh](zh/ui-ux-pro-max-pp) | UI/UX planning and design systems |
| `debug` | [dir](debug) | [zh](zh/debug) | Troubleshooting and triage |
| `kooboo-app-i18n` | [dir](kooboo-app-i18n) | [zh](zh/kooboo-app-i18n) | Vue app internationalization via Kooboo Labels |
| `lighthouse-optimizer` | [dir](lighthouse-optimizer) | [zh](zh/lighthouse-optimizer) | Performance auditing |
| `kooboo-ai-task-automation` | [dir](kooboo-ai-task-automation) | [zh](zh/kooboo-ai-task-automation) | Scheduled / complex AI jobs |
| `history` | [dir](history) | [zh](zh/history) | Site history and undo |
| `sandbox` | [dir](sandbox) | [zh](zh/sandbox) | Sandbox evaluation |
| `browser-control` | [dir](browser-control) | [zh](zh/browser-control) | Browser interaction |
| `kooboo-jsonld` | [dir](kooboo-jsonld) | [zh](zh/kooboo-jsonld) | Structured data |

## Source

- Exported with `Kooboo/scripts/ai/package-ai.sh` (or `package-ai.bat`), which copies
  `Kooboo.Sites/AI/Skills/packages/**` into `Kooboo/packages/kooboo-ai/skills/`.
- Source revision: Kooboo `9ab1cc7b8` (branch `wg/ai-chatbox`, 2026-10-09),
  export run 2026-10-10.
- Upstream ships one flat directory per skill, bilingual **by filename**:
  `SKILL.md` + `references/` (English) and `SKILL-zh.md` + `references-zh/` (Chinese).
  1012 files in total, including `.DS_Store`.

## Layout and normalizations

The upstream filename-based locales were regrouped into folder-based locales, matching
`Docs/official-docs/`. Every Chinese file was compared against its source with the
normalizations below applied, and all 248 match exactly.

| Change | Detail |
| --- | --- |
| Moved | `SKILL-zh.md` → `zh/<skill>/SKILL.md`, `references-zh/` → `zh/<skill>/references/` |
| Removed | `SKILL-zh.md` and `references-zh/` from the English tree, so Chinese content lives only under `zh/` |
| Rewrote | the string `references-zh/` → `references/` inside the Chinese files (links and skill-root resource names), 12 files affected |
| Repointed | 3 links in `zh/ui-ux-pro-max-pp/references/page-composition.md` that point at English-only files, so they resolve to `ui-ux-pro-max-pp/references/` in the English tree |
| Copied | the 8 small asset files the Chinese docs link to (`assets/vendor.md`, `assets/app-base/*.json`, `assets/backend-templates/**`), transitively, so the mirror has no dangling internal links |
| Edited | 1 assertion in `kooboo-coding/scripts/preflight-capabilities.spec.mjs`: it compared `references-zh/` with `references/` in the same skill, and now reads the Chinese rule mirrors from `../zh/kooboo-coding/references/`. The 26 tests in that spec still pass |
| Dropped | `.DS_Store` files |

The multi-megabyte `kooboo-coding/assets/design-systems/**` runtime bundles (≈6.6 MB of
vendored JS) and the skill `scripts/` exist only in the English tree; the Chinese docs
reference them as skill-root resource names, so no copy is needed.

## Refresh

```bash
KOOB_AI=/Users/achen/Priv/Yardi/Kooboo/packages/kooboo-ai
REPO=/Users/achen/Priv/Yardi/kb-github-docs

# 1. Re-export from the Kooboo source tree
(cd /Users/achen/Priv/Yardi/Kooboo && bash scripts/ai/package-ai.sh)

# 2. English: full copy, minus .DS_Store, then remove the Chinese duplicates.
#    Exclude the zh mirror and this README so --delete cannot remove them.
rsync -a --delete --exclude='.DS_Store' --exclude='/zh' --exclude='/README.md' \
  "$KOOB_AI/skills/" "$REPO/Skills/"
find "$REPO/Skills" -path "$REPO/Skills/zh" -prune -o -name 'SKILL-zh.md' -delete
find "$REPO/Skills" -path "$REPO/Skills/zh" -prune -o -type d -name 'references-zh' -exec rm -rf {} +

# 3. Chinese mirror: copy SKILL-zh.md -> zh/<skill>/SKILL.md and
#    references-zh/ -> zh/<skill>/references/, rewrite 'references-zh/' to
#    'references/' in the copied .md files, re-copy the 8 referenced asset files,
#    repoint the 3 cross-locale links in zh/ui-ux-pro-max-pp/references/page-composition.md
#    to ../../../ui-ux-pro-max-pp/references/, re-apply the preflight spec path edit,
#    and delete .DS_Store.
```

## Known limitations

- **7 broken relative links are pre-existing upstream** inside the vendored
  `kooboo-coding/assets/design-systems/element-plus/runtime/api/*.md` bundle: it links to
  `config-provider.md` while the file is named `el-config-provider.md`. Byte-for-byte
  identical to the source, so they were left untouched. The export's 8th broken link lives in
  the Chinese `authentication-guide.md` and resolves in `zh/` under the skill-root convention
  described below.
- A few Chinese links are written as skill-root-relative resource names (for example
  `references/Scenario/commerce-customer-login-flow.md`) rather than file-relative paths,
  matching the upstream convention used with `read_skill_resource`.
- The `zh/` tree is a mirror of the Chinese **content**. If you need to deploy the Chinese
  skills into Kooboo, restore the upstream `SKILL-zh.md` / `references-zh/` names — the
  runtime `read_skill_resource` resolves those, not the mirrored ones.

Related content: [`Tools/`](../Tools) holds the AI chat's exported function definitions and
[`Docs/official-docs/`](../Docs/official-docs) the product documentation.
