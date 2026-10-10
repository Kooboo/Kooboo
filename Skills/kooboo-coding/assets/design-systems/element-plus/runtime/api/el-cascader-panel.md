# ElCascaderPanel

ElCascaderPanel: native Element Plus component.

Kind: component. Import `ElCascaderPanel` from `/element-plus.js`. Shared setup and state ownership: [runtime guide](../usage.md).

Upstream reference: https://element-plus.org/en-US/component/cascader.html#cascaderpanel

## Props

| Name | Type | Default | Meaning |
| --- | --- | --- | --- |
| model-value | string / number / string[] / number[] / any | not declared | binding value |
| options | CascaderOption[] | not declared | data of the options, the key of `value` and `label` can be customize by `CascaderProps`. |
| [props] | CascaderProps | not declared | configuration options, see the following `CascaderProps` table. |
| virtual-scroll | boolean | false | whether to enable virtual scrolling for large data |
| item-size | number | 34 | node height for virtual scrolling (px) |
| height | number | 204 | menu height for virtual scrolling (px) |

## Events

- `change`: triggers when the binding value changes
- `update:model-value`: triggers when the binding value changes
- `expand-change`: triggers when expand option changes
- `close`: close panel event, provided to Cascader to put away the panel judgment.

## Slots

- `default`: the custom content of cascader node, which are current Node object and node data respectively.
- `empty`: the content of the panel when there is no data.
