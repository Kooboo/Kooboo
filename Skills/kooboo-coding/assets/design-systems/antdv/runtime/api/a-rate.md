# ARate

Ant Design Vue 4.2.6; native export `Rate`. Import `ARate` from `/antdv.js` and register locally. Native props, slots and events are unchanged.

Shared usage: [runtime guide](../usage.md).

## Props and defaults

- `allowClear`: boolean; default true. whether to allow clear when click again
- `allowHalf`: boolean; default false. whether to allow semi selection
- `autofocus`: boolean; default false. get focus when component mounted
- `character`: string | slot; default `<StarOutlined />`. custom character of rate
- `count`: number; default 5. star count
- `disabled`: boolean; default false. read only, unable to interact
- `tooltips`: string[]; default -. Customize tooltip by each character
- `value(v-model)`: number; default -. current value

## Events

- `blur`: callback when component lose focus
- `change`: callback when select value
- `focus`: callback when component get focus
- `hoverChange`: callback when hover item
- `keydown`: callback when keydown on component

## Slots

- `character`: custom character of rate

## Executable example

See `runtime/examples.js` for setup and registrations. This composition uses the native parent/child contract.

```html
<ARate v-model:value="rating" allow-half/><p>{{ rating }}</p>
```

Source: pinned upstream es/components.js, es/rate, vetur/web-types.json. Acceptance scope: [runtime guide](../usage.md).
