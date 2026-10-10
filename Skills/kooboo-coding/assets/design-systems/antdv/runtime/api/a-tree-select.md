# ATreeSelect

Ant Design Vue 4.2.6; native export `TreeSelect`. Import `ATreeSelect` from `/antdv.js` and register locally. Native props, slots and events are unchanged.

Shared usage: [runtime guide](../usage.md).

## Props and defaults



## Events

No events listed in the pinned upstream metadata.

## Slots

Use the native default slot where supported; no named slots listed in the pinned metadata.

## Executable example

See `runtime/examples.js` for setup and registrations. This composition uses the native parent/child contract.

```html
<ATreeSelect v-model:value="treeValue" :tree-data="treeData" style="width:240px" allow-clear tree-default-expand-all/><p>{{ treeValue }}</p>
```

Source: pinned upstream es/components.js, es/tree-select, vetur/web-types.json. Acceptance scope: [runtime guide](../usage.md).
