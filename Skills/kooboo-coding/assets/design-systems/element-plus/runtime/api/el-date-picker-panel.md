# ElDatePickerPanel

`DatePickerPanel` is the core component of `DatePicker`.

Kind: component. Import `ElDatePickerPanel` from `/element-plus.js`. Shared setup and state ownership: [runtime guide](../usage.md).

Upstream reference: https://element-plus.org/en-US/component/date-picker-panel.html#datepickerpanel

## Props

| Name | Type | Default | Meaning |
| --- | --- | --- | --- |
| model-value | number / string / Date / number[] / string[] / Date[] | '' | binding value, if it is an `range` picker, the length of the array should be 2 |
| border | boolean | true | whether the date picker is bordered |
| disabled | boolean | false | whether DatePicker is disabled |
| clearable | boolean | true | whether to show clear button |
| editable | boolean | true | whether the input is editable |
| type | 'year' \| 'years' \|'month' \| 'months' \| 'date' \| 'dates' \| 'datetime' \| 'week' \| 'quarter' \| 'quarters' \| 'datetimerange' \| 'daterange' \| 'monthrange' \| 'yearrange' \| 'quarterrange' | date | type of the picker. `quarter`, `quarters`, and `quarterrange` are supported since |
| default-value | Date / [Date, Date] | not declared | optional, default date of the calendar |
| default-time | Date / [Date, Date] | not declared | optional, the time value to use when selecting date range |
| value-format | string | not declared | optional, format of binding value. If not specified, the binding value will be a Date object |
| date-format | string see | YYYY-MM-DD | optional, format of the date displayed in input's inner panel |
| time-format | string see | HH:mm:ss | optional, format of the time displayed in input's inner panel |
| unlink-panels | boolean | false | unlink two date-panels in range-picker |
| single-panel | boolean | false | show only one panel in range-picker |
| disabled-date | (data: Date) => boolean | not declared | a function determining if a date is disabled with that date as its parameter. Should return a Boolean |
| shortcuts | Array<{ text: string, value: Date \| Function }> | [] | an object array to set shortcut options |
| cell-class-name | (data: Date) => string | not declared | set custom className |
| show-footer | boolean | false | whether to show footer where the date picker is one ^[enum]`'dates' \| 'months' \| 'years' \| 'quarters' \| 'datetime' \| 'datetimerange'` |
| show-confirm | boolean | false | whether to show the confirm button |
| show-week-number | boolean | false | show the week number besides the week |

## Events

- `calendar-change`: triggers when the calendar selected date is changed. Only for `range`
- `panel-change`: triggers when the navigation button click.
- `clear`: triggers when a clear button is clicked

## Slots

- `default`: custom cell content
- `prev-month`: prev month icon
- `next-month`: next month icon
- `prev-year`: prev year icon
- `next-year`: next year icon
