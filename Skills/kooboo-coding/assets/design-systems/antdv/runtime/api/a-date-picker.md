# ADatePicker

Ant Design Vue 4.2.6; native export `DatePicker`. Import `ADatePicker` from `/antdv.js` and register locally. Native props, slots and events are unchanged.

Default values are Dayjs objects. Prefer valueFormat="YYYY-MM-DD" for API strings; format is display only. RangePicker binds an array. Pick one local/UTC policy. Set ConfigProvider locale and dayjs.locale separately. Month/Week/QuarterPicker are native variants.

## Props and defaults

- `allowClear`: boolean; default true. Whether to show clear button
- `autofocus`: boolean; default false. If get focus when component mounted
- `bordered`: boolean; default true. Whether has border style
- `dateRender`: v-slot:dateRender="{current, today}"; default -. Custom rendering function for date cells
- `disabled`: boolean; default false. Determine whether the DatePicker is disabled
- `disabledDate`: (currentDate: dayjs) => boolean; default -. Specify the date that cannot be selected
- `format`: [formatType](#formattype); default `YYYY-MM-DD`. To set the date format, refer to [dayjs](https://day.js.org/). When an array is provided, all values are used for parsing and first value is used for formatting, support [Custom Format](#components-date-picker-demo-format)
- `popupClassName`: string; default -. To customize the className of the popup calendar
- `getPopupContainer`: function(trigger); default -. To set the container of the floating layer, while the default is to create a `div` element in `body`
- `inputReadOnly`: boolean; default false. Set the `readonly` attribute of the input tag (avoids virtual keyboard on touch devices)
- `locale`: object; default [default](https://github.com/vueComponent/ant-design-vue/blob/main/components/date-picker/locale/example.json). Localization configuration
- `mode`: `time` | `date` | `month` | `year` | `decade`; default -. The picker panel mode
- `nextIcon`: slot; default -. The custom next icon
- `open`: boolean; default -. The open state of picker
- `picker`: `date` | `week` | `month` | `quarter` | `year`; default `date`. Set picker type
- `placeholder`: string | [string,string]; default -. The placeholder of date input
- `placement`: `bottomLeft` `bottomRight` `topLeft` `topRight`; default bottomLeft. The position where the selection box pops up
- `popupStyle`: CSSProperties; default {}. To customize the style of the popup calendar
- `presets`: { label: slot, value: [dayjs](https://day.js.org/) }[]; default -. The preset ranges for quick selection
- `prevIcon`: slot; default -. The custom prev icon
- `size`: `large` | `middle` | `small`; default -. To determine the size of the input box, the height of `large` and `small`, are 40px and 24px respectively, while default size is 32px
- `status`: 'error' | 'warning'; default -. Set validation status
- `suffixIcon`: v-slot:suffixIcon; default -. The custom suffix icon
- `superNextIcon`: slot; default -. The custom super next icon
- `superPrevIcon`: slot; default -. The custom super prev icon
- `valueFormat`: string, [date formats](https://day.js.org/docs/en/display/format); default -. optional, format of binding value. If not specified, the binding value will be a Date object

## Events

No events listed in the pinned upstream metadata.

## Slots

- `dateRender`: Custom rendering function for date cells
- `nextIcon`: The custom next icon
- `presets`: The preset ranges for quick selection
- `prevIcon`: The custom prev icon
- `suffixIcon`: The custom suffix icon
- `superNextIcon`: The custom super next icon
- `superPrevIcon`: The custom super prev icon

## Executable example

See `runtime/examples.js` for setup and registrations. This composition uses the native parent/child contract.

```html
<ASpace wrap><ADatePicker v-model:value="dateText" value-format="YYYY-MM-DD"/><ARangePicker v-model:value="range" value-format="YYYY-MM-DD"/></ASpace><p>{{ dateText }} {{ range }}</p>
```

Source: pinned upstream es/components.js, es/date-picker, vetur/web-types.json. Acceptance scope: [runtime guide](../usage.md).
