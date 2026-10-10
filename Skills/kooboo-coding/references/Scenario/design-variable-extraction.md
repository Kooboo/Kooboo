# Design Variable Format and Extraction

The format rules for kooboo-var.css and the extraction/replacement workflow. The format section is the shared baseline for DesignPanel and AI site building; the extraction section describes the operating steps for analyzing an existing site.

---

# Format Rules

`kooboo-var.css` serves both as a CSS variable file usable by the browser and as a configuration source for DesignPanel.

```css
:root {
  /* @group colors */
  --color-primary: #1C1917;        /* 主色 */
  --color-cta: #CA8A04;            /* 强调色 */

  /* @group fonts */
  --font-heading: 'Cormorant', Georgia, serif;        /* 标题字体 */
  --font-body: 'Montserrat', 'Helvetica Neue', sans-serif; /* 正文字体 */
}
```

## Format Requirements

**Check each item during generation:**

| # | Requirement | Required | Description |
|---|------|------|------|
| ① | Wrapped by `:root { ... }` | Yes | Define all variables inside `:root` |
| ② | `/* @group xxx */` group marker | Yes | Write one line before each group; group keys are fixed as `colors` / `fonts` / `borderRadius` / `spacing` / `shadows` |
| ③ | Trailing `/* 变量名 */` comment on every line | **Yes** | Every declaration must end with `/* 中文或英文变量名 */`; DesignPanel uses it to render the display name |
| ④ | Variable begins with `--` | Yes | Such as `--color-primary` and `--font-heading` |

**③ is the item AI omits most easily**. Every CSS variable declaration must have a trailing comment, for example:

```css
--color-primary: #1C1917;        /* 主色 */       ← 必须有
--color-cta: #CA8A04;              /* 强调色 */      ← 必须有
```

- Group keys are fixed: `colors` / `fonts` / `borderRadius` / `spacing` / `shadows`
- Name variables according to the site's language: use Chinese names for a Chinese site, English names for an English site, and the primary content language for a mixed site

## Variable Naming

The DesignPanel variable dictionary is below. Prefer these variable names when generating `kooboo-var.css` so DesignPanel can display Chinese and English names correctly. Define custom variable names only when the dictionary has no coverage; variables outside the dictionary fall back to the comment name or key.

| Variable key | Chinese display | English display |
|----------|----------|----------|
| `--color-primary` | Primary color | Primary |
| `--color-primary-light` | Light primary | Primary Light |
| `--color-primary-dark` | Dark primary | Primary Dark |
| `--color-accent` | Accent color | Accent |
| `--color-accent-light` | Light accent | Accent Light |
| `--color-secondary` | Secondary color | Secondary |
| `--color-cta` | CTA color | CTA |
| `--color-cta-light` | Light CTA color | CTA Light |
| `--color-cta-dark` | Dark CTA color | CTA Dark |
| `--color-bg` | Background color | Background |
| `--color-text` | Text color | Text |
| `--color-text-secondary` | Secondary text color | Secondary Text |
| `--color-text-muted` | Muted text color | Muted Text |
| `--color-border` | Border color | Border |
| `--color-border-light` | Light border color | Border Light |
| `--color-surface` | Surface color | Surface |
| `--color-warning` | Warning color | Warning |
| `--color-error` | Error color | Error |
| `--color-success` | Success color | Success |
| `--font-heading` | Heading font | Heading Font |
| `--font-body` | Body font | Body Font |
| `--font-size-sm` | Small font size | Small |
| `--font-size-base` | Base font size | Base |
| `--font-weight-heading` | Heading weight | Heading Weight |
| `--radius-sm` | Small radius | Small Radius |
| `--radius-md` | Medium radius | Medium Radius |
| `--radius-lg` | Large radius | Large Radius |
| `--spacing-4` | Spacing 4 | Spacing 4 |
| `--spacing-6` | Spacing 6 | Spacing 6 |
| `--spacing-sm` | Small spacing | Small Spacing |
| `--spacing-base` | Base spacing | Base Spacing |
| `--spacing-lg` | Large spacing | Large Spacing |
| `--shadow-sm` | Small shadow | Small Shadow |
| `--shadow-base` | Base shadow | Base Shadow |
| `--shadow-lg` | Large shadow | Large Shadow |
| `--shadow-xl` | Extra-large shadow | XL Shadow |

**Important: the dictionary is a reference for “how to name”, not a list of “everything that must be generated”.** Generate only variables actually used by the site; do not write unused variables into `kooboo-var.css`. For example, if the site uses only one pink palette (primary / accent / bg / text / border), do not additionally generate unused state colors such as `--color-success` or `--color-warning`.

## Variable Selection Principles

**Variables placed in kooboo-var.css should represent brand-level design decisions**—changing the value should produce “a different brand expression” for the user.

| Category | Extraction scope | Description |
|------|------|------|
| **Color** | `--color-primary` family, `--color-accent` family, `--color-secondary`, `--color-cta`, `--color-bg`, `--color-text` family, `--color-border` family, `--color-surface`, `--color-*` state colors | Core colors adjustable through DesignPanel; colors outside the dictionary use comments as fallback |
| **Typography** | `--font-heading`, `--font-body`, `--font-size-base`, `--font-weight-heading` | Brand-font changes affect the entire site |
| **Spacing** | `--spacing-sm` / `--spacing-base` / `--spacing-lg` | Brand spacing, extracted when it does not use Tailwind's built-in scale |
| **Radius** | `--radius-sm` / `--radius-md` / `--radius-lg` | Brand corner-radius style |
| **Shadow** | `--shadow-sm` / `--shadow-base` / `--shadow-lg` / `--shadow-xl` | Brand shadow style |

