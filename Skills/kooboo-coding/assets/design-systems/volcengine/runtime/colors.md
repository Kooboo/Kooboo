# Yuanli color and theme contract

Read on demand when a user requests color/brand/theme customization. Canonical source: `assets/design-systems/volcengine/runtime/tokens.css`; inspect the final declaration of repeated variables. The acceptance site exposes these tokens at `/#/colors`, including original expressions and browser-resolved values. This is a light-theme inventory, not an automatic theme generator.

## Roles and scales

- `--primary-1..8`: brand scale. Final semantic aliases: `--color-primary` → 6; hover → 5; click → 7; disabled → 3; bg → 1; tag-bg → 2.
- `--success-1..8`, `--warning-1..8`, `--danger-1..8`: independent semantic scales. Brand changes do not redefine their meaning.
- `--grey-*`, `--color-text-*`, `--color-bg-*`, `--color-border-*`: neutral text, surfaces and boundaries. Read actual existing levels; do not assume every numbered token exists.
- `--primary-yuanliBlue-*`, `--primary`, `--primary-accent`, `--primary-link`, `--primary-ring`, `--link-*`, `--state-*` and `--chart-1` also exist. Some are independent literal values rather than aliases. Changing only `--color-primary` cannot update all component variants.
- Original and normalized palettes differ (including blue levels 3/4). Preserve the source unless explicitly changing it; do not describe all source ramps as monotonic.

## App-owned customization

Keep installed `yuanli.css` intact. Put approved overrides in the App's own Style loaded after it (normally `/app.css`). For an App-wide theme, use `:root`; subtree-only variables may not reach top-layer/teleported panels. Do not overwrite account or notification composition while changing color.

A concrete starting point for a purple primary scale (not a complete theme or contrast certification):

```css
:root {
  --primary-1: #faf5ff;
  --primary-2: #f3e8ff;
  --primary-3: #d8b4fe;
  --primary-4: #c084fc;
  --primary-5: #9333ea;
  --primary-6: #7e22ce;
  --primary-7: #6b21a8;
  --primary-8: #581c87;
  --color-primary: var(--primary-6);
  --color-primary-hover: var(--primary-5);
  --color-primary-click: var(--primary-7);
  --color-primary-disabled: var(--primary-3);
  --color-primary-bg: var(--primary-1);
  --color-primary-tag-bg: var(--primary-2);
}
```

Then inspect installed component CSS for the actual primary, link, chart and state tokens it consumes. Map independent aliases intentionally, preserve text-on-color contrast, and explicitly override any relevant literal-color selectors in business CSS. Current source still contains fixed blue RGBA/hex values, so a palette-only change is partial. Do not promise a universal one-token theme API or silently rewrite all semantic colors.

Check primary/default/hover/pressed/focus/disabled, selected navigation and Tabs, input errors, badges and popovers. Normal text needs 4.5:1 contrast, large text 3:1, essential control boundaries/focus indicators 3:1 against adjacent colors. Color must have a text/icon equivalent for status. Test reduced motion separately. Dark mode requires an explicit full surface/text/border/state palette and manual acceptance; it is not obtained by changing the primary hue.

Other design systems should expose their own token source, scales, semantics, public override mechanism and limitations. Reuse this documentation shape, never Yuanli variable names by assumption.
