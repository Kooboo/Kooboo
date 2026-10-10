# ACheckableTag

Ant Design Vue 4.2.6; native export `CheckableTag`. Import `ACheckableTag` from `/antdv.js` and register locally. Native props, slots and events are unchanged.

Shared usage: [runtime guide](../usage.md).

## Props and defaults



## Events

No events listed in the pinned upstream metadata.

## Slots

Use the native default slot where supported; no named slots listed in the pinned metadata.

## Executable example

See `runtime/examples.js` for setup and registrations. This composition uses the native parent/child contract.

```html
<ASpace><ATag color="success">已完成</ATag><ATag color="warning">待处理</ATag><ATag closable>可移除</ATag><ACheckableTag v-model:checked="checked">可选 {{ checked }}</ACheckableTag></ASpace>
```

Source: pinned upstream es/components.js, es/tag, vetur/web-types.json. Acceptance scope: [runtime guide](../usage.md).
