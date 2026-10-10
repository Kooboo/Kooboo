# ATimePicker

Ant Design Vue 4.2.6; native export `TimePicker`. Import `ATimePicker` from `/antdv.js` and register locally. Native props, slots and events are unchanged.

Use valueFormat="HH:mm:ss" for plain strings; otherwise values are Dayjs. Range values are pairs. Specify timezone at the business boundary.

## Props and defaults

- `allowClear`: boolean; default true. Whether allow clearing text
- `autofocus`: boolean; default false. If get focus when component mounted
- `bordered`: boolean; default true. Whether has border style
- `clearIcon`: v-slot:clearIcon; default -. The custom clear icon
- `clearText`: string; default clear. The clear tooltip of icon
- `disabled`: boolean; default false. Determine whether the TimePicker is disabled
- `disabledTime`: [DisabledTime](#disabledtime); default -. To specify the time that cannot be selected
- `format`: string; default `HH:mm:ss`. To set the time format
- `getPopupContainer`: function(trigger); default -. To set the container of the floating layer, while the default is to create a div element in body
- `hideDisabledOptions`: boolean; default false. Whether hide the options that can not be selected
- `hourStep`: number; default 1. Interval between hours in picker
- `inputReadOnly`: boolean; default false. Set the `readonly` attribute of the input tag (avoids virtual keyboard on touch devices)
- `minuteStep`: number; default 1. Interval between minutes in picker
- `open(v-model)`: boolean; default false. Whether to popup panel
- `placeholder`: string | [string, string]; default `Select a time`. Display when there's no value
- `placement`: `bottomLeft` `bottomRight` `topLeft` `topRight`; default bottomLeft. The position where the selection box pops up
- `popupClassName`: string; default -. The className of panel
- `popupStyle`: CSSProperties; default -. The style of panel
- `renderExtraFooter`: v-slot:renderExtraFooter; default -. Called from time picker panel to render some addon to its bottom
- `secondStep`: number; default 1. Interval between seconds in picker
- `showNow`: boolean; default -. Whether to show `Now` button on panel
- `suffixIcon`: v-slot:suffixIcon; default -. The custom suffix icon
- `use12Hours`: boolean; default false. Display as 12 hours format, with default format `h:mm:ss a`
- `value(v-model)`: [dayjs](https://day.js.org/); default -. To set time
- `valueFormat`: string, [date formats](https://day.js.org/docs/en/display/format); default -. optional, format of binding value. If not specified, the binding value will be a Date object

## Events

- `change`: a callback function, can be executed when the selected time is changing
- `openChange`: a callback function which will be called while panel opening/closing

## Slots

- `clearIcon`: The custom clear icon
- `renderExtraFooter`: Called from time picker panel to render some addon to its bottom
- `suffixIcon`: The custom suffix icon

## Executable example

See `runtime/examples.js` for setup and registrations. This composition uses the native parent/child contract.

```html
<ATimePicker v-model:value="timeText" value-format="HH:mm:ss"/><ATimeRangePicker v-model:value="times" value-format="HH:mm:ss"/><p>{{ timeText }} {{ times }}</p>
```

Source: pinned upstream es/components.js, es/time-picker, vetur/web-types.json. Acceptance scope: [runtime guide](../usage.md).
