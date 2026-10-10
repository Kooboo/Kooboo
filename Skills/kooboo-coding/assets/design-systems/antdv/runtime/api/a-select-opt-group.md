# ASelectOptGroup

Ant Design Vue 4.2.6; native export `SelectOptGroup`. Import `ASelectOptGroup` from `/antdv.js` and register locally. Native props, slots and events are unchanged.

Use v-model:value. Options are {value,label}; multiple mode binds an array. showSearch/filterOption control local search; remote fetch needs loading, empty, error and late-response protection.

## Props and defaults



## Events

No events listed in the pinned upstream metadata.

## Slots

Use the native default slot where supported; no named slots listed in the pinned metadata.

## Executable example

See `runtime/examples.js` for setup and registrations. This composition uses the native parent/child contract.

```html
<ASelect v-model:value="choice" :options="options" allow-clear show-search style="width:220px"/><ASelect v-model:value="selection" mode="multiple" :options="options" style="min-width:220px"/><p>{{ choice }} {{ selection }}</p>
```

Source: pinned upstream es/components.js, es/select, vetur/web-types.json. Acceptance scope: [runtime guide](../usage.md).
