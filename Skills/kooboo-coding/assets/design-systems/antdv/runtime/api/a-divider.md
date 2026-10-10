# ADivider

Ant Design Vue 4.2.6; native export `Divider`. Import `ADivider` from `/antdv.js` and register locally. Native props, slots and events are unchanged.

Shared usage: [runtime guide](../usage.md).

## Props and defaults

- `dashed`: boolean; default false. whether line is dashed
- `orientation`: `left` | `right` | `center`; default `center`. position of title inside divider
- `orientationMargin`: string | number; default -. The margin-left/right between the title and its closest border, while the `orientation` must be `left` or `right`
- `plain`: boolean; default true. Divider text show as plain style
- `type`: `horizontal` | `vertical`; default `horizontal`. direction type of divider

## Events

No events listed in the pinned upstream metadata.

## Slots

Use the native default slot where supported; no named slots listed in the pinned metadata.

## Executable example

See `runtime/examples.js` for setup and registrations. This composition uses the native parent/child contract.

```html
<p>订单信息</p><ADivider orientation="left">收货信息</ADivider><p>杭州仓库</p>
```

Source: pinned upstream es/components.js, es/divider, vetur/web-types.json. Acceptance scope: [runtime guide](../usage.md).
