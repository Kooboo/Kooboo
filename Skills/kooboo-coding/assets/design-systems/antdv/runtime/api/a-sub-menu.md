# ASubMenu

Ant Design Vue 4.2.6; native export `SubMenu`. Import `ASubMenu` from `/antdv.js` and register locally. Native props, slots and events are unchanged.

Shared usage: [runtime guide](../usage.md).

## Props and defaults



## Events

No events listed in the pinned upstream metadata.

## Slots

Use the native default slot where supported; no named slots listed in the pinned metadata.

## Executable example

See `runtime/examples.js` for setup and registrations. This composition uses the native parent/child contract.

```html
<AMenu v-model:selectedKeys="selectedKeys" mode="inline" :items="menuItems" style="max-width:260px"/><p>{{ selectedKeys }}</p>
```

Source: pinned upstream es/components.js, es/menu, vetur/web-types.json. Acceptance scope: [runtime guide](../usage.md).
