# ABadgeRibbon

Ant Design Vue 4.2.6; native export `BadgeRibbon`. Import `ABadgeRibbon` from `/antdv.js` and register locally. Native props, slots and events are unchanged.

Shared usage: [runtime guide](../usage.md).

## Props and defaults



## Events

No events listed in the pinned upstream metadata.

## Slots

Use the native default slot where supported; no named slots listed in the pinned metadata.

## Executable example

See `runtime/examples.js` for setup and registrations. This composition uses the native parent/child contract.

```html
<ABadge :count="count"><AAvatar shape="square">信</AAvatar></ABadge><AButton @click="count++">增加未读</AButton><ABadgeRibbon text="新品"><ACard title="商品">商品说明</ACard></ABadgeRibbon>
```

Source: pinned upstream es/components.js, es/badge, vetur/web-types.json. Acceptance scope: [runtime guide](../usage.md).