**Decision test**: after changing this variable, does the user see “a different brand expression”, or has “the page layout broken”? Put the former in the variable file, not the latter.

## Tailwind First

### When a Tailwind config exists, the config is authoritative

If the site defines a Tailwind theme through `window.tailwind.config` or a `<script>`, **extract variables from the config rather than guessing from page HTML**.

**Variable-source priority:**

| Priority | Source | Extraction method |
|--------|------|----------|
| 1 | `theme.extend.colors` in the Tailwind config | Each color key → `--color-{key}` |
| 2 | `theme.extend.fontFamily` in the Tailwind config | Each font key → `--font-{key}` |
| 3 | `theme.extend.borderRadius` in the Tailwind config | Each radius key → `--radius-{key}` |
| 4 | `theme.extend.spacing` in the Tailwind config | Each spacing key → `--spacing-{key}` |
| 5 | `theme.extend.boxShadow` in the Tailwind config | Each shadow key → `--shadow-{key}` |
| 6 | Hard-coded values in page HTML | Extract only when not defined in the config |

**After extraction, the Tailwind config must reference var():**

```html
<!-- ✅ 正确：引用 CSS 变量 -->
<script>
window.tailwind.config = {
  theme: {
    extend: {
      colors: {
        primary: 'var(--color-primary)',
        cta: 'var(--color-cta)',
      },
      fontFamily: {
        heading: 'var(--font-heading)',
        body: 'var(--font-body)',
      },
    },
  },
};
</script>
```

**Mapping rule**: `--color-*` → `theme.extend.colors`, `--font-*` → `theme.extend.fontFamily`.

### When no Tailwind config exists, extract from page code

Extract brand-level variables from the Style object, `<style>` tags, or hard-coded page values. After extraction, create a `tailwind.config` that references `var(--xxx)`, and use Tailwind theme classes in page classes instead of arbitrary values.

---

# Extraction and Replacement Workflow

Analyze an existing site, extract design variables, and replace hard-coded values in the site code.

## When to Load

- Standalone task: the user asks to extract/update design variables or replace hard-coded values
- The daily-task routing table matches “design variable extraction/update”
- Step ② of the site-building flow invokes it automatically (the format section is required reading)

## Prerequisites

1. The “Format Rules” section of this document has been read.
2. Use `function_discover` to discover functions related to Style, Script, Page, View, and Layout.
3. Call `use_function` to obtain: `list_site_object` / `read_object_text` / `update_object_text` / `create_style`.

## Execution Steps

### 1. Check for a Duplicate Analysis

Check whether `/kooboo-var.css` already exists (`list_site_object` filtered to Style with a matching name). Run the complete analysis flow whether or not it exists. The existing file may be used as a reference, but the site's current actual values are authoritative.

### 2. Find Variable Sources

Search in priority order:
1. `theme.extend` in `window.tailwind.config` (authoritative when Tailwind is used)
2. CSS custom properties in the Style object (`:root { --xxx }` blocks)
3. Design-variable definitions in `<style>` tags
4. Hard-coded brand values in pages

### 3. Extract Variables

- **With a Tailwind config**: extract from colors / fontFamily / borderRadius / spacing / boxShadow and convert to `--xxx` names. Prefer keys from the “Variable Naming” dictionary.
- **Without a Tailwind config**: extract brand colors (primary / accent / cta / text / bg / border, and so on) and brand fonts (heading / body) from the page, preferring dictionary keys.
- **Hard-coded values**: extract only brand-level color/font values that appear multiple times; use comments as the fallback for variable names outside the dictionary.

### 4. Write kooboo-var.css

Generate content according to the “Format Requirements” above and call `create_style`:

| Parameter | Value |
|------|-----|
| `name` | `"kooboo-var.css"` |
| `url` | `"/kooboo-var.css"` |
| `content` | CSS generated according to the format rules |

### 5. Replace Hard-Coded Values

Replace hard-coded values corresponding to defined variables in Page, View, Layout, Style, and Script objects. **Replace only** values already defined in `kooboo-var.css`; leave non-matching values unchanged.

| Context | Replacement | Example |
|--------|----------|------|
| Tailwind arbitrary values in a class | Tailwind theme class | `text-[#0C4A6E]` → `text-primary` |
| Value in a style attribute or CSS | `var(--xxx)` | `color: #0C4A6E` → `color: var(--color-primary)` |

- Replace longer values before shorter values to avoid a short value matching the prefix of a longer value;
- Skip values that have already been replaced;
- When the same value appears in multiple places, ensure that all occurrences are replaced.

### 6. Complete

Call `ai_task_finish` with Message: `写入 /kooboo-var.css（X 个变量），替换 Y 个文件 Z 处硬编码`。

## Related Documents

- [Site Building Entry Flow](site-building/index.md)
- [Frontend Object Creation](../Functions/frontend-create.md)
