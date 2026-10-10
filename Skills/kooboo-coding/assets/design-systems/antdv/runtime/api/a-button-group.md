# AButtonGroup

Ant Design Vue 4.2.6; native export `ButtonGroup`. Import `AButtonGroup` from `/antdv.js` and register locally. Native props, slots and events are unchanged.

Shared usage: [runtime guide](../usage.md).

## Props and defaults



## Events

No events listed in the pinned upstream metadata.

## Slots

Use the native default slot where supported; no named slots listed in the pinned metadata.

## Executable example

See `runtime/examples.js` for setup and registrations. This composition uses the native parent/child contract.

```html
<ASpace wrap><AButton type="primary" @click="count++">操作 {{ count }}</AButton><AButton disabled>禁用</AButton><AButton loading>加载中</AButton><AButton danger>删除</AButton></ASpace>
```

Source: pinned upstream es/components.js, es/button, vetur/web-types.json. Acceptance scope: [runtime guide](../usage.md).
