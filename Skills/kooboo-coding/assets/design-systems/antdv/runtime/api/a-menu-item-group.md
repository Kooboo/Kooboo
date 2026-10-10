# AMenuItemGroup

Ant Design Vue 4.2.6; native export `MenuItemGroup`. Import `AMenuItemGroup` from `/antdv.js` and register locally. Native props, slots and events are unchanged.

Shared usage: [runtime guide](../usage.md).

## Props and defaults

- `children`: MenuItem[]; default unspecified. sub-menu items
- `title`: string|slot; default unspecified. title of the group

## Events

No events listed in the pinned upstream metadata.

## Slots

- `title`: title of the group

## Executable example

See `runtime/examples.js` for setup and registrations. This composition uses the native parent/child contract.

```html
<AMenu v-model:selectedKeys="selectedKeys" mode="inline" :items="menuItems" style="max-width:260px"/><p>{{ selectedKeys }}</p>
```

Source: pinned upstream es/components.js, es/menu, vetur/web-types.json. Acceptance scope: [runtime guide](../usage.md).
