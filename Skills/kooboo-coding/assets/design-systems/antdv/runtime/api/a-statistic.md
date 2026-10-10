# AStatistic

Ant Design Vue 4.2.6; native export `Statistic`. Import `AStatistic` from `/antdv.js` and register locally. Native props, slots and events are unchanged.

Shared usage: [runtime guide](../usage.md).

## Props and defaults

- `decimalSeparator`: string; default .. decimal separator
- `formatter`: v-slot |({value}) => VNode; default -. customize value display logic
- `groupSeparator`: string; default ,. group separator
- `precision`: number; default -. precision of input value
- `prefix`: string | v-slot; default -. prefix node of value
- `suffix`: string | v-slot; default -. suffix node of value
- `title`: string | v-slot; default -. Display title
- `value`: string | number; default -. Display value
- `valueStyle`: style; default -. Set value css style

## Events

No events listed in the pinned upstream metadata.

## Slots

- `formatter`: customize value display logic
- `prefix`: prefix node of value
- `suffix`: suffix node of value
- `title`: Display title

## Executable example

See `runtime/examples.js` for setup and registrations. This composition uses the native parent/child contract.

```html
<ARow><ACol :span="12"><AStatistic title="销售额" :value="12850.5" :precision="2" prefix="¥"/></ACol><ACol :span="12"><AStatisticCountdown title="活动剩余" :value="deadline"/></ACol></ARow>
```

Source: pinned upstream es/components.js, es/statistic, vetur/web-types.json. Acceptance scope: [runtime guide](../usage.md).
