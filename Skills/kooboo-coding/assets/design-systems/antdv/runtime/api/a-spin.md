# ASpin

Ant Design Vue 4.2.6; native export `Spin`. Import `ASpin` from `/antdv.js` and register locally. Native props, slots and events are unchanged.

Shared usage: [runtime guide](../usage.md).

## Props and defaults

- `delay`: number (milliseconds); default -. specifies a delay in milliseconds for loading state (prevent flush)
- `indicator`: vNode |slot; default -. vue node of the spinning indicator
- `size`: string; default `default`. size of Spin, options: `small`, `default` and `large`
- `spinning`: boolean; default true. whether Spin is visible
- `tip`: string | slot; default -. customize description content when Spin has children
- `wrapperClassName`: string; default -. className of wrapper when Spin has children

## Events

No events listed in the pinned upstream metadata.

## Slots

- `indicator`: vue node of the spinning indicator
- `tip`: customize description content when Spin has children

## Executable example

See `runtime/examples.js` for setup and registrations. This composition uses the native parent/child contract.

```html
<ASwitch v-model:checked="busy"/><ASpin :spinning="busy" tip="加载中"><ACard>内容区</ACard></ASpin>
```

Source: pinned upstream es/components.js, es/spin, vetur/web-types.json. Acceptance scope: [runtime guide](../usage.md).
