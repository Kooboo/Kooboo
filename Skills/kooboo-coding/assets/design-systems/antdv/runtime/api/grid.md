# Grid

Ant Design Vue 4.2.6; native export `Grid`. Import `Grid` from `/antdv.js` and register locally. Native props, slots and events are unchanged.

Grid is a utility namespace, not a component. Call Grid.useBreakpoint() in setup; it returns a ref of responsive breakpoint flags.

## Props and defaults



## Events

No events listed in the pinned upstream metadata.

## Slots

Use the native default slot where supported; no named slots listed in the pinned metadata.

## Executable example

See `runtime/examples.js` for setup and registrations. This composition uses the native parent/child contract.

```html
<p>响应式断点：{{ screens }}</p><ARow><ACol :xs="24" :md="12">宽屏半列，窄屏整列</ACol></ARow>
```

Source: pinned upstream es/components.js, es/grid, vetur/web-types.json. Acceptance scope: [runtime guide](../usage.md).
