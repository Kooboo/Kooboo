# AMentions

Ant Design Vue 4.2.6; native export `Mentions`. Import `AMentions` from `/antdv.js` and register locally. Native props, slots and events are unchanged.

Shared usage: [runtime guide](../usage.md).

## Props and defaults



## Events

No events listed in the pinned upstream metadata.

## Slots

Use the native default slot where supported; no named slots listed in the pinned metadata.

## Executable example

See `runtime/examples.js` for setup and registrations. This composition uses the native parent/child contract.

```html
<AMentions v-model:value="text" :options="options" placeholder="输入 @ 选择用户"/><p>{{ text }}</p>
```

Source: pinned upstream es/components.js, es/mentions, vetur/web-types.json. Acceptance scope: [runtime guide](../usage.md).
