# AMenuItem

Ant Design Vue 4.2.6; native export `MenuItem`. Import `AMenuItem` from `/antdv.js` and register locally. Native props, slots and events are unchanged.

Shared usage: [runtime guide](../usage.md).

## Props and defaults

- `disabled`: boolean; default false. whether menu item is disabled or not
- `key`: string | number; default unspecified. unique id of the menu item
- `title`: string | slot; default unspecified. set display title for collapsed item

## Events

No events listed in the pinned upstream metadata.

## Slots

- `title`: set display title for collapsed item

## Executable example

See `runtime/examples.js` for setup and registrations. This composition uses the native parent/child contract.

```html
<AMenu v-model:selectedKeys="selectedKeys" mode="inline" :items="menuItems" style="max-width:260px"/><p>{{ selectedKeys }}</p>
```

Source: pinned upstream es/components.js, es/menu, vetur/web-types.json. Acceptance scope: [runtime guide](../usage.md).
