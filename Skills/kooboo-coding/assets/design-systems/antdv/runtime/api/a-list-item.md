# AListItem

Ant Design Vue 4.2.6; native export `ListItem`. Import `AListItem` from `/antdv.js` and register locally. Native props, slots and events are unchanged.

Shared usage: [runtime guide](../usage.md).

## Props and defaults

- `actions`: vNode[] |slot; default -. The actions content of list item. If `itemLayout` is `vertical`, shows the content on bottom, otherwise shows content on the far right.
- `extra`: string|slot; default -. The extra content of list item. If `itemLayout` is `vertical`, shows the content on right, otherwise shows content on the far right.

## Events

No events listed in the pinned upstream metadata.

## Slots

- `actions`: The actions content of list item. If `itemLayout` is `vertical`, shows the content on bottom, otherwise shows content on the far right.
- `extra`: The extra content of list item. If `itemLayout` is `vertical`, shows the content on right, otherwise shows content on the far right.

## Executable example

See `runtime/examples.js` for setup and registrations. This composition uses the native parent/child contract.

```html
<AList :data-source="rows" bordered><template #renderItem="{item}"><AListItem><AListItemMeta :title="item.name" :description="item.status"/></AListItem></template></AList>
```

Source: pinned upstream es/components.js, es/list, vetur/web-types.json. Acceptance scope: [runtime guide](../usage.md).
