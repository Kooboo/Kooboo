# ADirectoryTree

Ant Design Vue 4.2.6; native export `DirectoryTree`. Import `ADirectoryTree` from `/antdv.js` and register locally. Native props, slots and events are unchanged.

Shared usage: [runtime guide](../usage.md).

## Props and defaults

- `expandAction`: string; default click. Directory open logic, optional `false` `'click'` `'dblclick'`

## Events

No events listed in the pinned upstream metadata.

## Slots

Use the native default slot where supported; no named slots listed in the pinned metadata.

## Executable example

See `runtime/examples.js` for setup and registrations. This composition uses the native parent/child contract.

```html
<ATree v-model:selectedKeys="selectedKeys" v-model:checkedKeys="treeChecked" :tree-data="treeData" checkable default-expand-all/><p>{{ selectedKeys }} {{ treeChecked }}</p>
```

Source: pinned upstream es/components.js, es/tree, vetur/web-types.json. Acceptance scope: [runtime guide](../usage.md).
