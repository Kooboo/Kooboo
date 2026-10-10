# AWeekPicker

Ant Design Vue 4.2.6; native export `WeekPicker`. Import `AWeekPicker` from `/antdv.js` and register locally. Native props, slots and events are unchanged.

Default values are Dayjs objects. Prefer valueFormat="YYYY-MM-DD" for API strings; format is display only. RangePicker binds an array. Pick one local/UTC policy. Set ConfigProvider locale and dayjs.locale separately. Month/Week/QuarterPicker are native variants.

## Props and defaults



## Events

No events listed in the pinned upstream metadata.

## Slots

Use the native default slot where supported; no named slots listed in the pinned metadata.

## Executable example

See `runtime/examples.js` for setup and registrations. This composition uses the native parent/child contract.

```html
<ASpace wrap><ADatePicker v-model:value="dateText" value-format="YYYY-MM-DD"/><ARangePicker v-model:value="range" value-format="YYYY-MM-DD"/></ASpace><p>{{ dateText }} {{ range }}</p>
```

Source: pinned upstream es/components.js, es/date-picker, vetur/web-types.json. Acceptance scope: [runtime guide](../usage.md).
