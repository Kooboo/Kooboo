# ABreadcrumbItem

Ant Design Vue 4.2.6; native export `BreadcrumbItem`. Import `ABreadcrumbItem` from `/antdv.js` and register locally. Native props, slots and events are unchanged.

Shared usage: [runtime guide](../usage.md).

## Props and defaults

- `href`: string; default -. Target of hyperlink
- `overlay`: [Menu](/components/menu) | () => Menu; default -. The dropdown menu

## Events

No events listed in the pinned upstream metadata.

## Slots

Use the native default slot where supported; no named slots listed in the pinned metadata.

## Executable example

See `runtime/examples.js` for setup and registrations. This composition uses the native parent/child contract.

```html
<ABreadcrumb><ABreadcrumbItem>首页</ABreadcrumbItem><ABreadcrumbItem>客户</ABreadcrumbItem></ABreadcrumb>
```

Source: pinned upstream es/components.js, es/breadcrumb, vetur/web-types.json. Acceptance scope: [runtime guide](../usage.md).
