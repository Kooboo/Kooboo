# ElCalendar

Display date.

Kind: component. Import `ElCalendar` from `/element-plus.js`. Shared setup and state ownership: [runtime guide](../usage.md).

Upstream reference: https://element-plus.org/en-US/component/calendar.html#calendar

## Props

| Name | Type | Default | Meaning |
| --- | --- | --- | --- |
| model-value | Date | not declared | binding value |
| range | [Date, Date] | not declared | time range, including start time and end time. Start time must be start day of week, end time must be end day of week, the time span cannot exceed two months. |
| controller-type | 'button' \| 'select' | button | type of the controller for Calendar header |
| formatter | (value: number, type: 'year' \| 'month') => string / number | not declared | format label when `controller-type` is 'select' |

## Slots

- `date-cell`: `type` indicates which month the date belongs, optional values are prev-month, current-month, next-month; `isSelected` indicates whether the date is selected; `day` is the formatted date in the format `YYYY-MM-DD`; `date` is date the cell represents
- `header`: content of the Calendar header
