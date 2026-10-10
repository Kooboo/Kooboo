# AListItemMeta

Ant Design Vue 4.2.6; native export `ListItemMeta`. Import `AListItemMeta` from `/antdv.js` and register locally. Native props, slots and events are unchanged.

Shared usage: [runtime guide](../usage.md).

## Props and defaults

- `avatar`: slot; default -. The avatar of list item
- `description`: string|slot; default -. The description of list item
- `title`: string|slot; default -. The title of list item

## Events

No events listed in the pinned upstream metadata.

## Slots

- `avatar`: The avatar of list item
- `description`: The description of list item
- `title`: The title of list item

## Executable example

See `runtime/examples.js` for setup and registrations. This composition uses the native parent/child contract.

```html
<AList :data-source="rows" bordered><template #renderItem="{item}"><AListItem><AListItemMeta :title="item.name" :description="item.status"/></AListItem></template></AList>
```

Source: pinned upstream es/components.js, es/list, vetur/web-types.json. Acceptance scope: [runtime guide](../usage.md).
