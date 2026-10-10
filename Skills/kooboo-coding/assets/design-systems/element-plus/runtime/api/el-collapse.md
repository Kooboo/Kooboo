# ElCollapse

Use Collapse to store contents.

Kind: component. Import `ElCollapse` from `/element-plus.js`. Shared setup and state ownership: [runtime guide](../usage.md).

Upstream reference: https://element-plus.org/en-US/component/collapse.html#collapse

## Props

| Name | Type | Default | Meaning |
| --- | --- | --- | --- |
| model-value | string / array | [] | currently active panel, the type is `string` in accordion mode, otherwise it is `array` |
| accordion | boolean | false | whether to activate accordion mode |
| expand-icon-position | 'left' \| 'right' | right | set expand icon position |
| before-collapse | () => Promise<boolean> / boolean | not declared | before-collapse hook before the collapse state changes. If `false` is returned or a `Promise` is returned and then is rejected, will stop collapsing |

## Events

- `change`: triggers when active panels change, the parameter type is `string` in accordion mode, otherwise it is `array`

## Slots

- `default`: customize default content
