# ElAffix

Fix the element to a specific visible area.

Kind: component. Import `ElAffix` from `/element-plus.js`. Shared setup and state ownership: [runtime guide](../usage.md).

Upstream reference: https://element-plus.org/en-US/component/affix.html#affix

## Props

| Name | Type | Default | Meaning |
| --- | --- | --- | --- |
| offset | number | 0 | offset distance |
| position | 'top' \| 'bottom' | top | position of affix |
| target | string | not declared | target container (CSS selector) |
| z-index | number | 100 | `z-index` of affix |
| teleported | boolean | false | whether affix element is teleported, if `true` it will be teleported to where `append-to` sets |
| append-to | CSSSelector / HTMLElement | body | which element the affix element appends to |

## Events

- `change`: triggers when fixed state changed
- `scroll`: triggers when scrolling

## Slots

- `default`: customize default content
