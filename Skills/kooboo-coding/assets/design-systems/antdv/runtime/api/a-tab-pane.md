# ATabPane

Ant Design Vue 4.2.6; native export `TabPane`. Import `ATabPane` from `/antdv.js` and register locally. Native props, slots and events are unchanged.

Shared usage: [runtime guide](../usage.md).

## Props and defaults



## Events

No events listed in the pinned upstream metadata.

## Slots

Use the native default slot where supported; no named slots listed in the pinned metadata.

## Executable example

See `runtime/examples.js` for setup and registrations. This composition uses the native parent/child contract.

```html
<ATabs v-model:activeKey="tabKey"><ATabPane key="1" tab="概览">概览内容</ATabPane><ATabPane key="2" tab="历史">历史内容</ATabPane></ATabs>
```

Source: pinned upstream es/components.js, es/tabs, vetur/web-types.json. Acceptance scope: [runtime guide](../usage.md).
