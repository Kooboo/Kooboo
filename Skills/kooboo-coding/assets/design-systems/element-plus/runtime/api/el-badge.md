# ElBadge

A number or status mark on buttons and icons.

Kind: component. Import `ElBadge` from `/element-plus.js`. Shared setup and state ownership: [runtime guide](../usage.md).

Upstream reference: https://element-plus.org/en-US/component/badge.html#badge

## Props

| Name | Type | Default | Meaning |
| --- | --- | --- | --- |
| value | string / number | '' | display value. |
| max | number | 99 | maximum value, shows `{max}+` when exceeded. Only works if value is a number. |
| is-dot | boolean | false | if a little dot is displayed. |
| hidden | boolean | false | hidden badge. |
| type | 'primary' \| 'success' \| 'warning' \| 'danger' \| 'info' | danger | badge type. |
| show-zero | boolean | true | Whether to show badge when value is zero. |
| color | string | not declared | background color of the dot |
| offset | [number, number] | [0, 0] | offset of badge |
| badge-style | CSSProperties | not declared | custom style of badge |
| badge-class | string | not declared | custom class of badge |

## Slots

- `default`: customize default content
- `content`: customize badge content
