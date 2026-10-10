# ElCol

ElCol: native Element Plus component.

Kind: component. Import `ElCol` from `/element-plus.js`. Shared setup and state ownership: [runtime guide](../usage.md).

Upstream reference: https://element-plus.org/en-US/component/layout.html#col

## Props

| Name | Type | Default | Meaning |
| --- | --- | --- | --- |
| span | number | 24 | number of column the grid spans |
| offset | number | 0 | number of spacing on the left side of the grid |
| push | number | 0 | number of columns that grid moves to the right |
| pull | number | 0 | number of columns that grid moves to the left |
| xs | number / {span?: number, offset?: number, pull?: number, push?: number} | not declared | `<768px` Responsive columns or column props object |
| sm | number / {span?: number, offset?: number, pull?: number, push?: number} | not declared | `≥768px` Responsive columns or column props object |
| md | number / {span?: number, offset?: number, pull?: number, push?: number} | not declared | `≥992px` Responsive columns or column props object |
| lg | number / {span?: number, offset?: number, pull?: number, push?: number} | not declared | `≥1200px` Responsive columns or column props object |
| xl | number / {span?: number, offset?: number, pull?: number, push?: number} | not declared | `≥1920px` Responsive columns or column props object |
| tag | string | div | custom element tag |

## Slots

- `default`: customize default content
