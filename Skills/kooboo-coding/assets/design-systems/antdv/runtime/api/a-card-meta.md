# ACardMeta

Ant Design Vue 4.2.6; native export `CardMeta`. Import `ACardMeta` from `/antdv.js` and register locally. Native props, slots and events are unchanged.

Shared usage: [runtime guide](../usage.md).

## Props and defaults

- `avatar`: slot; default -. avatar or icon
- `description`: string|slot; default -. description content
- `title`: string|slot; default -. title content

## Events

No events listed in the pinned upstream metadata.

## Slots

- `avatar`: avatar or icon
- `description`: description content
- `title`: title content

## Executable example

See `runtime/examples.js` for setup and registrations. This composition uses the native parent/child contract.

```html
<ACard title="项目" style="max-width:400px"><template #extra><AButton type="link" @click="count++">操作 {{ count }}</AButton></template><ACardMeta title="订单服务" description="本月运行状态"/><ACardGrid style="width:50%">正常</ACardGrid><ACardGrid style="width:50%">待处理</ACardGrid></ACard>
```

Source: pinned upstream es/components.js, es/card, vetur/web-types.json. Acceptance scope: [runtime guide](../usage.md).
