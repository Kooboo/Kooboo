# ACalendar

Ant Design Vue 4.2.6; native export `Calendar`. Import `ACalendar` from `/antdv.js` and register locally. Native props, slots and events are unchanged.

Shared usage: [runtime guide](../usage.md).

## Props and defaults

- `dateCellRender`: v-slot:dateCellRender="{current: dayjs}"; default -. Customize the display of the date cell by setting a scoped slot, the returned content will be appended to the cell
- `dateFullCellRender`: v-slot:dateFullCellRender="{current: dayjs}"; default -. Customize the display of the date cell by setting a scoped slot, the returned content will override the cell
- `disabledDate`: (currentDate: dayjs) => boolean; default -. Function that specifies the dates that cannot be selected
- `fullscreen`: boolean; default `true`. Whether to display in full-screen
- `headerRender`: v-slot:headerRender="{value: dayjs, type: string, onChange: f(), onTypeChange: f()}"; default -. render custom header in panel
- `locale`: object; default [default](https://github.com/vueComponent/ant-design-vue/blob/main/components/date-picker/locale/example.json). The calendar's locale
- `mode`: `month` | `year`; default `month`. The display mode of the calendar
- `monthCellRender`: v-slot:monthCellRender="{current: dayjs}"; default -. Customize the display of the month cell by setting a scoped slot, the returned content will be appended to the cell
- `monthFullCellRender`: v-slot:monthFullCellRender="{current: dayjs}"; default -. Customize the display of the month cell by setting a scoped slot, the returned content will override the cell
- `validRange`: [[dayjs](https://day.js.org/), [dayjs](https://day.js.org/)]; default -. to set valid range
- `value(v-model)`: [dayjs](https://day.js.org/); default current date. The current selected date
- `valueFormat`: string, [date formats](https://day.js.org/docs/en/display/format); default -. optional, format of binding value. If not specified, the binding value will be a Date object

## Events

- `change`: Callback for when value change
- `panelChange`: Callback for when panel changes
- `select`: Callback for when a date is selected, include source info

## Slots

- `dateCellRender`: Customize the display of the date cell by setting a scoped slot, the returned content will be appended to the cell
- `dateFullCellRender`: Customize the display of the date cell by setting a scoped slot, the returned content will override the cell
- `headerRender`: render custom header in panel
- `monthCellRender`: Customize the display of the month cell by setting a scoped slot, the returned content will be appended to the cell
- `monthFullCellRender`: Customize the display of the month cell by setting a scoped slot, the returned content will override the cell

## Executable example

See `runtime/examples.js` for setup and registrations. This composition uses the native parent/child contract.

```html
<ACalendar v-model:value="date" :fullscreen="false"/><p>{{ date.format("YYYY-MM-DD") }}</p>
```

Source: pinned upstream es/components.js, es/calendar, vetur/web-types.json. Acceptance scope: [runtime guide](../usage.md).
