# ATimeRangePicker

Ant Design Vue 4.2.6; native export `TimeRangePicker`. Import `ATimeRangePicker` from `/antdv.js` and register locally. Native props, slots and events are unchanged.

Use valueFormat="HH:mm:ss" for plain strings; otherwise values are Dayjs. Range values are pairs. Specify timezone at the business boundary.

## Props and defaults



## Events

No events listed in the pinned upstream metadata.

## Slots

Use the native default slot where supported; no named slots listed in the pinned metadata.

## Executable example

See `runtime/examples.js` for setup and registrations. This composition uses the native parent/child contract.

```html
<ATimePicker v-model:value="timeText" value-format="HH:mm:ss"/><ATimeRangePicker v-model:value="times" value-format="HH:mm:ss"/><p>{{ timeText }} {{ times }}</p>
```

Source: pinned upstream es/components.js, es/time-picker, vetur/web-types.json. Acceptance scope: [runtime guide](../usage.md).
