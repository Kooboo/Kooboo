# ARadioGroup

Ant Design Vue 4.2.6; native export `RadioGroup`. Import `ARadioGroup` from `/antdv.js` and register locally. Native props, slots and events are unchanged.

Shared usage: [runtime guide](../usage.md).

## Props and defaults

- `buttonStyle`: `outline` | `solid`; default `outline`. style type of radio button
- `disabled`: boolean; default false. Disable all radio buttons
- `name`: string; default -. The `name` property of all `input[type="radio"]` children
- `options`: string[] | number[] | Array&lt;{ label: string value: string disabled?: boolean }>; default -. set children optional
- `optionType`: `default` | `button`; default `default`. Set Radio optionType
- `size`: `large` | `default` | `small`; default `default`. size for radio button style
- `value(v-model)`: any; default -. Used for setting the currently selected value.
- `change`: Function(e:Event); default unspecified. The callback function that is triggered when the state changes.

## Events

No events listed in the pinned upstream metadata.

## Slots

Use the native default slot where supported; no named slots listed in the pinned metadata.

## Executable example

See `runtime/examples.js` for setup and registrations. This composition uses the native parent/child contract.

```html
<ARadioGroup v-model:value="choice" :options="options"/><ARadioGroup v-model:value="choice"><ARadioButton value="杭州">杭州</ARadioButton><ARadioButton value="上海">上海</ARadioButton></ARadioGroup><p>{{ choice }}</p>
```

Source: pinned upstream es/components.js, es/radio, vetur/web-types.json. Acceptance scope: [runtime guide](../usage.md).
