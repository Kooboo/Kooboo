# AResult

Ant Design Vue 4.2.6; native export `Result`. Import `AResult` from `/antdv.js` and register locally. Native props, slots and events are unchanged.

Shared usage: [runtime guide](../usage.md).

## Props and defaults

- `extra`: slot; default -. operating area
- `icon`: slot; default -. custom back icon
- `status`: `success` | `error` | `info` | `warning` | `404` | `403` | `500`; default 'info'. result status,decide icons and colors
- `subTitle`: string | VNode | slot; default -. subTitle string
- `title`: string | VNode | slot; default -. title string

## Events

No events listed in the pinned upstream metadata.

## Slots

- `extra`: operating area
- `icon`: custom back icon
- `subTitle`: subTitle string
- `title`: title string

## Executable example

See `runtime/examples.js` for setup and registrations. This composition uses the native parent/child contract.

```html
<AResult status="success" title="提交成功" sub-title="订单已进入处理流程"><template #extra><AButton @click="count++">查看订单 {{ count }}</AButton></template></AResult>
```

Source: pinned upstream es/components.js, es/result, vetur/web-types.json. Acceptance scope: [runtime guide](../usage.md).
