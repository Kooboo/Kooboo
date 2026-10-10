# AAffix

Ant Design Vue 4.2.6; native export `Affix`. Import `AAffix` from `/antdv.js` and register locally. Native props, slots and events are unchanged.

Shared usage: [runtime guide](../usage.md).

## Props and defaults

- `offsetBottom`: number; default -. Offset from the bottom of the viewport (in pixels)
- `offsetTop`: number; default 0. Offset from the top of the viewport (in pixels)
- `target`: () => HTMLElement; default () => window. Specifies the scrollable area DOM node

## Events

- `change`: Callback for when Affix state is changed

## Slots

Use the native default slot where supported; no named slots listed in the pinned metadata.

## Executable example

See `runtime/examples.js` for setup and registrations. This composition uses the native parent/child contract.

```html
<AAffix :offset-top="8"><AButton>固定操作</AButton></AAffix>
```

Source: pinned upstream es/components.js, es/affix, vetur/web-types.json. Acceptance scope: [runtime guide](../usage.md).
