# AStep

Ant Design Vue 4.2.6; native export `Step`. Import `AStep` from `/antdv.js` and register locally. Native props, slots and events are unchanged.

Shared usage: [runtime guide](../usage.md).

## Props and defaults



## Events

No events listed in the pinned upstream metadata.

## Slots

Use the native default slot where supported; no named slots listed in the pinned metadata.

## Executable example

See `runtime/examples.js` for setup and registrations. This composition uses the native parent/child contract.

```html
<ASteps v-model:current="step" :items="[{title:'申请'},{title:'审核'},{title:'完成'}]"/><p>步骤 {{ step + 1 }}</p>
```

Source: pinned upstream es/components.js, es/steps, vetur/web-types.json. Acceptance scope: [runtime guide](../usage.md).
