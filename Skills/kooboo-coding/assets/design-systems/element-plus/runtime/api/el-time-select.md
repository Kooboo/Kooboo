# ElTimeSelect

Use Time Select for time input.

Kind: component. Import `ElTimeSelect` from `/element-plus.js`. Shared setup and state ownership: [runtime guide](../usage.md).

Upstream reference: https://element-plus.org/en-US/component/time-select.html#timeselect

## Props

| Name | Type | Default | Meaning |
| --- | --- | --- | --- |
| model-value | string | not declared | binding value |
| disabled | boolean | false | whether TimeSelect is disabled |
| editable | boolean | true | whether the input is editable |
| clearable | boolean | true | whether to show clear button |
| include-end-time | boolean | false | whether `end` is included in options |
| size | 'large' \| 'default' \| 'small' | default | size of Input |
| placeholder | string | not declared | placeholder in non-range mode |
| name | string | not declared | same as `name` in native input |
| effect | string / 'dark' \| 'light' | light | Tooltip theme, built-in theme: `dark` / `light` |
| prefix-icon | string / Component | Clock | custom prefix icon component |
| clear-icon | string / Component | CircleClose | custom clear icon component |
| start | string | 09:00 | start time |
| end | string | 18:00 | end time |
| step | string | 00:30 | time step |
| min-time | string | not declared | minimum time, any time before this time will be disabled |
| max-time | string | not declared | maximum time, any time after this time will be disabled |
| format | string see | HH:mm | set format of time |
| empty-values | array | not declared | empty values of component, [see config-provider](./config-provider.md#empty-values-configurations) |
| value-on-clear | string / number / boolean / Function | not declared | clear return value, [see config-provider](./config-provider.md#empty-values-configurations) |
| popper-class | string | '' | custom class name for TimeSelect's dropdown |
| popper-style | string / object | not declared | custom style for TimeSelect's dropdown |

## Events

- `change`: triggers when user confirms the value
- `blur`: triggers when Input blurs
- `focus`: triggers when Input focuses
- `clear`: triggers when the clear icon is clicked in a clearable TimeSelect
