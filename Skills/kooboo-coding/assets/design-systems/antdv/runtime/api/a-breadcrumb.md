# ABreadcrumb

Ant Design Vue 4.2.6; native export `Breadcrumb`. Import `ABreadcrumb` from `/antdv.js` and register locally. Native props, slots and events are unchanged.

Shared usage: [runtime guide](../usage.md).

## Props and defaults

- `itemRender`: ({route, params, routes, paths}) => vNode; default unspecified. Custom item renderer, #itemRender="{route, params, routes, paths}"
- `params`: object; default unspecified. Routing parameters
- `routes`: [routes[]](#routes); default unspecified. The routing stack information of router
- `separator`: string|slot; default unspecified. Custom separator

## Events

No events listed in the pinned upstream metadata.

## Slots

- `separator`: Custom separator

## Executable example

See `runtime/examples.js` for setup and registrations. This composition uses the native parent/child contract.

```html
<ABreadcrumb><ABreadcrumbItem>首页</ABreadcrumbItem><ABreadcrumbItem>客户</ABreadcrumbItem></ABreadcrumb>
```

Source: pinned upstream es/components.js, es/breadcrumb, vetur/web-types.json. Acceptance scope: [runtime guide](../usage.md).
