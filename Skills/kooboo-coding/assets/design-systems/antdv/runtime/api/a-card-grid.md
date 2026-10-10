# ACardGrid

Ant Design Vue 4.2.6; native export `CardGrid`. Import `ACardGrid` from `/antdv.js` and register locally. Native props, slots and events are unchanged.

Shared usage: [runtime guide](../usage.md).

## Props and defaults



## Events

No events listed in the pinned upstream metadata.

## Slots

Use the native default slot where supported; no named slots listed in the pinned metadata.

## Executable example

See `runtime/examples.js` for setup and registrations. This composition uses the native parent/child contract.

```html
<ACard title="项目" style="max-width:400px"><template #extra><AButton type="link" @click="count++">操作 {{ count }}</AButton></template><ACardMeta title="订单服务" description="本月运行状态"/><ACardGrid style="width:50%">正常</ACardGrid><ACardGrid style="width:50%">待处理</ACardGrid></ACard>
```

Source: pinned upstream es/components.js, es/card, vetur/web-types.json. Acceptance scope: [runtime guide](../usage.md).
