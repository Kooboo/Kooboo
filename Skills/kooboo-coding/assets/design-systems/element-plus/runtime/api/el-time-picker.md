# ElTimePicker

Use Time Picker for time input.

Kind: component. Import `ElTimePicker` from `/element-plus.js`. Shared setup and state ownership: [runtime guide](../usage.md).

Upstream reference: https://element-plus.org/en-US/component/time-picker.html#timepicker

## Props

| Name | Type | Default | Meaning |
| --- | --- | --- | --- |
| model-value | number / string / Date / [Date, Date] \| [number, number] \| [string, string] | '' | binding value, if it is an array, the length should be 2 |
| readonly | boolean | false | whether TimePicker is read only |
| disabled | boolean | false | whether TimePicker is disabled |
| editable | boolean | true | whether the input is editable |
| clearable | boolean | true | whether to show clear button |
| size | 'large' \| 'default' \| 'small' | not declared | size of Input |
| placeholder | string | '' | placeholder in non-range mode |
| start-placeholder | string | not declared | placeholder for the start time in range mode |
| end-placeholder | string | not declared | placeholder for the end time in range mode |
| is-range | boolean | false | whether to pick a time range |
| arrow-control | boolean | false | whether to pick time using arrow buttons |
| popper-class | string | '' | custom class name for TimePicker's dropdown |
| popper-style | string / object | not declared | custom style for TimePicker's dropdown |
| popper-options | Partial<PopperOptions> | {} | Customized popper option see more at [popper.js](https://popper.js.org/docs/v2/) |
| fallback-placements | Placement[] | ['bottom', 'top', 'right', 'left'] | list of possible positions for Tooltip [popper.js](https://popper.js.org/docs/v2/modifiers/flip/#fallbackplacements) |
| placement | Placement | bottom | position of dropdown |
| range-separator | string | '-' | range separator |
| format | string see | not declared | format of the displayed value in the input box |
| default-value | Date / [Date, Date] | not declared | optional, default date of the calendar |
| value-format | string see | not declared | optional, format of binding value. If not specified, the binding value will be a Date object |
| id | string / [string, string] | not declared | same as `id` in native input |
| name | string | '' | same as `name` in native input |
| aria-label | string | not declared | same as `aria-label` in native input |
| prefix-icon | string / Component | Clock | Custom prefix icon component |
| clear-icon | string / Component | CircleClose | Custom clear icon component |
| disabled-hours | (role: string, comparingDate?: Dayjs) => number[] | not declared | To specify the array of hours that cannot be selected |
| disabled-minutes | (hour: number, role: string, comparingDate?: Dayjs) => number[] | not declared | To specify the array of minutes that cannot be selected |
| disabled-seconds | (hour: number, minute: number, role: string, comparingDate?: Dayjs) => number[] | not declared | To specify the array of seconds that cannot be selected |
| teleported | boolean | true | whether time-picker dropdown is teleported to the body |
| tabindex | string / number | 0 | input tabindex |
| empty-values | array | not declared | empty values of component, [see config-provider](./config-provider.md#empty-values-configurations) |
| value-on-clear | string / number / boolean / Function | not declared | clear return value, [see config-provider](./config-provider.md#empty-values-configurations) |
| save-on-blur | boolean | true | Whether to auto-fill the input with the current time on focus when no value is selected |
| label | string | not declared | same as `aria-label` in native input |

## Events

- `change`: triggers when user confirms the value
- `blur`: triggers when Input blurs
- `focus`: triggers when Input focuses
- `clear`: triggers when the clear icon is clicked in a clearable TimePicker
- `visible-change`: triggers when the TimePicker's dropdown appears/disappears
