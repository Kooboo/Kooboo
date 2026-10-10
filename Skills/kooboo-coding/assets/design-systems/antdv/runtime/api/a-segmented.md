# ASegmented

Ant Design Vue 4.2.6; native export `Segmented`. Import `ASegmented` from `/antdv.js` and register locally. Native props, slots and events are unchanged.

Shared usage: [runtime guide](../usage.md).

## Props and defaults

- `block`: boolean; default false. Option to fit width to its parent\'s width
- `disabled`: boolean; default false. Disable all segments
- `options`: string[] | number[] | SegmentedOption[]; default []. Set children optional
- `size`: `large` | `middle` | `small`; default -. The size of the Segmented.
- `value`: string | number; default unspecified. Currently selected value
- `label`: v-slot:label="SegmentedBaseOption"; default unspecified. custom label by slot

## Events

- `change`: The callback function that is triggered when the state changes

## Slots

- `label`: custom label by slot

## Executable example

See `runtime/examples.js` for setup and registrations. This composition uses the native parent/child contract.

```html
<ASegmented v-model:value="choice" :options="['杭州','上海']"/><p>{{ choice }}</p>
```

Source: pinned upstream es/components.js, es/segmented, vetur/web-types.json. Acceptance scope: [runtime guide](../usage.md).
