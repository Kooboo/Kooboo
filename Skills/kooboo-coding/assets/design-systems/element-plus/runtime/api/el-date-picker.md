# ElDatePicker

Use Date Picker for date input.

Kind: component. Import `ElDatePicker` from `/element-plus.js`. Shared setup and state ownership: [runtime guide](../usage.md).

Upstream reference: https://element-plus.org/en-US/component/date-picker.html#datepicker

## Props

| Name | Type | Default | Meaning |
| --- | --- | --- | --- |
| model-value | number / string / Date / number[] / string[] / Date[] | '' | binding value, if it is an `range` picker, the length of the array should be 2 |
| readonly | boolean | false | whether DatePicker is read only |
| disabled | boolean | false | whether DatePicker is disabled |
| size | '' \| 'large' \| 'default' \| 'small' | not declared | size of Input |
| editable | boolean | true | whether the input is editable |
| clearable | boolean | true | whether to show clear button |
| placeholder | string | '' | placeholder in non-range mode |
| start-placeholder | string | not declared | placeholder for the start date in range mode |
| end-placeholder | string | not declared | placeholder for the end date in range mode |
| type | 'year' \| 'years' \|'month' \| 'months' \| 'date' \| 'dates' \| 'datetime' \| 'week' \| 'quarter' \| 'quarters' \| 'datetimerange' \| 'daterange' \| 'monthrange' \| 'yearrange' \| 'quarterrange' | date | type of the picker. `quarter`, `quarters`, and `quarterrange` are supported since |
| format | string see | YYYY-MM-DD | format of the displayed value in the input box |
| popper-class | string | not declared | custom class name for DatePicker's dropdown |
| popper-style | string / object | not declared | custom style for DatePicker's dropdown |
| popper-options | Partial<PopperOptions> | {} | Customized popper option see more at [popper.js](https://popper.js.org/docs/v2/) |
| range-separator | string | '-' | range separator |
| default-value | Date / [Date, Date] | not declared | optional, default date of the calendar |
| default-time | Date / [Date, Date] | not declared | optional, the time value to use when selecting date range |
| value-format | string see | not declared | optional, format of binding value. If not specified, the binding value will be a Date object |
| id | string / [string, string] | not declared | same as `id` in native input |
| name | string / [string, string] | '' | same as `name` in native input |
| unlink-panels | boolean | false | unlink two date-panels in range-picker |
| single-panel | boolean | false | show only one panel in range-picker |
| prefix-icon | string / Component | '' | custom prefix icon component. By default, if the value of `type` is `TimeLikeType`, the value is `Clock`, else is `Calendar` |
| clear-icon | string / Component | CircleClose | custom clear icon component |
| validate-event | boolean | true | whether to trigger form validation |
| disabled-date | (data: Date) => boolean | not declared | a function determining if a date is disabled with that date as its parameter. Should return a Boolean |
| shortcuts | Array<{ text: string, value: Date \| Function }> | [] | an object array to set shortcut options |
| cell-class-name | (data: Date) => string | not declared | set custom className |
| teleported | boolean | true | whether date-picker dropdown is teleported to the body |
| empty-values | array | not declared | empty values of component, [see config-provider](./config-provider.md#empty-values-configurations) |
| value-on-clear | string / number / boolean / Function | not declared | clear return value, [see config-provider](./config-provider.md#empty-values-configurations) |
| fallback-placements | Placement[] | ['bottom', 'top', 'right', 'left'] | list of possible positions for Tooltip [popper.js](https://popper.js.org/docs/v2/modifiers/flip/#fallbackplacements) |
| placement | Placement | bottom | position of dropdown |
| show-footer | boolean | true | whether to show footer |
| show-confirm | boolean | true | whether to show the confirm button |
| show-week-number | boolean | false | show the week number besides the week |
| automatic-dropdown | boolean | true | this prop decides if the date picker panel pops up when the input is focused. (The default value will be set to false in version 3.0) |

## Events

- `change`: triggers when user confirms the value or click outside
- `blur`: triggers when Input blurs
- `focus`: triggers when Input focuses
- `clear`: triggers when a clear button is clicked
- `calendar-change`: triggers when the calendar selected date is changed. Only for `range`
- `panel-change`: triggers when the navigation button click.
- `visible-change`: triggers when the DatePicker's dropdown appears/disappears

## Slots

- `default`: custom cell content
- `range-separator`: custom range separator content
- `prev-month`: prev month icon
- `next-month`: next month icon
- `prev-year`: prev year icon
- `next-year`: next year icon

## Usage and state ownership

Use an explicit value-format when the model should be a string, e.g. YYYY-MM-DD; display format is separate. With no value-format the model uses Date values. Range types use a pair. Do not turn a local booking time into UTC implicitly. Business owns bounds, conflict checks and invalid/empty handling. Locale comes from ElConfigProvider.

```js
export default { components: { ElDatePicker }, setup: () => ({ date: ref('') }), template: '<ElDatePicker v-model="date" type="date" value-format="YYYY-MM-DD" />' };
```
