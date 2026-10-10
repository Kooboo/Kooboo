# AEmpty

Ant Design Vue 4.2.6; native export `Empty`. Import `AEmpty` from `/antdv.js` and register locally. Native props, slots and events are unchanged.

Shared usage: [runtime guide](../usage.md).

## Props and defaults

- `description`: string | v-slot; default -. Customize description
- `image`: string | v-slot; default false. Customize image. Will tread as image url when string provided
- `imageStyle`: CSSProperties; default -. style of image

## Events

No events listed in the pinned upstream metadata.

## Slots

- `description`: Customize description
- `image`: Customize image. Will tread as image url when string provided

## Executable example

See `runtime/examples.js` for setup and registrations. This composition uses the native parent/child contract.

```html
<AEmpty description="暂无订单"><AButton type="primary" @click="count++">新建 {{ count }}</AButton></AEmpty>
```

Source: pinned upstream es/components.js, es/empty, vetur/web-types.json. Acceptance scope: [runtime guide](../usage.md).
